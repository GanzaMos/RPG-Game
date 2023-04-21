using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvasGroup;
        Coroutine _currentRoutine = null;
        [SerializeField] float fadingTime = 1f;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOutAndIn()
        {
            yield return FadeOut();

            yield return FadeIn();
        }

        public void FadeOutImmediately()
        {
            _canvasGroup.alpha = 1;
        }    
        
        public Coroutine FadeOut()
        {
            return Fade(1);
        }
        
        public Coroutine FadeIn()
        {
            return Fade(0);
        }

        public Coroutine Fade(float targetAlpha)
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
            }
            _currentRoutine = StartCoroutine(FadeRoutine(targetAlpha));
            return _currentRoutine;
        }

        IEnumerator FadeRoutine(float targetAlpha)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, targetAlpha))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, targetAlpha, Time.deltaTime / fadingTime);
                yield return null;
            }
        }
    }
}