using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a Syncro Spawner. Always make a subclass so you can have different Listeners for different Syncro Spawners.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Syncro Spawner Listener")]
    // ReSharper disable once CheckNamespace
    public class WaveSyncroSpawnerListener : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public string sourceSpawnerName = string.Empty;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<WaveSyncroPrefabSpawner>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceSpawnerName = name;
        }

        /// <summary>
        /// This method gets called when an item fails to spawn.
        /// </summary>
        /// <param name="failedPrefabTrans">Transform of the item that failed to spawn.</param>
        public virtual void ItemFailedToSpawn(Transform failedPrefabTrans) {
            // your code here. The transform is not spawned. This is just a reference
        }

        /// <summary>
        /// This method gets called immediately after an item spawns.
        /// </summary>
        /// <param name="spawnedTrans">The spawned item's Transform.</param>
        public virtual void ItemSpawned(Transform spawnedTrans) {
            // do something to the Transform.
        }

        /// <summary>
        /// This method gets called after the wave has spawned the last item.
        /// </summary>
        /// <param name="spec">The wave specifics.</param>
        public virtual void WaveFinishedSpawning(WaveSpecifics spec) {
            // Please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called when the wave is about to spawn the first item.
        /// </summary>
        /// <param name="spec">The wave specifics.</param>
        public virtual void WaveStart(WaveSpecifics spec) {
            // Please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called when an elimination wave has the last item despawned, on the first and every repeat.
        /// </summary>
        /// <param name="spec">The wave specifics.</param>
        public virtual void EliminationWaveCompleted(WaveSpecifics spec) {
            // called at the end of each wave, whether or not it is repeating. This is called before the Repeat delay
            // Please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called when a wave is about to repeat.
        /// </summary>
        /// <param name="spec">The wave specifics.</param>
        public virtual void WaveRepeat(WaveSpecifics spec) {
            // Please do not manipulate values in the "spec". It is for your read-only information
        }
    }
}