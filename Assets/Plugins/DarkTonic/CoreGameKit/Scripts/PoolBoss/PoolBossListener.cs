using UnityEngine;
using System.Collections;

namespace DarkTonic.CoreGameKit {
	/// <summary>
	/// This class is used to listen to key events in Pool Boss. 
	/// </summary>
	public class PoolBossListener : MonoBehaviour {
		/*! \cond PRIVATE */
		// ReSharper disable InconsistentNaming
		public string sourceTransName;
		// ReSharper restore InconsistentNaming
		/*! \endcond */
		
		// ReSharper disable once UnusedMember.Local
		private void Reset() {
			var src = GetComponent<PoolBoss>();
			if (src == null) {
				return;
			}
			src.listener = this;
			sourceTransName = name;
		}

		/// <summary>
		/// This method is called every time another item is initialized - use Initialize Time (Frames) field set to greater than 1 for this to mean anything.
		/// </summary>
		/// <param name="percentDone">Percent done.</param>
		public virtual void PercentInitialized(float percentDone) {
			// add code here
		}

		/// <summary>
		/// This is called when all items are ready and initialized.
		/// </summary>
		public virtual void InitializationComplete() {
			// add code here
		}

		/// <summary>
		/// This is called every time an item is spawned.
		/// </summary>
		/// <param name="cloneSpawned">Clone spawned.</param>
		public virtual void ItemSpawned(Transform cloneSpawned) {
			// add code here
		}

		/// <summary>
		/// This is called every time an item is despawned.
		/// </summary>
		/// <param name="transDespawned">Trans despawned.</param>
		public virtual void ItemDespawned(Transform transDespawned) {
			// add code here
		}
	}
}