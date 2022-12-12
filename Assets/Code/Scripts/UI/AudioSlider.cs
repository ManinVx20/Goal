using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class AudioSlider : MonoBehaviour
    {
        [System.Serializable]
        private enum AudioSliderType
        {
            Music,
            Sound
        }

        [SerializeField]
        private AudioSliderType _type;
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Sprite _offSprite;
        [SerializeField]
        private Sprite _onSprite;

        private void Awake()
        {
            _slider.onValueChanged.AddListener((value) =>
            {
                SetAudioVolume(value);
            });
        }

        private void Start()
        {
            SetAudioVolume(_slider.value);
        }

        private void SetAudioVolume(float value)
        {
            value /= _slider.maxValue;

            if (value == 0)
            {
                _image.sprite = _offSprite;
            }
            else
            {
                _image.sprite = _onSprite;
            }

            switch (_type)
            {
                case AudioSliderType.Music:
                    AudioManager.Instance.ChangeMusicVolume(value);
                    break;
                case AudioSliderType.Sound:
                    AudioManager.Instance.ChangeSoundVolume(value);
                    break;
            }
        }
    }
}
