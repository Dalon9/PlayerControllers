using S8l.CustomPlatformSharedInterfaces.Runtime;
using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class StandaloneAny : BasePlatform
#if UNITY_EDITOR
        , IBuildPlatform 
#endif
    {
        public override int Generality { get; } = 100;


        public static bool IsMyPlatform()
        {
            if (WindowsPlayer.IsMyPlatform() || MacPlayer.IsMyPlatform() || LinuxPlayer.IsMyPlatform())
            {
                return true;
            }
            else
            {
                return false;
            }
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
            return WindowsPlayer.IsBuildPlatform() || MacPlayer.IsBuildPlatform() || LinuxPlayer.IsBuildPlatform();
        }
#endif       

    }
}