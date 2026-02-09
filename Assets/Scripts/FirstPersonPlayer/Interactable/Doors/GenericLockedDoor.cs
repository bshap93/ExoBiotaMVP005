using Helpers.Events.Machine;
using LevelConstruct.Highlighting;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FirstPersonPlayer.Interactable.Doors
{
    public class GenericLockedDoor : GenericDoor, MMEventListener<DoorEvent>
    {
        [Header("Feedbacks")] [SerializeField] MMFeedbacks lockedDoorWasTriedFeedbacks;
        [SerializeField] MMFeedbacks unlockedDoorFeedbacks;

        [Header("Highlighting")] [SerializeField]
        HighlightEffectController associatedHighlightEffectController;

        [Header("Override Lock State")] [SerializeField]
        bool overrideLockState;
        [ShowIf("overrideLockState")] [SerializeField]
        bool startLocked = true;
        bool _isLocked;
        [SerializeField] Light doorLight;

        void Start()
        {
            if (overrideLockState) _isLocked = startLocked;
            if (_isLocked) associatedHighlightEffectController.SetSecondaryStateHighlightColor();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(DoorEvent eventType)
        {
            if (eventType.UniqueId != uniqueID) return;
            switch (eventType.EventType)
            {
                case DoorEventType.Unlock:
                    _isLocked = false;
                    associatedHighlightEffectController.SetPrimaryStateHighlightColor();
                    unlockedDoorFeedbacks?.PlayFeedbacks();
                    break;
                case DoorEventType.Lock:
                    _isLocked = true;
                    associatedHighlightEffectController.SetSecondaryStateHighlightColor();
                    break;
                case DoorEventType.Open:
                    OpenDoor();
                    break;
                case DoorEventType.Close:
                    CloseDoor();
                    break;
            }
        }

        public override void Interact()
        {
            if (_isLocked)
            {
                lockedDoorWasTriedFeedbacks?.PlayFeedbacks();
                return;
            }

            base.Interact();
        }
    }
}
