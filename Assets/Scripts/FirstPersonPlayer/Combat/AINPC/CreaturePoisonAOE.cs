using System.Collections;
using UnityEngine;

namespace FirstPersonPlayer.Combat.AINPC
{
    public class CreaturePoisonAOE : MonoBehaviour
    {
        [SerializeField] GameObject poisonAOEEffect;
        [SerializeField] float effectDuration;
        [SerializeField] ParticleSystem[] poisonEffectParticles;

        bool _isActivelyPoisoning;

        void Start()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (!_isActivelyPoisoning) return;
            if (other.CompareTag("FirstPersonPlayer"))
            {
                // Apply poisoning to player
            }
        }

        public void ReleasePoison()
        {
            if (_isActivelyPoisoning) return;
            _isActivelyPoisoning = true;
            poisonAOEEffect.SetActive(true);
            foreach (var particle in poisonEffectParticles) particle.Play();
            StartCoroutine(Cleanup());
        }
        
        IEnumerator Cleanup()
        {
            yield return new WaitForSeconds(effectDuration);
            _isActivelyPoisoning = false;
            foreach (var particle in poisonEffectParticles) particle.Stop();
            poisonAOEEffect.SetActive(false);
        }
    }
}
