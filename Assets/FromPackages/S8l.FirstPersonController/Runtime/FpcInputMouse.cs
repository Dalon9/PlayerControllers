using UnityEngine;

namespace S8l.FirstPersonController.Runtime
{

    public class FpcInputMouse : FpcInputStrategy
    {
        private bool m_mouseDown = false;

         
        public override void InputStart(FirstPersonController thisFirstPersonController)
        {
            base.InputStart(thisFirstPersonController);
        }


        public override bool InputUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                if (!m_mouseDown)
                {
                    m_mouseDown = true;
                    m_moveStart = Input.mousePosition;
                   CallOnTapBegin(Input.mousePosition);
                }
                else
                {
                    if (IsDrag(Input.mousePosition, m_moveStart) && !m_moving)
                    {
                        m_moving = true;
                        m_moveStart = Input.mousePosition;
                        CallOnStartDrag(m_moveStart);
                    }
                    else if (m_moving)
                    {
                        CallOnDrag(Input.mousePosition, (Vector2)Input.mousePosition - m_moveStart);
                        m_moveStart = Input.mousePosition;
                    }
                }
            }
            else
            {
                if (m_mouseDown)
                {
                    m_mouseDown = false;
                    //if (m_moving || IsDrag(Input.mousePosition, m_moveStart))
                    if (m_moving)
                    {
                        CallOnStopDrag(m_moveStart, Input.mousePosition, 0.0f);
                    }
                    else
                    {
                        if (Time.time - m_lastClickTime < DoubleClickThresholdTime)
                        {
                            CallOnDoubleTap(Input.mousePosition);
                        }
                        else
                        {
                            CallOnTap(Input.mousePosition);
                        }
                    }

                    m_moving = false;
                    m_lastClickTime = Time.time;
                }
            }
            return true;
        }
    }
}