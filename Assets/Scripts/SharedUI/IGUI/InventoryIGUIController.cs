using Helpers.Events;
using Inventory;
using Manager.Global;
using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace SharedUI.IGUI
{
    public class InventoryIGUIController : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        public const int FPPlayerInventoryTypeIndex = 0;
        public const int DirigibleInventoryTypeIndex = 1;
        [SerializeField] CustomDropdown inventoryTypeDropdown;
        [SerializeField] Transform listTransform;
        [SerializeField] GameObject inventoryListItemPrefab;


        [FormerlySerializedAs("_playerInventory")] [SerializeField]
        MoreMountains.InventoryEngine.Inventory playerInventory;
        [FormerlySerializedAs("_dirigibleInventory")] [SerializeField]
        MoreMountains.InventoryEngine.Inventory dirigibleInventory;
        [FormerlySerializedAs("_weightProgressBar")] [SerializeField] 
        MoreMountains.InventoryEngine.Inventory bioticAbilitiesController;
        [SerializeField]
        ProgressBar weightProgressBar;

        int _currentInventoryType;

        void OnEnable()
        {
            this.MMEventStartListening();
            if (inventoryTypeDropdown != null)
                inventoryTypeDropdown.onValueChanged.AddListener(OnInventoryTypeChanged);
        }

        void OnDisable()
        {
            this.MMEventStopListening();
            if (inventoryTypeDropdown != null)
                inventoryTypeDropdown.onValueChanged.RemoveListener(OnInventoryTypeChanged);
        }


        public void OnMMEvent(MMInventoryEvent eventType)
        {
            // if (eventType.InventoryEventType == MMInventoryEventType.ItemEquipped ||
            //     eventType.InventoryEventType == MMInventoryEventType.ItemUnEquipped ||
            //     eventType.InventoryEventType == MMInventoryEventType.ContentChanged)
            //     // Refresh the inventory list when items are equipped, unequipped, or content changes
            // {
            var inventoryName = eventType.TargetInventoryName;
            // Find inventory

            MoreMountains.InventoryEngine.Inventory inventory0 = null;
            if (playerInventory != null && playerInventory.name == inventoryName)
                inventory0 = playerInventory;
            else if (dirigibleInventory != null && dirigibleInventory.name == inventoryName)
                inventory0 = dirigibleInventory;

            if (inventory0 == null) return;

            GlobalInventoryManager.InventoryWithWeightLimit inventoryWithWeightType;

            if (inventory0.name == playerInventory.name)
                inventoryWithWeightType = GlobalInventoryManager.InventoryWithWeightLimit.PlayerMainInventory;
            else if (inventory0.name == dirigibleInventory.name)
                inventoryWithWeightType = GlobalInventoryManager.InventoryWithWeightLimit.DirigibleInventory;
            else
                return;


            Refresh(inventory0, inventoryWithWeightType);
            // }
        }

        public bool SetInventoryTypeDropdown(int value)
        {
            if (inventoryTypeDropdown == null) return false;
            if (value < 0 || value >= inventoryTypeDropdown.items.Count) return false;

            inventoryTypeDropdown.SetDropdownIndex(value);
            return true;
        }

        void OnInventoryTypeChanged(int arg0)
        {
            // 0 is FPPlayer, 1 is Dirigible
            var currentMode = GameStateManager.Instance.CurrentMode;
            if (currentMode == GameMode.FirstPerson && arg0 == 1)
            {
                // Can't switch to Dirigible inventory in FP mode
                SetInventoryTypeDropdown(FPPlayerInventoryTypeIndex);
                AlertEvent.Trigger(
                    AlertReason.TooFarFromDirigible,
                    "You cannot access the Dirigible slots while not in Dirigible.",
                    "Not In Dirigible");

                return;
            }

            switch (arg0)
            {
                case FPPlayerInventoryTypeIndex:
                    Refresh(playerInventory, GlobalInventoryManager.InventoryWithWeightLimit.PlayerMainInventory);
                    break;
                case DirigibleInventoryTypeIndex:
                    Refresh(dirigibleInventory, GlobalInventoryManager.InventoryWithWeightLimit.DirigibleInventory);
                    break;
                default:
                    Debug.LogError("Invalid inventory type selected.");
                    break;
            }
        }

        public void Refresh(MoreMountains.InventoryEngine.Inventory inventory,
            GlobalInventoryManager.InventoryWithWeightLimit inventoryWithWeightLimit)
        {
            if (inventory == null) return;

            if (GlobalInventoryManager.Instance == null)
            {
                Debug.LogError("Inventory Manager is null.");
                return;
            }

            var weight = 0f;
            if (inventoryWithWeightLimit == GlobalInventoryManager.InventoryWithWeightLimit.PlayerMainInventory)
                weight = GlobalInventoryManager.Instance.GetWeightOfPlayerMainPlusEquipped();
            else if (inventoryWithWeightLimit == GlobalInventoryManager.InventoryWithWeightLimit.DirigibleInventory)
                weight =
                    GlobalInventoryManager.Instance.GetTotalWeightInDirigible();

            var maxWeight = -1f;
            // reference equals
            if (inventoryWithWeightLimit == GlobalInventoryManager.InventoryWithWeightLimit.PlayerMainInventory)
                maxWeight = GlobalInventoryManager.Instance.GetPlayerMaxWeight();
            else if (inventoryWithWeightLimit == GlobalInventoryManager.InventoryWithWeightLimit.DirigibleInventory)
                maxWeight = GlobalInventoryManager.Instance.GetDirigibleMaxWeight();

            weightProgressBar.maxValue = maxWeight;
            weightProgressBar.currentPercent = maxWeight > 0f ? weight : 0f;
            weightProgressBar.UpdateUI();


            // Clear existing list
// InventoryIGUIController.Refresh()
            foreach (Transform child in listTransform)
                Destroy(child.gameObject);

            for (var i = 0; i < inventory.Content.Length; i++)
            {
                var item = inventory.Content[i];
                if (InventoryItem.IsNull(item)) continue;

                var go = Instantiate(inventoryListItemPrefab, listTransform);
                var element = go.GetComponent<InventoryItemIGUIListElement>();
                element.Initialize(inventory, i); // <-- pass source + index
            }
        }
    }
}
