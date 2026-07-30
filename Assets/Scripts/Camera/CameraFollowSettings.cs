using UnityEngine;

namespace Simple25DRPG.Camera
{
    /// <summary>
    /// Stores camera follow tuning values.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraFollowSettings", menuName = "Simple25DRPG/Camera/Follow Settings")]
    public sealed class CameraFollowSettings : ScriptableObject
    {
        [Header("Follow")]
        [Tooltip("World-space offset from the target position.")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 10f, -8f);

        [Tooltip("Maximum speed used by SmoothDamp while following.")]
        [Min(0f)]
        [SerializeField] private float _followSpeed = 50f;

        [Tooltip("Approximate time for the camera to reach the target position. Set to 0 for direct follow.")]
        [Min(0f)]
        [SerializeField] private float _smoothTime = 0.15f;

        /// <summary>
        /// Gets the world-space offset from the target position.
        /// </summary>
        public Vector3 Offset => _offset;

        /// <summary>
        /// Gets the maximum speed used by SmoothDamp while following.
        /// </summary>
        public float FollowSpeed => _followSpeed;

        /// <summary>
        /// Gets the approximate time for the camera to reach the target position.
        /// </summary>
        public float SmoothTime => _smoothTime;
    }
}
