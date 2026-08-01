using Simple25DRPG.Enemy;
using UnityEngine;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Spawns floating damage numbers on the gameplay Canvas when an enemy takes damage.
    /// </summary>
    public sealed class DamageNumberSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Camera used to convert world positions into screen positions.")]
        [SerializeField] private UnityEngine.Camera _worldCamera;

        [Tooltip("RectTransform of the gameplay Canvas that receives floating damage numbers.")]
        [SerializeField] private RectTransform _canvasRect;

        [Tooltip("Floating damage number prefab to spawn.")]
        [SerializeField] private FloatingDamageNumber _prefab;

        [Tooltip("Enemy health that raises damage events.")]
        [SerializeField] private EnemyHealth _enemyHealth;

        [Header("Position")]
        [Tooltip("World-space offset applied above the damaged target.")]
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 2f, 0f);

        private Canvas _canvas;
        private UnityEngine.Camera _eventCamera;
        private bool _isValid;

        private void Awake()
        {
            if (_enemyHealth == null)
            {
                _enemyHealth = GetComponent<EnemyHealth>();
            }

            if (_canvasRect != null)
            {
                _canvas = _canvasRect.GetComponent<Canvas>();
            }

            ValidateDependencies();
            CacheEventCamera();
        }

        private void OnEnable()
        {
            if (!_isValid)
            {
                return;
            }

            _enemyHealth.DamageTaken += Spawn;
        }

        private void OnDisable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.DamageTaken -= Spawn;
            }
        }

        private void Spawn(int damageAmount, Vector3 worldPosition)
        {
            Vector3 screenPosition = _worldCamera.WorldToScreenPoint(worldPosition + _worldOffset);

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPosition,
                _eventCamera,
                out Vector2 localPoint))
            {
                return;
            }

            FloatingDamageNumber number = Instantiate(_prefab, _canvasRect, false);
            RectTransform numberTransform = number.transform as RectTransform;
            if (numberTransform != null)
            {
                numberTransform.anchoredPosition = localPoint;
            }

            number.Show(damageAmount);
        }

        private void ValidateDependencies()
        {
            if (_worldCamera == null)
            {
                Debug.LogWarning($"{nameof(DamageNumberSpawner)} on {name} requires a world Camera.", this);
                return;
            }

            if (_canvasRect == null)
            {
                Debug.LogWarning($"{nameof(DamageNumberSpawner)} on {name} requires a Canvas RectTransform.", this);
                return;
            }

            if (_canvas == null)
            {
                Debug.LogWarning($"{nameof(DamageNumberSpawner)} on {name} requires the assigned RectTransform to have a Canvas.", this);
                return;
            }

            if (_prefab == null)
            {
                Debug.LogWarning($"{nameof(DamageNumberSpawner)} on {name} requires a FloatingDamageNumber prefab.", this);
                return;
            }

            if (_enemyHealth == null)
            {
                Debug.LogWarning($"{nameof(DamageNumberSpawner)} on {name} requires EnemyHealth.", this);
                return;
            }

            _isValid = true;
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
    }
}
