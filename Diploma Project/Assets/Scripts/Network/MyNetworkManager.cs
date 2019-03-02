using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class MyNetworkManager : MonoBehaviour
{

    public static MyNetworkManager Instance
    {
        get;
        private set;
    }
    public bool isAtStartup = true;
    NetworkClient myClient;


    void Awake()
    {
        MyNetworkManager.Instance = this;
    }
    void Update()
    {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SetupServer();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                SetupServer();
                SetupLocalClient();
            }
        }
    }
    void OnGUI()
    {
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");
            GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");
            GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
        }
    }




    // Create a server and listen on a port
    public void SetupServer()
    {
        NetworkServer.Reset();
        NetworkServer.Listen(4444);

        NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
        NetworkServer.RegisterHandler(ZMessageType.STRING, OnServerReceiveStringMessage);
        isAtStartup = false;
    }


    void OnServerReceiveStringMessage(NetworkMessage netMsg)
    {
        var stringMsg = netMsg.ReadMessage<StringMessage>();
        Debug.Log("You string text: " + stringMsg.stringValue);
    }

    void OnClientConnected(NetworkMessage netMsg)
    {
        Debug.Log(netMsg);
        StringMessage stringMsg = new StringMessage();
        netMsg.ReadMessage<StringMessage>(stringMsg);
        // Debug.Log("You string text: " + stringMsg.stringValue);
    }









    // Create a local client and connect to the local server
    public void SetupLocalClient()
    {
        myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler(ZMessageType.STRING, ClientReceiveStringMessage);
        isAtStartup = false;
    }


    // client function
    public void OnConnected(NetworkMessage netMsg)
    {
        int a = netMsg.channelId;
        Debug.Log("Connected to server");
        SentDataFromClient(Encoding.ASCII.GetBytes("someString"));
    }
    
    void SentDataFromClient(byte[] data)
    {
        myClient.SendBytes(data, 10, 4444);
    }
    
    void ClientReceiveStringMessage(NetworkMessage netMsg)
    {
        var stringMsg = netMsg.ReadMessage<StringMessage>();
        Debug.Log("Your message is " + stringMsg.stringValue);
    }
}