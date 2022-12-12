using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class MultiplayerView : View
    {
        [SerializeField]
        private Button _clientButton;
        [SerializeField]
        private Button _serverButton;
        [SerializeField]
        private Button _hostButton;
        [SerializeField]
        private Button _backButton;
        
        public override void Initialize()
        {
            _clientButton.onClick.AddListener(() =>
            {
                InstanceFinder.ClientManager.StartConnection();
            });

            _serverButton.onClick.AddListener(() =>
            {
                InstanceFinder.ServerManager.StartConnection();
            });

            _hostButton.onClick.AddListener(() =>
            {
                InstanceFinder.ServerManager.StartConnection();
                InstanceFinder.ClientManager.StartConnection();
            });

            _backButton.onClick.AddListener(() =>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            });

            base.Initialize();
        }
    }
}
