using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


namespace GameScreenItems
{
    public class KillsList : MonoBehaviour
    {
        [SerializeField] KillItem suicidePrefab;
        [SerializeField] KillItem realIncidentPrefab;
        [SerializeField] Transform spawnTransfom;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] float fadeInDuration;
        [SerializeField] float showDuration;
        [SerializeField] float fadeOutDuration;
        List<KillItem> spawnedItems = new List<KillItem>();

        #region Unity lifecycle

        void OnEnable()
        {
            Player.OnPlayerKilled += Player_OnPlayerKilled;
            
        }


        void OnDisable()
        {
            Player.OnPlayerKilled -= Player_OnPlayerKilled;

            spawnedItems.ForEach((item) =>
            {
                DOTween.Kill(item);
                Destroy(item.gameObject);
            });
            spawnedItems.Clear();

            DOTween.Kill(this);
        }

        #endregion



        #region Event handlers

        private void Player_OnPlayerKilled(Player killer, Player victim)
        {
            KillItem spanedPrefab = (killer == victim) ? suicidePrefab : realIncidentPrefab;
            KillItem item = Instantiate<KillItem>(spanedPrefab, spawnTransfom);
            item.Initilize(killer, victim);
            spawnedItems.Add(item);
            scrollRect.normalizedPosition = new Vector2(0, 0);

            Sequence sequence = DOTween.Sequence();
            item.Opacity = 0f;
            sequence.Append(DOTween.To(() => 0f, (x) => item.Opacity = x, item.MaxOpacity, Mathf.Max(fadeInDuration, CommonUtills.MinSequenceDuration)).SetEase(item.FadeInCurve).SetId(item));
            sequence.AppendInterval(Mathf.Max(showDuration, CommonUtills.MinSequenceDuration));
            sequence.Append(DOTween.To(() => item.MaxOpacity, (x) => item.Opacity = x, 0f, Mathf.Max(fadeOutDuration, CommonUtills.MinSequenceDuration)).SetEase(item.FadeOutCurve).SetId(item));
            sequence.SetId(this);
            sequence.OnComplete(() =>
            {
                spawnedItems.Remove(item);
                DOTween.Kill(item);
                Destroy(item.gameObject);
            });

        }

        #endregion
    }
}
