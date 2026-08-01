using Simple25DRPG.Combat;
using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Handles enemy attack timing and applies damage to a target when in range.
    /// </summary>
    public sealed class EnemyAttackController : MonoBehaviour
    {
        private enum AttackState
        {
            Ready,
            Windup,
            Cooldown
        }

        [Header("Dependencies")]
        [Tooltip("Enemy movement controller paused during attack windup.")]
        [SerializeField] private EnemyController _enemyController;

        [Tooltip("Enemy health used to stop attacks after death.")]
        [SerializeField] private EnemyHealth _enemyHealth;

        [Tooltip("Animation controller that receives attack animation requests.")]
        [SerializeField] private EnemyAnimationController _animationController;

        [Tooltip("Settings asset that controls enemy attack timing, range, and damage.")]
        [SerializeField] private EnemyAttackSettings _settings;

        [Tooltip("Target transform this enemy can attack.")]
        [SerializeField] private Transform _target;

        [Tooltip("Transform used as the origin for attack range checks.")]
        [SerializeField] private Transform _attackOrigin;

        private IDamageable _targetDamageable;
        private AttackState _state;
        private float _stateEndTime;
        private bool _isValid;

        private void Awake()
        {
            if (_enemyController == null)
            {
                _enemyController = GetComponent<EnemyController>();
            }

            if (_enemyHealth == null)
            {
                _enemyHealth = GetComponent<EnemyHealth>();
            }

            if (_animationController == null)
            {
                _animationController = GetComponent<EnemyAnimationController>();
            }

            if (_attackOrigin == null)
            {
                _attackOrigin = transform;
            }

            if (_target != null)
            {
                _target.TryGetComponent(out _targetDamageable);
            }

            ValidateDependencies();
        }

        private void OnEnable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.OnDied += HandleEnemyDied;
            }
        }

        private void OnDisable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.OnDied -= HandleEnemyDied;
            }
        }

        private void Update()
        {
            if (!_isValid || _enemyHealth.IsDead)
            {
                return;
            }

            if (_state == AttackState.Windup)
            {
                UpdateWindup();
                return;
            }

            if (_state == AttackState.Cooldown)
            {
                UpdateCooldown();
                return;
            }

            TryStartAttack();
        }

        private void OnDrawGizmosSelected()
        {
            if (_settings == null)
            {
                return;
            }

            Transform origin = _attackOrigin != null ? _attackOrigin : transform;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(origin.position, _settings.AttackRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(origin.position, 0.12f);
        }

        private void TryStartAttack()
        {
            if (!IsTargetInRange())
            {
                return;
            }

            _state = AttackState.Windup;
            _stateEndTime = Time.time + _settings.AttackWindup;
            _enemyController.PauseMovement(_settings.AttackWindup);
            _animationController.PlayAttack();

#if UNITY_EDITOR
            Debug.Log("Enemy attack accepted.", this);
            Debug.Log("Windup started.", this);
#endif
        }

        private void UpdateWindup()
        {
            if (Time.time < _stateEndTime)
            {
                return;
            }

            if (IsTargetInRange())
            {
                _targetDamageable.TakeDamage(_settings.Damage);

#if UNITY_EDITOR
                Debug.Log($"Damage applied: {_settings.Damage}.", this);
#endif
            }

            StartCooldown();
        }

        private void UpdateCooldown()
        {
            if (Time.time < _stateEndTime)
            {
                return;
            }

            _state = AttackState.Ready;
        }

        private void StartCooldown()
        {
            _state = AttackState.Cooldown;
            _stateEndTime = Time.time + _settings.AttackCooldown;
        }

        private bool IsTargetInRange()
        {
            if (_target == null || _targetDamageable == null)
            {
                return false;
            }

            Vector3 toTarget = _target.position - _attackOrigin.position;
            toTarget.y = 0f;
            return toTarget.sqrMagnitude <= _settings.AttackRange * _settings.AttackRange;
        }

        private void HandleEnemyDied()
        {
            _state = AttackState.Ready;
            enabled = false;
        }

        private void ValidateDependencies()
        {
            if (_enemyController == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires EnemyController.", this);
                return;
            }

            if (_enemyHealth == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires EnemyHealth.", this);
                return;
            }

            if (_animationController == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires EnemyAnimationController.", this);
                return;
            }

            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires EnemyAttackSettings.", this);
                return;
            }

            if (_target == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires a target Transform.", this);
                return;
            }

            if (_attackOrigin == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires an attack origin Transform.", this);
                return;
            }

            if (_targetDamageable == null)
            {
                Debug.LogWarning($"{nameof(EnemyAttackController)} on {name} requires a target with IDamageable.", this);
                return;
            }

            _isValid = true;
        }
    }
}
