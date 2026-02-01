using System.Collections.Generic;
using Helpers.Events;
using Helpers.Events.UI;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace FirstPersonPlayer
{
    public class ElevatorSceneChangeTrigger : MonoBehaviour
    {
        [ValueDropdown(nameof(GetSceneNames))] [SerializeField]
        string sceneToLoad;


        [ValueDropdown(nameof(GetSpawnPoints))] [SerializeField]
        string spawnPointId;

        [FormerlySerializedAs("BridgeName")] public string bridgeName;


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FirstPersonPlayer"))
            {
                SceneTransitionUIEvent.Trigger(SceneTransitionUIEventType.Show);
                SaveDataEvent.Trigger();

                // SpawnEvent.Trigger(
                //     SpawnEventType.ToMine, sceneToLoad, GameMode.FirstPerson,
                //     spawnPointId
                // );
                //
                SceneManager.LoadScene(bridgeName);
            }
        }

        static IEnumerable<string> GetSceneNames()
        {
            return PlayerSpawnManager.GetSceneOptions();
        }

        IEnumerable<string> GetSpawnPoints()
        {
            return PlayerSpawnManager.GetSpawnPointIdOptions();
        }
    }
}
