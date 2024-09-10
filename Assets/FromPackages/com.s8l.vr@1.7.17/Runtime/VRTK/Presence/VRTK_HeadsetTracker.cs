namespace VRTK
{
    using UnityEngine;
    using System.Collections;

    [AddComponentMenu("VRTK/Scripts/Presence/VRTK_HeadsetTracker")]
    public class VRTK_HeadsetTracker : MonoBehaviour
    {
        private Transform _head;

        private IEnumerator Start()
        {
            while (!_head)
            {
                _head = VRTK_DeviceFinder.DeviceTransform(VRTK_DeviceFinder.Devices.Headset);
                yield return null;
            }
        }

        private void Update()
        {
            if (!_head) return;
            
            transform.position = _head.position;
            transform.rotation = _head.rotation;
            transform.rotation = _head.rotation;
        }
    }
}