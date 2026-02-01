using DG.Tweening;
using FirstPersonPlayer.Interface;
using MoreMountains.Feedbacks;
using UnityEngine;

public class LootboxInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject topPiece;
    [SerializeField] MMFeedbacks openFeedbacks;
    [SerializeField] Vector3 openRotation;
    [SerializeField] Vector3 closedRotation;
    [SerializeField] float interactionDistance = 3.0f;

    [Header("Settings")] [SerializeField] float openDuration = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Interact()
    {
        openFeedbacks?.PlayFeedbacks();
        if (topPiece != null) topPiece.transform.DORotate(openRotation, openDuration);
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
