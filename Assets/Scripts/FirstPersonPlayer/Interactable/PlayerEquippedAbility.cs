using System;
using System.Collections;
using System.Collections.Generic;
using FirstPersonPlayer;
using FirstPersonPlayer.InputHandling;
using FirstPersonPlayer.Interactable;
using FirstPersonPlayer.ScriptableObjects.BioticAbility;
using FirstPersonPlayer.Tools;
using FirstPersonPlayer.Tools.Interface;
using FirstPersonPlayer.Tools.ItemObjectTypes;
using FirstPersonPlayer.UI.ProgressBars;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using SharedUI;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerEquippedAbility : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public MoreMountains.InventoryEngine.Inventory equippedAbilityInventory;
    [SerializeField] string equippedAbilityInventoryName = "EquippedAbilityInventory";
    public Transform bioticAbilityAnchor;

    public ProgressBarPurple progressBar;
    [SerializeField] RewiredFirstPersonInputs rewiredInput;
    [SerializeField] PlayerEquipment playerEquipment;

    BioticAbilityToolWrapper _bioticAbilityEquipped;

    public PlayerInteraction playerInteraction;

    public MyNormalMovement playerNormalMovement;
#if UNITY_EDITOR
    [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
    public int actionId;

    Coroutine _equipInitRoutine;

    float _nextUseTime;

    bool _wasUseButtonHeldLastFrame;

    public static PlayerEquippedAbility Instance { get; private set; }
    
    public BioticAbilityToolWrapper CurrentAbilityItemSo { get; private set; }
    public IRuntimeBioticAbility CurrentRuntimeAbility { get; private set; }
    
    void Awake()
    {
        Instance = this;
    }   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rewiredInput != null)
        {
            HandleBioticAbilityInput(rewiredInput.pressedSprintOrAbility, rewiredInput.heldSprintOrAbility);
            
        }
    }
    void HandleBioticAbilityInput(bool rewiredInputItemSprintOrAbilityPressed,
        bool rewiredInputItemSprintOrAbilityHeld)
    {
        if (_bioticAbilityEquipped == null || playerEquipment.AreBothHandsOccupied())
            return;


        if (rewiredInputItemSprintOrAbilityPressed)
            switch (_bioticAbilityEquipped.bioticAbility.usageType)
            {
                case BioticAbility.UsageType.SingleUse:
                    // Debug.Log("Using biotic ability: " + bioticAbilityEquipped.name);
                    CurrentRuntimeAbility.Activate(_bioticAbilityEquipped.bioticAbility, transform);
                    break;
                case BioticAbility.UsageType.UseWhileHeld:
                    // Debug.Log("Activating biotic ability: " + bioticAbilityEquipped.name);
                    CurrentRuntimeAbility.Activate(_bioticAbilityEquipped.bioticAbility, transform);
                    break;
            }

        if (rewiredInputItemSprintOrAbilityHeld)
            switch (_bioticAbilityEquipped.bioticAbility.usageType)
            {
                case BioticAbility.UsageType.SingleUse:
                    // Do nothing, single use abilities don't need to be deactivated
                    break;
                case BioticAbility.UsageType.UseWhileHeld:
                    Debug.Log("Deactivating biotic ability: " + _bioticAbilityEquipped.name);
                    CurrentRuntimeAbility.Deactivate();
                    break;
            }
    }
    void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();
        
        if (_equipInitRoutine != null) StopCoroutine(_equipInitRoutine);
        _equipInitRoutine = StartCoroutine(WaitForInventoryAndEquip());
        
    }
    IEnumerator WaitForInventoryAndEquip()
    {
        throw new NotImplementedException();
    }
    public void EquipBioticAbility(BioticAbilityToolWrapper bioticAbility)
    {
        _bioticAbilityEquipped = bioticAbility;

        
        throw new NotImplementedException();
    }


    
    
    void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();
        if (_equipInitRoutine != null)
        {
            StopCoroutine(_equipInitRoutine);
            _equipInitRoutine = null;
        }
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        
    }
    public void OnMMEvent(MMInventoryEvent e)
    {
        if (e.TargetInventoryName != equippedAbilityInventoryName) return;

        switch (e.InventoryEventType)
        {
            case MMInventoryEventType.ItemEquipped:
                if (e.EventItem is BioticAbilityToolWrapper ability01) EquipBioticAbility(ability01);
                break;
            case MMInventoryEventType.ItemUnEquipped:
                if (e.EventItem is BioticAbilityToolWrapper ability02) UnequipBioticAbility();
                break;
            case MMInventoryEventType.ItemUsed:
                if (e.EventItem is BioticAbilityToolWrapper ) UseCurrentBioticAbility();
                break;
            case MMInventoryEventType.Destroy:
                if (e.EventItem is BioticAbilityToolWrapper) UnequipBioticAbility();
                break;
            
            
        }
        
    }
    void UseCurrentBioticAbility()
    {
        throw new NotImplementedException();
    }
    void UnequipBioticAbility()
    {
        throw new NotImplementedException();
    }

#if UNITY_EDITOR
    // This will be called from the parent ScriptableObject
    IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
    {
        var parent = ControlsPromptSchemeSet._currentContextSO;
        if (parent == null || parent.inputManagerPrefab == null) yield break;

        var data = parent.inputManagerPrefab.userData;
        if (data == null) yield break;

        foreach (var action in data.GetActions_Copy())
            yield return new ValueDropdownItem<int>(action.name, action.id);
    }
#endif
}
