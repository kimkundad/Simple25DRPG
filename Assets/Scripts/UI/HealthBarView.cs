using UnityEngine;
using UnityEngine.UI;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Displays and fades a health bar without owning gameplay health state.
    /// </summary>
    public sealed class HealthBarView : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Image whose fill amount represents current HP.")]
        [SerializeField] private Image _fillImage;

        [Tooltip("CanvasGroup used to show and hide the health bar.")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Display")]
        [Tooltip("World-space height above the owner where the health bar appears.")]
        [Min(0f)]
        [SerializeField] private float _followHeight = 2f;

        [Tooltip("Seconds used to fade the health bar in or out.")]
        [Min(0.01f)]
        [SerializeField] private float _fadeDuration = 0.25f;

        [Tooltip("Speed used to smooth the fill amount.")]
        [Min(0f)]
        [SerializeField] private float _smoothSpeed = 8f;

        [Header("Timing")]
        [Tooltip("Seconds to remain visible after damage is received.")]
        [Min(0f)]
        [SerializeField] private float _visibleAfterDamageDuration = 2f;

        private float _targetFillAmount = 1f;
        private float _targetAlpha;
        private float _hideTime;
        private bool _isFull = true;
        private bool _isValid;

        /// <summary>
        /// Gets the world-space height above the owner where this health bar should appear.
        /// </summary>
        public float FollowHeight => _followHeight;

        private void Awake()
        {
            if (_fillImage == null)
            {
                _fillImage = GetComponentInChildren<Image>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            ValidateDependencies();

            if (!_isValid)
            {
                enabled = false;
                return;
            }

            _fillImage.fillAmount = 1f;
            _canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (!_isValid)
            {
                return;
            }

            _fillImage.fillAmount = Mathf.MoveTowards(
                _fillImage.fillAmount,
                _targetFillAmount,
                _smoothSpeed * Time.deltaTime);

            if (!_isFull && Time.time >= _hideTime)
            {
                _targetAlpha = 0f;
            }

            float fadeStep = Time.deltaTime / _fadeDuration;
            _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _targetAlpha, fadeStep);
        }

        /// <summary>
        /// Updates the displayed health value.
        /// </summary>
        /// <param name="currentHp">Current hit points.</param>
        /// <param name="maxHp">Maximum hit points.</param>
        public void SetHealth(int currentHp, int maxHp)
        {
            if (!_isValid || maxHp <= 0)
            {
                return;
            }

            _targetFillAmount = Mathf.Clamp01((float)currentHp / maxHp);
            _isFull = currentHp >= maxHp;

            if (_isFull)
            {
                _targetAlpha = 0f;
                return;
            }

            ShowTemporarily();
        }

        /// <summary>
        /// Shows the health bar for the configured damage visibility duration.
        /// </summary>
        public void ShowTemporarily()
        {
            if (!_isValid)
            {
                return;
            }

            _targetAlpha = 1f;
            _hideTime = Time.time + _visibleAfterDamageDuration;
        }

        private void OnValidate()
        {
            _followHeight = Mathf.Max(0f, _followHeight);
            _fadeDuration = Mathf.Max(0.01f, _fadeDuration);
            _smoothSpeed = Mathf.Max(0f, _smoothSpeed);
            _visibleAfterDamageDuration = Mathf.Max(0f, _visibleAfterDamageDuration);
        }

        private void ValidateDependencies()
        {
            if (_fillImage == null)
            {
                Debug.LogWarning($"{nameof(HealthBarView)} on {name} requires a fill Image.", this);
                return;
            }

            if (_canvasGroup == null)
            {
                Debug.LogWarning($"{nameof(HealthBarView)} on {name} requires a CanvasGroup.", this);
                return;
            }

            _isValid = true;
        }
    }
}
