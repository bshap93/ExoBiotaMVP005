using System.Collections;
using Events;
using Helpers.Events.Dialog;
using Manager.DialogueScene;
using Manager.SceneManagers.Dock;
using MoreMountains.Tools;
using Sirenix.Utilities;
using UnityEngine;

namespace Overview.NPC.Dialogue
{
    public class DialogueEventBridge : MonoBehaviour, MMEventListener<OverviewLocationEvent>,
        MMEventListener<FirstPersonDialogueEvent>
    {
        [SerializeField] NpcDatabase npcDatabase;
        [SerializeField] DialogueManager dialogueManager;

        public void OnEnable()
        {
            this.MMEventStartListening<OverviewLocationEvent>();
            this.MMEventStartListening<FirstPersonDialogueEvent>();
        }

        public void OnDisable()
        {
            this.MMEventStopListening<OverviewLocationEvent>();
            this.MMEventStopListening<FirstPersonDialogueEvent>();
        }

        public void OnMMEvent(FirstPersonDialogueEvent e)
        {
            if (e.Type != FirstPersonDialogueEventType.StartDialogue) return;

            if (!npcDatabase.TryGet(e.NPCId, out var def))
            {
                Debug.LogWarning($"No NPC with id {e.NPCId}");
                return;
            }

            if (!e.StartNodeOverride.IsNullOrWhitespace())
                dialogueManager.OpenNPCDialogue(def, startNodeOverride: e.StartNodeOverride, autoClose: true);
            else
                dialogueManager.OpenNPCDialogue(def);
        }

        public void OnMMEvent(OverviewLocationEvent e)
        {
            if (e.LocationActionType != LocationActionType.Approach) return;

            var locationDefinition = DockManager.Instance.GetLocationDefinition(e.LocationId);

            if (locationDefinition == null)
            {
                Debug.LogWarning($"No location definition for {e.LocationId}");
                return;
            }

            if (e.LocationType == LocationType.Dirigible)

                if (locationDefinition.npcInResidenceId == "None")
                {
                    Debug.LogWarning($"No npc in residence at {e.LocationId}");
                    StartCoroutine(WaitAndThenRetreat(e));
                    return;
                }

            var startNode = e.StartNodeOverride;


            // LocationId IS the NPC id now
            if (!npcDatabase.TryGet(locationDefinition.npcInResidenceId, out var def))
            {
                Debug.LogWarning($"No NPC with id {locationDefinition.npcInResidenceId}");
                return;
            }


            dialogueManager.OpenNPCDialogue(
                def, null, true,
                string.IsNullOrEmpty(startNode) ? null : startNode);
        }


        IEnumerator WaitAndThenRetreat(OverviewLocationEvent e)
        {
            yield return new WaitForSeconds(0.1f);
            OverviewLocationEvent.Trigger(e.LocationType, LocationActionType.RetreatFrom, e.LocationId, null);
        }
    }
}
