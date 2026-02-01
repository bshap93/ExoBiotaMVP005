using System;
using System.Collections.Generic;
using Dirigible.Input;
using Dirigible.Interface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dirigible.Interactable
{
    public class DirigibleElevationChangePoint : MonoBehaviour, IDirigibleInteractable
    {
        [SerializeField] LayerMask dirigibleLayers; // set to your Dirigible layer in Inspector
#if UNITY_EDITOR
        [FormerlySerializedAs("ActionId")] [ValueDropdown(nameof(GetAllRewiredActions))]
#endif
        public int actionId;
        readonly HashSet<Collider> _dirigibleOverlaps = new();
        float _ignoreUntil;

        public void OnTriggerEnter(Collider other)
        {
            if (Time.time < _ignoreUntil) return;
            if (!IsDirigible(other)) return;

            // if (_dirigibleOverlaps.Count == 0)
            //     DockingEvent.Trigger(DockingEventType.SetCurrentDock, def);

            _dirigibleOverlaps.Add(other);
        }
        public void Interact()
        {
            throw new NotImplementedException();
        }
        public void OnInteractionStart()
        {
            throw new NotImplementedException();
        }
        public void OnInteractionEnd()
        {
            throw new NotImplementedException();
        }
        public bool CanInteract()
        {
            throw new NotImplementedException();
        }
        public bool IsInteractable()
        {
            return true;
        }
        public void OnFocus()
        {
        }
        public void OnUnfocus()
        {
        }
        public void CompleteObjectiveOnInteract()
        {
        }
#if UNITY_EDITOR
        public IEnumerable<ValueDropdownItem<int>> GetAllRewiredActions()
        {
            return AllRewiredActions.GetAllRewiredActions();
        }

#endif

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        bool IsDirigible(Collider other)
        {
            if (((1 << other.gameObject.layer) & dirigibleLayers) == 0) return false;
            return other.GetComponentInParent<DirigiblePhysicalObject>() != null;
        }
    }
}
