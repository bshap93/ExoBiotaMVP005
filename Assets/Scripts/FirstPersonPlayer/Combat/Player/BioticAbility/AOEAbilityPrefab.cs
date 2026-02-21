using System;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.Tools.Interface;
using Manager.ProgressionMangers;
using MoreMountains.Feedbacks;
using UnityEngine;

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
        PlayerEquippedAbility _owner;
        bool _readyToFire = true;
        float _timeSinceLastUse;
        
        GameObject _aoeEffectInstance; // Instance of the AOE effect prefab

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
        void Update()
        {
            if (_timeSinceLastUse < abilityData.baseCooldownTime)
                _timeSinceLastUse += Time.deltaTime;
            else
                _readyToFire = true;
        }

        void Awake()
        {
            if (aoeEffectPrefab != null && rootPosition != null)
            {
                _aoeEffectInstance = Instantiate(aoeEffectPrefab, rootPosition.position, rootPosition.rotation);
                _aoeEffectInstance.transform.SetParent(rootPosition);
                
            }
            
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
            throw new NotImplementedException();
        }
        public void Use()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public MMFeedbacks GetUnequipFeedbacks()
        {
            throw new NotImplementedException();
        }
    }
}
