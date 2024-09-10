using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class EditorNot : BasePlatform
    {
        public override int Generality { get; } = 150;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
                
        public override bool IsThisMyPlatform()
        {
            return IsMyPlatform();
        }
        

    }
}