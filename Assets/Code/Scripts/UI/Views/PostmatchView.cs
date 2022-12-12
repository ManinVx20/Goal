using System.Collections;
using System.Collections.Generic;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class PostmatchView : View
    {
        [SerializeField]
        private TMP_Text _finalScorePlayerOneText;
        [SerializeField]
        private TMP_Text _finalScorePlayerTwoText;
        [SerializeField]
        private Button _exitButton;

        public override void Initialize()
        {
            _exitButton.onClick.AddListener(() =>
            {
                if (InstanceFinder.IsClient)
                {
                    InstanceFinder.ClientManager.StopConnection();
                }
                
                if (InstanceFinder.IsServer)
                {
                    InstanceFinder.ServerManager.StopConnection(true);
                }
            });

            base.Initialize();
        }

        public void SetFinalScoresText(int scorePlayerOne, int scorePlayerTwo)
        {
            _finalScorePlayerOneText.text = scorePlayerOne.ToString();
            _finalScorePlayerTwoText.text = scorePlayerTwo.ToString();
        }
    }
}
