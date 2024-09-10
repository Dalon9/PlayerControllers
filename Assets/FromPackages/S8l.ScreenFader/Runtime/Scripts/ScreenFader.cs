using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace S8l.ScreenFader.Runtime
{
    public class ScreenFader : MonoBehaviour
    {
        public string SceneToLoad;
        public Image BlendImage;
        public bool FadeOutOnAwake = true;
        public bool DontDestroy = true;

        [Tooltip("If FadeOutOnAwake is true, screen fades out from this color to transparent.")]
        public Color32 FadeColorOnAwake = new Color32(0, 0, 0, 255);
        [Tooltip("Screen Fader increases alpha to 1 and back to start alpha, so FadeColor alpha should be 0.")]
        public Color32 FadeColor = new Color32(0, 0, 0, 0);


        public static ScreenFader Instance = null;

        private bool isFadeActive = false;

        void Awake()
        {
            // Preventing other instances
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("An instance of this singleton already exists. Destroying " + this.gameObject.name);
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                if (DontDestroy)
                    DontDestroyOnLoad(gameObject);
            }

            if (FadeOutOnAwake)
            {             
                BlendImage.color = FadeColorOnAwake;
                FadeOut(2f);
            }
        }

        public void FadeOut(float duration = 2f, Action callback = null)
        {
            StartCoroutine(Work());
            IEnumerator Work()
            {
                BlendImage.enabled = true;
                LeanTween.cancel(BlendImage.gameObject);
                isFadeActive = true;
                LeanTween.alpha(BlendImage.rectTransform, 0f, duration);
                yield return new WaitForSeconds(duration);
                BlendImage.enabled = false;
                isFadeActive = false;
                callback?.Invoke();
            }
        }

        public void FadeIn(float duration = 2f, Action callback = null)
        {
            StartCoroutine(Work());
            IEnumerator Work()
            {
                LeanTween.cancel(BlendImage.gameObject);
                isFadeActive = true;
                BlendImage.enabled = true;
                BlendImage.color = FadeColor;
                LeanTween.alpha(BlendImage.rectTransform, 1f, duration);
                yield return new WaitForSeconds(duration);
                isFadeActive = false;
                callback?.Invoke();
            }
        }     

        public void JustFadeInAndOut(float duration = 4f, float midwayWaitDuration = 0f, Action midWayCallback = null, Action endCallback = null)
        {
            if (isFadeActive)
            {
                Debug.Log("ScreenFader is already running fade");
                return;
            }

            StartCoroutine(JustFadeInAndOutCoroutine(duration, midwayWaitDuration, midWayCallback, endCallback));
        }

        private IEnumerator JustFadeInAndOutCoroutine(float duration = 4f, float midwayWaitDuration = 0f, Action midWayCallback = null,
            Action endCallback = null)
        {
            LeanTween.cancel(BlendImage.gameObject);
            isFadeActive = true;
            BlendImage.enabled = true;
            BlendImage.color = FadeColor;

            LeanTween.alpha(BlendImage.rectTransform, 1f, duration / 2f);
            yield return new WaitForSeconds(duration / 2f);

            midWayCallback?.Invoke();
            yield return new WaitForSeconds(midwayWaitDuration);

            LeanTween.alpha(BlendImage.rectTransform, 0f, duration / 2f);
            yield return new WaitForSeconds(duration / 2f);

            endCallback?.Invoke();

            BlendImage.color = FadeColor;
            BlendImage.enabled = false;
            isFadeActive = false;
        }

        public void Load(Action midWayCallback = null)
        {
            if (isFadeActive)
            {
                Debug.Log("ScreenFader is already running fade");
                return;
            }

            StartCoroutine(WaitAndLoad());
        }

        IEnumerator WaitAndLoad(Action midWayCallback = null)
        {
            LeanTween.cancel(BlendImage.gameObject);
            isFadeActive = true;
            BlendImage.enabled = true;
            BlendImage.color = FadeColor;
            LeanTween.alpha(BlendImage.rectTransform, 1f, 2f);
            yield return new WaitForSeconds(2f);
            midWayCallback?.Invoke();
            yield return new WaitForEndOfFrame();

            if (!string.IsNullOrEmpty(SceneToLoad))
            {
                SceneManager.LoadScene(SceneToLoad);
            }
            else
            {
                Debug.LogError("No scene name specified");
            }

            isFadeActive = false;
        }
    }
}