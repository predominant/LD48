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
        [SerializeField]
        private float _refuelRate = 50f;

        private float _expireTime = 0.3f;

        private bool _landed = false;
        private float _landingTime;

        private bool _refuelCoroutineRunning = false;

        private Ship _ship;

        private WaitForSecondsRealtime DestroyPlatformWait;
        private WaitForFixedUpdate RefuelWaitFixedUpdate = new WaitForFixedUpdate();

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
            this.StopCoroutine(nameof(DestroyPlatform));
            
            this.StartCoroutine(nameof(Refuel));
        }

        private void OnCollisionExit(Collision other)
        {
            if (this._shipLayer != 1 << other.gameObject.layer)
                return;

            if (this.TimeExpired)
                this.StartCoroutine(nameof(DestroyPlatform));
            
            this.StopCoroutine(nameof(Refuel));
            this._refuelCoroutineRunning = false;
        }

        private void OnCollisionStay(Collision other)
        {
            if (this._shipLayer != 1 << other.gameObject.layer)
                return;
            
            this.StopCoroutine(nameof(DestroyPlatform));
        }

        private IEnumerator DestroyPlatform()
        {
            var startTime = Time.fixedTime;
            while (startTime + this._destroyTime > Time.fixedTime)
                yield return new WaitForFixedUpdate();
            
            GameObject.Destroy(this.gameObject);
        }

        private IEnumerator Refuel()
        {
            this._refuelCoroutineRunning = true;

            if (this._ship == null)
                this._ship = GameObject.Find("Ship").GetComponent<Ship>();
            
            while (this._ship.Fuel < 100f)
            {
                this._ship.Fuel += this._refuelRate * Time.fixedDeltaTime;
                yield return this.RefuelWaitFixedUpdate;
            }

            this._refuelCoroutineRunning = false;
        }
    }
}