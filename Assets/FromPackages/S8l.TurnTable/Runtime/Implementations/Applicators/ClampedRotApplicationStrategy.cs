using S8l.TurnTable.Runtime.Interfaces;
using UnityEngine;

namespace S8l.TurnTable.Runtime.Implementations.Applicators
{
    public class ClampedRotApplicationStrategy : IRotApplicationStrategy
    {
        [Tooltip("Max deviation angle from horizon in each direction")]
        public float YClamp = 45.0f;
        
        private ITurnTable _holder;
        public void Init(ITurnTable parent)
        {
            _holder = parent;
        }

        public void ApplyRotation(Quaternion delta)
        {
            Vector3 newAngle = _holder.Target.localEulerAngles;
            newAngle += delta.eulerAngles;
            _holder.Target.localEulerAngles = ClampLocalRotY(newAngle);
        }
        
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
    }
}