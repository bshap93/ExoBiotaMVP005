using FirstPersonPlayer.Tools.Interface;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Combat.Player.BioticAbility
{
    public class SingleBeamAbilityPrefab : MonoBehaviour, IRuntimeBioticAbility
    {
        public void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData, Transform originTransform)
        {
            // Usage sceheme not using this.
        }
        public IRuntimeBioticAbility.UsageScheme GetUsageScheme()
        {
            return IRuntimeBioticAbility.UsageScheme.UseTool;

        }
        public void Deactivate()
        {
            // Usage sceheme not using this.
        }
        public bool IsActive()
        {
            // Usage sceheme not using this.
            return false;
        }
        public void Initialize(PlayerEquippedAbility owner)
        {
            throw new System.NotImplementedException();
        }
        public void Use()
        {
            throw new System.NotImplementedException();
        }
        public void Unequip()
        {
            throw new System.NotImplementedException();
        }
        public void Equip()
        {
            throw new System.NotImplementedException();
        }
        public bool CanInteractWithObject(GameObject colliderGameObject)
        {
            throw new System.NotImplementedException();
        }
        public bool AbilityMustBeHeldToUse()
        {
            throw new System.NotImplementedException();
        }
        public bool CanAbortAction()
        {
            throw new System.NotImplementedException();
        }
        public MMFeedbacks GetEquipFeedbacks()
        {
            throw new System.NotImplementedException();
        }
        public MMFeedbacks GetUnequipFeedbacks()
        {
            throw new System.NotImplementedException();
        }
    }
}
