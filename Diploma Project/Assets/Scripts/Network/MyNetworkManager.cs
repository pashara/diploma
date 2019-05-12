using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class PlayerInfo
{
    public Player instance;
    public GlobalUserData info;
}


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


    [SerializeField] List<Color> colors;
    
    List<PlayerInfo> playersAll = new List<PlayerInfo>();
    public Dictionary<NetworkConnection, Player> players = new Dictionary<NetworkConnection, Player>();
    bool isHostStarted = false;

    #endregion



    #region Properties

    public static MyNetworkManager Instance
    {
        get;
        private set;
    }


    public List<Color> Colors => new List<Color>(colors);


    public List<PlayerInfo> Players
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
        Player.OnIdSeted += Player_OnIdSeted;
    }


    void OnDisable()
    {
        Player.OnPlayerCreated -= Player_OnPlayerCreated;
        Player.OnPlayerDestroy -= Player_OnPlayerDestroy;
        Player.OnIdSeted -= Player_OnIdSeted;
    }


    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var currentPlayersCount = NetworkServer.connections.Count;

        GameObject player = Instantiate(playerPrefab, startPositions[currentPlayersCount - 1].position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        Player playerInstance = player.GetComponent<Player>();
        players.Add(conn, playerInstance);
        
        
        //}
        //else
        //{
        //    conn.Disconnect();
        //}
    }



    public Color ColorByIndex(int index)
    {
        return Colors[Mathf.Clamp(Mathf.Max(0, index), 0, Colors.Count)];
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
        isHostStarted = true;
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
        isHostStarted = false;
        OnStop?.Invoke(HostType.Host);
    }


    public override void OnStopClient()
    {
        base.OnStopClient();
        OnStop?.Invoke(HostType.Client);
    }

    #endregion



    #region Event handlers

    private void Player_OnPlayerCreated(Player obj)
    {
        List<int> allowedIndexes = new List<int>();
        List<int> notAllowedIndexes = new List<int>();
        int colorIndex = -1;
        if (isHostStarted)
        {
            for (int i = 0; i < playersAll.Count; i++)
            {
                notAllowedIndexes.Add(playersAll[i].instance.ColorIndex);
            }

            for (int i = 0; i < Colors.Count; i++)
            {
                if (!notAllowedIndexes.Contains(i))
                {
                    allowedIndexes.Add(i);
                }
            }

            if (allowedIndexes.Count == 0)
            {
                colorIndex = UnityEngine.Random.Range(0, Colors.Count);
            }
            else
            {
                colorIndex = allowedIndexes[UnityEngine.Random.Range(0, allowedIndexes.Count)];
            }
        }

        obj.ColorIndex = colorIndex;

        playersAll.Add(new PlayerInfo
        {
            instance = obj
        });


        playersAll.ForEach((item) =>
        {
            item.instance.UpdateAllDataFromClient();

            if (isHostStarted)
            {
                item.instance.UpdateColorDataFromServer();
            }
        });
    }


    private void Player_OnPlayerDestroy(Player obj)
    {
        playersAll.RemoveAll((i) =>
        {
            return i.instance == obj;
        });
    }

    private void Player_OnIdSeted(Player playerInstance, int playerId)
    {
        string requestUri = $"{GlobalServerManager.Instance.PlayeInfoURI}?id={playerId}";
        GlobalServerManager.Instance.LoadDataFromUrl(requestUri, (isGood, dataString) =>
        {
            if (isGood)
            {
                var data = JsonUtility.FromJson<GlobalResponseUserData>(dataString);
                playersAll.Find((item) => item.instance == playerInstance).info = data.data;
            }
            else
            {
                Debug.Log("No internet");
            }
        });        
    }

    #endregion

}