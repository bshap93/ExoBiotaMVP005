using Helpers.Events.Status;
using Manager;
using Manager.Status;
using MoreMountains.Tools;
using SharedUI.HUD.InGameTime;
using TMPro;
using UnityEngine;

namespace SharedUI.HUD.Infection
{
    public class BodyInfectionHUD : MonoBehaviour,
        MMEventListener<InfectionUIEvent>, MMEventListener<StatsStatusEvent>

    {
        // [SerializeField] ProgressBar nextInfectionProgressBar;

        [SerializeField] CanvasGroup bodyInfectionHUDCanvasGroup;

        [SerializeField] MinutesTillNextInfectionPb nextInfectionProgressBar;

        [SerializeField] TMP_Text nextInfectionStatusText;

        [SerializeField] InfectionHUDListview infectionHUDListview;

        [SerializeField] CanvasGroup topBoxCanvasGroup;
        [SerializeField] CanvasGroup bottomBoxCanvasGroup;

        void Start()
        {
            if (PlayerMutableStatsManager.Instance.IsContaminationMaxed())
                Show();
            else
                Hide();

            if (InfectionManager.Instance.OngoingInfections.Count > 0)
                UpdateTopVisibility(true);
            else
                UpdateTopVisibility(false);
        }

        void OnEnable()
        {
            this.MMEventStartListening<InfectionUIEvent>();
            this.MMEventStartListening<StatsStatusEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<InfectionUIEvent>();
            this.MMEventStopListening<StatsStatusEvent>();
        }

        public void OnMMEvent(InfectionUIEvent eventType)
        {
            nextInfectionProgressBar.UpdateUI(eventType.MinutesUntilNextInfection, eventType.MinutesPerInfection);

            if (!eventType.Enable)
            {
                nextInfectionProgressBar.UpdateUI(1f, 99f);

                infectionHUDListview.ClearInfection(eventType.OngoingInfection);
            }

            // if (eventType.NewInfection != null && eventType.Enable)
            //     infectionHUDListview.AddInfection(eventType.NewInfection);
            UpdateTopVisibility(InfectionManager.Instance.OngoingInfections.Count > 0);
        }
        public void OnMMEvent(StatsStatusEvent eventType)
        {
            if (eventType.StatType == StatsStatusEvent.StatsStatusType.Contamination &&
                eventType.Status == StatsStatusEvent.StatsStatus.IsMax)
                UpdateVisibility(eventType.Enabled);
        }

        public void Hide()
        {
            bodyInfectionHUDCanvasGroup.alpha = 0f;
            bodyInfectionHUDCanvasGroup.interactable = false;
            bodyInfectionHUDCanvasGroup.blocksRaycasts = false;
        }

        public void Show()
        {
            bodyInfectionHUDCanvasGroup.alpha = 1f;
            bodyInfectionHUDCanvasGroup.interactable = true;
            bodyInfectionHUDCanvasGroup.blocksRaycasts = true;
        }

        void UpdateVisibility(bool isContaminationMaxed)
        {
            if (isContaminationMaxed)
                Show();
            else
                Hide();
        }

        public void UpdateTopVisibility(bool isVisible)
        {
            topBoxCanvasGroup.alpha = isVisible ? 1f : 0f;
            topBoxCanvasGroup.interactable = isVisible;
            topBoxCanvasGroup.blocksRaycasts = isVisible;
        }
    }
}
