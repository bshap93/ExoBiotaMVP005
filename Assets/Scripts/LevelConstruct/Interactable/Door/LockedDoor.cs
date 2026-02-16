using System;
using Animancer;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities.Interface;

namespace LevelConstruct.Interactable.Door
{
    public class LockedDoor : MonoBehaviour, IRequiresUniqueID
    {
        public bool isLocked;
        public string uniqueID;


        public string keyID;


        [SerializeField] bool usesAnimationClips = true;
        [ShowIf("usesAnimationClips")] [SerializeField]
        AnimancerComponent animancerComponent;


        [ShowIf("usesAnimationClips")] [SerializeField]
        AnimationClip openAnimation;
        [ShowIf("usesAnimationClips")] [SerializeField]
        AnimationClip closeAnimation;
        [ShowIf("usesAnimationClips")] [SerializeField]
        AnimationClip openedAnimation;

        [SerializeField] bool usesDotWeenForSwing;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        bool doubleDoors = true;

        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        GameObject leftDoor;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        GameObject rightDoor;

        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 leftDoorOpenRotation;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 rightDoorOpenRotation;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 leftDoorCloseRotation;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 rightDoorCloseRotation;

        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 leftDoorOpenPosition;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 rightDoorOpenPosition;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 leftDoorClosePosition;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Vector3 rightDoorClosePosition;

        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        float swingDuration = 1f;
        [ShowIf("usesDotWeenForSwing")] [SerializeField]
        Ease swingEase = Ease.InOutSine;

        [SerializeField] MMFeedbacks openFeedbacks;
        [SerializeField] MMFeedbacks closeFeedbacks;

        public bool isOpen;

        public string UniqueID => uniqueID;
        public void SetUniqueID()
        {
            uniqueID = Guid.NewGuid().ToString();
        }
        public bool IsUniqueIDEmpty()
        {
            return string.IsNullOrEmpty(uniqueID);
        }

        public void ToggleDoor()
        {
            if (isOpen)
                CloseDoor();
            else
                OpenDoor();
        }


        public void OpenDoor()
        {
            if (isOpen) return;

            if (usesAnimationClips && openAnimation != null)
            {
                var openState = animancerComponent.Play(openAnimation);

                openFeedbacks?.PlayFeedbacks();

                openState.Events(this).OnEnd = () =>
                {
                    // When fully open, idle in opened pose (optional)
                    if (openedAnimation != null)
                        animancerComponent.Play(openedAnimation);

                    isOpen = true;
                };
            }
            else if (usesDotWeenForSwing)
            {
                openFeedbacks?.PlayFeedbacks();

                rightDoor.transform.DOLocalRotate(rightDoorOpenRotation, swingDuration).SetEase(swingEase);
                rightDoor.transform.DOLocalMove(rightDoorOpenPosition, swingDuration).SetEase(swingEase);

                if (doubleDoors)
                {
                    leftDoor.transform.DOLocalRotate(leftDoorOpenRotation, swingDuration).SetEase(swingEase);
                    leftDoor.transform.DOLocalMove(leftDoorOpenPosition, swingDuration).SetEase(swingEase);
                }

                isOpen = true;
            }
        }

        public void CloseDoor()
        {
            if (!isOpen) return;

            if (usesAnimationClips && closeAnimation != null)
            {
                closeFeedbacks?.PlayFeedbacks();
                var closeState = animancerComponent.Play(closeAnimation);
                closeState.Events(this).OnEnd = () =>
                {
                    isOpen = false;
                    closeState.Stop();
                };
            }
            else if (usesDotWeenForSwing)
            {
                closeFeedbacks?.PlayFeedbacks();
                rightDoor.transform.DOLocalRotate(rightDoorCloseRotation, swingDuration).SetEase(swingEase);
                rightDoor.transform.DOLocalMove(rightDoorClosePosition, swingDuration).SetEase(swingEase);

                if (doubleDoors)
                {
                    leftDoor.transform.DOLocalRotate(leftDoorCloseRotation, swingDuration).SetEase(swingEase);
                    leftDoor.transform.DOLocalMove(leftDoorClosePosition, swingDuration).SetEase(swingEase);
                }

                isOpen = false;
            }

            // isOpen = false;
        }
    }
}
