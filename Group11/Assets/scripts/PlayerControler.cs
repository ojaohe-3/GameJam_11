using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    private TcpListener tcpListener;
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;

    private int port = 1337;
    private String host = "127.0.0.1";

    // Start is called before the first frame update
    void Start()
    {
        // Start TcpServer background thread
        tcpListenerThread = new Thread (new ThreadStart(ListenForIncomingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }

    private void ListenForIncomingRequests() {
        try {
            // Create listener on localhost port 8052.
            tcpListener = new TcpListener(IPAddress.Parse(host), port);
            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            while (true) {
                using (connectedTcpClient = tcpListener.AcceptTcpClient()) {
                    // Get a stream object for reading
                    using (NetworkStream stream = connectedTcpClient.GetStream()) {
                        int length;
                        // Read incoming stream into byte arrary.
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // Convert byte array to string message.
                            string clientMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("client message received as: " + clientMessage);

                        }
                    }
                }
            }
        }
        catch (SocketException socketException) {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private float moveSpeed = 10f;

    void move()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector2.up * (moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector2.down * (moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector2.left * (moveSpeed * Time.deltaTime));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector2.right * (moveSpeed * Time.deltaTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }
}