using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace S8l.TestClickerVR.Runtime
{
    public class MultiClickerVRTK : TestClicker.Runtime.MultiClicker
    {
        private VRTK_ControllerEvents _events;

        private void Start()
        {
            _events = GetComponent<VRTK_ControllerEvents>();
            _events.TriggerReleased += TriggerReleased;
        }

        private void TriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            //Needs check if the currently selected gameobject is the one that has been clicked but works for now
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                PerformMultiClicks();
            }
        }    
    }
}