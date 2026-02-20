using Helpers.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Dirigible
{
    public class DirigiblePhysicalObject : MonoBehaviour, MMEventListener<DockingEvent>
    {
        public new Rigidbody rigidbody;

        public GameObject dockingGear;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(DockingEvent eventType)
        {
            if (eventType.EventType == DockingEventType.Undock) dockingGear.SetActive(false);

            if (eventType.EventType == DockingEventType.DockAtLocation) dockingGear.SetActive(true);
        }
    }
}
