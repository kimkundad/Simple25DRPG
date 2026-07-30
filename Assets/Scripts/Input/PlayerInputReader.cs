using UnityEngine;
using UnityEngine.InputSystem;

namespace Simple25DRPG.Input
{
    /// <summary>
    /// Reads player input from Unity's Input System and exposes gameplay-friendly values.
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Input Actions")]
        [Tooltip("Input System action that provides player movement as a Vector2.")]
        [SerializeField] private InputActionReference _moveAction;

        /// <summary>
        /// Gets the latest movement input value from the configured move action.
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        private void OnEnable()
        {
            if (_moveAction == null || _moveAction.action == null)
            {
                Debug.LogWarning($"{nameof(PlayerInputReader)} on {name} is missing a Move InputActionReference.", this);
                return;
            }

            _moveAction.action.performed += OnMovePerformed;
            _moveAction.action.canceled += OnMoveCanceled;
            _moveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (_moveAction == null || _moveAction.action == null)
            {
                return;
            }

            _moveAction.action.performed -= OnMovePerformed;
            _moveAction.action.canceled -= OnMoveCanceled;
            _moveAction.action.Disable();
            MoveInput = Vector2.zero;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveInput = Vector2.zero;
        }
    }
}
