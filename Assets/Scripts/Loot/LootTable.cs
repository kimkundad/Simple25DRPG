using System.Collections.Generic;
using UnityEngine;

namespace Simple25DRPG.Loot
{
    /// <summary>
    /// Stores independently rolled loot entries for enemy drops.
    /// </summary>
    [CreateAssetMenu(fileName = "LootTable", menuName = "Simple 2.5D RPG/Loot/Loot Table")]
    public sealed class LootTable : ScriptableObject
    {
        [Header("Loot Entries")]
        [Tooltip("Entries that each roll independently when this table drops loot.")]
        [SerializeField] private List<LootEntry> _entries = new List<LootEntry>();

        /// <summary>
        /// Gets the configured loot entries.
        /// </summary>
        public IReadOnlyList<LootEntry> Entries => _entries;

        /// <summary>
        /// Rolls one entry and returns a valid amount when it drops.
        /// </summary>
        /// <param name="entry">Entry to roll.</param>
        /// <param name="amount">Rolled amount.</param>
        /// <returns>True when this entry drops.</returns>
        public bool TryRollEntry(LootEntry entry, out int amount)
        {
            amount = 0;

            if (entry == null || !entry.Roll())
            {
                return false;
            }

            amount = Random.Range(entry.MinAmount, entry.MaxAmount + 1);
            return true;
        }

        private void OnValidate()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                if (_entries[i] != null)
                {
                    _entries[i].ClampValues();
                }
            }
        }
    }
}
