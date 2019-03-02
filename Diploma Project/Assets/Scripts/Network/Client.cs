using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client : MonoBehaviour
{

    #region Fields

    [SerializeField] ServerUI clientUi;
    bool isConnected = false;


    IPEndPoint remoteEndPoint;
    UdpClient client;



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
            clientUi.MainButtonText = ((value) ? "Disconnect" : "Connect");
        }
    }

    #endregion



    #region Unity lifecycle

    void OnEnable()
    {
        if (clientUi != null)
        {
            Initialize(clientUi);
        }
    }

    void OnDisable()
    {
        Deinitialize();
    }

    #endregion



    #region Public methods

    public void Initialize(ServerUI uiElementh)
    {
        clientUi = uiElementh;
        clientUi.OnSendButtonPressed += ClientUI_OnSendButtonPressed;
        clientUi.OnMainButtonPressed += ClientUI_OnMainButtonPressed;
        IsConnected = false;
        clientUi.IpAddress = "127.0.0.1";
        clientUi.PortAddress = 8080;
    }


    public void Deinitialize()
    {
        if (clientUi != null)
        {
            clientUi.OnSendButtonPressed -= ClientUI_OnSendButtonPressed;
            clientUi.OnMainButtonPressed -= ClientUI_OnMainButtonPressed;
        }
    }
    #endregion


    #region Event handlers

    void ClientUI_OnSendButtonPressed(ServerUI sender)
    {
        sendString(sender.InputText);
        // Debug.Log("Try send client#" + clientId + ": " + sender.InputText);
        if (IsConnected)
        {
            sender.ClearInput();
        }
    }


    void ClientUI_OnMainButtonPressed(ServerUI sender)
    {

        if (!IsConnected)
        {
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(clientUi.IpAddress), clientUi.PortAddress);
            client = new UdpClient();
            IsConnected = !IsConnected;
        }
        else
        {
            IsConnected = !IsConnected;
        }
    }


    #endregion

















    // inputFromConsole
    private void inputFromConsole()
    {
        try
        {
            string text;
            do
            {
                text = Console.ReadLine();

                // Den Text zum Remote-Client senden.
                if (text != "")
                {

                    // Daten mit der UTF8-Kodierung in das Binärformat kodieren.
                    byte[] data = Encoding.UTF8.GetBytes(text);

                    // Den Text zum Remote-Client senden.
                    client.Send(data, data.Length, remoteEndPoint);
                }
            } while (text != "");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }

    }

    // sendData
    private void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            Debug.Log("Sedn data from " + this.GetHashCode() + " string " + message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

}