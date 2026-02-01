using Animancer;
using UnityEngine;

namespace FirstPersonPlayer.FPNPCs.AlienNPC
{
    public class AlienNPCAnimancerController : MonoBehaviour
    {
        [SerializeField] AnimationClip[] alienIdleAnimations;
        [SerializeField] AnimancerComponent animancerComponent;
        void Start()
        {
            PlaySequenceOfIdleAnimations();
        }

        void PlaySequenceOfIdleAnimations()
        {
            if (alienIdleAnimations.Length == 0) return;

            var sequence = new AnimancerState[alienIdleAnimations.Length];
            for (var i = 0; i < alienIdleAnimations.Length; i++)
            {
                sequence[i] = animancerComponent.Play(alienIdleAnimations[i]);
                var i1 = i;
                sequence[i].Events(this).OnEnd = () =>
                {
                    var nextIndex = (i1 + 1) % alienIdleAnimations.Length;
                    animancerComponent.Play(alienIdleAnimations[nextIndex]);
                };
            }

            animancerComponent.Play(alienIdleAnimations[0]);
        }
    }
}
