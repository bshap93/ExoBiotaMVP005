using LevelConstruct.Highlighting;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FirstPersonPlayer.Interactable.Doors
{
    public class GenericLockedDoor : GenericDoor
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

        void Start()
        {
            if (overrideLockState) _isLocked = startLocked;
            if (_isLocked) associatedHighlightEffectController.SetSecondaryStateHighlightColor();
        }

        public override void Interact()
        {
            if (_isLocked) return;
            base.Interact();
        }
    }
}
