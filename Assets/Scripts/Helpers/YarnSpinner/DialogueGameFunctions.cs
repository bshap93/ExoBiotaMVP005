using Manager.ProgressionMangers;
using UnityEngine;
using Yarn.Unity;

namespace Helpers.YarnSpinner
{
    public class DialogueGameFunctions : MonoBehaviour
    {
        [Tooltip("If not assigned, will try DialogueManager.Instance.dialogueRunner")]
        public DialogueRunner dialogueRunner;


        // Scene/ Travel


        // Progression Getters

        [YarnFunction("get_current_class_id")]
        public static int GetUnsetClassStatus()
        {
            if (LevelingManager.Instance != null)
            {
                var playerClassId = LevelingManager.Instance.CurrentPlayerClass.id;
                return playerClassId;
            }

            return 0;
        }

        [YarnFunction("get_current_level")]
        public static int GetCurrentLevel()
        {
            if (LevelingManager.Instance != null) return LevelingManager.Instance.CurrentLevel;

            Debug.LogError("LevelingManager instance is null. Returning 0 for current level.");
            return 0;
        }

        // [YarnFunction("get_stat_upgrade_points_unused")]
        // public static int GetStatUpgradePointsUnused()
        // {
        //     if (LevelingManager.Instance != null) return LevelingManager.Instance.UnspentStatUpgrades;
        //
        //     Debug.LogError("LevelingManager instance is null. Returning 0 for unspent stat upgrades.");
        //     return 0;
        // }

        [YarnFunction("get_attribute_points_unused")]
        public static int GetAttributePointsUnused()
        {
            if (LevelingManager.Instance != null) return LevelingManager.Instance.UnspentAttributePoints;
            Debug.LogError("LevelingManager instance is null. Returning 0 for unspent attribute points.");
            return 0;
        }

        // [YarnFunction("get_current_health_upgrade_level")]
        // public static int GetCurrentHealthUpgradeLevel()
        // {
        //     if (LevelingManager.Instance != null) return LevelingManager.Instance.HealthUpgradeLevel;
        //     Debug.LogError("LevelingManager instance is null. Returning 0 for current health upgrade level.");
        //     return 0;
        // }

        // [YarnFunction("get_current_stamina_upgrade_level")]
        // public static int GetCurrentStaminaUpgradeLevel()
        // {
        //     if (LevelingManager.Instance != null) return LevelingManager.Instance.StaminaUpgradeLevel;
        //     Debug.LogError("LevelingManager instance is null. Returning 0 for current stamina upgrade level.");
        //     return 0;
        // }

        // [YarnFunction("get_current_contamination_upgrade_level")]
        // public static int GetCurrentContaminationUpgradeLevel()
        // {
        //     if (LevelingManager.Instance != null) return LevelingManager.Instance.ContaminationUpgradeLevel;
        //     Debug.LogError("LevelingManager instance is null. Returning 0 for current contamination upgrade level.");
        //     return 0;
        // }


        // Attribute Getters

        [YarnFunction("get_dexterity")]
        public static int GetDexterity()
        {
            return AttributesManager.Instance.Dexterity;
        }

        [YarnFunction("get_agility")]
        public static int GetAgility()
        {
            return AttributesManager.Instance.Agility;
        }


        [YarnFunction("get_strength")]
        public static int GetStrength()
        {
            return AttributesManager.Instance.Strength;
        }


        [YarnFunction("get_biotic_level")]
        public static int GetBioticLevel()
        {
            return AttributesManager.Instance.Exobiotic;
        }

        public class AttributFunctions
        {
        }
    }
}
