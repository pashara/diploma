using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameScreenItems
{
    public class MiniMapBaseItem : MonoBehaviour
    {
        [SerializeField] Image infoImage;
        public Player playerInstance;

        public Color Color
        {
            set
            {
                infoImage.color = value;
            }
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
