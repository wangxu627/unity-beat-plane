using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a Triggered Despawner. Always make a subclass so you can have different Listeners for different Triggered Despawners.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Triggered Despawner Listener")]
    // ReSharper disable once CheckNamespace
    public class TriggeredDespawnerListener : MonoBehaviour {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public string sourceDespawnerName;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<TriggeredDespawner>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceDespawnerName = name;
        }

        /// <summary>
        /// This method gets called when the Triggered Despawner is about to despawn its game object.
        /// </summary>
        /// <param name="eType">The event type triggering the despawn.</param>
        /// <param name="transDespawning">The Transform about to despawn.</param>
		public virtual void Despawning(TriggeredSpawner.EventType eType, Transform transDespawning) {
            // Your code here.
        }
    }
}