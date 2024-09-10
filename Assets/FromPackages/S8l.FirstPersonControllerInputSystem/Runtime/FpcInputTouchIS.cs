using S8l.FirstPersonController.Runtime;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace S8l.FirstPersonControllerInputSystem.Runtime
{
    public class FpcInputTouchIS : FpcInputStrategy
    {
        private bool m_fingerDown = false;
        private bool m_touchMoveStart = false;

        public override void InputStart(FirstPersonController.Runtime.FirstPersonController thisFirstPersonController)
        {
            EnhancedTouchSupport.Enable();
            base.InputStart(thisFirstPersonController);
        }
        public override bool InputUpdate()
        {
            // Return if no touch occurred       
            if (Touch.activeTouches.Count <= 0)
            {
                if (m_fingerDown)
                {
                    m_fingerDown = false;
                    m_moving = false;
                    UnityEngine.Debug.Log("no more finger in use");
                }

                return false;
            }

            var firstTouch = Touch.activeTouches[0];

            switch (firstTouch.phase)
            {
                case TouchPhase.Began:
                    m_fingerDown = true;
                    m_moving = false;
                    m_moveStart = firstTouch.screenPosition;
                    CallOnTapBegin(firstTouch.screenPosition);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (m_moving)
                    {
                        CallOnStopDrag(m_moveStart, firstTouch.screenPosition, 0.0f);
                    }
                    else
                    {
                        if (firstTouch.tapCount > 1)
                        {
                            CallOnDoubleTap(firstTouch.screenPosition);
                        }
                        else
                        {
                            CallOnTap(firstTouch.screenPosition);
                        }
                    }
                    break;
                case TouchPhase.Stationary:
                    // nothing
                    break;
                case TouchPhase.Moved:
                    if (IsDrag(firstTouch.screenPosition, m_moveStart))
                    {
                        if (!m_moving)
                        {
                            m_moving = true;
                            CallOnStartDrag(m_moveStart);
                        }
                        else
                        {
                            CallOnDrag(firstTouch.screenPosition, firstTouch.screenPosition - m_moveStart);
                            m_moveStart = firstTouch.screenPosition;
                        }
                    }
                    break;
            }
            return true;
        }
    }
}