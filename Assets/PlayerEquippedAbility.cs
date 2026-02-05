using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using SharedUI;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerEquippedAbility : MonoBehaviour,  MMEventListener<MMInventoryEvent>
{

    public MoreMountains.InventoryEngine.Inventory equippedAbilityInventory;
    [SerializeField] string equippedAbilityInventoryName = "EquippedAbilityInventory";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
    public int actionId;
    public void OnMMEvent(MMInventoryEvent eventType)
    {
        
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
