using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StormDreams
{
    public class MatchView : View
    {
        [SerializeField]
        private TMP_Text _matchTimeText;
        [SerializeField]
        private TMP_Text _scorePlayerOneText;
        [SerializeField]
        private TMP_Text _scorePlayerTwoText;

        public void SetMatchTimeText(float time)
        {
            if (time > 0.0f)
            {
                time += 1;
            }

            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(time % 60.0f);

            _matchTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void SetScoresText(int scorePlayerOne, int scorePlayerTwo)
        {
            _scorePlayerOneText.text = scorePlayerOne.ToString();
            _scorePlayerTwoText.text = scorePlayerTwo.ToString();
        }
    }
}
