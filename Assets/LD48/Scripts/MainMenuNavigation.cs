using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.LD48.Scripts
{
    public class MainMenuNavigation : MonoBehaviour
    {
        public Button PlayButton;
        
        private void Awake()
        {
            // this.PlayButton.Select();
            // this.PlayButton.OnSelect(null);

            // EventSystem.current.SetSelectedGameObject(null);
            // EventSystem.current.SetSelectedGameObject(this.PlayButton.gameObject);
        }
    }
}