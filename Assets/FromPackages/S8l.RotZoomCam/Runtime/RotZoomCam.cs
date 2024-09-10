using System;
using System.Collections.Generic;
using S8l.CustomPlatformDetector.Runtime;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using S8l.CustomPlatformSharedInterfaces.Runtime;
#if ENABLE_LEGACY_INPUT_MANAGER
using S8l.RotZoomCam.Runtime.Implementations.Legacy;
#endif
using S8l.RotZoomCam.Runtime.Interfaces;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

namespace S8l.RotZoomCam.Runtime
{
    [RequireComponent(typeof(Camera))]
    public class RotZoomCam : SerializedMonoBehaviour, IRotZoomCam
    {
        public Action OnUpdate { get; set; }
        public Camera CameraReference {
            get { return this.GetComponent<Camera>(); }
        }
    

        [OdinSerialize]
        private Dictionary<IPlatform, IRotStrategy> RotStrategies = new Dictionary<IPlatform, IRotStrategy>()
        #if ENABLE_LEGACY_INPUT_MANAGER
        {
            {new Any(), new LegacyMouseRotStrategy()}, 
            {new IPhonePlayer(), new LegacyTouchRotStrategy()},
            {new AndroidNonVR(), new LegacyTouchRotStrategy()}
        }
        #endif
        ;
    
        [OdinSerialize]
        private Dictionary<IPlatform, IZoomStrategy> ZoomStrategies = new Dictionary<IPlatform, IZoomStrategy>()
        #if ENABLE_LEGACY_INPUT_MANAGER
        {
            {new Any(), new LegacyMouseZoomStrategy()}, 
            {new IPhonePlayer(), new LegacyTouchZoomStrategy()},
            {new AndroidNonVR(), new LegacyTouchZoomStrategy()}
        }
        #endif
        ;



        void Awake()
        {

            var detectedPlatforms = CustomPlatformDetectorFacade.DetectPlatforms(orderBy:true);
        
            InitRotStrategy(detectedPlatforms);
            InitZoomStrategy(detectedPlatforms);
        }




        void LateUpdate()
        {
            OnUpdate?.Invoke();
        }


        #region Initialization

        void InitRotStrategy(List<IPlatform> detectedPlatforms)
        {
        
            // Init only the strategy with the least generality which is found
            foreach (var detectedPlatform in detectedPlatforms)
            {
                foreach (var keyValue in RotStrategies)
                {
                    if (detectedPlatform.Equals(keyValue.Key))
                    {
                        keyValue.Value?.Init(this);
                        return;
                    }
                }
            }
        }

        void InitZoomStrategy(List<IPlatform> detectedPlatforms)
        {
            // Init only the strategy with the least generality which is found
            foreach (var detectedPlatform in detectedPlatforms)
            {
                foreach (var keyValue in ZoomStrategies)
                {
                    if (detectedPlatform.Equals(keyValue.Key))
                    {
                        keyValue.Value?.Init(this);
                        return;
                    }
                }
            }
        }

        #endregion


    }
}
