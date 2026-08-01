using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Defines reusable enemy melee attack tuning values.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyAttackSettings", menuName = "Simple 2.5D RPG/Enemy/Enemy Attack Settings")]
    public sealed class EnemyAttackSettings : ScriptableObject
    {
        [Header("Damage")]
        [Tooltip("Damage applied by one enemy attack.")]
        [Min(0)]
        [SerializeField] private int _damage = 10;

        [Header("Range")]
        [Tooltip("Maximum horizontal distance from the attack origin to the target.")]
        [Min(0f)]
        [SerializeField] private float _attackRange = 1.6f;

        [Header("Timing")]
        [Tooltip("Seconds between accepted enemy attacks.")]
        [Min(0f)]
        [SerializeField] private float _attackCooldown = 1.2f;

        [Tooltip("Seconds between accepting an attack and applying damage.")]
        [Min(0f)]
        [SerializeField] private float _attackWindup = 0.2f;

        /// <summary>
        /// Gets the damage applied by one enemy attack.
        /// </summary>
        public int Damage => _damage;

        /// <summary>
        /// Gets the maximum horizontal attack distance.
        /// </summary>
        public float AttackRange => _attackRange;

        /// <summary>
        /// Gets the cooldown between accepted attacks.
        /// </summary>
        public float AttackCooldown => _attackCooldown;

        /// <summary>
        /// Gets the delay before damage is applied.
        /// </summary>
        public float AttackWindup => _attackWindup;

        private void OnValidate()
        {
            _damage = Mathf.Max(0, _damage);
            _attackRange = Mathf.Max(0f, _attackRange);
            _attackCooldown = Mathf.Max(0f, _attackCooldown);
            _attackWindup = Mathf.Max(0f, _attackWindup);
        }
    }
}
