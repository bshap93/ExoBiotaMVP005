using Events;
using Lightbug.Utilities;
using Overview.Locations;
using UnityEngine;

namespace FirstPersonPlayer.UI.LocationButtonBase.OverviewModeButtonDeriv
{
    public class TraderOverviewModeButtons : OverviewModeLocationButtons
    {
        TraderStationLocationAnchor _traderStationLocationAnchor;


        public override void Interact()
        {
            _traderStationLocationAnchor = GetComponentInParent<TraderStationLocationAnchor>();

            var nodeToUse = GetAppropriateStartNode();

            if (_traderStationLocationAnchor == null)
            {
                Debug.LogError("TraderStationLocationAnchor component not found in parent hierarchy.");
                return;
            }

            // Move camera to the trader station anchor
            // _cameraAnchorTransform = _traderStationLocationAnchor.locationCameraAnchor;

            LocationId = _traderStationLocationAnchor.dockOvLocationDefinition.locationId;

            if (nodeToUse.IsNullOrWhiteSpace())
                OverviewLocationEvent.Trigger(
                    LocationType.Trader, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform);
            else
                OverviewLocationEvent.Trigger(
                    LocationType.Trader, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform, nodeToUse);


            // OverviewLocationEvent.Trigger(
            //     LocationType.Trader, LocationActionType.Approach, LocationId,
            //     CameraAnchorTransform);

            HideCanvasGroup();
        }
    }
}
