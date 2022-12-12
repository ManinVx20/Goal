using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StormDreams
{
    [System.Serializable]
    public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
    {
        public AssetReferenceAudioClip(string guid) : base(guid) { }
    }

    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance;

        [Header("Skills")]
        [SerializeField]
        private AssetLabelReference _skillReference;

        [Header("UI")]
        [SerializeField]
        private AssetReferenceGameObject _skillListItemReference;
        [SerializeField]
        private AssetReferenceGameObject _playerListItemReference;

        [Header("Characters")]
        [SerializeField]
        private AssetReferenceGameObject _dummyReference;

        [Header("Audio Clips")]
        [SerializeField]
        private AssetReferenceAudioClip _buttonClickSoundReference;

        public Dictionary<string, Skill> Skills { get; private set; }
        public GameObject SkillListItemPrefab { get; private set; }
        public GameObject PlayerListItemPrefab { get; private set; }
        public GameObject DummyPrefab { get; private set; }
        public AudioClip ButtonClickSound { get; private set; }

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

        private void Start()
        {
            LoadResources();
        }

        private void LoadResources()
        {
            Skills = new();
            Addressables.LoadAssetsAsync<Skill>(_skillReference, (skill) =>
            {
                Skills.Add(skill.Name, skill);
            });

            SkillListItemPrefab = null;
            SkillListItemPrefab = _skillListItemReference.LoadAssetAsync<GameObject>().WaitForCompletion();

            PlayerListItemPrefab = null;
            PlayerListItemPrefab = _playerListItemReference.LoadAssetAsync<GameObject>().WaitForCompletion();

            DummyPrefab = null;
            DummyPrefab = _dummyReference.LoadAssetAsync<GameObject>().WaitForCompletion();

            ButtonClickSound = null;
            ButtonClickSound = _buttonClickSoundReference.LoadAssetAsync<AudioClip>().WaitForCompletion();
        }
    }
}
