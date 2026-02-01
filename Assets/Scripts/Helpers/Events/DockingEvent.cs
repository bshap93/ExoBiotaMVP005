using MoreMountains.Tools;
using Overview.OverviewMode.ScriptableObjectDefinitions;

namespace Helpers.Events
{
    public enum DockingEventType
    {
        DockAtLocation,
        Undock,
        SetCurrentDock,
        UnsetCurrentDock,
        FinishedDocking
    }

    public struct DockingEvent
    {
        public DockingEventType EventType;
        public DockDefinition DockDefinition;
        public static DockingEvent _e;

        public static void Trigger(DockingEventType eventType, DockDefinition dockDefinition = null)
        {
            _e.DockDefinition = dockDefinition;
            _e.EventType = eventType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
