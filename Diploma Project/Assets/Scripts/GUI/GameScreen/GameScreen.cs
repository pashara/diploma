using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScreenItems;

public class GameScreen : BaseScreen
{
    [SerializeField] Button disconnectButton;

    [SerializeField] MiniMap minimap;
    [SerializeField] PlayersList playersList;

    [SerializeField] List<GameObject> mobileControls;

    private void OnEnable()
    {
        bool isEnabledControlsOnDisplay = InputAdapter.IsMobileDevice;
        mobileControls.ForEach((item) => item.SetActive(isEnabledControlsOnDisplay));
    }


    public void Initialize()
    {
        //playerNameLabel.text = GameManager.Instance.UserData.user_name;
    }


    void Update()
    {
        minimap.CustomUpdate(Time.deltaTime);
        playersList.CustomUpdate(Time.deltaTime);
    }
}
