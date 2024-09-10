using System.Collections.Generic;
using S8l.CustomPlatformDetector.Runtime;
using S8l.CustomPlatformDetector.Runtime.Platforms;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using S8l.TurnTable.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace S8l.TurnTable.Runtime.Implementations.Legacy
{
    public class LegacyTouchZoomRetrievalStrategy : IZoomRetrievalStrategy
    {
        [Tooltip("Execute this strategy on these platforms")]
        [SerializeReference]
        public List<IPlatform> ExecuteOn = new List<IPlatform>() {new IPhonePlayer(), new AndroidNonVR()};
        [Tooltip("But never execute on these platforms")]
        [SerializeReference]
        public List<IPlatform> ButNotOn = new List<IPlatform>();
        public float ZoomSpeed = 1f;
        [Tooltip("Zoom even when the cursor is over an UI element")]
        public bool IgnoreUI = false;
        private ITurnTable _holder;
        
        public void Init(ITurnTable parent)
        {
            _holder = parent;
            _holder.OnZoomUpdate += this.OnUpdate;
        }

        public float OnUpdate()
        {
            
            #if ENABLE_LEGACY_INPUT_MANAGER
            
            var detectedPlatforms = CustomPlatformDetectorFacade.DetectPlatforms();
            
            if(CustomPlatformUtils.HasIntersection(ButNotOn, detectedPlatforms))
                return 0f;
            
            if (!CustomPlatformUtils.HasIntersection(ExecuteOn, detectedPlatforms))
                return 0f;

            if (Input.touchCount == 2 && (IgnoreUI == true || IsMouseOverGUI() == false))
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
                
                return deltaMagnitudeDiff;
            }
            
            #endif

            return 0f;
        }
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
    }


}