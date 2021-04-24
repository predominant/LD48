using System;
using UnityEngine;

namespace Assets.LD48.Scripts
{
    public class Ship : MonoBehaviour
    {
        [SerializeField]
        private float _thrustForce = 600f;
        [SerializeField]
        private float _torque = 10f;
        [SerializeField]
        private ParticleSystem[] ThrustParticles;
        
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._audioSource = this.GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetButton("Fire1"))
            {
                this._rigidbody.AddForce(this.transform.up * this._thrustForce);
                foreach (var p in this.ThrustParticles)
                    if (p.isStopped)
                        p.Play();
                
                if (!this._audioSource.isPlaying)
                    this._audioSource.Play();
            }
            else
            {
                foreach (var p in this.ThrustParticles)
                    if (p.isPlaying)
                        p.Stop();

                if (this._audioSource.isPlaying)
                    this._audioSource.Stop();
            }

            var turn = Input.GetAxis("Horizontal");
            this._rigidbody.AddTorque(Vector3.back * this._torque * turn);
            
            // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            //     this._rigidbody.AddTorque(Vector3.forward * this._torque);
            //
            // if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            //     this._rigidbody.AddTorque(Vector3.back * this._torque);
        }
    }
}