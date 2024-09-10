using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class EditorMac : BasePlatform
    {
        public override int Generality { get; } = 50;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
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