using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameScreenItems
{
    public class KillItem : MonoBehaviour
    {
        #region Fields

        [SerializeField] Text killerNameLabel;
        [SerializeField] Text killerLevelLabel;
        [SerializeField] Text victimNameLabel;
        [SerializeField] Text victimLevelLabel;
        [SerializeField] Image backgroudnImage;
        [SerializeField] AnimationCurve fadeInCurve;
        [SerializeField] AnimationCurve fadeOutCurve;
        [SerializeField] float maxOpacity;

        float opacity = 1f;

        #endregion


        #region Properties

        public AnimationCurve FadeInCurve => fadeInCurve;


        public AnimationCurve FadeOutCurve => fadeOutCurve;


        public float MaxOpacity => maxOpacity;


        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                opacity = value;

                if (killerNameLabel != null)
                {
                    killerNameLabel.color = ChangeAlpha(killerNameLabel, opacity);
                }

                if (killerLevelLabel != null)
                {
                    killerLevelLabel.color = ChangeAlpha(killerLevelLabel, opacity);
                }

                victimNameLabel.color = ChangeAlpha(victimNameLabel, opacity);
                victimLevelLabel.color = ChangeAlpha(victimLevelLabel, opacity);
                backgroudnImage.color = ChangeAlpha(backgroudnImage, opacity);
            }
        }

        #endregion



        #region Public methods

        public void Initilize(Player killer, Player victim)
        {
            if (killer != null)
            {
                PlayerInfo killerInfo = MyNetworkManager.Instance.Players.Find((item) => item.instance == killer);
                if (killerNameLabel != null) killerNameLabel.text = killerInfo.info.user_name;
                if (killerLevelLabel != null) killerLevelLabel.text = killerInfo.info.level.ToString();
            }

            PlayerInfo victimInfo = MyNetworkManager.Instance.Players.Find((item) => item.instance == victim);
            victimNameLabel.text = victimInfo.info.user_name;
            victimLevelLabel.text = victimInfo.info.level.ToString();

        }

        #endregion


        #region Private methods

        Color ChangeAlpha(Graphic graphic, float alpha)
        {
            return new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
        }

        #endregion
    }
}
