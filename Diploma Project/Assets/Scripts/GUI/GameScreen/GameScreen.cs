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
    [SerializeField] Timer timer;

    [SerializeField] List<GameObject> mobileControls;

    private void OnEnable()
    {
        Player.OnTimerChanged += Player_OnTimerChanged;
        bool isEnabledControlsOnDisplay = InputAdapter.IsMobileDevice;
        timer.Seconds = 0;
        mobileControls.ForEach((item) => item.SetActive(isEnabledControlsOnDisplay));
    }

    private void OnDisable()
    {
        Player.OnTimerChanged -= Player_OnTimerChanged;
    }

    private void Player_OnTimerChanged(int obj)
    {
        timer.Seconds = obj;
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
