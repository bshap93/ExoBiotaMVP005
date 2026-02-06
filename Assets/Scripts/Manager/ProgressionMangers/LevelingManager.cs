using System;
using FirstPersonPlayer.Tools.ItemObjectTypes;
using Helpers.Events;
using Helpers.Events.Progression;
using Helpers.Interfaces;
using Helpers.StaticHelpers;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager.ProgressionMangers
{
    public class LevelingManager : MonoBehaviour, ICoreGameService,
        MMEventListener<BioticCoreXPConversionEvent>, MMEventListener<EnemyXPRewardEvent>
    {
        [SerializeField] AttributesManager attributesManager;

        [Header("Leveling Stats")] [SerializeField]
        LevelStats[] levelStats;
        [SerializeField] int levelCap = 20;

        [Header("Mutable Max Stats by Upgrade")] [SerializeField]
        HealthAmountByUpgrade[] healthAmountByUpgrade;
        [SerializeField] StaminaAmountByUpgrade[] staminaAmountByUpgrade;
        [SerializeField] ContaminationAmountByUpgrade[] contaminationAmountByUpgrade;
        [SerializeField] bool autoSave;

        [SerializeField] int attributePointsStartWith = 7;
        bool _dirty;

        string _savePath;

        int _unspentAttribuePoints;
        int _unspentStatUpgrades;

        public int UnspentAttributePoints
        {
            get => _unspentAttribuePoints;
            set
            {
                _unspentAttribuePoints = value;
                MarkDirty();
            }
        }

        public int UnspentStatUpgrades
        {
            get => _unspentStatUpgrades;
            set
            {
                _unspentStatUpgrades = value;
                MarkDirty();
            }
        }

        public int CurrentLevel { get; set; }
        public int CurrentTotalXP { get; set; }
        public int HealthUpgradeLevel { get; set; }
        public int StaminaUpgradeLevel { get; set; }
        public int ContaminationUpgradeLevel { get; set; }


        public int TotalXpNeededForNextLevel
        {
            get
            {
                if (CurrentLevel >= levelCap)
                    return 0; // No more XP needed if at or above level cap

                return GetLevelStats(CurrentLevel + 1).totalXPRequired;
            }
        }
        public int MoreXpNeededForNextLevel => TotalXpNeededForNextLevel - CurrentTotalXP;

        public static LevelingManager Instance { get; private set; }

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
            this.MMEventStartListening<BioticCoreXPConversionEvent>();
            this.MMEventStartListening<EnemyXPRewardEvent>();
        }
        void OnDisable()
        {
            this.MMEventStopListening<BioticCoreXPConversionEvent>();
            this.MMEventStopListening<EnemyXPRewardEvent>();
        }

        public void Save()
        {
            var path = GetSaveFilePath();

            ES3.Save("CurrentLevel", CurrentLevel, path);
            ES3.Save("CurrentTotalXP", CurrentTotalXP, path);
            ES3.Save("HealthUpgradeLevel", HealthUpgradeLevel, path);
            ES3.Save("StaminaUpgradeLevel", StaminaUpgradeLevel, path);
            ES3.Save("ContaminationUpgradeLevel", ContaminationUpgradeLevel, path);
            ES3.Save("UnspentAttributePoints", UnspentAttributePoints, path);
            ES3.Save("UnspentStatUpgrades", UnspentStatUpgrades, path);
            _dirty = false;
        }
        public void Load()
        {
            var path = GetSaveFilePath();

            if (ES3.KeyExists("CurrentLevel", path))
                CurrentLevel = ES3.Load<int>("CurrentLevel", path);

            if (ES3.KeyExists("CurrentTotalXP", path))
                CurrentTotalXP = ES3.Load<int>("CurrentTotalXP", path);

            if (ES3.KeyExists("HealthUpgradeLevel", path))
                HealthUpgradeLevel = ES3.Load<int>("HealthUpgradeLevel", path);

            if (ES3.KeyExists("StaminaUpgradeLevel", path))
                StaminaUpgradeLevel = ES3.Load<int>("StaminaUpgradeLevel", path);


            if (ES3.KeyExists("ContaminationUpgradeLevel", path))
                ContaminationUpgradeLevel = ES3.Load<int>("ContaminationUpgradeLevel", path);

            if (ES3.KeyExists("UnspentAttributePoints", path))
                UnspentAttributePoints = ES3.Load<int>("UnspentAttributePoints", path);

            if (ES3.KeyExists("UnspentStatUpgrades", path))
                UnspentStatUpgrades = ES3.Load<int>("UnspentStatUpgrades", path);
        }
        public void Reset()
        {
            CurrentLevel = 1;
            CurrentTotalXP = 0;
            HealthUpgradeLevel = 1;
            StaminaUpgradeLevel = 1;
            ContaminationUpgradeLevel = 1;
            UnspentAttributePoints = attributePointsStartWith;
            UnspentStatUpgrades = 0;
            MarkDirty();
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
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.LevelingSave);
        }
        public void CommitCheckpointSave()
        {
            if (_dirty) Save();
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath ?? GetSaveFilePath());
        }

        public void OnMMEvent(BioticCoreXPConversionEvent conversionEventType)
        {
            if (conversionEventType.EventType == BioticCoreXPEventType.ConvertCoreToXP)
                ConvertCoreToXP(conversionEventType.CoreGrade);
        }

        public void OnMMEvent(EnemyXPRewardEvent eventType)
        {
            throw new NotImplementedException();
        }
        // public void OnMMEvent(XPEvent eventType)
        // {
        //     if (eventType.EventType == XPEventType.AwardXPToPlayer)
        //         AwardXPToPlayer(eventType.Amount);
        // }

        /// <summary>
        ///     Adds to total XP, and triggers level up if earned.
        /// </summary>
        /// <param name="xpToAward"></param>
        void AwardXPToPlayer(int xpToAward)
        {
            CurrentTotalXP += xpToAward;

            // Check for level up
            while (CurrentLevel < levelCap && CurrentTotalXP >= TotalXpNeededForNextLevel)
            {
                var newLevel = CurrentLevel + 1;
                LevelUpPlayer(newLevel);
            }

            XPEvent.Trigger(XPEventType.AwardXPToPlayer, xpToAward);

            ProgressionUpdateListenerNotifier.Trigger(
                CurrentTotalXP, CurrentLevel, UnspentStatUpgrades,
                UnspentAttributePoints);
        }

        /// <summary>
        /// </summary>
        /// <param name="newLevel"></param>
        void LevelUpPlayer(int newLevel)
        {
            if (newLevel != CurrentLevel + 1)
                throw new ArgumentException("New level must be exactly one greater than current level.");

            CurrentLevel = newLevel;
            AwardStatUpgradeToPlayer(newLevel);
            AwardAttributePointsToPlayer(newLevel);

            LevelingEvent.Trigger(LevelingEventType.LevelUp, newLevel);
        }

        /// <summary>
        ///     For now, awards one upgrade per level.
        /// </summary>
        /// <param name="level"></param>
        void AwardStatUpgradeToPlayer(int level)
        {
            UnspentStatUpgrades += 1;
        }

        /// <summary>
        ///     If the level reached grants attribute points, adds them.
        /// </summary>
        /// <param name="level"></param>
        void AwardAttributePointsToPlayer(int level)
        {
            var stats = GetLevelStats(level);
            if (stats.attributePointsGranted > 0)
                UnspentAttributePoints += stats.attributePointsGranted;
        }

        LevelStats GetLevelStats(int level)
        {
            if (level < 1 || level > levelCap)
                throw new ArgumentException("Level must be greater than or equal to 1.", nameof(level));

            foreach (var stats in levelStats)
                if (stats.level == level)
                    return stats;

            throw new Exception($"Level {level} not found in levelStats array.");
        }

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

        public void ConvertCoreToXP(
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


            AwardXPToPlayer(amount);

            MarkDirty();
        }

        [Serializable]
        public class LevelStats
        {
            [FormerlySerializedAs("Level")] public int level;
            [FormerlySerializedAs("TotalXPRequired")]
            public int totalXPRequired;
            [FormerlySerializedAs("AttributePointsGranted")]
            public int attributePointsGranted;
        }

        [Serializable]
        public class HealthAmountByUpgrade
        {
            [FormerlySerializedAs("UpgradeLevel")] public int upgradeLevel;
            [FormerlySerializedAs("HealthAmount")] public float healthAmount;
        }

        [Serializable]
        public class StaminaAmountByUpgrade
        {
            public int upgradeLevel;
            public float staminaAmount;
        }

        [Serializable]
        public class ContaminationAmountByUpgrade
        {
            public int upgradeLevel;
            public float contaminationAmount;
        }
    }
}
