using S8l.BaseConfiguration;
using UnityEngine;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using System.Collections.Generic;

namespace S8l.FirstPersonController.Runtime
{
    [CreateAssetMenu(order = 0, fileName = "Fpc Config", menuName = MenuName + "FPC")]
    public class FpcPackageConfiguration : PackageConfiguration
    {
        [SerializeField]
        public IFpcConfig config = new FpcConfig();    
        [SerializeField]
        public IFpcRaycastStrategy raycastStrategy = new FpcRaycastDefault();
        [SerializeField]
        public Dictionary<BasePlatform, FpcInputStrategy> inputStrategies;
    }
}