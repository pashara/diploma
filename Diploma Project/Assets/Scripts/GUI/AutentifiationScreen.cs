using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AutentifiationScreen : BaseScreen
{

    [SerializeField] Button loginButton;
    [SerializeField] InputField loginField;

    void OnEnable()
    {

        loginButton.onClick.AddListener(() => LoginButton_OnClick());
    }


    void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
    }





    void LoginButton_OnClick()
    {
        StartAuthAction();
    }





    void StartAuthAction()
    {
        StartCoroutine(LoadDataFromUrl($"http://serw/site/user-info?id={loginField.text}"));
    }


    IEnumerator LoadDataFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (!request.isHttpError && !request.isNetworkError)
        {
            var data = JsonUtility.FromJson<GlobalResponseUserData>(request.downloadHandler.text);
            GameManager.Instance.UserData = data.data;

            GuiManager.Instance.HideScreen(ScreenType.AutentificationScreen, true, (loginScreen) =>
            {

                GuiManager.Instance.ShowScreen(ScreenType.MainMenu, true, (menuScreen) =>
                {
                    (menuScreen as StartScreen).Initialize(data.data);
                });
            });
        }
        else
        {
            Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
        }
        request.Dispose();
    }


}
