using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace S8l.FirstPersonController.Runtime
{
    public abstract class FpcInputStrategy
    {
        public float StillTabThreshold = 100f;
        public bool DoubleClickToMove;
        private float m_dragDistance;

        public const float Threshold = 100.0f;
        public const float ClickThresholdDistance = 10.0f;
        public const float DoubleClickThresholdTime = 0.3f;

        public bool IgnoreInput { get; set; }
        
        // the following values need to reset on each run and not serialized
        [HideInInspector]
        [System.NonSerialized]
        public bool Locked = false;
        [System.NonSerialized]
        protected Vector2 m_moveStart;
        [System.NonSerialized]
        protected bool m_moving = false;
        [System.NonSerialized]
        protected float m_lastClickTime;

        private FirstPersonController fPC;

        public virtual void InputStart(FirstPersonController thisFirstPersonController)
        {
            fPC = thisFirstPersonController;
            OnTap += HandleTap;
            OnDoubleTap += HandleDoubleTap;
            OnDrag += HandleDrag;
            OnStartDrag += HandleStartDrag;
            OnStopDrag += HandleStopDrag;
        }

        private void OnDestroy()
        {
            fPC = null;
            OnTap -= HandleTap;
            OnDoubleTap -= HandleDoubleTap;
            OnDrag -= HandleDrag;
            OnStartDrag -= HandleStartDrag;
            OnStopDrag -= HandleStopDrag;
        }

        public abstract bool InputUpdate();


        public delegate void TapBeginHandler(Vector2 position);

        public delegate void TapHandler(Vector2 position);

        public delegate void StartDragHandler(Vector2 startPos);

        public delegate void DragHandler(Vector2 newPos, Vector2 delta);

        public delegate void StopDragHandler(Vector2 startPos, Vector2 endPos, float duration);

        public event TapHandler OnTapBegin;
        public event TapHandler OnTap;
        public event TapHandler OnDoubleTap;
        public event StartDragHandler OnStartDrag;
        public event DragHandler OnDrag;
        public event StopDragHandler OnStopDrag;


        /// <summary>
        /// calls the events from a child class, which is not possible directly because of some C# reason
        /// </summary>
        /// <param name="position"></param>
        protected void CallOnTapBegin(Vector2 position)
        {
            OnTapBegin?.Invoke(position);
        }

        protected void CallOnTap(Vector2 position)
        {
            OnTap?.Invoke(position);
        }

        protected void CallOnDoubleTap(Vector2 position)
        {
            OnDoubleTap?.Invoke(position);
        }

        protected void CallOnStopDrag(Vector2 startPos, Vector2 endPos, float duration)
        {
            OnStopDrag?.Invoke(startPos, endPos, duration);
        }

        protected void CallOnStartDrag(Vector2 position)
        {
            OnStartDrag?.Invoke(position);
        }

        protected void CallOnDrag(Vector2 newPos, Vector2 delta)
        {
            OnDrag?.Invoke(newPos, delta);
        }


        /// <summary>
        /// First tap means moving, next tap means stop, etc.
        /// </summary>
        /// <param name="pos">Where did the user tap?</param>
        public void HandleTap(Vector2 pos)
        {
            if (DoubleClickToMove)
            {
                return;
            }

            //Don't do anything if we're over a UI object
            if ((EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) || Locked)
            {
                return;
            }

            GameObject clickedObject;
            if (GetGameObjectAtMouse(out clickedObject, null, pos))
            {
                return;
            }

            ManageTapping(pos);
        }

        private void HandleDoubleTap(Vector2 pos)
        {
            if (!DoubleClickToMove)
            {
                return;
            }

            //Don't do anything if we're over a UI object
            if ((EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) || Locked)
            {
                return;
            }

            GameObject clickedObject;
            if (GetGameObjectAtMouse(out clickedObject, null, pos))
            {
                return;
            }

            ManageTapping(pos);
        }

        /// <summary>
        /// Manages taps
        /// </summary>
        /// <param name="pos">tap position</param>
        private void ManageTapping(Vector2 pos)
        {
            if ((fPC.PlayerState == PlayerState.Talking) || IgnoreInput)
            {
                return;
            }

            // see which point / object was tapped

            fPC.RaycastStrategy.Raycast(new Vector2(pos.x, pos.y));
        }


        public void HandleDrag(Vector2 pos, Vector2 delta)
        {
            //Don't do anything if we're over a UI object
            if ((EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) || Locked)
            {
                return;
            }

            if (IgnoreInput || fPC.PlayerState == PlayerState.Talking)
            {
                return;
            }

            m_dragDistance += delta.magnitude;

            if (m_dragDistance < (fPC.Config.DragThreshold / 1024.0f * Screen.width))
            {
                return;
            }

            // rotation must be relative to screen resolution and the camera's field of view
            // otherwise rotation speed would be heavily device-dependent
            var dx = delta.x / fPC.Cam.pixelWidth * fPC.Cam.fieldOfView * fPC.Cam.aspect;
            var dy = delta.y / fPC.Cam.pixelHeight * fPC.Cam.fieldOfView;

            if (fPC.PlayerState != PlayerState.Talking)
            {
                fPC.CameraX.transform.Rotate(Vector3.up, -dx * fPC.Config.RotationSpeed);

                // Make the player look between min and max up and down value.
                if ((dy < 0 && fPC.YViewRotation > -fPC.Config.YViewMaxDrag) ||
                    (dy > 0 && fPC.YViewRotation < fPC.Config.YViewMaxDrag))
                {
                    // extra check so that dy + fPC.m_yViewRotation does not exceed m_yViewMaxDrag
                    if (dy < 0 && fPC.YViewRotation + dy < -fPC.Config.YViewMaxDrag)
                    {
                        dy = -fPC.Config.YViewMaxDrag - fPC.YViewRotation;
                    }
                    else if (dy > 0 && fPC.YViewRotation + dy > fPC.Config.YViewMaxDrag)
                    {
                        dy = fPC.Config.YViewMaxDrag - fPC.YViewRotation;
                    }
                    
                    fPC.CameraY.transform.Rotate(Vector3.right, dy * fPC.Config.RotationSpeed);
                    fPC.YViewRotation += dy;
                }
            }
        }


        private void HandleStartDrag(Vector2 pos)
        {
            m_dragDistance = 0;
            // while the player is manually looking around, override the agent's rotation control
            fPC.NavMeshAgent.updateRotation = false;
        }

        private void HandleStopDrag(Vector2 startPos, Vector2 endPos, float duration)
        {
            if (m_dragDistance < StillTabThreshold / 1000f * Screen.width)
            {
                HandleTap(startPos);
            }

            // pass rotation control back to the nav mesh agent
            if (fPC.PlayerState == PlayerState.Idle)
            {
                fPC.NavMeshAgent.updateRotation = true;
            }
        }


        /// <summary>
        /// Can probably be removed
        /// </summary>
        public bool GetGameObjectAtMouse(out GameObject hitObject, Camera usedCamera, Vector2 pos)
        {
            hitObject = null;

            if (EventSystem.current != null)
            {
                // test for uGUI elements
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = pos;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);

                string all = "";
                foreach (RaycastResult h in hits)
                {
                    all += "," + h.gameObject.name;
                }
                //Debug.LogWarning("ray hit: " + all);

                if (hits.Count > 0)
                {
                    hitObject = hits[0].gameObject;
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool IsDrag(Vector2 to, Vector2 from)
        {
            return Vector2.Distance(to, from) > ClickThresholdDistance;
        }

        public void DestroyFpcRef()
        {
            fPC = null;
        }
    }
}