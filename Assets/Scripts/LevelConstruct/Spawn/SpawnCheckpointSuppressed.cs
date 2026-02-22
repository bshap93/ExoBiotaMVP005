using Helpers.Events;
using Manager;
using Manager.Global;
using Manager.Settings;
using Sirenix.OdinInspector;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace LevelConstruct.Spawn
{
    public class SpawnCheckpointSuppressed : MonoBehaviour
    {
        // Set by the spawn system just before the player is placed in the world.
        // All checkpoints will suppress their next trigger until the player exits their collider.
        static bool _spawnJustOccurred;

#if UNITY_EDITOR
        [ValueDropdown("GetListOfTags")] [SerializeField]
#endif
        string playerPawnTag = "FirstPersonPlayer";
        [FormerlySerializedAs("_point")] [SerializeField]
        SpawnPoint point;

        [SerializeField] bool useAsAutoSavePoint;

        // Per-instance: true while we are waiting for the player to leave after a spawn.
        bool _suppressUntilExit;


        void Awake()
        {
            if (point == null)
                point = GetComponent<SpawnPoint>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(playerPawnTag)) return;
            if (!other.CompareTag(playerPawnTag)) return;

            // If a spawn just happened, begin suppressing this checkpoint until the player leaves.
            if (_spawnJustOccurred)
            {
                _suppressUntilExit = true;
                _spawnJustOccurred = false; // consumed — only one checkpoint needs to catch this
                Debug.Log("[SpawnCheckpoint] Trigger suppressed — player just spawned, waiting for exit.");
                return;
            }

            // Still inside from a previous spawn, not yet exited.
            if (_suppressUntilExit) return;

            var globalSettingsMgr = GlobalSettingsManager.Instance;
            if (globalSettingsMgr == null)
            {
                Debug.LogError("[SpawnCheckpoint] No GlobalSettingsManager found in scene.");
                return;
            }

            if (!globalSettingsMgr.AutoSaveAtCheckpoints)
                return;

            if (!useAsAutoSavePoint)
            {
                Debug.Log("[SpawnCheckpoint] This checkpoint is not set to be used as an autosave point.");
                return;
            }

            var spawnInfo = new SpawnInfo
            {
                SceneName = gameObject.scene.name,
                Mode = GameStateManager.Instance.CurrentMode,
                SpawnPointId = point.Id
            };

            PlayerSpawnManager.Instance.Save(spawnInfo);

            SaveDataEvent.Trigger();

            AlertEvent.Trigger(
                AlertReason.AutoSave, "Saved at checkpoint: " + point.Id, "Checkpoint Reached", AlertType.Basic, 2f);
        }

        void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(playerPawnTag)) return;
            if (!other.CompareTag(playerPawnTag)) return;

            // Grace period is over — the player has physically left the spawn zone.
            _suppressUntilExit = false;
        }

        /// <summary>
        ///     Call this immediately before/after the player is spawned so that any
        ///     checkpoint the player lands inside ignores that first trigger.
        /// </summary>
        public static void NotifySpawned()
        {
            _spawnJustOccurred = true;
        }

#if UNITY_EDITOR
        public static string[] GetListOfTags()
        {
            return InternalEditorUtility.tags;
        }
#endif
    }
}
