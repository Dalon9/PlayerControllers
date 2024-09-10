using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class EditorAny : BasePlatform
    {
        public override int Generality { get; } = 150;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor)
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