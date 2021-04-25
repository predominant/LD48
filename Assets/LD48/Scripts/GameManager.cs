using System;
using TMPro;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text ScoreText;

        private Transform _shipTransform;
        private Ship _ship;

        private void Awake()
        {
            this.Init();
        }

        private void Init()
        {
            this._shipTransform = GameObject.Find("Ship").transform;
            this._ship = this._shipTransform.GetComponent<Ship>();
        }

        private void Update()
        {
            if (this._ship.Alive)
                this.UpdateScore();
        }
        
        private void UpdateScore()
        {
            var score = this._shipTransform.position.y * -1;
            if (score < 0)
                score = 0;
            this.ScoreText.text = Mathf.RoundToInt(score).ToString();
        }
    }
}