using Simple25DRPG.Animation;
using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Updates player Animator parameters from player movement state.
    /// </summary>
    public sealed class PlayerAnimationController : MonoBehaviour, IAnimationController
    {
        private static readonly int MoveSpeedParameter = Animator.StringToHash("MoveSpeed");
        private static readonly int IsMovingParameter = Animator.StringToHash("IsMoving");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");

        [Header("Dependencies")]
        [Tooltip("Animator that receives player animation parameters.")]
        [SerializeField] private Animator _animator;

        [Tooltip("Movement controller used as the source of movement state.")]
        [SerializeField] private PlayerMovementController _movementController;

        private void Awake()
        {
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            if (_movementController == null)
            {
                _movementController = GetComponent<PlayerMovementController>();
            }

            ValidateDependencies();
        }

        private void Update()
        {
            bool isMoving = _movementController.HasMovementInput;
            SetMoveSpeed(isMoving ? 1f : 0f);
            _animator.SetBool(IsMovingParameter, isMoving);
        }

        /// <summary>
        /// Plays the attack animation.
        /// </summary>
        public void PlayAttack()
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetTrigger(AttackTrigger);
        }

        /// <summary>
        /// Plays the hit reaction animation.
        /// </summary>
        public void PlayHit()
        {
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

        private void ValidateDependencies()
        {
            if (_animator == null)
            {
                Debug.LogWarning($"{nameof(PlayerAnimationController)} on {name} requires an Animator.", this);
                enabled = false;
                return;
            }

            if (_movementController == null)
            {
                Debug.LogWarning($"{nameof(PlayerAnimationController)} on {name} requires a PlayerMovementController.", this);
                enabled = false;
            }
        }
    }
}
