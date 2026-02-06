using DG.Tweening;
using FirstPersonPlayer.Interface;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FirstPersonPlayer.Interactable
{
    public class GenericDoor : MonoBehaviour, IInteractable
    {
        [Header("Rotation Settings")] [SerializeField]
        GameObject doorModel;
        [SerializeField] bool useRotationChange = true;
        [ShowIf("useRotationChange")] [SerializeField]
        Vector3 openRotation;
        [ShowIf("useRotationChange")] [SerializeField]
        Vector3 closedRotation;

        [Header("Position Settings")] [SerializeField]
        bool usePositionChange;
        [ShowIf("usePositionChange")] [SerializeField]
        Vector3 openPosition;
        [ShowIf("usePositionChange")] [SerializeField]
        Vector3 closedPosition;

        [Header("Feedbacks")] [SerializeField] MMFeedbacks openFeedback;
        [SerializeField] MMFeedbacks closeFeedback;

        [Header("Settings")] [SerializeField] float openCloseDuration = 1f;

        [SerializeField] float distanceToInteract = 3f;

        [ShowIf("shouldDisableColliderOnInteraction")] [SerializeField]
        Collider[] interactionCollider;
        [SerializeField] bool shouldDisableColliderOnInteraction = true;

        bool _isOpen;
        public void Interact()
        {
            ToggleDoor();
            if (interactionCollider != null && shouldDisableColliderOnInteraction)
                foreach (var col in interactionCollider)
                    col.enabled = false;
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
            if (useRotationChange)
                doorModel.transform.DOLocalRotate(openRotation, openCloseDuration);

            if (usePositionChange)
                doorModel.transform.DOLocalMove(openPosition, openCloseDuration);

            _isOpen = true;
        }

        public void CloseDoor()
        {
            closeFeedback?.PlayFeedbacks();
            if (useRotationChange)
                doorModel.transform.DOLocalRotate(closedRotation, openCloseDuration);

            if (usePositionChange)
                doorModel.transform.DOLocalMove(closedPosition, openCloseDuration);

            _isOpen = false;
        }
    }
}
