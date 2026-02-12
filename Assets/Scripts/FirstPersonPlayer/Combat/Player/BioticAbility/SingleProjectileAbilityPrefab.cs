using System;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.Tools.Interface;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Combat.Player.BioticAbility
{
    public class SingleProjectileAbilityPrefab : MonoBehaviour, IRuntimeBioticAbility
    {
        [SerializeField] GameObject projectilePrefab;
        public LayerMask ProjectileLayerMask = -1;
        [SerializeField] GameObject muzzleFlashPrefab;
        [SerializeField] float projectileSpeed = 15f;
        Camera _mainCamera;
        PlayerEquippedAbility _owner;
        bool _readyToFire = true;
        float _timeSinceLastUse;
        
        

        public void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData,
            Transform originTransform)
        {
        }
        public IRuntimeBioticAbility.UsageScheme GetUsageScheme()
        {
            return IRuntimeBioticAbility.UsageScheme.UseTool;
        }
        public void Deactivate()
        {
        }
        public bool IsActive()
        {
            return false;
        }
        public void Initialize(PlayerEquippedAbility owner)
        {
            _owner = owner;
            _mainCamera = Camera.main;

            if (_owner != null && _owner.bioticAbilityAnchor != null)
            {
                // Position the ability prefab correctly
                transform.SetParent(_owner.bioticAbilityAnchor);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
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
