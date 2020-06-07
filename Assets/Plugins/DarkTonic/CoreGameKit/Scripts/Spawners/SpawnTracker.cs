/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    // ReSharper disable once CheckNamespace
    public class SpawnTracker : MonoBehaviour {
        private Transform _trans;

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDisable() {
            if (SourceSpawner == null) {
                return;
            }

            SourceSpawner.RemoveSpawnedMember(Trans);
            SourceSpawner = null;
        }

        public WaveSyncroPrefabSpawner SourceSpawner { get; set; }

        public Transform Trans {
            get {
                if (_trans != null) {
                    return _trans;
                }

                _trans = transform;

                return _trans;
            }
        }
    }
}
/*! \endcond */