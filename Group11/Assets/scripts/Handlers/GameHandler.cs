using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Objects;
using TMPro;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private static GameHandler _instance;

    private static readonly Vector2 DefaultSpawnPoint = new(-2, 1);
    [SerializeField] private float _roundtime = 120f;
    private float _time_limit;
    [SerializeField] private GameObject _task_pref;
    private List<NodeObject> _tasks;
    private ProgressBar _pg;
    private AudioSource _as;
    public int CurrentScore { get; set; }

    public int MaxScore {get; set;}
    public GameObject character;
    public static Player Player;

    private readonly Dictionary<string, Character> _players = new();


    public static GameHandler Instance => _instance;

    public static string PlayerName;
    public static string Host;

    private void OnDestroy()
    {
        _as.Stop(); // Not safe yet
    }

    public GameObject GetClosestTask(Vector2 origin)
    {
        // Sort based on distance using Linq
        this._tasks = _tasks.OrderByDescending(t => Vector2.Distance(t.transform.position, origin)).ToList();
        return _tasks[0].gameObject;
    }


    private void Update()
    {
        _roundtime -= Time.deltaTime;
        _pg._current = this._roundtime;
        if (_roundtime / _time_limit < 0.1f && !_as.isPlaying)
        {
            // _as.Play();
            
        }
        else
        {
            _as.Stop();
        }
        

    }

    private void initPlayerNameAndHost()
    {
        if (PlayerName == null)
            PlayerName = GetCommandArgs("player", "player");
        if (Host == null)
            Host = GetCommandArgs("host", "");
    }

    public void Start()
    {
        _time_limit = _roundtime;
        // txp = GetComponentInChildren<TextMeshPro>();
        // txp.text = "placeholder";
        _as = GetComponent<AudioSource>();
        _pg = GetComponentInChildren<ProgressBar>();
        _pg._max = this._roundtime;

        _tasks = new List<NodeObject>(GetComponentsInChildren<NodeObject>());
        _instance = this;
        MaxScore = _tasks.Count;
        // attaching delegate to each node to the score
        _tasks.ForEach(n => n.StatusChange += delegate(bool b) { this.CurrentScore += b ? 1 : -1; });
        initPlayerNameAndHost();
        Debug.Log("Player name: " + PlayerName);
        Debug.Log("Host name: " + Host);
        NetworkManager.Start(Host);
        InvokeRepeating("SendPlayerInfo", 1f, 1f);
        InvokeRepeating("SendPlayerPos", 1f, 1f);
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

    public void EnqueueMovement(Dictionary<string, string> message)
    {
        var target = message.GetValueOrDefault("target", "");
        var x = float.Parse(message.GetValueOrDefault("x", "0"));
        var y = float.Parse(message.GetValueOrDefault("y", "0"));
        var v = new Vector2(x, y);

        Character character;
        if (_players.TryGetValue(target, out character))
        {
            character.Move(v);
        }
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
            _players.Add(name, c);
            SendPlayerInfo();
            SendPlayerPos();
        }
    }

    public void NotifyPos(Vector2 pos)
    {
        NetworkManager.SendMove(PlayerName, pos);
    }

    private void SendPlayerInfo()
    {
        Dictionary<string, string> message = new();
        message.Add("type", "playerInfo");
        message.Add("name", PlayerName);
        NetworkManager.Send(message);
    }

    private void SendPlayerPos()
    {
        NotifyPos(Player.GetPos());
    }
}