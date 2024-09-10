using System;
using S8l.TurnTable.Runtime.Interfaces;
using UnityEngine;

namespace S8l.TurnTable.Runtime.Implementations.Applicators
{
    public class MoveTargetZoomApplicationStrategy : IZoomApplicationStrategy
    {
        public float MinDistance = 0.1f;
        public float MaxDistance = 5f;
        
        private ITurnTable _holder;
        private float _zoomDistCache;
        public void Init(ITurnTable parent)
        {
            _holder = parent;
            _zoomDistCache = Vector3.Distance(_holder.Target.transform.position,
                _holder.CameraReference.transform.position);
        }

        public void ApplyZoom(float delta)
        {
            _zoomDistCache += delta;
            _zoomDistCache = Mathf.Clamp(_zoomDistCache, MinDistance, MaxDistance);
            // Could be cached for fixed cam position
            Vector3 direction = (_holder.Target.transform.position - _holder.CameraReference.transform.position)
                .normalized;
            _holder.Target.transform.position = _holder.CameraReference.transform.position + direction * _zoomDistCache;
        }
    }
}