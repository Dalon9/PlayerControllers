using S8l.RotZoomCam.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace S8l.RotZoomCam.Runtime.Implementations.Legacy
{
    public class LegacyMouseRotStrategy : IRotStrategy
    {

        public MouseButton RotMouseButton = MouseButton.LeftMouse;
        [Tooltip("Negative speed inverts axis")]
        public float XSpeed = 120.0f;
        [Tooltip("Negative speed inverts axis")]
        public float YSpeed = 60.0f;
        [Tooltip("Max deviation angle from horizon in each direction")]
        public float YClamp = 45.0f;
        [Tooltip("Scale the mouse axis in regards to the arrow keys")]
        public float MouseSpeedFactor = 0.25f;
        [Tooltip("Invert mouse only but keep arrow keys (unlike negative speed)")]
        public bool InvertMouse = false;
        [Tooltip("Use Arrow and WASD Input axis")]
        public bool AllowKeys = true;
        [SerializeField]
        [Tooltip("Number of frames for the buffer")]
        private int BufferSize = 5;
        
        private IRotZoomCam _holder;


        private float[] _xBuffer;
        private float[] _yBuffer;
        private int _xPointer = 0;
        private int _yPointer = 0;

        public void Init(IRotZoomCam parent)
        {
            _holder = parent;
            _holder.OnUpdate += this.OnUpdate;
            
            _xBuffer = new float[BufferSize];
            _yBuffer = new float[BufferSize];
        }

        public void OnUpdate()
        {

            
            float x = 0.0f;
            float y = 0.0f;

            if (Input.GetMouseButton((int)RotMouseButton))
            {
                var invert = InvertMouse ? -1f : 1f;
                x += (Input.GetAxis("Mouse X") * MouseSpeedFactor * invert + (AllowKeys ? Input.GetAxis("Horizontal") : 0f)) * XSpeed * Time.deltaTime;
                y -= (Input.GetAxis("Mouse Y") * MouseSpeedFactor * invert + (AllowKeys ? Input.GetAxis("Vertical") : 0f)) * YSpeed * Time.deltaTime;

                _xBuffer[_xPointer] = x;
                _yBuffer[_yPointer] = y;


                var newAngle = _holder.CameraReference.transform.localEulerAngles;
                newAngle += new Vector3(Average(_yBuffer), Average(_xBuffer), 0f);
                _holder.CameraReference.transform.localEulerAngles = ClampLocalRotY(newAngle);;


                _xPointer = (_xPointer + 1) % _xBuffer.Length;
                _yPointer = (_yPointer + 1) % _yBuffer.Length;
            }



        }

        
        #region Helpers
        private Vector3 ClampLocalRotY(Vector3 rot)
        {
            var newX = 0f;
            if (rot.x < YClamp || rot.x > 360f - YClamp)
            {
                newX = rot.x;
            }
            else
            {
                if (rot.x > 180f)
                    newX = 360f - YClamp;
                else
                    newX = YClamp;
            }

            return new Vector3(newX, rot.y, rot.z);
        }

        private float Average(float[] arr)
        {
            var res = 0f;
            for (int i = 0; i < arr.Length; i++)
            {
                res += arr[i];
            }
            res /= arr.Length;
            return res;
        }
        
        #endregion
    }
}