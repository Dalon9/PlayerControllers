using System;
using UnityEngine;

namespace S8l.TurnTable.Runtime.Interfaces
{
    public interface ITurnTable
    {
        Func<Quaternion> OnRotUpdate { get; set;  }
        Func<float> OnZoomUpdate { get; set;  }
        Camera CameraReference { get; }
        Transform Target { get; }
    }
}