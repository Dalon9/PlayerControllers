using UnityEngine;

namespace S8l.Guardian.Runtime.Interfaces
{
    public interface IGuardianShapeProvider
    {
        /// <summary>
        /// Sample a given position in the play area
        /// </summary>
        /// <param name="position">The position to sample</param>
        /// <returns>A float between 0 and 1 indicating how far the player is from the play area bounds.
        /// A value of 0 indicates that the player is inside the play area soft bounds, a value of 1
        /// indicates that the player is outside the play area hard bounds.</returns>
        float SamplePosition(Vector3 position);
    }
}
