using System.Collections.Generic;
using S8l.CustomPlatformDetector.Runtime;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using S8l.TurnTable.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

namespace S8l.TurnTable.Runtime.Implementations.Legacy
{
    public class LegacyMouseRotRetrievalStrategy : IRotRetrievalStrategy
    {
        [Tooltip("Execute this strategy on these platforms")]
        [SerializeReference]
        public List<IPlatform> ExecuteOn = new List<IPlatform>(){new StandaloneAny(), new EditorAny(), new WebGLPlayer()};
        [Tooltip("But never execute on these platforms")]
        [SerializeReference]
        public List<IPlatform> ButNotOn = new List<IPlatform>();
        
        
        public MouseButton RotMouseButton = MouseButton.LeftMouse;
        [Tooltip("Negative speed inverts axis")]
        public float XSpeed = 120.0f;
        [Tooltip("Negative speed inverts axis")]
        public float YSpeed = 60.0f;
        [Tooltip("Scale the mouse axis in regards to the arrow keys")]
        public float MouseSpeedFactor = 0.25f;
        [Tooltip("Invert mouse only but keep arrow keys (unlike negative speed)")]
        public bool InvertMouse = false;
        [Tooltip("Use Arrow and WASD Input axis")]
        public bool AllowKeys = true;
        [Tooltip("Rotate even when the cursor is over an UI element")]
        public bool IgnoreUI = false;
        [SerializeField]
        [Tooltip("Number of frames for the buffer")]
        private int BufferSize = 5;
        
        private ITurnTable _holder;


        private float[] _xBuffer;
        private float[] _yBuffer;
        private int _xPointer = 0;
        private int _yPointer = 0;

        public void Init(ITurnTable parent)
        {
            _holder = parent;
            _holder.OnRotUpdate += this.OnUpdate;
            
            _xBuffer = new float[BufferSize];
            _yBuffer = new float[BufferSize];
        }

        public Quaternion OnUpdate()
        {          
            float x = 0.0f;
            float y = 0.0f;

            #if ENABLE_LEGACY_INPUT_MANAGER
            
            var detectedPlatforms = CustomPlatformDetectorFacade.DetectPlatforms();
            
            if(CustomPlatformUtils.HasIntersection(ButNotOn, detectedPlatforms))
                return Quaternion.identity;
            
            if (!CustomPlatformUtils.HasIntersection(ExecuteOn, detectedPlatforms))
                return Quaternion.identity;
            
            
            if (Input.GetMouseButton((int)RotMouseButton) && (IgnoreUI == true || IsMouseOverGUI() == false))
            {
                var invert = InvertMouse ? -1f : 1f;
                x -= (Input.GetAxis("Mouse X") * MouseSpeedFactor * invert + (AllowKeys ? Input.GetAxis("Horizontal") : 0f)) * XSpeed * Time.deltaTime;
                y -= (Input.GetAxis("Mouse Y") * MouseSpeedFactor * invert + (AllowKeys ? Input.GetAxis("Vertical") : 0f)) * YSpeed * Time.deltaTime;

                _xBuffer[_xPointer] = x;
                _yBuffer[_yPointer] = y;

                _xPointer = (_xPointer + 1) % _xBuffer.Length;
                _yPointer = (_yPointer + 1) % _yBuffer.Length;

                return Quaternion.Euler(new Vector3(Average(_yBuffer), Average(_xBuffer), 0f));
            }
            #endif

            return Quaternion.identity;
        }

        
        #region Helpers

        private bool IsMouseOverGUI()
        {

            // If no eventsystem is there report no UI
            if (EventSystem.current == null)
                return false;
			
            //return GUIUtility.hotControl != 0;
            Vector2 clickPos = Input.mousePosition;
            
            PointerEventData PED = new PointerEventData(EventSystem.current);
            PED.position =  clickPos;
 
            List<RaycastResult> HITS = new List<RaycastResult>();
            EventSystem.current.RaycastAll( PED, HITS );

            return HITS.Count > 0;
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