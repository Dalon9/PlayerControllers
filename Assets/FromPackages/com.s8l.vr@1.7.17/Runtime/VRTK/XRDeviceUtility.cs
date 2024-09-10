using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public static class XRDeviceUtility
{
    public static bool IsPresent()
    {
        var displaySubs = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(displaySubs);
        return displaySubs.Any(xrDisplay => xrDisplay.running);
    }

    public static void StartDisplaySubsystems()
    {
        var subs = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(subs);
        foreach (var s in subs)
        {
            Debug.Log("[XRDeviceUtility] Starting " + s.subsystemDescriptor.id);
            s.Start();
        }
    }
    
    public static void StopDisplaySubsystems()
    {
        var subs = new List<XRDisplaySubsystem>();
        SubsystemManager.GetInstances(subs);
        foreach (var s in subs)
        {
            Debug.Log("[XRDeviceUtility] Starting " + s.subsystemDescriptor.id);
            s.Stop();
        }
    }
}