using System;
using UnityEngine;

namespace S8l.RotZoomCam.Runtime.Interfaces
{
    public interface IRotZoomCam
    {
        Action OnUpdate { get; set;  }
        Camera CameraReference { get; }
    }
}