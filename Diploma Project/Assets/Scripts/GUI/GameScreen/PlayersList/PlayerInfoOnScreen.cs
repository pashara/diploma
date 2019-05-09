using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreenItems
{
    public class PlayerInfoOnScreen : MonoBehaviour
    {
        [SerializeField] LoadableImage avatar;
        [SerializeField] Text playerNameLabel;
        [SerializeField] Text pointsLabel;

        [SerializeField] Image borderImage;
        [SerializeField] Image backgroundImage;

        public LoadedTexture LoadedAvatarTexture
        {
            get;
        } = new LoadedTexture();


        string playerName;
        int playerPoints;
        Sprite avatarSprite = null;
    

        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                playerName = value;
                playerNameLabel.text = PlayerName;
            }
        }


        public int PlayerPoints
        {
            get
            {
                return playerPoints;
            }
            set
            {
                playerPoints = value;
                pointsLabel.text = PlayerPoints.ToString();
            }
        }


        public Sprite AvatarSprite
        {
            get
            {
                return avatarSprite;
            }
            set
            {
                avatarSprite = value;
                avatar.OriginalImage.sprite = AvatarSprite;
                avatar.IsSeted = true;
            }
        }


        public LoadableImage AvatarImage => avatar;




        public void Initialize()
        {
            PlayerName = string.Empty;
            PlayerPoints = 0;
            AvatarImage.Initialize();
        }


        public void Deinitialize()
        {

        }


    }
}