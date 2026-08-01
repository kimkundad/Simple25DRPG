using System;
using System.Collections.Generic;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Inventory
{
    /// <summary>
    /// Stores player inventory slots and implements item collection.
    /// </summary>
    public sealed class PlayerInventory : MonoBehaviour, IItemCollector
    {
        [Header("Dependencies")]
        [Tooltip("Settings asset that defines inventory capacity.")]
        [SerializeField] private InventorySettings _settings;

        [Header("Debug")]
        [Tooltip("Runtime inventory slots visible for prototype debugging.")]
        [SerializeField] private List<InventorySlot> _slots = new List<InventorySlot>();

        private bool _isValid;
        private bool _duplicateIdWarningLogged;

        /// <summary>
        /// Raised once after a successful inventory change.
        /// </summary>
        public event Action InventoryChanged;

        /// <summary>
        /// Raised once after an item amount is added.
        /// </summary>
        public event Action<ItemData, int> ItemAdded;

        /// <summary>
        /// Raised once after an item amount is removed.
        /// </summary>
        public event Action<ItemData, int> ItemRemoved;

        /// <summary>
        /// Gets the inventory slots.
        /// </summary>
        public IReadOnlyList<InventorySlot> Slots => _slots;

        /// <summary>
        /// Gets the configured inventory capacity.
        /// </summary>
        public int Capacity => _settings != null ? _settings.Capacity : 0;

        private void Awake()
        {
            ValidateDependencies();

            if (!_isValid)
            {
                enabled = false;
                return;
            }

            EnsureSlotCount();
        }

        /// <summary>
        /// Attempts to collect an item amount.
        /// </summary>
        /// <param name="item">Item data being collected.</param>
        /// <param name="amount">Amount being collected.</param>
        /// <returns>True when the full amount was added.</returns>
        public bool TryCollect(ItemData item, int amount)
        {
            if (!CanAdd(item, amount))
            {
#if UNITY_EDITOR
                if (item != null && amount > 0)
                {
                    Debug.Log($"Collection rejected because inventory is full: {amount} x {item.DisplayName}.", this);
                }
#endif
                return false;
            }

            WarnIfDuplicateItemId(item);
            AddUnchecked(item, amount);
            ItemAdded?.Invoke(item, amount);
            InventoryChanged?.Invoke();

#if UNITY_EDITOR
            Debug.Log($"Item added: {amount} x {item.DisplayName}. Current total: {GetAmount(item)}.", this);
#endif

            return true;
        }

        /// <summary>
        /// Gets whether the full amount can be added without partial collection.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="amount">Amount to add.</param>
        /// <returns>True when the entire amount can fit.</returns>
        public bool CanAdd(ItemData item, int amount)
        {
            if (!IsValidItemRequest(item, amount))
            {
                return false;
            }

            int remainingAmount = amount;

            for (int i = 0; i < _slots.Count; i++)
            {
                InventorySlot slot = _slots[i];
                if (slot.CanStack(item))
                {
                    remainingAmount -= Mathf.Min(remainingAmount, slot.RemainingCapacity);
                }
            }

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    remainingAmount -= Mathf.Min(remainingAmount, item.MaxStackSize);
                }
            }

            return remainingAmount <= 0;
        }

        /// <summary>
        /// Gets the total amount of an item across all slots.
        /// </summary>
        /// <param name="item">Item to query.</param>
        /// <returns>Total amount found.</returns>
        public int GetAmount(ItemData item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
            {
                return 0;
            }

            int totalAmount = 0;

            for (int i = 0; i < _slots.Count; i++)
            {
                InventorySlot slot = _slots[i];
                if (!slot.IsEmpty && IsSameItem(slot.Item, item))
                {
                    totalAmount += slot.Amount;
                }
            }

            return totalAmount;
        }

        /// <summary>
        /// Attempts to remove the full requested amount.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <param name="amount">Amount to remove.</param>
        /// <returns>True when the full amount was removed.</returns>
        public bool TryRemove(ItemData item, int amount)
        {
            if (!Contains(item, amount))
            {
                return false;
            }

            int remainingAmount = amount;

            for (int i = 0; i < _slots.Count && remainingAmount > 0; i++)
            {
                InventorySlot slot = _slots[i];
                if (!slot.IsEmpty && IsSameItem(slot.Item, item))
                {
                    remainingAmount -= slot.Remove(remainingAmount);
                }
            }

            ItemRemoved?.Invoke(item, amount);
            InventoryChanged?.Invoke();

#if UNITY_EDITOR
            Debug.Log($"Item removed: {amount} x {item.DisplayName}.", this);
#endif

            return true;
        }

        /// <summary>
        /// Gets whether the inventory contains at least the requested amount.
        /// </summary>
        /// <param name="item">Item to query.</param>
        /// <param name="amount">Required amount.</param>
        /// <returns>True when enough items exist.</returns>
        public bool Contains(ItemData item, int amount)
        {
            return IsValidItemRequest(item, amount) && GetAmount(item) >= amount;
        }

        /// <summary>
        /// Builds a small diagnostic string for prototype debugging.
        /// </summary>
        /// <returns>Readable item totals by display name.</returns>
        public string GetDebugSummary()
        {
            string summary = string.Empty;

            for (int i = 0; i < _slots.Count; i++)
            {
                InventorySlot slot = _slots[i];
                if (slot.IsEmpty)
                {
                    continue;
                }

                if (summary.Length > 0)
                {
                    summary += "\n";
                }

                summary += $"{slot.Item.DisplayName} x{slot.Amount}";
            }

            return summary;
        }

        private void OnValidate()
        {
            if (_settings != null)
            {
                EnsureSlotCount();
            }
        }

        private void AddUnchecked(ItemData item, int amount)
        {
            int remainingAmount = amount;

            for (int i = 0; i < _slots.Count && remainingAmount > 0; i++)
            {
                InventorySlot slot = _slots[i];
                if (slot.CanStack(item))
                {
                    remainingAmount = slot.Add(item, remainingAmount);
                }
            }

            for (int i = 0; i < _slots.Count && remainingAmount > 0; i++)
            {
                InventorySlot slot = _slots[i];
                if (slot.IsEmpty)
                {
                    remainingAmount = slot.Add(item, remainingAmount);
                }
            }
        }

        private void EnsureSlotCount()
        {
            int targetCapacity = Capacity;
            if (targetCapacity <= 0)
            {
                return;
            }

            while (_slots.Count < targetCapacity)
            {
                _slots.Add(new InventorySlot());
            }

            while (_slots.Count > targetCapacity)
            {
                _slots.RemoveAt(_slots.Count - 1);
            }
        }

        private void ValidateDependencies()
        {
            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(PlayerInventory)} on {name} requires InventorySettings.", this);
                return;
            }

            _isValid = true;
        }

        private void WarnIfDuplicateItemId(ItemData item)
        {
#if UNITY_EDITOR
            if (_duplicateIdWarningLogged || item == null || string.IsNullOrWhiteSpace(item.Id))
            {
                return;
            }

            for (int i = 0; i < _slots.Count; i++)
            {
                InventorySlot slot = _slots[i];
                if (!slot.IsEmpty && slot.Item != item && slot.Item.Id == item.Id)
                {
                    _duplicateIdWarningLogged = true;
                    Debug.LogWarning($"Inventory found different ItemData assets using duplicate ID '{item.Id}'.", this);
                    return;
                }
            }
#endif
        }

        private static bool IsValidItemRequest(ItemData item, int amount)
        {
            return item != null && amount > 0 && !string.IsNullOrWhiteSpace(item.Id);
        }

        private static bool IsSameItem(ItemData first, ItemData second)
        {
            if (first == null || second == null)
            {
                return false;
            }

            if (first == second)
            {
                return true;
            }

            bool hasFirstId = !string.IsNullOrWhiteSpace(first.Id);
            bool hasSecondId = !string.IsNullOrWhiteSpace(second.Id);
            return hasFirstId && hasSecondId && first.Id == second.Id;
        }
    }
}
