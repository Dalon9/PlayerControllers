using S8l.RotZoomCam.Runtime.Interfaces;
using UnityEngine;

namespace S8l.RotZoomCam.Runtime.Implementations.Legacy
{
    public class LegacyTouchZoomStrategy : IZoomStrategy
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
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * ZoomSpeed * 5f * Time.deltaTime;
                
                _holder.CameraReference.fieldOfView = Mathf.Clamp(_holder.CameraReference.fieldOfView + deltaMagnitudeDiff, MinFOV, MaxFOV);
            }
        }
    }


}