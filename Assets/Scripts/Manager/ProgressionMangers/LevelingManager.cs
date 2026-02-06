using System;
using Helpers.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager.ProgressionMangers
{
    public class LevelingManager : MonoBehaviour, ICoreGameService
    {
        [SerializeField] AttributesManager attributesManager;

        [Header("Leveling Stats")] [SerializeField]
        LevelStats[] levelStats;
        [SerializeField] int levelCap = 20;

        [Header("Mutable Max Stats by Upgrade")] [SerializeField]
        HealthAmountByUpgrade[] healthAmountByUpgrade;
        [SerializeField] StaminaAmountByUpgrade[] staminaAmountByUpgrade;
        [SerializeField] ContaminationAmountByUpgrade[] contaminationAmountByUpgrade;

        string _savePath;

        public int CurrentLevel { get; }
        public int CurrentTotalXP { get; }
        public int HealthUpgradeLevel { get; }
        public int StaminaUpgradeLevel { get; }
        public int ContaminationUpgradeLevel { get; }


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
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
        public void Load()
        {
            throw new NotImplementedException();
        }
        public void Reset()
        {
            throw new NotImplementedException();
        }
        public void ConditionalSave()
        {
            throw new NotImplementedException();
        }
        public void MarkDirty()
        {
            throw new NotImplementedException();
        }
        public string GetSaveFilePath()
        {
            throw new NotImplementedException();
        }
        public void CommitCheckpointSave()
        {
            throw new NotImplementedException();
        }
        public bool HasSavedData()
        {
            throw new NotImplementedException();
        }

        public LevelStats GetLevelStats(int level)
        {
            if (level < 1 || level > levelCap)
                throw new ArgumentException("Level must be greater than or equal to 1.", nameof(level));

            foreach (var stats in levelStats)
                if (stats.level == level)
                    return stats;

            throw new Exception($"Level {level} not found in levelStats array.");
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
