using FirstPersonPlayer.Interface;
using Helpers.Events;
using Inventory;
using LevelConstruct.Interactable.Door;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Interactable.Doors
{
    public class DoorConsole : MonoBehaviour, IInteractable
    {
        [SerializeField] LockedDoor lockedDoor;
        [SerializeField] MMFeedbacks denyEntryFeedbacks;

        public Vector3 switchOnPosition;
        public Vector3 switchOffPosition;

        public Vector3 switchOnRotation;
        public Vector3 switchOffRotation;

        public GameObject switchObject;

        [SerializeField] float interactionDistance = 2f;

        [SerializeField] MMFeedbacks switchFeedbacks;

        void Start()
        {
            // Initialize switch animation based on door state
            AnimateSwitch(!lockedDoor.isLocked);
        }


        public void Interact()
        {
            if (!CanInteract()) return;

            // Toggle door open/close
            lockedDoor.ToggleDoor();

            // Update console switch animation
            AnimateSwitch(!lockedDoor.isOpen);
        }

        public bool CanInteract()
        {
            if (!lockedDoor.isLocked)
                return true;

            if (GlobalInventoryManager.Instance.HasKeyForDoor(lockedDoor.keyID))
            {
                lockedDoor.isLocked = false;
                switchFeedbacks?.PlayFeedbacks();
                return true;
            }

            AlertEvent.Trigger(AlertReason.DoorLocked, "The door is locked. You need a key to open it.");
            denyEntryFeedbacks?.PlayFeedbacks();

            return false;
        }
        public void OnInteractionStart()
        {
        }
        public void OnInteractionEnd(string param)
        {
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
            return interactionDistance;
        }

        void AnimateSwitch(bool isOn)
        {
            if (switchObject == null) return;

            if (isOn)
            {
                switchObject.transform.localPosition = switchOnPosition;
                switchObject.transform.localEulerAngles = switchOnRotation;
            }
            else
            {
                switchObject.transform.localPosition = switchOffPosition;
                switchObject.transform.localEulerAngles = switchOffRotation;
            }
        }
    }
}
