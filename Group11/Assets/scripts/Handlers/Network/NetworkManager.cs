using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManager
{
    private const int Port = 4242;
    private static GameServer _server;
    private static GameClient _client;

    public static void Start(string serverHost)
    {
        if (_client != null) return;
        if (serverHost == null)
        {
            serverHost = Dns.GetHostName();
            _server = new(serverHost, Port);
            _server.Run();
        }

        _client = new GameClient(serverHost, Port);
        _client.Run();
    }

    public static IPAddress GetIpAddress(string host)
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(host);
        foreach (var ip in ipHostInfo.AddressList)
        {
            if (ip.AddressFamily ==
                AddressFamily.InterNetwork)
            {
                return ip;
            }
        }

        return null;
    }

    public static void SendMove(string target, Vector2 moveInput)
    {
        Dictionary<string, string> message = new();
        message.Add("type", "move");
        message.Add("target", target);
        message.Add("x", moveInput.x.ToString());
        message.Add("y", moveInput.y.ToString());
        Send(message);
    }

    private static void Send(Dictionary<string, string> message)
    {
        _client?.Send(JsonConvert.SerializeObject(message));
    }

    public static void SendPlayerInfo(string name)
    {
        Dictionary<string, string> message = new();
        message.Add("type", "playerInfo");
        message.Add("name", name);
        Send(message);
    }
}