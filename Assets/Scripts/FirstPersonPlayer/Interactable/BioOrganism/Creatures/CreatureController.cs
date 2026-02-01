using System;
using System.Collections;
using AINPC.ScriptableObjects;
using Animancer;
using DG.Tweening;
using FirstPersonPlayer.Combat.AINPC;
using FirstPersonPlayer.Combat.AINPC.ScriptableObjects;
using FirstPersonPlayer.Combat.Player.ScriptableObjects;
using Helpers.Events;
using Helpers.Events.Combat;
using Helpers.Events.NPCs;
using Helpers.Events.Status;
using HighlightPlus;
using Manager;
using Manager.StateManager;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities.Interface;
using Random = UnityEngine.Random;

namespace FirstPersonPlayer.Interactable.BioOrganism.Creatures
{
    [RequireComponent(typeof(Blackboard))]
    [DisallowMultipleComponent]
    public abstract class CreatureController : MonoBehaviour, IRequiresUniqueID, MMEventListener<PauseEvent>,
        IDamageable
    {
        public enum CreatureState
        {
            Normal,
            Stunned,
            Dead
        }


        public bool isDead;

        public string uniqueID;

        [SerializeField] protected Blackboard blackboard;
        [SerializeField] protected AnimancerComponent animancerComponent;
        [FormerlySerializedAs("owner")] [SerializeField]
        FSMOwner fsmOwner;
        [SerializeField] GameObject feedbacksContainer;
        [SerializeField] public CreatureType creatureType;
        public CreatureStateManager.CreatureState initialCreatureState;
        [Header("Feedbacks")] [SerializeField] protected MMFeedbacks deathFeedbacks;

        [SerializeField] protected MMFeedbacks critDamageFeedbacks;
        [Header("Basic Hit Feedbacks")] [SerializeField]
        protected MMFeedbacks meleeHitFeedbacksBasic;
        [SerializeField] protected MMFeedbacks rangedHitFeedbacksBasic;
        [Header("Heavy Hit Feedbacks")] [SerializeField]
        protected MMFeedbacks meleeHitFeedbacksHeavy;
        [SerializeField] protected MMFeedbacks rangedHitFeedbacksHeavy;

        [Header("Settings")] [SerializeField] protected float secondsBeforeSettingShouldBeDestroyed = 5f;


        public float currentHealth;
        [Header("Attacks")] public int attackCount = 1;

        public AttackInstance[] attackInstances;

        public float currentStunDamage;


        [SerializeField] protected HighlightEffect highlightEffect;

        [Header("Flags")] public bool startAsActivated;
        public bool isStunned;
        public bool destroyAfterDeath = true;
        public bool doesNotImmediatelyNeedToMove;

        public bool appearsOnlyOnce;


        int _currentIdleIndex;

        Tween _hitTween;

        bool _isPaused;


        Coroutine _stunDecayCoroutine;

        protected AnimancerState HitState;

        protected AnimancerState IdleState;

        protected bool IsActivated;
        protected AnimancerState MoveState;

        public CreatureState CurrentCreatureState
        {
            get
            {
                if (currentHealth <= 0f) return CreatureState.Dead;
                if (isStunned) return CreatureState.Stunned;
                return CreatureState.Normal;
            }
        }

        public float StunDuration => creatureType.stunCooldownTime;
        public bool IsPlayingCustomAnimation { get; set; }

        public float MaxHealth => creatureType.maxHealth;
        public float StunThreshold => creatureType.stunTreshold;

        protected virtual void Awake()
        {
            if (startAsActivated)
                SetupAnimationStates();
            else DeactivateCreature();
        }

        protected virtual void Start()
        {
            StartCoroutine(InitializeAfterCreatureStateManager());
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public virtual void PlayHitAnimation(AnimationClip value)
        {
            HitState = animancerComponent.Play(value, 0.05f);

            HitState.Events(this).OnEnd = () => { };
        }

        public virtual void OnDeath()
        {
            var lootDef = creatureType.lootDefinition;

            // Instantiate loot based on loot definition
            if (lootDef != null)
            {
                var loot = lootDef.GetLoot();
                // A bit above the current position
                var position = transform.position + Vector3.up * 0.5f;
                if (loot != null) Instantiate(loot, transform.position, Quaternion.identity);
            }

            ResetStunState();

            CreatureStateEvent.Trigger(
                CreatureStateEventType.SetNewCreatureState, uniqueID,
                CreatureStateManager.CreatureState.ShouldBeDestroyed);


            EnemyDamageEvent.Trigger(
                0f, currentHealth, MaxHealth,
                DamageEventType.Death, creatureType.creatureName, DamageType.None);

            deathFeedbacks?.PlayFeedbacks();
            // if (deathParticlesPrefab != null)
            //     Instantiate(deathParticlesPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

            if (destroyAfterDeath)
                Destroy(gameObject, 2f);
        }
        public void ProcessAttackDamage(PlayerAttack playerAttack, Vector3 attackOrigin)
        {
            var attributeManager = AttributesManager.Instance;
            var damageAmount = playerAttack.rawDamage;
            var stunAmount = playerAttack.rawStunDamage;
            var attackType = playerAttack.attackType;
            var rawKnockbackForce = playerAttack.rawKnockbackForce;
            var creatureWeaknessToKnockback = creatureType.weaknessToKnockback;
            var knockbackForce = rawKnockbackForce * creatureWeaknessToKnockback;

            var isCriticalHit = Random.value <= playerAttack.critChance;

            if (attackType == PlayerAttackType.Melee)
            {
                // Placeholder for strength stat for player
                var playerStrength = attributeManager.Strength;
                // Provisional damage scaling based on player strength
                var playerStrengthMultiplier = 1f + (playerStrength - 1) * 0.5f;
                var knockbackForceWithStrength = knockbackForce * playerStrengthMultiplier;

                var knockDir = transform.position - attackOrigin;
                knockDir.y = 0f; // REMOVE vertical force
                knockDir.Normalize();

                transform.DOPunchPosition(
                    knockDir * knockbackForceWithStrength,
                    0.3f,
                    6,
                    0.5f
                );

                if (isCriticalHit)
                {
                    damageAmount *= playerAttack.critMultiplier;
                    stunAmount *= playerAttack.critMultiplier;
                    critDamageFeedbacks?.PlayFeedbacks();
                }

                damageAmount *= playerStrengthMultiplier;
                stunAmount *= playerStrengthMultiplier;

                // Melee contamination
                var contaminationAmt = creatureType.baseBlowbackContaminationAmt *
                                       playerAttack.baseBlowbackContaminationMultiplier;

                PlayerStatsEvent.Trigger(
                    PlayerStatsEvent.PlayerStat.CurrentContamination,
                    PlayerStatsEvent.PlayerStatChangeType.Increase,
                    contaminationAmt);

                // StartCoroutine(CooldownAfterWasHit());

                if (playerAttack.hitType == HitType.Normal)
                {
                    meleeHitFeedbacksBasic?.PlayFeedbacks();
                    PlayHitTween(t => t.DOPunchPosition(
                        new Vector3(creatureType.meleeAttackShakeIntensity, 0f, creatureType.meleeAttackShakeIntensity),
                        creatureType.meleeAttackShakeDuration));

                    blackboard.SetVariableValue("wasHit", true);
                    Debug.Log("Normal Hit registered on " + creatureType.creatureName);
                }
                else if (playerAttack.hitType == HitType.Heavy)
                {
                    meleeHitFeedbacksHeavy?.PlayFeedbacks();
                    blackboard.SetVariableValue("wasHitHeavy", true);
                    Debug.Log("Heavy Hit registered on " + creatureType.creatureName);
                    PlayHitTween(t => t.DOShakePosition(
                        creatureType.meleeAttackShakeDuration,
                        new Vector3(
                            creatureType.heavyMeleeAttackShakeIntensity, 0f,
                            creatureType.heavyMeleeAttackShakeIntensity))); // or still punch
                }
            }
            else if (attackType == PlayerAttackType.Ranged)
            {
                var playerDexterity = attributeManager.Dexterity;
                var playerDexterityMultiplier = 1f + (playerDexterity - 1) * 0.5f;
                if (isCriticalHit)
                {
                    damageAmount *= playerAttack.critMultiplier;
                    stunAmount *= playerAttack.critMultiplier;
                    critDamageFeedbacks?.PlayFeedbacks();
                }

                damageAmount *= playerDexterityMultiplier;
                stunAmount *= playerDexterityMultiplier;

                if (playerAttack.hitType == HitType.Normal)
                {
                    rangedHitFeedbacksBasic?.PlayFeedbacks();
                    PlayHitTween(t => t.DOPunchPosition(
                        new Vector3(
                            creatureType.rangedAttackShakeIntensity, 0f, creatureType.rangedAttackShakeIntensity),
                        creatureType.rangedAttackShakeDuration));

                    blackboard.SetVariableValue("wasHit", true);
                    Debug.Log("Ranged Normal Hit registered on " + creatureType.creatureName);
                }
                else if (playerAttack.hitType == HitType.Heavy)
                {
                    rangedHitFeedbacksHeavy?.PlayFeedbacks();
                    blackboard.SetVariableValue("wasHitHeavy", true);
                    Debug.Log("Ranged Heavy Hit registered on " + creatureType.creatureName);
                    PlayHitTween(t => t.DOShakePosition(
                        creatureType.rangedAttackShakeDuration,
                        new Vector3(
                            creatureType.heavyRangedAttackShakeIntensity, 0f,
                            creatureType.heavyRangedAttackShakeIntensity))); // or still punch
                }
            }

            var eventType = isCriticalHit
                ? DamageEventType.CriticalHitDamage
                : DamageEventType.DealtDamage;

            EnemyDamageEvent.Trigger(
                currentHealth - damageAmount, currentHealth, creatureType.maxHealth,
                eventType, creatureType.creatureName, DamageType.Health);

            EnemyDamageEvent.Trigger(
                currentStunDamage + stunAmount, currentStunDamage, creatureType.stunTreshold,
                DamageEventType.DealtDamage, creatureType.creatureName, DamageType.Stun);

            // blackboard.SetVariableValue(blackboardWasHitKey, true);
            // Debug.Log("Fallback hit registered on " + creatureType.creatureName);

            currentHealth -= damageAmount;
            currentStunDamage += stunAmount;

            if (currentStunDamage >= StunThreshold) currentStunDamage = StunThreshold;

            // Check if creature is now stunned
            if (!isStunned && currentStunDamage >= StunThreshold)
            {
                isStunned = true;
                blackboard.SetVariableValue("isStunned", true);
                blackboard.SetVariableValue("wasStunnedAtThreshold", true);
                Debug.Log(creatureType.creatureName + " is now stunned!");

                // Start the stun decay coroutine
                if (_stunDecayCoroutine != null) StopCoroutine(_stunDecayCoroutine);

                _stunDecayCoroutine = StartCoroutine(StunDecayCoroutine());
            }
            else if (isStunned)
            {
                // If already stunned and hit again, stun damage increases (capped), extending stun duration
                Debug.Log(creatureType.creatureName + " stun duration extended!");
            }

            highlightEffect.HitFX();
        }

        public void PlayHitTween(Func<Transform, Tween> buildTween, bool killPrevious = true)
        {
            if (killPrevious) _hitTween?.Kill();
            _hitTween = buildTween(transform);
        }
        public string UniqueID => uniqueID;
        public void SetUniqueID()
        {
            uniqueID = Guid.NewGuid().ToString();
        }
        public bool IsUniqueIDEmpty()
        {
            return string.IsNullOrEmpty(uniqueID);
        }

        public void OnMMEvent(PauseEvent eventType)
        {
            if (eventType.EventType == PauseEventType.PauseOn)
                _isPaused = true;
            else if (eventType.EventType == PauseEventType.PauseOff) _isPaused = false;
            else if (eventType.EventType == PauseEventType.TogglePause)
                _isPaused = !_isPaused;
        }
        protected void SetupAnimationStates()
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

            if (doesNotImmediatelyNeedToMove)
                return;

            if (creatureType.animationSet.moveAnimation == null)
            {
                Debug.Log(
                    $"CreatureType {creatureType.name} does not have a move animation assigned in its AnimationSet.");

                return;
            }

            var moveAnimSpeedMultiplier =
                creatureType.animationSet.moveAnimationSpeedMultiplier;

            MoveState = animancerComponent.States.GetOrCreate(creatureType.animationSet.moveAnimation);
            MoveState.Speed = moveAnimSpeedMultiplier;
            MoveState.Time = 0f;
            MoveState.Events(this).OnEnd = () =>
            {
                MoveState.Time = 0f;
                MoveState.Speed = 1f;
            };
        }

        public virtual void ActivateCreature()
        {
            if (IsActivated) return;

            IsActivated = true;
            animancerComponent.enabled = true;
            fsmOwner.enabled = true;
            feedbacksContainer.SetActive(true);
            SetupAnimationStates();


            // Additional activation logic can be added here
        }

        public virtual void DeactivateCreature()
        {
            if (!IsActivated) return;

            IsActivated = false;
            // Additional deactivation logic can be added here
            animancerComponent.enabled = false;
            fsmOwner.enabled = false;
            feedbacksContainer.SetActive(false);
            DisableAnimationStates();
        }

        void DisableAnimationStates()
        {
            IdleState = null;
            MoveState = null;
        }

        void SetupIdleCycling()
        {
        }

        protected void PlayNextIdle()
        {
            var set = creatureType.animationSet;

            if (set.numberOfIdleAnimations <= 1)
            {
                IdleState.Time = 0f;
                return;
            }

            _currentIdleIndex = (_currentIdleIndex + 1) % set.numberOfIdleAnimations;

            AnimationClip nextClip;
            float speed;

            switch (_currentIdleIndex)
            {
                case 1:
                    nextClip = set.additionalIdleAnimation0;
                    speed = set.additionalIdleAnimation0SpeedMultiplier;
                    break;
                case 2:
                    nextClip = set.additionalIdleAnimation1;
                    speed = set.additionalIdleAnimation1SpeedMultiplier;
                    break;
                default:
                    nextClip = set.idleAnimation;
                    speed = set.idleAnimationSpeedMultiplier;
                    break;
            }

            IdleState.Clip = nextClip;
            IdleState.Speed = speed;
            IdleState.Time = 0f;
        }

        public void SetIdleAnimationSpeed(float speed, AnimationClip value)
        {
            if (IdleState != null)
            {
                IdleState.Speed = speed;
                IdleState.Clip = value;
            }
        }

        IEnumerator StunDecayCoroutine()
        {
            Debug.Log(creatureType.creatureName + " stun decay started");

            // Calculate decay rate: stun threshold should decay to 0 over the stun cooldown time
            var decayRate = StunThreshold / creatureType.stunCooldownTime;

            var timeSinceLastUpdate = 0f;
            var uiUpdateInterval = 0.5f; // Only update UI every 0.1 seconds

            while (currentStunDamage > 0f && !_isPaused)
            {
                yield return null; // Still run every frame

                // Decrease damage every frame (smooth)
                currentStunDamage -= decayRate * Time.deltaTime;

                blackboard.SetVariableValue("stunDamage", currentStunDamage);

                // Accumulate time
                timeSinceLastUpdate += Time.deltaTime;

                // Only trigger event periodically OR when reaching zero
                if (timeSinceLastUpdate >= uiUpdateInterval || currentStunDamage <= 0f)
                {
                    EnemyDamageEvent.Trigger(
                        currentStunDamage, currentStunDamage + decayRate * Time.deltaTime, StunThreshold,
                        DamageEventType.DealtDamage, creatureType.creatureName, DamageType.Stun);


                    timeSinceLastUpdate = 0f;
                }
            }

            while (currentStunDamage > 0f)
            {
                yield return null;


                // Clamp to prevent going below zero
                if (currentStunDamage < 0f) currentStunDamage = 0f;
            }

            // Stun damage has reached zero, unstun the creature
            if (isStunned) isStunned = false;
            blackboard.SetVariableValue("isStunned", false);
            _stunDecayCoroutine = null;
        }

        public void PlayAnimationClip(AnimationClip clip)
        {
            IsPlayingCustomAnimation = true;
            var state = animancerComponent.Play(clip, 0.2f);
            state.Events(this).OnEnd = () => { IsPlayingCustomAnimation = false; };
        }

        public void StopPlayingCustomAnimation()
        {
            IsPlayingCustomAnimation = false;
            animancerComponent.Stop();
        }
        protected virtual IEnumerator InitializeAfterCreatureStateManager()
        {
            yield return null;

            var creatureStateManager = CreatureStateManager.Instance;
            if (creatureStateManager != null)
            {
                var creatureState = creatureStateManager.GetCreatureState(uniqueID);

                if (creatureState == CreatureStateManager.CreatureState.None) creatureState = initialCreatureState;

                if (creatureState == CreatureStateManager.CreatureState.HasBeenInitialized)
                {
                    ReLoadCreatureStateData();
                }
                else if (creatureState == CreatureStateManager.CreatureState.ShouldBeDestroyed)
                {
                    Debug.Log("Destroying creature on load");
                    Destroy(gameObject);
                }
            }

            if (appearsOnlyOnce)
            {
                // Wait x seconds 
                yield return new WaitForSeconds(secondsBeforeSettingShouldBeDestroyed);
                CreatureStateEvent.Trigger(
                    CreatureStateEventType.SetNewCreatureState, uniqueID,
                    CreatureStateManager.CreatureState.ShouldBeDestroyed);
            }
        }

        public void ResetStunState()
        {
            // Stop the decay coroutine if it's running
            if (_stunDecayCoroutine != null)
            {
                StopCoroutine(_stunDecayCoroutine);
                _stunDecayCoroutine = null;
            }

            isStunned = false;
            currentStunDamage = 0f;
            blackboard.SetVariableValue("isStunned", false);
            Debug.Log(creatureType.creatureName + " stun state reset (manually)");
        }


        public CreatureEffectsAndFeedbacks GetEffectsAndFeedbacks()
        {
            if (creatureType == null || creatureType.effectsAndFeedbacks == null)
            {
                Debug.LogWarning(
                    "CreatureType is null or EffectsAndFeedbacks is null, cannot get EffectsAndFeedbacks.");

                return null;
            }

            return creatureType.effectsAndFeedbacks;
        }

        protected virtual void ReLoadCreatureStateData()
        {
            Debug.Log("Loading creature state data");
        }

        [Serializable]
        public class AttackInstance
        {
            public string attackName;
            [FormerlySerializedAs("PlayerAttackData")]
            public EnemyAttack playerAttackData;
            public AnimationClip attackAnimationClip;
            public float animationSpeedMultiplier = 1f;
            public float attackDuration;
            // public float attackStartupTime;
            // public float hitActiveTime;
            public EnemyHitbox attackHitbox;
            /// <summary>
            ///     Second part is used when Animation has two distinct attack parts (e.g. bite + swipe)
            ///     If animation is single part, leave this unchecked and use multiple AttackInstances instead.
            /// </summary>
            // public bool hasSecondPart;
            // [ShowIf("hasSecondPart")] public float delayBetweenAttacks;
            // [ShowIf("hasSecondPart")] public EnemyAttack secondPartPlayerAttackData;
            public float leadupTime;
        }
    }
}
