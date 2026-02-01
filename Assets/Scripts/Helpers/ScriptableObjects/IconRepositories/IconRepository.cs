using UnityEngine;
using UnityEngine.Serialization;

namespace Helpers.ScriptableObjects.IconRepositories
{
    [CreateAssetMenu(fileName = "IconRepository", menuName = "Scriptable Objects/Helpers/IconRepository", order = 0)]
    public class IconRepository : ScriptableObject
    {
        [FormerlySerializedAs("SaveConsoleIcon")]
        public Sprite saveConsoleIcon;

        [FormerlySerializedAs("BioOrganismIcon")]
        public Sprite bioOrganismIcon;

        public Sprite sampleCartridgeIcon;
        public Sprite mineOreIcon;
        public Sprite pickupIcon;
        public Sprite usableConsoleIcon;
        public Sprite dockIcon;
        public Sprite doorIcon;
        public Sprite buttonIcon;
        public Sprite pushIcon;
        public Sprite pickaxeIcon;
        public Sprite liquidSampleIcon;
        public Sprite airSampleIcon;
        public Sprite axeIcon;
        public Sprite addItemIcon;
        public Sprite removeItemIcon;
        public Sprite addLiquidSampleIcon;
        public Sprite mediStatHubIcon;
        public Sprite mediStatHubRestIcon;
        public Sprite ladderIcon;
        public Sprite climbIcon;
    }
}
