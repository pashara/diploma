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
        string requestUri = $"{GlobalServerManager.Instance.AuthURI}?id={loginField.text}";
        GlobalServerManager.Instance.LoadDataFromUrl(requestUri, (isGood, dataString) =>
        {
            if (isGood)
            {
                var data = JsonUtility.FromJson<GlobalResponseUserData>(dataString);
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
                Debug.Log("No internet");
            }
        });
    }

}
