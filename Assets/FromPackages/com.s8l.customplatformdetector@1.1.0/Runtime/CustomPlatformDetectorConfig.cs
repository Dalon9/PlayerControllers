using System.Collections.Generic;
using S8l.BaseConfiguration;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using UnityEngine;

namespace S8l.CustomPlatformDetector.Runtime
{
    [CreateAssetMenu(order = 0, fileName = "CustomPlatformDetector Config", menuName = MenuName + "CustomPlatformDetector")]
    public class CustomPlatformDetectorConfig : PackageConfiguration
    {
        public bool CacheResult = true;

        [SerializeReference] public List<IPlatform> DoOverrideWith;
    
        [Space(20)]
        [SerializeReference]
        public List<IPlatform> KnownPlatforms = new List<IPlatform> {new Any(), new EditorAny(), new EditorNot(), new EditorPC(), new EditorLinux(), new EditorMac(), new Android(), new IPhonePlayer(), new WebGLPlayer(), new WindowsPlayer(), new LinuxPlayer(), new MacPlayer(), new AndroidNonVR(), new AndroidVR(), new VRHTCFocus(), new VRPico(), new VROculusQuest(), new VROculusQuest1(), new VROculusQuest2()};
    
    
    
    }
}
