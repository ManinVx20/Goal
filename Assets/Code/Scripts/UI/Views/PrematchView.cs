using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StormDreams
{
    public class PrematchView : View
    {
        [SerializeField]
        private TMP_Text _countdownTimeText;

        public void SetCountdownTimeText(float time)
        {
            if (time > 0.0f)
            {
                time += 1;
            }

            int minutes = Mathf.FloorToInt(time / 60.0f);
            int seconds = Mathf.FloorToInt(time % 60.0f);

            if (minutes > 0)
            {
                _countdownTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                _countdownTimeText.text = string.Format("{0:0}", seconds);
            }
        }
    }
}
