using TMPro;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class HighscoreRow : MonoBehaviour
    {
        public TMP_Text NameText;
        public TMP_Text ScoreText;

        public void UpdateData(string name, int score)
        {
            this.NameText.text = name;
            this.ScoreText.text = score.ToString("#,0");
        }
    }
}