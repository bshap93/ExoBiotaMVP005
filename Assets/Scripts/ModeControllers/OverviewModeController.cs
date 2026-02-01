using System.Collections;
using System.ComponentModel;
using Dirigible;
using Dirigible.Interactable;
using Events;
using Helpers.Events;
using Manager.Global;
using Manager.SceneManagers.Dock;
using MoreMountains.Tools;
using Overview.UI;
using Structs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace ModeControllers
{
    public class OverviewModeController : ModeController, MMEventListener<DockingEvent>
    {
        [SerializeField] CinemachineCamera overviewCamera;
        [SerializeField] DirigiblePhysicalObject dirigiblePhysicalObject;

        [FormerlySerializedAs("overviewCameraAnchor")] [SerializeField]
        Transform overviewCameraTarget;

        [FormerlySerializedAs("_dirigibleDockInteractable")] [SerializeField] [ReadOnly(true)]
        public DirigibleDockInteractable dirigibleDockInteractable;


        [SerializeField] RootOverviewController _rootOverviewController;

        public void Start()
        {
            _rootOverviewController = GetComponentInChildren<RootOverviewController>();

            if (_rootOverviewController == null) Debug.LogWarning("RootOverviewController not found in children.");
        }

        public void OnEnable()
        {
            this.MMEventStartListening();

            if (DockManager.Instance == null)
            {
                Debug.LogError("OverviewManager not found in the scene.");
            }
            else
            {
                DockManager.Instance.TriggerGetDocksInScene();
                dirigibleDockInteractable = DockManager.Instance.GetCurrentDockObject();
                if (dirigibleDockInteractable != null)
                    PassDockToChildren(dirigibleDockInteractable);
                else
                    Debug.Log("[Overview] currentDock not set yet; waiting for DockingEvent.");
            }
        }

        public void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(DockingEvent e)
        {
            if (e.EventType == DockingEventType.SetCurrentDock
                && DockManager.Instance != null
                && GameStateManager.Instance != null
                && GameStateManager.Instance.CurrentMode == GameMode.Overview)
            {
                var dock = DockManager.Instance.GetDockInCurrentScne(e.DockDefinition);
                if (dock != null)
                {
                    dirigibleDockInteractable = dock;
                    PassDockToChildren(dock);
                    StartCoroutine(MoveCameraTargetToLocation(dock.overviewCameraTarget));
                    _rootOverviewController?.ShowOverview();
                }
            }

            if (e.EventType == DockingEventType.UnsetCurrentDock) dirigibleDockInteractable = null;
        }


        public override IEnumerator Attach()
        {
            yield return null; // Wait for the next frame to ensure the camera is ready


            Cursor.lockState = CursorLockMode.None; // <─ unlock
            Cursor.visible = true; // show pointer


            // ModeLoadEvent.Trigger(ModeLoadEventType.Load, GameMode.Overview);

            if (DockManager.Instance == null)
            {
                Debug.LogError("DockManager not found in the scene.");
                yield break;
            }

            DockManager.Instance.TriggerGetDocksInScene();
            dirigibleDockInteractable = DockManager.Instance.GetCurrentDockObject();
            if (dirigibleDockInteractable != null)
            {
                PassDockToChildren(dirigibleDockInteractable);
                StartCoroutine(MoveCameraTargetToLocation(dirigibleDockInteractable.overviewCameraTarget));
                // if (_rootOverviewController != null)
                //     _rootOverviewController.ShowOverview(dirigibleDockInteractable.dockLocationHotspotLookupEntries);
            }

            if (dirigibleDockInteractable == null || dirigibleDockInteractable.def == null ||
                string.IsNullOrEmpty(dirigibleDockInteractable.def.dockId))
            {
                Debug.LogWarning("DirigibleDockInteractable or its definition is not set properly.");
                yield break;
            }

            ModeLoadEvent.Trigger(ModeLoadEventType.Load, GameMode.Overview, dirigibleDockInteractable.def.dockId);
        }

        public IEnumerator MoveCameraTargetToLocation(Transform target)
        {
            if (overviewCamera != null && target != null)
            {
                overviewCameraTarget.transform.position = target.position;
                overviewCameraTarget.transform.rotation = target.rotation;
                yield return null; // Wait for the next frame to ensure the camera is updated
            }
            else
            {
                Debug.LogWarning("Overview camera or target is not set.");
            }
        }

        void PassDockToChildren(DirigibleDockInteractable dockInteractable)
        {
            _rootOverviewController.dirigibleDockInteractable = dockInteractable;
        }

        public override void Detach()
        {
            _rootOverviewController?.HideOverview(); // Hide the overview UI
            Cursor.lockState = CursorLockMode.Locked; // <─ lock again (or Confined)
            Cursor.visible = false;
        }
    }
}
