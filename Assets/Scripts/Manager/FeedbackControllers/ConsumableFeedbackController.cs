using Helpers.Events.Feedback;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Manager.FeedbackControllers
{
    public class ConsumableFeedbackController : MonoBehaviour, MMEventListener<ConsumableFeedbackEvent>
    {
        [SerializeField] MMFeedbacks injectableAbilityItemUsedFeedback;
        [SerializeField] MMSoundManagerSound injectableAbilityItemUsedSound;
        void OnEnable()
        {
            this.MMEventStartListening();
        }
        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(ConsumableFeedbackEvent eventType)
        {
            if (eventType.FeedbackEventType == ConsumableFeedbackEventType.InjectableAbilityItemUsed)
            {
                injectableAbilityItemUsedFeedback?.PlayFeedbacks();
                Debug.Log(eventType.FeedbackEventType);
            }
        }
    }
}
