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
    [SerializeField] NetworkManager networkManager;

    [SerializeField] List<InitializableMonobehaviour> managers;

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


    private void Awake()
    {
        Instance = this;

        managers.ForEach((item) =>
        {
            item.Initialize();
        });


        GuiManager.Instance.ShowScreen(ScreenType.AutentificationScreen);
    }
}
