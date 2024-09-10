using System.Collections;
using S8l.Guardian.Runtime.Interfaces;
using UnityEngine;

namespace S8l.Guardian.Runtime.Effects
{
    public class GuardianFadeUIEffect : GuardianEffect, IGuardianEffectProvider
    {
        [SerializeField] private float duration;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool _fadeIn, _fadeOut;
        private Coroutine _coroutine;
        private float _currentFade;

        public void PlayEffect(float strength)
        {
            if (strength >= trigger && _fadeOut)
            {
                StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(FadeCoroutine(true));
            }
            else if (strength >= trigger && !_fadeIn)
            {
                _coroutine = StartCoroutine(FadeCoroutine(true));
            }
            else if (strength < trigger && _fadeIn)
            {
                StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(FadeCoroutine(false));
            }
            else if (strength < trigger && !_fadeOut)
            {
                _coroutine = StartCoroutine(FadeCoroutine(false));
            }
        }

        private IEnumerator FadeCoroutine(bool fadeIn)
        {
            _fadeIn = fadeIn;
            _fadeOut = !fadeIn;
            while (fadeIn ? (_currentFade <= 1) : (_currentFade >= 0))
            {
                canvasGroup.alpha = Mathf.Lerp(0, 1, _currentFade);
                _currentFade = fadeIn
                    ? (_currentFade + Time.deltaTime / duration)
                    : (_currentFade - Time.deltaTime / duration);
                yield return null;
            }
            _fadeIn = false;
            _fadeOut = false;
            _coroutine = null;
        }
    }
}