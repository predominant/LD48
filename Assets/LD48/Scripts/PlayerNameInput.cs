using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class PlayerNameInput : MonoBehaviour
    {
        private TMP_InputField _input;
        
        private void Awake()
        {
            this._input = this.GetComponent<TMP_InputField>();
            this.UpdatePlayerText();
        }

        private void UpdatePlayerText()
        {
            this._input.text = ScoreManager.PlayerName();
        }

        public void StorePlayerName(string n)
        {
            var r = new Regex("[^a-zA-Z0-9 _-]");
            ScoreManager.UpdatePlayerName(r.Replace(n.ToUpper(), ""));
            this.UpdatePlayerText();
        }
    }
}