using EditorScripts;
using Manager.SceneManagers.Dock;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Overview.OverviewMode.ScriptableObjectDefinitions
{
    [CreateAssetMenu(
        fileName = "DockDefinition", menuName = "Scriptable Objects/Dock/DockDefinition",
        order = 1)]
    public class DockDefinition : ScriptableObject
    {
        public string dockName = "Dock";
        [Header("Ids & Scene")] [ValueDropdown("GetDockIdOptions")]
        public string dockId;

        public string[] locationIds; // list of location ids that this dock contains

        // public string worldScene = "Overworld"; // scene to load if not active

        // [Tooltip("Spawn point to place the player root (usually beside dirigible)")]
        // public string spawnPointId = "DirigibleDockSpawn";


        [FormerlySerializedAs("overrideSpawnInfo")] [SerializeField] [InlineProperty] [HideLabel]
        public SpawnInfoEditor spawnInfo;

        public float entryBlendTime = 1.0f; // pan length (unused for now)

        static string[] GetDockIdOptions()
        {
            // return DockManager.GetDockIdOptions();
            return new string[] { "Location1", "Location2", "Location3" };
        }
    }
}
