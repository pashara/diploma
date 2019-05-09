using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameScreenItems
{
    public class MiniMap : MonoBehaviour
    {
        class MapPlayerInfo
        {
            public Player player;
            public Transform playerPositionizer;
        }

        [SerializeField] float mapScale;
        [SerializeField] Vector2 minimapBounds;
        [SerializeField] Transform playersParent;
        [SerializeField] MiniMapBaseItem playerObjectPrefab;
        List<MiniMapBaseItem> playerItems = new List<MiniMapBaseItem>();

        public void CustomUpdate(float deltaTime)
        {
            UpdateVisual();
        }


        void CheckPlayers(List<MiniMapBaseItem> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].playerInstance == null)
                {
                    players.RemoveAt(i);
                    i--;
                }
            }
        }

        void UpdateVisual()
        {
            List<PlayerInfo> allPlayersInfo = MyNetworkManager.Instance.Players;
            CheckPlayers(playerItems);
            MapPlayerInfo[] info = new MapPlayerInfo[allPlayersInfo.Count];
            
            for (int i = 0; i < allPlayersInfo.Count; i++)
            {
                MapPlayerInfo currentInfo = new MapPlayerInfo();
                info[i] = currentInfo;
                currentInfo.player = allPlayersInfo[i].instance;
                currentInfo.playerPositionizer = allPlayersInfo[i].instance.CarDriver.MovablePart;
            }


            for (int i = 0; i < info.Length; i++)
            {
                Vector3 clampedPosition = new Vector3(info[i].playerPositionizer.position.x, info[i].playerPositionizer.position.z, info[i].playerPositionizer.position.y);

                Vector3 lbPart = LevelManager.Instance.CurrentLevel.RTBoundTransform.position;
                Vector3 rtPart = LevelManager.Instance.CurrentLevel.LBBoundTransform.position;
                

                lbPart = new Vector2(lbPart.x, lbPart.z);
                rtPart = new Vector2(rtPart.x, rtPart.z);
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, lbPart.x, rtPart.x);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, rtPart.y, lbPart.y);

                float xFactor = 1f - (clampedPosition.x - rtPart.x) / (lbPart.x - rtPart.x);
                float yFactor = (clampedPosition.y - rtPart.y) / (lbPart.y - rtPart.y);

                Vector2 factorVector = new Vector2(xFactor, yFactor);
                Vector2 positionOnMap = Vector2.Scale(minimapBounds, factorVector) * mapScale;

                MiniMapBaseItem minimapObjectInstance = playerItems.Find((item) => item.playerInstance == info[i].player);
                if (minimapObjectInstance == null)
                {
                    minimapObjectInstance = Instantiate<MiniMapBaseItem>(playerObjectPrefab, playersParent);
                    minimapObjectInstance.playerInstance = info[i].player;
                    playerItems.Add(minimapObjectInstance);
                }

                minimapObjectInstance.transform.localPosition = positionOnMap;
                
            }
        }
    }
}