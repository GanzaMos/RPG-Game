using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarHandler : MonoBehaviour
{
    [SerializeField] RectTransform greenLine;
    [SerializeField] Canvas canvas;

    public void SetHealthPercentage(float percentage)
    {
        if (percentage <= 0)
        {
            canvas.enabled = false;
            SetRectTransformScale(percentage);
        }
        else if (percentage > 0 && !canvas.enabled)
        {
            canvas.enabled = true;
            SetRectTransformScale(percentage);
        }
        else
        {
            SetRectTransformScale(percentage);
        }
    }

    void SetRectTransformScale(float percentage)
    {
        greenLine.localScale = new Vector3(percentage, 1, 1);
    }
}
