using Helpers.Events.Progression;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Manager.FeedbackControllers
{
    public class ProgressionFeedbackController : MonoBehaviour, MMEventListener<XPEvent>, MMEventListener<LevelingEvent>
    {
        [SerializeField] MMFeedbacks xpGainFeedbacks;
        [SerializeField] MMFeedbacks levelUpFeedbacks;


        void OnEnable()
        {
            this.MMEventStartListening<XPEvent>();
            this.MMEventStartListening<LevelingEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<XPEvent>();
            this.MMEventStopListening<LevelingEvent>();
        }
        public void OnMMEvent(LevelingEvent eventType)
        {
            if (eventType.EventType == LevelingEventType.LevelUp) levelUpFeedbacks?.PlayFeedbacks();
        }
        public void OnMMEvent(XPEvent eventType)
        {
            if (eventType.EventType == XPEventType.AwardXPToPlayer)
                if (!eventType.CausedLevelUp)
                    xpGainFeedbacks?.PlayFeedbacks();
        }
    }
}
