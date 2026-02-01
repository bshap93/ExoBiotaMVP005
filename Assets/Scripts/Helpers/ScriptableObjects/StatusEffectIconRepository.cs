using UnityEngine;

namespace Helpers.ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "UIIconRepository", menuName = "Scriptable Objects/Helpers/UIIconRepository",
        order = 1)]
    public class StatusEffectIconRepository : ScriptableObject
    {
        public Sprite keyboardInteractIcon;

        public Sprite heartIcon;
        public Sprite lungsIcon;
        public Sprite skinIcon;
        public Sprite eyesIcon;
        public Sprite brainIcon;

        public Sprite InteractIcon()
        {
            return keyboardInteractIcon;
        }
    }
}
