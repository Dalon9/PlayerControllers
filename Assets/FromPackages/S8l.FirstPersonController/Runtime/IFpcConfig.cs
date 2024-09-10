using UnityEngine;

namespace S8l.FirstPersonController.Runtime
{
    public interface IFpcConfig
    {
        float RotationSpeed { get; set; }
        float WalkSpeed { get; set; }
        float DragThreshold { get; set; }
        float YViewMaxDrag { get; set; }
    }
}