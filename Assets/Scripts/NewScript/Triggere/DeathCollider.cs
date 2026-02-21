using Helpers.Events;
using Manager.FirstPerson;
using UnityEngine;

namespace NewScript.Triggere
{
    public class DeathCollider : MonoBehaviour
    {
        [SerializeField] DeathInformation deathInformation;
        // [SerializeField] bool arrestCameraOnDeath = true;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FirstPersonPlayer"))
                PlayerDeathEvent.Trigger(deathInformation);
        }
    }
}
