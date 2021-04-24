using System;
using System.Collections;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class Platform : MonoBehaviour
    {
        public bool Starter = false;
        [SerializeField]
        private LayerMask _shipLayer;
        [SerializeField]
        private float _destroyTime = 0.25f;

        private float _expireTime = 0.3f;

        private bool _landed = false;
        private float _landingTime;

        private WaitForSecondsRealtime DestroyPlatformWait;

        private bool TimeExpired => this._landed && Time.time > (this._landingTime + this._expireTime);

        private void Awake()
        {
            this.DestroyPlatformWait = new WaitForSecondsRealtime(this._destroyTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (this._shipLayer != 1 << other.gameObject.layer)
                return;

            this._landed = true;
        }

        private void OnCollisionExit(Collision other)
        {
            if (this._shipLayer != 1 << other.gameObject.layer)
                return;

            if (this.TimeExpired)
                this.StartCoroutine(this.DestroyPlatform());
        }

        private IEnumerator DestroyPlatform()
        {
            yield return this.DestroyPlatformWait;
            GameObject.Destroy(this.gameObject);
        }
    }
}