using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Objects;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler _instance;
    private static readonly object padlock = new();
    public GameObject character;
    public static GameHandler Instance
    {
        get
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    var o = new GameObject();
                    _instance = o.AddComponent<GameHandler>();
                }
                return _instance;
            }
        }
    }

    public string playerName = "player";
    private List<Character> players = new();

    public static readonly Dictionary<string, ConcurrentQueue<Vector2>> MovementQueues = new();

    public void Start()
    {
        _instance = this;
        NetworkManager.Start(null);
        NetworkManager.SendPlayerInfo(playerName);
    }

    public static void EnqueueMovement(Dictionary<string, string> message)
    {
        var target = message.GetValueOrDefault("target", "");
        var x = float.Parse(message.GetValueOrDefault("x", "0"));
        var y = float.Parse(message.GetValueOrDefault("y", "0"));
        var v = new Vector2(x, y);

        ConcurrentQueue<Vector2> queue;
        lock (MovementQueues)
        {
            if (!MovementQueues.TryGetValue(target, out queue))
            {
                queue = new();
                MovementQueues.Add(target, queue);
            }
        }

        queue.Enqueue(v);
    }

    public void HandleMessage(string message)
    {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        switch (dict.GetValueOrDefault("type", ""))
        {
            case "playerInfo":
                RegisterPlayer(dict);
                break;
            case "move":
                Debug.Log("Movement message: " + message);
                EnqueueMovement(dict);
                break;
            default:
                Debug.Log("Unknown message: " + message);
                break;
        }
    }

    private void RegisterPlayer(Dictionary<string,string>? message)
    {
        var name = message.GetValueOrDefault("name", "");
        if (!name.Equals(playerName))
        {
            Debug.Log("create character " + name);
            Instantiate(character, Vector2.zero, Quaternion.identity);
            var c = character.GetComponent<Character>();
            c.Name = name;
            players.Add(c);
        }
    }

    public void NotifyMove(Vector2 moveInput)
    {
        NetworkManager.SendMove(playerName, moveInput);
    }
}