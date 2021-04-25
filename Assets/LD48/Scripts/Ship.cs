using System;
using System.Collections;
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
        [SerializeField]
        private LayerMask _platformLayer;
        [SerializeField]
        private float _platformImpactScale = 0.5f;
        [SerializeField]
        private ParticleSystem _explosionParticles;
        [SerializeField]
        private AudioSource _explosionAudioSource;
        [SerializeField]
        private AudioSource _hurtAudioSource;
        [SerializeField]
        private GameObject _model;
        [SerializeField]
        private float _thrustTimeout = 2f;
        [SerializeField]
        private LayerMask _wallLayer;
        [SerializeField]
        private Transform[] _cheatCheckPoints;

        private int _cheatCount = 0;
        private bool _died = false;
        
        private bool ThrustTimedOut => Time.fixedTime > this._lastThrust + this._thrustTimeout;
        private bool AtRest => this._rigidbody.velocity.magnitude <= 0.1f;
        
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;

        private bool _devmodeShip = false;

        private WaitForSeconds CheatWait = new WaitForSeconds(2f);

        [SerializeField]
        private float _fuelBurnRate = 1f;
        private float _fuel = 100;
        public float Fuel
        {
            get => this._fuel;
            set
            {
                this._fuel = Mathf.Clamp(value, 0f, 100f);
                OnFuelChanged?.Invoke(value);
            }
        }

        [SerializeField]
        private float _maxImpactTolerance = 2f;
        [SerializeField]
        private float _impactScale = 1f;
        private float _health = 100;
        public float Health
        {
            get => this._health;
            set
            {
                this._health = Mathf.Clamp(value, 0f, 100f);
                OnHealthChanged?.Invoke(value);
                if (value <= 0f)
                {
                    OnShipDied?.Invoke();
                    this.Die();
                }
            }
        }

        private bool _paused = false;
        private Vector3 _prePause_angularVelocity;
        private Vector3 _prePause_velocity;
        public bool Paused
        {
            get => this._paused;
            set
            {
                this._paused = value;

                if (this._paused)
                {
                    this._prePause_angularVelocity = this._rigidbody.angularVelocity;
                    this._prePause_velocity = this._rigidbody.velocity;
                    this._rigidbody.isKinematic = true;
                }
                else
                {
                    this._rigidbody.isKinematic = false;
                    this._rigidbody.angularVelocity = this._prePause_angularVelocity;
                    this._rigidbody.velocity = this._prePause_velocity;
                }
            }
        }

        private bool _thrusting = false;
        private bool _thrustButton = false;
        private float _lastThrust = 0f;
        private float _turnValue = 0f;

        public bool Alive => this._devmodeShip || (this.Fuel > 0f && this.Health > 0f);
        
        public delegate void FuelChanged(float amount);
        public static event FuelChanged OnFuelChanged;

        public delegate void HealthChanged(float amount);
        public static event HealthChanged OnHealthChanged;

        public delegate void ShipDied();
        public static event ShipDied OnShipDied;

        private void Awake()
        {
            this._rigidbody = this.GetComponent<Rigidbody>();
            this._audioSource = this.GetComponent<AudioSource>();

            this.StartCoroutine(nameof(CheatCheck));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetButtonDown("Fire1"))
                this._thrustButton = true;
            
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetButtonUp("Fire1"))
                this._thrustButton = false;
            
            #if UNITY_EDITOR
            if (Input.GetButtonDown("Fire2"))
            {
                this._devmodeShip = !this._devmodeShip;
                Debug.Log($"Dev Mode: {this._devmodeShip}");
            }
            #endif

            this._turnValue = Input.GetAxis("Horizontal");

            // This forces player death when they have no fuel, and have finished falling.
            if (this.Fuel <= 0f && this.AtRest && this.ThrustTimedOut && this.Health > 0f)
                this.Health = 0f;
        }

        private void DoThrust()
        {
            this._thrusting = true;
            this._lastThrust = Time.fixedTime;
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
            if (this._thrustButton)
            {
                if (this.Alive)
                    this.DoThrust();
                else
                    this.StopThrust();
            }
            else
            {
                this.StopThrust();
            }
            
            if (this.Alive)
                this._rigidbody.AddTorque(Vector3.back * this._torque * this._turnValue);
            
            if (this._thrusting)
                this.Fuel -= this._fuelBurnRate * Time.fixedDeltaTime;
        }

        private void OnCollisionEnter(Collision other)
        {
            var impactMagnitude = other.relativeVelocity.magnitude;
            if (impactMagnitude < this._maxImpactTolerance)
                return;
            
            var impactScale = this._impactScale;

            if (this._platformLayer == 1 << other.gameObject.layer)
                impactScale *= this._platformImpactScale;

            this.Health -= impactScale * impactMagnitude;
            this._hurtAudioSource.Play();
        }

        private void Die()
        {
            if (this._died)
                return;

            this._died = true;
            this._explosionParticles.Play();
            this._explosionAudioSource.Play();
            this._rigidbody.isKinematic = true;
            this._model.SetActive(false);
        }

        private IEnumerator CheatCheck()
        {
            while (this.Alive)
            {
                yield return this.CheatWait;

                if (this.Cheating())
                {
                    this._cheatCount++;
                    Debug.Log("CHEATING DETECTED");
                }

                if (this._cheatCount > 2)
                {
                    this.Die();
                    yield break;
                }
            }
        }

        private bool Cheating()
        {
            bool left = false;
            bool right = false;
            
            foreach (var t in this._cheatCheckPoints)
            {
                if (Physics.Raycast(this.transform.position, t.localPosition, 50f, this._wallLayer))
                    if (t.localPosition.x < 0)
                        left = true;
                    else
                        right = true;

                if (left && right)
                    return false;
            }
            
            return true;
        }
    }
}