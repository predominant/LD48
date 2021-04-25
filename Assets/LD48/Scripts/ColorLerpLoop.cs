using TMPro;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class ColorLerpLoop : MonoBehaviour
    {
        public Color[] Colors;
        public TMP_Text Text;
        public float Speed = 1f;
        
        private void Update()
        {
            this.Text.color = Color.Lerp(this.Colors[0], this.Colors[1], Mathf.Sin(Time.time * this.Speed) / 2f + 0.5f);
        }
    }
}