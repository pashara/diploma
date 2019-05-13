using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class AutentifiationScreen : BaseScreen
{

    [SerializeField] Button loginButton;
    [SerializeField] InputField loginField;

    [SerializeField] Graphic blocker;
    [SerializeField] float blockerOpacity;
    [SerializeField] float animationDuration;

    bool isLoading;
    public bool IsLoading
    {
        get
        {
            return isLoading;
        }
        set
        {
            if (isLoading != value)
            {
                DOTween.Kill(blocker);
                isLoading = value;
                Color fromColor = blocker.color;
                Color toColor = blocker.color;
                if (isLoading)
                {
                    blocker.gameObject.SetActive(true);
                    fromColor.a = 0f;
                    toColor.a = blockerOpacity;
                }
                else
                {
                    fromColor.a = blockerOpacity;
                    toColor.a = 0f;
                }
                blocker.color = fromColor;

                DOTween.To(() => fromColor, (x) => blocker.color = x, toColor, 1f).OnComplete(() =>
                {
                    if (!isLoading)
                    {
                        blocker.gameObject.SetActive(false);
                    }
                });
            }
        }
    }


    void OnEnable()
    {
        IsLoading = false;
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
        IsLoading = true;
        string requestUri = $"{GlobalServerManager.Instance.AuthURI}?user_id={loginField.text}";
        GlobalServerManager.Instance.LoadDataFromUrl(requestUri, (isGood, dataString) =>
        {
            if (isGood)
            {
                var data = JsonUtility.FromJson<GlobalResponseUserData>(dataString);
                GameManager.Instance.UserData = data.data;

                if (data.status.Equals(GlobalServerManager.SuccessDataAnswer))
                {
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
                    Debug.Log("NotCorrect login");
                }
            }
            else
            {
                Debug.Log("No internet");
            }
            IsLoading = false;
        });
    }

}
