using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace S8l.TestClicker.Runtime
{
    public abstract class MultiClicker : MonoBehaviour
    {
        public int NumberOfClicks = 5;
        public int FrameDuration = 5;

        private EventSystem eventSystem;

        private void Start()
        {
            eventSystem = EventSystem.current;
        }

        protected void PerformMultiClicks()
        {
            StartCoroutine(MultiClicks());

            IEnumerator MultiClicks()
            {               
                if (NumberOfClicks >= FrameDuration)
                {
                    int clicks = 0;
                    int clicksPerFrame = Mathf.CeilToInt((float)NumberOfClicks / FrameDuration);
                    for (int i = 0; i < FrameDuration; i++)
                    {
                        yield return null;
                        for (int j = 0; j < clicksPerFrame; j++)
                        {                         
                            PerformClick();
                            clicks++;
                            if (clicks >= NumberOfClicks)
                            {
                                break;
                            }                           
                        }                       
                    }
                }
                else
                {
                    float clicksPerFrame = (float)NumberOfClicks / FrameDuration;
                    float clickFrame = 0f;
                    for (int i = 0; i < FrameDuration; i++)
                    {
                        yield return null;
                        clickFrame += clicksPerFrame;
                        if (clickFrame >= 1)
                        {
                            PerformClick();
                            clickFrame -= 1f;
                        }
                    }
                }
            }
        }

        private void PerformClick()
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }

        protected bool CheckClick2D()
        {
            Vector2 clickPos = Input.mousePosition;

            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = clickPos;

            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, hits);

            foreach (RaycastResult hit in hits)
            {
                GameObject go = hit.gameObject;
                var click = go;
                //check children and the direct parent for UIButton component
                if (click.GetComponent<Button>() == null)
                {
                    var child = go.GetComponentInChildren<Button>();
                    if (child != null)
                        click = child.gameObject;
                    if (click.GetComponent<Button>() == null)
                    {
                        var parent = click.transform.parent.GetComponent<Button>();
                        if (parent != null)
                        {
                            click = parent.gameObject;
                        }
                    }
                }

                // Stop after topmost active interactable button
                if (click == eventSystem.currentSelectedGameObject)
                {
                    return true;
                }
            }
            return false;
        }
    }
}