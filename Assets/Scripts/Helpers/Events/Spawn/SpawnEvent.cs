using MoreMountains.Tools;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using Structs;

namespace Helpers.Events
{
    public enum SpawnEventType
    {
        ToDock,
        ToMine,
        ToCaverns
    }

    public struct SpawnEvent
    {
        static SpawnEvent _e;

        public SpawnEventType spawnEventType; // unused for now, but can be used to differentiate events
        public string sceneName;
        public GameMode gameMode;
        public string spawnPointId;
        public DockDefinition dockDefinition;


        public static void Trigger(SpawnEventType spawnEventType, string sceneName, GameMode gameMode,
            string spawnPointId,
            DockDefinition dockDefinition = null)
        {
            _e.sceneName = sceneName;
            _e.gameMode = gameMode;
            _e.spawnPointId = spawnPointId;
            _e.dockDefinition = dockDefinition;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
