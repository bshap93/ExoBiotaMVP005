using FirstPersonPlayer.Combat.Player.ScriptableObjects;
using FirstPersonPlayer.Tools.Interface;
using Helpers.Events;
using Helpers.Events.Combat;
using Helpers.Events.Status;
using Manager;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Tools.ToolPrefabScripts.Weapon
{
    public class BaseSpearToolPrefab : MeleeToolPrefab, IRuntimeTool
    {
        public string[] allowedTags;
        public float normalAttackCooldown = 1f;

        public float spearPower = 1;

        [SerializeField] Sprite defaultReticleForTool;

        [SerializeField]
        protected float lastAttackTime = -999f;

        float StaminaCostPerNormalAttack
        {
            get
            {
                var attrMgr = AttributesManager.Instance;
                if (attrMgr == null) return toolAttackProfile.basicAttack.baseStaminaCost;

                var agility = attrMgr.Agility;
                var reduction = toolAttackProfile.agilityReductionFactor * (agility - 1); // Example: 0.05

                var finalCost = toolAttackProfile.basicAttack.baseStaminaCost * (1f - reduction);

                
                return Mathf.Max(0.1f, finalCost); // Ensure a minimum cost
                
            }
        }
        
        public override void Initialize(PlayerEquipment owner)
        {
            mainCamera = Camera.main;
            AnimController = owner.animancerPrimaryArmsController;
        }
        public override void Use()
        {
            if (attributesManager == null) attributesManager = AttributesManager.Instance;

            if (ChargeTimeElapsed > 0f && !ToolIsHeldInChargePosition)
            {
                //
            }

            if (!ToolIsHeldInChargePosition)
            {
                if (PlayerMutableStatsManager.Instance.CurrentStamina < StaminaCostPerNormalAttack)
                {
                    // Not enough stamina
                    AlertEvent.Trigger(
                        AlertReason.NotEnoughStamina, "Not enough stamina to use pickaxe.", "Insufficient Stamina");

                    return;
                } 
                
                PerformToolAction();

                ChargeToolEvent.Trigger(ChargeToolEventType.Release);
            }
        }
        public override Sprite GetReticleForTool(GameObject colliderGameObject)
        {
            // Check if the object has a tag that should show inability reticle
            if (tagsWhichShouldShowInabilityReticle != null)
                foreach (var tagName in tagsWhichShouldShowInabilityReticle)
                    if (colliderGameObject.CompareTag(tagName))
                        return reticleForHittable;

            // Default to the normal reticle
            return defaultReticleForTool;
        }
        public override MMFeedbacks GetEquipFeedbacks()
        {
            return equipFeedbacks;
        }
        public override MMFeedbacks GetUnequipFeedbacks()
        {
            return unequippedFeedbacks;
        }
        public override void ApplyHit(HitType hitType = HitType.Normal)
        {
            if (!mainCamera) mainCamera = Camera.main;
            if (!mainCamera) return;
            
            if (!Physics.Raycast(
                    mainCamera.transform.position, mainCamera.transform.forward,
                    out var hit, reach, hitMask, QueryTriggerInteraction.Ignore))
                return;
            
            var go = hit.collider.gameObject;
            
            // do damage to valid targets
            
            Debug.Log($"[BaseSpearToolPrefab] Hit object: {go.name}, tag: {go.tag}");
        }
        public override void PerformToolAction()
        {
            normalAttackCooldown -= agilityCooldownSecondsReducePerPoint * (attributesManager.Agility - 1);
            if (Time.time - lastAttackTime < normalAttackCooldown) return;
            lastAttackTime = Time.time;
            
            PlayerStatsEvent.Trigger(PlayerStatsEvent.PlayerStat.CurrentStamina, PlayerStatsEvent.PlayerStatChangeType.Decrease, StaminaCostPerNormalAttack);

            if (useMultipleSwings && AnimController.currentToolAnimationSet != null)
            {
                PlaySwingSequence();
                ToolIsHeldInChargePosition = false;
            }
            else
            {
                AnimController.PlayToolUseOneShot(speedMultiplier: swingSpeedMultiplier);
                ToolIsHeldInChargePosition = false;
                StartCoroutine(ApplyNormalHitAfterDelay(defaultHitDelay / swingSpeedMultiplier));
            }

            ChargeTimeElapsed = 0f;
            ToolIsHeldInChargePosition = false;
        }
        public override void PerformPartiallyChargedToolAction()
        {
            throw new System.NotImplementedException();
        }
        public override void PerformHeavyChargedToolAction()
        {
            throw new System.NotImplementedException();
        }
    }
}
