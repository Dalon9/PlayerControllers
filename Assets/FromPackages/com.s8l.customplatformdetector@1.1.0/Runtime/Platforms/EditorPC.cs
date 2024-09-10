using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class EditorPC : BasePlatform
    {
        public override int Generality { get; } = 50;


        public static bool IsMyPlatform()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
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