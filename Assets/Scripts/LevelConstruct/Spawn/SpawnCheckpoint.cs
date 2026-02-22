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
    public class SpawnCheckpoint : MonoBehaviour
    {
        const float SpawnGraceDuration = 10f;

        static float _spawnedAtRealtime = float.MinValue;
#if UNITY_EDITOR
        [ValueDropdown("GetListOfTags")] [SerializeField]
#endif
        string playerPawnTag = "FirstPersonPlayer";
        [FormerlySerializedAs("_point")] [SerializeField]
        SpawnPoint point;

        [SerializeField] bool useAsAutoSavePoint;

        static bool IsInSpawnGracePeriod =>
            Time.realtimeSinceStartup - _spawnedAtRealtime < SpawnGraceDuration;


        void Awake()
        {
            if (point == null)
                point = GetComponent<SpawnPoint>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(playerPawnTag)) return;
            if (!other.CompareTag(playerPawnTag)) return;

            if (IsInSpawnGracePeriod)
            {
                Debug.Log("[SpawnCheckpoint] Trigger suppressed — within spawn grace period.");
                return;
            }

            var globalSettingsMgr = GlobalSettingsManager.Instance;
            if (globalSettingsMgr == null)
            {
                Debug.LogError("[SpawnCheckpoint] No GlobalSettingsManager found in scene.");
                return;
            }

            if (!globalSettingsMgr.AutoSaveAtCheckpoints)
                // Debug.Log("[SpawnCheckpoint] Autosave at the checkpoint is disabled in Global Settings.");
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
            // CheckpointEvent.Trigger(spawnInfo);
        }

        public static void NotifySpawned()
        {
            _spawnedAtRealtime = Time.realtimeSinceStartup;
        }

#if UNITY_EDITOR
        public static string[] GetListOfTags()
        {
            return InternalEditorUtility.tags;
        }
#endif
    }
}
