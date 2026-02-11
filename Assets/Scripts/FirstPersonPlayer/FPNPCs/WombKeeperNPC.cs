using System;
using FirstPersonPlayer.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using MoreMountains.Feedbacks;
using Overview.NPC;
using SharedUI.Interface;
using UnityEngine;
using Utilities.Interface;

namespace FirstPersonPlayer.FPNPCs
{
    public class WombKeeperNPC : MonoBehaviour, IRequiresUniqueID, IInteractable, IBillboardable
    {
        public string uniqueID;
        [Header("NPC Definition")] public NpcDefinition npcDefinition;
        public string nodeToUse;

        [Header("Feedbacks")] [SerializeField] MMFeedbacks startDialogueFeedback;
        public string GetName()
        {
            return npcDefinition.characterName;
        }
        public Sprite GetIcon()
        {
            if (npcDefinition.characterIcon == null)
            {
                Debug.LogWarning($"NPC {npcDefinition.characterName} does not have a character icon assigned.");
                return null;
            }

            return npcDefinition.characterIcon;
        }
        public string ShortBlurb()
        {
            if (npcDefinition.npcDescription == null)
            {
                Debug.LogWarning($"NPC {npcDefinition.characterName} does not have a description assigned.");
                return "";
            }

            return npcDefinition.npcDescription;
        }
        public Sprite GetActionIcon()
        {
            return null;
        }
        public string GetActionText()
        {
            return "Begin Telepathy";
        }
        public void Interact()
        {
            FirstPersonDialogueEvent.Trigger(FirstPersonDialogueEventType.StartDialogue, npcDefinition.npcId, nodeToUse);

            startDialogueFeedback?.PlayFeedbacks();

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
    }
}
