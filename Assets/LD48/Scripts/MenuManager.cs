using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.LD48.Scripts
{
    public class MenuManager : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Game");
        }

        public void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }

        public void HighScores()
        {
        }
    }
}