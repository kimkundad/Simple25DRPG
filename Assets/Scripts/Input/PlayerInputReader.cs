using UnityEngine;
using UnityEngine.InputSystem;
using Simple25DRPG.UI;

namespace Simple25DRPG.Input
{
    /// <summary>
    /// Reads player movement input and exposes gameplay-friendly values.
    /// </summary>
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Input Actions")]
        [Tooltip("Input System action that provides player movement as a Vector2.")]
        [SerializeField] private InputActionReference _moveAction;

        [Header("Mobile Input")]
        [Tooltip("Virtual joystick used for Android builds. Editor and desktop builds continue to use the Input System action.")]
        [SerializeField] private VirtualJoystick _virtualJoystick;

        [Tooltip("Use the virtual joystick in the Unity Editor for testing. Android builds always use the joystick.")]
        [SerializeField] private bool _useVirtualJoystickInEditor;

        private bool _useVirtualJoystick;
        private bool _missingJoystickWarningLogged;

        /// <summary>
        /// Gets the latest movement input value from the active input source.
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            _useVirtualJoystick = true;
#elif UNITY_EDITOR
            _useVirtualJoystick = _useVirtualJoystickInEditor;
#else
            _useVirtualJoystick = false;
#endif
        }

        private void OnEnable()
        {
            if (_useVirtualJoystick)
            {
                ValidateVirtualJoystick();
                return;
            }

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
            if (_useVirtualJoystick)
            {
                MoveInput = Vector2.zero;
                return;
            }

            if (_moveAction == null || _moveAction.action == null)
            {
                return;
            }

            _moveAction.action.performed -= OnMovePerformed;
            _moveAction.action.canceled -= OnMoveCanceled;
            _moveAction.action.Disable();
            MoveInput = Vector2.zero;
        }

        private void Update()
        {
            if (!_useVirtualJoystick || _virtualJoystick == null)
            {
                return;
            }

            MoveInput = _virtualJoystick.Direction;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveInput = Vector2.zero;
        }

        private void ValidateVirtualJoystick()
        {
            if (_virtualJoystick != null || _missingJoystickWarningLogged)
            {
                return;
            }

            Debug.LogWarning($"{nameof(PlayerInputReader)} on {name} is configured for Android joystick input but no VirtualJoystick is assigned.", this);
            _missingJoystickWarningLogged = true;
        }
    }
}
