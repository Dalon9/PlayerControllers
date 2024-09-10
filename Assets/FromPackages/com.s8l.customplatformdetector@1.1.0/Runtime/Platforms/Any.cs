using S8l.CustomPlatformSharedInterfaces.Runtime;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class Any : BasePlatform 
#if UNITY_EDITOR
        , IBuildPlatform 
#endif
    {
        public override int Generality { get; } = 1000;


        public static bool IsMyPlatform()
        {

            return true;

        }
        
                
        public override bool IsThisMyPlatform()
        {
            return IsMyPlatform();
        }


        #if UNITY_EDITOR
        public bool IsThisBuildPlatform()
        {
            return IsBuildPlatform();
        }

        public static bool IsBuildPlatform()
        {
            return IsMyPlatform();
        }
        #endif
    }
}