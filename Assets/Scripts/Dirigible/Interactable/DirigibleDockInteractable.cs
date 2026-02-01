using System;
using System.Collections;
using System.Collections.Generic;
using Dirigible.HighlightTriggers;
using Dirigible.Input;
using Dirigible.Interface;
using Events;
using Helpers.Events;
using Helpers.Events.ManagerEvents;
using Helpers.Wrappers;
using Manager.Global;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Objectives.ScriptableObjects;
using Overview.Locations;
using Overview.Locations.Anchor;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using OWPData.ScriptableObjects;
using Sirenix.OdinInspector;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Interface;

namespace Dirigible.Interactable
{
    [Serializable]
    public class DockLocationHotspotLookupEntry
    {
        // Rest of information about this dock in SO
        public DockOvLocationDefinition LocationDefinition;

        // Location on UI canvas where button is located
        public RectTransform hotspotRectTransform; // for easy access in codeq

        // Should be a component on the above RectTransform
        public LocationAnchorObject anchorObject;
    }


    [RequireComponent(typeof(Collider))]
    public class DirigibleDockInteractable : MonoBehaviour, IDirigibleInteractable, MMEventListener<DockingEvent>,
        MMEventListener<ModeLoadEvent>, IRequiresUniqueID
    {
        [SerializeField] Transform dockAnchor; // assign DockAnchor child
        [SerializeField] ObjectiveObject objectiveToCompleteOnInteract;

#if UNITY_EDITOR
        [FormerlySerializedAs("ActionId")] [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
        public int actionId;

        [SerializeField] GameObject rewiredPrefab;

        [FormerlySerializedAs("overviewCameraAnchor")]
        public Transform overviewCameraTarget; // assign OverviewCameraAnchor child

        // public ObjectiveObject linkedObjective; // optional, assign in Inspector

        [SerializeField] float slideTime = 0.75f;

        [SerializeField] public DockDefinition def;

        [SerializeField] MMFeedbacks dockingFeedbacks;

        [FormerlySerializedAs("dockHotspots")] [SerializeField]
        public List<DockLocationHotspotLookupEntry> dockLocationHotspotLookupEntries;

        public List<GamePOIWrapper> overworldSettlementPOIList; // assign in Inspector

        [FormerlySerializedAs("locationCanvasController")] [SerializeField]
        CanvasGroup locationCanvas;

        [FormerlySerializedAs("LocationCanvasGroup")] [SerializeField]
        DirigiblePhysicalObject dirigible;

        [SerializeField] LayerMask dirigibleLayers; // set to your Dirigible layer in Inspector

        readonly HashSet<Collider> _dirigibleOverlaps = new();

        DockHighlightTrigger _highlightTrigger;
        float _ignoreUntil;


        void Awake()
        {
            HideLocationCanvas();
            _highlightTrigger = GetComponent<DockHighlightTrigger>();
        }

        void OnEnable()
        {
            this.MMEventStartListening<ModeLoadEvent>();
            this.MMEventStartListening<DockingEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<ModeLoadEvent>();
            this.MMEventStopListening<DockingEvent>();
        }


        public void OnTriggerEnter(Collider other)
        {
            if (Time.time < _ignoreUntil) return;
            if (!IsDirigible(other)) return;


            _dirigibleOverlaps.Add(other);
        }

        public void OnTriggerExit(Collider other)
        {
            if (Time.time < _ignoreUntil) return;

            if (!IsDirigible(other)) return;

            _dirigibleOverlaps.Remove(other);
            if (_dirigibleOverlaps.Count == 0)
                DockingEvent.Trigger(DockingEventType.UnsetCurrentDock, def);
        }


        public bool IsInteractable()
        {
            return true;
        }

        public bool CanInteract()
        {
            return true;
        }

        public void OnFocus()
        {
            /* highlight */
        }

        public void OnUnfocus()
        {
            /* un‑highlight */
        }
        public void CompleteObjectiveOnInteract()
        {
            if (objectiveToCompleteOnInteract != null)
                ObjectiveEvent.Trigger(
                    objectiveToCompleteOnInteract.objectiveId, ObjectiveEventType.ObjectiveCompleted
                );
        }

        public void OnInteractionStart()
        {
            // Debug.Log("Interacted with " + gameObject.name);
            //
            // StartCoroutine(SlideInAndEnterOverview());
        }

        public void OnInteractionEnd()
        {
        }

        public virtual void Interact()
        {
            dirigible = FindFirstObjectByType<DirigiblePhysicalObject>();
            Debug.Log("Interacted with " + gameObject.name);

            StartCoroutine(SlideInAndEnterOverview());
        }
        public string UniqueID => def.dockId;
        public void SetUniqueID()
        {
            // UniqueID = Guid.NewGuid().ToString();
            // Not needed, dock ID comes from ScriptableObject
        }
        public bool IsUniqueIDEmpty()
        {
            return string.IsNullOrEmpty(def.dockId);
        }

        public void OnMMEvent(DockingEvent e)
        {
            if (e.EventType == DockingEventType.Undock)
                _ignoreUntil = Time.time + 0.25f; // ignore for 0.25s

            if (e.DockDefinition == null || e.DockDefinition.dockId != def.dockId) return;

            var inOverview = GameStateManager.Instance &&
                             GameStateManager.Instance.CurrentMode == GameMode.Overview;

            switch (e.EventType)
            {
                case DockingEventType.DockAtLocation:
                    if (inOverview) ShowLocationCanvas();
                    break;

                case DockingEventType.SetCurrentDock:
                    // Important: On initial load you start inside the trigger,
                    // so SetCurrentDock may be the only event you get.
                    if (inOverview) ShowLocationCanvas();
                    break;

                case DockingEventType.Undock:
                case DockingEventType.UnsetCurrentDock:
                    HideLocationCanvas();
                    break;
            }
        }
        public void OnMMEvent(ModeLoadEvent eventType)
        {
            if (eventType.EventType == ModeLoadEventType.Load)
                if (eventType.ModeName == GameMode.Overview)
                    if (def.dockId == eventType.DockId)
                        // We are loading into overview at this dock
                        TriggerPOIEvent();
        }

#if UNITY_EDITOR
        public IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
        {
            return AllRewiredActions.GetAllRewiredActions();
        }

#endif


        bool IsDirigible(Collider other)
        {
            if (((1 << other.gameObject.layer) & dirigibleLayers) == 0) return false;
            return other.GetComponentInParent<DirigiblePhysicalObject>() != null;
        }

        public void HideLocationCanvas()
        {
            if (locationCanvas != null)
            {
                locationCanvas.alpha = 0f;
                locationCanvas.interactable = false;
                locationCanvas.blocksRaycasts = false;
            }
            else
            {
                Debug.LogWarning("[Dock] Location canvas is null, cannot hide it.");
            }
        }

        public void ShowLocationCanvas()
        {
            if (locationCanvas != null)
            {
                locationCanvas.alpha = 1f;
                locationCanvas.interactable = true;
                locationCanvas.blocksRaycasts = true;

                // Bind buttons to their location defs (idempotent and cheap)
                foreach (var e in dockLocationHotspotLookupEntries)
                {
                    if (e?.hotspotRectTransform == null || e.LocationDefinition == null) continue;
                    var spawner = e.hotspotRectTransform.GetComponentInChildren<SpawnLocationButton>();
                    if (spawner != null) spawner.Setup(e.LocationDefinition);
                }
            }
            else
            {
                Debug.LogWarning("[Dock] Location canvas is null, cannot show it.");
            }
        }

// #if UNITY_EDITOR
//         // This will be called from the parent ScriptableObject
//         IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
//         {
//             var parent = ControlsPromptSchemeSet._currentContextSO;
//             if (parent == null || parent.inputManagerPrefab == null) yield break;
//
//             var data = parent.inputManagerPrefab.userData;
//             if (data == null) yield break;
//
//             foreach (var action in data.GetActions_Copy())
//                 yield return new ValueDropdownItem<int>(action.name, action.id);
//         }
// #endif


        IEnumerator SlideInAndEnterOverview()
        {
            // ─── 1. Disable flight input & stabilise physics ───────────────────────────
            if (DirigibleInput.Instance != null)
                DirigibleInput.Instance.enabled = false;
            else
                Debug.LogWarning("[Dock] DirigibleInput singleton was null — skipping input disable");

            if (dirigible == null)
            {
                Debug.LogError("[Dock] DirigiblePhysicalObject not found — aborting dock");
                yield break;
            }

            CompleteObjectiveOnInteract();


            ControlsHelpEvent.Trigger(ControlHelpEventType.ShowUseThenHide, actionId);


            var rb = dirigible.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            // 2. Lerp to anchor
            var t = 0f;
            try
            {
                if (dockingFeedbacks != null)
                {
                    dockingFeedbacks.AutoInitialization = true;
                    dockingFeedbacks?.PlayFeedbacks();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Dock] Exception during docking feedbacks: " + ex);
            }

            var startPos = dirigible.transform.position;
            var startRot = dirigible.transform.rotation;


            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / slideTime;
                dirigible.transform.position =
                    Vector3.Lerp(startPos, dockAnchor.position, t);

                dirigible.transform.rotation =
                    Quaternion.Slerp(startRot, dockAnchor.rotation, t);

                yield return null;
            }

            dockingFeedbacks?.StopFeedbacks();


            // Order-> Set dock, then load overview mode, then dock at location
            // DockingEvent.Trigger(DockingEventType.SetCurrentDock, def);
            ModeLoadEvent.Trigger(ModeLoadEventType.Enabled, GameMode.Overview);
            DockingEvent.Trigger(DockingEventType.DockAtLocation, def);

            SpawnEvent.Trigger(
                SpawnEventType.ToDock, def.spawnInfo.SceneName,
                GameMode.Overview, def.spawnInfo.SpawnPointId, def);

            CheckpointEvent.Trigger(def.spawnInfo.ToSpawnInfo());
            _highlightTrigger.showDockingInstructions = false;


            // overworldPOIHelper.gameObject.SetActive(false);

            // Wait one second 
            // yield return new WaitForSeconds(1f);
            // DockingEvent.Trigger(DockingEventType.FinishedDocking, def);
            // Get the POI
            TriggerPOIEvent();
        }
        void TriggerPOIEvent()
        {
            var poi = GetComponent<GamePOIWrapper>();
            if (poi == null)
            {
                Debug.LogError("[Dock] No GamePOIWrapper found on dock " + gameObject.name);
                return;
            }

            var sceneName = gameObject.scene.name;

            // ObjectiveEvent.Trigger(
            //     linkedObjective?.objectiveId, ObjectiveEventType.ObjectiveCompleted
            // );

            foreach (var poiWrapper in overworldSettlementPOIList)
            {
                if (poiWrapper == null) continue;
                GamePOIEvent.Trigger(poiWrapper.UniqueID, GamePOIEventType.MakeAlwaysVisible, sceneName);
            }
        }
    }
}
