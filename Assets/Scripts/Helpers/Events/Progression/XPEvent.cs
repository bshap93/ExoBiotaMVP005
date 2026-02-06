using MoreMountains.Tools;

namespace Helpers.Events.Progression
{
    public enum XPEventType
    {
        AwardXPToPlayer
    }

    public struct XPEvent
    {
        static XPEvent _e;
        public int Amount;
        public XPEventType EventType;
        public static void Trigger(XPEventType type, int amount)
        {
            _e.Amount = amount;
            _e.EventType = type;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
