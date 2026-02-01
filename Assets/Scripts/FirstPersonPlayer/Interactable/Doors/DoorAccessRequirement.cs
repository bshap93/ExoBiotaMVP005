using FirstPersonPlayer.Interactable.Doors.ScriptableObjects;
using Manager.SceneManagers;
using UnityEngine;

namespace FirstPersonPlayer.Interactable.Doors
{
    public class DoorAccessRequirement : MonoBehaviour
    {
        public DoorDefinition doorDefinition;
        public bool permanentlyUnlockOnOpen;

        public bool CanOpen()
        {
            if (doorDefinition == null) return true;
            if (DoorManager.Instance == null) return true; 
            return DoorManager.Instance.CanOpen(doorDefinition);
        }

        public void MarkOpenedIfPermanent()
        {
            if (permanentlyUnlockOnOpen && doorDefinition != null)
                DoorManager.Instance.PermanentlyUnlock(doorDefinition.doorId);
        }
    }
}