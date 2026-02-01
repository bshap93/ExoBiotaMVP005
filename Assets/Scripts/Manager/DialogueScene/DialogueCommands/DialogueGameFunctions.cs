using UnityEngine;
using Yarn.Unity;

namespace Manager.DialogueScene.DialogueCommands
{
    public class DialogueGameFunctions : MonoBehaviour
    {
        [Tooltip("If not assigned, will try DialogueManager.Instance.dialogueRunner")]
        public DialogueRunner dialogueRunner;
        AttributesManager _attributesManager;


        [YarnFunction("get_dexterity")]
        public static int GetDexterity()
        {
            return AttributesManager.Instance.Dexterity;
        }

        [YarnFunction("get_agility")]
        public static int GetAgility()
        {
            return AttributesManager.Instance.Agility;
        }


        [YarnFunction("get_strength")]
        public static int GetStrength()
        {
            return AttributesManager.Instance.Strength;
        }

        [YarnFunction("get_mental_toughness")]
        public static int GetMentalToughness()
        {
            return AttributesManager.Instance.MentalToughness;
        }

        [YarnFunction("get_biotic_level")]
        public static int GetBioticLevel()
        {
            return AttributesManager.Instance.Exobiotic;
        }

        public class AttributFunctions
        {
        }
    }
}
