using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace RPG.Core
{
    public class TextToCamera : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        void Update()
        {
            text.transform.forward = Camera.main.transform.forward;
        }
    }
}