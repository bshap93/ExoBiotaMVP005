using System;
using System.Collections;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.Tools.Interface;
using Helpers.Events;
using Manager;
using Manager.ProgressionMangers;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.VFX;

namespace FirstPersonPlayer.Combat.Player.BioticAbility
{
    public class AOEAbilityPrefab : MonoBehaviour, IRuntimeBioticAbility
    {
        [SerializeField] GameObject aoeEffectPrefab; // Prefab for the AOE effect (e.g., explosion)
        [SerializeField] Collider abilityCollider; // Collider that defines the AOE range
        [SerializeField] Transform rootPosition;
        [SerializeField] FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility
            abilityData; // Reference to the ScriptableObject for range and damage info
        [SerializeField] float cooldownTime = 1f; // Cooldown time in seconds
        [SerializeField] float delayBeforeAOEAfterFeedbacks = 0.4f;

        [Header("Feedbacks")] [SerializeField] MMFeedbacks equipFeedbacks;
        [SerializeField] MMFeedbacks unequipFeedbacks;
        VisualEffect _aoeEffectComponent;

        GameObject _aoeEffectInstance; // Instance of the AOE effect prefab
        FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility _currentAbilityData;
        PlayerEquippedAbility _owner;
        bool _readyToFire = true;
        float _timeSinceLastUse;
        float ContaminationCostPerNormalUse
        {
            get
            {
                var attrMgr = AttributesManager.Instance;
                if (attrMgr == null) return abilityData.baseContaminationCostPerUse;

                var biotic = attrMgr.Exobiotic;
                var reduction = abilityData.bioticReductionFactor * (biotic - 1);
                var finalCost = abilityData.baseContaminationCostPerUse * (1 - reduction);
                return Mathf.Max(0.1f, finalCost);
            }
        }

        void Awake()
        {
            if (aoeEffectPrefab != null && rootPosition != null)
            {
                _aoeEffectInstance = Instantiate(aoeEffectPrefab, rootPosition.position, rootPosition.rotation);
                _aoeEffectInstance.transform.SetParent(rootPosition);
                _aoeEffectComponent = _aoeEffectInstance.GetComponent<VisualEffect>();

                if (_aoeEffectComponent != null) _aoeEffectComponent.Stop();
            }
        }
        void Update()
        {
            if (_timeSinceLastUse < abilityData.baseCooldownTime)
                _timeSinceLastUse += Time.deltaTime;
            else
                _readyToFire = true;
        }

        void OnDestroy()
        {
            if (_aoeEffectInstance != null) Destroy(_aoeEffectInstance);
        }
        public void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData,
            Transform originTransform)
        {
            // UseTool scheme doesn't use Activate - handled in Use() instead   
        }
        public IRuntimeBioticAbility.UsageScheme GetUsageScheme()
        {
            return IRuntimeBioticAbility.UsageScheme.UseTool;
        }
        public void Deactivate()
        {
            // UseTool scheme doesn't use Deactivate - handled in Use() instead
        }
        public bool IsActive()
        {
            return false; // UseTool scheme doesn't use IsActive
        }
        public void Initialize(PlayerEquippedAbility owner)
        {
            _owner = owner;
            if (_owner != null && _owner.bioticAbilityAnchor != null)
            {
                transform.SetParent(_owner.bioticAbilityAnchor);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
        public void Use()
        {
            if (!_readyToFire)
            {
                Debug.Log("On cooldown");
                return;
            }

            if (_currentAbilityData == null)
            {
                Debug.LogError("Ability data not set for AOEAbilityPrefab.");
                return;
            }

            if (PlayerMutableStatsManager.Instance.CurrentContamination < ContaminationCostPerNormalUse)
            {
                // Not enough stamina
                AlertEvent.Trigger(
                    AlertReason.NotEnoughContamination, "Not enough contamination to use ability.",
                    "Insufficient Contamination");


                return;
            }

            StartCoroutine(FireAOEAbility());

            _readyToFire = false;
            _timeSinceLastUse = 0f;
        }
        public void Unequip()
        {
            throw new NotImplementedException();
        }
        public void Equip()
        {
            throw new NotImplementedException();
        }
        public bool CanInteractWithObject(GameObject colliderGameObject)
        {
            throw new NotImplementedException();
        }
        public bool AbilityMustBeHeldToUse()
        {
            throw new NotImplementedException();
        }
        public bool CanAbortAction()
        {
            throw new NotImplementedException();
        }
        public MMFeedbacks GetEquipFeedbacks()
        {
            return equipFeedbacks;
        }
        public MMFeedbacks GetUnequipFeedbacks()
        {
            return unequipFeedbacks;
        }
        IEnumerator FireAOEAbility()
        {
            yield return new WaitForSeconds(delayBeforeAOEAfterFeedbacks);
        }

        // Public method to set ability data (called during equip)
        public void SetAbilityData(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData)
        {
            _currentAbilityData = abilityData;
        }
    }
}
