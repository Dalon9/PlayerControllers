using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace S8l.GuidePath.Runtime
{
    public class GuidePath : MonoBehaviour
    {
        [SerializeReference]
        public IPathSmoother SmoothStrategy;
        [SerializeReference]
        public IPathVisualizer VisualizeStrategy = new PathVisualizerLineRenderer { };

        protected Vector3[] _pathPoints;

        protected NavMeshPath _path;
        protected NavMeshHit _hit;

        protected Transform _player;
        protected Transform _target;
        protected bool _showing = false;

        protected void Awake()
        {
            _path = new NavMeshPath();
        }

        protected void Update()
        {
            if (!_showing)
                return;

            CalculatePathToTarget();
            VisualizePath();
        }

        public void StartShowing(Transform player, Transform target)
        {
            _player = player;
            _target = target;
            _showing = true;
        }

        public void StopShowing()
        {
            _showing = false;
            VisualizeStrategy.Hide();
        }

        protected virtual void CalculatePathToTarget()
        {
            NavMesh.SamplePosition(_target.position, out _hit, 100f, NavMesh.AllAreas);
            NavMesh.CalculatePath(_player.position, _hit.position, NavMesh.AllAreas, _path);
            if (_path.corners.Length > 0)
            {
                _pathPoints = SmoothStrategy != null ? SmoothStrategy.Smooth(_path.corners) : _path.corners;
            }
        }

        protected virtual void VisualizePath()
        {
            VisualizeStrategy.Visualize(_pathPoints, this);
        }
    }
}