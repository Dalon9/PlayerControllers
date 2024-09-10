using UnityEngine;

namespace S8l.FirstPersonController.Runtime
{

    public class FpcInputTouch : FpcInputStrategy
    {
        //touch variables:


        private bool m_fingerDown = false;
        //private float m_fingerDistance = 5.0f;
        private bool m_touchMoveStart = false;


        public override void InputStart(FirstPersonController thisFirstPersonController)
        {
            base.InputStart(thisFirstPersonController);
            
        }
        public override bool InputUpdate()
        {
            // Return if no touch occurred
            if (Input.touchCount <= 0)
            {
                if (m_fingerDown)
                {
                    m_fingerDown = false;
                    m_moving = false;
                    UnityEngine.Debug.Log("no more finger in use");
                }

                return false;
            }

            var firstTouch = Input.GetTouch(0);

            switch (firstTouch.phase)
            {
                case TouchPhase.Began:
                    m_fingerDown = true;
                    m_moving = false;
                    m_moveStart = firstTouch.position;
                    CallOnTapBegin(firstTouch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (m_moving)
                    {
                        CallOnStopDrag(m_moveStart, firstTouch.position, 0.0f);
                    }
                    else
                    {
                        if (firstTouch.tapCount > 1)
                        {
                            CallOnDoubleTap(firstTouch.position);
                        }
                        else
                        {
                            CallOnTap(firstTouch.position);
                        }
                    }
                    break;
                case TouchPhase.Stationary:
                    // nothing
                    break;
                case TouchPhase.Moved:
                    if (IsDrag(firstTouch.position, m_moveStart))
                    {
                        if (!m_moving)
                        {
                            m_moving = true;
                            CallOnStartDrag(m_moveStart);
                        }
                        else
                        {
                            CallOnDrag(firstTouch.position, firstTouch.position - m_moveStart);
                            m_moveStart = firstTouch.position;
                        }
                    }
                    break;
            }
            return true;
        }
    }
}