using FirstPersonPlayer.Interactable.Samples;
using Helpers.Events;
using Manager.Global;
using MoreMountains.Tools;
using UnityEngine;

namespace FirstPersonPlayer.UI.Samples
{
    public class BioSamplesIGUILIstView : MonoBehaviour, MMEventListener<BioSampleEvent>
    {
        [SerializeField] Transform listTransform;
        [SerializeField] GameObject samplesListViewElementPrefab;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(BioSampleEvent eventType)
        {
            if (eventType.EventType == BioSampleEventType.CompleteCollection) Refresh();
        }

        public void Refresh()
        {
            var bioMgr = BioSamplesManager.Instance;
            if (bioMgr == null) return;

            foreach (Transform child in listTransform) Destroy(child.gameObject);

            foreach (var sample in bioMgr.GetSamplesCarried())
            {
                var go = Instantiate(samplesListViewElementPrefab, listTransform);
                var element = go.GetComponent<SamplesListViewElement>();
                if (element != null)
                    element.Bind(sample); // <-- Standard row only
            }
        }
    }
}
