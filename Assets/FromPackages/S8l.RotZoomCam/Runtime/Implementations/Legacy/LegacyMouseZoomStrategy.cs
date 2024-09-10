using S8l.RotZoomCam.Runtime.Interfaces;
using UnityEngine;

namespace S8l.RotZoomCam.Runtime.Implementations.Legacy
{
    public class LegacyMouseZoomStrategy : IZoomStrategy
    {
        public float ZoomSpeed = 1f;
        public float MinFOV = 40;
        public float MaxFOV = 80;
        private IRotZoomCam _holder;
        
        public void Init(IRotZoomCam parent)
        {
            _holder = parent;
            _holder.OnUpdate += this.OnUpdate;
        }

        public void OnUpdate()
        {
            float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * 2000f * Time.deltaTime;
            _holder.CameraReference.fieldOfView = Mathf.Clamp(_holder.CameraReference.fieldOfView + wheelDelta, MinFOV, MaxFOV);
        }
    }
}