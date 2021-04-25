using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.LD48.Scripts
{
    public class EndScreenPanel : MonoBehaviour
    {
        public GameObject RetryButton;
        
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(this.RetryButton);
        }
    }
}