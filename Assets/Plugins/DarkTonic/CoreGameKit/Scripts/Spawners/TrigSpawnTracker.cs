/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    // ReSharper disable once CheckNamespace
    public class TrigSpawnTracker : MonoBehaviour {
        private Transform _trans;
        private TriggeredSpawnerV2 _sourceSpawner;
        private TriggeredSpawner.EventType _waveEventType;
        private TriggeredWaveMetaData _waveMeta;
        private string _customEventName;

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDisable() {
            if (_sourceSpawner == null || _waveMeta == null) {
                return;
            }

            _sourceSpawner.RemoveItemFromWave(Trans, _waveEventType, _customEventName, _waveMeta);

            _sourceSpawner = null;
            _waveMeta = null;
            _customEventName = null;
        }

        public void StartTracking(TriggeredSpawnerV2 sourceSpawner, TriggeredSpawner.EventType eType, string customEventName, TriggeredWaveMetaData waveMeta) {
            _sourceSpawner = sourceSpawner;
            _waveEventType = eType;
            _customEventName = customEventName;
            _waveMeta = waveMeta;
        }

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