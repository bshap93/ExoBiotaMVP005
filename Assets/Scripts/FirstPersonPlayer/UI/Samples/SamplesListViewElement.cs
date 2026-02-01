// SampleDetailsPanel.cs

using System.Text;
using FirstPersonPlayer.ScriptableObjects;
using Manager;
using Manager.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPersonPlayer.Interactable.Samples
{
    public class SamplesListViewElement : MonoBehaviour
    {
        [Header("Basic")] [SerializeField] private Image speciesIcon;

        [SerializeField] private TMP_Text speciesName;
        [SerializeField] private TMP_Text subtitle;

        [Header("Markers")] [SerializeField] private TMP_Text markersBlock; // simple multiline text for now

        // TODO: replace with your real analysis gate later
        private bool IsSequenced(string speciesId)
        {
            // For now: biologicals are *not recognized* until analysis; always false here.
            // Later: hook to a BiologyAnalysisManager.HasSequenced(speciesId)
            return false;
        }

        public void Bind(BioOrganismSample sample)
        {
            if (sample == null)
            {
                Clear();
                return;
            }

            var bioMgr = BioSamplesManager.Instance; // singleton is set in Awake

            if (!sample.isKnown)
            {
                speciesName.text = "Unknown Organism";
                speciesIcon.sprite = ExaminationManager.Instance?.iconRepository.sampleCartridgeIcon;
                subtitle.text = "Requires sequencing and analysis";
                markersBlock.text = "";
                return;
            }

            // Known → render using the species log
            if (bioMgr.TryGetSampleLog(sample, out var log))
            {
                speciesName.text = string.IsNullOrEmpty(log.speciesName) ? "Organism" : log.speciesName;
                subtitle.text = $"Samples contributing: {log.sampleIds.Count}";

                var sb = new StringBuilder();
                foreach (var kv in log.markerAmounts) // marker → amount (float)
                    sb.AppendLine($"{kv.Key}: {kv.Value * 100f:0.#}%");
                markersBlock.text = sb.ToString();
            }
        }


        public void Clear()
        {
            speciesName.text = "";
            subtitle.text = "";
            markersBlock.text = "";
            speciesIcon.sprite = null;
        }
    }
}