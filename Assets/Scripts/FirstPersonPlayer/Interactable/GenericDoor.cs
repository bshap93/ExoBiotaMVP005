using DG.Tweening;
using FirstPersonPlayer.Interface;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Interactable
{
    public class GenericDoor : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")] [SerializeField]
        GameObject doorModel;
        [SerializeField] Vector3 openRotation;
        [SerializeField] Vector3 closedRotation;

        [Header("Feedbacks")] [SerializeField] MMFeedbacks openFeedback;
        [SerializeField] MMFeedbacks closeFeedback;

        [Header("Settings")] [SerializeField] float openCloseDuration = 1f;

        [SerializeField] float distanceToInteract = 3f;

        bool _isOpen;
        public void Interact()
        {
            ToggleDoor();
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
            return distanceToInteract;
        }

        public void ToggleDoor()
        {
            if (_isOpen)
                CloseDoor();
            else
                OpenDoor();
        }

        public void OpenDoor()
        {
            openFeedback?.PlayFeedbacks();
            doorModel.transform.DOLocalRotate(openRotation, openCloseDuration);
            _isOpen = true;
        }

        public void CloseDoor()
        {
            closeFeedback?.PlayFeedbacks();
            doorModel.transform.DOLocalRotate(closedRotation, openCloseDuration);
            _isOpen = false;
        }
    }
}
