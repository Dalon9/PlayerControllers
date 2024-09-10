using System;
using UnityEngine;
namespace S8l.FirstPersonController.Runtime
{
    [Serializable]
    public class FpcConfig : IFpcConfig
    {
        [field: SerializeField] public float RotationSpeed { get; set; } = 1.1f;

        [field: SerializeField] public float WalkSpeed { get; set; } = 3f;

        [field: SerializeField] public float DragThreshold { get; set; } = 100f;

        [field: SerializeField] public float YViewMaxDrag { get; set; } = 50f;
    }
}
