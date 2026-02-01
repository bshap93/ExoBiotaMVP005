using FirstPersonPlayer.Interactable.BioOrganism.Creatures;
using FirstPersonPlayer.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using Lightbug.Utilities;
using Manager;
using Manager.DialogueScene;
using MoreMountains.Feedbacks;
using Overview.NPC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstPersonPlayer.FPNPCs.AlienNPC
{
    public enum AlienNPCState
    {
        Hailable,
        InDialogue,
        Unavailable
    }

    public class AlienNPCController : CreatureController, IInteractable
    {
        [FormerlySerializedAs("NPCId")] [ValueDropdown("GetNpcIdOptions")]
        public
            string npcId;

        [SerializeField] float interactDistanceOverride = 5f;
        [SerializeField] int exobioticLanguageThreshold = 2;

        [SerializeField] string defaultStartNode;
        [SerializeField] MMFeedbacks startDialogueFeedback;
        [SerializeField] AlienNPCState initialState = AlienNPCState.Hailable;
        [SerializeField] bool isInteractable = true;
        [SerializeField] NpcDefinition npcDefinition;
        protected AlienNPCState CurrentState;
        protected override void Start()
        {
            base.Start();
            CurrentState = initialState;
        }
        public float GetInteractionDistance()
        {
            return interactDistanceOverride;
        }
        public void Interact()
        {
            if (!CanInteract()) return;

            var attributeMgr = AttributesManager.Instance;
            if (attributeMgr == null)
            {
                Debug.LogError("AttributesManager instance not found.");
                return;
            }

            var exobioticAttrLevel = attributeMgr.Exobiotic;


            if (npcDefinition.nativeLanguage == LanguageType.ModernGalactic)
                DialoguePresentationEvent.Trigger(
                    DialoguePresentationEventType.ChangeFontsOfNPCSide, LanguageType.ModernGalactic);
            else if (npcDefinition.nativeLanguage == LanguageType.Sheolite)
                if (exobioticAttrLevel >= exobioticLanguageThreshold)
                    DialoguePresentationEvent.Trigger(
                        DialoguePresentationEventType.ChangeFontsOfNPCSide, LanguageType.ModernGalactic);
                else
                    DialoguePresentationEvent.Trigger(
                        DialoguePresentationEventType.ChangeFontsOfNPCSide, LanguageType.Sheolite);


            var nodeToUse = GetAppropriateDialogueNode();

            if (nodeToUse.IsNullOrWhiteSpace())
                FirstPersonDialogueEvent.Trigger(FirstPersonDialogueEventType.StartDialogue, npcId, defaultStartNode);
            else
                FirstPersonDialogueEvent.Trigger(FirstPersonDialogueEventType.StartDialogue, npcId, nodeToUse);

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
            if (CurrentState == AlienNPCState.Unavailable) return false;
            if (CurrentState == AlienNPCState.InDialogue) return false;
            if (!isInteractable) return false;
            return true;
        }
        public bool IsInteractable()
        {
            return isInteractable;
        }
        public void OnFocus()
        {
        }
        public void OnUnfocus()
        {
        }


        protected string GetAppropriateDialogueNode()
        {
            // For now, just return the default start node.
            return defaultStartNode;
        }
        static string[] GetNpcIdOptions()
        {
            return DialogueManager.GetAllNpcIdOptions();
        }
    }
}
