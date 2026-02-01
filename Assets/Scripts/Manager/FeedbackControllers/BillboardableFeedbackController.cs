using Helpers.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager.FeedbackControllers
{
    public class BillboardableFeedbackController : MonoBehaviour, MMEventListener<BillboardEvent>
    {
        [FormerlySerializedAs("OnBillboardEnableFeedbacks")]
        public MMFeedbacks onBillboardEnableFeedbacks;
        void OnEnable()
        {
            this.MMEventStartListening();
        }


        void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(BillboardEvent eventType)
        {
            if (eventType.EventType == BillboardEventType.Show) onBillboardEnableFeedbacks?.PlayFeedbacks();
        }
    }
}
