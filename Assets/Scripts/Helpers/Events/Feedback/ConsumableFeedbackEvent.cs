using MoreMountains.Tools;

namespace Helpers.Events.Feedback
{
    public enum ConsumableFeedbackEventType
    {
        InjectableAbilityItemUsed
    }

    public struct ConsumableFeedbackEvent
    {
        static ConsumableFeedbackEvent _e;

        public ConsumableFeedbackEventType FeedbackEventType;
        public string Id;

        public static void Trigger(ConsumableFeedbackEventType feedbackEventType, string id = null)
        {
            _e.FeedbackEventType = feedbackEventType;
            _e.Id = id;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
