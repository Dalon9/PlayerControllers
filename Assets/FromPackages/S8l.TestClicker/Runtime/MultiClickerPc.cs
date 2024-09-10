using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace S8l.TestClicker.Runtime
{
    public class MultiClickerPc : MultiClicker
    {
        void Update()
        {
            if (Input.GetMouseButtonUp(0))
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