using FirstPersonPlayer.ScriptableObjects.BioticAbility;

namespace FirstPersonPlayer.Tools.ItemObjectTypes
{
    [UnityEngine.CreateAssetMenu(
        fileName = "BioticAbilityToolWrapper", menuName = "Scriptable Objects/Items/BioticAbilityToolWrapper",
        order = 0)]
    public class BioticAbilityToolWrapper : BaseTool
    {
        public BioticAbility bioticAbility;
            
        
    }
}
