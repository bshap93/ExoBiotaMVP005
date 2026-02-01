using System;
using System.Collections.Generic;
using Events;
using FirstPersonPlayer.Tools.ItemObjectTypes;
using JetBrains.Annotations;
using Manager;
using Manager.DialogueScene;
using Manager.SceneManagers.Dock;
using Objectives.ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace OWPData.ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(
        fileName = "DockOvLocationDefinition",
        menuName = "Scriptable Objects/Dock/DockOvLocationDefinition")]
    public class DockOvLocationDefinition : ScriptableObject
    {
        [FormerlySerializedAs("LocationName")] public string locationName;
        public LocationType locationType;


        [FormerlySerializedAs("doorDefinition")] [CanBeNull] [ValueDropdown("GetLocationIdOptions")]
        public string locationId;

        [ValueDropdown("GetNpcIdOptions")] public string npcInResidenceId;

        [ValueDropdown("GetDockIdOptions")] public string dockId;

        [FormerlySerializedAs("IsUnlockingKeyItem")] [Header("Unlocking Key Item")] [ToggleLeft]
        public bool isUnlockingKeyItem;
        [ShowIf("isUnlockingKeyItem")] public
            KeyItemObject keyItemToUnlock;

        [ValueDropdown("GetSceneOptions")] public string sceneName;

        [Header("Spawn Point")] [ValueDropdown("SpawnPointOptions")]
        public string spawnPointId;

        [ValueDropdown("GetSceneOptions")] public string spawnPointSceneName;

        [Header("Buttons Prefab")] public GameObject prefab;

        public List<ObjectiveObject> associatedObjectivesList;

        public HashSet<string> GetAssociatedObjectivesSet()
        {
            var set = new HashSet<string>();
            if (associatedObjectivesList != null)
                foreach (var obj in associatedObjectivesList)
                    if (obj != null && !string.IsNullOrEmpty(obj.objectiveId))
                        set.Add(obj.objectiveId);

            return set;
        }


        static string[] GetSceneOptions()
        {
            return PlayerSpawnManager.GetSceneOptions();
        }

        static string[] GetDockIdOptions()
        {
            return DockManager.GetDockIdOptions();
        }

        static string[] GetNpcIdOptions()
        {
            return DialogueManager.GetAllNpcIdOptions();
        }

        static string[] GetLocationIdOptions()
        {
            return DockManager.GetLocationIdOptions();
        }

        static string[] SpawnPointOptions()
        {
            return PlayerSpawnManager.GetSpawnPointIdOptions();
        }
        public string GetMineName()
        {
            if (locationType == LocationType.Mine) return locationName;

            return null;
        }
    }
}
