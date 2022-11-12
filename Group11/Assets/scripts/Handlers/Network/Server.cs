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

public class GameServer
{
    private readonly IPAddress _ipAddress;
    private readonly List<ServerClientHandler> clients = new();
    private readonly int _port;

    public GameServer(string host, int port)
    {
        _port = port;
        _ipAddress = NetworkManager.GetIpAddress(host);
        if (_ipAddress == null)
            throw new Exception("No IPv4 address for server");
    }

    public async Task Run()
    {
        TcpListener listener = new TcpListener(_ipAddress, _port);
        listener.Start();
        Debug.Log("Game server is now running on port " + _port);
        while (true) {
            try {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                Debug.Log("Received connection request from " + tcpClient.Client.RemoteEndPoint);
                var client = new ServerClientHandler(tcpClient, this);
                clients.Add(client);
                client.Run();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Process(string message)
    {
        foreach (var client in clients)
            client.Send(message);
    }

    public void Disconnect(ServerClientHandler serverClientHandler)
    {
        Debug.Log("Disconnected client " + serverClientHandler.ClientHost());
        clients.Remove(serverClientHandler);
    }
}

public class ServerClientHandler
{
    private readonly TcpClient _conn;
    private readonly GameServer _server;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;

    public ServerClientHandler(TcpClient conn, GameServer server)
    {
        _conn = conn;
        _server = server;
        var networkStream = conn.GetStream();
        _reader = new StreamReader(networkStream);
        _writer = new StreamWriter(networkStream);
    }

    public async void Send(string message)
    {
        Debug.Log("Server sending message to " + ClientHost() + ": " + message);
        await _writer.WriteLineAsync(message);
    }

    public EndPoint ClientHost()
    {
        return _conn.Client.RemoteEndPoint;
    }

    public async Task Run()
    {
        try {
            _writer.AutoFlush = true;
            while (true) {
                string request = await _reader.ReadLineAsync();
                if (request != null) {
                    Debug.Log("Server received message from " + ClientHost() + ": " + request);
                    _server.Process(request);
                }
                else
                {
                    _server.Disconnect(this);
                    break; // Client closed connection
                }
            }
            _conn.Close();
        }
        catch (Exception ex) {
            Debug.Log(ex.Message);
            if (_conn.Connected)
                _conn.Close();
            _server.Disconnect(this);
        }
    }
}