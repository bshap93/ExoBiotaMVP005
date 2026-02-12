using FirstPersonPlayer.Combat.Player.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstPersonPlayer.ScriptableObjects.BioticAbility
{
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

        [SerializeField] AudioClip injectionOfAbilityFluidClip;

        [Header("Basic Properties")] public string displayName;
        public BioticAbilityType abilityType;
        public UsageType usageType;

        [FormerlySerializedAs("contaminationCostPerUse")]
        [Header("Contamination Cost")]
        [ShowIf("usageType", UsageType.SingleUse)]
        public float baseContaminationCostPerUse;

        [ShowIf("usageType", UsageType.UseWhileHeld)]
        public float contaminationCostPerSecond; // Cost while held

        [Header("Attack Effects")] [SerializeField]
        bool hasAttackEffect;
        [FormerlySerializedAs("attackEffect")] [ShowIf("hasAttackEffect")] [SerializeField]
        PlayerAttack playerAttack;


        public float abilityBaseRange;
        public float bioticReductionFactor = 0.05f;

        public string UniqueID => name; // Using the asset's name as a unique identifier

        public PlayerAttack GetPlayerAttack()
        {
            return hasAttackEffect ? playerAttack : null;
        }

        // public GameObject runtimeAbilityPrefab;
    }
}
