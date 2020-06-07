using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a Prefab Pool. Always make a subclass so you can have different Listeners for different Prefab Pools.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Prefab Pool Listener")]
    // ReSharper disable once CheckNamespace
    public class WavePrefabPoolListener : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public string sourcePrefabPoolName;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<WavePrefabPool>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourcePrefabPoolName = name;
        }

        /// <summary>
        /// This method gets called when a Prefab gets pulled from the random weighted pool, just before it gets returned.
        /// </summary>
        /// <param name="transGrabbed">Transform of the prefab.</param>
        public virtual void PrefabGrabbedFromPool(Transform transGrabbed) {
            // your code here
        }

        /// <summary>
        /// This method gets called after all items in the pool have been used so the pool will refill, unless you've disabled refilling.
        /// </summary>
        public virtual void PoolRefilling() {
            // your code here
        }
    }
}