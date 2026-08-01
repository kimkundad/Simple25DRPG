using System;
using Simple25DRPG.Combat;
using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Stores player health and exposes damage and death events.
    /// </summary>
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [Header("Dependencies")]
        [Tooltip("Settings asset that defines player maximum HP.")]
        [SerializeField] private PlayerHealthSettings _settings;

        private int _currentHp;
        private bool _isDead;

        /// <summary>
        /// Raised after damage is applied. Parameters are current HP and applied damage.
        /// </summary>
        public event Action<int, int> OnDamaged;

        /// <summary>
        /// Raised after damage is applied. Parameters are damage amount and world position.
        /// </summary>
        public event Action<int, Vector3> DamageTaken;

        /// <summary>
        /// Raised once when HP reaches zero.
        /// </summary>
        public event Action OnDied;

        /// <summary>
        /// Gets the current hit points.
        /// </summary>
        public int CurrentHp => _currentHp;

        /// <summary>
        /// Gets the maximum hit points.
        /// </summary>
        public int MaxHp => _settings != null ? _settings.MaxHp : 0;

        /// <summary>
        /// Gets whether the player has died.
        /// </summary>
        public bool IsDead => _isDead;

        /// <summary>
        /// Gets the current health as a normalized value from 0 to 1.
        /// </summary>
        public float HealthNormalized => MaxHp > 0 ? (float)_currentHp / MaxHp : 0f;

        private void Awake()
        {
            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(PlayerHealth)} on {name} requires PlayerHealthSettings.", this);
                enabled = false;
                return;
            }

            _currentHp = _settings.MaxHp;
        }

        /// <summary>
        /// Applies damage to the player and raises damage or death events.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0 || _settings == null)
            {
                return;
            }

            int appliedDamage = Mathf.Min(damage, _currentHp);
            _currentHp = Mathf.Clamp(_currentHp - damage, 0, _settings.MaxHp);
            bool diedFromDamage = _currentHp <= 0;

            if (diedFromDamage)
            {
                _isDead = true;
            }

            OnDamaged?.Invoke(_currentHp, appliedDamage);
            DamageTaken?.Invoke(appliedDamage, transform.position);

#if UNITY_EDITOR
            Debug.Log($"Player HP remaining: {_currentHp}.", this);
#endif

            if (!diedFromDamage)
            {
                return;
            }

#if UNITY_EDITOR
            Debug.Log("Player died.", this);
#endif

            OnDied?.Invoke();
        }
    }
}
