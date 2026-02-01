using System;
using System.Collections.Generic;
using Dirigible.Interactable;
using Events;
using Helpers.Events;
using MoreMountains.Tools;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using OWPData.ScriptableObjects;
using ScriptableObjects;
using Sirenix.OdinInspector;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager.SceneManagers.Dock
{
    public class DockManager : MonoBehaviour, MMEventListener<DockingEvent>, MMEventListener<OverviewLocationEvent>,
        MMEventListener<ModeLoadEvent>, MMEventListener<AlertNewContentEvent>
    {
        [FormerlySerializedAs("LocationTable")]
        public DockDefinitionLocationTable locationTable;

        [SerializeField] DockInteractionFeedbacks dockInteractionFeedbacks;

        [FormerlySerializedAs("mostRecentDockDefinition")]
        public DockDefinition currentDock;

        public LocationType currentLocationType;
        public string currentLocationId;

        [ValueDropdown("GetSceneNameOptions")] public string sceneName;

        public DirigibleDockInteractable[] dirigibleDocksInCurrentScene;

        public DirigibleDockInteractable currentDockInteractable;

        public List<AlertNewContentEvent> alertNewContentEvents;
        public static DockManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (SaveManager.Instance.saveManagersDontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            this.MMEventStartListening<DockingEvent>();
            this.MMEventStartListening<OverviewLocationEvent>();
            this.MMEventStartListening<ModeLoadEvent>();
            this.MMEventStartListening<AlertNewContentEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<DockingEvent>();
            this.MMEventStopListening<OverviewLocationEvent>();
            this.MMEventStopListening<ModeLoadEvent>();
            this.MMEventStopListening<AlertNewContentEvent>();
        }


        public void OnMMEvent(AlertNewContentEvent eventType)
        {
            AddAlertNewContentEvent(eventType);
        }

        public void OnMMEvent(DockingEvent e)
        {
            switch (e.EventType)
            {
                case DockingEventType.SetCurrentDock:
                    SetCurrentDockIfChanged(e.DockDefinition);
                    // Trigger the checking of state for new content
                    DockingEvent.Trigger(DockingEventType.FinishedDocking, e.DockDefinition);

                    break;

                case DockingEventType.UnsetCurrentDock:
                    UnsetCurrentDockIfMatches(e.DockDefinition);
                    break;

                case DockingEventType.DockAtLocation:
                    // Optional: only react if it's the current one
                    if (currentDock != null && currentDock.dockId == e.DockDefinition.dockId)
                        Debug.Log($"[DockManager] DockAtLocation -> {e.DockDefinition.dockId}");

                    break;
            }
        }

        public void OnMMEvent(ModeLoadEvent eventType)
        {
            if (eventType.EventType == ModeLoadEventType.Enabled && eventType.ModeName == GameMode.DirigibleFlight)
                // When the Overview mode is enabled, we can trigger the dock retrieval
                TriggerGetDocksInScene();
        }

        public void OnMMEvent(OverviewLocationEvent eventType)
        {
            if (eventType.LocationActionType == LocationActionType.Approach)
            {
                currentLocationId = eventType.LocationId;
                currentLocationType = eventType.LocationType;
            }
        }

        public void AddAlertNewContentEvent(AlertNewContentEvent alertNewContentEvent)
        {
            if (alertNewContentEvents == null)
                alertNewContentEvents = new List<AlertNewContentEvent>();

            alertNewContentEvents.Add(alertNewContentEvent);
        }

        public void RemoveAlertNewContentEvent(AlertNewContentEvent alertNewContentEvent)
        {
            if (alertNewContentEvents == null) return;
            alertNewContentEvents.Remove(alertNewContentEvent);
        }

        void SetCurrentDockIfChanged(DockDefinition def)
        {
            if (currentDock != null && currentDock.dockId == def.dockId) return;

            currentDock = def;
            currentDockInteractable = GetDockInCurrentScne(def);
            Debug.Log($"[DockManager] SetCurrentDock -> {def.dockId}");
        }

        bool UnsetCurrentDockIfMatches(DockDefinition def)
        {
            if (currentDock == null || currentDock.dockId != def.dockId)
                return false; // ignore stray/duplicate

            Debug.Log($"[DockManager] UnsetCurrentDock -> {def.dockId}");
            currentDock = null;
            currentDockInteractable = null;
            return true;
        }

        // Add any dock-related methods or properties here
        public bool DoesDockContainLocation(string dockId, string locationId)
        {
            // Implement logic to check if the dock contains the specified location
            // This is a placeholder implementation
            return false;
        }

        public static string[] GetDockIdOptions()
        {
            return new[]
            {
                "ScienceSettlementDock", "ClarasOutpostDock",
                "AshpoolMineDock"
            };
        }

        public DockOvLocationDefinition GetLocationDefinition(string locationId)
        {
            if (Instance == null || Instance.locationTable == null)
            {
                Debug.LogError("DockManager or LocationTable is not initialized.");
                return null;
            }

            return Instance.locationTable.GetLocationDefinition(locationId);
        }


        public static string[] GetLocationIdOptions()
        {
            return new[]
            {
                "DirigibleLocation_Mine01", "DirigibleLocation_Science001", "HouseNPC001", "Laboratory001",
                "MineLocation01", "CoreTraderStorefront", "Test", "Core", "DirigibleLocation_Trader001",
                "TraderShopSciencePost", "MineLocation01a", "DirigibleLocation_Mine01a", "MineLocationThreeJane",
                "WreckedRunnerSite_OverworldTile01"
            };
        }

        public static string[] GetSceneNameOptions()
        {
            return new[]
            {
                "Overworld", "Mine01"
            };
        }

        public DirigibleDockInteractable GetCurrentDockObject()
        {
            if (currentDock == null) return null; // ← guard

            if (dirigibleDocksInCurrentScene != null && dirigibleDocksInCurrentScene.Length > 0)
                foreach (var dock in dirigibleDocksInCurrentScene)
                    if (dock != null && dock.def != null && dock.def.dockId == currentDock.dockId)
                        return dock;

            return null;
        }

        public void TriggerGetDocksInScene()
        {
            dirigibleDocksInCurrentScene = FindObjectsByType<DirigibleDockInteractable>(FindObjectsSortMode.None);
        }

        public DirigibleDockInteractable GetDockInCurrentScne(DockDefinition eventTypeDockDefinition)
        {
            if (dirigibleDocksInCurrentScene != null && dirigibleDocksInCurrentScene.Length > 0)
                foreach (var dock in dirigibleDocksInCurrentScene)
                    if (dock.def.dockId == eventTypeDockDefinition.dockId)
                        return dock;

            Debug.LogWarning($"No dock found for id: {eventTypeDockDefinition.dockId}");
            return null;
        }

        public static DockOvLocationDefinition GetCentralLocation(string dockId)
        {
            throw new NotImplementedException();
        }
    }
}
