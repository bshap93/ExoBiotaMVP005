using System.Collections;
using DG.Tweening;
using Helpers.Events.Progression;
using Manager.ProgressionMangers;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace SharedUI.Progression
{
    public class XphudElement : MonoBehaviour, MMEventListener<XPEvent>,
        MMEventListener<ProgressionUpdateListenerNotifier>, MMEventListener<LevelingEvent>
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
        [SerializeField] TMP_Text currentLevelText;
        [SerializeField] TMP_Text unusedUpgradesText;
        [SerializeField] TMP_Text unusedAttributePointsText;

        void Start()
        {
            notificationCanvasGroup.alpha = 0;
            debugChipsCanvasGroup.alpha = debugMode ? 1 : 0;

            if (levelingManager != null)
            {
                totalXPText.text = levelingManager.CurrentTotalXP.ToString();
                currentLevelText.text = levelingManager.CurrentLevel.ToString();
                unusedUpgradesText.text = levelingManager.UnspentStatUpgrades.ToString();
                unusedAttributePointsText.text = levelingManager.UnspentAttributePoints.ToString();
            }

            xpNotify.SetActive(false);
            levelNotify.SetActive(false);
        }

        void OnEnable()
        {
            this.MMEventStartListening<XPEvent>();
            this.MMEventStartListening<ProgressionUpdateListenerNotifier>();
            this.MMEventStartListening<LevelingEvent>();
        }

        public void OnMMEvent(LevelingEvent eventType)
        {
            if (eventType.EventType == LevelingEventType.LevelUp) ShowLevelUpNotification(eventType.NewLevel);
        }
        public void OnMMEvent(ProgressionUpdateListenerNotifier eventType)
        {
            // for debug
            totalXPText.text = eventType.CurrentTotalXP.ToString();
            currentLevelText.text = eventType.CurrentLevel.ToString();
            unusedUpgradesText.text = eventType.CurrentUpgradesUnused.ToString();
            unusedAttributePointsText.text = eventType.CurrentAttributePointsUnused.ToString();
        }
        public void OnMMEvent(XPEvent eventType)
        {
            if (eventType.EventType == XPEventType.AwardXPToPlayer) ShowXPNotification(eventType.Amount);
        }

        void ShowXPNotification(int amount)
        {
            StartCoroutine(ShowXPNotificationCoroutine(amount));
        }

        void ShowLevelUpNotification(int newLevel)
        {
            StartCoroutine(ShowLevelUpNotificationCoroutine(newLevel));
        }

        IEnumerator ShowLevelUpNotificationCoroutine(int newLevel)
        {
            levelNotify.SetActive(true);
            levelNotifyComponent.SetLevelText(newLevel);
            // fades in tween
            notificationCanvasGroup.DOFade(1f, fadeInDuration);
            // notificationCanvasGroup.alpha = 1;
            yield return new WaitForSeconds(2f);
            // fades out
            notificationCanvasGroup.DOFade(0f, fadeOutDuration);
            levelNotify.SetActive(false);
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
