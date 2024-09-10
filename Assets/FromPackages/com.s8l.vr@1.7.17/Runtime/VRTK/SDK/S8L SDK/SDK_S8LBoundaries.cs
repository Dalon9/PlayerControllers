using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRTK
{
    [SDK_Description(typeof(SDK_S8LSystem))]
    [SDK_Description(typeof(SDK_S8LSystem), 1)]
    [SDK_Description(typeof(SDK_S8LSystem), 2)]
    [SDK_Description(typeof(SDK_S8LSystem), 3)]
    public class SDK_S8LBoundaries : SDK_BaseBoundaries
    {
        public override void InitBoundaries()
        {
        }

        public override Transform GetPlayArea()
        {
            cachedPlayArea = GetSDKManagerPlayArea();
            if (cachedPlayArea != null) return cachedPlayArea;

            var rig = VRTK_SharedMethods.FindEvenInactiveGameObject<Transform>("XR Rig",true);
            if (rig != null)
            {
                cachedPlayArea = rig.transform;
            }
            return cachedPlayArea;
        }

        public override Vector3[] GetPlayAreaVertices()
        {
            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances<XRInputSubsystem>(subsystems);
            if (subsystems.Count <= 0) return null;

            var boundaries = new List<Vector3>();
            return subsystems[0].TryGetBoundaryPoints(boundaries) ? boundaries.ToArray() : null;
        }

        public override float GetPlayAreaBorderThickness()
        {
            return 0.1f;
        }

        public override bool IsPlayAreaSizeCalibrated()
        {
            return true;
        }

        public override bool GetDrawAtRuntime()
        {
            return false;
        }

        public override void SetDrawAtRuntime(bool value)
        {
        }
    }
}