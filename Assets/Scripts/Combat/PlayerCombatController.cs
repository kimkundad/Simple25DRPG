using System;
using Simple25DRPG.Input;
using Simple25DRPG.Player;
using UnityEngine;

namespace Simple25DRPG.Combat
{
    /// <summary>
    /// Handles player melee attack requests, animation triggers, and immediate hit detection.
    /// </summary>
    public sealed class PlayerCombatController : MonoBehaviour
    {
        private const int MaxHits = 16;

        [Header("Dependencies")]
        [Tooltip("Input reader that raises attack requests.")]
        [SerializeField] private PlayerInputReader _inputReader;

        [Tooltip("Animation controller that receives attack animation requests.")]
        [SerializeField] private PlayerAnimationController _animationController;

        [Tooltip("Settings asset that controls attack timing, size, damage, and hittable layers.")]
        [SerializeField] private AttackSettings _attackSettings;

        [Tooltip("Transform used as the center of melee hit detection.")]
        [SerializeField] private Transform _attackOrigin;

        private readonly Collider[] _hitResults = new Collider[MaxHits];
        private readonly Component[] _damagedTargets = new Component[MaxHits];
        private float _nextAttackTime;
        private bool _isValid;

        private void Awake()
        {
            if (_inputReader == null)
            {
                _inputReader = GetComponent<PlayerInputReader>();
            }

            if (_animationController == null)
            {
                _animationController = GetComponent<PlayerAnimationController>();
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
            _animationController.PlayAttack();

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
                if (hit == null || !TryGetDamageable(hit, out IDamageable damageable, out Component damageableComponent) || HasAlreadyDamaged(damageableComponent, damagedCount))
                {
                    continue;
                }

                if (damagedCount < _damagedTargets.Length)
                {
                    _damagedTargets[damagedCount] = damageableComponent;
                    damagedCount++;
                }

                damageable.TakeDamage(_attackSettings.Damage);
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

            if (_animationController == null)
            {
                Debug.LogWarning($"{nameof(PlayerCombatController)} on {name} requires a PlayerAnimationController.", this);
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

        private static bool TryGetDamageable(Collider hit, out IDamageable damageable, out Component damageableComponent)
        {
            if (hit.TryGetComponent(out damageable))
            {
                damageableComponent = damageable as Component;
                return true;
            }

            Transform current = hit.transform.parent;
            while (current != null)
            {
                if (current.TryGetComponent(out damageable))
                {
                    damageableComponent = damageable as Component;
                    return true;
                }

                current = current.parent;
            }

            damageable = null;
            damageableComponent = null;
            return false;
        }

        private bool HasAlreadyDamaged(Component damageable, int damagedCount)
        {
            for (int i = 0; i < damagedCount; i++)
            {
                if (_damagedTargets[i] == damageable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
