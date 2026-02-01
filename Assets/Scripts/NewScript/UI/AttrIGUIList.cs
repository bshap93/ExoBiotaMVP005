using Helpers.Events;
using Helpers.Events.Gated;
using Manager;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace NewScript.UI
{
    public class AttrIGUIList : MonoBehaviour, MMEventListener<GatedLevelingEvent>, MMEventListener<LoadedManagerEvent>
    {
        [SerializeField] TMP_Text strengthText;
        [SerializeField] TMP_Text agilityText;
        [SerializeField] TMP_Text dexterityText;
        [SerializeField] TMP_Text mentalToughnessText;
        [SerializeField] TMP_Text exobioticText;


        void OnEnable()
        {
            this.MMEventStartListening<GatedLevelingEvent>();
            this.MMEventStartListening<LoadedManagerEvent>();
        }
        void OnDisable()
        {
            this.MMEventStopListening<GatedLevelingEvent>();
            this.MMEventStopListening<LoadedManagerEvent>();
        }
        public void OnMMEvent(GatedLevelingEvent eventType)
        {
            if (eventType.EventType == GatedInteractionEventType.CompleteInteraction)
            {
                strengthText.text = eventType.AttributeValues.strength.ToString();
                agilityText.text = eventType.AttributeValues.agility.ToString();
                dexterityText.text = eventType.AttributeValues.dexterity.ToString();
                mentalToughnessText.text = eventType.AttributeValues.mentalToughness.ToString();
                exobioticText.text = eventType.AttributeValues.exobiotic.ToString();
            }
        }
        public void OnMMEvent(LoadedManagerEvent eventType)
        {
            if (eventType.ManagerType == ManagerType.All) Initialize();
        }

        public void Initialize()
        {
            var attrMgr = AttributesManager.Instance;
            strengthText.text = attrMgr.Strength.ToString();
            agilityText.text = attrMgr.Agility.ToString();
            dexterityText.text = attrMgr.Dexterity.ToString();
            mentalToughnessText.text = attrMgr.MentalToughness.ToString();
            exobioticText.text = attrMgr.Exobiotic.ToString();
        }
    }
}
