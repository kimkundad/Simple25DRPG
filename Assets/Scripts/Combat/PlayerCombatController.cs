using System;
using Simple25DRPG.Input;
using UnityEngine;

namespace Simple25DRPG.Combat
{
    /// <summary>
    /// Handles player melee attack requests, animation triggers, and immediate hit detection.
    /// </summary>
    public sealed class PlayerCombatController : MonoBehaviour
    {
        private const int MaxHits = 16;

        // Animator setup: add an Attack trigger parameter, transition into an attack animation,
        // then return to locomotion after the animation finishes.
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        [Header("Dependencies")]
        [Tooltip("Input reader that raises attack requests.")]
        [SerializeField] private PlayerInputReader _inputReader;

        [Tooltip("Animator that receives the Attack trigger.")]
        [SerializeField] private Animator _animator;

        [Tooltip("Settings asset that controls attack timing, size, damage, and hittable layers.")]
        [SerializeField] private AttackSettings _attackSettings;

        [Tooltip("Transform used as the center of melee hit detection.")]
        [SerializeField] private Transform _attackOrigin;

        private readonly Collider[] _hitResults = new Collider[MaxHits];
        private readonly IDamageable[] _damagedTargets = new IDamageable[MaxHits];
        private float _nextAttackTime;
        private bool _isValid;

        private void Awake()
        {
            if (_inputReader == null)
            {
                _inputReader = GetComponent<PlayerInputReader>();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }

            ValidateDependencies();
        }

        private void OnEnable()
        {
            if (!_isValid)
            {
                return;
            }

            _inputReader.AttackPressed += TryAttack;
        }

        private void OnDisable()
        {
            if (_inputReader != null)
            {
                _inputReader.AttackPressed -= TryAttack;
            }
        }

        /// <summary>
        /// Attempts to start an attack if cooldown and dependencies allow it.
        /// </summary>
        public void TryAttack()
        {
            if (!_isValid || Time.time < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = Time.time + _attackSettings.AttackCooldown;
            _animator.SetTrigger(AttackTrigger);

            // This is intentionally isolated so an animation event can call it later.
            PerformAttackHitDetection();
        }

        /// <summary>
        /// Performs immediate melee hit detection and applies damage to unique damageable targets.
        /// </summary>
        public void PerformAttackHitDetection()
        {
            if (!_isValid)
            {
                return;
            }

            Array.Clear(_damagedTargets, 0, _damagedTargets.Length);

            int hitCount = Physics.OverlapSphereNonAlloc(
                _attackOrigin.position,
                _attackSettings.AttackRadius,
                _hitResults,
                _attackSettings.HittableLayers,
                QueryTriggerInteraction.Ignore);

            int damagedCount = 0;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _hitResults[i];
                if (hit == null || !TryGetDamageable(hit, out IDamageable damageable) || HasAlreadyDamaged(damageable, damagedCount))
                {
                    continue;
                }

                damageable.TakeDamage(_attackSettings.Damage);

                if (damagedCount < _damagedTargets.Length)
                {
                    _damagedTargets[damagedCount] = damageable;
                    damagedCount++;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_attackOrigin == null || _attackSettings == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackOrigin.position, _attackSettings.AttackRadius);
        }

        private void ValidateDependencies()
        {
            if (_inputReader == null)
            {
                Debug.LogWarning($"{nameof(PlayerCombatController)} on {name} requires a PlayerInputReader.", this);
                return;
            }

            if (_animator == null)
            {
                Debug.LogWarning($"{nameof(PlayerCombatController)} on {name} requires an Animator.", this);
                return;
            }

            if (_attackSettings == null)
            {
                Debug.LogWarning($"{nameof(PlayerCombatController)} on {name} requires AttackSettings.", this);
                return;
            }

            if (_attackOrigin == null)
            {
                Debug.LogWarning($"{nameof(PlayerCombatController)} on {name} requires an attack origin Transform.", this);
                return;
            }

            _isValid = true;
        }

        private static bool TryGetDamageable(Collider hit, out IDamageable damageable)
        {
            if (hit.TryGetComponent(out damageable))
            {
                return true;
            }

            Transform current = hit.transform.parent;
            while (current != null)
            {
                if (current.TryGetComponent(out damageable))
                {
                    return true;
                }

                current = current.parent;
            }

            damageable = null;
            return false;
        }

        private bool HasAlreadyDamaged(IDamageable damageable, int damagedCount)
        {
            for (int i = 0; i < damagedCount; i++)
            {
                if (ReferenceEquals(_damagedTargets[i], damageable))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
