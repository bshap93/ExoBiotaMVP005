using FirstPersonPlayer.Interactable.Doors;
using FirstPersonPlayer.UI;
using OWPData.ScriptableObjects;
using UnityEngine;

namespace Overview.Locations
{
    public class SpawnLocationButton : MonoBehaviour
    {
        [SerializeField] MineOverviewModeLocation locationBtn;
        [SerializeField] DoorAccessRequirement access;

        // // Called at runtime with the location definition for THIS instance
        // public void Setup(DockOvLocationDefinition def)
        // {
        //     if (def == null)
        //     {
        //         Debug.LogError("SpawnLocationButton.Setup: def is null");
        //         return;
        //     }
        //
        //     if (locationBtn == null) locationBtn = GetComponent<MineOverviewModeLocation>();
        //     if (access == null) access = GetComponent<DoorAccessRequirement>();
        // }

        // If this is wired to a UI OnClick()
        public void OnClick()
        {
            locationBtn?.Interact();
        }
    }
}
