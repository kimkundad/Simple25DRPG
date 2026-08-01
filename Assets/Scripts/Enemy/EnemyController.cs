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

            Vector3 toTarget = _target.position - transform.position;
            toTarget.y = 0f;

            float distance = toTarget.magnitude;
            if (distance > _settings.DetectionRange || distance <= _settings.StoppingDistance)
            {
                return;
            }

            Vector3 direction = toTarget / distance;
            RotateToward(direction);
            _characterController.Move(direction * (_settings.MoveSpeed * Time.deltaTime));
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
            _isValid = false;
            enabled = false;
        }
    }
}
