using S8l.FirstPersonController.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace S8l.FirstPersonControllerInputSystem.Runtime
{
    public class FpcInputMouseIS : FpcInputStrategy
    {
        private bool m_mouseDown = false;
        private Vector2 m_mousePos;

        public override void InputStart(FirstPersonController.Runtime.FirstPersonController thisFirstPersonController)
        {
            base.InputStart(thisFirstPersonController);
        }

        public override bool InputUpdate()
        {
            m_mousePos = Mouse.current.position.ReadValue();
            if (Mouse.current.leftButton.isPressed)
            {
                if (!m_mouseDown)
                {
                    m_mouseDown = true;
                    m_moveStart = m_mousePos;
                    CallOnTapBegin(m_mousePos);
                }
                else
                {
                    if (IsDrag(m_mousePos, m_moveStart) && !m_moving)
                    {
                        m_moving = true;
                        m_moveStart = m_mousePos;
                        CallOnStartDrag(m_moveStart);
                    }
                    else if (m_moving)
                    {
                        CallOnDrag(m_mousePos, m_mousePos - m_moveStart);
                        m_moveStart = m_mousePos;
                    }
                }
            }
            else
            {
                if (m_mouseDown)
                {
                    m_mouseDown = false;
                    if (m_moving)
                    {
                        CallOnStopDrag(m_moveStart, m_mousePos, 0.0f);
                    }
                    else
                    {
                        if (Time.time - m_lastClickTime < DoubleClickThresholdTime)
                        {
                            CallOnDoubleTap(m_mousePos);
                        }
                        else
                        {
                            CallOnTap(m_mousePos);
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