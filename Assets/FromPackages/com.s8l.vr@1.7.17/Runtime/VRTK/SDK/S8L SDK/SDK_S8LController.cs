using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.XR.Oculus;
using UnityEngine;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace VRTK
{
    [SDK_Description(typeof(SDK_S8LSystem))]
    [SDK_Description(typeof(SDK_S8LSystem), 1)]
    [SDK_Description(typeof(SDK_S8LSystem), 2)]
    [SDK_Description(typeof(SDK_S8LSystem), 3)]
    public class SDK_S8LController : SDK_BaseController
    {
        protected SDK_OculusBoundaries cachedBoundariesSDK;
        protected VRTK_TrackedController cachedLeftController;
        protected VRTK_TrackedController cachedRightController;

        protected VRTK_VelocityEstimator cachedLeftVelocityEstimator;
        protected VRTK_VelocityEstimator cachedRightVelocityEstimator;

        protected InputDevice RightDevice => InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        protected InputDevice LeftDevice => InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        protected HapticCapabilities hapticsCapabilitiesLeft;
        protected HapticCapabilities hapticsCapabilitiesRight;

        protected bool[] previousHairTriggerState = new bool[2];
        protected bool[] currentHairTriggerState = new bool[2];
        protected float[] hairTriggerLimit = new float[2];

        protected Dictionary<string, bool[]> pressedLastState = new Dictionary<string, bool[]>();

        public override void ProcessUpdate(VRTK_ControllerReference controllerReference,
            Dictionary<string, object> options)
        {
            foreach (var kvp in pressedLastState)
            {
                kvp.Value[1] = kvp.Value[0];
            }

            // Update Hair Value
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                var index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                var device = GetTrackedObject(controllerReference.actual);
                if (device == null)
                {
                    return;
                }

                UpdateHairValues(GetButtonAxis(ButtonTypes.Trigger, controllerReference).x,
                    GetButtonHairlineDelta(ButtonTypes.Trigger, controllerReference),
                    ref previousHairTriggerState[index],
                    ref currentHairTriggerState[index],
                    ref hairTriggerLimit[index]);
            }
        }

        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference,
            Dictionary<string, object> options)
        {
        }

        public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            return ControllerType.Oculus_OculusTouch;
        }

        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            return "ControllerColliders/Fallback";
        }

        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand,
            bool fullPath = false)
        {
            return "";
        }

        public override uint GetControllerIndex(GameObject controller)
        {
            var trackedObject = GetTrackedObject(controller);
            return (trackedObject != null ? trackedObject.index : uint.MaxValue);
        }

        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            SetTrackedControllerCaches();
            var sdkManager = VRTK_SDKManager.instance;

            if (sdkManager == null) return null;

            if (cachedLeftController != null && cachedLeftController.index == index)
                return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);

            if (cachedRightController != null && cachedRightController.index == index)
                return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);

            return null;
        }

        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            var controller = GetSDKManagerControllerLeftHand(actual);

            if (controller == null && actual)
                controller =
                    VRTK_SharedMethods.FindEvenInactiveGameObject<Transform>("Camera Offset/LeftHand Controller", true);

            return controller;
        }

        public override GameObject GetControllerRightHand(bool actual = false)
        {
            var controller = GetSDKManagerControllerRightHand(actual);

            if (controller == null && actual)
                controller =
                    VRTK_SharedMethods.FindEvenInactiveGameObject<Transform>("Camera Offset/RightHand Controller",
                        true);

            return controller;
        }

        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        public override bool WaitForControllerModel(ControllerHand hand)
        {
            return false;
        }

        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        public override GameObject GetControllerModel(ControllerHand hand)
        {
            var model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                switch (hand)
                {
                    case ControllerHand.Left:
                        model = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        model = GetControllerRightHand(true);
                        break;
                }
            }

            return model;
        }

        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            return null;
        }

        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference)) return;

            var index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            var controller = GetControllerByIndex(index);

            if (IsControllerLeftHand(controller))
            {
                if (hapticsCapabilitiesLeft.supportsImpulse)
                {
                    LeftDevice.SendHapticImpulse(0, strength, 1);
                }
            }
            else if (IsControllerRightHand(controller))
            {
                if (hapticsCapabilitiesRight.supportsImpulse)
                {
                    RightDevice.SendHapticImpulse(0, strength, 1);
                }
            }
        }

        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            throw new System.NotImplementedException();
        }

        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            var modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.8f;
            modifiers.intervalModifier = 1f;
            return modifiers;
        }

        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                var index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                var controller = GetControllerByIndex(index);
                var device = IsControllerLeftHand(controller) ? LeftDevice : RightDevice;

                if (device.TryGetFeatureValue(CommonUsages.deviceVelocity, out var velocity))
                {
                    return velocity;
                }
            }

            return Vector3.zero;
        }

        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                var index = VRTK_ControllerReference.GetRealIndex(controllerReference);
                var controller = GetControllerByIndex(index);
                var device = IsControllerLeftHand(controller) ? LeftDevice : RightDevice;

                if (device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out var velocity))
                {
                    return velocity;
                }
            }

            return Vector3.zero;
        }

        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues,
            int compareFidelity)
        {
            return (VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference)) return Vector2.zero;

            var device = GetInputDeviceForReference(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    if (device.TryGetFeatureValue(CommonUsages.primary2DAxis, out var touchPad))
                    {
                        return touchPad;
                    }

                    break;
                case ButtonTypes.Trigger:
                    if (device.TryGetFeatureValue(CommonUsages.trigger, out var trigger))
                    {
                        return new Vector2(trigger, 0);
                    }

                    break;
                case ButtonTypes.Grip:
                    if (device.TryGetFeatureValue(CommonUsages.grip, out var grip))
                    {
                        return new Vector2(grip, 0);
                    }

                    break;
            }

            return Vector2.zero;
        }

        public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference)) return 0f;

            var senseResult = false;

            var device = GetInputDeviceForReference(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Touchpad:
                    if (device.TryGetFeatureValue(OculusUsages.thumbTouch, out var touchpad))
                    {
                        senseResult = touchpad;
                    }

                    break;

                case ButtonTypes.Trigger:
                    if (device.TryGetFeatureValue(OculusUsages.indexTouch, out var trigger))
                    {
                        senseResult = trigger;
                    }

                    break;
            }

            return senseResult ? 1f : 0f;
        }

        public override float GetButtonHairlineDelta(ButtonTypes buttonType,
            VRTK_ControllerReference controllerReference)
        {
            // TODO: Don't really know what to do here, check out SDK_OculusController.cs for more details
            return 0.1f;
        }

        protected virtual void UpdateHairValues(float axisValue, float hairDelta, ref bool previousState,
            ref bool currentState, ref float hairLimit)
        {
            previousState = currentState;
            if (currentState)
            {
                if (axisValue < (hairLimit - hairDelta) || axisValue <= 0f)
                {
                    currentState = false;
                }
            }
            else
            {
                if (axisValue > (hairLimit + hairDelta) || axisValue >= 1f)
                {
                    currentState = true;
                }
            }

            hairLimit = (currentState ? Mathf.Max(hairLimit, axisValue) : Mathf.Min(hairLimit, axisValue));
        }


        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType,
            VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference)) return false;

            var device = GetInputDeviceForReference(controllerReference);

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(device, pressType, CommonUsages.trigger);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(device, pressType, OculusUsages.indexTouch);
                    }

                    break;
                case ButtonTypes.Grip:
                    return IsButtonPressed(device, pressType, CommonUsages.gripButton);
                case ButtonTypes.Touchpad:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(device, pressType, CommonUsages.primary2DAxisClick);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(device, pressType, CommonUsages.primary2DAxisTouch);
                    }

                    break;
                case ButtonTypes.ButtonOne:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(device, pressType, CommonUsages.primaryButton);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(device, pressType, CommonUsages.primaryTouch);
                    }

                    break;
                case ButtonTypes.ButtonTwo:
                    switch (pressType)
                    {
                        case ButtonPressTypes.Press:
                        case ButtonPressTypes.PressDown:
                        case ButtonPressTypes.PressUp:
                            return IsButtonPressed(device, pressType, CommonUsages.secondaryButton);
                        case ButtonPressTypes.Touch:
                        case ButtonPressTypes.TouchDown:
                        case ButtonPressTypes.TouchUp:
                            return IsButtonPressed(device, pressType, CommonUsages.secondaryTouch);
                    }

                    break;
            }

            return false;
        }

        public virtual bool IsButtonPressed(InputDevice device, ButtonPressTypes pressType,
            InputFeatureUsage<bool> usage, string customKey = "")
        {
            if (!device.isValid) return false;

            // Getting the value of the input feature.
            var value = device.TryGetFeatureValue(usage, out var usageResult) && usageResult;
            var key = GetButtonDictionaryKey(usage, device, customKey);
            // Create dictionary entry if it doesn't exists
            if (!pressedLastState.ContainsKey(key)) pressedLastState.Add(key, new[] {false, false});

            bool result = false;

            switch (pressType)
            {
                case ButtonPressTypes.TouchDown:
                case ButtonPressTypes.PressDown:
                    result = !pressedLastState[key][1] && value;
                    pressedLastState[key][0] = value;
                    break;
                case ButtonPressTypes.TouchUp:
                case ButtonPressTypes.PressUp:
                    result = pressedLastState[key][1] && !value;
                    break;
                case ButtonPressTypes.Touch:
                case ButtonPressTypes.Press:
                    result = pressedLastState[key][1] && value;
                    pressedLastState[key][0] = value;
                    break;
            }

            return result;
        }
        
        public virtual string GetButtonDictionaryKey(InputFeatureUsage<bool> usage, InputDevice device,
            string customKey = "")
        {
            return $"{device.name}{(customKey != string.Empty ? "_" + customKey : "")}_{usage.name}";
        }

        public virtual string GetButtonDictionaryKey(InputFeatureUsage<float> usage, InputDevice device,
            string customKey = "")
        {
            return $"{device.name}{(customKey != string.Empty ? "_" + customKey : "")}_{usage.name}";
        }

        public virtual bool IsButtonPressed(InputDevice device, ButtonPressTypes pressType,
            InputFeatureUsage<float> usage, string customKey = "")
        {
            if (!device.isValid) return false;
            var value = GetButtonPress(device, usage);
            var key = GetButtonDictionaryKey(usage, device, customKey);

            if (!pressedLastState.ContainsKey(key)) pressedLastState.Add(key, new[] {false, false});

            bool result = false;

            switch (pressType)
            {
                case ButtonPressTypes.TouchDown:
                case ButtonPressTypes.PressDown:
                    result = !pressedLastState[key][1] && value > 0;
                    pressedLastState[key][0] = value > 0;
                    break;
                case ButtonPressTypes.TouchUp:
                case ButtonPressTypes.PressUp:
                    result = pressedLastState[key][1] && value == 0;
                    break;
                case ButtonPressTypes.Touch:
                case ButtonPressTypes.Press:
                    result = pressedLastState[key][1] && value > 0;
                    pressedLastState[key][0] = value > 0;
                    break;
            }

            return result;
        }

        public virtual float GetButtonPress(InputDevice device, InputFeatureUsage<float> usage)
        {
            if (!device.isValid) return 0f;

            return device.TryGetFeatureValue(usage, out var value) ? value : 0f;
        }

        protected virtual InputDevice GetInputDeviceForReference(VRTK_ControllerReference controllerReference)
        {
            var index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            var controller = GetControllerByIndex(index);

            return IsControllerLeftHand(controller) ? LeftDevice : RightDevice;
        }

        protected virtual VRTK_TrackedController GetTrackedObject(GameObject controller)
        {
            SetTrackedControllerCaches();
            VRTK_TrackedController trackedObject = null;

            if (IsControllerLeftHand(controller))
            {
                trackedObject = cachedLeftController;
            }
            else if (IsControllerRightHand(controller))
            {
                trackedObject = cachedRightController;
            }

            return trackedObject;
        }

        protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                cachedLeftController = null;
                cachedRightController = null;
            }

            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager == null) return;
            if (cachedLeftController == null && sdkManager.loadedSetup.actualLeftController)
            {
                cachedLeftController =
                    sdkManager.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
                if (cachedLeftController != null)
                {
                    cachedLeftController.index = 0;
                    cachedLeftVelocityEstimator = (cachedLeftController.GetComponent<VRTK_VelocityEstimator>() != null
                        ? cachedLeftController.GetComponent<VRTK_VelocityEstimator>()
                        : cachedLeftController.gameObject.AddComponent<VRTK_VelocityEstimator>());
                }
            }

            if (cachedRightController == null && sdkManager.loadedSetup.actualRightController)
            {
                cachedRightController =
                    sdkManager.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();

                if (cachedRightController == null) return;

                cachedRightController.index = 1;
                cachedRightVelocityEstimator = (cachedRightController.GetComponent<VRTK_VelocityEstimator>() != null
                    ? cachedRightController.GetComponent<VRTK_VelocityEstimator>()
                    : cachedRightController.gameObject.AddComponent<VRTK_VelocityEstimator>());
            }
        }
    }
}