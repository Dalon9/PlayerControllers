using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.GuidePath.Runtime
{
    public class PathVisualizerLineRenderer : IPathVisualizer
    {
        public GameObject LineRendererPrefab;
        public bool AnimatedColor;
        public Color Color1, Color2;
        public float Speed = 0.1f;
        public int MinPoints = 8;

        private LineRenderer _lineRenderer;
        private bool _colorInitiated = false;
        private float _offset;
        private GradientColorKey[] _gcks;
        private GradientAlphaKey[] _gaks;
        private Gradient _gradient;

        public void Visualize(Vector3[] path, GuidePath guidePath)
        {
            if (_lineRenderer == null)
                CreateLineRenderer();

            _lineRenderer.gameObject.SetActive(true);
            _lineRenderer.positionCount = path.Length;
            _lineRenderer.SetPositions(path);

            if (AnimatedColor && _lineRenderer.positionCount > 1)
                AnimateColor();

        }

        private void AnimateColor()
        {
            if(!_colorInitiated)
            {
                InitiateColors();
            }

            if (_lineRenderer.positionCount < MinPoints)
            {
                AddAdditionalPoints();   
            }

            _offset = (_offset + Time.deltaTime * Speed) % 1f;

            float t = 1f - Mathf.Abs(1f - 2f * _offset);

            _gcks[0] = new GradientColorKey(Color.Lerp(Color1, Color2, t), 0f);
            _gcks[1] = new GradientColorKey(Color1, _offset);
            _gcks[2] = new GradientColorKey(Color2, (0.5f + _offset) % 1f);
            _gcks[3] = new GradientColorKey(Color.Lerp(Color1, Color2, t), 1f);

            _gradient.SetKeys(_gcks, _gaks);

            _lineRenderer.colorGradient = _gradient;
        }

        private void InitiateColors()
        {
            _gcks = new GradientColorKey[4];
            _gcks[0] = new GradientColorKey(Color1, 0f);
            _gcks[1] = new GradientColorKey(Color1, 0f);
            _gcks[2] = new GradientColorKey(Color2, 0.5f);
            _gcks[3] = new GradientColorKey(Color1, 1f);

            _gaks = new GradientAlphaKey[2];
            _gaks[0] = new GradientAlphaKey(1f, 0f);
            _gaks[1] = new GradientAlphaKey(1f, 1f);

            _gradient = new Gradient();
            _gradient.SetKeys(_gcks, _gaks);

            _lineRenderer.colorGradient = _gradient;
            _colorInitiated = true;
        }

        private void AddAdditionalPoints()
        {
            int extraPoints = MinPoints - _lineRenderer.positionCount;
            int startPoint = _lineRenderer.positionCount - 1;
            int oldCount = _lineRenderer.positionCount;
            Vector3 pos0 = _lineRenderer.GetPosition(0);
            Vector3 pos1 = _lineRenderer.GetPosition(1);
            _lineRenderer.positionCount += extraPoints;
            for (int i = oldCount - 1; i > 0; i--)
            {
                _lineRenderer.SetPosition(i + extraPoints, _lineRenderer.GetPosition(i));
            }                          
            for (int i = 1; i <= extraPoints; i++)
            {
                _lineRenderer.SetPosition(i, Vector3.Lerp(pos0, pos1, i / (extraPoints + 1f)));
            }
        }

        private void CreateLineRenderer()
        {
            var obj = GameObject.Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity);
            _lineRenderer = obj.GetComponent<LineRenderer>();
        }

        public void Hide()
        {
            _lineRenderer.gameObject.SetActive(false);
        }
    }
}