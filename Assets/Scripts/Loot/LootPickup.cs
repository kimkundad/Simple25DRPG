using System;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Loot
{
    /// <summary>
    /// Collects a world loot pickup when the player enters its trigger.
    /// </summary>
    public sealed class LootPickup : MonoBehaviour
    {
        [Header("Item")]
        [Tooltip("Item collected by this pickup.")]
        [SerializeField] private ItemData _item;

        [Tooltip("Amount collected by this pickup.")]
        [Min(1)]
        [SerializeField] private int _amount = 1;

        private bool _collected;
        private bool _collectionInProgress;

        /// <summary>
        /// Raised immediately before this pickup is destroyed.
        /// </summary>
        public event Action<ItemData, int> PickedUp;

        /// <summary>
        /// Assigns the runtime item and amount for this pickup.
        /// </summary>
        /// <param name="item">Item data to collect.</param>
        /// <param name="amount">Amount to collect.</param>
        public void Initialize(ItemData item, int amount)
        {
            _item = item;
            _amount = Mathf.Max(1, amount);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_collected || _item == null)
            {
                return;
            }

            if (!TryGetCollector(other, out IItemCollector collector))
            {
                return;
            }

            TryCollect(collector);
        }

        private void OnValidate()
        {
            _amount = Mathf.Max(1, _amount);
        }

        private void TryCollect(IItemCollector collector)
        {
            if (_collectionInProgress)
            {
                return;
            }

            _collectionInProgress = true;

            if (!collector.TryCollect(_item, _amount))
            {
#if UNITY_EDITOR
                Debug.Log($"Pickup rejected by collector: {_amount} x {_item.DisplayName}.", this);
#endif
                _collectionInProgress = false;
                return;
            }

            _collected = true;
            PickedUp?.Invoke(_item, _amount);
            Destroy(gameObject);
        }

        private static bool TryGetCollector(Collider other, out IItemCollector collector)
        {
            if (other.TryGetComponent(out collector))
            {
                return true;
            }

            collector = other.GetComponentInParent<IItemCollector>();
            return collector != null;
        }
    }
}
