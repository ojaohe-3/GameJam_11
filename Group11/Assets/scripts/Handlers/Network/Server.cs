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