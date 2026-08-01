using System;
using Simple25DRPG.Inventory;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Compatibility adapter that forwards item collection to PlayerInventory.
    /// </summary>
    public sealed class PlayerItemCollector : MonoBehaviour, IItemCollector
    {
        [Header("Dependencies")]
        [Tooltip("Inventory backend that owns all player item storage.")]
        [SerializeField] private PlayerInventory _inventory;

        /// <summary>
        /// Raised when this adapter forwards a successful collection.
        /// </summary>
        public event Action<ItemData, int> ItemCollected;

        private void Awake()
        {
            if (_inventory == null)
            {
                _inventory = GetComponent<PlayerInventory>();
            }

            if (_inventory == null)
            {
                Debug.LogWarning($"{nameof(PlayerItemCollector)} on {name} requires PlayerInventory.", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_inventory != null)
            {
                _inventory.ItemAdded += HandleItemAdded;
            }
        }

        private void OnDisable()
        {
            if (_inventory != null)
            {
                _inventory.ItemAdded -= HandleItemAdded;
            }
        }

        /// <summary>
        /// Attempts to collect an item amount by forwarding to PlayerInventory.
        /// </summary>
        /// <param name="item">Item data being collected.</param>
        /// <param name="amount">Amount being collected.</param>
        /// <returns>True when the inventory accepted the full amount.</returns>
        public bool TryCollect(ItemData item, int amount)
        {
            return _inventory != null && _inventory.TryCollect(item, amount);
        }

        /// <summary>
        /// Gets the currently collected amount from PlayerInventory.
        /// </summary>
        /// <param name="item">Item to query.</param>
        /// <returns>Collected amount, or zero when unavailable.</returns>
        public int GetAmount(ItemData item)
        {
            return _inventory != null ? _inventory.GetAmount(item) : 0;
        }

        private void HandleItemAdded(ItemData item, int amount)
        {
            ItemCollected?.Invoke(item, amount);
        }
    }
}
