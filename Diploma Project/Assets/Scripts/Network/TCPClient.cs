using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class TCPClient
{
    
    public TcpClient Client
    {
        get;
        private set;
    }
    public int IpPort
    {
        get;
        set;
    }
    Thread receiveThread;

    public string IpAddress
    {
        get;
        set;
    }


    public TCPClient()
    {

    }


    public void Start()
    {
        receiveThread = new Thread(new ThreadStart(Fuck));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }


    public void Stop()
    {
        receiveThread.Abort();
        Client.GetStream().Close();
        Client.Close();
    }


    void Fuck()
    {
        try
        {
            Client = new TcpClient();
            Client.Connect(IpAddress, IpPort);
            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();
            NetworkStream stream = Client.GetStream();
            bool isDataRecived = false;
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

                if(isDataRecived)
                {
                    Debug.Log("Get data from Server: " + response.ToString());
                    response.Clear();
                }
            } while (true);

        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: {0}", e.Message);
        }
    }


    
    public void SendDataToServer(string data)
    {
        byte[] dataByte = Encoding.UTF8.GetBytes(data);
        Client.GetStream().Write(dataByte, 0, dataByte.Length);
    }
}

