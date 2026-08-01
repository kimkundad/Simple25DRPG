using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Handles simple enemy idle and chase movement toward a target.
    /// </summary>
    public sealed class EnemyController : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("CharacterController used to move the enemy.")]
        [SerializeField] private CharacterController _characterController;

        [Tooltip("Settings asset that controls movement, detection, and stopping distance.")]
        [SerializeField] private EnemySettings _settings;

        [Tooltip("Health component that notifies this controller when the enemy dies.")]
        [SerializeField] private EnemyHealth _health;

        [Tooltip("Target transform detected and chased by this enemy.")]
        [SerializeField] private Transform _target;

        private bool _isValid;
        private float _movementPausedUntil;

        /// <summary>
        /// Gets whether the enemy is currently moving.
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Gets the current movement speed normalized for animation.
        /// </summary>
        public float NormalizedMoveSpeed { get; private set; }

        /// <summary>
        /// Stops enemy movement and prevents future chase updates.
        /// </summary>
        public void StopMovement()
        {
            _isValid = false;
            SetMovementState(false);
        }

        /// <summary>
        /// Pauses enemy chase movement for the requested duration.
        /// </summary>
        /// <param name="duration">Pause duration in seconds.</param>
        public void PauseMovement(float duration)
        {
            if (duration <= 0f)
            {
                return;
            }

            _movementPausedUntil = Mathf.Max(_movementPausedUntil, Time.time + duration);
            SetMovementState(false);

#if UNITY_EDITOR
            Debug.Log("Movement paused.", this);
#endif
        }

        private void Awake()
        {
            if (_characterController == null)
            {
                _characterController = GetComponent<CharacterController>();
            }

            if (_health == null)
            {
                _health = GetComponent<EnemyHealth>();
            }

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

        private void Update()
        {
            if (!_isValid)
            {
                return;
            }

            if (Time.time < _movementPausedUntil)
            {
                SetMovementState(false);
                return;
            }

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;

            float distance = toTarget.magnitude;
            if (distance > _settings.DetectionRange || distance <= _settings.StoppingDistance)
            {
                SetMovementState(false);
                return;
            }

            Vector3 direction = toTarget / distance;
            RotateToward(direction);
            _characterController.Move(direction * (_settings.MoveSpeed * Time.deltaTime));
            SetMovementState(true);
        }

        private void OnDrawGizmosSelected()
        {
            if (_settings == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _settings.DetectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _settings.StoppingDistance);
        }

        private void ValidateDependencies()
        {
            if (_characterController == null)
            {
                Debug.LogWarning($"{nameof(EnemyController)} on {name} requires a CharacterController.", this);
                return;
            }

            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(EnemyController)} on {name} requires EnemySettings.", this);
                return;
            }

            if (_health == null)
            {
                Debug.LogWarning($"{nameof(EnemyController)} on {name} requires EnemyHealth.", this);
                return;
            }

            if (_target == null)
            {
                Debug.LogWarning($"{nameof(EnemyController)} on {name} requires a target Transform.", this);
                return;
            }

            _isValid = true;
        }

        private void RotateToward(Vector3 direction)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _settings.RotationSpeed * Time.deltaTime);
        }

        private void HandleDied()
        {
            StopMovement();
            enabled = false;
        }

        private void SetMovementState(bool isMoving)
        {
            IsMoving = isMoving;
            NormalizedMoveSpeed = isMoving ? 1f : 0f;
        }
    }
}
