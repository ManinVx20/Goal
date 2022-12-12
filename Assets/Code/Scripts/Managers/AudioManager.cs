using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormDreams
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Core")]
        [SerializeField]
        private AudioSource _musicSource;
        [SerializeField]
        private AudioSource _soundSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ChangeMusicVolume(float value)
        {
            _musicSource.volume = value;
        }

        public void ChangeSoundVolume(float value)
        {
            _soundSource.volume = value;
        }

        public void PlaySound(AudioClip clip)
        {
            _soundSource.PlayOneShot(clip);
        }
    }
}
