using UnityEngine;

namespace RPG.Core
{
    public class SoundRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] audioClip;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
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