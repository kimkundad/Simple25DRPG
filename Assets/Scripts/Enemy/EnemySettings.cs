using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Defines reusable tuning values for a simple enemy.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Simple 2.5D RPG/Enemy/Enemy Settings")]
    public sealed class EnemySettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Enemy movement speed in units per second.")]
        [Min(0f)]
        [SerializeField] private float _moveSpeed = 3.5f;

        [Tooltip("Enemy rotation speed while turning toward its target.")]
        [Min(0f)]
        [SerializeField] private float _rotationSpeed = 8f;

        [Header("Detection")]
        [Tooltip("Maximum distance at which the enemy starts chasing its target.")]
        [Min(0f)]
        [SerializeField] private float _detectionRange = 8f;

        [Tooltip("Distance from the target where the enemy stops chasing.")]
        [Min(0f)]
        [SerializeField] private float _stoppingDistance = 1.5f;

        [Header("Health")]
        [Tooltip("Maximum hit points for this enemy.")]
        [Min(1)]
        [SerializeField] private int _maxHp = 30;

        /// <summary>
        /// Gets the enemy movement speed in units per second.
        /// </summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>
        /// Gets the enemy rotation speed.
        /// </summary>
        public float RotationSpeed => _rotationSpeed;

        /// <summary>
        /// Gets the maximum target detection distance.
        /// </summary>
        public float DetectionRange => _detectionRange;

        /// <summary>
        /// Gets the distance from the target where movement stops.
        /// </summary>
        public float StoppingDistance => _stoppingDistance;

        /// <summary>
        /// Gets the maximum hit points.
        /// </summary>
        public int MaxHp => _maxHp;

        private void OnValidate()
        {
            _moveSpeed = Mathf.Max(0f, _moveSpeed);
            _rotationSpeed = Mathf.Max(0f, _rotationSpeed);
            _detectionRange = Mathf.Max(0f, _detectionRange);
            _stoppingDistance = Mathf.Max(0f, _stoppingDistance);
            _maxHp = Mathf.Max(1, _maxHp);
        }
    }
}
