using Simple25DRPG.Animation;
using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Updates enemy Animator triggers from enemy health events.
    /// </summary>
    public sealed class EnemyAnimationController : MonoBehaviour, IAnimationController
    {
        private static readonly int MoveSpeedParameter = Animator.StringToHash("MoveSpeed");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");

        [Header("Dependencies")]
        [Tooltip("Health component used as the source of damage and death events.")]
        [SerializeField] private EnemyHealth _health;

        [Tooltip("Enemy controller used as the source of movement state and hit reaction pauses.")]
        [SerializeField] private EnemyController _enemyController;

        [Tooltip("Optional Animator that receives Hit and Death triggers.")]
        [SerializeField] private Animator _animator;

        [Header("Hit Reaction")]
        [Tooltip("Seconds to pause movement after receiving non-lethal damage.")]
        [Min(0f)]
        [SerializeField] private float _hitReactionDuration = 0.15f;

        private bool _deathPlayed;

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

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (_health == null)
            {
                Debug.LogWarning($"{nameof(EnemyAnimationController)} on {name} requires EnemyHealth.", this);
                enabled = false;
                return;
            }

            if (_enemyController == null)
            {
                Debug.LogWarning($"{nameof(EnemyAnimationController)} on {name} has no EnemyController assigned. Movement speed and hit pause will be skipped.", this);
            }
        }

        private void Update()
        {
            if (_enemyController == null)
            {
                return;
            }

            SetMoveSpeed(_enemyController.NormalizedMoveSpeed);
        }

        private void OnValidate()
        {
            _hitReactionDuration = Mathf.Max(0f, _hitReactionDuration);
        }

        private void OnEnable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnDamaged += HandleDamaged;
            _health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnDamaged -= HandleDamaged;
            _health.OnDied -= HandleDied;
        }

        private void HandleDamaged(int currentHp, int damage)
        {
            if (currentHp <= 0)
            {
                return;
            }

#if UNITY_EDITOR
            Debug.Log($"Hit received. HP remaining: {currentHp}.", this);
#endif

            if (_enemyController != null)
            {
                _enemyController.PauseMovement(_hitReactionDuration);
            }

            PlayHit();
        }

        private void HandleDied()
        {
            PlayDeath();
        }

        /// <summary>
        /// Plays the hit reaction animation.
        /// </summary>
        public void PlayHit()
        {
#if UNITY_EDITOR
            Debug.Log("Hit animation.", this);
#endif

            if (_animator == null)
            {
                return;
            }

            _animator.SetTrigger(HitTrigger);
        }

        /// <summary>
        /// Plays the death animation.
        /// </summary>
        public void PlayDeath()
        {
            if (_deathPlayed)
            {
                return;
            }

            _deathPlayed = true;

#if UNITY_EDITOR
            Debug.Log("Death animation.", this);
#endif

            if (_animator == null)
            {
                return;
            }

            _animator.SetTrigger(DeathTrigger);
        }

        /// <summary>
        /// Updates the movement speed animation parameter.
        /// </summary>
        /// <param name="speed">Current movement speed value.</param>
        public void SetMoveSpeed(float speed)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetFloat(MoveSpeedParameter, speed);
        }

        /// <summary>
        /// Plays the attack animation.
        /// </summary>
        public void PlayAttack()
        {
        }
    }
}
