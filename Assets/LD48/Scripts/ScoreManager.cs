using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.LD48.Scripts
{
    public class ScoreManager : MonoBehaviour
    {
        #region Secrets
        public static string DreamloURLPrivate = "https://dreamlo-proxy.grahamweldon.com/lb/1ARSDY58mkyqggN7ij0ZqgCUoKj3MdiUy85Kaqf6ssmQ";
        public static string DreamloURLPublic = "https://dreamlo-proxy.grahamweldon.com/lb/60850c468f40bb12282b1bea";
        // Private Code: 1ARSDY58mkyqggN7ij0ZqgCUoKj3MdiUy85Kaqf6ssmQ
        // Public Code: 60850c468f40bb12282b1bea
        #endregion

        // NOTE: There needs to be at least 2 entries in the system, otherwise an array is not returned.
        // So, be sure to add at least 2 entries if the leaderboard is cleared.
        
        public static readonly string PlayerNameKey = "_player_name";
        public static readonly string PlayerScoreKey = "_player_score";

        public static Dictionary<string, int> Scores = new Dictionary<string, int>();
        
        public delegate void ScoresUpdated(Dictionary<string, int> scores);
        public static event ScoresUpdated OnScoresUpdated;

        public static string PlayerName()
        {
            return PlayerPrefs.GetString(PlayerNameKey, "PLAYER NAME");
        }

        public static void UpdatePlayerName(string name)
        {
            PlayerPrefs.SetString(PlayerNameKey, name);
        }

        public int BestScore()
        {
            return PlayerPrefs.GetInt($"{PlayerScoreKey}_{name}", 0);
        }

        public static IEnumerator SubmitScore(string name, int score)
        {
            var request = UnityWebRequest.Get($"{DreamloURLPrivate}/add/{name}/{score}");
            yield return request.SendWebRequest();

            var bestScore = PlayerPrefs.GetInt($"{PlayerScoreKey}_{name}", 0);
            if (score > bestScore)
                PlayerPrefs.SetInt($"{PlayerScoreKey}_{name}", score);
        }
        
        public static IEnumerator GetTopScores()
        {
            var request = UnityWebRequest.Get($"{DreamloURLPublic}/json/10");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"Error in web request: {request.error}");
                yield break;
            }

            Scores = new Dictionary<string, int>();
            
            var data = JObject.Parse(request.downloadHandler.text);
            foreach (var entry in data["dreamlo"]["leaderboard"]["entry"])
                Scores.Add(entry["name"].ToString(), int.Parse(entry["score"].ToString()));
            
            OnScoresUpdated?.Invoke(Scores);
        }
    }
}