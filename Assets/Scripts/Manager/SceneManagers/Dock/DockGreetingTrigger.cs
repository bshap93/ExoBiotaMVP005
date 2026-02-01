using Events;
using Helpers.Events;
using Manager.DialogueScene;
using MoreMountains.Tools;
using Overview.NPC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager.SceneManagers.Dock
{
    /// Attaclh to the Core scene (lives across modes)
    public class DockGreetingTrigger : MonoBehaviour, MMEventListener<DockingEvent>
    {
        [SerializeField] DialogueManager dialogueManager;
        [SerializeField] NpcDatabase npcDatabase;

        [ValueDropdown("GetNpcGreeterIdOptions")] [SerializeField]
        string npcGreeterId;

        [FormerlySerializedAs("DockId")] [ValueDropdown("GetDockIdOptions")]
        public string dockId;

        public void OnEnable()
        {
            this.MMEventStartListening();
        }

        public void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(DockingEvent e)
        {
            if (e.EventType != DockingEventType.DockAtLocation) return;
            if (e.DockDefinition.dockId != dockId) return;

            // 1. build $clancyGreeted, $adaGreeted, …
            var varKey = BuildGreetingFlag(npcGreeterId);

            // 2. skip if already greeted
            var storage = dialogueManager.variableStorage;
            if (storage.TryGetValue<bool>(varKey, out var greeted) && greeted)
                return;

            OverviewLocationEvent.Trigger(
                LocationType.Any,
                LocationActionType.BeApproached, null, null);

            // 3. open the dialogue
            if (npcDatabase.TryGet(npcGreeterId, out var def))
                dialogueManager.OpenNPCDialogue(def, autoClose: true);
        }

        static string[] GetNpcGreeterIdOptions()
        {
            return DialogueManager.GetAllNpcIdOptions();
        }

        static string[] GetDockIdOptions()
        {
            return DockManager.GetDockIdOptions();
        }

        static string BuildGreetingFlag(string npcId)
        {
            // UpperCamelName  -> lowerCamelGreeted
            if (string.IsNullOrEmpty(npcId))
                return "$greeted";

            var lowerCamel = char.ToLowerInvariant(npcId[0]) + npcId.Substring(1) + "Greeted";
            return $"${lowerCamel}";
        }
    }
}
