using System;
using System.Collections.Generic;
using Manager;
using Sirenix.OdinInspector;
using Structs;

namespace EditorScripts

{
    [Serializable]
    public struct SpawnInfoEditor
    {
        [ValueDropdown(nameof(GetSceneNames))] public string SceneName;

        [ValueDropdown(nameof(GetSpawnPoints))]
        public string SpawnPointId;

        public GameMode Mode;

        public SpawnInfo ToSpawnInfo()
        {
            return new SpawnInfo
            {
                SceneName = SceneName,
                SpawnPointId = SpawnPointId,
                Mode = Mode
            };
        }

        private static IEnumerable<string> GetSceneNames()
        {
            return PlayerSpawnManager.GetSceneOptions();
        }

        private IEnumerable<string> GetSpawnPoints()
        {
            return PlayerSpawnManager.GetSpawnPointIdOptions();
        }

        private static readonly Dictionary<string, List<string>> _sceneToSpawnPoints = new()
        {
            {
                "Overworld",
                new List<string> { "ScienceDockSpawn", "Mine01Dock", "MidFlightTestSpawn", "OutpostDockingSpawn01" }
            },
            { "Mine01", new List<string> { "SpawnDeep01", "Mine01DoorSpawn" } }
        };
    }
}