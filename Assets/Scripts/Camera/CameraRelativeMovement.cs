using UnityEngine;

namespace Simple25DRPG.Camera
{
    /// <summary>
    /// Converts movement input into a world-space direction relative to a camera.
    /// </summary>
    public sealed class CameraRelativeMovement : MonoBehaviour
    {
        [Header("Camera")]
        [Tooltip("Camera used as the movement reference. If empty, the active main camera is cached in Awake.")]
        [SerializeField] private Transform _cameraTransform;

        private void Awake()
        {
            if (_cameraTransform == null && UnityEngine.Camera.main != null)
            {
                _cameraTransform = UnityEngine.Camera.main.transform;
            }

            if (_cameraTransform == null)
            {
                Debug.LogWarning($"{nameof(CameraRelativeMovement)} on {name} could not find a camera transform.", this);
            }
        }

        /// <summary>
        /// Converts 2D movement input into a normalized X/Z world-space direction.
        /// </summary>
        /// <param name="input">Movement input from an Input System action.</param>
        /// <returns>A normalized world-space movement direction, or zero when input is zero.</returns>
        public Vector3 ToWorldDirection(Vector2 input)
        {
            if (input.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            if (_cameraTransform == null)
            {
                return new Vector3(input.x, 0f, input.y).normalized;
            }

            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return ((right * input.x) + (forward * input.y)).normalized;
        }
    }
}
