using System;
using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvasGroup;
        [SerializeField] float fadingTime = 1f;

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOutAndIn()
        {
            yield return FadeOut();
            print("Faded out");
            
            yield return FadeIn();
            print("Faded in");
        }

        public void FadeOutImmediately()
        {
            _canvasGroup.alpha = 1;
        }    
        
        public IEnumerator FadeOut()
        {
            while (_canvasGroup.alpha < 1)
            {
                _canvasGroup.alpha += Time.deltaTime / fadingTime;
                yield return null;
            }
        }    
        
        public IEnumerator FadeIn()
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / fadingTime;
                yield return null;
            }
        }
    }
}