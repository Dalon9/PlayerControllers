using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.Compass.Runtime
{
    public class Compass3D : MonoBehaviour
    {
        public Transform Arrow;
        public bool ignoreY;

        [SerializeField]
        private Transform target;

        void Update()
        {
            if (target != null)
            {
                Arrow.LookAt(target);
                if (ignoreY)
                {
                    Arrow.rotation = Quaternion.Euler(Vector3.Scale(Arrow.rotation.eulerAngles, Vector3.up));
                }
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}