using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FacingCanvasToCamera : MonoBehaviour
{

    [SerializeField] Image debugBackgroundImage;
    CanvasGroup _canvasGroup;
    
    //Cached
    Transform _camera;
    Transform _debugBackgroundImageTransform;

    void Awake()
    {
        _camera = Camera.main.transform;
        _canvasGroup = GetComponent<CanvasGroup>();
        _debugBackgroundImageTransform = debugBackgroundImage.transform;
    }
    
    void Start()
    {
        _debugBackgroundImageTransform.rotation = _camera.rotation;
    }

    void Update()
    {
        //It'll rotate Canvas to the camera only if Canvas will be visible
        if (_canvasGroup.alpha != float.Epsilon)
            _debugBackgroundImageTransform.rotation = _camera.rotation;
    }
}
