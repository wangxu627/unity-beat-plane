/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Despawners/Particle Despawner")]
    [RequireComponent(typeof(ParticleSystem))]
    // ReSharper disable once CheckNamespace
    public class ParticleDespawner : MonoBehaviour {
        private ParticleSystem _particles;
        private Transform _trans;

        // Update is called once per frame
        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _trans = transform;
            _particles = GetComponent<ParticleSystem>();
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            if (!_particles.IsAlive()) {
                PoolBoss.Despawn(_trans);
            }
        }
    }
}
/*! \endcond */