using System;
using System.Linq;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using UnityEngine;
using UnityEngine.XR.Management;

namespace S8l.CustomPlatformDetector.Runtime.Platforms
{
    public class VROculusQuest : BasePlatform
#if UNITY_EDITOR
        , IBuildPlatform 
#endif
    {
        public override int Generality { get; } = 5;


        public static bool IsMyPlatform()
        {


            XRLoader loader = XRGeneralSettings.Instance?.Manager?.activeLoader;

            if (loader == null)
                return false;

            if (loader.name == "Oculus Loader")
                return true;

            return false;

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
            var loaders = UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget
                .XRGeneralSettingsForBuildTarget(UnityEditor.BuildTargetGroup.Android).AssignedSettings.activeLoaders;


            return UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android && loaders.Count > 0 && loaders[0].name == "Oculus Loader";
            
        }
#endif

    }
}