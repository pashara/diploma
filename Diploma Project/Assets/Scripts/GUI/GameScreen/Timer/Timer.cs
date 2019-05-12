using UnityEngine;
using UnityEngine.UI;

namespace GameScreenItems
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] Text label;


        int seconds;

        public int Seconds
        {
            get
            {
                return seconds;
            }
            set
            {
                seconds = Mathf.Max(value, 0);
                string result = string.Empty;
                int minutes = seconds / 60;
                int clampedSeconds = seconds % 60;

                if (minutes > 0)
                {
                    result = string.Format("{0}:{1:00}", minutes, clampedSeconds);
                }
                else
                {
                    result = string.Format("{0}", clampedSeconds);
                }

                label.text = result;
            }
        }
    }
}