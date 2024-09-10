using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S8l.ScreenFader.Runtime
{

    public class LWRPScreenFade : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Material"/> to use for the overlay.
        /// </summary>
        /// 
        [SerializeField] private Color _OverlayColor = Color.black;

        public Color OverlayColor
        {
            get { return _OverlayColor; }
            set { _OverlayColor = value; }
        }

        [SerializeField] private Material _OverlayMaterial;

        public Material OverlayMaterial
        {
            get { return _OverlayMaterial; }
            set { _OverlayMaterial = value; }
        }

        /// <summary>
        /// The duration of time to apply the overlay <see cref="Color"/>.
        /// </summary>
        [SerializeField] private float _AddDuration = 1;

        public float AddDuration
        {
            get { return _AddDuration; }
            set { _AddDuration = value; }
        }

        /// <summary>
        /// The duration of time to remove the overlay <see cref="Color"/>.
        /// </summary>
        [SerializeField] private float _RemoveDuration = 1;

        public float RemoveDuration
        {
            get { return _RemoveDuration; }
            set { _RemoveDuration = value; }
        }

        /// <summary>
        /// The duration of time to wait once the overlay <see cref="Color"/> is applied before it is removed.
        /// </summary>
        [SerializeField] private float _AppliedDuration;

        public float AppliedDuration
        {
            get { return _AppliedDuration; }
            set { _AppliedDuration = value; }
        }

        public bool IsAddTransitioning { get; protected set; }

        /// <summary>
        /// Whether an overlay remove transition is in progress.
        /// </summary>
        public bool IsRemoveTransitioning { get; protected set; }

        /// <summary>
        /// The target duration to process the color change for.
        /// </summary>
        protected float targetDuration;

        /// <summary>
        /// A copy of the <see cref="OverlayMaterial"/> to apply the transition overlay color on.
        /// </summary>
        protected Material workingMaterial;

        /// <summary>
        /// The target color to apply to the camera overlay during the process.
        /// </summary>
        protected Color targetColor = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// The current color of the camera overlay during the process.
        /// </summary>
        protected Color currentColor = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// The difference in color of the camera overlay during the process.
        /// </summary>
        protected Color deltaColor = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// The routine for handling the fade in and out of the camera overlay.
        /// </summary>
        protected Coroutine blinkRoutine;

        /// <summary>
        /// Delays the reset of <see cref="blinkRoutine"/> by <see cref="AddDuration"/> plus <see cref="AppliedDuration"/> seconds.
        /// </summary>
        // protected WaitForSeconds resetDelayYieldInstruction;

        private MeshRenderer fadeRenderer;

        public void AddColorOverlay(Color overlayColor, float addDuration)
        {
            OverlayColor = overlayColor;
            AddDuration = addDuration;
            AddColorOverlay();
        }

        /// <summary>
        /// Applies the <see cref="OverlayColor"/> to the cameras via <see cref="CameraValidity"/> over the given <see cref="AddDuration"/>.
        /// </summary>
        public void AddColorOverlay()
        {
            ApplyColorOverlay(OverlayColor, AddDuration);
        }

        /// <summary>
        /// Removes the <see cref="OverlayColor"/> to the cameras via <see cref="CameraValidity"/> over the given <see cref="RemoveDuration"/>.
        /// </summary>
        public void RemoveColorOverlay()
        {
            ApplyColorOverlay(Color.clear, RemoveDuration);
        }

        /// <summary>
        /// Applies the <see cref="OverlayColor"/> to the cameras via <see cref="CameraValidity"/> over the given <see cref="AddDuration"/> then waits for the given <see cref="AppliedDuration"/> then removes the <see cref="OverlayColor"/> over the given <see cref="RemoveDuration"/>.
        /// </summary>
        public void Blink(Action callback = null)
        {
            ApplyColorOverlay(OverlayColor, AddDuration);
            blinkRoutine = StartCoroutine(ResetBlink(callback));
        }

        protected virtual void OnEnable()
        {
            CopyMaterialOverlayToWorking();
        }

        protected virtual void OnDisable()
        {
            CancelBlinkRoutine();
            // Camera.onPostRender -= PostRender;
        }

        /// <summary>
        /// Applies the given <see cref="Color"/> to the cameras via <see cref="CameraValidity"/> over the given duration.
        /// </summary>
        /// <param name="newColor"><see cref="Color"/> to apply to the overlay.</param>
        /// <param name="duration">The duration over which the <see cref="Color"/> is applied.</param>
        protected virtual void ApplyColorOverlay(Color newColor, float duration)
        {
            CancelBlinkRoutine();

            if (newColor != targetColor || (Mathf.Abs(duration - targetDuration) > Mathf.Epsilon)
            ) // !duration.ApproxEquals(targetDuration))
            {
                targetDuration = duration;
                targetColor = newColor;
                if (duration > 0.0f)
                {
                    deltaColor = (targetColor - currentColor) / duration;
                }
                else
                {
                    currentColor = newColor;
                }

                IsAddTransitioning = false;
                IsRemoveTransitioning = false;
                if (newColor != Color.clear)
                {
                    IsAddTransitioning = true;
                    // Added?.Invoke(eventData.Set(newColor));
                }
                else
                {
                    IsRemoveTransitioning = true;
                }
            }
        }

        protected virtual IEnumerator ResetBlink(Action callback = null)
        {
            yield return new WaitForSeconds(AddDuration);
            callback?.Invoke();
            yield return new WaitForSeconds(AppliedDuration);
            RemoveColorOverlay();
        }

        /// <summary>
        /// Cancels any existing running <see cref="ResetBlink(float)"/> Coroutine.
        /// </summary>
        protected virtual void CancelBlinkRoutine()
        {
            if (blinkRoutine != null)
            {
                StopCoroutine(blinkRoutine);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // create the fade material
            MeshFilter fadeMesh = gameObject.AddComponent<MeshFilter>();
            fadeRenderer = gameObject.AddComponent<MeshRenderer>();

            var mesh = new Mesh();
            fadeMesh.mesh = mesh;

            Vector3[] vertices = new Vector3[4];

            float width = 100f;
            float height = 100f;
            float depth = 1f;

            vertices[0] = new Vector3(-width, -height, depth);
            vertices[1] = new Vector3(width, -height, depth);
            vertices[2] = new Vector3(-width, height, depth);
            vertices[3] = new Vector3(width, height, depth);

            mesh.vertices = vertices;

            int[] tri = new int[6];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];

            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;

            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.uv = uv;


        }

        // Update is called once per frame
        void Update()
        {
            if (currentColor != targetColor)
            {
                if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
                {
                    currentColor = targetColor;
                    deltaColor = new Color(0, 0, 0, 0);
                }
                else
                {
                    currentColor += deltaColor * Time.deltaTime;
                }
            }
            else if (IsAddTransitioning)
            {
                IsAddTransitioning = false;
            }
            else if (IsRemoveTransitioning)
            {
                IsRemoveTransitioning = false;
            }

            if (currentColor.a > 0f && workingMaterial != null)
            {
                currentColor.a = (targetColor.a > currentColor.a && currentColor.a > 0.98f ? 1f : currentColor.a);
                workingMaterial.color = currentColor;


                // workingMaterial.renderQueue = renderQueue;
                fadeRenderer.material = workingMaterial;
                fadeRenderer.enabled = true;
            }
            else
            {
                fadeRenderer.enabled = false;
            }
        }

        protected void CopyMaterialOverlayToWorking()
        {
            if (Application.isPlaying)
            {
                Destroy(workingMaterial);
            }
            else
            {
                DestroyImmediate(workingMaterial);
            }

            if (OverlayMaterial != null)
            {
                workingMaterial = new Material(OverlayMaterial);
            }
        }
    }
}