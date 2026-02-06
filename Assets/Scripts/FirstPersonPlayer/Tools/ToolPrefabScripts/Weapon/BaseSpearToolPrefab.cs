using System;
using Feedbacks.Interface;
using FirstPersonPlayer.Combat.Player.ScriptableObjects;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.Interactable.BioOrganism.Creatures;
using FirstPersonPlayer.Minable;
using FirstPersonPlayer.Tools.Interface;
using Helpers.Events;
using Helpers.Events.Combat;
using Helpers.Events.Status;
using Manager;
using Manager.ProgressionMangers;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Tools.ToolPrefabScripts.Weapon
{
    public class BaseSpearToolPrefab : MeleeToolPrefab, IRuntimeTool
    {
        public string[] allowedTags;
        public float normalAttackCooldown = 0.6f;

        public int spearPower = 1;

        [SerializeField] Sprite defaultReticleForTool;

        [SerializeField] protected float lastAttackTime = -999f;

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

        float StaminaCostPerHeavyAttack
        {
            get
            {
                var attrMgr = AttributesManager.Instance;
                if (attrMgr == null) return toolAttackProfile.heavyAttack.baseStaminaCost;

                var agility = attrMgr.Agility;
                var reduction = toolAttackProfile.agilityReductionFactor * (agility - 1); // Example: 0.05

                var finalCost = toolAttackProfile.heavyAttack.baseStaminaCost * (1f - reduction);


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
                PerformToolAction();

                ChargeTimeElapsed = 0f;
                ChargeToolEvent.Trigger(ChargeToolEventType.Release);
                return;
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
            else if (ChargeTimeElapsed >= timeToFullCharge && ToolIsHeldInChargePosition)
            {
                if (PlayerMutableStatsManager.Instance.CurrentStamina < StaminaCostPerNormalAttack)
                {
                    // Not enough stamina
                    AlertEvent.Trigger(
                        AlertReason.NotEnoughStamina, "Not enough stamina to use pickaxe.", "Insufficient Stamina");


                    return;
                }

                PerformHeavyChargedToolAction();
                ChargeToolEvent.Trigger(ChargeToolEventType.Release);
            }
            else if (ToolIsHeldInChargePosition)
            {
                if (PlayerMutableStatsManager.Instance.CurrentStamina < StaminaCostPerNormalAttack)
                {
                    // Not enough stamina
                    AlertEvent.Trigger(
                        AlertReason.NotEnoughStamina, "Not enough stamina to use pickaxe.", "Insufficient Stamina");


                    return;
                }

                PerformPartiallyChargedToolAction();
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
            if (go.TryGetComponent<IBreakable>(out var breakable))
            {
                // hardness/HP handled inside component
                breakable.ApplyHit(spearPower, hit.point, hit.normal, hitType);

                if (go.CompareTag("MiscRigidOrganism")) hitRigidOrganismFeedbacks?.PlayFeedbacks();
            }
            else if (go.TryGetComponent<MyOreNode>(out var oreNode))
            {
                // No apply here – ore nodes are for pickaxe only

                SpawnFxForIneffectualHit(hit.point, hit.normal);
                hitRockFeedbacks?.PlayFeedbacks();
            }
            else if (go.TryGetComponent<IFleshyObject>(out var fleshyObject))
            {
                hitFleshyFeedbacks?.PlayFeedbacks();
                fleshyObject.MakeJiggle();
                var contaminationAmt = fleshyObject.BaseBlowbackContaminationAmt;
                if (contaminationAmt > 0f)
                    PlayerStatsEvent.Trigger(
                        PlayerStatsEvent.PlayerStat.CurrentContamination,
                        PlayerStatsEvent.PlayerStatChangeType.Increase,
                        contaminationAmt);
            }
            else if (go.CompareTag("DiggerChunk") || go.CompareTag("MainSceneTerrain"))
            {
                SpawnFxForIneffectualHit(hit.point, hit.normal);
                hitRockFeedbacks?.PlayFeedbacks();
            }
            else if (go.CompareTag("MiscRigidOrganism"))
            {
                hitRigidOrganismFeedbacks?.PlayFeedbacks();
            }
            else if (go.CompareTag("EnemyNPC"))
            {
                var enemyController = go.GetComponentInParent<CreatureController>();

                if (enemyController == null)
                {
                    Debug.LogWarning("HatchetToolPrefab: Hit enemy NPC but no EnemyController found in parents.");
                    return;
                }

                var playerAttack = DetermineCorrectPlayerToolAttack(hitType);


                // Spawn VFX with proper cleanup
                var effectsAndFeedbacks = enemyController.GetEffectsAndFeedbacks();
                GameObject vfx = null;
                if (effectsAndFeedbacks != null) vfx = enemyController.GetEffectsAndFeedbacks().basicHitVFX;

                if (vfx != null)
                {
                    var vfxInstance = Instantiate(vfx, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(vfxInstance, 2f); // Clean up after 2 seconds
                }


                enemyController.ProcessAttackDamage(playerAttack, hit.point);
                if (hitType == HitType.Heavy)

                    Debug.Log("Stamina decreased by: " + StaminaCostPerHeavyAttack);
                else

                    Debug.Log("Stamina decreased by: " + StaminaCostPerNormalAttack);
            }
            else
            {
                // No apply here – ore nodes are for pickaxe only

                SpawnFxForIneffectualHit(hit.point, hit.normal);
                hitRockFeedbacks?.PlayFeedbacks();
            }

            Debug.Log($"[BaseSpearToolPrefab] Hit object: {go.name}, tag: {go.tag}");
        }
        public override void PerformToolAction()
        {
            normalAttackCooldown -= agilityCooldownSecondsReducePerPoint * (attributesManager.Agility - 1);
            if (Time.time < lastAttackTime + normalAttackCooldown) return;
            lastAttackTime = Time.time;

            PlayerStatsEvent.Trigger(
                PlayerStatsEvent.PlayerStat.CurrentStamina, PlayerStatsEvent.PlayerStatChangeType.Decrease,
                StaminaCostPerNormalAttack);

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
            throw new NotImplementedException();
        }
        public override void PerformHeavyChargedToolAction()
        {
            throw new NotImplementedException();
        }
    }
}
