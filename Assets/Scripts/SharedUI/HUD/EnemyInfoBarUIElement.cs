using DG.Tweening;
using Helpers.Events.Combat;
using Helpers.Events.UI;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace SharedUI.HUD
{
    public class EnemyInfoBarUIElement : MonoBehaviour, MMEventListener<EnemyDamageEvent>, MMEventListener<HotbarEvent>
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text enemyNameText;
        [SerializeField] MMProgressBar enemyHealthBar;
        [SerializeField] MMProgressBar enemyStunDamageBar;
        [Header("Feedbacks")] [SerializeField] MMFeedbacks infoBarDeathFeedbacks;
        [SerializeField] MMFeedbacks hitEnemyFeedbacks;
        [SerializeField] MMFeedbacks criticalHitEnemyFeedbacks;
        [Header("Update")] [Tooltip("Minimum absolute change before we push a UI update")] [SerializeField]
        float epsilon = 0.001f;
        [SerializeField] float fadeInOnDamageDuration = 0.1f;
        [SerializeField] float fadeInOnStunDamageDuration = 0.1f;
        [SerializeField] float fadeOutOnTimeoutDuration = 0.3f;
        [SerializeField] float visibleDurationAfterDamageDealt = 5f;
        [SerializeField] GameObject barVisual;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        Tween _fadeTween;

        bool _isVisible;

        float _timeSinceLastDamageDealt;

        void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        }

        void Start()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        void Update()
        {
            if (!_isVisible) return;
            _timeSinceLastDamageDealt += Time.deltaTime;

            if (_timeSinceLastDamageDealt >= visibleDurationAfterDamageDealt) FadeOut(fadeOutOnTimeoutDuration);
        }
        void OnEnable()
        {
            this.MMEventStartListening<EnemyDamageEvent>();
            this.MMEventStartListening<HotbarEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<EnemyDamageEvent>();
            this.MMEventStopListening<HotbarEvent>();
        }
        public void OnMMEvent(EnemyDamageEvent eventType)
        {
            if (enemyNameText != null)
                enemyNameText.text = eventType.EnemyName;


            if (eventType.EventType == DamageEventType.DealtDamage)
            {
                FadeIn(fadeInOnDamageDuration);
                if (eventType.TypeOfDamage == DamageType.Health)
                    TryUpdateHealthBar(ref eventType.LastValue, eventType.CurrentValue, 0f, eventType.DefaultValue);
                else if (eventType.TypeOfDamage == DamageType.Stun)
                    TryUpdateStunBar(ref eventType.LastValue, eventType.CurrentValue, eventType.DefaultValue);

                hitEnemyFeedbacks?.PlayFeedbacks();
                _timeSinceLastDamageDealt = 0f;
            }

            if (eventType.EventType == DamageEventType.CriticalHitDamage)
            {
                FadeIn(fadeInOnDamageDuration);
                TryUpdateHealthBar(ref eventType.LastValue, eventType.CurrentValue, 0f, eventType.DefaultValue);
                criticalHitEnemyFeedbacks?.PlayFeedbacks();
                _timeSinceLastDamageDealt = 0f;
            }
            else if (eventType.EventType == DamageEventType.Death)
            {
                infoBarDeathFeedbacks?.PlayFeedbacks();
                FadeOut(fadeOutOnTimeoutDuration);
                ResetBar();
            }
        }
        public void OnMMEvent(HotbarEvent eventType)
        {
            if (eventType.EventType == HotbarEvent.HotbarEventType.HideHotbars) barVisual.SetActive(false);
            else if (eventType.EventType == HotbarEvent.HotbarEventType.ShowHotbars) barVisual.SetActive(true);
        }
        void TryUpdateStunBar(ref float eventTypeLastValue, float eventTypeCurrentValue, float stunThreshold)
        {
            if (enemyStunDamageBar == null) return;

            // Stun bar increases from 0, so we don't clamp to min
            // Only push an update when the source value actually changed
            if (float.IsNaN(eventTypeLastValue) || Mathf.Abs(eventTypeCurrentValue - eventTypeLastValue) > epsilon)
            {
                // Smooth animated update (MMProgressBar handles the tween)
                enemyStunDamageBar.UpdateBar(eventTypeCurrentValue, 0f, stunThreshold);
                eventTypeLastValue = eventTypeCurrentValue;
            }
        }

        void ResetBar()
        {
        }

        void TryUpdateHealthBar(ref float last, float current, float min, float max)
        {
            if (enemyHealthBar == null) return;
            current = Mathf.Clamp(current, min, max);

            // Only push an update when the source value actually changed
            if (float.IsNaN(last) || Mathf.Abs(current - last) > epsilon)
            {
                // Smooth animated update (MMProgressBar handles the tween)
                enemyHealthBar.UpdateBar(current, min, max);
                last = current;
            }
        }

        public void FadeIn(float duration)
        {
            if (_isVisible && canvasGroup.alpha >= 1) return;
            _fadeTween?.Kill();
            _isVisible = true;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            _fadeTween = canvasGroup
                .DOFade(1f, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                });
        }

        public void FadeOut(float duration)
        {
            if (!_isVisible) return;
            _fadeTween?.Kill();
            _isVisible = false;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            _fadeTween = canvasGroup
                .DOFade(0f, duration)
                .SetEase(Ease.InQuad);
        }
    }
}
