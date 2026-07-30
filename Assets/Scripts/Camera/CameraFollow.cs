using UnityEngine;

namespace Simple25DRPG.Camera
{
    /// <summary>
    /// Follows a target transform using a configurable offset.
    /// </summary>
    public sealed class CameraFollow : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Transform the camera should follow.")]
        [SerializeField] private Transform _target;

        [Tooltip("Camera follow tuning data.")]
        [SerializeField] private CameraFollowSettings _settings;

        private Transform _cachedTransform;
        private Vector3 _smoothVelocity;
        private bool _canFollow;

        private void Awake()
        {
            _cachedTransform = transform;
            ValidateDependencies();
        }

        private void LateUpdate()
        {
            if (!_canFollow)
            {
                return;
            }

            Vector3 targetPosition = _target.position + _settings.Offset;

            if (_settings.SmoothTime <= Mathf.Epsilon)
            {
                _cachedTransform.position = targetPosition;
                return;
            }

            _cachedTransform.position = Vector3.SmoothDamp(
                _cachedTransform.position,
                targetPosition,
                ref _smoothVelocity,
                _settings.SmoothTime,
                _settings.FollowSpeed,
                Time.deltaTime);
        }

        private void ValidateDependencies()
        {
            _canFollow = true;

            if (_target == null)
            {
                Debug.LogWarning($"{nameof(CameraFollow)} on {name} requires a target Transform.", this);
                _canFollow = false;
            }

            if (_settings == null)
            {
                Debug.LogWarning($"{nameof(CameraFollow)} on {name} requires CameraFollowSettings.", this);
                _canFollow = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_target == null || _settings == null)
            {
                return;
            }

            Vector3 targetPosition = _target.position;
            Vector3 desiredCameraPosition = targetPosition + _settings.Offset;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.25f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(desiredCameraPosition, 0.25f);
            Gizmos.DrawLine(targetPosition, desiredCameraPosition);
        }
    }
}
