using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a TimedDespawner. Always make a subclass so you can have different Listeners for different TimedDespawners.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Timed Despawner Listener")]
    // ReSharper disable once CheckNamespace
    public class TimedDespawnerListener : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public string sourceDespawnerName;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<TimedDespawner>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceDespawnerName = name;
        }

        /// <summary>
        /// This method gets called when the Timed Despawner is about to despawn its game object.
        /// </summary>
        /// <param name="transDespawning">The Transform about to despawn.</param>
        public virtual void Despawning(Transform transDespawning) {
            // Your code here.
        }
    }
}