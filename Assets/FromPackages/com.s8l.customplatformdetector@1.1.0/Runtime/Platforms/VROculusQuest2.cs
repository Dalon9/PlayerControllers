using System;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Management;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class VROculusQuest2 : BasePlatform
    {
        public override int Generality { get; } = 0;


        public static bool IsMyPlatform()
        {


            if (SystemInfo.deviceName == "Oculus Quest 2")
                return true;

            return false;

        }
                
        public override bool IsThisMyPlatform()
        {
            return IsMyPlatform();
        }
        

    }
}