using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public enum GameState
{
    None = 0,
    Login = 1,
    MainMenu = 2,
    InGame = 3,
}

public class GameManager : MonoBehaviour
{
    #region Fields

    [SerializeField] NetworkManager networkManager;
    [SerializeField] List<InitializableMonobehaviour> managers;

    #endregion



    #region Properties

    public static GameManager Instance
    {
        get;
        private set;
    }


    public NetworkManager NetworkManager
    {
        get
        {
            return networkManager;
        }
    }

    public GlobalUserData UserData
    {
        get;
        set;
    }

    #endregion



    #region Unity lifecycle

    private void Awake()
    {
        Instance = this;

        managers.ForEach((item) =>
        {
            item.Initialize();
        });


        GuiManager.Instance.ShowScreen(ScreenType.AutentificationScreen);
    }


    void OnEnable()
    {
        MyNetworkManager.OnStart += MyNetworkManager_OnStart;
        MyNetworkManager.OnStop += MyNetworkManager_OnStop;
    }


    void OnDisable()
    {
        MyNetworkManager.OnStart -= MyNetworkManager_OnStart;
        MyNetworkManager.OnStop -= MyNetworkManager_OnStop;
    }

    #endregion



    #region Event handlers

    private void MyNetworkManager_OnStop(MyNetworkManager.HostType obj)
    {
        if (obj == MyNetworkManager.HostType.Client)
        {
            GuiManager.Instance.HideScreen(ScreenType.GameScreen, true, (screen) =>
            {
                GuiManager.Instance.ShowScreen(ScreenType.MainMenu, true, (menuScreen) =>
                {
                    (menuScreen as StartScreen).Initialize(GameManager.Instance.UserData);
                });
            });
        }
    }

    private void MyNetworkManager_OnStart(MyNetworkManager.HostType obj)
    {
        if (obj == MyNetworkManager.HostType.Client)
        {
            GuiManager.Instance.HideScreen(ScreenType.MainMenu, true, (screen) =>
            {
                GuiManager.Instance.ShowScreen(ScreenType.GameScreen, true, (gameScreen) =>
                {
                    (gameScreen as GameScreen).Initialize();
                });
            });
        }
    }

    #endregion
}
