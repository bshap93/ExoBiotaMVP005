using System.Collections;
using Animancer;
using FirstPersonPlayer.Combat.AINPC;
using UnityEngine;
using Utilities.Interface;

namespace FirstPersonPlayer.Interactable.BioOrganism.Creatures
{
    public class CessileRangedSpittingCreature : CreatureController, IDamageable
    {
        // Each of the 8 trunk points 0.2m up (in negative X direction) from the Root
        [SerializeField] GameObject[] trunkPoints;

        // The head above all 8 trunk points, root stationary so not referenced
        [SerializeField] GameObject centerOfHead;

        [SerializeField] Transform projectileSpawnPoint;

        [SerializeField] CreaturePoisonAOE poisonAOEPrefab;

        // protected AnimancerState AttackState;
        protected AnimancerState DeathState;

        protected bool IsPlacated;

        public bool IsAttacking { get; private set; }

        protected override void Awake()
        {
            // Pre-load looping animation states
            IdleState = animancerComponent.States.GetOrCreate(creatureType.animationSet.idleAnimation);
            IdleState.Speed = 1f;
            IdleState.Time = 0f;
            IdleState.Events(this).OnEnd = () =>
            {
                IdleState.Time = 0f;
                PlayNextIdle();
            };
        }

        protected void Update()
        {
            if (IsAttacking) return; // Only attacks block everything


            // // Idle should NOT interrupt custom animations
            if (!IsPlayingCustomAnimation && !IdleState.IsPlaying)
                animancerComponent.Play(IdleState, 0.2f);

            if (currentHealth <= 0f && !isDead)
            {
                isDead = true;
                DeathState = animancerComponent.Play(creatureType.animationSet.deathAnimation, 0.1f);
                DeathState.Events(this).OnEnd = () => { Destroy(gameObject); };

                OnDeath();
            }
        }


        public IEnumerator StartAttack(int attackIndex)
        {
            if (IsAttacking) yield break;
            if (attackIndex >= attackInstances.Length) yield break;

            // use of attack instances

            IsAttacking = true;
            IsPlayingCustomAnimation = false;


            FinishAttack(attackIndex);
        }

        void FinishAttack(int attackIndex)
        {
            //
            if (attackIndex >= attackInstances.Length) return;
            // implement ranged spitting attack logic here

            IsAttacking = false;

            // end effect
        }
    }
}
