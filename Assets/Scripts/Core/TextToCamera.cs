using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace RPG.Core
{
    public class TextToCamera : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        //Cached
        Transform _camera;
        Transform _textTransform;
        
        void Awake()
        {
            _camera = Camera.main.transform;
            _textTransform = text.transform;
        }

        void Update()
        {
            _textTransform.forward = _camera.forward;
        }
    }
}