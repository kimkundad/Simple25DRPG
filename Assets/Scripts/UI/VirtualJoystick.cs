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

        private Canvas _canvas;
        private UnityEngine.Camera _eventCamera;
        private float _radius;

        /// <summary>
        /// Gets the current normalized joystick direction.
        /// </summary>
        public Vector2 Direction { get; private set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            if (_background == null || _handle == null)
            {
                Debug.LogWarning($"{nameof(VirtualJoystick)} on {name} requires background and handle RectTransform references.", this);
                enabled = false;
                return;
            }

            if (_canvas == null)
            {
                Debug.LogWarning($"{nameof(VirtualJoystick)} on {name} requires a parent Canvas.", this);
                enabled = false;
                return;
            }

            _eventCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
            _radius = Mathf.Min(_background.rect.width, _background.rect.height) * 0.5f;

            if (_radius <= 0f)
            {
                Debug.LogWarning($"{nameof(VirtualJoystick)} on {name} has an invalid background radius.", this);
                enabled = false;
            }
        }

        /// <summary>
        /// Updates joystick direction when pointer input starts.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_EDITOR
            Debug.Log("VirtualJoystick pointer down.", this);
#endif
            UpdateDirection(eventData);
        }

        /// <summary>
        /// Updates joystick direction while pointer input is dragged.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnDrag(PointerEventData eventData)
        {
#if UNITY_EDITOR
            Debug.Log("VirtualJoystick drag.", this);
#endif
            UpdateDirection(eventData);
        }

        /// <summary>
        /// Resets joystick direction when pointer input is released.
        /// </summary>
        /// <param name="eventData">Pointer data provided by Unity's EventSystem.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_EDITOR
            Debug.Log("VirtualJoystick pointer up.", this);
#endif
            Direction = Vector2.zero;
            _handle.anchoredPosition = Vector2.zero;
        }

        private void UpdateDirection(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, _eventCamera, out Vector2 localPoint))
            {
                return;
            }

            Vector2 handlePosition = Vector2.ClampMagnitude(localPoint, _radius);
            _handle.anchoredPosition = handlePosition;
            Direction = handlePosition / _radius;
        }
    }
}
