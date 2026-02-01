using DG.Tweening;
using Helpers.Events;
using Helpers.Events.Combat;
using MoreMountains.Tools;
using Rewired.Integration.Cinemachine3;
using Unity.Cinemachine;
using UnityEngine;

namespace Helpers
{
    public class FPCameraEventHandler : MonoBehaviour, MMEventListener<PlayerDamageEvent>,
        MMEventListener<PlayerDeathEvent>
    {
        [SerializeField] CinemachineCamera cinemachineCamera;
        // [SerializeField] DOTweenAnimation dOTweenAnimation;
        [SerializeField] RewiredCinemachineInputAxisController axisController;

        void OnEnable()
        {
            this.MMEventStartListening<PlayerDamageEvent>();
            this.MMEventStartListening<PlayerDeathEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<PlayerDamageEvent>();
            this.MMEventStopListening<PlayerDeathEvent>();
        }

        public void OnMMEvent(PlayerDamageEvent e)
        {
            if (e.HitType == PlayerDamageEvent.HitTypes.CriticalHit)
                ShakeCamera(0.1f, 0.05f);
            // dOTweenAnimation.DORestart();
            else if (e.HitType == PlayerDamageEvent.HitTypes.Normal) ShakeCamera(0.05f, 0.05f);
            // dOTweenAnimation.DORestart();
        }
        public void OnMMEvent(PlayerDeathEvent eventType)
        {
            axisController.enabled = false;
        }

        void ShakeCamera(float intensity, float duration)
        {
            transform.DOShakePosition(duration, new Vector3(intensity, intensity, intensity))
                .SetEase(Ease.InOutElastic).SetLoops(2, LoopType.Yoyo);
        }
    }
}
