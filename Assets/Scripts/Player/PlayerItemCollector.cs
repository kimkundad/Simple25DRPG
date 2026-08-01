using System;
using System.Collections.Generic;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Temporary in-memory item collector used until the inventory system exists.
    /// </summary>
    public sealed class PlayerItemCollector : MonoBehaviour, IItemCollector
    {
        private readonly Dictionary<string, int> _amountsByItemId = new Dictionary<string, int>();

        /// <summary>
        /// Raised when this player successfully collects an item amount.
        /// </summary>
        public event Action<ItemData, int> ItemCollected;

        /// <summary>
        /// Attempts to collect an item amount.
        /// </summary>
        /// <param name="item">Item data being collected.</param>
        /// <param name="amount">Amount being collected.</param>
        /// <returns>True when the item was accepted.</returns>
        public bool TryCollect(ItemData item, int amount)
        {
            if (item == null || amount <= 0 || string.IsNullOrWhiteSpace(item.Id))
            {
                return false;
            }

            if (_amountsByItemId.TryGetValue(item.Id, out int currentAmount))
            {
                _amountsByItemId[item.Id] = currentAmount + amount;
            }
            else
            {
                _amountsByItemId.Add(item.Id, amount);
            }

            ItemCollected?.Invoke(item, amount);

#if UNITY_EDITOR
            Debug.Log($"Player collected {amount} x {item.DisplayName}.", this);
#endif

            return true;
        }

        /// <summary>
        /// Gets the currently collected amount for an item.
        /// </summary>
        /// <param name="item">Item to query.</param>
        /// <returns>Collected amount, or zero when none has been collected.</returns>
        public int GetAmount(ItemData item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
            {
                return 0;
            }

            return _amountsByItemId.TryGetValue(item.Id, out int amount) ? amount : 0;
        }
    }
}
