using AINPC.ScriptableObjects;
using Helpers.Events.NPCs;
using UnityEngine;

namespace NewScript
{
    public class MovingProjectileController : MonoBehaviour
    {
        [SerializeField] GameObject burstEffectPrefab;
        [SerializeField] EnemyAttack enemyAttack;
        [SerializeField] float maxLifetime = 5f;
        [SerializeField] LayerMask ignoreLayers;

        float _lifetimeTimer;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _lifetimeTimer = maxLifetime;
        }

        // Update is called once per frame
        void Update()
        {
            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f) Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            // Ignore collisions with specified layers
            if (((1 << other.gameObject.layer) & ignoreLayers) != 0) return;
            // Instantiate burst effect at the collision point
            if (burstEffectPrefab != null) Instantiate(burstEffectPrefab, transform.position, Quaternion.identity);
            if (other.CompareTag("FirstPersonPlayer"))
                NPCAttackEvent.Trigger(enemyAttack);

            Destroy(gameObject);
        }
    }
}
