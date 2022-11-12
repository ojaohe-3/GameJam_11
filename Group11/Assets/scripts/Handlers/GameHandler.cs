using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Objects;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler _instance;

    private static readonly Vector2 DefaultSpawnPoint = new(-2, 1);
    [SerializeField] private float _roundtime = 120f;
    private List<NodeObject> _tasks;
    private ProgressBar _pg;

    public GameObject character;

    public static GameHandler Instance => _instance;

    public static readonly string PlayerName = GetCommandArgs("player", "player");
    private readonly Dictionary<string, Character> _players = new();

    public static readonly Dictionary<string, ConcurrentQueue<Vector2>> MovementQueues = new();

    private void Update()
    {
        this._roundtime -= Time.deltaTime;
        _pg._current = this._roundtime;

    }

    public void Start()
    {
        _pg = GetComponentsInChildren<ProgressBar>()[0];
        _pg._max = this._roundtime;

        _tasks = new List<NodeObject>(GetComponentsInChildren<NodeObject>());
        _instance = this;
        var host = GetCommandArgs("host", null);
        Debug.Log("Player name: " + PlayerName);
        Debug.Log("Host name: " + host);
        NetworkManager.Start(host);
        InvokeRepeating("SendPlayerInfo", 0f, 1f);
    }

    private static string GetCommandArgs(string name, string defaultValue)
    {
        var args = System.Environment.GetCommandLineArgs();
        bool next = false;
        foreach (string s in args)
        {
            if (next)
                return s;
            next = s.Equals("--" + name);
        }
        return defaultValue;
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

    public void HandleMessage(string message, GameClient source)
    {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        switch (dict.GetValueOrDefault("type", ""))
        {
            case "playerInfo":
                RegisterCharacter(dict.GetValueOrDefault("name", null), source);
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

    private void RegisterCharacter(string name, GameClient source)
    {
        if (name != null && !name.Equals(PlayerName) && !_players.ContainsKey(name))
        {
            Debug.Log("create character " + name);
            var o = Instantiate(character, DefaultSpawnPoint, Quaternion.identity);
            var c = o.GetComponent<Character>();
            c.Name = name;
            foreach (var p in _players.Values)
            {
                Dictionary<string, string> msg = new();
                msg.Add("type", "playerInfo");
                msg.Add("name", p.name);
                source.Send(JsonConvert.SerializeObject(msg));
            }
            _players.Add(name, c);
        }
    }

    public void NotifyMove(Vector2 moveInput)
    {
        NetworkManager.SendMove(PlayerName, moveInput);
    }

    private void SendPlayerInfo()
    {
        Dictionary<string, string> message = new();
        message.Add("type", "playerInfo");
        message.Add("name", PlayerName);
        NetworkManager.Send(message);
    }
}