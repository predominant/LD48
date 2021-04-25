using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.LD48.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class FloatingOrigin : MonoBehaviour
    {
        public float threshold = 100.0f;
        public float physicsThreshold = 1000.0f; // Set to zero to disable
        public float defaultSleepThreshold = 0.14f;

        ParticleSystem.Particle[] parts = null;

        void LateUpdate()
        {
            var cameraPosition = this.transform.position;
            // cameraPosition.z = 0f;

            if (cameraPosition.magnitude > threshold)
            {
                Debug.Log("Floating Origin - Repositioning");
                foreach (var g in SceneManager.GetActiveScene().GetRootGameObjects())
                    g.transform.position -= cameraPosition;

                var objects = FindObjectsOfType(typeof(ParticleSystem));
                foreach (var o in objects)
                {
                    var sys = (ParticleSystem)o;

                    if (sys.main.simulationSpace != ParticleSystemSimulationSpace.World)
                        continue;

                    int particlesNeeded = sys.main.maxParticles;

                    if (particlesNeeded <= 0)
                        continue;

                    var wasPaused = sys.isPaused;
                    var wasPlaying = sys.isPlaying;

                    if (!wasPaused)
                        sys.Pause();

                    // ensure a sufficiently large array in which to store the particles
                    if (parts == null || parts.Length < particlesNeeded)
                        parts = new ParticleSystem.Particle[particlesNeeded];

                    // now get the particles
                    int num = sys.GetParticles(parts);

                    for (int i = 0; i < num; i++)
                        parts[i].position -= cameraPosition;

                    sys.SetParticles(parts, num);

                    if (wasPlaying)
                        sys.Play();
                }

                if (physicsThreshold > 0f)
                {
                    var physicsThreshold2 = physicsThreshold * physicsThreshold; // simplify check on threshold
                    objects = FindObjectsOfType(typeof(Rigidbody));
                    foreach (var o in objects)
                    {
                        var r = (Rigidbody)o;
                        if (r.gameObject.transform.position.sqrMagnitude > physicsThreshold2)
                            r.sleepThreshold = float.MaxValue;
                        else
                            r.sleepThreshold = defaultSleepThreshold;
                    }
                }
            }
        }
    }
}