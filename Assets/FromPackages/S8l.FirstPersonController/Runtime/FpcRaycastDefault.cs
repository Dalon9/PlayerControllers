using System;
using UnityEngine;

namespace S8l.FirstPersonController.Runtime
{

    [Serializable]
    public class FpcRaycastDefault : IFpcRaycastStrategy
    {
        [field: SerializeField] public string InteractableAtDistanceTag { get; set; } = "InteractableAtDistance";
        [field: SerializeField] public string FloorTag { get; set; } = "Floor";
        [field: SerializeField] public float MaxInteractionDistance {get; set;} = 3f;
        // not exposing this to inspector since it's set from RaycastStart()
        public FirstPersonController FPC { get; set; }
        // not exposing this to inspector since it's set from RaycastStart()
        public int Layermask { get; set; }
        
        public void RaycastStart(FirstPersonController thisFirstPersonController)
        {
            FPC = thisFirstPersonController;
            Layermask = 1 << 9;
            Layermask = 1 << 2; 
        }

        public virtual void Raycast(Vector2 screenPos)
        {
            var ray = FPC.Cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y));

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, ~Layermask))
            {
                //adjust position - move away from object using hit normal
                Vector3 pos1 = hit.point + hit.normal * FPC.NavMeshAgent.radius * 1.5f;

                if (hit.transform.tag == FloorTag)
                {
                    FPC.SetWalkDestination(pos1);
                    FPC.InteractiveObjectTarget = null;
                }

                /*else if (hit.transform.tag == "Interactable" || (hit.transform.tag == "NPC" && NpcWithDialog(hit.transform.gameObject)))
                {
                    SetWalkDestination(pos1);
                    interactiveObjectTarget = hit.transform.gameObject;
                }*/

                else if (hit.transform.tag == InteractableAtDistanceTag && Vector3.Distance(hit.transform.position, FPC.transform.position) < MaxInteractionDistance)
                {
                    hit.transform.gameObject.SendMessage("OnInteraction", FPC.Cam.transform.tag);
                }
            }
        }

        public void DestroyFpcRef()
        {
            FPC = null;
        }
    }
}
