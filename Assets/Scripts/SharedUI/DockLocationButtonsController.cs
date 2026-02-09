using Events;
using Helpers.Events;
using Helpers.Events.Gated;
using Manager.Global;
using Manager.SceneManagers.Dock;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace SharedUI
{
    public class DockLocationButtonsController : MonoBehaviour, MMEventListener<ModeLoadEvent>,
        MMEventListener<DockingEvent>, MMEventListener<OverviewLocationEvent>,
        MMEventListener<MyUIEvent>,
        MMEventListener<DialogueEvent>, MMEventListener<GatedRestEvent>
    {
        [ValueDropdown("GetDockIdOptions")] [FormerlySerializedAs("DockId")]
        public string dockId;

        CanvasGroup _canvasGroup;

        void Awake()
        {
            EnsureInit();
            // start hidden
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }


        void OnEnable()
        {
            this.MMEventStartListening<ModeLoadEvent>();
            this.MMEventStartListening<DockingEvent>();
            this.MMEventStartListening<OverviewLocationEvent>();
            this.MMEventStartListening<MyUIEvent>();
            this.MMEventStartListening<DialogueEvent>();
            this.MMEventStartListening<GatedRestEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<ModeLoadEvent>();
            this.MMEventStopListening<DockingEvent>();
            this.MMEventStopListening<OverviewLocationEvent>();
            this.MMEventStopListening<MyUIEvent>();
            this.MMEventStopListening<DialogueEvent>();
            this.MMEventStopListening<GatedRestEvent>();
        }

        public void OnMMEvent(DialogueEvent eventType)
        {
            if (eventType.EventType == DialogueEventType.DialogueStarted)
                Hide(); // now guaranteed to work
        }

        // Handle DockingEvent which should be triggered AFTER ModeLoadEvent
        public void OnMMEvent(DockingEvent dockingEvent)
        {
            if (dockingEvent.EventType == DockingEventType.DockAtLocation)
                if (dockingEvent.DockDefinition.dockId == dockId)
                    Show();

            if (dockingEvent.EventType == DockingEventType.Undock)
                Hide();
        }
        public void OnMMEvent(GatedRestEvent eventType)
        {
            if (eventType.EventType == GatedInteractionEventType.TriggerGateUI)
            {
                if (dockId == eventType.DockId)
                    Hide();
            }
            else if (eventType.EventType == GatedInteractionEventType.CompleteInteraction)
            {
                if (dockId == eventType.DockId)
                    Show();
            }
            else if (eventType.EventType == GatedInteractionEventType.CloseGatedInteractionUI)
            {
                if (dockId == eventType.DockId)
                    Show();
            }
        }

        // Handle ModeLoadEvent which should be triggered BEFORE DockingEvent
        public void OnMMEvent(ModeLoadEvent modeLoadEvent)
        {
            if (modeLoadEvent.EventType == ModeLoadEventType.Enabled ||
                modeLoadEvent.EventType == ModeLoadEventType.Load)
                if (modeLoadEvent.ModeName != GameMode.Overview)
                    Hide();
        }

        public void OnMMEvent(MyUIEvent e)
        {
            if (e.uiType != UIType.InGameUI) return;

            if (e.uiActionType == UIActionType.Open)
            {
                // keep it visible if you want, but make it non-interactive
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;
                }
            }
            else if (e.uiActionType == UIActionType.Close)
            {
                // only re-enable if we're actually in Overview and this dock should be shown
                var inOverview = GameStateManager.Instance &&
                                 GameStateManager.Instance.CurrentMode == GameMode.Overview;

                if (!inOverview) return;

                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void OnMMEvent(OverviewLocationEvent eventType)
        {
            var locationId = eventType.LocationId;

            // var locationDefiniton = DockManager.Instance.locationTable.GetLocationDefinition(locationId);
            // if (locationDefiniton == null) return;

            // if (eventType.LocationActionType == LocationActionType.RetreatFrom)
            //     if (dockId == locationDefiniton.dockId)
            //         Show();
        }

        void EnsureInit()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null) Debug.LogError("CanvasGroup missing on DockLocationButtonsController.");
            }
        }

        // Show the canvas group
        public void Show()
        {
            EnsureInit();
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        // Hide the canvas group
        public void Hide()
        {
            EnsureInit();
            if (_canvasGroup == null) return;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        // static string[] GetDockIdOptions()
        // {
        //     return DockManager.GetDockIdOptions();
        // }
    }
}
