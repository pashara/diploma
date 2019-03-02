using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;


[DataContract]
internal class Person
{
    [DataMember]
    internal string name;

    [DataMember]
    internal int age;

    public override string ToString()
    {
        return name + "_" + age;
    }


    public static Person ReadToObject(string json)
    {
        Person deserializedUser = new Person();
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
        try
        {
            deserializedUser = ser.ReadObject(ms) as Person;
        }
        catch(SerializationException e)
        {
            deserializedUser = null;
            // Debug.LogError(e.StackTrace);
        }
        ms.Close();
        return deserializedUser;
    }
}


public class TCPController : MonoBehaviour
{
    List<TCPClient> tcpClients = new List<TCPClient>();
    TCPServer tcpServer = new TCPServer();
    TcpClient client;
    bool isSmth = false;
    float timer = 0f;


    void Awake()
    {
        tcpServer.IpAddress = "127.0.0.1";
        tcpServer.IpPort = 8888;
        tcpServer.Start();
        tcpServer.OnClientConnected += TcpServer_OnClientConnected;

        for (int i = 0; i < 3; i++)
        {
            TCPClient tCPClient = new TCPClient();
            tCPClient.IpAddress = tcpServer.IpAddress;
            tCPClient.IpPort = tcpServer.IpPort;
            tCPClient.Start();
            tcpClients.Add(tCPClient);
        }
    }

    bool isClientsSend = false;
    bool isServerSend = false;
    void Update()
    {
        if (isSmth)
        {
            timer += Time.deltaTime;

            if (timer > 3f && !isServerSend)
            {

                Person p = new Person();
                p.name = "John";
                p.age = 42;

                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Person));
                ser.WriteObject(stream1, p);
                stream1.Position = 0;
                StreamReader sr = new StreamReader(stream1);

                string sendData = sr.ReadToEnd();
                // sendData = "Epta";
                Debug.Log("Try send " + sendData);
                tcpServer.SendData(tcpClients[0].Client, sendData);
                isServerSend = true;
            }
            else if (timer > 3.1f && !isClientsSend)
            {
                for (int i = 0; i < tcpClients.Count; i++)
                {
                    tcpClients[i].SendDataToServer("11");
                }
                isClientsSend = true;
            }
        }
    }

    void TcpServer_OnClientConnected(TcpClient client)
    {
        this.client = client;
        tcpServer.SendData(client, "Hello, pidor!\r\n");
        isSmth = true;
    }

}
