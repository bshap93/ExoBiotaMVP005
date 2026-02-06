using MoreMountains.Tools;

namespace Helpers.Events.Progression
{
    public struct EnemyXPRewardEvent
    {
        static EnemyXPRewardEvent _e;

        public void Trigger(int xpReward)
        {
            MMEventManager.TriggerEvent(_e);
        }
    }
}
