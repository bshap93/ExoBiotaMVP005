using System.Collections.Generic;
using Helpers.Events.Dialog;
using Helpers.Interfaces;
using Manager;
using MoreMountains.Tools;
using Overview.NPC;
using UnityEngine;

namespace FirstPersonPlayer.FPNPCs
{
    public class FriendlyNPCManager : MonoBehaviour, ICoreGameService, MMEventListener<MakeContactWithNPCEvent>
    {
        const string NPCsContactedKey = "NPCsContacted";
        public bool autoSave;
        [SerializeField] NpcDatabase npcDatabase;
        bool _dirty;
        string _savePath;

        public static FriendlyNPCManager Instance { get; private set; }

        HashSet<string> NPCsContactedAtLeastOnce { get; set; } = new();

        void Awake()
        {
            if (Instance == null)
                Instance = this;

            else
                Destroy(gameObject);
        }
        void Start()
        {
            _savePath = GetSaveFilePath();
            if (!ES3.FileExists(_savePath))
            {
                Debug.Log("[PlayerSaveManager] No save file found, forcing initial save...");
                Reset();
            }

            Load();
        }
        void OnEnable()
        {
            this.MMEventStartListening();
        }
        void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void Save()
        {
            var savePath = GetSaveFilePath();

            ES3.Save(NPCsContactedKey, NPCsContactedAtLeastOnce, savePath);
        }
        public void Load()
        {
            var savePath = GetSaveFilePath();

            if (!ES3.FileExists(savePath)) return;

            if (ES3.KeyExists(NPCsContactedKey, savePath))
                NPCsContactedAtLeastOnce = ES3.Load<HashSet<string>>(NPCsContactedKey, savePath);
            else
                NPCsContactedAtLeastOnce = new HashSet<string>();
        }
        public void Reset()
        {
            NPCsContactedAtLeastOnce.Clear();

            MarkDirty();
            ConditionalSave();
        }
        public void ConditionalSave()
        {
            if (autoSave && _dirty)
            {
                Save();
                _dirty = false;
            }
        }
        public void MarkDirty()
        {
            _dirty = true;
        }
        public string GetSaveFilePath()
        {
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.FriendlyNPCSave);
        }
        public void CommitCheckpointSave()
        {
            if (_dirty)
            {
                Save();
                _dirty = false;
            }
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(_savePath ?? GetSaveFilePath());
        }
        public void OnMMEvent(MakeContactWithNPCEvent eventType)
        {
            NPCsContactedAtLeastOnce.Add(eventType.NPCId);


            MarkDirty();
        }

        public bool HasNPCBeenContactedAtLeastOnce(string npcID)
        {
            return NPCsContactedAtLeastOnce.Contains(npcID);
        }
    }
}
