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
            Debug.Log("Client sent message to server: " + message);
        }
    }

    public async Task Run()
    {
        await Connect();
        while (true)
        {
            var message = await _reader.ReadLineAsync();
            if (message != null)
            {
                Debug.Log("Client got message from server: " + message);
                GameHandler.Instance.HandleMessage(message);
            }
            else
                break;
        }
    }
}