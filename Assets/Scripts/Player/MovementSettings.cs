using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Stores movement tuning values for a CharacterController-driven player.
    /// </summary>
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Simple25DRPG/Player/Movement Settings")]
    public sealed class MovementSettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Maximum movement speed in world units per second.")]
        [Min(0f)]
        [SerializeField] private float _moveSpeed = 4.5f;

        [Tooltip("How quickly the player rotates toward the movement direction.")]
        [Range(0f, 30f)]
        [SerializeField] private float _rotationSpeed = 14f;

        [Header("Gravity")]
        [Tooltip("Downward acceleration applied while the player is airborne.")]
        [SerializeField] private float _gravity = -25f;

        [Tooltip("Small downward velocity used while grounded to keep the CharacterController stable.")]
        [Range(-10f, 0f)]
        [SerializeField] private float _groundedVerticalVelocity = -2f;

        /// <summary>
        /// Gets the maximum movement speed in world units per second.
        /// </summary>
        public float MoveSpeed => _moveSpeed;

        /// <summary>
        /// Gets how quickly the player rotates toward the movement direction.
        /// </summary>
        public float RotationSpeed => _rotationSpeed;

        /// <summary>
        /// Gets the downward acceleration applied while airborne.
        /// </summary>
        public float Gravity => _gravity;

        /// <summary>
        /// Gets the grounded downward velocity used by the CharacterController.
        /// </summary>
        public float GroundedVerticalVelocity => _groundedVerticalVelocity;
    }
}
