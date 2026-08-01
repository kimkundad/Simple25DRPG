using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Defines reusable health tuning values for the player.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerHealthSettings", menuName = "Simple 2.5D RPG/Player/Player Health Settings")]
    public sealed class PlayerHealthSettings : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("Maximum player hit points.")]
        [Min(1)]
        [SerializeField] private int _maxHp = 100;

        /// <summary>
        /// Gets the maximum player hit points.
        /// </summary>
        public int MaxHp => _maxHp;

        private void OnValidate()
        {
            _maxHp = Mathf.Max(1, _maxHp);
        }
    }
}
