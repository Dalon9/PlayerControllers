using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using S8l.GuidePath.Runtime;

namespace S8l.GuidePathVisualizerShapes.Runtime
{
    [System.Serializable]
    public class PathVisualizerShapesPolyLine : IPathVisualizer
    {
        public GameObject PolyLinePrefab;

        private Polyline _line;

        public void Visualize(Vector3[] path, GuidePath.Runtime.GuidePath guidePath)
        {
            if (_line == null)
                CreatePolyLine();

            _line.gameObject.SetActive(true);
            _line.SetPoints(path);
        }

        private void CreatePolyLine()
        {
            var obj = GameObject.Instantiate(PolyLinePrefab, Vector3.zero, Quaternion.identity);
            _line = obj.GetComponent<Polyline>();
        }

        public void Hide()
        {
            _line.gameObject.SetActive(false);
        }
    }
}