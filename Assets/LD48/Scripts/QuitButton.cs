using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class QuitButton : MonoBehaviour
    {
        private void Awake()
        {
            #if UNITY_WEBGL
            this.gameObject.SetActive(false);
            #endif
        }
    }
}