using S8l.BaseConfiguration;
using S8l.CustomPlatformSharedInterfaces.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using S8l.CustomPlatformDetector.Runtime;
using S8l.CustomPlatformDetector.Runtime.Platforms;

#if PLAYER_SETUP_PRESENT
using S8l.PlayerSetup.Runtime;
#endif


namespace S8l.FirstPersonController.Runtime
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Talking,
        LadderClimbing,
        FreeClimbing
    }

    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class FirstPersonController : MonoBehaviour
    {
        public bool UsePackageConfig;
        //serialized interfaces
        [SerializeReference]
        public IFpcConfig Config = new FpcConfig();
        [SerializeReference]
        public FpcInputStrategy InputStrategy = new FpcInputMouse();
        [SerializeReference]
        public IFpcRaycastStrategy RaycastStrategy = new FpcRaycastDefault();

        public GameObject PositionMarker;
        public PlayerState PlayerState = PlayerState.Idle;
        public GameObject InteractiveObjectTarget;

        public UnityEngine.AI.NavMeshAgent NavMeshAgent;
        public Camera Cam;
        public Transform CameraX;
        public Transform CameraY;
        public GameObject PCAvatar;
        public float YViewRotation;

        protected Transform[] _positions; //position where player left off when panning into object view
        protected FpcPackageConfiguration _packageConfig;
        protected Transform _conversationCamBuffer;

        protected void Awake()
        {
            _packageConfig = PackageConfiguration.GetCurrentConfiguration<FpcPackageConfiguration>(out _);
            if (UsePackageConfig && _packageConfig != null)
            {
                LoadPackageConfig();
            }
            else
            {
                if (UsePackageConfig)
                {
                    Debug.Log("No FpcPackageConfiguration could be retrieved, using default");
                }
                if (Config == null)
                {
                    Debug.Log("No Config");
                }
                if (InputStrategy == null)
                {
                    Debug.Log("no FpcInputHandling strategy set, using default");
                    InputStrategy = new FpcInputMouse();
                }
                if (RaycastStrategy == null)
                {
                    Debug.Log("no FpcInputHandling strategy set, using default");
                    RaycastStrategy = new FpcRaycastDefault();
                }
            }          

            Cam = GetComponentInChildren<Camera>();
            _positions = new Transform[2];


            if (Cam == null)
                Cam = GetComponentInChildren<Camera>();

            NavMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            CameraX = transform;
            CameraY = Cam.transform;

            NavMeshAgent.updatePosition = true;
            NavMeshAgent.updateRotation = true;
         
            InputStrategy.InputStart(this);          
            RaycastStrategy.RaycastStart(this);

            InteractiveObjectTarget = null;


#if PLAYER_SETUP_PRESENT
            if (InputStrategy is FpcInputMouse)
            {
                Info.instance.playerSetupType = PlayerSetupType.Mouse;
            }
            else if (InputStrategy is FpcInputTouch)
            {
                Info.instance.playerSetupType = PlayerSetupType.Touch;
            }
#endif
        }

        protected void LoadPackageConfig()
        {
            RaycastStrategy = _packageConfig.raycastStrategy;
            Config = _packageConfig.config;
            if (_packageConfig.inputStrategies != null)
            {
                List<IPlatform> detectedPlatforms = CustomPlatformDetectorFacade.DetectPlatforms(orderBy: true); // Ordered with least general first
                foreach (BasePlatform platform in detectedPlatforms)
                {
                    foreach (var keyValuePair in _packageConfig.inputStrategies)
                    {
                        if (platform.Equals(keyValuePair.Key))
                        {
                            InputStrategy = keyValuePair.Value;
                            return;                                    // Stop after first match
                        }
                    }
                }
            }
            InputStrategy = new FpcInputMouse();
        }

        public void OnConversationStart()
        {
            PlayerState = PlayerState.Talking;
            _conversationCamBuffer = transform;
        }

        public void OnConversationEnd()
        {
            PlayerState = PlayerState.Idle;
            InteractiveObjectTarget = null;
        }

        protected virtual void Update()
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
            else
            {
                PositionMarker.SetActive(false);
            }

            InputStrategy.InputUpdate();
        }

        public void SetWalkDestination(Vector3 position)
        {
            NavMeshAgent.speed = Config.WalkSpeed;

            if (NavMeshAgent.isActiveAndEnabled && NavMeshAgent.isOnNavMesh)
                NavMeshAgent.SetDestination(position);


            PlayerState = PlayerState.Walking;

            NavMeshAgent.updateRotation = true;
        }

        public void StopMoving()
        {
            PlayerState = PlayerState.Idle;

            Vector3 playerPosition = gameObject.transform.position;
            playerPosition.y -= GetComponent<UnityEngine.AI.NavMeshAgent>().baseOffset;

            NavMeshAgent.SetDestination(playerPosition);

            NavMeshAgent.updateRotation = true;
        }

        public void PanCameraFromOtherScript(int num)
        {
            StartCoroutine("PanCamera", num);
        }

        public IEnumerator PanCamera(int num)
        {
            float t = 1.0f;
            float startTime = Time.time;
            float delta;

            while (true)
            {
                float bub = Time.time - startTime;
                delta = (bub / t);

                Cam.transform.position = Vector3.Lerp(_positions[1 - num].transform.position,
                    _positions[num].transform.position, delta);
                Cam.transform.rotation = Quaternion.Lerp(_positions[1 - num].transform.rotation,
                    _positions[num].transform.rotation, delta);

                if (bub > t)
                {
                    if (num == 0)
                    {
                        InputStrategy.IgnoreInput = false;
                    }

                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        protected void OnDestroy()
        {
            InputStrategy.DestroyFpcRef();
            RaycastStrategy.DestroyFpcRef();
        }
    }
}