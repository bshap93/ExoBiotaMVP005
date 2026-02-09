using DG.Tweening;
using FirstPersonPlayer.Interface;
using HighlightPlus;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Interactable
{
    public class LootboxInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject topPiece;
        [SerializeField] MMFeedbacks openFeedbacks;
        [Header("Rotation Settings")] [SerializeField]
        Vector3 openRotation;
        [SerializeField] Vector3 closedRotation;
        [Header("Position Settings")] [SerializeField]
        Vector3 openPosition;
        [SerializeField] Vector3 closedPosition;
        [Header("Interaction Settings")] [SerializeField]
        float interactionDistance = 3.0f;
        [SerializeField] BoxCollider interactionCollider;
        [SerializeField] HighlightEffect highlightEffect;
        [SerializeField] bool disableHighlightOnOpen = true;

        [Header("Settings")] [SerializeField] float openDuration = 1.0f;

        // Update is called once per frame
        public void Interact()
        {
            openFeedbacks?.PlayFeedbacks();
            if (topPiece != null) topPiece.transform.DORotate(openRotation, openDuration);
            if (topPiece != null) topPiece.transform.DOLocalMove(openPosition, openDuration);
            interactionCollider.enabled = false;
            if (disableHighlightOnOpen) highlightEffect.enabled = false;
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
            return interactionDistance;
        }
    }
}
