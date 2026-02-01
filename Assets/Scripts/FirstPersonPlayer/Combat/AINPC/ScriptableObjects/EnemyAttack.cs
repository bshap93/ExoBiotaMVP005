using System;
using ParadoxNotion.Design;
using UnityEngine;

namespace AINPC.ScriptableObjects
{
    [Serializable]
    public enum NPCAttackType
    {
        Melee,
        Ranged,
        ContaminantPOE
    }

    [Serializable]
    [CreateAssetMenu(
        fileName = "EnemyAttack",
        menuName = "Scriptable Objects/Character/Enemy NPC/Enemy Attack",
        order = 0)]
    public class EnemyAttack : ScriptableObject
    {
        public string displayName;
        [UnityEngine.Header("Attack Properties")]
        public float rawDamage;
        // Amount that an attack ignores armor. 
        // [Range(0f, 1f)] public float armorPenetration;
        public float contaminationAmount;
        [Range(0f, 1f)] public float critChance;
        public float critMultiplier = 1f;
        public float knockbackForce = 1f;
        public bool causesBleeding;
        // showif
        [ShowIf("causesBleeding", 1)] [Range(0f, 1f)]
        public float chanceToCauseBleeding;
        public bool causesStagger;
        // showif
        [ShowIf("causesStagger", 1)] [Range(0f, 1f)]
        public float chanceToCauseStagger;

        public NPCAttackType attackType;
        public string AttackId => name;
    }
}
