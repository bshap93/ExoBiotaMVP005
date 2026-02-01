using Events;
using Overview.Locations;
using UnityEngine;

namespace FirstPersonPlayer.UI.LocationButtonBase
{
    public class TestOverviewModeButtons : OverviewModeLocationButtons
    {
        private TestLocationAnchor _testLocationAnchor;

        public override void Interact()
        {
            _testLocationAnchor = GetComponentInParent<TestLocationAnchor>();

            if (_testLocationAnchor == null)
            {
                Debug.Log("Location anchor not found in parent.");
                return;
            }

            CameraAnchorTransform = _testLocationAnchor.locationCameraAnchor;
            LocationId = _testLocationAnchor.dockOvLocationDefinition.locationId;

            OverviewLocationEvent.Trigger(LocationType.MiscNpc, LocationActionType.Approach, LocationId,
                CameraAnchorTransform);

            HideCanvasGroup();
        }
    }
}