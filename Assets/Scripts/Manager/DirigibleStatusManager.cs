using System;
using Helpers.Interfaces;
using UnityEngine;

namespace Manager
{
    public class DirigibleStatusManager : MonoBehaviour, ICoreGameService
    {
        // float _hullInteagrity = 100f;

        public static DirigibleStatusManager Instance { get; private set; }


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
        public void Load()
        {
            throw new NotImplementedException();
        }
        public void Reset()
        {
            throw new NotImplementedException();
        }
        public void ConditionalSave()
        {
            throw new NotImplementedException();
        }
        public void MarkDirty()
        {
            throw new NotImplementedException();
        }
        public string GetSaveFilePath()
        {
            throw new NotImplementedException();
        }
        public void CommitCheckpointSave()
        {
            throw new NotImplementedException();
        }
        public bool HasSavedData()
        {
            throw new NotImplementedException();
        }
    }
}
