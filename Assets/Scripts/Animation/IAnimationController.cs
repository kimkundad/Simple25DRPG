namespace Simple25DRPG.Animation
{
    /// <summary>
    /// Defines common animation commands used by gameplay controllers.
    /// </summary>
    public interface IAnimationController
    {
        /// <summary>
        /// Plays the attack animation.
        /// </summary>
        void PlayAttack();

        /// <summary>
        /// Plays the hit reaction animation.
        /// </summary>
        void PlayHit();

        /// <summary>
        /// Plays the death animation.
        /// </summary>
        void PlayDeath();

        /// <summary>
        /// Updates the movement speed animation parameter.
        /// </summary>
        /// <param name="speed">Current movement speed value.</param>
        void SetMoveSpeed(float speed);
    }
}
