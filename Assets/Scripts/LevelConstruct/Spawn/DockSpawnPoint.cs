using LevelConstruct.Spawn;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using UnityEngine.Serialization;

namespace Spawn
{
    public class DockSpawnPoint : SpawnPoint
    {
        [FormerlySerializedAs("DockDefinition")]
        public DockDefinition dockDefinition;
    }
}
