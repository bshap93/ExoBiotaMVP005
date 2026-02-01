using FirstPersonPlayer.Tools.ItemObjectTypes;
using MoreMountains.Tools;

namespace Helpers.Events
{
    public enum InnerCoreXPEventType
    {
        ConvertCoreToXP
    }

    public struct OuterCoreXPEvent
    {
        static OuterCoreXPEvent _e;
        public OuterCoreItemObject.CoreObjectValueGrade CoreGrade;
        public InnerCoreXPEventType EventType;
        public static void Trigger(InnerCoreXPEventType eventType,
            OuterCoreItemObject.CoreObjectValueGrade coreGrade)
        {
            _e.EventType = eventType;
            _e.CoreGrade = coreGrade;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
