using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class HighscorePanel : MonoBehaviour
    {
        public GameObject ScoreRowPanelPrefab;
        public Transform ScoreListPanel;
        
        private void Awake()
        {
            ScoreManager.OnScoresUpdated += this.OnScoresUpdated;
        }
        
        private void OnEnable()
        {
            this.StartCoroutine(ScoreManager.GetTopScores());
        }

        private void ClearScores()
        {
            try
            {
                foreach (Transform t in this.ScoreListPanel)
                    GameObject.Destroy(t.gameObject);
            }
            catch (Exception e)
            {
                Debug.Log($"Exception: {e}");
            }
        }

        private void OnScoresUpdated(Dictionary<string, int> scores)
        {
            this.ClearScores();
            
            foreach (var key in scores.Keys)
            {
                var row = GameObject.Instantiate(this.ScoreRowPanelPrefab, this.ScoreListPanel, false);
                row.GetComponent<HighscoreRow>().UpdateData(key, scores[key]);
            }
        }
    }
}