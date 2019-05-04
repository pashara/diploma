using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;



public class MyNetworkManager : NetworkManager
{
    #region Nested types

    public enum HostType
    {
        None = 0,
        Host = 1,
        Server = 2,
        Client = 3,
    }
    #endregion


    #region Fields

    public static event Action<HostType> OnStart;
    public static event Action<HostType> OnStop;
    public static event Action<Player> OnRemovePlayer;

    public Dictionary<NetworkConnection, Player> players = new Dictionary<NetworkConnection, Player>();

    #endregion



    #region Properties

    public static MyNetworkManager Instance
    {
        get;
        private set;
    }

    List<Player> playersAll = new List<Player>();
    public List<Player> Players
    {
        get
        {
            return playersAll;
        }
    }

    #endregion



    #region Unity lifecycle

    private void Awake()
    {
        Instance = this;
    }


    void OnEnable()
    {
        Player.OnPlayerCreated += Player_OnPlayerCreated;
        Player.OnPlayerDestroy += Player_OnPlayerDestroy;
    }


    void OnDisable()
    {
        Player.OnPlayerCreated += Player_OnPlayerCreated;
        Player.OnPlayerDestroy += Player_OnPlayerDestroy;
    }


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);


        var currentPlayersCount = NetworkServer.connections.Count;

        //if (currentPlayersCount <= 2)
            //{
            GameObject player = Instantiate(playerPrefab, startPositions[currentPlayersCount - 1].position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            players.Add(conn, player.GetComponent<Player>());
            //}
        //else
        //{
        //    conn.Disconnect();
        //}
    }


    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Player currentPlayer;
        if (players.TryGetValue(conn, out currentPlayer))
        {
            players.Remove(conn);
        }

        OnRemovePlayer?.Invoke(currentPlayer);
        base.OnServerRemovePlayer(conn, player);
    }


    public override void OnStartHost()
    {
        base.OnStartHost();
        OnStart?.Invoke(HostType.Host);
    }


    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        OnStart?.Invoke(HostType.Client);
    }


    public override void OnStopHost()
    {
        base.OnStopHost();
        OnStop?.Invoke(HostType.Host);
    }


    public override void OnStopClient()
    {
        base.OnStopClient();
        OnStop?.Invoke(HostType.Client);
    }

    #endregion



    #region Event handlers

    private void Player_OnPlayerDestroy(Player obj)
    {
        playersAll.Remove(obj);
    }

    private void Player_OnPlayerCreated(Player obj)
    {
        playersAll.Add(obj);
    }

    #endregion

}