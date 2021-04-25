using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class Section : MonoBehaviour
    {
        public int Height;
        public LevelGenerator Generator;
        
        private void LateUpdate()
        {
            if (this.Generator == null)
                return;

            if (this.transform.position.y > this.Generator.Height * 2f)
            {
                Debug.Log($"Section {this.Height} moved out of range, disposing.");
                this.Generator.RemoveSection(this.Height);
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}