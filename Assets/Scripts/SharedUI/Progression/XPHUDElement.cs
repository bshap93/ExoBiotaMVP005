using System.Collections;
using DG.Tweening;
using Helpers.Events.Progression;
using Manager.ProgressionMangers;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace SharedUI.Progression
{
    public class XphudElement : MonoBehaviour, MMEventListener<XPEvent>
    {
        [Header("Main Canvas Group")] [SerializeField]
        CanvasGroup notificationCanvasGroup;
        [SerializeField] CanvasGroup debugCanvasGroup;
        [SerializeField] bool debugMode = true;

        [Header("References")] [SerializeField]
        LevelingManager levelingManager;

        [SerializeField] XPNotify xpNotifyComponent;
        [SerializeField] LevelNotify levelNotifyComponent;

        [Header("Notification")] [SerializeField]
        GameObject xpNotify;
        [SerializeField] GameObject levelNotify;
        [SerializeField] float fadeInDuration = 0.5f;
        [SerializeField] float fadeOutDuration = 0.5f;


        [Header("Debug")] [SerializeField] CanvasGroup debugChipsCanvasGroup;
        [SerializeField] TMP_Text totalXPText;

        void Start()
        {
            notificationCanvasGroup.alpha = 0;
            debugChipsCanvasGroup.alpha = debugMode ? 1 : 0;

            xpNotify.SetActive(false);
            levelNotify.SetActive(false);
        }
        public void OnMMEvent(XPEvent eventType)
        {
            if (eventType.EventType == XPEventType.AwardXPToPlayer)
            {
                ShowXPNotification(eventType.Amount);
                // for debug
                totalXPText.text = "Total XP: " + levelingManager.CurrentTotalXP;
            }
        }

        public void ShowXPNotification(int amount)
        {
            StartCoroutine(ShowXPNotificationCoroutine(amount));
        }

        IEnumerator ShowXPNotificationCoroutine(int amount)
        {
            xpNotify.SetActive(true);
            xpNotifyComponent.SetXPText(amount);
            // fades in tween
            notificationCanvasGroup.DOFade(1f, fadeInDuration);
            // notificationCanvasGroup.alpha = 1;
            yield return new WaitForSeconds(2f);
            // fades out
            notificationCanvasGroup.DOFade(0f, fadeOutDuration);
            xpNotify.SetActive(false);
        }
    }
}
