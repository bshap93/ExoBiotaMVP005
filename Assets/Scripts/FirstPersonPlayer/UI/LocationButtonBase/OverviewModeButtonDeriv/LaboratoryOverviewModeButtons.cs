using Events;
using MoreMountains.Feedbacks;
using Overview.Locations;
using UnityEngine;

namespace FirstPersonPlayer.UI.LocationButtonBase.OverviewModeButtonDeriv
{
    public class LaboratoryOverviewModeButtons : OverviewModeLocationButtons
    {
        [SerializeField] MMFeedbacks startInteractionFeedbacks;
        LaboratoryLocationAnchor _laboratoryLocationAnchor;

        public override void Interact()
        {
            _laboratoryLocationAnchor = GetComponentInParent<LaboratoryLocationAnchor>();

            var nodeToUse = GetAppropriateStartNode();

            if (_laboratoryLocationAnchor == null)
            {
                Debug.Log("LaboratoryLocationAnchor not found in parent.");
                return;
            }

            // _cameraAnchorTransform = _laboratoryLocationAnchor.locationCameraAnchor;

            LocationId = _laboratoryLocationAnchor.dockOvLocationDefinition.locationId;

            startInteractionFeedbacks?.PlayFeedbacks();

            if (string.IsNullOrWhiteSpace(nodeToUse))
                OverviewLocationEvent.Trigger(
                    LocationType.Laboratory, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform, defaultStartNode);
            else

                OverviewLocationEvent.Trigger(
                    LocationType.Laboratory, LocationActionType.Approach, LocationId,
                    CameraAnchorTransform, nodeToUse);

            HideCanvasGroup();
        }
    }
}
