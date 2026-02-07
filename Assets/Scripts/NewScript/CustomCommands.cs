using System;
using Helpers.Events;
using Helpers.Events.ManagerEvents;
using Helpers.Events.Progression;
using Inventory;
using MoreMountains.InventoryEngine;
using Objectives;
using Overview.NPC;
using UnityEngine;
using Yarn.Unity;

namespace NewScript
{
    public class CustomCommands : MonoBehaviour
    {
        // Drag and drop your Dialogue Runner into this variable.
        public DialogueRunner dialogueRunner;
        public GameObject characterNPCRoot;

        public void Awake()
        {
            // Create a new command called 'camera_look', which looks at a target. 
            // Note how we're listing 'GameObject' as the parameter type.
            dialogueRunner.AddCommandHandler(
                "camera_look", // the name of the command
                CameraLookAtTarget // the method to run
            );

            // Inventory Commands

            dialogueRunner.AddCommandHandler<string, int>(
                "give_player_item",
                GivePlayerItem
            );

            // Dialogue Gestures

            dialogueRunner.AddCommandHandler<string, string>(
                "trigger_gesture",
                TriggerGesture
            );

            dialogueRunner.AddCommandHandler<string, string>(
                "switch_idle_animation",
                SwitchIdleLoopingAnimation
            );

            // ----------- Objectives commands ----------

            dialogueRunner.AddCommandHandler<string>(
                "add_objective",
                AddObjective
            );

            dialogueRunner.AddCommandHandler<string>(
                "activate_objective",
                ActivateObjective
            );

            dialogueRunner.AddCommandHandler<string>(
                "make_objective_inactive",
                MakeObjectiveInactive
            );

            dialogueRunner.AddCommandHandler<string>(
                "complete_objective",
                CompleteObjective
            );

            dialogueRunner.AddCommandHandler<string>(
                "mark_poi_as_having_new_content",
                MarkPOIAsHavingNewContent
            );

            dialogueRunner.AddCommandHandler<int>(
                "trigger_stat_upgrade",
                TriggerStatUpgrade
            );
        }

        // The method that gets called when '<<camera_look>>' is run.
        void CameraLookAtTarget()
        {
            Debug.LogWarning("Looking at target: ");
        }

        // Inventory Commands

        public void GivePlayerItem(string itemId, int amount = 1)
        {
            Debug.Log($"[Yarn] give_player_item on {name} (instanceID={GetInstanceID()}) x{amount}");

            var inv = GlobalInventoryManager.Instance;
            if (inv == null)
            {
                Debug.LogWarning("GlobalInventoryManager not found, cannot give item.");
                return;
            }

            var item = inv.CreateItem(itemId); // SINGLE unit item
            if (item == null)
            {
                Debug.LogWarning($"Item with ID '{itemId}' not found.");
                return;
            }

            MMInventoryEvent.Trigger(
                MMInventoryEventType.Pick, null,
                item.TargetInventoryName, item, amount, 0, inv.playerId);
        }


        // Progression Commands
        public void TriggerStatUpgrade(int typeId)
        {
            if (typeId < 0 || typeId >= Enum.GetValues(typeof(StatType)).Length)
            {
                Debug.LogWarning($"Invalid StatType id: {typeId}");
                return;
            }

            var statType = (StatType)typeId;

            SpendStatUpgradeEvent.Trigger(statType);
        }

        // Dialogue Gestures


        public void TriggerGesture(string npcId, string key)
        {
            // Find NPC by id in the scene
            if (characterNPCRoot == null)
            {
                Debug.LogError($"NPC '{npcId}' not found in scene.");
                return;
            }

            var helper = characterNPCRoot.GetComponentInChildren<NPCCharacterAnimancerHelper>();

            if (helper == null) return;

            helper.PlayGesture(key);
        }

        public void SwitchIdleLoopingAnimation(string npcId, string key)
        {
            // Find NPC by id in the scene
            if (characterNPCRoot == null)
            {
                Debug.LogError($"NPC '{npcId}' not found in scene.");
                return;
            }

            var helper = characterNPCRoot.GetComponentInChildren<NPCCharacterAnimancerHelper>();

            if (helper == null) return;

            helper.SwitchIdleLoopingAnimation(key);
        }

        // ----------- Objectives commands ----------

        public void AddObjective(string objectiveId)
        {
            var objMgr = ObjectivesManager.Instance;
            if (objMgr == null)
            {
                Debug.LogWarning("ObjectivesManager not found, cannot add objective.");
                return;
            }

            var obj = objMgr.GetObjectiveById(objectiveId);
            if (obj == null)
            {
                Debug.LogWarning($"Objective with ID '{objectiveId}' not found.");
                return;
            }

            ObjectiveEvent.Trigger(objectiveId, ObjectiveEventType.ObjectiveAdded);
            AlertEvent.Trigger(AlertReason.NewObjective, obj.objectiveText, obj.objectiveId);
        }

        public void ActivateObjective(string objectiveId)
        {
            var objMgr = ObjectivesManager.Instance;
            if (objMgr == null)
            {
                Debug.LogWarning("ObjectivesManager not found, cannot add objective.");
                return;
            }

            var obj = objMgr.GetObjectiveById(objectiveId);
            if (obj == null)
            {
                Debug.LogWarning($"Objective with ID '{objectiveId}' not found.");
                return;
            }

            ObjectiveEvent.Trigger(objectiveId, ObjectiveEventType.ObjectiveActivated);
            AlertEvent.Trigger(AlertReason.NewObjective, obj.objectiveText, obj.objectiveId);
        }

        public void MakeObjectiveInactive(string objectiveId)
        {
            var objMgr = ObjectivesManager.Instance;
            if (objMgr == null)
            {
                Debug.LogWarning("ObjectivesManager not found, cannot add objective.");
                return;
            }

            var obj = objMgr.GetObjectiveById(objectiveId);
            if (obj == null)
            {
                Debug.LogWarning($"Objective with ID '{objectiveId}' not found.");
                return;
            }

            ObjectiveEvent.Trigger(objectiveId, ObjectiveEventType.ObjectiveDeactivated);
        }

        public void MarkPOIAsHavingNewContent(string uniqueID)
        {
            GamePOIEvent.Trigger(uniqueID, GamePOIEventType.POIMarkedAsHavingNewContent, null);
        }

        public void CompleteObjective(string objectiveId)
        {
            var objMgr = ObjectivesManager.Instance;
            if (objMgr == null)
            {
                Debug.LogWarning("ObjectivesManager not found, cannot complete objective.");
                return;
            }

            var obj = objMgr.GetObjectiveById(objectiveId);
            if (obj == null)
            {
                Debug.LogWarning($"Objective with ID '{objectiveId}' not found.");
                return;
            }

            ObjectiveEvent.Trigger(objectiveId, ObjectiveEventType.ObjectiveCompleted);
        }
    }
}
