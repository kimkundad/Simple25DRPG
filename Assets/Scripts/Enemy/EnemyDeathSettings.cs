using UnityEngine;

namespace Simple25DRPG.Enemy
{
    /// <summary>
    /// Defines reusable death-flow settings for enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyDeathSettings", menuName = "Simple 2.5D RPG/Enemy/Enemy Death Settings")]
    public sealed class EnemyDeathSettings : ScriptableObject
    {
        [Header("Timing")]
        [Tooltip("Delay in seconds before the enemy GameObject is destroyed.")]
        [Min(0f)]
        [SerializeField] private float _destroyDelay = 2f;

        [Header("Disable Behavior")]
        [Tooltip("Disable enemy colliders after death so combat can no longer hit them.")]
        [SerializeField] private bool _disableCollider = true;

        [Tooltip("Disable the enemy movement controller after death.")]
        [SerializeField] private bool _disableController = true;

        [Header("Animation")]
        [Tooltip("Trigger a Death animation if an Animator is assigned.")]
        [SerializeField] private bool _playAnimation = true;

        /// <summary>
        /// Gets the delay in seconds before the enemy GameObject is destroyed.
        /// </summary>
        public float DestroyDelay => _destroyDelay;

        /// <summary>
        /// Gets whether enemy colliders should be disabled after death.
        /// </summary>
        public bool DisableCollider => _disableCollider;

        /// <summary>
        /// Gets whether the enemy movement controller should be disabled after death.
        /// </summary>
        public bool DisableController => _disableController;

        /// <summary>
        /// Gets whether a Death animation trigger should be sent when possible.
        /// </summary>
        public bool PlayAnimation => _playAnimation;

        private void OnValidate()
        {
            _destroyDelay = Mathf.Max(0f, _destroyDelay);
        }
    }
}
