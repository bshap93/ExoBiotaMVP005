using System;
using System.Collections.Generic;
using Helpers.Events.NPCs;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Manager.StateManager
{
    public class CreatureStateManager : StateManager<CreatureStateManager>, MMEventListener<CreatureStateEvent>
    {
        [Serializable]
        public enum CreatureState
        {
            None,
            ShouldBeInitialized,
            HasBeenInitialized,
            ShouldBeDestroyed
        }

        public bool overrideAllCreaturesState;
        [ShowIf("overrideAllCreaturesState")] [SerializeField]
        CreatureState overrideCreatureState;

        Dictionary<string, CreatureState> _creatureStates = new(StringComparer.Ordinal);
        public override void Reset()
        {
            _creatureStates.Clear();
            MarkDirty();
            ConditionalSave();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(CreatureStateEvent eventType)
        {
            if (eventType.EventType == CreatureStateEventType.SetNewCreatureState)
                AddOrSetCreatureState(eventType.UniqueID, eventType.CreatureState);
        }
        public override void Save()
        {
            var path = GetSaveFilePath();
            ES3.Save("CreatureStates", _creatureStates, path);
            Dirty = false;
        }
        public override void Load()
        {
            var path = GetSaveFilePath();
            _creatureStates.Clear();


            if (ES3.KeyExists("CreatureStates", path))
                _creatureStates = ES3.Load<Dictionary<string, CreatureState>>("CreatureStates", path);

            if (overrideAllCreaturesState)
            {
                var keys = new List<string>(_creatureStates.Keys);
                foreach (var key in keys) _creatureStates[key] = overrideCreatureState;
            }

            Dirty = false;
        }

        public CreatureState GetCreatureState(string uniqueID)
        {
            return _creatureStates.GetValueOrDefault(uniqueID, CreatureState.None);
        }

        public void AddOrSetCreatureState(string uniqueId, CreatureState state)
        {
            if (string.IsNullOrEmpty(uniqueId)) return;

            _creatureStates[uniqueId] = state;
            MarkDirty();
            ConditionalSave();
        }
        protected override string GetSaveFilePath()
        {
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.CreatureStateSave);
        }
    }
}
