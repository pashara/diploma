using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager
{

    public List<Player> players = new List<Player>();

    public static MyNetworkManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        //base.OnServerAddPlayer(conn, playerControllerId);


        var currentPlayersCount = NetworkServer.connections.Count;

        //if (currentPlayersCount <= 2)
            //{
            GameObject player = Instantiate(playerPrefab, startPositions[currentPlayersCount - 1].position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            players.Add(player.GetComponent<Player>());
            //}
        //else
        //{
        //    conn.Disconnect();
        //}
    }



    public override void OnStartHost()
    {
        base.OnStartHost();
        ShowGameUI();
    }


    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        ShowGameUI();
    }


    public override void OnStopHost()
    {
        base.OnStopHost();
        HideGameUI();
    }


    public override void OnStopClient()
    {
        base.OnStopClient();
        HideGameUI();
    }


    void ShowGameUI()
    {
        Debug.Log("Show game UI");
        GuiManager.Instance.HideScreen(ScreenType.MainMenu, true, (screen) =>
        {
            GuiManager.Instance.ShowScreen(ScreenType.GameScreen, true, (gameScreen) =>
            {
                (gameScreen as GameScreen).Initialize();
            });
        });
    }


    void HideGameUI()
    {
        Debug.Log("HideGameUI");
        GuiManager.Instance.HideScreen(ScreenType.GameScreen, true, (screen) =>
        {
            GuiManager.Instance.ShowScreen(ScreenType.MainMenu, true, (menuScreen) =>
            {
                (menuScreen as StartScreen).Initialize(GameManager.Instance.UserData);
            });
        });
    }
}