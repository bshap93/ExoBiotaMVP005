using System;
using Helpers.Events.Status;
using UnityEngine;

namespace Helpers.Collider
{
    public enum LiquidType
    {
        Water,
        HotspringPoolWater,
        DeathFog
    }

    public class LiquidEffectTrigger : MonoBehaviour
    {
        [SerializeField] LiquidType liquidType;
        void Start()
        {
        }

        void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (other.CompareTag("FirstPersonPlayer"))
                switch (liquidType)
                {
                    case LiquidType.HotspringPoolWater:
                        PlayerStatsEvent.Trigger(
                            PlayerStatsEvent.PlayerStat.CurrentHealth, PlayerStatsEvent.PlayerStatChangeType.Increase,
                            20f);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }
    }
}
