using FirstPersonPlayer.Interactable;
using Lightbug.CharacterControllerPro.Core;
using Manager.Global;
using NodeCanvas.Framework;
using UnityEngine;

namespace FirstPersonPlayer.Combat.AINPC
{
    public class AssignPlayerToBT : MonoBehaviour
    {
        public Blackboard blackboard;
        public float delay = 0.5f;


        void Start()
        {
            Invoke(nameof(Assign), delay);
        }

        void Assign()
        {
            if (blackboard == null)
                blackboard = GetComponent<Blackboard>();


            // Get the top-level PlayerRoot
            var root = GameStateManager.Instance.PlayerRoot;
            if (root == null)
            {
                Debug.LogError("No PlayerRoot found!");
                return;
            }

            // Get the first active child (your actual moving pawn)
            Transform movingPawn = null;

            foreach (Transform child in root)
                if (child.gameObject.activeInHierarchy)
                {
                    movingPawn = child;
                    break;
                }

            if (movingPawn == null)
            {
                var player = FindFirstObjectByType<PlayerInteraction>();

                if (player == null)
                {
                    Debug.LogError("No moving player pawn found under PlayerRoot.");
                    return;
                }

                movingPawn = player.gameObject.transform;
            }

            var capsuleScaler = movingPawn.GetComponentInChildren<CharacterGraphicsScaler>();

            if (capsuleScaler == null)
            {
                Debug.LogError("No CharacterGraphicsScaler found on the moving player pawn.");
                return;
            }

            // Assign THIS instead of the root
            blackboard.SetVariableValue("playerTransform", movingPawn);
            blackboard.SetVariableValue("capsule", capsuleScaler.gameObject);
        }
    }
}
