using System;
using Manager.SceneManagers.Dock;
using Overview.OverviewMode.ScriptableObjectDefinitions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OWPData.ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "DockDefinitionLocationTable",
        menuName = "Scriptable Objects/Dock/DockDefinitionLocationTable")]
    public class DockDefinitionLocationTable : ScriptableObject
    {
        public DockDefinitionEntry[] dockDefinitions;
        public LocationDefinitionTableEntry[] locationDefinitions;

        public DockDefinition GetDockDefinition(string dockId)
        {
            foreach (var entry in dockDefinitions)
                if (entry.dockId == dockId)
                    return entry.dockDefinition;

            return null;
        }

        public DockOvLocationDefinition GetLocationDefinition(string locationId)
        {
            foreach (var entry in locationDefinitions)
                if (entry.locationId == locationId)
                    return entry.locationDefinition;

            return null;
        }
    }


    [Serializable]
    public struct DockDefinitionEntry
    {
        [ValueDropdown("GetDockIdOptions")] public string dockId;
        public DockDefinition dockDefinition;

        static string[] GetDockIdOptions()
        {
            return DockManager.GetDockIdOptions();
        }
    }

    [Serializable]
    public struct LocationDefinitionTableEntry
    {
        public string locationId;
        public DockOvLocationDefinition locationDefinition;
    }
}
