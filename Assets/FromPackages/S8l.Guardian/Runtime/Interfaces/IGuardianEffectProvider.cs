namespace S8l.Guardian.Runtime.Interfaces
{
    public interface IGuardianEffectProvider
    {
        /// <summary>
        /// Plays an effect for the player when he leaves the play area bounds.
        /// </summary>
        /// <param name="strength">A value from 0 to 1 indicating how far the player is from the soft bounds.</param>
        void PlayEffect(float strength);
    }
}
