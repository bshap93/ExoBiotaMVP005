using System;
using System.Collections;
using Animancer;
using DG.Tweening;
using FirstPersonPlayer.Combat.AINPC;
using FirstPersonPlayer.Combat.AINPC.ScriptableObjects;
using Helpers.Events.NPCs;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace FirstPersonPlayer.Interactable.BioOrganism.Creatures
{
    [RequireComponent(typeof(AssignPlayerToBT))]
    [DisallowMultipleComponent]
    public class EnemyController : CreatureController
    {
        [SerializeField] protected NavMeshAgent navMeshAgent;


        [SerializeField] bool hasRangedAttack;
        [ShowIf("hasRangedAttack")]
        [SerializeField] Transform rangedAttackOrigin;
        // [SerializeField] MMFeedbacks deathFeedbacks;

        [SerializeField] MMFeedbacks movementLoopFeedbacks;

        Tween _hitTween;

        protected AnimancerState AttackState;
        protected AnimancerState DeathState;


        public bool IsAttacking { get; private set; }

        protected override void Awake()
        {
            if (startAsActivated)
                SetupAnimationStates();
            else DeactivateCreature();
        }

        protected void Update()
        {
            if (!IsActivated) return;
            if (IsAttacking) return; // Only attacks block everything

            var speed = navMeshAgent.velocity.magnitude;

            if (speed < 0.1f)
            {
                // Idle should NOT interrupt custom animations
                if (!IsPlayingCustomAnimation && !IdleState.IsPlaying)
                    animancerComponent.Play(IdleState, 0.2f);

                movementLoopFeedbacks?.StopFeedbacks();
            }
            else
            {
                // Move SHOULD interrupt custom animations
                if (!MoveState.IsPlaying)
                {
                    animancerComponent.Play(MoveState, 0.2f);
                    movementLoopFeedbacks?.PlayFeedbacks();
                    IsPlayingCustomAnimation = false; // Reset the flag when interrupted
                }
            }

            if (currentHealth <= 0f && !isDead)
            {
                isDead = true;
                DeathState = animancerComponent.Play(creatureType.animationSet.deathAnimation, 0.1f);
                DeathState.Events(this).OnEnd = () => { Destroy(gameObject); };

                OnDeath();
            }
        }

        public override void ActivateCreature()
        {
            base.ActivateCreature();
            navMeshAgent.enabled = true;
        }

        public override void DeactivateCreature()
        {
            base.DeactivateCreature();
            navMeshAgent.enabled = false;
        }

        public override void OnDeath()
        {
            navMeshAgent.isStopped = true;
            movementLoopFeedbacks?.StopFeedbacks();

            base.OnDeath();
        }


        public IEnumerator StartAttack(int attackIndex)
        {
            if (IsAttacking) yield break;

            if (attackIndex >= attackInstances.Length) yield break;

            var hitboxCollider = attackInstances[attackIndex].attackHitbox;
            var animationClip = attackInstances[attackIndex].attackAnimationClip;
            var animSpeedMult = attackInstances[attackIndex].animationSpeedMultiplier;

            IsAttacking = true;
            IsPlayingCustomAnimation = false;


            AttackState = animancerComponent.Play(animationClip, 0.05f);
            yield return new WaitForSeconds(attackInstances[attackIndex].leadupTime);
            hitboxCollider.Activate();
            AttackState.Speed = animSpeedMult;
            yield return new WaitForSeconds(attackInstances[attackIndex].attackDuration);
            hitboxCollider.Deactivate();

            // wait for attack duration
            AttackState.Events(this).OnEnd = () => { AttackState.Speed = 1f; };
            yield return new WaitForSeconds(creatureType.primaryAttackDuration);
            FinishAttack(attackIndex);
        }

        void FinishAttack(int attackIndex)
        {
            if (attackIndex >= attackInstances.Length) return;
            var hitboxCollider = attackInstances[attackIndex].attackHitbox;
            IsAttacking = false;
            if (hitboxCollider != null)
                hitboxCollider.Deactivate();
        }
        public void OnHitPlayer(Collider other, AttackUsed attackUsed)
        {
            if (other.CompareTag("FirstPersonPlayer"))
                switch (attackUsed)
                {
                    case AttackUsed.Primary:
                        if (attackInstances.Length < 1) return;
                        NPCAttackEvent.Trigger(attackInstances[0].playerAttackData);
                        break;
                    case AttackUsed.Secondary:
                        if (attackInstances.Length < 2) return;
                        NPCAttackEvent.Trigger(attackInstances[1].playerAttackData);
                        break;
                    case AttackUsed.Third:
                        if (attackInstances.Length < 3) return;
                        NPCAttackEvent.Trigger(attackInstances[2].playerAttackData);
                        break;
                }
        }
        public IEnumerator StartDoubleAttack(int attackIndexValue)
        {
            throw new NotImplementedException();
        }
    }
}
