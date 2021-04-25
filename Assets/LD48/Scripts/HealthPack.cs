using System;
using System.Collections;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class HealthPack : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _shipLayer;
        private AudioSource _audioSource;

        private Ship _ship;
        private bool _activated = false;

        private void Awake()
        {
            this._audioSource = this.GetComponent<AudioSource>();
            
            if (this._ship == null)
                this._ship = GameObject.FindObjectOfType<Ship>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this._activated || this._shipLayer != 1 << other.gameObject.layer)
                return;

            Debug.Log("Ship entered my collider");
            
            this._activated = true;
            this._ship.Health = 100f;
            this._audioSource.Play();
            this.StartCoroutine(this.Hide());
        }

        private IEnumerator Hide()
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(1f);
            GameObject.Destroy(this.gameObject);
        }
    }
}