using UnityEngine;

namespace Simple25DRPG.Player
{
    /// <summary>
    /// Stores player movement tuning values that can be shared and adjusted from the Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementSettings", menuName = "Simple25DRPG/Player/Movement Settings")]
    public sealed class PlayerMovementSettings : ScriptableObject
    {
        [Header("Movement")]
        [Tooltip("Maximum horizontal movement speed in world units per second.")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 4.5f;

        [Tooltip("How quickly the player turns to face the movement direction.")]
        [Min(0f)]
        [SerializeField] private float rotationSpeed = 14f;

        [Header("Gravity")]
        [Tooltip("Gravity applied while the player is airborne.")]
        [SerializeField] private float gravity = -25f;

        [Tooltip("Small downward velocity applied while grounded to keep the CharacterController stable on slopes.")]
        [SerializeField] private float groundedVerticalVelocity = -2f;

        /// <summary>
        /// Gets the maximum horizontal movement speed in world units per second.
        /// </summary>
        public float MoveSpeed => moveSpeed;

        /// <summary>
        /// Gets how quickly the player turns to face the movement direction.
        /// </summary>
        public float RotationSpeed => rotationSpeed;

        /// <summary>
        /// Gets the gravity value applied while airborne.
        /// </summary>
        public float Gravity => gravity;

        /// <summary>
        /// Gets the downward velocity used while grounded.
        /// </summary>
        public float GroundedVerticalVelocity => groundedVerticalVelocity;
    }
}
