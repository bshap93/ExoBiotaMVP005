using System;
using Animancer;
using MoreMountains.Feedbacks;
using UnityEngine;
using Utilities.Interface;

namespace LevelConstruct.Interactable.Door
{
    public class LockedDoor : MonoBehaviour, IRequiresUniqueID
    {
        public bool isLocked;
        public string uniqueID;


        public string keyID;


        [SerializeField] AnimancerComponent animancerComponent;


        [SerializeField] AnimationClip openAnimation;
        [SerializeField] AnimationClip closeAnimation;
        [SerializeField] AnimationClip openedAnimation;

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

            if (openAnimation != null)
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

            // // // When fully open, idle in opened pose (optional)
            // // if (openedAnimation != null)
            // //     animancerComponent.Play(openedAnimation);
            //
            // isOpen = true;
        }

        public void CloseDoor()
        {
            if (!isOpen) return;

            if (closeAnimation != null)
            {
                closeFeedbacks?.PlayFeedbacks();
                var closeState = animancerComponent.Play(closeAnimation);
                closeState.Events(this).OnEnd = () =>
                {
                    isOpen = false;
                    closeState.Stop();
                };
            }

            // isOpen = false;
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
    }
}
