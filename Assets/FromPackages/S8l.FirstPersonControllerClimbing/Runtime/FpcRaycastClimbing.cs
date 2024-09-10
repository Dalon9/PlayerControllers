using System;
using UnityEngine;
using S8l.FirstPersonController.Runtime;
using S8l.ClimbingHelper.Runtime;

namespace S8l.FirstPersonControllerClimbing.Runtime
{
    [Serializable]
    public class FpcRaycastClimbing : IFpcRaycastStrategy
    {
        [field: SerializeField] public string InteractableAtDistanceTag { get; set; } = "InteractableAtDistance";
        [field: SerializeField] public string FloorTag { get; set; } = "Floor";
        [field: SerializeField] public string ClimbableTag { get; set; } = "Climbable";
        [field: SerializeField] public float MaxInteractionDistance {get; set;} = 3f;
        // not exposing this to inspector since it's set from RaycastStart()
        public FirstPersonController.Runtime.FirstPersonController FPC { get; set; }
        // not exposing this to inspector since it's set from RaycastStart()

        private FirstPersonControllerClimbing _climbingFPC;
        public int Layermask { get; set; }
        
        public void RaycastStart(FirstPersonController.Runtime.FirstPersonController thisFirstPersonController)
        {
            FPC = thisFirstPersonController;
            _climbingFPC = FPC as FirstPersonControllerClimbing;
            Layermask = 1 << 9;
            Layermask = 1 << 2;
        }

        public void Raycast(Vector2 screenPos)
        {
            var ray = _climbingFPC.Cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y));

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, ~Layermask))
            {
                //adjust position - move away from object using hit normal
                Vector3 pos1 = hit.point + hit.normal * _climbingFPC.NavMeshAgent.radius * 1.5f;

                if (hit.transform.tag == FloorTag)
                {
                    _climbingFPC.ToClimbable = false;
                    _climbingFPC.InteractiveObjectTarget = null;

                    if (_climbingFPC.PlayerState != PlayerState.LadderClimbing && _climbingFPC.PlayerState != PlayerState.FreeClimbing)
                    {
                        _climbingFPC.SetWalkDestination(pos1);                        
                    }
                    else
                    {
                        _climbingFPC.ClimbTarget = _climbingFPC.CurrentClimbObj.LadderEntry.position;
                        _climbingFPC.WalkTarget = pos1;
                    }
                }

                if (hit.transform.tag == ClimbableTag)
                {
                    if (_climbingFPC.PlayerState != PlayerState.LadderClimbing && _climbingFPC.PlayerState != PlayerState.FreeClimbing)
                    {
                        _climbingFPC.SetWalkDestination(hit.transform.GetComponent<ClimbableObject>().LadderEntry.position);
                        _climbingFPC.ToClimbable = true;
                    }

                    _climbingFPC.WalkTarget = null;
                    _climbingFPC.ClimbTarget = pos1;
                    _climbingFPC.CurrentClimbObj = hit.transform.GetComponent<ClimbableObject>();
                }

                else if (hit.transform.tag == InteractableAtDistanceTag && Vector3.Distance(hit.transform.position, _climbingFPC.transform.position) < MaxInteractionDistance)
                {
                    hit.transform.gameObject.SendMessage("OnInteraction", _climbingFPC.Cam.transform.tag);
                }
            }
        }

        public void DestroyFpcRef()
        {
            FPC = null;
            _climbingFPC = null;
        }
    }
}
