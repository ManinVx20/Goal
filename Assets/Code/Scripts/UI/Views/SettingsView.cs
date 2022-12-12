using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class SettingsView : View
    {
        [SerializeField]
        private Button _backButton;

        public override void Initialize()
        {
            _backButton.onClick.AddListener(() =>
            {
                ViewManager.Instance.Show<HomeView>();
            });

            base.Initialize();
        }
    }
}
