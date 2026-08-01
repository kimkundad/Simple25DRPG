using System;
using Simple25DRPG.Combat;
using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Stores enemy health and exposes damage/death events.
    /// </summary>
    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Dependencies")]
        [Tooltip("Settings asset that defines maximum HP.")]
        [SerializeField] private EnemySettings _settings;

        private int _currentHp;
        private bool _isDead;

        /// <summary>
        /// Raised after damage is applied. Parameters are current HP and damage amount.
        /// </summary>
        public event Action<int, int> OnDamaged;

        /// <summary>
        /// Raised once when HP reaches zero.
        /// </summary>
        public event Action OnDied;

        /// <summary>
        /// Gets the current hit points.
        /// </summary>
        public int CurrentHp => _currentHp;

        /// <summary>
        /// Gets whether this enemy has died.
        /// </summary>
        public bool IsDead => _isDead;

        private void Awake()
        {
            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(EnemyHealth)} on {name} requires EnemySettings.", this);
                enabled = false;
                return;
            }

            _currentHp = _settings.MaxHp;
        }

        /// <summary>
        /// Applies damage to the enemy and raises damage/death events.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0)
            {
                return;
            }

            int appliedDamage = Mathf.Min(damage, _currentHp);
            _currentHp = Mathf.Clamp(_currentHp - damage, 0, _settings.MaxHp);
            OnDamaged?.Invoke(_currentHp, appliedDamage);

            if (_currentHp > 0)
            {
                return;
            }

            _isDead = true;
            OnDied?.Invoke();
        }
    }
}
