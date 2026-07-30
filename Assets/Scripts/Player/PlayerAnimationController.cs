using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Updates player Animator parameters from player movement state.
    /// </summary>
    public sealed class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int MoveSpeedParameter = Animator.StringToHash("MoveSpeed");
        private static readonly int IsMovingParameter = Animator.StringToHash("IsMoving");

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
            _animator.SetFloat(MoveSpeedParameter, isMoving ? 1f : 0f);
            _animator.SetBool(IsMovingParameter, isMoving);
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
