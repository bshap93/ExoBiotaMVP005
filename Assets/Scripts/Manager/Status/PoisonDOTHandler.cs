using System.Collections;
using Helpers.Events.Combat;
using Helpers.Events.Status;
using Manager.Status.Scriptable;
using MoreMountains.Tools;
using UnityEngine;

namespace Manager.Status
{
    /// <summary>
    ///     Attach to the Player. Listens for a poison status effect being applied or removed.
    ///     While active, drains health at a configurable rate for a configurable duration.
    ///     Automatically removes the status effect when the duration expires.
    ///     An antidote (or anything that removes the effect via PlayerStatusEffectEvent) stops the drain immediately.
    /// </summary>
    public class PoisonDOTHandler : MonoBehaviour, MMEventListener<PlayerStatusEffectEvent>
    {
        [Header("Poison Settings")] [Tooltip("Must match the effectID used by CreaturePoisonAOE")] [SerializeField]
        string poisonEffectID = "Poison";

        [Tooltip("Health lost per second while poisoned")] [SerializeField]
        float damagePerSecond = 3f;

        [Tooltip("How long the poison lasts before auto-clearing (seconds)")] [SerializeField]
        float poisonDuration = 10f;
        bool _isPoisoned;

        Coroutine _poisonRoutine;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(PlayerStatusEffectEvent eventType)
        {
            // Only react to outbound events (confirmed applied/removed by the manager)
            if (eventType.Direction != PlayerStatusEffectEvent.DirectionOfEvent.Outbound) return;
            if (eventType.EffectID != poisonEffectID) return;

            if (eventType.Type == PlayerStatusEffectEvent.StatusEffectEventType.Apply)
                StartPoison();
            else if (eventType.Type == PlayerStatusEffectEvent.StatusEffectEventType.Remove) StopPoison();
        }

        void StartPoison()
        {
            if (_isPoisoned) return;
            _isPoisoned = true;
            StatusDebuffEvent.Trigger(
                StatusDebuffEvent.StatusDebuffEventType.Apply,
                StatusDebuffEvent.DebuffType.Poison, poisonEffectID);

            _poisonRoutine = StartCoroutine(PoisonDrainRoutine());
        }

        void StopPoison()
        {
            if (!_isPoisoned) return;
            _isPoisoned = false;

            if (_poisonRoutine != null)
            {
                StopCoroutine(_poisonRoutine);
                _poisonRoutine = null;
            }
        }

        IEnumerator PoisonDrainRoutine()
        {
            var elapsed = 0f;

            while (elapsed < poisonDuration)
            {
                var damage = damagePerSecond * Time.deltaTime;

                PlayerStatsEvent.Trigger(
                    PlayerStatsEvent.PlayerStat.CurrentHealth,
                    PlayerStatsEvent.PlayerStatChangeType.Decrease,
                    damage
                );

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Duration expired — remove the status effect (which also triggers StopPoison via the event)
            _poisonRoutine = null;
            _isPoisoned = false;

            PlayerStatusEffectEvent.Trigger(
                PlayerStatusEffectEvent.StatusEffectEventType.Remove,
                poisonEffectID,
                "",
                PlayerStatusEffectEvent.DirectionOfEvent.Inbound,
                StatusEffect.StatusEffectKind.None
            );
        }
    }
}
