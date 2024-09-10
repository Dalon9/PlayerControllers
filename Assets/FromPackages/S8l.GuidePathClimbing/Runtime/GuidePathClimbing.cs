using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using S8l.ClimbingHelper.Runtime;

namespace S8l.GuidePathClimbing.Runtime
{
    public class GuidePathClimbing : S8l.GuidePath.Runtime.GuidePath
    {
      
        public GameObject WalkLinePrefab;
        public GameObject ClimbLinePrefab;
        public GameObject FreeClimbLinePrefab;
      
        private LineRenderer _walkLine;
        private LineRenderer _climbLine;
        private LineRenderer _freeClimbLine;

        ClimbableObject _climbableObj;

        protected override void CalculatePathToTarget()
        {
            if (_target.gameObject.CompareTag("Climbable"))
            {
                if (_climbLine == null)
                {                   
                    _walkLine = Instantiate(WalkLinePrefab).GetComponent<LineRenderer>();
                    _climbLine = Instantiate(ClimbLinePrefab).GetComponent<LineRenderer>();
                    _freeClimbLine = Instantiate(FreeClimbLinePrefab).GetComponent<LineRenderer>();
                }

                _climbableObj = _target.GetComponentInParent<ClimbableObject>();
                List<Vector3> tempPathPoints = new List<Vector3>();
                NavMesh.SamplePosition(_climbableObj.LadderEntry.position, out _hit, 100f, NavMesh.AllAreas);
                NavMesh.CalculatePath(_player.position, _hit.position, NavMesh.AllAreas, _path);

                tempPathPoints = (SmoothStrategy != null ? SmoothStrategy.Smooth(_path.corners) : _path.corners).ToList();
                tempPathPoints.Add(_climbableObj.LadderEntry.position);
               
                _pathPoints = tempPathPoints.ToArray();
            }
            else
            {
                _climbableObj = null;
                if (_climbLine != null)
                {                   
                    Destroy(_climbLine.gameObject);
                    Destroy(_freeClimbLine.gameObject);
                }

                NavMesh.SamplePosition(_target.position, out _hit, 100f, NavMesh.AllAreas);
                NavMesh.CalculatePath(_player.position, _hit.position, NavMesh.AllAreas, _path);
                _pathPoints = SmoothStrategy != null ? SmoothStrategy.Smooth(_path.corners) : _path.corners;
            }
        }

        protected override void VisualizePath()
        {
            if (_climbableObj != null)
            {
                VisualizeFootPath();
                VisualizeLadder();
                VisualizeFreeClimbing();
            }
            else
            {
                VisualizeStrategy.Visualize(_pathPoints, this);
            }          
        }

        private void VisualizeFootPath()
        {
            _walkLine.positionCount = _pathPoints.Length;
            _walkLine.SetPositions(_pathPoints);
            float length = 0f;
            for (int i = 0; i < (_pathPoints.Length - 1); i++)
            {
                length += (_walkLine.GetPosition(i) - _walkLine.GetPosition(i + 1)).sqrMagnitude;
            }
            length = Mathf.Sqrt(length);
            _walkLine.material.SetTextureScale("_MainTex", new Vector2(10f * length, 1f));
        }

        private void VisualizeLadder()
        {
            _climbLine.positionCount = 2;
            _climbLine.SetPositions(new Vector3[] { _climbableObj.LadderEntry.position, _climbableObj.LadderExit.position });
            _climbLine.material.SetTextureScale("_MainTex", new Vector2(10f * (_climbableObj.LadderExit.position.y - _climbableObj.LadderEntry.position.y), 1f));
        }

        private void VisualizeFreeClimbing()
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(_climbableObj.LadderExit.position);
            
            if (Mathf.Abs(_target.position.x - _climbableObj.LadderExit.position.x) < Mathf.Abs(_target.position.z - _climbableObj.LadderExit.position.z))
            {
                points.Add(new Vector3(_target.position.x, _climbableObj.LadderExit.position.y, _climbableObj.LadderExit.position.z));
                points.Add(new Vector3(_target.position.x, _climbableObj.LadderExit.position.y, _target.position.z));
            }
            else
            {
                points.Add(new Vector3(_climbableObj.LadderExit.position.x, _climbableObj.LadderExit.position.y, _target.position.z));
                points.Add(new Vector3(_target.position.x, _climbableObj.LadderExit.position.y, _target.position.z));
            }
            points.Add(_target.position);
            _freeClimbLine.positionCount = points.Count;
            _freeClimbLine.SetPositions(points.ToArray());
        }
    }
}