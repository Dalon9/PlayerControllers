using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using S8l.GuidePath.Runtime;

namespace S8l.GuidePathSmootherBezier.Runtime
{
    [System.Serializable]
    public class PathSmootherBezier : IPathSmoother
    {
        public GameObject PathCreatorPrefab;
        public float MaxAngleError = .3f;
        public float MinVertexSpacing = .01f;

        private PathCreator _pathCreator;

        public Vector3[] Smooth(Vector3[] path)
        {
            if (_pathCreator == null)
            {
                var obj = GameObject.Instantiate(PathCreatorPrefab, Vector3.zero, Quaternion.identity);
                _pathCreator = obj.GetComponent<PathCreator>();
                _pathCreator.EditorData.vertexPathMaxAngleError = MaxAngleError;
                _pathCreator.EditorData.vertexPathMinVertexSpacing = MinVertexSpacing;
            }
            _pathCreator.bezierPath = new BezierPath(Vector3.zero);
            _pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Automatic;

            _pathCreator.bezierPath.MovePoint(0, path[0]);
            _pathCreator.bezierPath.MovePoint(3, path[path.Length - 1]);

            for (int i = 1; i < path.Length - 1; i++)
            {
                _pathCreator.bezierPath.SplitSegment(path[i], i - 1, 0.5f);
            }

            return _pathCreator.path.localPoints;
        }

        public PathCreator GetPathCreator()
        {
            return _pathCreator;
        }
    }
}