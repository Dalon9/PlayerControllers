using System;
using UnityEngine;

namespace S8l.FirstPersonController.Runtime
{
    public interface IFpcRaycastStrategy
    {
        string InteractableAtDistanceTag { get; set; }
        string FloorTag { get; set; }
        float MaxInteractionDistance { get; set; }
        
        FirstPersonController FPC { get; set; }

        int Layermask { get; set; }
        void RaycastStart(FirstPersonController thisFirstPersonController);
        
        /// <summary>
        /// when an input touch or click was made, this function needs the target position and sends events to the fPC on its own
        /// </summary>
        /// <param name="screenPos"></param>
        void Raycast(Vector2 screenPos);

        void DestroyFpcRef();
    }
}
