using System;
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

        private void Awake()
        {
            Ship.OnFuelChanged += this.UpdateFuel;
        }

        private void OnDestroy()
        {
            Ship.OnFuelChanged -= this.UpdateFuel;
        }

        private void UpdateFuel(float amount)
        {
            for (var i = 0; i < 10; i++)
            {
                if (amount >= i * 10f)
                    this[i].color = this.ActiveColor;
                else
                    this[i].color = this.InactiveColor;
            }
        }

        private Image this[int index]
        {
            get => this.transform.GetChild(index).GetComponent<Image>();
        }

        public void SetWarning()
        {
            
        }

        public void SetOkay()
        {
            
        }
    }
}