using Helpers.Events;
using Manager.FirstPerson;
using UnityEngine;

namespace NewScript
{
    public class DeathCollider : MonoBehaviour
    {
        [SerializeField] DeathInformation deathInformation;
        [SerializeField] bool arrestCameraOnDeath = true;

        void OnTriggerEnter(Collider other)
        {
            PlayerDeathEvent.Trigger(deathInformation);
        }
    }
}
