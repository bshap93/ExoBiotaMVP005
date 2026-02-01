using System.Collections.Generic;
using Dirigible.Input;
using Dirigible.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using Lightbug.Utilities;
using Manager.SceneManagers.Dock;
using Objectives;
using Objectives.ScriptableObjects;
using Overview.NPC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dirigible.Interactable
{
    public class DirigibleNonDockNPCInteractable : MonoBehaviour, IDirigibleInteractable
    {
        public DockLocationHotspotLookupEntry dockLocationHotspotLookupEntry;
        [Header("Conditional Dialogue Nodes")] public
            DialogueCondition[] dialogueConditions;
        [SerializeField] public NpcDefinition npcDefinition;
        public string defaultStartNode;
        [FormerlySerializedAs("LocationId")] [SerializeField] [ValueDropdown("GetLocationIdOptions")]
        protected string locationId;
        [FormerlySerializedAs("CameraAnchorTransform")]
        public Transform cameraAnchorTransform;
        [SerializeField] LayerMask dirigibleLayers; // set to your Dirigible layer in Inspector
#if UNITY_EDITOR
        [FormerlySerializedAs("ActionId")] [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
        public int actionId;

        readonly HashSet<Collider> _dirigibleOverlaps = new();
        readonly bool _isInteractable = true;
        float _ignoreUntil;
        public void OnTriggerEnter(Collider other)
        {
            if (Time.time < _ignoreUntil) return;
            if (!IsDirigible(other)) return;

            // if (_dirigibleOverlaps.Count == 0)
            //     DockingEvent.Trigger(DockingEventType.SetCurrentDock, def);

            _dirigibleOverlaps.Add(other);

            Debug.Log("DirigibleNonDockNPCInteractable: Dirigible entered overlap with NPC interactable.");
        }
        public void OnTriggerExit(Collider other)
        {
            if (Time.time < _ignoreUntil) return;

            if (!IsDirigible(other)) return;

            _dirigibleOverlaps.Remove(other);
            // if (_dirigibleOverlaps.Count == 0)
            //     DockingEvent.Trigger(DockingEventType.UnsetCurrentDock, def);
        }


        public void Interact()
        {
            Debug.Log("DirigibleNonDockNPCInteractable: Interact called.");
            var nodeToUse = GetAppropriateStartNode();
            if (nodeToUse.IsNullOrWhiteSpace())
                FirstPersonDialogueEvent.Trigger(
                    FirstPersonDialogueEventType.StartDialogue, npcDefinition.npcId, defaultStartNode);
            else
                FirstPersonDialogueEvent.Trigger(
                    FirstPersonDialogueEventType.StartDialogue, npcDefinition.npcId, nodeToUse);

            MyUIEvent.Trigger(UIType.Any, UIActionType.Open);
            ControlsHelpEvent.Trigger(ControlHelpEventType.Hide, 0);
        }
        public void OnInteractionStart()
        {
        }
        public void OnInteractionEnd()
        {
        }
        public bool CanInteract()
        {
            return IsInteractable();
        }
        public bool IsInteractable()
        {
            return _isInteractable;
        }
        public void OnFocus()
        {
        }
        public void OnUnfocus()
        {
        }
        public void CompleteObjectiveOnInteract()
        {
        }
#if UNITY_EDITOR
        public IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
        {
            return AllRewiredActions.GetAllRewiredActions();
        }

#endif
        static string[] GetLocationIdOptions()
        {
            return DockManager.GetLocationIdOptions();
        }

        protected string GetAppropriateStartNode()
        {
            var objectivesManager = ObjectivesManager.Instance;
            if (objectivesManager == null)
            {
                Debug.LogWarning("[CommsConsole] ObjectivesManager not found, using default node");
                return defaultStartNode;
            }

            // Check each condition in order
            if (dialogueConditions != null)
                foreach (var condition in dialogueConditions)
                    if (condition.CheckCondition(objectivesManager))
                        return condition.startNode;

            // Fallback to original override
            return defaultStartNode;
        }

        bool IsDirigible(Collider other)
        {
            if (((1 << other.gameObject.layer) & dirigibleLayers) == 0) return false;
            return other.GetComponentInParent<DirigiblePhysicalObject>() != null;
        }
    }
}
