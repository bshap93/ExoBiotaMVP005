using System;
using FirstPersonPlayer.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using Helpers.Events.NPCs;
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

        [Header("Dialogue Camera")]
        [Tooltip(
            "Transform the dialogue camera will look at during conversation. " +
            "Drag a child bone/empty here (e.g. head or chest). " +
            "If left null, the NPC's root transform is used as a fallback.")]
        [SerializeField]
        Transform dialogueFocusPoint;

        [Header("Feedbacks")] [SerializeField] MMFeedbacks startDialogueFeedback;

#if UNITY_EDITOR
        /// Draws a gizmo so you can visually confirm focus point placement in the editor.
        void OnDrawGizmosSelected()
        {
            var target = dialogueFocusPoint != null ? dialogueFocusPoint.position : transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target, 0.08f);
            Gizmos.DrawLine(transform.position, target);
        }
#endif
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
            FirstPersonDialogueEvent.Trigger(
                FirstPersonDialogueEventType.StartDialogue, npcDefinition.npcId, nodeToUse);

            // Focus the dialogue camera on this NPC
            var focusTarget = dialogueFocusPoint != null ? dialogueFocusPoint : transform;
            DialogueCameraEvent.Trigger(DialogueCameraEventType.FocusOnTarget, focusTarget);


            startDialogueFeedback?.PlayFeedbacks();
            MyUIEvent.Trigger(UIType.Any, UIActionType.Open);
        }
        public void OnInteractionStart()
        {
        }
        public void OnInteractionEnd(string param)
        {
            // Release camera focus when dialogue ends
            DialogueCameraEvent.Trigger(DialogueCameraEventType.ReleaseFocus);
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
            return 6f;
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
