using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.LD48.Scripts
{
    public class UIIndicator : MonoBehaviour
    {
        [SerializeField]
        protected Color ActiveColor;
        [SerializeField]
        protected Color InactiveColor;
        [SerializeField]
        protected Color WarningColor;
        [SerializeField]
        protected TMP_Text Label;
        [SerializeField]
        protected Image Background;
        
        protected Image this[int index] => this.transform.GetChild(index).GetComponent<Image>();
        
        protected void UpdateIndicator(float amount)
        {
            for (var i = 0; i < 10; i++)
            {
                if (amount >= i * 10f)
                    this[i].color = this.ActiveColor;
                else
                    this[i].color = this.InactiveColor;
            }

            if (amount < 30)
                this.SetWarning();
            else
                this.ClearWarning();
        }

        private void SetWarning()
        {
            this.Label.color = this.WarningColor;
            this.Background.color = this.WarningColor;
        }

        private void ClearWarning()
        {
            this.Label.color = this.ActiveColor;
            this.Background.color = this.ActiveColor;
        }
    }
}