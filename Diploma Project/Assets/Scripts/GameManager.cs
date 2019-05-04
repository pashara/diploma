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

    [SerializeField] string gameVersion;
    [SerializeField] CustomNetworkManager networkManager;
    [SerializeField] List<InitializableMonobehaviour> managers;

    #endregion



    #region Properties

    public static GameManager Instance
    {
        get;
        private set;
    }


    public CustomNetworkManager NetworkManager
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


    public string GameVersion
    {
        get
        {
            return gameVersion;
        }
    }

    #endregion



    #region Unity lifecycle

    private void Awake()
    {
        Instance = this;

        networkManager.Initialize();

        managers.ForEach((item) =>
        {
            item.Initialize();
        });


        GuiManager.Instance.ShowScreen(ScreenType.AutentificationScreen);
    }


    void OnEnable()
    {
        CustomNetworkManager.OnStart += MyNetworkManager_OnStart;
        CustomNetworkManager.OnStop += MyNetworkManager_OnStop;
    }


    void OnDisable()
    {
        CustomNetworkManager.OnStart -= MyNetworkManager_OnStart;
        CustomNetworkManager.OnStop -= MyNetworkManager_OnStop;
    }

    #endregion



    #region Event handlers

    private void MyNetworkManager_OnStop(MyNetworkManager.HostType obj)
    {
        GuiManager.Instance.HideScreen(ScreenType.GameScreen, true, (screen) =>
        {
            GuiManager.Instance.ShowScreen(ScreenType.MainMenu, true, (menuScreen) =>
            {
                (menuScreen as StartScreen).Initialize(GameManager.Instance.UserData);
            });
        });
    }

    private void MyNetworkManager_OnStart(MyNetworkManager.HostType obj)
    {
        GuiManager.Instance.HideScreen(ScreenType.MainMenu, true, (screen) =>
        {
            GuiManager.Instance.ShowScreen(ScreenType.GameScreen, true, (gameScreen) =>
            {
                (gameScreen as GameScreen).Initialize();
            });
        });
    }

    #endregion
}
