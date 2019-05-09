using UnityEngine;
using UnityEngine.UI;


namespace StartScreenItems
{
    public class Header : MonoBehaviour
    {
        [SerializeField] Text titleLabel;

        public string TitleText
        {
            get
            {
                return titleLabel.text;
            }
            set
            {
                titleLabel.text = value;
            }
        }
    }
}
