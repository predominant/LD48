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

            if (this.transform.localPosition.y > this.Generator.Height * 3f)
            {
                this.Generator.RemoveSection(this.Height);
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}