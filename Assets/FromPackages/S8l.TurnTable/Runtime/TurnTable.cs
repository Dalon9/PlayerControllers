using System;
using System.Collections.Generic;
using S8l.TurnTable.Runtime.Implementations.Applicators;
#if ENABLE_LEGACY_INPUT_MANAGER
using S8l.TurnTable.Runtime.Implementations.Legacy;
#endif
using S8l.TurnTable.Runtime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace S8l.TurnTable.Runtime
{

    public class TurnTable : MonoBehaviour, ITurnTable
    {
        public Camera CameraReference
        {
        	get{return _cameraReference;}
            set{_cameraReference = value;}
        }
        [SerializeField] private Camera _cameraReference;

        public Transform Target => _target;
        [SerializeField] private Transform _target;
        
        [Tooltip("Positive values make the subject spin counterclockwise. Zero equals off.")]
        public float AutoTurn = 5f;
        public float AutoTurnTimer = 10f;
        private DateTime _lastInteraction = DateTime.Now;

        public Action OnUpdate { get; set; }
        public Func<Quaternion> OnRotUpdate { get; set; }
        public Func<float> OnZoomUpdate { get; set; }

        [SerializeReference]
        public IRotApplicationStrategy RotApplicationStrategy = new ClampedRotApplicationStrategy();

        [SerializeReference]
        public IZoomApplicationStrategy ZoomApplicationStrategy = new MoveTargetZoomApplicationStrategy();

        [SerializeReference] public List<IRotRetrievalStrategy> RotRetrievalStrategies =
            new List<IRotRetrievalStrategy>()
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                new LegacyMouseRotRetrievalStrategy(), new LegacyTouchRotRetrievalStrategy()
#endif
            };

        [SerializeReference] public List<IZoomRetrievalStrategy> ZoomRetrievalStrategies =
            new List<IZoomRetrievalStrategy>()
            {
#if ENABLE_LEGACY_INPUT_MANAGER
                new LegacyMouseZoomRetrievalStrategy(), new LegacyTouchZoomRetrievalStrategy()
#endif
            };

        void Awake()
        {
            InitStrategies();
        }

        void LateUpdate()
        {
            Quaternion rotDelta = RetrieveRotDelta();
            float zoomDelta = RetrieveZoomDelta();

            if (rotDelta.eulerAngles.magnitude + zoomDelta != 0f)
                _lastInteraction = DateTime.Now;

            Quaternion autoTurnEffect = Quaternion.identity;

            if ((DateTime.Now - _lastInteraction).Seconds > AutoTurnTimer)
                autoTurnEffect = Quaternion.Euler(new Vector3(0F, AutoTurn * Time.deltaTime, 0f));

            ApplyRotDelta(rotDelta * autoTurnEffect);
            ApplyZoomDelta(zoomDelta);
        }

        void ApplyRotDelta(Quaternion rotDelta)
        {
            RotApplicationStrategy?.ApplyRotation(rotDelta);
        }

        void ApplyZoomDelta(float zoomDelta)
        {
            ZoomApplicationStrategy?.ApplyZoom(zoomDelta);
        }


        #region Helpers

        void InitStrategies()
        {

            RotApplicationStrategy?.Init(this);
            ZoomApplicationStrategy?.Init(this);

            foreach (var s in RotRetrievalStrategies)
                s?.Init(this);
            foreach (var s in ZoomRetrievalStrategies)
                s?.Init(this);
        }

        float RetrieveZoomDelta()
        {
            float deltaZoom = 0f;
            if (OnZoomUpdate != null)
            {

                foreach (Delegate d in OnZoomUpdate.GetInvocationList())
                {
                    float partResult = (float)d.DynamicInvoke();
                    deltaZoom += partResult;
                }

                //Debug.Log($"Accumulated DeltaZoom: {deltaZoom}");
            }

            return deltaZoom;
        }

        Quaternion RetrieveRotDelta()
        {
            Quaternion deltaRot = Quaternion.identity;
            if (OnRotUpdate != null)
            {
                foreach (Delegate d in OnRotUpdate.GetInvocationList())
                {
                    Quaternion partResult = (Quaternion)d.DynamicInvoke();
                    deltaRot *= partResult;
                }
                //Debug.Log($"Accumulated DeltaRot: {deltaRot}");
            }

            return deltaRot;
        }

        #endregion
    }
}
