using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VRTK_TrackingSpaceTracker : MonoBehaviour
{
    private Transform _ts;

    private IEnumerator Start()
    {
        while (!_ts)
        {
            _ts = VRTK_DeviceFinder.PlayAreaTransform();
            yield return null;
        }
    }

    private void Update()
    {
        if (!_ts) return;
        var t = transform;
        
        t.position = _ts.position;
        t.rotation = _ts.rotation;
    }
}
