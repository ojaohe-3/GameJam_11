using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameHandler
{
    private static GameHandler instance;
    private static readonly object padlock = new();

    public static readonly Dictionary<string, ConcurrentQueue<Vector2>> MovementQueues = new();

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

    public static GameHandler Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new GameHandler();
                }
                return instance;
            }
        }
    }


    public static void HandleMessage(string message)
    {
        var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
        switch (dict.GetValueOrDefault("type", ""))
        {
            case "move":
                Debug.Log("Movement message: " + message);
                EnqueueMovement(dict);
                break;
            default:
                Debug.Log("Unknown message: " + message);
                break;
        }
    }
}