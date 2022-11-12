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
    public static readonly Queue<string> Queue = new();

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

    public static void SendMove(Vector2 moveInput)
    {
        Dictionary<string, string> message = new();
        message.Add("type", "move");
        message.Add("x", moveInput.x.ToString());
        message.Add("y", moveInput.y.ToString());
        _client?.Send(JsonConvert.SerializeObject(message));
    }
}

public class GameClient
{
    private readonly IPAddress _serverIp;
    private readonly int _serverPort;
    private TcpClient _client;
    private StreamWriter _writer;
    private StreamReader _reader;

    public GameClient(string host, int port)
    {
        this._serverIp = NetworkManager.GetIpAddress(host);
        this._serverPort = port;
    }

    public async Task Connect()
    {
        if (_serverIp == null)
            throw new Exception("No IPv4 found for server");
        _client = new TcpClient();
        await _client.ConnectAsync(_serverIp, _serverPort);
        Debug.Log("Client connected to " + _serverIp);
        NetworkStream networkStream = _client.GetStream();
        _writer = new StreamWriter(networkStream);
        _reader = new StreamReader(networkStream);
        _writer.AutoFlush = true;
    }

    public async Task Send(string message)
    {
        if (_writer != null)
        {
            await _writer.WriteLineAsync(message);
            Debug.Log("Message sent to server");
        }
    }

    public async Task Run()
    {
        await Connect();
        while (true)
        {
            var response = await _reader.ReadLineAsync();
            if (response != null)
            {
                Debug.Log("Message from server added to queue: " + response);
                NetworkManager.Queue.Enqueue(response);
            }
            else
                break;
        }
    }
}

public class ClientHandler
{
    private readonly TcpClient _conn;
    private readonly GameServer _server;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;


    public ClientHandler(TcpClient conn, GameServer server)
    {
        _conn = conn;
        _server = server;
        var networkStream = conn.GetStream();
        _reader = new StreamReader(networkStream);
        _writer = new StreamWriter(networkStream);
    }

    public async void Send(string message)
    {
        Debug.Log("Sending to " + _conn.Client.RemoteEndPoint + ": " + message);
        await _writer.WriteLineAsync(message);
    }

    public async Task Run()
    {
        try {
            _writer.AutoFlush = true;
            while (true) {
                string request = await _reader.ReadLineAsync();
                if (request != null) {
                    Debug.Log("Received message from " + _conn.Client.RemoteEndPoint + ": " + request);
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
        }
    }
}

public class GameServer
{
    private readonly IPAddress _ipAddress;
    private readonly List<ClientHandler> clients = new();
    private readonly int _port;

    public GameServer(string host, int port)
    {
        this._port = port;
        this._ipAddress = NetworkManager.GetIpAddress(host);
        if (this._ipAddress == null)
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
                var client = new ClientHandler(tcpClient, this);
                clients.Add(client);
                client.Run();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void Process(string request)
    {
        foreach (var client in clients)
        {
            client.Send(request);
        }
    }

    public void Disconnect(ClientHandler clientHandler)
    {
        clients.Remove(clientHandler);
    }
}