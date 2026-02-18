using System.Collections.Generic;
using Dirigible.Input;
using FirstPersonPlayer.Interactable.BioOrganism.Creatures;
using FirstPersonPlayer.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using Lightbug.Utilities;
using Manager;
using Manager.DialogueScene;
using Manager.ProgressionMangers;
using MoreMountains.Feedbacks;
using Overview.NPC;
using SharedUI.Interface;
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

    public class AlienNPCController : CreatureController, IInteractable, IHoverable, IBillboardable
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

        [Header("Controls Help & Action Info")]
#if UNITY_EDITOR
        [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
        public int actionId;

        SceneObjectData _sceneObjectData;
        protected AlienNPCState CurrentState;
        protected override void Start()
        {
            base.Start();
            CurrentState = initialState;
        }
        public string GetName()
        {
            return npcDefinition != null ? npcDefinition.characterName : "NPC";
        }
        public Sprite GetIcon()
        {
            return npcDefinition != null ? npcDefinition.characterIcon : null;
        }
        public string ShortBlurb()
        {
            return npcDefinition != null ? npcDefinition.npcDescription : string.Empty;
        }
        public Sprite GetActionIcon()
        {
            // For now, just return a generic talk icon. This can be expanded in the future to return different icons based on the NPC's state or other factors.
            return PlayerUIManager.Instance.defaultIconRepository.talkIcon;
        }
        public string GetActionText()
        {
            // For now, just return "Talk". This can be expanded in the future to return different action texts based on the NPC's state or other factors.
            return "Talk";
        }
        public bool OnHoverStart(GameObject go)
        {
            _sceneObjectData = SceneObjectData.Empty();

            _sceneObjectData.ActionIcon = GetActionIcon();
            _sceneObjectData.ActionText = GetActionText();
            _sceneObjectData.Name = GetName();
            _sceneObjectData.ShortBlurb = ShortBlurb();
            _sceneObjectData.Icon = GetIcon();

            BillboardEvent.Trigger(_sceneObjectData, BillboardEventType.Show);

            if (actionId != 0) ControlsHelpEvent.Trigger(ControlHelpEventType.Show, actionId);

            return true;
        }
        public bool OnHoverStay(GameObject go)
        {
            return true;
        }
        public bool OnHoverEnd(GameObject go)
        {
            if (_sceneObjectData == null) _sceneObjectData = SceneObjectData.Empty();

            BillboardEvent.Trigger(_sceneObjectData, BillboardEventType.Hide);
            if (actionId != 0) ControlsHelpEvent.Trigger(ControlHelpEventType.Hide, actionId);

            return true;
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


#if UNITY_EDITOR
        public IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
        {
            return AllRewiredActions.GetAllRewiredActions();
        }

#endif


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
