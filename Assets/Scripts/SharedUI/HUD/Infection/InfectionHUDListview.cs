using System.Collections.Generic;
using Helpers.Events.Status;
using Manager.Status;
using MoreMountains.Tools;
using SharedUI.Interface;
using UnityEngine;

namespace SharedUI.HUD.Infection
{
    public class InfectionHUDListview : MonoBehaviour, IItemList, MMEventListener<InfectionUIEvent>
    {
        [SerializeField] Transform listRoot;
        [SerializeField] GameObject listItemPrefab;
        [SerializeField] List<GameObject> listItems = new();
        [SerializeField] InfectionManager infectionManager;
        [SerializeField] BodyInfectionHUD infectionHUD;

        void Start()
        {
            RefreshItemList();
        }
        void OnEnable()
        {
            this.MMEventStartListening();
        }
        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void RefreshItemList()
        {
            foreach (var go in listItems) Destroy(go);
            listItems.Clear();

            if (infectionManager == null || infectionManager.OngoingInfections == null) return;

            foreach (var infection in infectionManager.OngoingInfections)
            {
                var row = Instantiate(listItemPrefab, listRoot);
                listItems.Add(row);

                if (row.TryGetComponent(out InfectionRepElementHUD ui)) ui.SetNewInfection(infection);
            }

            if (infectionManager.OngoingInfections.Count == 0) infectionHUD.UpdateTopVisibility(false);
        }


        public void OnMMEvent(InfectionUIEvent eventType)
        {
            RefreshItemList();
        }
        public void ClearInfection(InfectionManager.OngoingInfection eventTypeOngoingInfection)
        {
            // throw new System.NotImplementedException();
        }
    }
}
