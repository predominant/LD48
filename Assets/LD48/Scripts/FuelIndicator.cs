using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.LD48.Scripts
{
    public class FuelIndicator : MonoBehaviour
    {
        [SerializeField]
        private Color ActiveColor;
        [SerializeField]
        private Color InactiveColor;
        [SerializeField]
        private Color WarningColor;
        [SerializeField]
        private TMP_Text Label;
        [SerializeField]
        private Image Background;

        
        public void SetWarning()
        {
            
        }

        public void SetOkay()
        {
            
        }
    }
}