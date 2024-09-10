using PathCreation;
using S8l.GuidePath.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using S8l.GuidePathSmootherBezier.Runtime;

namespace S8l.GuidePathVisualizerBezierPoints.Runtime
{
    public class PathVisualizerBezierPoints : IPathVisualizer
    {
        public GameObject VisualizerPrefab;
        public float Distance = 1f;
        public float Speed = 0f;
        public bool ApplyOrientation = false;

        private PathCreator _path;
        private List<GameObject> _points;
        private float _offset;
        private int _currentPointCount;
        private int _pointIndex;
        private Transform _pointsParent;

        public void Visualize(Vector3[] path, GuidePath.Runtime.GuidePath caller)
        {
            if (_path == null)
            {
                _path = (caller.SmoothStrategy as PathSmootherBezier).GetPathCreator();
                _points = new List<GameObject>();
                _pointsParent = new GameObject("PointsParent").transform;
            }

            if (Distance <= 0)
                return;

            _pointsParent.gameObject.SetActive(true);

            _offset = (_offset + Time.deltaTime * Speed) % Distance;
            _points.RemoveAll(x => x == null);
            _currentPointCount = _points.Count;
            _pointIndex = 0;

            for (float f = _path.path.length - (Distance - _offset); f > 0; f -= Distance)
            {
                _pointIndex++;

                if (_pointIndex > _currentPointCount)
                {
                    var obj = Object.Instantiate(VisualizerPrefab, _path.path.GetPointAtDistance(f, EndOfPathInstruction.Stop),
                        ApplyOrientation ? _path.path.GetRotationAtDistance(f) : Quaternion.identity, _pointsParent);
                    obj.name = _pointIndex.ToString();
                    _points.Add(obj);
                }
                else
                {
                    _points[_pointIndex - 1].transform.position = _path.path.GetPointAtDistance(f, EndOfPathInstruction.Stop);
                    _points[_pointIndex - 1].name = _pointIndex.ToString();
                    if (ApplyOrientation)
                        _points[_pointIndex - 1].transform.rotation = _path.path.GetRotationAtDistance(f);
                }
            }

            for (int i = _pointIndex; i < _points.Count; i++)
            {
                Object.Destroy(_points[i].gameObject);
            }

        }

        public void Hide()
        {
            _pointsParent.gameObject.SetActive(false);
        }
    }
}