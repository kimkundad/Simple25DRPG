using System;
using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Coordinates the enemy death flow after health reaches zero.
    /// </summary>
    public sealed class EnemyDeathController : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Health component that raises the death event.")]
        [SerializeField] private EnemyHealth _health;

        [Tooltip("Movement controller to stop when this enemy dies.")]
        [SerializeField] private EnemyController _enemyController;

        [Tooltip("Settings asset that controls death cleanup behavior.")]
        [SerializeField] private EnemyDeathSettings _settings;

        [Tooltip("CharacterController disabled after death.")]
        [SerializeField] private CharacterController _characterController;

        private Collider[] _combatColliders;
        private bool _deathStarted;
        private bool _deathCompleted;
        private bool _isValid;

        /// <summary>
        /// Raised after the configured death delay completes, before the GameObject is destroyed.
        /// </summary>
        public event Action DeathCompleted;

        private void Awake()
        {
            if (_health == null)
            {
                _health = GetComponent<EnemyHealth>();
            }

            if (_enemyController == null)
            {
                _enemyController = GetComponent<EnemyController>();
            }

            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
            }

            _combatColliders = GetComponentsInChildren<Collider>();
            ValidateDependencies();
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnDied += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (!_isValid || _deathStarted)
            {
                return;
            }

            _deathStarted = true;

#if UNITY_EDITOR
            Debug.Log("Enemy died.", this);
#endif

            StopEnemyBehavior();
            DisableHitDetection();
            ScheduleDestroy();
        }

        private void StopEnemyBehavior()
        {
            if (_enemyController == null)
            {
                return;
            }

            _enemyController.StopMovement();

            if (_settings.DisableController)
            {
                _enemyController.enabled = false;
            }
        }

        private void DisableHitDetection()
        {
            if (_characterController != null)
            {
                _characterController.enabled = false;
            }

            if (!_settings.DisableCollider || _combatColliders == null)
            {
                return;
            }

            for (int i = 0; i < _combatColliders.Length; i++)
            {
                if (_combatColliders[i] != null)
                {
                    _combatColliders[i].enabled = false;
                }
            }
        }

        private void ScheduleDestroy()
        {
            if (_settings.DestroyDelay <= 0f)
            {
                CompleteDeath();
                return;
            }

            Invoke(nameof(CompleteDeath), _settings.DestroyDelay);
        }

        private void CompleteDeath()
        {
            if (_deathCompleted)
            {
                return;
            }

            _deathCompleted = true;
            DeathCompleted?.Invoke();
            Destroy(gameObject);
        }

        private void ValidateDependencies()
        {
            if (_health == null)
            {
                Debug.LogWarning($"{nameof(EnemyDeathController)} on {name} requires EnemyHealth.", this);
                return;
            }

            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(EnemyDeathController)} on {name} requires EnemyDeathSettings.", this);
                return;
            }

            _isValid = true;
        }
    }
}
