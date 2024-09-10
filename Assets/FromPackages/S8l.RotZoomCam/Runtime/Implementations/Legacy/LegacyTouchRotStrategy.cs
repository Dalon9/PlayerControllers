using S8l.RotZoomCam.Runtime.Interfaces;
using UnityEngine;

namespace S8l.RotZoomCam.Runtime.Implementations.Legacy
{
    public class LegacyTouchRotStrategy : IRotStrategy
    {
        [Tooltip("Negative speed inverts axis")]
        public float XSpeed = 120.0f;
        [Tooltip("Negative speed inverts axis")]
        public float YSpeed = 60.0f;
        [Tooltip("Max deviation angle from horizon in each direction")]
        public float YClamp = 45.0f;
        [Tooltip("Above this touch/finger count dont rotate")]
        public int MaxTouches = 1;
        [SerializeField]
        [Tooltip("Number of frames for the buffer")]
        private int BufferSize = 5;
        
        private float[] _xBuffer;
        private float[] _yBuffer;
        private int _xPointer = 0;
        private int _yPointer = 0;
        
        private IRotZoomCam _holder;

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

            if (Input.touchCount > 0 && Input.touchCount <= MaxTouches)
            {
                x += Input.touches[0].deltaPosition.x * XSpeed * 0.1f * Time.deltaTime;
                y -= Input.touches[0].deltaPosition.y * YSpeed * 0.1f * Time.deltaTime;
                
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