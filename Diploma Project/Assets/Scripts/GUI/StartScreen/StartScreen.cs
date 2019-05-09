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
    [SerializeField] Button startServerButton;
    [SerializeField] Button connectServerButton;
    [SerializeField] InputField serverAddressInput;
    [SerializeField] InputField serverPortInput;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        startServerButton.onClick.AddListener(() => StartServerButton_OnClick());
        connectServerButton.onClick.AddListener(() => ConnectButton_OnClick());
    }


    void OnDisable()
    {
        startServerButton.onClick.RemoveAllListeners();
        connectServerButton.onClick.RemoveAllListeners();
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
        GameManager.Instance.NetworkManager.networkAddress = serverAddressInput.text;
        GameManager.Instance.NetworkManager.StartHost();
    }


    void ConnectButton_OnClick()
    {
        GameManager.Instance.NetworkManager.networkAddress = serverAddressInput.text;
        GameManager.Instance.NetworkManager.StartClient();
    }

    #endregion

}