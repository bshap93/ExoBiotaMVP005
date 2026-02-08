using MoreMountains.Tools;

namespace Helpers.Events.Progression
{
    public struct NotifyAttributesNewlySetEvent
    {
        static NotifyAttributesNewlySetEvent _e;

        public int Strength;
        public int Agility;
        public int Dexterity;
        public int BioticLevel;

        public static void Trigger(int strength, int agility, int dexterity, int bioticLevel)
        {
            _e.Strength = strength;
            _e.Agility = agility;
            _e.Dexterity = dexterity;
            _e.BioticLevel = bioticLevel;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
