using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using S8l.GuidePath.Runtime;

namespace S8l.GuidePathVisualizerShapes.Runtime
{

    [System.Serializable]
    public class PathVisualizerShapesLine : IPathVisualizer
    {
        public GameObject LinePrefab;

        private GameObject _lineParent;
        private List<Line> _lines;

        public void Visualize(Vector3[] path, GuidePath.Runtime.GuidePath guidePath)
        {
            if (_lineParent == null)
                CreateLine();

            SetLinePositions(path);
        }

        private void CreateLine()
        {
            _lineParent = new GameObject();
            _lines = new List<Line>();
        }

        private void SetLinePositions(Vector3[] path)
        {
            _lines.RemoveAll(x => x == null);
            int currentLineCount = _lines.Count;
            for (int i = 0; i < (path.Length - 1); i++)
            {
                if (i + 1 > currentLineCount)
                {
                    var obj = Object.Instantiate(LinePrefab, Vector3.zero, Quaternion.identity, _lineParent.transform);
                    _lines.Add(obj.GetComponent<Line>());
                }
                _lines[i].Start = path[i];
                _lines[i].End = path[i + 1];
            }

            for (int i = path.Length - 1; i < _lines.Count; i++)
            {
                Object.Destroy(_lines[i].gameObject);
            }
        }

        public void Hide()
        {
            _lineParent.gameObject.SetActive(false);
        }
    }
}