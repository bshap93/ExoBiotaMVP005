using Events;
using Helpers.Events;
using Helpers.Events.Gated;
using Helpers.ScriptableObjects.Gated;
using Manager;
using Overview.Locations.Anchor;
using Structs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstPersonPlayer.UI.LocationButtonBase.OverviewModeButtonDeriv
{
    public class DirigibleOverviewModeButtons : OverviewModeLocationButtons
    {
        [SerializeField] GatedRestDetails dirigibleRestDetails;
        DirigibleLocationAnchor _dirigibleLocationAnchor;


        string _spawnPointId;

        protected override void Awake()
        {
            base.Awake();
        }

        public void LeaveDock()
        {
            // ModeLoadEvent should always be triggered before DockingEvent
            ModeLoadEvent.Trigger(ModeLoadEventType.Enabled, GameMode.DirigibleFlight);
            DockingEvent.Trigger(DockingEventType.Undock);
            HideCanvasGroup();
        }

        public void OpenInventory()
        {
        }


        public override void Interact()
        {
            _dirigibleLocationAnchor = GetComponentInParent<DirigibleLocationAnchor>();
            if (_dirigibleLocationAnchor == null)
            {
                Debug.LogError("DirigibleLocationAnchor component is missing on the parent GameObject.");
                return;
            }

            CameraAnchorTransform = _dirigibleLocationAnchor.locationCameraAnchor;
            LocationId = _dirigibleLocationAnchor.dockOvLocationDefinition.locationId;

            OverviewLocationEvent.Trigger(
                LocationType.Dirigible, LocationActionType.Approach, LocationId,
                CameraAnchorTransform);

            HideCanvasGroup();
        }

        public void InitiateDirigibleRest()
        {
            _dirigibleLocationAnchor = GetComponentInParent<DirigibleLocationAnchor>();
            if (_dirigibleLocationAnchor == null)
            {
                Debug.LogError("DirigibleLocationAnchor component is missing on the parent GameObject.");
                return;
            }

            var dockId = _dirigibleLocationAnchor.dockOvLocationDefinition.dockId;
            if (dockId == null)
            {
                Debug.LogError("DirigibleLocationAnchor component is missing on the parent GameObject.");
                return;
            }

            GatedRestEvent.Trigger(
                GatedInteractionEventType.TriggerGateUI, dirigibleRestDetails,
                dirigibleRestDetails.defaultRestTimeMinutes, dockId);
        }

        public void SaveGame()
        {
            _dirigibleLocationAnchor = GetComponentInParent<DirigibleLocationAnchor>();
            _spawnPointId = _dirigibleLocationAnchor.dockOvLocationDefinition.spawnPointId;
            if (string.IsNullOrEmpty(_spawnPointId))
            {
                Debug.LogWarning(
                    "[SaveConsole] No SpawnPointId on this console; saving without updating checkpoint.");

                AlertEvent.Trigger(
                    AlertReason.CannotSave, "No spawn point defined for this location.",
                    "Cannot Save");

                return;
            }

            var info = new SpawnInfo
            {
                // Active Scene
                SceneName = SceneManager.GetActiveScene().name,
                SpawnPointId = _spawnPointId,
                Mode = GameMode.Overview
            };


            AlertEvent.Trigger(
                AlertReason.SavingGame,
                "Set this console as your checkpoint and save all current progress?",
                "Save Game?",
                AlertType.ChoiceModal,
                0f,
                onConfirm: () =>
                {
                    // Only set checkpoint if we had a spawnPoint
                    PlayerSpawnManager.Instance.Save(info); // writes checkpoint

                    // Perform the global save
                    SaveDataEvent.Trigger();

                    // Optional: toast after success (basic notification)
                    AlertEvent.Trigger(AlertReason.SavingGame, "All data saved successfully!", "Saved Game");
                    MyUIEvent.Trigger(UIType.Any, UIActionType.Close);
                },
                onCancel: () => { }
            );
        }
    }
}
