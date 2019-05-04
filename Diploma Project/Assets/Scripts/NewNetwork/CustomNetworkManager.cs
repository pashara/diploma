using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CustomNetworkManager : Photon.MonoBehaviour
{


    public static event Action<MyNetworkManager.HostType> OnStart;
    public static event Action<MyNetworkManager.HostType> OnStop;


    #region Properties

    public static CustomNetworkManager Instance
    {
        get;
        private set;
    }

    #endregion



    #region Public methods

    public void Initialize()
    {
        Instance = this;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.sendRate = 30;
        PhotonNetwork.sendRateOnSerialize = 20;
        PhotonNetwork.ConnectUsingSettings(GameManager.Instance.GameVersion);
    }


    public void JoinLobby(string lobbyName)
    {
        TypedLobby typedLobby = TypedLobby.Default;// new TypedLobby(lobbyName, LobbyType.Default);
        PhotonNetwork.JoinLobby(typedLobby);
    }

    #endregion


    #region Private methods



    void OnJoinedLobby()
    {
        string roomName = "testRoom";
        RoomOptions roomOptions = new RoomOptions();
        TypedLobby typedLobby = TypedLobby.Default;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby);
    }


    void OnJoinedRoom()
    {
        Player createdPlayer = PhotonNetwork.Instantiate("Player", new Vector3(15.5f, 24.28f, 0f), Quaternion.identity, 0).GetComponent<Player>();
        OnStart?.Invoke(MyNetworkManager.HostType.Client);
    }

    #endregion
}
