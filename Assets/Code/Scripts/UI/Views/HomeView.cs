using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class HomeView : View
    {
        [SerializeField]
        private Button _trainButton;
        [SerializeField]
        private Button _challengeButton;
        [SerializeField]
        private Button _settingsButton;
        [SerializeField]
        private Button _profileButton;
        [SerializeField]
        private Button _quitButton;

        public override void Initialize()
        {
            _trainButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySound(ResourceManager.Instance.ButtonClickSound);
            });

            _challengeButton.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            });

            _settingsButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.Show<SettingsView>();
            });

            _profileButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.Show<ProfileView>();
            });

            _quitButton.onClick.AddListener(() =>
            {
                Application.Quit();
                Debug.Log("You have quit the game");
            });

            base.Initialize();
        }
    }
}
