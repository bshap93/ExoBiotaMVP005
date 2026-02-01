using System;
using System.Collections.Generic;
using FirstPersonPlayer.Interactable.Doors.ScriptableObjects;
using Helpers.Events;
using Helpers.Interfaces;
using UnityEngine;

namespace Manager.SceneManagers
{
    [Serializable]
    public class DoorRuntimeState
    {
        public bool permanentlyUnlocked; // e.g., after a quest, the door never checks keys again
        public bool wasOpenedOnce; // optional, if you want “first-time” logic
    }


    [Serializable]
    public class DoorSaveData
    {
        // NEW: so defaults are applied only once per save file
        public bool DefaultKeysGranted;
        public Dictionary<string, DoorRuntimeState> Doors = new(); // by doorId
        public HashSet<string> OwnedKeys = new(); // player’s keyring
    }

    public class DoorManager : MonoBehaviour, ICoreGameService
    {
        [SerializeField] List<DoorKey> defaultKeys
            = new(); // assign in Inspector

        [SerializeField] DoorSaveData _data = new(); // serialize for debug, save to disk too

        public Sprite keyAccessChangeIcon; // icon for key access change alerts
        [SerializeField] bool autoSave; // checkpoint-only by default

        bool _dirty;

        // bool _loadedOnce; // add this field near the top of the class
        string _savePath;
        public static DoorManager Instance { get; private set; }


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (SaveManager.Instance.saveManagersDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();

            if (!HasSavedData())
            {
                Debug.Log("[DoorManager] No save file found, forcing initial save...");
                Reset(); // clean default state
                Save(); // ensure the file exists
            }

            Load();

            // ---- GRANT DEFAULTS & SAVE IMMEDIATELY ----
            if (!_data.DefaultKeysGranted && defaultKeys != null && defaultKeys.Count > 0)
            {
                foreach (var k in defaultKeys)
                    if (k)
                        _data.OwnedKeys.Add(k.keyId);

                _data.DefaultKeysGranted = true;

                // write right now so any later Load() sees the updated state
                Save();
            }
        }


        public void Save()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();
            ES3.Save("DoorData", _data, _savePath); // or JSONUtility to path
            _dirty = false;
        }

        public void Load()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();

            try
            {
                _data = ES3.KeyExists("DoorData", _savePath)
                    ? ES3.Load<DoorSaveData>("DoorData", _savePath)
                    : new DoorSaveData();
            }
            catch (TypeLoadException e)
            {
                Debug.LogWarning($"[DoorManager] Save incompatible with current DoorSaveData. Resetting. {e.Message}");
                // Option 1: nuke and start fresh
                _data = new DoorSaveData();
                ES3.DeleteKey("DoorData", _savePath);
                // Option 2 (future): attempt a migration here
            }
        }


        public void ConditionalSave()
        {
            if (autoSave && _dirty) Save();
        }

        public void MarkDirty()
        {
            _dirty = true;
        }

        public void Reset()
        {
            _data = new DoorSaveData();
            _dirty = true;
            ConditionalSave(); // persist immediately if autosave is on
        }

        public string GetSaveFilePath()
        {
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.DoorSave);
        }

        public void CommitCheckpointSave()
        {
            if (_dirty) Save();
        }

        public bool HasSavedData()
        {
            if (_savePath == null) _savePath = GetSaveFilePath();
            return ES3.FileExists(_savePath);
        }


        // -------- Keys ----------
        public bool HasKey(string keyId)
        {
            return _data.OwnedKeys.Contains(keyId);
        }

        public bool GrantKey(string keyId)
        {
            var added = _data.OwnedKeys.Add(keyId);
            if (added)
            {
                AlertEvent.Trigger(
                    AlertReason.KeyAccessChange, $"You now have the key: {keyId}",
                    "Key access changed",
                    AlertType.Basic, 3f, keyAccessChangeIcon);

                MarkDirty();
                ConditionalSave();
            }

            return added;
        }

        public void GrantKeys(IEnumerable<DoorKey> keys)
        {
            foreach (var k in keys)
                if (k)
                    _data.OwnedKeys.Add(k.keyId); // uses DoorKey.keyId

            MarkDirty();
            ConditionalSave();
        }

        public void GrantKeys(IEnumerable<string> ids)
        {
            var changed = false;
            foreach (var id in ids)
                changed |= _data.OwnedKeys.Add(id);

            if (changed)
            {
                MarkDirty();
                ConditionalSave();
            }
        }

        public bool RevokeKey(string keyId)
        {
            var removed = _data.OwnedKeys.Remove(keyId);
            if (removed)
            {
                MarkDirty();
                ConditionalSave();
            }

            return removed;
        }

        // -------- Door state ----
        DoorRuntimeState GetDoorState(string doorId)
        {
            if (!_data.Doors.TryGetValue(doorId, out var state))
            {
                state = new DoorRuntimeState();
                _data.Doors[doorId] = state;

                // Not necessarily dirty until something flips, but it’s safe:
                MarkDirty();
                ConditionalSave();
            }

            return state;
        }

        public void SetOpenedOnce(string doorId)
        {
            var s = GetDoorState(doorId);
            if (!s.wasOpenedOnce)
            {
                s.wasOpenedOnce = true;
                MarkDirty();
                ConditionalSave();
            }
        }

        public void PermanentlyUnlock(string doorId)
        {
            var s = GetDoorState(doorId);
            if (!s.permanentlyUnlocked)
            {
                s.permanentlyUnlocked = true;
                MarkDirty();
                ConditionalSave();
            }
        }

        public bool CanOpen(in DoorDefinition def)
        {
            if (def == null) return true; // treat missing def as unlocked
            var s = GetDoorState(def.doorId);
            if (s.permanentlyUnlocked || def.accessMode == DoorDefinition.AccessMode.Unlocked) return true;

            if (def.requiredKeyIds == null || def.requiredKeyIds.Count == 0) return true;

            if (def.accessMode == DoorDefinition.AccessMode.RequireAll)
            {
                foreach (var k in def.requiredKeyIds)
                    if (!HasKey(k))
                        return false;

                return true;
            } // RequireAny

            foreach (var k in def.requiredKeyIds)
                if (HasKey(k))
                    return true;

            return false;
        }
    }
}
