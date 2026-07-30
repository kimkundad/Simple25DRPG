using Simple25DRPG.Camera;
using Simple25DRPG.Input;
using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Moves a player character with a CharacterController using camera-relative movement input.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovementController : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Input reader that supplies movement input from Unity's Input System.")]
        [SerializeField] private PlayerInputReader _inputReader;

        [Tooltip("Movement tuning values for speed, rotation, and gravity.")]
        [SerializeField] private MovementSettings _movementSettings;

        [Tooltip("Converts movement input into camera-relative world-space movement.")]
        [SerializeField] private CameraRelativeMovement _cameraRelativeMovement;

        private CharacterController _characterController;
        private float _verticalVelocity;
        private bool _hasRequiredDependencies;

        /// <summary>
        /// Gets whether the controller currently has movement input.
        /// </summary>
        public bool HasMovementInput => _inputReader != null && _inputReader.MoveInput.sqrMagnitude > Mathf.Epsilon;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            if (_inputReader == null)
            {
                _inputReader = GetComponent<PlayerInputReader>();
            }

            ValidateDependencies();
        }

        private void Update()
        {
            if (!_hasRequiredDependencies)
            {
                return;
            }

            Vector3 moveDirection = _cameraRelativeMovement.ToWorldDirection(_inputReader.MoveInput);
            ApplyGravity();
            Move(moveDirection);
            RotateTowards(moveDirection);
        }

        private void ValidateDependencies()
        {
            _hasRequiredDependencies = true;

            if (_characterController == null)
            {
                Debug.LogWarning($"{nameof(PlayerMovementController)} on {name} requires a CharacterController.", this);
                _hasRequiredDependencies = false;
            }

            if (_inputReader == null)
            {
                Debug.LogWarning($"{nameof(PlayerMovementController)} on {name} requires a PlayerInputReader.", this);
                _hasRequiredDependencies = false;
            }

            if (_movementSettings == null)
            {
                Debug.LogWarning($"{nameof(PlayerMovementController)} on {name} requires MovementSettings.", this);
                _hasRequiredDependencies = false;
            }

            if (_cameraRelativeMovement == null)
            {
                Debug.LogWarning($"{nameof(PlayerMovementController)} on {name} requires CameraRelativeMovement.", this);
                _hasRequiredDependencies = false;
            }
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = _movementSettings.GroundedVerticalVelocity;
                return;
            }

            _verticalVelocity += _movementSettings.Gravity * Time.deltaTime;
        }

        private void Move(Vector3 moveDirection)
        {
            Vector3 horizontalVelocity = moveDirection * _movementSettings.MoveSpeed;
            Vector3 velocity = horizontalVelocity + (Vector3.up * _verticalVelocity);
            _characterController.Move(velocity * Time.deltaTime);
        }

        private void RotateTowards(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude <= Mathf.Epsilon || _movementSettings.RotationSpeed <= 0f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                _movementSettings.RotationSpeed * Time.deltaTime);
        }
    }
}
