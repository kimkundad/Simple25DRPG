using UnityEngine;

namespace Simple25DRPG.Camera
{
    /// <summary>
    /// Converts two-dimensional movement input into a world-space direction relative to a camera.
    /// </summary>
    public static class CameraRelativeMovementUtility
    {
        /// <summary>
        /// Converts a 2D input vector into an X/Z world-space movement direction that preserves input magnitude.
        /// </summary>
        /// <param name="input">The movement input, usually from a stick, virtual joystick, or keyboard composite.</param>
        /// <param name="cameraTransform">The camera transform used as the movement reference.</param>
        /// <returns>A world-space direction, or zero when there is no meaningful input.</returns>
        public static Vector3 ToWorldDirection(Vector2 input, Transform cameraTransform)
        {
            if (input.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            if (cameraTransform == null)
            {
                return new Vector3(input.x, 0f, input.y).normalized;
            }

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 worldDirection = (right * input.x) + (forward * input.y);
            return worldDirection.sqrMagnitude > 1f ? worldDirection.normalized : worldDirection;
        }
    }
}
