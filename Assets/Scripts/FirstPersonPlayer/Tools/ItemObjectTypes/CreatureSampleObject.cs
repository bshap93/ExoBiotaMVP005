using System;
using FirstPersonPlayer.Combat.AINPC.ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace FirstPersonPlayer.Tools.ItemObjectTypes
{
    [CreateAssetMenu(
        fileName = "CreatureSampleObject",
        menuName = "Scriptable Objects/Items/CreatureSampleObject",
        order = 0)]
    [Serializable]
    public class CreatureSampleObject : MyBaseItem
    {
        [FormerlySerializedAs("CreatureSourceType")]
        public CreatureType creatureSourceType;
    }
}
