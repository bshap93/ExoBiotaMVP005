using MoreMountains.Tools;

namespace Helpers.Events.Progression
{
    public enum StatType
    {
        HealthMax,
        // ContaminationMax,
        BaseStaminaRestoreRate,
        ContaminationMax
    }

    public struct SpendStatUpgradeEvent
    {
        static SpendStatUpgradeEvent _e;


        public StatType StatType;

        public static void Trigger(StatType statType)
        {
            _e.StatType = statType;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
