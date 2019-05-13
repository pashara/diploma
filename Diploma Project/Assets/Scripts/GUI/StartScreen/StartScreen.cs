using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using StartScreenItems;
using System;

public class StartScreen : BaseScreen
{
    #region Fields

    [SerializeField] UserInfo userInfo;
    [SerializeField] ServersInfo serversInfo;
    [SerializeField] Button startServerButton;
    [SerializeField] Button connectServerButton;
    [SerializeField] Button signOutButton;
    [SerializeField] InputField serverPortInput;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        startServerButton.onClick.AddListener(() => StartServerButton_OnClick());
        connectServerButton.onClick.AddListener(() => ConnectButton_OnClick());
        signOutButton.onClick.AddListener(() => SignOutButton_OnClick());
    }


    void OnDisable()
    {
        startServerButton.onClick.RemoveAllListeners();
        connectServerButton.onClick.RemoveAllListeners();
        signOutButton.onClick.RemoveAllListeners();
    }

    #endregion



    #region Public methods

    public override void Hide(Action<BaseScreen> onHided)
    {
        onHided += (screen) =>
        {
            (screen as StartScreen).Deinitialize();
        };

        base.Hide(onHided);
    }


    public void Initialize(GlobalUserData data)
    {
        serverPortInput.text = "7777";
        userInfo.Initialize(data);
    }


    public void Deinitialize()
    {
        userInfo.Deinitialize();
    }

    #endregion



    #region Event handlers
    
    void StartServerButton_OnClick()
    {
        GameManager.Instance.NetworkManager.networkAddress = MyNetworkManager.LocalIPAddress;// serverAddressInput.text;
        GameManager.Instance.NetworkManager.networkPort = Int32.Parse(serverPortInput.text);
        GameManager.Instance.NetworkManager.StartHost();
    }


    void ConnectButton_OnClick()
    {
        var identificator = serversInfo.SelectedItemIdentificator;
        if (identificator != null)
        {
            GameManager.Instance.NetworkManager.networkAddress = identificator.ipAddress;
            GameManager.Instance.NetworkManager.networkPort = identificator.port;
            GameManager.Instance.NetworkManager.StartClient();
        }
    }


    void SignOutButton_OnClick()
    {
        GuiManager.Instance.HideScreen(ScreenType.MainMenu, true, (menuScreen) =>
        {
            (menuScreen as StartScreen).Deinitialize();
            GameManager.Instance.UserData = null;
            GuiManager.Instance.ShowScreen(ScreenType.AutentificationScreen, true, (loginScreen) =>
            {
            });
        });
    }

    #endregion

}