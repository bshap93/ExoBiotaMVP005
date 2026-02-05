using MoreMountains.Feedbacks;
using UnityEngine;

namespace FirstPersonPlayer.Tools.Interface
{
    public interface IRuntimeBioticAbility
    {
        /// <summary>
        ///     Called when the ability is first activated
        /// </summary>
        void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData,
            Transform originTransform);

        /// <summary>
        ///     Called when the ability should stop (button released, ran out of resources, etc.)
        /// </summary>
        void Deactivate();

        /// <summary>
        ///     Returns true if the ability is currently active
        /// </summary>
        bool IsActive();
        
        
        void Initialize(PlayerEquippedAbility owner);

        void Use();
        
        void Unequip();
        
        void Equip();
        
        bool CanInteractWithObject(GameObject colliderGameObject);
        
        bool AbilityMustBeHeldToUse();
        
        bool CanAbortAction();
        
        MMFeedbacks GetEquipFeedbacks();
        MMFeedbacks GetUnequipFeedbacks();
        
        

    }
}
