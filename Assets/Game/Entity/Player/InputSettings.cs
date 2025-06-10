using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "Game/Entity/Player/Input", order = 1)]
    public class InputSettings : ScriptableObject
    {
        [field: SerializeField]
        public MenuKey menu { get; private set; }
        [System.Serializable]
        public struct MenuKey
        {
            [field: SerializeField]
            public KeyCode Open { get; private set; }
        }
        [field: SerializeField]
        public MoveKey move { get; private set; }
        [System.Serializable]
        public struct MoveKey
        {
            [field: SerializeField]
            public float SensativityX { get; private set; }
            [field: SerializeField]
            public float SensativityY { get; private set; }
            [field: SerializeField]
            public KeyCode Sprint { get; private set; }
            [field: SerializeField]
            public KeyCode Crawl { get; private set; }
            [field: SerializeField]
            public KeyCode Forward { get; private set; }
            [field: SerializeField]
            public KeyCode Back { get; private set; }
            [field: SerializeField]
            public KeyCode Left { get; private set; }
            [field: SerializeField]
            public KeyCode Right { get; private set; }
            [field: SerializeField]
            public KeyCode Jump { get; private set; }
        }
        [field: SerializeField]
        public InventoryKey Inventory { get; private set; }
        [System.Serializable]
        public struct InventoryKey
        {
            [field: SerializeField]
            public KeyCode[] Switch { get; private set; }
            [field: SerializeField]
            public KeyCode Grab { get; private set; }
            [field: SerializeField]
            public KeyCode Put { get; private set; }
            [field: SerializeField]
            public KeyCode Drop { get; private set; }
        }
        [field: SerializeField]
        public ItemKey item { get; private set; }
        [System.Serializable]
        public struct ItemKey
        {
            [field: SerializeField]
            public KeyCode BaseUse { get; private set; }
            [field: SerializeField]
            public KeyCode ActiveUse { get; private set; }
            [field: SerializeField]
            public KeyCode PassiveUse { get; private set; }
            [field: SerializeField]
            public KeyCode Reload { get; private set; }
        }
        [Header("Other")]
        [field: SerializeField]
        public KeyCode Interact = KeyCode.F;
    }
}
