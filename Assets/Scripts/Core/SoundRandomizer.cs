using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPG.Core
{
    public class SoundRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] audioClip;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>() ?? throw new Exception($"Missing AudioSource component for {name}, ID {GetInstanceID()}");
        }

        public void PlayRandomSound()
        {
            if (audioClip.Length > 0)
            {
                int index = Random.Range(0, audioClip.Length);
                _audioSource.clip = audioClip[index];
                _audioSource.Play();
            }
        }
    }
}