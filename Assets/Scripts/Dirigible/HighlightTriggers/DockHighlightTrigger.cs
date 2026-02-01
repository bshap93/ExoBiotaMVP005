using Dirigible.Interactable;
using Helpers.Events;
using HighlightPlus;
using MoreMountains.Feedbacks;
using UnityEngine;
using Utilities.Interface;

namespace Dirigible.HighlightTriggers
{
    [RequireComponent(typeof(HighlightEffect))]
    public class DockHighlightTrigger : MonoBehaviour, IRequiresUniqueID
    {
        [SerializeField] HighlightEffect highlightEffect;
        [SerializeField] DirigibleDockInteractable dockInteractable;

        [SerializeField] MMFeedbacks enterRangeFeedbacks;
        [SerializeField] MMFeedbacks exitRangeFeedbacks;

        [SerializeField] bool alertEnabled = true;

        public bool showDockingInstructions;

        void Start()
        {
            if (dockInteractable == null)
                dockInteractable = GetComponent<DirigibleDockInteractable>();

            if (dockInteractable == null) Debug.LogError("NO dock component on gameobject");
        }

        void OnTriggerEnter(Collider other)
        {
            if (!showDockingInstructions)
            {
                showDockingInstructions = true;
                return;
            }

            if (other.CompareTag("Player"))
            {
                highlightEffect.highlighted = true;
                if (alertEnabled)
                    AlertEvent.Trigger(
                        AlertReason.InRangeOfDockingStation,
                        "In Range of Docking Station. Approach and press E to initiate docking procedure.",
                        "Docking Station Nearby");

                ControlsHelpEvent.Trigger(
                    ControlHelpEventType.Show,
                    dockInteractable.actionId);

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
                        AlertReason.InRangeOfDockingStation,
                        "Left Range of Docking Station.", "Left Docking Station Range");

                ControlsHelpEvent.Trigger(ControlHelpEventType.Hide, dockInteractable.actionId);

                exitRangeFeedbacks?.PlayFeedbacks();
            }
        }
        public string UniqueID => dockInteractable.def.dockId;
        public void SetUniqueID()
        {
            // UniqueID is derived from the dockInteractable, so no action needed here.
        }
        public bool IsUniqueIDEmpty()
        {
            return string.IsNullOrEmpty(UniqueID);
        }
    }
}
