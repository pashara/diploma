using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StartScreen : BaseScreen
{
    #region Fields

    [SerializeField] Image image;
    [SerializeField] Text usernameLabel;
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


    void StartServerButton_OnClick()
    {

        CustomNetworkManager.Instance.JoinLobby("Lobbi");

        //GameManager.Instance.NetworkManager.networkAddress = serverAddressInput.text;
        //GameManager.Instance.NetworkManager.StartHost();
    }

    void ConnectButton_OnClick()
    {
        CustomNetworkManager.Instance.JoinLobby("Lobbi");
        //GameManager.Instance.NetworkManager.StartClient();
    }


    public void Initialize(GlobalUserData data)
    {
        usernameLabel.text = data.user_name;
        GlobalServerManager.Instance.LoadTexture(data.user_avatar, (isSuccess, texture) =>
        {
            image.canvasRenderer.GetMaterial().mainTexture = texture;

            Sprite s = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height),
                Vector2.zero, 1f);
            image.sprite = s;
        });
    }
    
}