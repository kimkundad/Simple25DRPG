using Simple25DRPG.Enemy;
using Simple25DRPG.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Simple25DRPG.UI
{
    /// <summary>
    /// Spawns and registers UI health bars for scene health components.
    /// </summary>
    public sealed class HealthBarSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Camera used to position health bars above world targets.")]
        [SerializeField] private UnityEngine.Camera _worldCamera;

        [Tooltip("Canvas RectTransform that receives health bar instances.")]
        [SerializeField] private RectTransform _canvasRect;

        [Tooltip("Health bar prefab to spawn for each registered health owner.")]
        [SerializeField] private HealthBarView _prefab;

        private readonly List<HealthBarRegistration> _registrations = new List<HealthBarRegistration>();
        private bool _hasStarted;
        private bool _isValid;

        private void Awake()
        {
            ValidateDependencies();
        }

        private void OnEnable()
        {
            if (!_hasStarted)
            {
                return;
            }

            RegisterSceneHealthComponents();
        }

        private void Start()
        {
            _hasStarted = true;
            RegisterSceneHealthComponents();
        }

        private void OnDisable()
        {
            for (int i = 0; i < _registrations.Count; i++)
            {
                _registrations[i].Dispose();
            }

            _registrations.Clear();
        }

        private void RegisterSceneHealthComponents()
        {
            if (!_isValid)
            {
                return;
            }

            RegisterScenePlayers();
            RegisterSceneEnemies();
        }

        /// <summary>
        /// Registers a player health component and creates a health bar for it.
        /// </summary>
        /// <param name="health">Player health component to display.</param>
        public void RegisterPlayer(PlayerHealth health)
        {
            if (health == null || ContainsOwner(health))
            {
                return;
            }

            HealthBarView view = CreateView(health.transform);
            HealthBarRegistration registration = HealthBarRegistration.ForPlayer(health, view);
            registration.Initialize();
            _registrations.Add(registration);
        }

        /// <summary>
        /// Registers an enemy health component and creates a health bar for it.
        /// </summary>
        /// <param name="health">Enemy health component to display.</param>
        public void RegisterEnemy(EnemyHealth health)
        {
            if (health == null || ContainsOwner(health))
            {
                return;
            }

            HealthBarView view = CreateView(health.transform);
            HealthBarRegistration registration = HealthBarRegistration.ForEnemy(health, view);
            registration.Initialize();
            _registrations.Add(registration);
        }

        /// <summary>
        /// Unregisters a health component and removes its health bar.
        /// </summary>
        /// <param name="owner">Registered health owner to remove.</param>
        public void Unregister(Component owner)
        {
            for (int i = _registrations.Count - 1; i >= 0; i--)
            {
                if (_registrations[i].Owner != owner)
                {
                    continue;
                }

                _registrations[i].Dispose();
                _registrations.RemoveAt(i);
                return;
            }
        }

        private HealthBarView CreateView(Transform target)
        {
            HealthBarView view = Instantiate(_prefab, _canvasRect, false);
            RectTransform viewTransform = view.transform as RectTransform;
            if (viewTransform != null)
            {
                viewTransform.localScale = Vector3.one;
                viewTransform.SetAsLastSibling();
            }

            HealthBarFollower follower = view.GetComponent<HealthBarFollower>();
            if (follower != null)
            {
                follower.Initialize(target, _worldCamera, _canvasRect);
            }

            return view;
        }

        private bool ContainsOwner(Component owner)
        {
            for (int i = 0; i < _registrations.Count; i++)
            {
                if (_registrations[i].Owner == owner)
                {
                    return true;
                }
            }

            return false;
        }

        private void RegisterScenePlayers()
        {
            PlayerHealth[] players = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++)
            {
                RegisterPlayer(players[i]);
            }
        }

        private void RegisterSceneEnemies()
        {
            EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                RegisterEnemy(enemies[i]);
            }
        }

        private void ValidateDependencies()
        {
            if (_worldCamera == null)
            {
                Debug.LogWarning($"{nameof(HealthBarSpawner)} on {name} requires a world Camera.", this);
                return;
            }

            if (_canvasRect == null)
            {
                Debug.LogWarning($"{nameof(HealthBarSpawner)} on {name} requires a Canvas RectTransform.", this);
                return;
            }

            if (_prefab == null)
            {
                Debug.LogWarning($"{nameof(HealthBarSpawner)} on {name} requires a HealthBar prefab.", this);
                return;
            }

            _isValid = true;
        }

        private sealed class HealthBarRegistration
        {
            private readonly PlayerHealth _playerHealth;
            private readonly EnemyHealth _enemyHealth;
            private readonly HealthBarView _view;
            private readonly int _maxHp;

            private HealthBarRegistration(Component owner, PlayerHealth playerHealth, EnemyHealth enemyHealth, HealthBarView view, int maxHp)
            {
                Owner = owner;
                _playerHealth = playerHealth;
                _enemyHealth = enemyHealth;
                _view = view;
                _maxHp = maxHp;
            }

            public Component Owner { get; }

            public static HealthBarRegistration ForPlayer(PlayerHealth health, HealthBarView view)
            {
                return new HealthBarRegistration(health, health, null, view, health.MaxHp);
            }

            public static HealthBarRegistration ForEnemy(EnemyHealth health, HealthBarView view)
            {
                return new HealthBarRegistration(health, null, health, view, health.CurrentHp);
            }

            public void Initialize()
            {
                _view.SetHealth(_maxHp, _maxHp);

                if (_playerHealth != null)
                {
                    _playerHealth.OnDamaged += HandleDamaged;
                    _playerHealth.OnDied += HandleDied;
                    return;
                }

                if (_enemyHealth != null)
                {
                    _enemyHealth.OnDamaged += HandleDamaged;
                    _enemyHealth.OnDied += HandleDied;
                }
            }

            public void Unsubscribe()
            {
                if (_playerHealth != null)
                {
                    _playerHealth.OnDamaged -= HandleDamaged;
                    _playerHealth.OnDied -= HandleDied;
                }

                if (_enemyHealth != null)
                {
                    _enemyHealth.OnDamaged -= HandleDamaged;
                    _enemyHealth.OnDied -= HandleDied;
                }
            }

            public void Dispose()
            {
                Unsubscribe();
                DestroyView();
            }

            private void HandleDamaged(int currentHp, int appliedDamage)
            {
                _view.SetHealth(currentHp, _maxHp);
            }

            private void HandleDied()
            {
                DestroyView();
            }

            private void DestroyView()
            {
                if (_view != null)
                {
                    UnityEngine.Object.Destroy(_view.gameObject);
                }
            }
        }
    }
}
