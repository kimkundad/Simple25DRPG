using UnityEngine;

namespace Simple25DRPG.Items
{
    /// <summary>
    /// Defines reusable item data shared by loot, pickups, and future inventory systems.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Simple 2.5D RPG/Items/Item Data")]
    public sealed class ItemData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable item identifier used by save data and future inventory systems.")]
        [SerializeField] private string _id = "item_id";

        [Tooltip("Display name shown to players.")]
        [SerializeField] private string _displayName = "Item";

        [Header("Presentation")]
        [Tooltip("Optional icon used by future UI.")]
        [SerializeField] private Sprite _icon;

        [Tooltip("World pickup prefab spawned when this item drops.")]
        [SerializeField] private GameObject _pickupPrefab;

        [Header("Stacking")]
        [Tooltip("Maximum number of this item that can stack together.")]
        [Min(1)]
        [SerializeField] private int _maxStackSize = 99;

        /// <summary>
        /// Gets the stable item identifier.
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Gets the player-facing display name.
        /// </summary>
        public string DisplayName => _displayName;

        /// <summary>
        /// Gets the optional inventory icon.
        /// </summary>
        public Sprite Icon => _icon;

        /// <summary>
        /// Gets the world pickup prefab.
        /// </summary>
        public GameObject PickupPrefab => _pickupPrefab;

        /// <summary>
        /// Gets the maximum stack size.
        /// </summary>
        public int MaxStackSize => _maxStackSize;

        private void OnValidate()
        {
            if (_id != null)
            {
                _id = _id.Trim();
            }

            _maxStackSize = Mathf.Max(1, _maxStackSize);

#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(_id))
            {
                Debug.LogWarning($"{nameof(ItemData)} '{name}' has an empty ID.", this);
            }
#endif
        }
    }
}
