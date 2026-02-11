using System;
using EditorScripts;
using FirstPersonPlayer.Interface;
using Helpers.Events;
using Helpers.Events.Dialog;
using Helpers.Events.Spawn;
using LevelConstruct.Spawn;
using Lightbug.Utilities;
using Manager.DialogueScene;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Interface;

namespace FirstPersonPlayer.Interactable
{
    public class MapShowingHoloInteractable : MonoBehaviour, IInteractable, IRequiresUniqueID
    {
        [ValueDropdown("GetNpcIdOptions")] public
            string npcId;

        [SerializeField] MMFeedbacks startDialogueFeedback;

        [SerializeField] SpawnPoint spawnPoint;

        [SerializeField] string defaultStartNode = "NavigationServerSwitch";

        [SerializeField] string nodeToUse;

        [SerializeField] Sprite mapSprite;

        [SerializeField] string uniqueID;

        [SerializeField] Vector2 positionOnMap;

        [SerializeField] bool showPlayerPosition;
        [SerializeField] bool showAdditionalOverlay;

        [SerializeField] [InlineProperty] [HideLabel]
        SpawnInfoEditor overrideSpawnInfo;

        [ShowIf("showAdditionalOverlay")] [SerializeField]
        Sprite additionalOverlaySprite;
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
            return 3f;
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
