using TMPro;
using UnityEngine;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Displays a single floating damage number and removes it after its lifetime.
    /// </summary>
    public sealed class FloatingDamageNumber : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Text component used to display the damage amount.")]
        [SerializeField] private TMP_Text _text;

        [Tooltip("Canvas group used to fade the damage number.")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Display")]
        [Tooltip("Seconds before the damage number is cleaned up.")]
        [Min(0.01f)]
        [SerializeField] private float _lifetime = 0.8f;

        [Tooltip("Vertical UI units moved per second.")]
        [Min(0f)]
        [SerializeField] private float _upwardSpeed = 50f;

        [Tooltip("Maximum random horizontal spawn offset in UI units.")]
        [Min(0f)]
        [SerializeField] private float _horizontalRandomOffset = 12f;

        [Tooltip("Font size applied when the number is shown.")]
        [Min(1f)]
        [SerializeField] private float _fontSize = 36f;

        private RectTransform _rectTransform;
        private float _elapsed;
        private bool _isPlaying;

        /// <summary>
        /// Initializes and starts the floating damage number.
        /// </summary>
        /// <param name="damageAmount">Damage amount to display.</param>
        public void Show(int damageAmount)
        {
            if (!enabled)
            {
                return;
            }

            _elapsed = 0f;
            _isPlaying = true;

            if (_text != null)
            {
                _text.SetText("{0}", damageAmount);
                _text.fontSize = _fontSize;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            if (_rectTransform != null && _horizontalRandomOffset > 0f)
            {
                Vector2 position = _rectTransform.anchoredPosition;
                position.x += Random.Range(-_horizontalRandomOffset, _horizontalRandomOffset);
                _rectTransform.anchoredPosition = position;
            }
        }

        private void Awake()
        {
            _rectTransform = transform as RectTransform;

            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_rectTransform == null || _text == null || _canvasGroup == null)
            {
                Debug.LogWarning($"{nameof(FloatingDamageNumber)} on {name} requires RectTransform, TMP_Text, and CanvasGroup.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_elapsed / _lifetime);

            Vector2 position = _rectTransform.anchoredPosition;
            position.y += _upwardSpeed * Time.deltaTime;
            _rectTransform.anchoredPosition = position;

            _canvasGroup.alpha = 1f - normalizedTime;

            if (_elapsed >= _lifetime)
            {
                Cleanup();
            }
        }

        private void OnValidate()
        {
            _lifetime = Mathf.Max(0.01f, _lifetime);
            _upwardSpeed = Mathf.Max(0f, _upwardSpeed);
            _horizontalRandomOffset = Mathf.Max(0f, _horizontalRandomOffset);
            _fontSize = Mathf.Max(1f, _fontSize);
        }

        private void Cleanup()
        {
            Destroy(gameObject);
        }
    }
}
