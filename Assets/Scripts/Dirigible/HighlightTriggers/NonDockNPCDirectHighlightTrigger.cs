using Dirigible.Interactable;
using Helpers.Events;
using HighlightPlus;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dirigible.HighlightTriggers
{
    [RequireComponent(typeof(HighlightEffect))]
    public class NonDockNPCDirectHighlightTrigger : MonoBehaviour
    {
        [SerializeField] HighlightEffect highlightEffect;
        [FormerlySerializedAs("dockInteractable")] [SerializeField]
        DirigibleNonDockNPCInteractable npcNonDockInteractable;

        [SerializeField] MMFeedbacks enterRangeFeedbacks;
        [SerializeField] MMFeedbacks exitRangeFeedbacks;

        [SerializeField] bool alertEnabled = true;

        void Start()
        {
            if (npcNonDockInteractable == null)
                npcNonDockInteractable = GetComponent<DirigibleNonDockNPCInteractable>();

            if (npcNonDockInteractable == null) Debug.LogError("NO Non-Dock NPC component on gameobject");
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                highlightEffect.highlighted = true;

                if (alertEnabled)
                    AlertEvent.Trigger(
                        AlertReason.InRangeOfOverworldNPCDirect,
                        $"In Range of NPC {npcNonDockInteractable.npcDefinition.characterName}. Approach and press E to interact.",
                        "NPC Nearby");

                ControlsHelpEvent.Trigger(
                    ControlHelpEventType.Show,
                    npcNonDockInteractable.actionId);

                enterRangeFeedbacks?.PlayFeedbacks();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                highlightEffect.highlighted = false;
                if (alertEnabled)
                    AlertEvent.Trigger(
                        AlertReason.InRangeOfOverworldNPCDirect,
                        $"Left Range of NPC {npcNonDockInteractable.npcDefinition.characterName}",
                        "Left NPC Contact Range");

                ControlsHelpEvent.Trigger(ControlHelpEventType.Hide, npcNonDockInteractable.actionId);

                exitRangeFeedbacks?.PlayFeedbacks();
            }
        }
    }
}
