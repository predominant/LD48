using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class FlashingLight : MonoBehaviour
    {
        public Material Material1;
        public Material Material2;
        public float Speed = 5f;
        public float Intensity = 6.4f;
        
        private Renderer _renderer;
        
        
        private void Awake()
        {
            this._renderer = this.GetComponent<Renderer>();
        }

        private void Update()
        {
            this._renderer.material.SetColor(
                "_EmissionColor",
                Color.Lerp(
                    this.Material1.GetColor("_EmissionColor"),
                    this.Material2.GetColor("_EmissionColor"),
                    Mathf.Sin(Time.time * this.Speed) / 2f + 0.5f));
        }
    }
}