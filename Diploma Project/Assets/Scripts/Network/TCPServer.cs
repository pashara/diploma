using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

public class TCPServer
{

    #region Helpers

    class ClientInfo
    {
        public Thread clientListnerThread;
        public TcpClient tcpClient;
    }

    #endregion



    #region Events

    public event Action<TcpClient> OnClientConnected;

    #endregion



    Thread receiveThread;
    public string IpAddress
    {
        get;
        set;
    }
    public int IpPort
    {
        get;
        set;
    }

    List<ClientInfo> clients;


    #region Public methods

    public TCPServer()
    {
        clients = new List<ClientInfo>();
    }

    public void Start()
    {
        receiveThread = new Thread(new ThreadStart(Listen));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    #endregion



    void Listen()
    {
        TcpListener server = null;
        try
        {
            IPAddress localAddr = IPAddress.Parse(IpAddress);
            server = new TcpListener(localAddr, IpPort);

            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Подключен клиент. Выполнение запроса...");

                Thread childSocketThread = new Thread(() =>
                {
                    NetworkStream stream = client.GetStream();
                    bool isDataRecived = false;
                    byte[] data = new byte[256];
                    StringBuilder response = new StringBuilder();

                    do
                    {
                        isDataRecived = false;
                        do
                        {
                            int bytes = stream.Read(data, 0, data.Length);
                            response.Append(Encoding.UTF8.GetString(data, 0, bytes));
                            isDataRecived = true;
                        }
                        while (stream.DataAvailable);

                        if (isDataRecived)
                        {
                            Debug.Log("Get data from Client: " + response.ToString());
                            Person a = Person.ReadToObject(response.ToString());
                            if (a != null)
                            {
                                Debug.Log(a);
                            }
                            response.Clear();
                        }
                    } while (true);
                });

                ClientInfo clientInfo = new ClientInfo();
                clientInfo.tcpClient = client;
                clientInfo.clientListnerThread = childSocketThread;
                childSocketThread.Start();


                if (OnClientConnected != null)
                {
                    OnClientConnected(client);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            if (server != null)
                server.Stop();
        }
    }


    public void SendData(TcpClient client, string data)
    {
        byte[] dataByte = Encoding.UTF8.GetBytes(data);
        client.GetStream().Write(dataByte, 0, dataByte.Length);
    }


    public void CloseConnection(TcpClient client)
    {
        byte[] dataByte = Encoding.UTF8.GetBytes("close");
        client.GetStream().Write(dataByte, 0, dataByte.Length);
        client.GetStream().Close();
        client.Close();
        // clients.Remove();
    }
}
