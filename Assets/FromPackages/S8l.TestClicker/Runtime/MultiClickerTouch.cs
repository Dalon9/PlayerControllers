using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace S8l.TestClicker.Runtime
{
    public class MultiClickerTouch : MultiClicker
    {
        void Update()
        {
            if (Input.touchCount > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    if (EventSystem.current.currentSelectedGameObject != null)
                    {
                        if (CheckClick2D())
                        {
                            PerformMultiClicks();
                        }
                    }
                }
            }
        }
    }
}