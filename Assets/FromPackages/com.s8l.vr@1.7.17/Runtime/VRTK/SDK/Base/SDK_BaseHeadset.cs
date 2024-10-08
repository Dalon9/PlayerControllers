﻿// Base Headset|SDK_Base|005
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
#if UNITY_2017_2_OR_NEWER
    using UnityEngine.XR;
#else
    using XRDevice = UnityEngine.VR.VRDevice;
    using XRSettings = UnityEngine.VR.VRSettings;
#endif

    /// <summary>
    /// The Base Headset SDK script provides a bridge to SDK methods that deal with the VR Headset.
    /// </summary>
    /// <remarks>
    /// This is an abstract class to implement the interface required by all implemented SDKs.
    /// </remarks>
    public abstract class SDK_BaseHeadset : SDK_Base
    {
        /// <summary>
        /// The connected headset type
        /// </summary>
        public enum HeadsetType
        {
            /// <summary>
            /// The headset connected is unknown.
            /// </summary>
            Undefined,
            /// <summary>
            /// The headset associated with the simulator.
            /// </summary>
            Simulator,
            /// <summary>
            /// The HTC Vive headset.
            /// </summary>
            HTCVive,
            /// <summary>
            /// The Oculus Rift DK1 headset.
            /// </summary>
            OculusRiftDK1,
            /// <summary>
            /// The Oculus Rift DK2 headset.
            /// </summary>
            OculusRiftDK2,
            /// <summary>
            /// The Oculus Rift headset.
            /// </summary>
            OculusRift,
            /// <summary>
            /// The Oculus GearVR headset.
            /// </summary>
            OculusGearVR,
            /// <summary>
            /// The Google Daydream headset.
            /// </summary>
            GoogleDaydream,
            /// <summary>
            /// The Google Cardboard headset.
            /// </summary>
            GoogleCardboard,
            /// <summary>
            /// The HyperealVR headset.
            /// </summary>
            HyperealVR,
            /// <summary>
            /// The Windows Mixed Reality headset.
            /// </summary>
            WindowsMixedReality
        }
        protected Transform cachedHeadset;
        protected Transform cachedHeadsetCamera;

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public abstract void ProcessUpdate(Dictionary<string, object> options);

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public abstract void ProcessFixedUpdate(Dictionary<string, object> options);

        /// <summary>
        /// The GetHeadset method returns the Transform of the object that is used to represent the headset in the scene.
        /// </summary>
        /// <returns>A transform of the object representing the headset in the scene.</returns>
        public abstract Transform GetHeadset();

        /// <summary>
        /// The GetHeadsetCamera method returns the Transform of the object that is used to hold the headset camera in the scene.
        /// </summary>
        /// <returns>A transform of the object holding the headset camera in the scene.</returns>
        public abstract Transform GetHeadsetCamera();

        /// <summary>
        /// The GetHeadsetType method returns a string representing the type of headset connected.
        /// </summary>
        /// <returns>The string of the headset connected.</returns>
        public abstract string GetHeadsetType();

        /// <summary>
        /// The GetHeadsetVelocity method is used to determine the current velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current velocity of the headset.</returns>
        public abstract Vector3 GetHeadsetVelocity();

        /// <summary>
        /// The GetHeadsetAngularVelocity method is used to determine the current angular velocity of the headset.
        /// </summary>
        /// <returns>A Vector3 containing the current angular velocity of the headset.</returns>
        public abstract Vector3 GetHeadsetAngularVelocity();

        /// <summary>
        /// The HeadsetFade method is used to apply a fade to the headset camera to progressively change the colour.
        /// </summary>
        /// <param name="color">The colour to fade to.</param>
        /// <param name="duration">The amount of time the fade should take to reach the given colour.</param>
        /// <param name="fadeOverlay">Determines whether to use an overlay on the fade.</param>
        public abstract void HeadsetFade(Color color, float duration, bool fadeOverlay = false);

        /// <summary>
        /// The HasHeadsetFade method checks to see if the given game object (usually the camera) has the ability to fade the viewpoint.
        /// </summary>
        /// <param name="obj">The Transform to check to see if a camera fade is available on.</param>
        /// <returns>Returns true if the headset has fade functionality on it.</returns>
        public abstract bool HasHeadsetFade(Transform obj);

        /// <summary>
        /// The AddHeadsetFade method attempts to add the fade functionality to the game object with the camera on it.
        /// </summary>
        /// <param name="camera">The Transform to with the camera on to add the fade functionality to.</param>
        public abstract void AddHeadsetFade(Transform camera);

        protected Transform GetSDKManagerHeadset()
        {
            VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null && sdkManager.loadedSetup != null && sdkManager.loadedSetup.actualHeadset != null)
            {
                cachedHeadset = (sdkManager.loadedSetup.actualHeadset ? sdkManager.loadedSetup.actualHeadset.transform : null);
                return cachedHeadset;
            }
            return null;
        }

        protected virtual string ScrapeHeadsetType()
        {
            string model = CleanPropertyString(InputDevices.GetDeviceAtXRNode(XRNode.Head).name);
            string deviceName = CleanPropertyString(XRSettings.loadedDeviceName);
            switch (model)
            {
                case "oculusriftcv1":
                case "oculusriftes07":
                    return CleanPropertyString("oculusrift");
                case "vivemv":
                case "vivedvt":
                    return CleanPropertyString("htcvive");
                case "googleinc-daydreamview":
                    return "googledaydream";
                case "googleinc-defaultcardboard":
                    return "googlecardboard";
                case "galaxynote5":
                case "galaxys6":
                case "galaxys6edge":
                case "galaxys7":
                case "galaxys7edge":
                case "galaxys8":
                case "galaxys8+":
                    if (deviceName == "oculus")
                    {
                        return "oculusgearvr";
                    }
                    break;
                case "oculusriftdk1":
                    return CleanPropertyString("oculusriftdk1");
                case "oculusriftdk2":
                    return CleanPropertyString("oculusriftdk2");
                case "acermixedreality":
                    return CleanPropertyString("windowsmixedreality");
            }
            return "";
        }

        protected string CleanPropertyString(string inputString)
        {
            return inputString.Replace(" ", "").Replace(".", "").Replace(",", "").ToLowerInvariant();
        }
    }
}