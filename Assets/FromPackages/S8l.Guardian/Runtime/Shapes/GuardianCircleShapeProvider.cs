using S8l.Guardian.Runtime.Interfaces;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace S8l.Guardian.Runtime.Shapes
{
    public class GuardianCircleShapeProvider : MonoBehaviour, IGuardianShapeProvider
    {
        public float softBoundRadius = 5;
        public float hardBoundRadius = 7;

        public float SamplePosition(Vector3 position)
        {
            var distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(position.x, 0, position.z));

            // ratio = bounds size - distance from the soft bound
            var ratio = (hardBoundRadius - softBoundRadius) - (distance - softBoundRadius);
            return 1 - Mathf.Clamp01(ratio / (hardBoundRadius - softBoundRadius));
        }
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = new Color(0, .8f, .1f, .1f);
            Handles.DrawSolidDisc(transform.position, transform.up, softBoundRadius);
            Handles.color = new Color(.7f, .8f, .1f, .1f);
            Handles.DrawSolidDisc(transform.position, transform.up, hardBoundRadius);
        }
        #endif
    }
}