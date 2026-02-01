using Manager.StateManager;
using MoreMountains.Tools;

namespace Helpers.Events.NPCs
{
    public enum CreatureStateEventType
    {
        SetNewCreatureState
    }

    public struct CreatureStateEvent
    {
        static CreatureStateEvent _e;

        public CreatureStateEventType EventType;
        public string UniqueID;
        public CreatureStateManager.CreatureState CreatureState;

        public static void Trigger(CreatureStateEventType eventType, string uniqueID,
            CreatureStateManager.CreatureState creatureState)
        {
            _e.EventType = eventType;
            _e.UniqueID = uniqueID;
            _e.CreatureState = creatureState;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
