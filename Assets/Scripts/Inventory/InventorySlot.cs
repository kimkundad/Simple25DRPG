using System;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Inventory
{
    /// <summary>
    /// Stores one inventory item stack.
    /// </summary>
    [Serializable]
    public sealed class InventorySlot
    {
        [SerializeField] private ItemData _item;
        [Min(0)]
        [SerializeField] private int _amount;

        /// <summary>
        /// Gets the item stored in this slot.
        /// </summary>
        public ItemData Item => _item;

        /// <summary>
        /// Gets the amount stored in this slot.
        /// </summary>
        public int Amount => _amount;

        /// <summary>
        /// Gets whether this slot contains no item.
        /// </summary>
        public bool IsEmpty => _item == null || _amount <= 0;

        /// <summary>
        /// Gets whether this slot cannot accept more of its current item.
        /// </summary>
        public bool IsFull => !IsEmpty && _amount >= _item.MaxStackSize;

        /// <summary>
        /// Gets how many more items this slot can accept.
        /// </summary>
        public int RemainingCapacity => IsEmpty ? 0 : Mathf.Max(0, _item.MaxStackSize - _amount);

        /// <summary>
        /// Adds as much of the amount as this slot can accept.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="amount">Amount to add.</param>
        /// <returns>Amount that did not fit.</returns>
        public int Add(ItemData item, int amount)
        {
            if (item == null || amount <= 0)
            {
                return Mathf.Max(0, amount);
            }

            if (IsEmpty)
            {
                _item = item;
                _amount = 0;
            }
            else if (!CanStack(item))
            {
                return amount;
            }

            int remainingCapacity = Mathf.Max(0, _item.MaxStackSize - _amount);
            int acceptedAmount = Mathf.Min(amount, remainingCapacity);
            _amount += acceptedAmount;
            return amount - acceptedAmount;
        }

        /// <summary>
        /// Removes as much of the requested amount as this slot contains.
        /// </summary>
        /// <param name="amount">Amount to remove.</param>
        /// <returns>Amount actually removed.</returns>
        public int Remove(int amount)
        {
            if (IsEmpty || amount <= 0)
            {
                return 0;
            }

            int removedAmount = Mathf.Min(amount, _amount);
            _amount -= removedAmount;

            if (_amount <= 0)
            {
                Clear();
            }

            return removedAmount;
        }

        /// <summary>
        /// Clears this slot.
        /// </summary>
        public void Clear()
        {
            _item = null;
            _amount = 0;
        }

        /// <summary>
        /// Gets whether this slot can stack the provided item.
        /// </summary>
        /// <param name="item">Item to compare.</param>
        /// <returns>True when this slot can accept the item.</returns>
        public bool CanStack(ItemData item)
        {
            if (item == null || IsEmpty || IsFull)
            {
                return false;
            }

            if (_item == item)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(_item.Id) && _item.Id == item.Id;
        }
    }
}
