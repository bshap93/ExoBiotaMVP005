using Manager.Status;
using MoreMountains.Tools;

namespace Helpers.Events.Status
{
    public struct InfectionUIEvent
    {
        static InfectionUIEvent _e;
        public int MinutesUntilNextInfection;
        public int MinutesPerInfection;
        public InfectionManager.OngoingInfection OngoingInfection;
        public bool Enable;
        public static void Trigger(int minutesUntil, int minutesPer,
            InfectionManager.OngoingInfection newInfection = null, bool enable = true)
        {
            _e.MinutesUntilNextInfection = minutesUntil;
            _e.MinutesPerInfection = minutesPer;

            _e.OngoingInfection = newInfection;
            _e.Enable = enable;


            MMEventManager.TriggerEvent(_e);
        }
    }
}
