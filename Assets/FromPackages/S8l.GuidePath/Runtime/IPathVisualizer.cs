using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.GuidePath.Runtime
{
    public interface IPathVisualizer
{
    void Visualize(Vector3[] path, GuidePath caller = null);

    void Hide();
    }
}