using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace StartScreenItems
{
    public class ServersInfo : MonoBehaviour
    {

        public class IdentificationInfo
        {
            public string ipAddress;
            public int port;


            public IdentificationInfo(string ipAddress, int port)
            {
                this.ipAddress = ipAddress;
                this.port = port;
            }
        }


        class ServerInfoAnswer
        {
            public int createResultTime;
            public string gameAlias;
            public GlobalResponseServerInfo[] servers;

        }

        [SerializeField] ServersInfoItem itemPrefab;
        [SerializeField] Transform spawnTransfom;
        [SerializeField] int updateTime;

   

        IdentificationInfo selectedItemIdentificator = null;
        Coroutine getInfoCoroutine;

        List<ServersInfoItem> itemsOnScreen = new List<ServersInfoItem>();


        public IdentificationInfo SelectedItemIdentificator
        {
            get
            {
                return selectedItemIdentificator;
            }
        }


        private void OnEnable()
        {
            getInfoCoroutine = StartCoroutine(LoadData());
            ServersInfoItem.OnItemSelected += ServersInfoItem_OnItemSelected;
        }

        private void OnDisable()
        {
            ServersInfoItem.OnItemSelected -= ServersInfoItem_OnItemSelected;
            if (getInfoCoroutine != null)
            {
                StopCoroutine(getInfoCoroutine);
            }

            itemsOnScreen.ForEach((item) =>
            {
                Destroy(item.gameObject);
            });
            itemsOnScreen.Clear();
        }


        IEnumerator LoadData()
        {
            do
            {
                UnityWebRequest request = UnityWebRequest.Get(GlobalServerManager.Instance.GetServersInfoURI);

                yield return request.SendWebRequest();

                if (!request.isHttpError && !request.isNetworkError)
                {
                    var data = JsonUtility.FromJson<ServerInfoAnswer>(request.downloadHandler.text);
                    if (data != null)
                    {
                        bool isSelectedItemInCurrentList = false;
                        GlobalResponseServerInfo[] servers = data.servers;
                        for (int i = 0; i < servers.Length; i++)
                        {
                            bool shouldInstantiate = i > itemsOnScreen.Count - 1;
                            ServersInfoItem instance = null;
                            if (shouldInstantiate)
                            {
                                instance = Instantiate(itemPrefab, spawnTransfom);
                                itemsOnScreen.Add(instance);
                            }
                            else
                            {
                                instance = itemsOnScreen[i];
                            }

                            instance.gameObject.SetActive(true);
                            instance.info = servers[i];
                            instance.ServerName = servers[i].title;
                            instance.ServerAddress = $"{servers[i].ip_address}:{servers[i].port}";
                            instance.IsSelected = false;

                            if (selectedItemIdentificator != null && instance.IsCurrent(selectedItemIdentificator))
                            {
                                instance.IsSelected = true;
                                isSelectedItemInCurrentList = true;
                            }
                        }

                        for(int i = servers.Length; i < itemsOnScreen.Count; i++)
                        {
                            itemsOnScreen[i].gameObject.SetActive(false);
                        }


                        if (!isSelectedItemInCurrentList)
                        {
                            selectedItemIdentificator = null;
                        }
                    }
                }
                yield return new WaitForSeconds(updateTime);
            } while (true);
        }


        private void ServersInfoItem_OnItemSelected(ServersInfoItem obj)
        {
            selectedItemIdentificator = new IdentificationInfo(obj.info.ip_address, obj.info.port);

            for (int i = 0; i < itemsOnScreen.Count; i++)
            {
                itemsOnScreen[i].IsSelected = itemsOnScreen[i].IsCurrent(selectedItemIdentificator);
            }
        }
    }


}
