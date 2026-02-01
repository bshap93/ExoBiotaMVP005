namespace FirstPersonPlayer.Interface
{
    public interface IInteractable
    {
        void Interact();
        void OnInteractionStart();
        void OnInteractionEnd(string param);
        bool CanInteract();

        bool IsInteractable();

        void OnFocus();

        void OnUnfocus();

        float GetInteractionDistance();
    }
}
