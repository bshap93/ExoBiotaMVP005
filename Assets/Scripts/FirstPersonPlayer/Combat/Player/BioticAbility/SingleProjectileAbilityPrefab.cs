using System;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.Tools.Interface;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Combat.Player.BioticAbility
{
    public class SingleProjectileAbilityPrefab : MonoBehaviour, IRuntimeBioticAbility
    {
        public void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData,
            Transform originTransform)
        {
            throw new NotImplementedException();
        }
        public IRuntimeBioticAbility.UsageScheme GetUsageScheme()
        {
            throw new NotImplementedException();
        }
        public void Deactivate()
        {
            throw new NotImplementedException();
        }
        public bool IsActive()
        {
            throw new NotImplementedException();
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
