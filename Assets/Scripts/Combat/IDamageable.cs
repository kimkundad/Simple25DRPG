namespace Simple25DRPG.Combat
{
    /// <summary>
    /// Represents an object that can receive combat damage.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Applies damage to this object.
        /// </summary>
        /// <param name="damage">Amount of damage to apply.</param>
        void TakeDamage(int damage);
    }
}
