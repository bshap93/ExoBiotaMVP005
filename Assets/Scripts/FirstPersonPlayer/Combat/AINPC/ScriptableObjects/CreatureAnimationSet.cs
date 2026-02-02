using UnityEngine;

namespace FirstPersonPlayer.Combat.AINPC.ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "EnemyNPCAnimationSet",
        menuName = "Scriptable Objects/Character/Enemy NPC/Enemy NPC Animation Set",
        order = 0)]
    public class CreatureAnimationSet : ScriptableObject
    {
        [Header("Idle Animations")] public AnimationClip idleAnimation;
        public AnimationClip additionalIdleAnimation0;
        public AnimationClip additionalIdleAnimation1;
        public AnimationClip rangedAttackAnimation;

        [Header("Idle Animation Speed Multipliers")] [Range(0, 3)]
        public int numberOfIdleAnimations = 1;
        public float idleAnimationSpeedMultiplier = 1f;
        public float additionalIdleAnimation0SpeedMultiplier = 1f;
        public float additionalIdleAnimation1SpeedMultiplier = 1f;

        [Header("Movement and Combat Animations")]
        public AnimationClip moveAnimation;
        // public AnimationClip attackAnimation;
        // public AnimationClip additionalAttackAnimation0;
        // public AnimationClip additionalAttackAnimation1;
        public AnimationClip deathAnimation;
        public AnimationClip getHitAnimation;

        [Header("Movement Animation Speed Multiplier")]
        public float moveAnimationSpeedMultiplier = 1f;
        public float deathAnimationSpeedMultiplier = 1f;
        public float getHitAnimationSpeedMultiplier = 1f;
    }
}
