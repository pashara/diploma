using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : BaseScreen
{
    [SerializeField] Text playerNameLabel;
    [SerializeField] Button disconnectButton;


    private void OnEnable()
    {
    }


    public void Initialize()
    {
        playerNameLabel.text = GameManager.Instance.UserData.user_name;
    }
}
