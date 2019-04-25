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
        GameManager.Instance.NetworkManager.networkAddress = serverAddressInput.text;
        GameManager.Instance.NetworkManager.StartHost();
    }

    void ConnectButton_OnClick()
    {
        GameManager.Instance.NetworkManager.StartClient();
    }


    public void Initialize(GlobalUserData data)
    {
        usernameLabel.text = data.user_name;
        StartCoroutine(SetImage(image, data.user_avatar));
    }


    IEnumerator SetImage(Image image, string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.canvasRenderer.GetMaterial().mainTexture = myTexture;

            Sprite s = Sprite.Create(myTexture as Texture2D, new Rect(0, 0, myTexture.width, myTexture.height),
                Vector2.zero, 1f);
            image.sprite = s;

        }
    }
}