using Simple25DRPG.Enemy;
using Simple25DRPG.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Spawns floating damage numbers on the gameplay Canvas when a registered actor takes damage.
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

        [Header("Position")]
        [Tooltip("World-space offset applied above the damaged target.")]
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 2f, 0f);

        private readonly List<EnemyHealth> _subscribedEnemies = new List<EnemyHealth>();
        private readonly List<PlayerHealth> _subscribedPlayers = new List<PlayerHealth>();
        private Canvas _canvas;
        private UnityEngine.Camera _eventCamera;
        private bool _isValid;

        private void Awake()
        {
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

            SubscribeToSceneEnemies();
            SubscribeToScenePlayers();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _subscribedEnemies.Count; i++)
            {
                if (_subscribedEnemies[i] != null)
                {
                    _subscribedEnemies[i].DamageTaken -= Spawn;
                }
            }

            _subscribedEnemies.Clear();

            for (int i = 0; i < _subscribedPlayers.Count; i++)
            {
                if (_subscribedPlayers[i] != null)
                {
                    _subscribedPlayers[i].DamageTaken -= Spawn;
                }
            }

            _subscribedPlayers.Clear();
        }

        /// <summary>
        /// Subscribes this spawner to an enemy health component.
        /// </summary>
        /// <param name="enemyHealth">Enemy health component that raises damage events.</param>
        public void RegisterEnemy(EnemyHealth enemyHealth)
        {
            if (enemyHealth == null || _subscribedEnemies.Contains(enemyHealth))
            {
                return;
            }

            enemyHealth.DamageTaken += Spawn;
            _subscribedEnemies.Add(enemyHealth);
        }

        /// <summary>
        /// Unsubscribes this spawner from an enemy health component.
        /// </summary>
        /// <param name="enemyHealth">Enemy health component to unsubscribe from.</param>
        public void UnregisterEnemy(EnemyHealth enemyHealth)
        {
            if (enemyHealth == null)
            {
                return;
            }

            int index = _subscribedEnemies.IndexOf(enemyHealth);
            if (index < 0)
            {
                return;
            }

            enemyHealth.DamageTaken -= Spawn;
            _subscribedEnemies.RemoveAt(index);
        }

        /// <summary>
        /// Subscribes this spawner to a player health component.
        /// </summary>
        /// <param name="playerHealth">Player health component that raises damage events.</param>
        public void RegisterPlayer(PlayerHealth playerHealth)
        {
            if (playerHealth == null || _subscribedPlayers.Contains(playerHealth))
            {
                return;
            }

            playerHealth.DamageTaken += Spawn;
            _subscribedPlayers.Add(playerHealth);
        }

        /// <summary>
        /// Unsubscribes this spawner from a player health component.
        /// </summary>
        /// <param name="playerHealth">Player health component to unsubscribe from.</param>
        public void UnregisterPlayer(PlayerHealth playerHealth)
        {
            if (playerHealth == null)
            {
                return;
            }

            int index = _subscribedPlayers.IndexOf(playerHealth);
            if (index < 0)
            {
                return;
            }

            playerHealth.DamageTaken -= Spawn;
            _subscribedPlayers.RemoveAt(index);
        }

        private void Spawn(int damageAmount, Vector3 worldPosition)
        {
            Vector3 spawnWorldPosition = worldPosition + _worldOffset;
            Vector3 screenPosition = _worldCamera.WorldToScreenPoint(spawnWorldPosition);

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
                numberTransform.localScale = Vector3.one;
                numberTransform.anchoredPosition = localPoint;
                numberTransform.SetAsLastSibling();
            }

            number.Show(damageAmount);
        }

        private void SubscribeToSceneEnemies()
        {
            EnemyHealth[] enemyHealthComponents = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            for (int i = 0; i < enemyHealthComponents.Length; i++)
            {
                RegisterEnemy(enemyHealthComponents[i]);
            }
        }

        private void SubscribeToScenePlayers()
        {
            PlayerHealth[] playerHealthComponents = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
            for (int i = 0; i < playerHealthComponents.Length; i++)
            {
                RegisterPlayer(playerHealthComponents[i]);
            }
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
