using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameScreenItems;

public class GameScreen : BaseScreen
{
    [SerializeField] Text playerNameLabel;
    [SerializeField] Button disconnectButton;

    [SerializeField] MiniMap minimap;
    [SerializeField] PlayersList playersList;

    private void OnEnable()
    {
    }


    public void Initialize()
    {
        playerNameLabel.text = GameManager.Instance.UserData.user_name;
    }


    void Update()
    {
        minimap.CustomUpdate(Time.deltaTime);
        playersList.CustomUpdate(Time.deltaTime);
    }
}
