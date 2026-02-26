using System;
using FirstPersonPlayer.Combat.Player.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstPersonPlayer.ScriptableObjects.BioticAbility
{
    [Serializable]
    public class ContaminationCostsByExobioticLevel
    {
        [Range(1,20)] public int ExobioticLevel = 1;
        public float ContaminationCost;
    }
    [CreateAssetMenu(
        fileName = "PlayerBioticAbility_",
        menuName = "Scriptable Objects/Character/First Person Player/Player Biotic Ability",
        order = 0)]
    public class BioticAbility : ScriptableObject
    {
        public enum BioticAbilityType
        {
            RangedHitscanAttack,
            RangedSlowProjectileAttack,
            AreaOfEffectAttack,
            RangedEffect,
            InstantiateObject,
            Passive
        }



        public enum UsageType
        {
            SingleUse,
            UseWhileHeld
        }
        
        [FormerlySerializedAs("ContaminationCostsByExobioticLevel")] [ShowIf("usageType", UsageType.SingleUse)]
        public ContaminationCostsByExobioticLevel[] contaminationCostsByExobioticLevel;
        
        public float GetContaminationCostForExobioticLevel(int exobioticLevel)
        {
            foreach (var entry in contaminationCostsByExobioticLevel)
            {
                if (entry.ExobioticLevel == exobioticLevel)
                    return entry.ContaminationCost;
            }
            Debug.LogWarning($"No contamination cost found for exobiotic level {exobioticLevel}. Returning 0.");
            return 0f; // Default cost if not found
        }

        [SerializeField] AudioClip injectionOfAbilityFluidClip;

        [Header("Basic Properties")] public string displayName;
        public BioticAbilityType abilityType;
        public UsageType usageType;



        [ShowIf("usageType", UsageType.UseWhileHeld)]
        public float contaminationCostPerSecond; // Cost while held

        [Header("Attack Effects")] [SerializeField]
        bool hasAttackEffect;
        [FormerlySerializedAs("attackEffect")] [ShowIf("hasAttackEffect")] [SerializeField]
        PlayerAttack playerAttack;


        public float abilityBaseRange;
        public float bioticReductionFactor = 0.05f;

        [FormerlySerializedAs("cooldownTime")] public float baseCooldownTime = 1f; // Cooldown time in seconds

        public string UniqueID => name; // Using the asset's name as a unique identifier

        public PlayerAttack GetPlayerAttack()
        {
            return hasAttackEffect ? playerAttack : null;
        }

        // public GameObject runtimeAbilityPrefab;
    }
}
