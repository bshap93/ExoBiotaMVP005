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
            RangedAttack,
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
        
        public PlayerAttack GetPlayerAttack()
        {
            return hasAttackEffect ? playerAttack : null;
        }

        [Header("Basic Properties")] public string displayName;
        public BioticAbilityType abilityType;
        public UsageType usageType;

        [Header("Contamination Cost")] [ShowIf("usageType", UsageType.SingleUse)]
        public float contaminationCostPerUse;

        [ShowIf("usageType", UsageType.UseWhileHeld)]
        public float contaminationCostPerSecond; // Cost while held

        [Header("Attack Effects")] [SerializeField]
        bool hasAttackEffect;
        [FormerlySerializedAs("attackEffect")] [ShowIf("hasAttackEffect")] [SerializeField]
        PlayerAttack playerAttack;


        public float abilityBaseRange;

        // public GameObject runtimeAbilityPrefab;
    }
}
