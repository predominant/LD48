using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform Ship;

        private void Awake()
        {
            if (this.Ship == null)
                this.Ship = GameObject.Find("Ship").transform;
        }

        private void Update()
        {
            var pos = this.Ship.position;
            pos.z = this.transform.position.z;
            this.transform.position = pos;
        }
    }
}