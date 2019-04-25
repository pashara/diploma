using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ScreenType
{
    None = 0,
    MainMenu = 1,
    GameScreen = 2,
    AutentificationScreen = 3,
}


public class GuiManager : InitializableMonobehaviour
{

    [SerializeField] List<BaseScreen> screensOnScene;

    public static GuiManager Instance {
        get;
        private set;
    }

    public override void Initialize ()
    {
        Instance = this;
        screensOnScene.ForEach ((item) => {
            item.gameObject.SetActive (false);
        });
    }

    
    public void ShowScreen (ScreenType screenType, bool isImmediately = false, Action<BaseScreen> onStartShow = null)
    {
        screensOnScene.ForEach ((item) => {
            if (item.ScreenType == screenType)
            {
                item.gameObject.SetActive(true);
                item.Show(onStartShow);
            }
        });
    }


    public void HideScreen (ScreenType screenType, bool isImmediately = false, Action<BaseScreen> onHided = null)
    {
        onHided += (item) =>
        {
            item.gameObject.SetActive(false);
        };

        screensOnScene.ForEach((item) => {
            if (item.ScreenType == screenType)
            {
                item.Hide(onHided);
            }
        });
    }
}