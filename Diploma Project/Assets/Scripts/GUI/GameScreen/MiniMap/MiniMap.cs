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
        Dictionary<Player, MiniMapBaseItem> playerItems = new Dictionary<Player, MiniMapBaseItem>();

        public void CustomUpdate(float deltaTime)
        {
            UpdateVisual();
        }


        void UpdateVisual()
        {
            List<Player> allPlayersInfo = MyNetworkManager.Instance.Players;
            MapPlayerInfo[] info = new MapPlayerInfo[allPlayersInfo.Count];
            
            for (int i = 0; i < allPlayersInfo.Count; i++)
            {
                MapPlayerInfo currentInfo = new MapPlayerInfo();
                info[i] = currentInfo;
                currentInfo.player = allPlayersInfo[i];
                currentInfo.playerPositionizer = allPlayersInfo[i].CarDriver.MovablePart;
            }


            for (int i = 0; i < info.Length; i++)
            {
                Vector3 clampedPosition = new Vector3(info[i].playerPositionizer.position.x, info[i].playerPositionizer.position.z, info[i].playerPositionizer.position.y);

                Vector3 lbPart = LevelManager.Instance.CurrentLevel.RTBoundTransform.position;
                Vector3 rtPart = LevelManager.Instance.CurrentLevel.LBBoundTransform.position;

                Debug.Log($"User {info[i].playerPositionizer.position}");
                Debug.Log($"User {clampedPosition}");

                lbPart = new Vector2(lbPart.x, lbPart.z);
                rtPart = new Vector2(rtPart.x, rtPart.z);
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, lbPart.x, rtPart.x);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, rtPart.y, lbPart.y);

                float xFactor = 1f - (clampedPosition.x - rtPart.x) / (lbPart.x - rtPart.x);
                float yFactor = (clampedPosition.y - rtPart.y) / (lbPart.y - rtPart.y);

                Vector2 factorVector = new Vector2(xFactor, yFactor);
                Vector2 positionOnMap = Vector2.Scale(minimapBounds, factorVector) * mapScale;

                MiniMapBaseItem minimapObjectInstance;
                if (!playerItems.TryGetValue(info[i].player, out minimapObjectInstance))
                {
                    minimapObjectInstance = Instantiate<MiniMapBaseItem>(playerObjectPrefab);
                    minimapObjectInstance.transform.SetParent(playersParent);
                    playerItems.Add(info[i].player, minimapObjectInstance);
                }

                minimapObjectInstance.transform.localPosition = positionOnMap;
                
            }

            Debug.Log("--");
        }
    }
}