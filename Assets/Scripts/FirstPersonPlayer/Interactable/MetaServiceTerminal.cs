using System;
using EditorScripts;
using FirstPersonPlayer.Interface;
using FirstPersonPlayer.ScriptableObjects;
using Helpers.Events;
using Helpers.Events.Dialog;
using Helpers.Events.Spawn;
using Lightbug.Utilities;
using Manager.DialogueScene;
using MoreMountains.Feedbacks;
using SharedUI.Interface;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Interface;

namespace FirstPersonPlayer.Interactable
{
    public class MetaServiceTerminal : MonoBehaviour, IInteractable, IRequiresUniqueID, IBillboardable
    {
        public string uniqueID;
        [SerializeField] MMFeedbacks startDialogueFeedback;

        [SerializeField] string defaultStartNode = "NavigationServerSwitch";

        [SerializeField] string nodeToUse;

        [SerializeField] MetaTerminalInfoSO metaTerminalInfoSO;

        [ValueDropdown("GetNpcIdOptions")] public
            string npcId;

        [SerializeField] [InlineProperty] [HideLabel]
        SpawnInfoEditor overrideSpawnInfo;
        public string GetName()
        {
            throw new NotImplementedException();
        }
        public Sprite GetIcon()
        {
            throw new NotImplementedException();
        }
        public string ShortBlurb()
        {
            throw new NotImplementedException();
        }
        public Sprite GetActionIcon()
        {
            throw new NotImplementedException();
        }
        public string GetActionText()
        {
            throw new NotImplementedException();
        }
        public void Interact()
        {
            if (!CanInteract()) return;

            if (nodeToUse.IsNullOrWhiteSpace())
                FirstPersonDialogueEvent.Trigger(FirstPersonDialogueEventType.StartDialogue, npcId, defaultStartNode);
            else
                FirstPersonDialogueEvent.Trigger(FirstPersonDialogueEventType.StartDialogue, npcId, nodeToUse);

            startDialogueFeedback?.PlayFeedbacks();
            
            SpawnAssignmentEvent.Trigger(
                SpawnAssignmentEventType.SetMostRecentSpawnPoint, overrideSpawnInfo.SceneName,
                overrideSpawnInfo.SpawnPointId);

            MyUIEvent.Trigger(UIType.Any, UIActionType.Open);
        }
        public void OnInteractionStart()
        {
        }
        public void OnInteractionEnd(string param)
        {
        }
        public bool CanInteract()
        {
            return true;
        }
        public bool IsInteractable()
        {
            return true;
        }
        public void OnFocus()
        {
        }
        public void OnUnfocus()
        {
        }
        public float GetInteractionDistance()
        {
            return 5f;
        }
        public string UniqueID => uniqueID;
        public void SetUniqueID()
        {
            uniqueID = Guid.NewGuid().ToString();
        }
        public bool IsUniqueIDEmpty()
        {
            return string.IsNullOrEmpty(uniqueID);
        }

        static string[] GetNpcIdOptions()
        {
            return DialogueManager.GetAllNpcIdOptions();
        }
    }
}
