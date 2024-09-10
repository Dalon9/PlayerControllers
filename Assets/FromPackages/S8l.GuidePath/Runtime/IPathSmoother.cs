using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.GuidePath.Runtime
{
    public interface IPathSmoother
    {
        Vector3[] Smooth(Vector3[] path);
    }
}