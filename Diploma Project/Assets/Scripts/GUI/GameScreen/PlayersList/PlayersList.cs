using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameScreenItems
{
    public class PlayersList : MonoBehaviour, ICoroutineInfo
    {

        #region Nested types

        class PlayerByUiInstanceContainer
        {

            public Player playerInstance;
            public PlayerInfoOnScreen uiInstance;

        }

        #endregion



        #region Fields

        [SerializeField] PlayerInfoOnScreen playerInfoOnScreenPrefab;
        [SerializeField] Transform spawnParent;

        List<PlayerByUiInstanceContainer> uiInstances = new List<PlayerByUiInstanceContainer>();

        #endregion


        #region ICoroutine

        public void CoroutineStop(Coroutine c)
        {
            StopCoroutine(c);
        }

        #endregion



        #region Public methods

        public void CustomUpdate(float deltaTime)
        {
            List<PlayerInfo> players = MyNetworkManager.Instance.Players;

            CheckInstances(players, uiInstances);
            for (int i = 0; i < players.Count; i++)
            {
                PlayerInfo info = players[i];
                PlayerByUiInstanceContainer uiInfo = InfoContainer(info.instance);
                UpdateData(info, uiInfo);
            }
        }

        #endregion



        #region Private methods

        PlayerByUiInstanceContainer InfoContainer(Player playerInstance)
        {
            PlayerByUiInstanceContainer uiInfo = uiInstances.Find((item) =>
            {
                return item.playerInstance == playerInstance;
            });

            if (uiInfo == null)
            {
                uiInfo = new PlayerByUiInstanceContainer();
                uiInfo.playerInstance = playerInstance;
                PlayerInfoOnScreen uiInstance = Instantiate<PlayerInfoOnScreen>(playerInfoOnScreenPrefab, spawnParent);
                uiInfo.uiInstance = uiInstance;

                uiInstance.Initialize();

                uiInstances.Add(uiInfo);
            }
            return uiInfo;
        }


        void CheckInstances(List<PlayerInfo> players, List<PlayerByUiInstanceContainer> uiPlayers)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].instance == null)
                {
                    players.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < uiPlayers.Count; i++)
            {
                if (uiPlayers[i].playerInstance == null)
                {
                    uiPlayers[i]?.uiInstance?.Deinitialize();
                    if (uiPlayers[i]?.uiInstance != null)
                    {
                        Destroy(uiPlayers[i]?.uiInstance.gameObject);
                    }
                    uiPlayers.RemoveAt(i);
                    i--;
                }
            }
        }


        void UpdateData(PlayerInfo info, PlayerByUiInstanceContainer uiInfo)
        {
            string playerName = info?.info?.user_name;

            uiInfo.uiInstance.PlayerPoints = info.instance.playerPoints;

            if (playerName != null)
            {
                uiInfo.uiInstance.PlayerName = playerName;
            }

            if (!uiInfo.uiInstance.LoadedAvatarTexture.IsStartLoading)
            {
                string playeAvatarUrl = info?.info?.user_avatar;
                if (playeAvatarUrl != null)
                {
                    uiInfo.uiInstance.LoadedAvatarTexture.StartLoadData(playeAvatarUrl, this);
                }
            }
            else
            {
                if (uiInfo.uiInstance.LoadedAvatarTexture.IsLoaded && uiInfo.uiInstance.AvatarSprite == null)
                {
                    Texture loadedTexture = uiInfo.uiInstance.LoadedAvatarTexture.Data;
                    Rect sizeRect = new Rect(0, 0, loadedTexture.width, loadedTexture.height);
                    Sprite sprite = Sprite.Create(loadedTexture as Texture2D, sizeRect, Vector2.zero, 1f);
                    uiInfo.uiInstance.AvatarSprite = sprite;
                }
            }
        }

        #endregion

    }
}