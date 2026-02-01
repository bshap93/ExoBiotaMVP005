using FirstPersonPlayer.Combat.Player.BioticAbility;
using FirstPersonPlayer.ScriptableObjects.BioticAbility;
using FirstPersonPlayer.Tools;
using UnityEngine;

namespace FirstPersonPlayer.BioticAbilities
{
    public class PlayerBioticPowerEquipped : MonoBehaviour
    {
        // cannot set in inspector, but can be seen for debugging
        [HideInInspector] public BioticAbility bioticAbilityEquipped;
        [SerializeField] RewiredFirstPersonInputs rewiredInput;
        [SerializeField] PlayerEquipment playerEquipment;

        [SerializeField] // for testing only
        BioticAbility testBioticAbility;
        IRuntimeBioticAbility _bioticAbilityRuntime;
        GameObject _bioticAbilityRuntimeObject;
        GameObject _runtimeInstance;
        public static PlayerBioticPowerEquipped Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            if (playerEquipment == null)
                Debug.LogError("No playerEquipment assigned!");

            // for testing only
            if (testBioticAbility != null)
                EquipBioticAbility(testBioticAbility);
        }

        void Update()
        {
            if (rewiredInput != null)
                HandleBioticAbilityInput(
                    rewiredInput.itemUseModifierDown, rewiredInput.itemUseModifierHeld, rewiredInput.itemUseModifierUp);
        }

        public void EquipBioticAbility(BioticAbility bioticAbility)
        {
            bioticAbilityEquipped = bioticAbility;
            _bioticAbilityRuntimeObject = bioticAbilityEquipped.runtimeAbilityPrefab;
            _bioticAbilityRuntime = _bioticAbilityRuntimeObject.GetComponent<IRuntimeBioticAbility>();
            _runtimeInstance = Instantiate(_bioticAbilityRuntimeObject, transform);
        }
        void HandleBioticAbilityInput(bool rewiredInputItemUseModifierDown, bool rewiredInputItemUseModifierHeld,
            bool rewiredInputItemUseModifierUp)
        {
            if (bioticAbilityEquipped == null || playerEquipment.AreBothHandsOccupied())
                return;


            if (rewiredInputItemUseModifierDown)
                switch (bioticAbilityEquipped.usageType)
                {
                    case BioticAbility.UsageType.SingleUse:
                        // Debug.Log("Using biotic ability: " + bioticAbilityEquipped.name);
                        _bioticAbilityRuntime.Activate(bioticAbilityEquipped, transform);
                        break;
                    case BioticAbility.UsageType.UseWhileHeld:
                        // Debug.Log("Activating biotic ability: " + bioticAbilityEquipped.name);
                        _bioticAbilityRuntime.Activate(bioticAbilityEquipped, transform);
                        break;
                }

            if (rewiredInputItemUseModifierUp)
                switch (bioticAbilityEquipped.usageType)
                {
                    case BioticAbility.UsageType.SingleUse:
                        // Do nothing, single use abilities don't need to be deactivated
                        break;
                    case BioticAbility.UsageType.UseWhileHeld:
                        Debug.Log("Deactivating biotic ability: " + bioticAbilityEquipped.name);
                        _bioticAbilityRuntime.Deactivate();
                        break;
                }
        }
    }
}
