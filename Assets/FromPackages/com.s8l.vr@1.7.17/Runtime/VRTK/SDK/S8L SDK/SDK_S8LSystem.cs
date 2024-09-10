using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using VRTK;

namespace VRTK
{
    [SDK_Description("Pico Neo 2 (Android:XRPicoNeo)", "", "PicoXR Display", "Android")]
    [SDK_Description("Oculus Quest (Android:XROculusQuest)", "", "oculus display", "Android", 1)]
    [SDK_Description("Vive Focus Plus(Android:ViveFocusPlus)", "", "WVR Display Provider", "Android", 2)]
    [SDK_Description("OpenXR(Standalone:OpenXR)", "", "OpenXR Display", "Standalone", 3)]

    public class SDK_S8LSystem : SDK_BaseSystem
    {
        public override bool IsDisplayOnDesktop()
        {
            return false;
        }

        public override bool ShouldAppRenderWithLowResources()
        {
            return false;
        }

        public override void ForceInterleavedReprojectionOn(bool force)
        {
            
        }

        public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {
            List<XRInputSubsystem> lst = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(lst);
            for (int i = 0; i < lst.Count; i++)
            {
                lst[i].TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            }
        }
    }
}