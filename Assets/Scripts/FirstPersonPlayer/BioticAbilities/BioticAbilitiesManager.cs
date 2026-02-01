using Helpers.Interfaces;
using Manager;
using UnityEngine;

namespace FirstPersonPlayer.BioticAbilities
{
    public class BioticAbilitiesManager : MonoBehaviour, ICoreGameService
    {
        [SerializeField] bool autoSave;
        bool _dirty;

        string _savePath;
        public static BioticAbilitiesManager Instance { get; private set; }

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

        public void Save()
        {
            var savePath = GetSaveFilePath();
        }
        public void Load()
        {
            var savePath = GetSaveFilePath();
        }
        public void Reset()
        {
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
            return SaveManager.Instance.GetGlobalSaveFilePath(GlobalManagerType.BioticAbilitiesSave);
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
    }
}
