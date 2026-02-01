using Helpers.Events;
using MoreMountains.Tools;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using UnityEngine;

namespace Overview.UI
{
    public class DockingCanvasController : MonoBehaviour, MMEventListener<DockingEvent>
    {
        [SerializeField] CanvasGroup overviewCanvasGroup;


        public void OnEnable()
        {
            this.MMEventStartListening();
        }

        public void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(DockingEvent eventType)
        {
            if (eventType.EventType == DockingEventType.DockAtLocation)
                ShowOverviewCanvas(eventType.DockDefinition);
            else if (eventType.EventType == DockingEventType.Undock) HideOverviewCanvas();
        }

        void ShowOverviewCanvas(DockDefinition eventTypeDockDefinition)
        {
            if (overviewCanvasGroup == null)
            {
                Debug.LogError("Overview Canvas Group is not assigned.");
                return;
            }

            overviewCanvasGroup.alpha = 1f;
            overviewCanvasGroup.interactable = true;
            overviewCanvasGroup.blocksRaycasts = true;

            // Additional logic to set up the overview canvas based on the dock definition
            // For example, you might want to update UI elements or load specific content
            Debug.Log($"Showing overview canvas for dock: {eventTypeDockDefinition.name}");
        }

        void HideOverviewCanvas()
        {
            if (overviewCanvasGroup == null)
            {
                Debug.LogError("Overview Canvas Group is not assigned.");
                return;
            }

            overviewCanvasGroup.alpha = 0f;
            overviewCanvasGroup.interactable = false;
            overviewCanvasGroup.blocksRaycasts = false;

            // Additional logic to clean up the overview canvas if necessary
            Debug.Log("Hiding overview canvas.");
        }
    }
}
