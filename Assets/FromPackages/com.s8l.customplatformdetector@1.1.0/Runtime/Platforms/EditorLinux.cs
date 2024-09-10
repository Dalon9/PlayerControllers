using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class EditorLinux : BasePlatform
    {
        public override int Generality { get; } = 50;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.LinuxEditor)
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
        

    }
}