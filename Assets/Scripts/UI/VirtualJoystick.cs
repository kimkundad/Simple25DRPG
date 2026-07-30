using UnityEngine;
using UnityEngine.EventSystems;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Provides normalized directional input from a draggable mobile joystick UI.
    /// </summary>
    public sealed class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Joystick References")]
        [Tooltip("RectTransform that defines the joystick input area and movement radius.")]
        [SerializeField] private RectTransform _background;

        [Tooltip("RectTransform moved visually while the player drags the joystick.")]
        [SerializeField] private RectTransform _handle;

        private float _radius;

        /// <summary>
        /// Gets the current normalized joystick direction.
        /// </summary>
        public Vector2 Direction { get; private set; }

        private void Awake()
        {
            if (_background == null)
            {
                _background = transform as RectTransform;
            }

            if (_background == null || _handle == null)
            {
                Debug.LogWarning($"{nameof(VirtualJoystick)} on {name} requires background and handle RectTransform references.", this);
                enabled = false;
                return;
            }

            _radius = Mathf.Min(_background.rect.width, _background.rect.height) * 0.5f;
        }

        /// <summary>
        /// Updates joystick direction when pointer input starts.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateDirection(eventData);
        }

        /// <summary>
        /// Updates joystick direction while pointer input is dragged.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnDrag(PointerEventData eventData)
        {
            UpdateDirection(eventData);
        }

        /// <summary>
        /// Resets joystick direction when pointer input is released.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
        }

        private void UpdateDirection(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                return;
            }

            Vector2 clampedPoint = Vector2.ClampMagnitude(localPoint, _radius);
            _handle.anchoredPosition = clampedPoint;
            Direction = _radius > Mathf.Epsilon ? clampedPoint / _radius : Vector2.zero;
        }
    }
}
