namespace S8l.CustomPlatformSharedInterfaces.Runtime
{
    public interface IPlatform
    {
        /// <summary>
        /// Let the platform decide if "this" is a match or not
        /// </summary>
        /// <returns>This matches this platform</returns>
        bool IsThisMyPlatform();

        /// <summary>
        /// How vage / big is this definition, can be used for prioritizing
        /// </summary>
        int Generality { get; }
        
    }
}