using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameScreenItems
{
    public class PlayersList : MonoBehaviour
    {
        [SerializeField] Text textLabel;


        public void CustomUpdate(float deltaTime)
        {
            List<Player> players = MyNetworkManager.Instance.Players;

            string result = "";

            players.ForEach((item) =>
            {
                result += $"{item.playerID}: {item.playerPoints}\n";
            });
            textLabel.text = result;
        }
    }
}