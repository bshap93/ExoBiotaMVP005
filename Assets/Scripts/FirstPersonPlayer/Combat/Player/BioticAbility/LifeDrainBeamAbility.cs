using UnityEngine;

namespace FirstPersonPlayer.Combat.Player.BioticAbility
{
    public class LifeDrainBeamAbility : MonoBehaviour, IRuntimeBioticAbility
    {
        [SerializeField] GameObject physicalRoot;
        [Header("Visual Effects")] [SerializeField]
        Transform muzzlePosition;
        [SerializeField] GameObject muzzleFlashPrefab;
        [SerializeField] GameObject hitSparksPrefab;
        [SerializeField] GameObject missSparksPrefab;
        [SerializeField] float beamWidth = 0.03f;
        [SerializeField] float beamDuration = 0.1f;
        [SerializeField] Color beamColor = Color.green;
        [SerializeField] LineRenderer lifeDrainBeamLineRenderer;
        Vector3 _initialLocalPos;
        bool _isActive;

        GameObject _muzzleFlashInstance;
        ParticleSystem[] _muzzleParticles;

        void Awake()
        {
            _initialLocalPos = physicalRoot.transform.localPosition;

            SetupBeamRenderer();

            if (muzzleFlashPrefab != null && muzzlePosition != null)
            {
                _muzzleFlashInstance = Instantiate(muzzleFlashPrefab, muzzlePosition.position, muzzlePosition.rotation);
                _muzzleFlashInstance.transform.SetParent(muzzlePosition);
                _muzzleParticles = _muzzleFlashInstance.GetComponentsInChildren<ParticleSystem>();

                // Stop all particles initially
                foreach (var ps in _muzzleParticles)
                    if (ps.isPlaying)
                        ps.Stop();
            }
        }

        void OnDestroy()
        {
            // Clean up persistent muzzle flash
            if (_muzzleFlashInstance != null) Destroy(_muzzleFlashInstance);
        }
        public void Activate(FirstPersonPlayer.ScriptableObjects.BioticAbility.BioticAbility abilityData,
            Transform originTransform)
        {
            _isActive = true;
            PlayMuzzleFlash(originTransform);

            Debug.Log("Activating biotic ability: " + abilityData.name);
        }
        public void Deactivate()
        {
            _isActive = false;
            Debug.Log("Deactivating biotic ability");
        }
        public bool IsActive()
        {
            return _isActive;
        }

        void PlayMuzzleFlash(Transform originTransform)
        {
            if (muzzleFlashPrefab == null) return;

            // Spawn a fresh instance each time
            var flash = Instantiate(muzzleFlashPrefab, originTransform.position, originTransform.rotation);

            // Get and play all particle systems
            var particles = flash.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in particles)
                ps.Play();

            // Auto-destroy after 2 seconds
            Destroy(flash, 2f);
        }


        void SetupBeamRenderer()
        {
            if (lifeDrainBeamLineRenderer == null)
            {
                Debug.LogError("No LineRenderer assigned to LifeDrainBeamAbility!");
                return;
            }

            if (lifeDrainBeamLineRenderer != null)
            {
                lifeDrainBeamLineRenderer.enabled = false;
                lifeDrainBeamLineRenderer.startColor = beamColor;
                lifeDrainBeamLineRenderer.endColor = beamColor;
                lifeDrainBeamLineRenderer.startWidth = beamWidth;
                lifeDrainBeamLineRenderer.endWidth = beamWidth * 0.8f;
                lifeDrainBeamLineRenderer.positionCount = 2;

                lifeDrainBeamLineRenderer.gameObject.SetActive(false);
            }
        }
    }
}
