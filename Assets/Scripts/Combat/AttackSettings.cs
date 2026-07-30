using UnityEngine;

namespace Simple25DRPG.Combat
{
    /// <summary>
    /// Defines reusable melee attack tuning values for a combat actor.
    /// </summary>
    [CreateAssetMenu(fileName = "AttackSettings", menuName = "Simple 2.5D RPG/Combat/Attack Settings")]
    public sealed class AttackSettings : ScriptableObject
    {
        [Header("Timing")]
        [Tooltip("Minimum time in seconds between accepted attacks.")]
        [Min(0f)]
        [SerializeField] private float _attackCooldown = 0.6f;

        [Header("Hit Detection")]
        [Tooltip("Prototype melee reach value reserved for positioning attack origins and future targeting.")]
        [Min(0f)]
        [SerializeField] private float _attackRange = 1.2f;

        [Tooltip("Radius of the overlap sphere used for melee hit detection.")]
        [Min(0.01f)]
        [SerializeField] private float _attackRadius = 0.6f;

        [Tooltip("Damage applied to each valid damageable target hit by the attack.")]
        [Min(0)]
        [SerializeField] private int _damage = 10;

        [Tooltip("Layers that can be hit by this attack.")]
        [SerializeField] private LayerMask _hittableLayers = ~0;

        /// <summary>
        /// Gets the minimum time in seconds between accepted attacks.
        /// </summary>
        public float AttackCooldown => _attackCooldown;

        /// <summary>
        /// Gets the prototype melee reach value.
        /// </summary>
        public float AttackRange => _attackRange;

        /// <summary>
        /// Gets the radius of the overlap sphere used for melee hit detection.
        /// </summary>
        public float AttackRadius => _attackRadius;

        /// <summary>
        /// Gets the damage applied to each valid target.
        /// </summary>
        public int Damage => _damage;

        /// <summary>
        /// Gets the layers that can be hit by this attack.
        /// </summary>
        public LayerMask HittableLayers => _hittableLayers;

        private void OnValidate()
        {
            _attackCooldown = Mathf.Max(0f, _attackCooldown);
            _attackRange = Mathf.Max(0f, _attackRange);
            _attackRadius = Mathf.Max(0.01f, _attackRadius);
            _damage = Mathf.Max(0, _damage);
        }
    }
}
