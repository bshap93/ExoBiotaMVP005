using Events;
using Lightbug.Utilities;
using Overview.Locations.Anchor;
using UnityEngine;

namespace FirstPersonPlayer.UI.LocationButtonBase.OverviewModeButtonDeriv
{
    public class FabricatorOverviewModeButtons : OverviewModeLocationButtons
    {
        FabricatorLocationAnchor _fabricatorLocationAnchor;

        void OnEnable()
        {
            _fabricatorLocationAnchor = GetComponentInParent<FabricatorLocationAnchor>();

            if (_fabricatorLocationAnchor == null)
                Debug.LogWarning("FabricatorLocationAnchor component not found in parent.");
        }

        public override void Interact()
        {
            _fabricatorLocationAnchor = GetComponentInParent<FabricatorLocationAnchor>();

            var nodeToUse = GetAppropriateStartNode();

            if (_fabricatorLocationAnchor == null)
            {
                Debug.LogError("FabricatorLocationAnchor component not found in parent.");
                return;
            }

            // _cameraAnchorTransform = _fabricatorLocationAnchor.locationCameraAnchor;

            LocationId = _fabricatorLocationAnchor.dockOvLocationDefinition.locationId;

            if (nodeToUse.IsNullOrWhiteSpace())
                OverviewLocationEvent.Trigger(
                    LocationType.NpcResidence, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform, defaultStartNode);
            else
                OverviewLocationEvent.Trigger(
                    LocationType.NpcResidence, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform, nodeToUse);

            HideCanvasGroup();
        }
    }
}
