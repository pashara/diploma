using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server : MonoBehaviour
{
    #region Fields
    [SerializeField] ServerUI serverUI;

    bool isConnected = false;

    Thread receiveThread;
    UdpClient client;
    string lastReceivedUDPPacket = "";
    string allReceivedUDPPackets = "";
    string lastAddedDataInfo = "";


    #endregion



    #region Properties

    bool IsConnected
    {
        get
        {
            return isConnected;
        }
        set
        {
            isConnected = value;
            serverUI.MainButtonText = ((value) ? "Stop Server" : "Start Server");
        }
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        serverUI.OnSendButtonPressed += ServerUI_OnSendButtonPressed;
        serverUI.OnClearButtonPressed += ServerUI_OnClearButtonPressed;
        serverUI.OnMainButtonPressed += ServerUI_OnMainButtonPressed;
        IsConnected = false;
        serverUI.PortAddress = 8080;
    }

    void OnDisable()
    {
        serverUI.OnSendButtonPressed -= ServerUI_OnSendButtonPressed;
        serverUI.OnClearButtonPressed -= ServerUI_OnClearButtonPressed;
        serverUI.OnMainButtonPressed -= ServerUI_OnMainButtonPressed;
    }

    void Update()
    {
        if (!lastAddedDataInfo.Equals(""))
        {
            serverUI.AddToOutput(lastAddedDataInfo);
            lastAddedDataInfo = string.Empty;
        }
    }

    #endregion



    #region Event handlers

    void ServerUI_OnSendButtonPressed(ServerUI sender)
    {
        Debug.Log("Try send server " + sender.InputText);
        if (IsConnected)
        {
            sender.ClearInput();
        }
    }


    void ServerUI_OnClearButtonPressed(ServerUI sender)
    {

    }


    void ServerUI_OnMainButtonPressed(ServerUI sender)
    {

        if (!IsConnected)
        {
            receiveThread = new Thread(
                new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            IsConnected = !IsConnected;
        }
        else
        {
            // IsConnected = !IsConnected;
        }
    }



    #endregion



























    private void ReceiveData()
    {
        client = new UdpClient(serverUI.PortAddress);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);
                //Debug.Log(text);
                lastAddedDataInfo += text;
                // serverUI.AddToOutput(text);

                lastReceivedUDPPacket = text;

                allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                Debug.LogError(err.ToString());
            }
        }
    }

    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}