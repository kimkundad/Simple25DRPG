using UnityEngine;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Keeps a UI health bar positioned above a world-space target.
    /// </summary>
    public sealed class HealthBarFollower : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Target transform this health bar follows.")]
        [SerializeField] private Transform _target;

        [Tooltip("Camera used to convert world positions to screen positions.")]
        [SerializeField] private UnityEngine.Camera _worldCamera;

        [Tooltip("Canvas RectTransform that contains this health bar.")]
        [SerializeField] private RectTransform _canvasRect;

        [Tooltip("View that provides display settings for this health bar.")]
        [SerializeField] private HealthBarView _view;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        private UnityEngine.Camera _eventCamera;
        private bool _isValid;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;

            if (_view == null)
            {
                _view = GetComponent<HealthBarView>();
            }

            if (_canvasRect != null)
            {
                _canvas = _canvasRect.GetComponent<Canvas>();
            }

            ValidateDependencies();
            CacheEventCamera();
        }

        private void LateUpdate()
        {
            if (!_isValid)
            {
                return;
            }

            Vector3 worldPosition = _target.position + Vector3.up * _view.FollowHeight;
            Vector3 screenPosition = _worldCamera.WorldToScreenPoint(worldPosition);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPosition,
                _eventCamera,
                out Vector2 localPoint))
            {
                _rectTransform.anchoredPosition = localPoint;
            }

            FaceCamera();
        }

        /// <summary>
        /// Assigns the references required for following a target.
        /// </summary>
        /// <param name="target">Target transform this health bar follows.</param>
        /// <param name="worldCamera">Camera used for world-to-screen conversion.</param>
        /// <param name="canvasRect">Canvas RectTransform that contains this health bar.</param>
        public void Initialize(Transform target, UnityEngine.Camera worldCamera, RectTransform canvasRect)
        {
            _target = target;
            _worldCamera = worldCamera;
            _canvasRect = canvasRect;

            if (_canvasRect != null)
            {
                _canvas = _canvasRect.GetComponent<Canvas>();
            }

            ValidateDependencies();
            CacheEventCamera();
        }

        private void FaceCamera()
        {
            if (_canvas == null || _canvas.renderMode != RenderMode.WorldSpace)
            {
                _rectTransform.rotation = Quaternion.identity;
                return;
            }

            Vector3 direction = _rectTransform.position - _worldCamera.transform.position;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            _rectTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private void CacheEventCamera()
        {
            if (_canvas == null || _canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                _eventCamera = null;
                return;
            }

            _eventCamera = _canvas.worldCamera != null ? _canvas.worldCamera : _worldCamera;
        }

        private void ValidateDependencies()
        {
            _isValid = false;

            if (_rectTransform == null)
            {
                Debug.LogWarning($"{nameof(HealthBarFollower)} on {name} requires a RectTransform.", this);
                return;
            }

            if (_target == null)
            {
                return;
            }

            if (_worldCamera == null)
            {
                Debug.LogWarning($"{nameof(HealthBarFollower)} on {name} requires a world Camera.", this);
                return;
            }

            if (_canvasRect == null)
            {
                Debug.LogWarning($"{nameof(HealthBarFollower)} on {name} requires a Canvas RectTransform.", this);
                return;
            }

            if (_canvas == null)
            {
                Debug.LogWarning($"{nameof(HealthBarFollower)} on {name} requires the assigned RectTransform to have a Canvas.", this);
                return;
            }

            if (_view == null)
            {
                Debug.LogWarning($"{nameof(HealthBarFollower)} on {name} requires a HealthBarView.", this);
                return;
            }

            _isValid = true;
        }
    }
}
