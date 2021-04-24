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

        [SerializeField]
        private float _fuelBurnRate = 1f;
        private float _fuel = 100;
        private float Fuel
        {
            get => this._fuel;
            set
            {
                this._fuel = Mathf.Clamp(value, 0f, 100f);
                OnFuelChanged?.Invoke(value);
            }
        }

        [SerializeField]
        private float _impactScale = 1f;
        private float _health = 100;
        private float Health
        {
            get => this._health;
            set
            {
                this._health = Mathf.Clamp(value, 0f, 100f);
                OnHealthChanged?.Invoke(value);
            }
        }

        private bool _thrusting = false;
        
        public delegate void FuelChanged(float amount);
        public static event FuelChanged OnFuelChanged;

        public delegate void HealthChanged(float amount);
        public static event HealthChanged OnHealthChanged;

        private void Awake()
        {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._audioSource = this.GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetButton("Fire1"))
            {
                if (this.Fuel > 0f)
                    this.DoThrust();
                else
                    this.StopThrust();
            }
            else
            {
                this.StopThrust();
            }

            var turn = Input.GetAxis("Horizontal");
            this._rigidbody.AddTorque(Vector3.back * this._torque * turn);
            
            // if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            //     this._rigidbody.AddTorque(Vector3.forward * this._torque);
            //
            // if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            //     this._rigidbody.AddTorque(Vector3.back * this._torque);
        }

        private void DoThrust()
        {
            this._thrusting = true;
            this._rigidbody.AddForce(this.transform.up * this._thrustForce);
            foreach (var p in this.ThrustParticles)
                if (p.isStopped)
                    p.Play();

            if (!this._audioSource.isPlaying)
                this._audioSource.Play();
        }

        private void StopThrust()
        {
            this._thrusting = false;
            foreach (var p in this.ThrustParticles)
                if (p.isPlaying)
                    p.Stop();

            if (this._audioSource.isPlaying)
                this._audioSource.Stop();
        }

        private void FixedUpdate()
        {
            if (this._thrusting)
                this.Fuel -= this._fuelBurnRate * Time.fixedDeltaTime;
        }
    }
}