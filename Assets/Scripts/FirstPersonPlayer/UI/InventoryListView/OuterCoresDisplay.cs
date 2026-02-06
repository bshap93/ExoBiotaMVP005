using FirstPersonPlayer.Tools.ItemObjectTypes;
using Helpers.Events;
using Inventory;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using SharedUI.Inventory;
using UnityEngine;

namespace FirstPersonPlayer.UI.InventoryListView
{
    public class OuterCoresDisplay : MonoBehaviour, MMEventListener<MMInventoryEvent>,
        MMEventListener<LoadedManagerEvent>
    {
        [SerializeField] GradeCoresUILVRow standardCoreRow;
        [SerializeField] GradeCoresUILVRow radiantCoreRow;
        [SerializeField] GradeCoresUILVRow stellarCoreRow;
        [SerializeField] GradeCoresUILVRow unreasonableCoreRow;

        [SerializeField] bool condensedView;

        // [SerializeField] GatedLevelingUIController gatedLevelingUIController;


        void Start()
        {
            Initialize();
        }

        void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
            this.MMEventStartListening<LoadedManagerEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();
            this.MMEventStopListening<LoadedManagerEvent>();
        }
        public void OnMMEvent(LoadedManagerEvent eventType)
        {
            if (eventType.ManagerType == ManagerType.All) Initialize();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.TargetInventoryName != GlobalInventoryManager.OuterCoresInventoryName) return;
            if (eventType.InventoryEventType == MMInventoryEventType.ContentChanged)
                Refresh();
        }

        void Initialize()
        {
            Refresh();
        }

        public void Refresh()
        {
            var globalInventoryManager = GlobalInventoryManager.Instance;
            var numStandard = globalInventoryManager.GetNumberOfOuterCoresInInventory(
                OuterCoreItemObject.CoreObjectValueGrade.StandardGrade);

            var numRadiant = globalInventoryManager.GetNumberOfOuterCoresInInventory(
                OuterCoreItemObject.CoreObjectValueGrade.Radiant);

            var numStellar = globalInventoryManager.GetNumberOfOuterCoresInInventory(
                OuterCoreItemObject.CoreObjectValueGrade.Stellar);

            var numUnreasonable = globalInventoryManager.GetNumberOfOuterCoresInInventory(
                OuterCoreItemObject.CoreObjectValueGrade.Unreasonable);

            // var numExotic = globalInventoryManager.GetNumberOfInnerCoresInInventory(
            //     HarvestableInnerObject.InnerObjectValueGrade.MiscExotic);

            standardCoreRow.Initialize(OuterCoreItemObject.CoreObjectValueGrade.StandardGrade, numStandard);
            radiantCoreRow.Initialize(OuterCoreItemObject.CoreObjectValueGrade.Radiant, numRadiant);
            stellarCoreRow.Initialize(OuterCoreItemObject.CoreObjectValueGrade.Stellar, numStellar);
            unreasonableCoreRow.Initialize(OuterCoreItemObject.CoreObjectValueGrade.Unreasonable, numUnreasonable);

            if (condensedView)
            {
                if (numStandard == 0) standardCoreRow.gameObject.SetActive(false);
                else standardCoreRow.gameObject.SetActive(true);

                if (numRadiant == 0) radiantCoreRow.gameObject.SetActive(false);
                else radiantCoreRow.gameObject.SetActive(true);

                if (numStellar == 0) stellarCoreRow.gameObject.SetActive(false);
                else stellarCoreRow.gameObject.SetActive(true);

                if (numUnreasonable == 0) unreasonableCoreRow.gameObject.SetActive(false);
                else unreasonableCoreRow.gameObject.SetActive(true);
            }
            else
            {
                if (standardCoreRow.convertToXPButton != null)
                {
                    if (numStandard == 0) standardCoreRow.convertToXPButton.gameObject.SetActive(false);
                    else standardCoreRow.convertToXPButton.gameObject.SetActive(true);
                }

                if (radiantCoreRow.convertToXPButton != null)
                {
                    if (numRadiant == 0) radiantCoreRow.convertToXPButton.gameObject.SetActive(false);
                    else radiantCoreRow.convertToXPButton.gameObject.SetActive(true);
                }

                if (stellarCoreRow.convertToXPButton != null)
                {
                    if (numStellar == 0) stellarCoreRow.convertToXPButton.gameObject.SetActive(false);
                    else stellarCoreRow.convertToXPButton.gameObject.SetActive(true);
                }

                if (unreasonableCoreRow.convertToXPButton != null)
                {
                    if (numUnreasonable == 0) unreasonableCoreRow.convertToXPButton.gameObject.SetActive(false);
                    else unreasonableCoreRow.convertToXPButton.gameObject.SetActive(true);
                }
            }
        }
    }
}
