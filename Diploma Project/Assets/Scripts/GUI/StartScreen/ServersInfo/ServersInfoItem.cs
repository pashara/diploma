using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using static StartScreenItems.ServersInfo;

namespace StartScreenItems
{
    public class ServersInfoItem : MonoBehaviour, IPointerClickHandler
    {

        public static event Action<ServersInfoItem> OnItemSelected; 

        [SerializeField] Text serverName;
        [SerializeField] Text serverAddress;
        [SerializeField] Image defaultBackground;
        [SerializeField] Image selectedBackground;


        bool isSelected;


        public GlobalResponseServerInfo info;

        public string ServerName
        {
            get
            {
                return serverName.text;
            }
            set
            {
                serverName.text = value;
            }
        }

        public string ServerAddress
        {
            get
            {
                return serverAddress.text;
            }
            set
            {
                serverAddress.text = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                if (IsSelected != value)
                {
                    isSelected = value;
                    defaultBackground.gameObject.SetActive(!value);
                    selectedBackground.gameObject.SetActive(value);
                }
            }
        }


        public bool IsCurrent(IdentificationInfo selectedItemIdentificator)
        {
            return selectedItemIdentificator.ipAddress.Equals(info.ip_address) && selectedItemIdentificator.port == info.port;
        }


        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnItemSelected?.Invoke(this);
        }
    }
}