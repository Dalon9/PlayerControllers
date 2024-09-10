using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRTK
{
    [SDK_Description(typeof(SDK_S8LSystem))]
    [SDK_Description(typeof(SDK_S8LSystem), 1)]
    [SDK_Description(typeof(SDK_S8LSystem), 2)]
    [SDK_Description(typeof(SDK_S8LSystem), 3)]
    public class SDK_S8LHeadset : SDK_BaseHeadset
    {
        protected VRTK_VelocityEstimator cachedHeadsetVelocityEstimator;

        public override void ProcessUpdate(Dictionary<string, object> options)
        {
        }

        public override void ProcessFixedUpdate(Dictionary<string, object> options)
        {
        }

        public override Transform GetHeadset()
        {
            cachedHeadset = GetSDKManagerHeadset();
            if (cachedHeadset == null)
            {
                cachedHeadset = VRTK_SharedMethods.FindEvenInactiveGameObject<Transform>("Camera Offset/Main Camera",true).transform;
            }

            return cachedHeadset;
        }

        public override Transform GetHeadsetCamera()
        {
            cachedHeadsetCamera = GetSDKManagerHeadset();
            if (cachedHeadsetCamera == null)
            {
                cachedHeadsetCamera = GetHeadset();
            }
            return cachedHeadsetCamera;
        }

        public override string GetHeadsetType()
        {
            var device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            return CleanPropertyString(device.name);
        }

        public override Vector3 GetHeadsetVelocity()
        {
            SetHeadsetCaches();
            return cachedHeadsetVelocityEstimator.GetVelocityEstimate();
        }

        public override Vector3 GetHeadsetAngularVelocity()
        {
            SetHeadsetCaches();
            return cachedHeadsetVelocityEstimator.GetAngularVelocityEstimate();
        }

        public override void HeadsetFade(Color color, float duration, bool fadeOverlay = false)
        {
            VRTK_ScreenFade.Start(color, duration);
        }

        public override bool HasHeadsetFade(Transform obj)
        {
            return obj.GetComponentInChildren<VRTK_ScreenFade>();
        }

        public override void AddHeadsetFade(Transform camera)
        {
            if (camera && !camera.GetComponent<VRTK_ScreenFade>())
            {
                camera.gameObject.AddComponent<VRTK_ScreenFade>();
            }
        }
        
        protected virtual void SetHeadsetCaches()
        {
            var currentHeadset = GetHeadset();
            if (cachedHeadsetVelocityEstimator == null && currentHeadset != null)
            {
                cachedHeadsetVelocityEstimator = (currentHeadset.GetComponent<VRTK_VelocityEstimator>() != null ? currentHeadset.GetComponent<VRTK_VelocityEstimator>() : currentHeadset.gameObject.AddComponent<VRTK_VelocityEstimator>());
            }
        }
    }
}