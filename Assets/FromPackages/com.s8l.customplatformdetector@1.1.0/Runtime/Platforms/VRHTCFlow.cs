using System;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using UnityEngine;
using UnityEngine.XR.Management;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class VRHTCFlow : BasePlatform

    {
        public override int Generality { get; } = 0;


        public static bool IsMyPlatform()
        {

            XRLoader loader = XRGeneralSettings.Instance?.Manager?.activeLoader;

            if (loader == null)
                return false;

            if (loader.name.Contains("Wave") && SystemInfo.deviceName.Contains("Flow"))
                return true;

            return false;
            
        }
                
        public override bool IsThisMyPlatform()
        {
            return IsMyPlatform();
        }
        

    }
}