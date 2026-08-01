using UnityEngine;

namespace Simple25DRPG.Inventory
{
    /// <summary>
    /// Defines player inventory capacity.
    /// </summary>
    [CreateAssetMenu(fileName = "InventorySettings", menuName = "Simple 2.5D RPG/Inventory/Inventory Settings")]
    public sealed class InventorySettings : ScriptableObject
    {
        [Header("Inventory")]
        [Tooltip("Number of inventory slots available to the player.")]
        [Min(1)]
        [SerializeField] private int _capacity = 20;

        /// <summary>
        /// Gets the number of inventory slots.
        /// </summary>
        public int Capacity => _capacity;

        private void OnValidate()
        {
            _capacity = Mathf.Max(1, _capacity);
        }
    }
}
