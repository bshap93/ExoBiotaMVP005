using MoreMountains.Tools;
using Structs;

namespace Helpers.Events
{
    public struct CheckpointEvent
    {
        private static CheckpointEvent _e;

        public SpawnInfo SpawnInfo;


        public static void Trigger(SpawnInfo spawnInfo)
        {
            _e.SpawnInfo = spawnInfo;
            MMEventManager.TriggerEvent(_e);
        }
    }
}