using System;
using S8l.Guardian.Runtime.Interfaces;
using UnityEditor;
using UnityEngine;

namespace S8l.Guardian.Runtime.Shapes
{
    public class GuardianDonutShapeProvider : MonoBehaviour, IGuardianShapeProvider
    {
        public float outerRadius = 5;
        public float innerRadius = 1;
        public float outerFadeDistance = .5f;
        public float innerFadeDistance = .1f;

        public float SamplePosition(Vector3 position)
        {
            var hardOuterBound = outerRadius + outerFadeDistance;
            var hardInnerBound = innerRadius - innerFadeDistance;
            
            var distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(position.x, 0, position.z));
            
            // Player is in red area, outside of bounds
            if (distance <= hardInnerBound || distance >= hardOuterBound)
            {
                return 1;
            }
            
            // Player is in green area, inside the bounds
            if (distance <= outerRadius && distance >= innerRadius)
            {
                return 0;
            }

            // If outside of outer radius
            if (distance > outerRadius)
            {
                var outerRatio = hardOuterBound - outerRadius - (distance - outerRadius);
                return 1 - Mathf.Clamp01(outerRatio / (hardOuterBound - outerRadius));
            }

            // If inside of inner radius
            if (distance < innerRadius)
            {
                var distanceFromHardBound = distance - hardInnerBound;
                var innerBoundSize = innerRadius - (innerRadius - innerFadeDistance);
                var ratio = innerBoundSize - distanceFromHardBound;
                return Mathf.Clamp01(ratio / innerBoundSize);
            }

            // Return out of bounds if not in any of the conditions above, shouldn't happen.
            return 1;

        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = new Color(0, .8f, .1f, .1f);
            Handles.DrawSolidDisc(transform.position, transform.up, outerRadius);
            Handles.color = new Color(.7f, .8f, .1f, .1f);
            Handles.DrawSolidDisc(transform.position, transform.up, innerRadius);
            Handles.DrawSolidDisc(transform.position, transform.up, outerRadius + outerFadeDistance);
            Handles.color = new Color(.8f, .1f, .1f, .1f);
            Handles.DrawSolidDisc(transform.position, transform.up, innerRadius - innerFadeDistance);
        }
#endif
    }
}