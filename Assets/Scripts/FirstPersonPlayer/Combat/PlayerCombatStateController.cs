using Manager;
using UnityEngine;

namespace FirstPersonPlayer.Combat
{
    public class PlayerCombatStateController : MonoBehaviour
    {
        [SerializeField] PlayerMutableStatsManager playerMutableStatsManager;

        public float CurrentStamina { get; private set; }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (playerMutableStatsManager == null)
                playerMutableStatsManager = GetComponent<PlayerMutableStatsManager>();

            if (playerMutableStatsManager != null) CurrentStamina = playerMutableStatsManager.CurrentMaxStamina;
        }
    }
}
