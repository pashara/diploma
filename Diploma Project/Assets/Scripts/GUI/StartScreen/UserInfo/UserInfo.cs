using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace StartScreenItems
{
    public class UserInfo : MonoBehaviour
    {
        [SerializeField] LoadableImage avatarImage;
        [SerializeField] Text usernameLabel;
        [SerializeField] RectTransform scrollInfoParent;
        [SerializeField] DataRow rowPrefab;
        [SerializeField] Header headerPrefab;

        List<GameObject> spawnedOnScroll = new List<GameObject>();
        Coroutine loadImageCoroutine;


        public void Initialize(GlobalUserData data)
        {
            usernameLabel.text = data.user_name;
            avatarImage.IsSeted = false;
            loadImageCoroutine = GlobalServerManager.Instance.LoadTexture(data.user_avatar, (isSuccess, texture) =>
            {
                Sprite s = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
                avatarImage.OriginalImage.sprite = s;
                avatarImage.IsSeted = true;
            });

            Header header = Instantiate<Header>(headerPrefab, scrollInfoParent);
            header.TitleText = "Ahah. User info";
            spawnedOnScroll.Add(header.gameObject);



            DataRow rowData;

            rowData = Instantiate<DataRow>(rowPrefab, scrollInfoParent);
            rowData.TitleText = "Level";
            rowData.ValueText = data.level.ToString();
            spawnedOnScroll.Add(rowData.gameObject);

            rowData = Instantiate<DataRow>(rowPrefab, scrollInfoParent);
            rowData.TitleText = "Exp";
            rowData.ValueText = data.expirience.ToString();
            spawnedOnScroll.Add(rowData.gameObject);

            rowData = Instantiate<DataRow>(rowPrefab, scrollInfoParent);
            rowData.TitleText = "Money";
            rowData.ValueText = $"${data.money.ToString()}";
            spawnedOnScroll.Add(rowData.gameObject);


        }


        public void Deinitialize()
        {
            if (loadImageCoroutine != null)
            {
                StopCoroutine(loadImageCoroutine);
            }
            loadImageCoroutine = null;


            spawnedOnScroll.ForEach((item) =>
            {
                Destroy(item.gameObject);
            });
            spawnedOnScroll.Clear();

        }
    }
}