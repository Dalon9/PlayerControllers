using S8l.CustomPlatformSharedInterfaces.Runtime;
using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class IPhonePlayer : BasePlatform
#if UNITY_EDITOR
        , IBuildPlatform 
#endif
    {
        public override int Generality { get; } = 0;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
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
            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS;
        }
#endif
    }
}