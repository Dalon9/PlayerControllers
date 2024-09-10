using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Management;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class VROculusQuest1 : BasePlatform
    {
        public override int Generality { get; } = 0;


        public static bool IsMyPlatform()
        {


            if (SystemInfo.deviceName == "Oculus Quest")
                return true;

            return false;

        }
                
        public override bool IsThisMyPlatform()
        {
            return IsMyPlatform();
        }
        

    }
}