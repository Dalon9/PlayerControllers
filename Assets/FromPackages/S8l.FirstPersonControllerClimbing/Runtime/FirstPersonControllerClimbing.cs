using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using S8l.ClimbingHelper.Runtime;
using S8l.FirstPersonController.Runtime;

#if PLAYER_SETUP_PRESENT
using S8l.PlayerSetup.Runtime;
#endif

namespace S8l.FirstPersonControllerClimbing.Runtime
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class FirstPersonControllerClimbing : S8l.FirstPersonController.Runtime.FirstPersonController
    {

        public bool ToClimbable = false;
        public Vector3 ClimbTarget;
        private Vector3? IntermittentClimbTarget;
        public Vector3? WalkTarget;
        public ClimbableObject CurrentClimbObj;
        public float ClimbSpeed = 0.35f;


        private Vector3 _currentClimbTarget;
        private Vector3 _toObj, _direction, _target, _temp;

        protected override void Update()
        {            
            if (PlayerState == PlayerState.Talking)
                return;

            //check if the player has reached his destination, and if so set the state to Idle
            if (PlayerState == PlayerState.Walking)
            {
                if (!NavMeshAgent.pathPending)
                {
                    if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
                    {
                        PlayerState = PlayerState.Idle;
                        if (ToClimbable)
                        {
                            StartClimbing();
                            ToClimbable = false;
                        }
                    }

                    if (InteractiveObjectTarget != null)
                    {
                        PositionMarker.transform.position = InteractiveObjectTarget.transform.position;
                        PositionMarker.SetActive(true);
                    }
                    else
                    {
                        PositionMarker.transform.position = NavMeshAgent.pathEndPosition;
                        PositionMarker.SetActive(true);
                    }
                }
            }
            else if (PlayerState == PlayerState.LadderClimbing)
            {
                float neg = 1f;
                if (ClimbTarget.y < transform.position.y)
                    neg = -1f;

                if (Mathf.Abs(ClimbTarget.y - transform.position.y) > 0.15f)
                {
                    transform.Translate(neg * Vector3.up * NavMeshAgent.speed * ClimbSpeed * Time.deltaTime);
                } 
                
                if (neg > 0 && transform.position.y >= (CurrentClimbObj.LadderExit.position.y - 0.15f))
                {
                    PlayerState = PlayerState.FreeClimbing;
                }

                if (neg < 0 && transform.position.y <= (CurrentClimbObj.LadderEntry.position.y + NavMeshAgent.height / 2f))
                {
                    StopClimbing();
                }
            }

            else if (PlayerState == PlayerState.FreeClimbing)
            {
                if (ClimbTarget.y < CurrentClimbObj.LadderExit.position.y)
                {
                    IntermittentClimbTarget = CurrentClimbObj.LadderExit.position;
                    RaycastHit hit;
                    Physics.Raycast(transform.position, Vector3.Scale(CurrentClimbObj.transform.position - transform.position, new Vector3(1f, 0f, 1f)), out hit);
                    float distance = hit.distance;
                    Physics.Raycast(CurrentClimbObj.LadderExit.position, Vector3.Scale(CurrentClimbObj.transform.position - CurrentClimbObj.LadderExit.position, new Vector3(1f, 0f, 1f)), out hit);
                    IntermittentClimbTarget = hit.point + hit.normal * distance;
                }
                else
                {
                    IntermittentClimbTarget = null;
                }
                _currentClimbTarget = IntermittentClimbTarget != null ? IntermittentClimbTarget.Value : ClimbTarget;
               
                if (Vector2.Distance(new Vector2(_currentClimbTarget.x, _currentClimbTarget.z), new Vector2(transform.position.x, transform.position.z)) > 0.15f)
                {
                    //horizontal
                    _toObj = Vector3.Scale(CurrentClimbObj.transform.position - transform.position, new Vector3(1f, 0f, 1f));
                    _target = Vector3.Scale(_currentClimbTarget - transform.position, new Vector3(1, 0, 1));
                    if (Vector3.SignedAngle(_toObj, Vector3.Scale(_currentClimbTarget - transform.position, new Vector3(1f, 0f, 1f)), Vector3.up) < 0)
                    {
                        _direction = Vector3.Cross(_toObj, Vector3.up).normalized;
                        transform.Translate(_direction * NavMeshAgent.speed * ClimbSpeed * Time.deltaTime, Space.World);
                    }
                    else
                    {
                        _direction = -Vector3.Cross(_toObj, Vector3.up).normalized;
                        transform.Translate(_direction * NavMeshAgent.speed * ClimbSpeed * Time.deltaTime, Space.World);
                    }
                }
                else if (Mathf.Abs(_currentClimbTarget.y - transform.position.y) > 0.1f)
                {
                    //vertical
                    float neg = _currentClimbTarget.y > transform.position.y ? 1f : -1f;
                    transform.Translate(neg * Vector3.up * NavMeshAgent.speed * ClimbSpeed * Time.deltaTime, Space.World);                  
                }

                if (IntermittentClimbTarget != null && Mathf.Abs(transform.position.y - IntermittentClimbTarget.Value.y) < 0.15f)
                {
                    PlayerState = PlayerState.LadderClimbing;
                    IntermittentClimbTarget = null;
                }

            }

            else
            {
                PositionMarker.SetActive(false);
            }

            InputStrategy.InputUpdate();
        }

        public void StartClimbing()
        {
            PlayerState = PlayerState.LadderClimbing;
            NavMeshAgent.enabled = false;
        }

        public void StopClimbing()
        {
            NavMeshAgent.enabled = true;
            PlayerState = PlayerState.Walking;
            if (WalkTarget != null)
            {
                SetWalkDestination(WalkTarget.Value);
            }
        }
    }
}