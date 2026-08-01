using System.Collections.Generic;
using Simple25DRPG.Enemy;
using Simple25DRPG.Items;
using UnityEngine;

namespace Simple25DRPG.Loot
{
    /// <summary>
    /// Spawns loot selected from a loot table when an enemy dies.
    /// </summary>
    public sealed class LootDropController : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Enemy health that raises the death event.")]
        [SerializeField] private EnemyHealth _enemyHealth;

        [Tooltip("Loot table used to select the dropped item.")]
        [SerializeField] private LootTable _lootTable;

        [Header("Spawn")]
        [Tooltip("Optional transform used as the center of loot drops.")]
        [SerializeField] private Transform _dropOrigin;

        [Tooltip("Random horizontal radius used to separate loot from the enemy center.")]
        [Min(0f)]
        [SerializeField] private float _scatterRadius = 0.75f;

        [Tooltip("Vertical offset applied to spawned loot.")]
        [SerializeField] private float _verticalOffset = 0.25f;

        private bool _hasDropped;
        private bool _isValid;

        private void Awake()
        {
            if (_enemyHealth == null)
            {
                _enemyHealth = GetComponent<EnemyHealth>();
            }

            ValidateDependencies();
        }

        private void OnEnable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.OnDied += HandleDied;
            }
        }

        private void OnDisable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.OnDied -= HandleDied;
            }
        }

        private void OnValidate()
        {
            _scatterRadius = Mathf.Max(0f, _scatterRadius);
        }

        private void HandleDied()
        {
            if (!_isValid || _hasDropped)
            {
                return;
            }

            _hasDropped = true;
            DropLoot();
        }

        private void DropLoot()
        {
            IReadOnlyList<LootEntry> entries = _lootTable.Entries;
            for (int i = 0; i < entries.Count; i++)
            {
                LootEntry entry = entries[i];
                if (entry == null || entry.Item == null)
                {
                    Debug.LogWarning($"{nameof(LootDropController)} on {name} has a loot entry with missing ItemData.", this);
                    continue;
                }

                if (entry.Item.PickupPrefab == null)
                {
                    Debug.LogWarning($"{nameof(LootDropController)} on {name} has item '{entry.Item.DisplayName}' with no pickup prefab.", this);
                    continue;
                }

                if (_lootTable.TryRollEntry(entry, out int amount))
                {
                    SpawnPickup(entry.Item, amount);
                }
            }
        }

        private void SpawnPickup(ItemData item, int amount)
        {
            Vector2 randomOffset = Random.insideUnitCircle * _scatterRadius;
            Vector3 origin = _dropOrigin != null ? _dropOrigin.position : transform.position;
            Vector3 spawnPosition = origin + new Vector3(randomOffset.x, _verticalOffset, randomOffset.y);
            GameObject pickupObject = Instantiate(item.PickupPrefab, spawnPosition, Quaternion.identity);

            if (pickupObject.TryGetComponent(out LootPickup pickup))
            {
                pickup.Initialize(item, amount);
            }

#if UNITY_EDITOR
            Debug.Log($"Enemy dropped {amount} x {item.DisplayName}.", this);
#endif
        }

        private void ValidateDependencies()
        {
            if (_enemyHealth == null)
            {
                Debug.LogWarning($"{nameof(LootDropController)} on {name} requires EnemyHealth.", this);
                return;
            }

            if (_lootTable == null)
            {
                Debug.LogWarning($"{nameof(LootDropController)} on {name} requires a LootTable.", this);
                return;
            }

            _isValid = true;
        }
    }
}
