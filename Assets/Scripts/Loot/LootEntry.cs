using System;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Loot
{
    /// <summary>
    /// Defines one weighted loot table entry.
    /// </summary>
    [Serializable]
    public sealed class LootEntry
    {
        [Tooltip("Item that can be selected by this loot entry.")]
        [SerializeField] private ItemData _item;

        [Tooltip("Minimum amount spawned when this entry is selected.")]
        [Min(1)]
        [SerializeField] private int _minAmount = 1;

        [Tooltip("Maximum amount spawned when this entry is selected.")]
        [Min(1)]
        [SerializeField] private int _maxAmount = 1;

        [Tooltip("Independent drop chance from 0 to 1.")]
        [Min(0f)]
        [SerializeField] private float _dropChance = 1f;

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public ItemData Item => _item;

        /// <summary>
        /// Gets the minimum drop amount.
        /// </summary>
        public int MinAmount => _minAmount;

        /// <summary>
        /// Gets the maximum drop amount.
        /// </summary>
        public int MaxAmount => _maxAmount;

        /// <summary>
        /// Gets the independent drop chance from 0 to 1.
        /// </summary>
        public float DropChance => _dropChance;

        /// <summary>
        /// Gets whether this entry can currently be selected.
        /// </summary>
        public bool IsValid => _item != null && _dropChance > 0f && _maxAmount > 0;

        /// <summary>
        /// Rolls this entry's independent drop chance.
        /// </summary>
        /// <returns>True when this entry should drop.</returns>
        public bool Roll()
        {
            return IsValid && Random.value <= _dropChance;
        }

        /// <summary>
        /// Clamps invalid serialized values.
        /// </summary>
        public void ClampValues()
        {
            _minAmount = Mathf.Max(1, _minAmount);
            _maxAmount = Mathf.Max(_minAmount, _maxAmount);
            _dropChance = Mathf.Clamp01(_dropChance);
        }
    }
}
