using UnityEngine;
using UnityEngine.UI;


namespace StartScreenItems
{
    public class DataRow : MonoBehaviour
    {
        [SerializeField] Text titleLabel;
        [SerializeField] Text valueLabel;

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

        public string ValueText
        {
            get
            {
                return valueLabel.text;
            }
            set
            {
                valueLabel.text = value;
            }
        }
    }
}