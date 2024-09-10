using System.Collections.Generic;
using UnityEngine;

namespace S8l.TurnTable.Runtime
{
    /// <summary>
    /// ATTENTION using this helper allows for easy camera space Y orbit rotation, but this circumvents the Y clamping and
    /// may result in - for the user - strange looking angles. If you want a similar effect with Y clamping and moving the
    /// camera - instead of the object - is an option, the orbit camera package might be what you are looking for...  
    /// </summary>
    public class DetachedPivot : MonoBehaviour
    {
        public Transform Face;
        private List<Transform> _children = new List<Transform>();

        void Awake()
        {
            // Assumes direct children dont change after awake
            for (int i = 0; i < this.transform.childCount; i++)
            {
                _children.Add(this.transform.GetChild(i));
            }
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.DetachChildren();
            if (Face != null)
                this.transform.LookAt(Face);
            foreach (var child in _children)
            {
                child.SetParent(this.transform);
            }
        }
    }
}
