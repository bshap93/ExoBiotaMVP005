using FirstPersonPlayer.Tools.ItemObjectTypes;
using Helpers.Events;
using Helpers.Events.Gated;
using Helpers.Events.Progression;
using Helpers.Interfaces;
using Helpers.StaticHelpers;
using MoreMountains.Tools;
using OWPData.ScriptableObjects;
using SharedUI.Progression;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager
{
    public class AttributesManager : MonoBehaviour, ICoreGameService, MMEventListener<OuterCoreXPEvent>,
        MMEventListener<GatedLevelingEvent>
    {
        const float baseCost = 20f; // cost for first level
        const float growth = 1.4f; // how fast it scales
        public bool autoSave;

        [SerializeField] PlayerStatsSheet playerStatsSheet;

        public bool overrideAttributesOnLoad;
        [ShowIf("overrideAttributesOnLoad")] [SerializeField]
        int overrideStrength = 2;
        [ShowIf("overrideAttributesOnLoad")] [SerializeField]
        int overrideAgility = 2;
        [ShowIf("overrideAttributesOnLoad")] [SerializeField]
        int overrideDexterity = 2;
        [ShowIf("overrideAttributesOnLoad")] [SerializeField]
        int overrideMentalToughness = 2;
        [ShowIf("overrideAttributesOnLoad")] [SerializeField]
        int overrideExobiotic = 2;

        [SerializeField] float staminaPerAgilityIncrease = 5;
        [SerializeField] float staminaPerMentalToughnessIncrease = 2.5f;
        [SerializeField] float healthPerStrengthIncrease = 5;
        [SerializeField] float contaminationResistPerMentalToughnessIncrease = 2.5f;
        [SerializeField] float contaminationResistPerExobioticIncrease = 5f;
        // has endurance and agility's traditional 
        // functions been merged into a single stat...for now
        int _agility;

        int _currentUnusedXP;


        // has perception and dexterity's traditional (and possibly thief)
        // functions been merged into a single stat...for now
        int _dexterity;
        bool _dirty;

        // stat for assimilation of exobiota
        int _exobiotic;
        // has intelligence and charisma's traditional 
        // functions been merged into a single stat...for now
        int _mentalToughness;

        string _savePath;
        // just strength as normal
        int _strength;
        public int CurrentUnusedXP
        {
            get => _currentUnusedXP;
            set
            {
                _currentUnusedXP = value;
                MarkDirty();
            }
        }

        public int Agility
        {
            get => _agility;
            set
            {
                _agility = value;
                MarkDirty();
            }
        }

        public int Dexterity
        {
            get => _dexterity;
            set
            {
                _dexterity = value;
                MarkDirty();
            }
        }

        public int Exobiotic
        {
            get => _exobiotic;
            set
            {
                _exobiotic = value;
                MarkDirty();
            }
        }

        public int MentalToughness
        {
            get => _mentalToughness;
            set
            {
                _mentalToughness = value;
                MarkDirty();
            }
        }


        public int Strength
        {
            get => _strength;
            set
            {
                _strength = value;
                MarkDirty();
            }
        }


        public static AttributesManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                Reset();
                return;
            }

            Load();
        }
        void OnEnable()
        {
            this.MMEventStartListening<OuterCoreXPEvent>();
            this.MMEventStartListening<GatedLevelingEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<OuterCoreXPEvent>();
            this.MMEventStopListening<GatedLevelingEvent>();
        }
        public void Save()
        {
            var path = GetSaveFilePath();
            ES3.Save("Strength", _strength, path);
            ES3.Save("Agility", _agility, path);
            ES3.Save("Dexterity", _dexterity, path);
            ES3.Save("MentalToughness", _mentalToughness, path);
            ES3.Save("Exobiotic", _exobiotic, path);
            ES3.Save("CurrentUnusedXP", _currentUnusedXP, path);


            _dirty = false;
        }
        public void Load()
        {
            if (overrideAttributesOnLoad)
            {
                _strength = overrideStrength;
                _agility = overrideAgility;
                _dexterity = overrideDexterity;
                _mentalToughness = overrideMentalToughness;
                _exobiotic = overrideExobiotic;

                _currentUnusedXP = 0;

                MarkDirty();

                ConditionalSave();

                return;
            }

            var path = GetSaveFilePath();

            if (ES3.KeyExists("Strength", path))
                _strength = ES3.Load<int>("Strength", path);

            if (ES3.KeyExists("Agility", path))
                _agility = ES3.Load<int>("Agility", path);

            if (ES3.KeyExists("Dexterity", path))
                _dexterity = ES3.Load<int>("Dexterity", path);

            if (ES3.KeyExists("MentalToughness", path))
                _mentalToughness = ES3.Load<int>("MentalToughness", path);

            if (ES3.KeyExists("Exobiotic", path))
                _exobiotic = ES3.Load<int>("Exobiotic", path);

            if (ES3.KeyExists("CurrentUnusedXP", path))
                CurrentUnusedXP = ES3.Load<int>("CurrentUnusedXP", path);
        }
        public void Reset()
        {
            _strength = 1;
            _agility = 1;
            _dexterity = 1;
            _mentalToughness = 1;
            _exobiotic = 1;

            _currentUnusedXP = 0;

            MarkDirty();

            ConditionalSave();
        }
        public void ConditionalSave()
        {
            if (autoSave && _dirty) Save();
        }
        public void MarkDirty()
        {
            _dirty = true;
        }

        public string GetSaveFilePath()
        {
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.AttributesSave);
        }
        public void CommitCheckpointSave()
        {
            if (_dirty) Save();
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath ?? GetSaveFilePath());
        }
        public void OnMMEvent(GatedLevelingEvent eventType)
        {
            if (eventType.EventType == GatedInteractionEventType.CompleteInteraction)
            {
                var newAttributeValues = eventType.AttributeValues;

                // If any have increased, call AttributeLevelUpEvent
                if (newAttributeValues.strength > _strength)
                    AttributeLevelUpEvent.Trigger(AttributeType.Strength, newAttributeValues.strength);

                if (newAttributeValues.agility > _agility)
                    AttributeLevelUpEvent.Trigger(AttributeType.Agility, newAttributeValues.agility);

                if (newAttributeValues.dexterity > _dexterity)
                    AttributeLevelUpEvent.Trigger(AttributeType.Dexterity, newAttributeValues.dexterity);

                if (newAttributeValues.mentalToughness > _mentalToughness)
                    AttributeLevelUpEvent.Trigger(AttributeType.MentalToughness, newAttributeValues.mentalToughness);

                if (newAttributeValues.exobiotic > _exobiotic)
                    AttributeLevelUpEvent.Trigger(AttributeType.Exobiotic, newAttributeValues.exobiotic);


                Strength = newAttributeValues.strength;
                Agility = newAttributeValues.agility;
                Dexterity = newAttributeValues.dexterity;
                MentalToughness = newAttributeValues.mentalToughness;
                _exobiotic = newAttributeValues.exobiotic;

                MarkDirty();
            }
        }
        public void OnMMEvent(OuterCoreXPEvent eventType)
        {
            if (eventType.EventType == InnerCoreXPEventType.ConvertCoreToXP)
                ConvertCoreToXP(eventType.CoreGrade);
        }


        public int GetXpRequiredForLevel(int level)
        {
            if (level <= 1)
                return Mathf.RoundToInt(baseCost);

            return Mathf.RoundToInt(baseCost * Mathf.Pow(growth, level - 2));
        }

        void ConvertCoreToXP(
            OuterCoreItemObject.CoreObjectValueGrade coreGrade)
        {
            // remove one core from inventory
            InventoryHelperCommands.RemoveOuterCore(coreGrade);

            // add the XP
            var amount = 0;
            switch (coreGrade)
            {
                case OuterCoreItemObject.CoreObjectValueGrade.StandardGrade:
                    amount = 10;
                    break;
                case OuterCoreItemObject.CoreObjectValueGrade.Radiant:
                    amount = 20;
                    break;
                case OuterCoreItemObject.CoreObjectValueGrade.Stellar:
                    amount = 30;
                    break;
                case OuterCoreItemObject.CoreObjectValueGrade.Unreasonable:
                    amount = 50;
                    break;
                case OuterCoreItemObject.CoreObjectValueGrade.MiscExotic:
                    amount = 0;
                    break;
            }

            _currentUnusedXP += amount;

            XPEvent.Trigger(XPEventType.SetUnusedXP, _currentUnusedXP);

            MarkDirty();
        }

        // public int AmtXPNeededForNextAttributePoint(AttributeType attributeType)
        // {
        //     
        // }
        public int GetXPGainedForCoreGrade(OuterCoreItemObject.CoreObjectValueGrade eventTypeCoreGrade)
        {
            switch (eventTypeCoreGrade)
            {
                case OuterCoreItemObject.CoreObjectValueGrade.StandardGrade:
                    return 10;
                case OuterCoreItemObject.CoreObjectValueGrade.Radiant:
                    return 20;
                case OuterCoreItemObject.CoreObjectValueGrade.Stellar:
                    return 30;
                case OuterCoreItemObject.CoreObjectValueGrade.Unreasonable:
                    return 50;
                case OuterCoreItemObject.CoreObjectValueGrade.MiscExotic:
                    return 0;
                default:
                    return 0;
            }
        }
        public void ApplyPendingAttributeChanges(int pendingNewDexterity, int pendingNewMentalToughness,
            int pendingNewAgility, int pendingNewStrength, int pendingNewExobiotic)
        {
            _dexterity = pendingNewDexterity;
            _mentalToughness = pendingNewMentalToughness;
            _agility = pendingNewAgility;
            _strength = pendingNewStrength;
            _exobiotic = pendingNewExobiotic;

            MarkDirty();
        }
        public void ApplyPendingUnusedXP(int pendingNewUnusedXP)
        {
            _currentUnusedXP = pendingNewUnusedXP;

            MarkDirty();
        }
        public float GetEffectiveTimeCostMultiplier(GatedInteractionType interactionType)
        {
            switch (interactionType)
            {
                case GatedInteractionType.BreakObstacle:
                    return 1.0f - Strength * 0.05f;
                case GatedInteractionType.HarvesteableBiological:
                    return 1.0f - Dexterity * 0.05f;
                case GatedInteractionType.InteractMachine:
                    return 1.0f - Dexterity * 0.05f;
                case GatedInteractionType.NotGated:
                    return 1.0f - Dexterity * 0.05f;
                case GatedInteractionType.Rest:
                    return 1.0f - MentalToughness * 0.05f;
                default:
                    return 1.0f;
            }
        }
        public float GetStatusEffectSeverityMultiplier(string effectID)
        {
            // higher mental toughness reduces severity of status effects
            return 1.0f - MentalToughness * 0.05f;
        }
        public float GetStaminaPerAgilityIncrease()
        {
            return staminaPerAgilityIncrease;
        }
        public float GetStaminaPerMentalToughnessIncrease()
        {
            return staminaPerMentalToughnessIncrease;
        }

        public float GetHealthPerStrengthIncrease()
        {
            return healthPerStrengthIncrease;
        }
        public float GetContaminationResistPerMentalToughnessIncrease()
        {
            return contaminationResistPerMentalToughnessIncrease;
        }
        public float GetContaminationResistPerExobioticIncrease()
        {
            return contaminationResistPerExobioticIncrease;
        }
    }
}
