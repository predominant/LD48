using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.LD48.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text ScoreText;

        [SerializeField]
        private GameObject DeathMenu;

        [SerializeField]
        private GameObject _pauseMenu;
        
        private Transform _shipTransform;
        private Ship _ship;

        private float _scoreOffset;
        private float _currentScore;

        private bool _shipDied = false;
        
        private bool _paused = false;

        public static GameManager Instance;
        
        public bool Paused
        {
            get => this._paused;
            private set => this._paused = value;
        }

        private void Awake()
        {
            this.Init();
            FloatingOrigin.OnRepositioned += this.OnRepositioned;
            FloatingOrigin.OnWillReposition += this.OnWillReposition;
            Ship.OnShipDied += this.OnShipDied;
        }

        private void OnDestroy()
        {
            FloatingOrigin.OnRepositioned -= this.OnRepositioned;
            FloatingOrigin.OnWillReposition -= this.OnWillReposition;
            Ship.OnShipDied -= this.OnShipDied;
        }

        private void Init()
        {
            if (Instance == null)
                Instance = this;
            
            this._shipTransform = GameObject.Find("Ship").transform;
            this._ship = this._shipTransform.GetComponent<Ship>();
        }

        private void Update()
        {
            if (this._ship.Alive || !this._shipDied)
                this.UpdateScore();

            // Reset the level
            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire3"))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            // Pause
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
                this.TogglePause(!this._paused);
            
            if (this._shipDied)
            {
                if (Input.GetButton("Fire1"))
                {
                    // Retry? So you don't have to put the controller down.
                }
            }
        }
        
        private void UpdateScore()
        {
            var score = this._shipTransform.position.y * -1;

            score += this._scoreOffset;
            if (score < 0)
                score = 0;

            this._currentScore = score;
            this.ScoreText.text = Mathf.RoundToInt(score).ToString();
        }

        private void OnWillReposition()
        {
            this._scoreOffset += this._shipTransform.position.y * -1;
        }
        
        private void OnRepositioned()
        {
            
        }

        private void OnShipDied()
        {
            this._shipDied = true;
            this.StartCoroutine(this.ShowDeathMenu());
            this.StartCoroutine(
                ScoreManager.SubmitScore(
                    PlayerPrefs.GetString(ScoreManager.PlayerNameKey),
                    Mathf.RoundToInt(this._currentScore)));
        }

        private IEnumerator ShowDeathMenu()
        {
            yield return new WaitForSecondsRealtime(2f);
            this.DeathMenu.SetActive(true);
        }

        public void TogglePause(bool pause)
        {
            if (!this._ship.Alive)
                return;

            this._paused = pause;
            this._pauseMenu.SetActive(pause);
            this._ship.Paused = pause;
        }
    }
}