using UnityEngine;

/*! \cond PRIVATE */
// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
	public class PoolableInfo : MonoBehaviour {
		public string poolItemName = string.Empty;
		
		void OnSpawned() {
			PoolBoss.UnregisterNonStartInScenePoolable(this);
		}
		
		void OnEnable() {
			PoolBoss.RegisterPotentialInScenePoolable(this);
		}
		
		void OnDisable() {
			PoolBoss.UnregisterNonStartInScenePoolable(this);
		}
		
		void Reset() {
			if (!Application.isPlaying) {
				FindPoolItemName();
			}
		}
		
		public void FindPoolItemName() {
			if (!string.IsNullOrEmpty(poolItemName)) {
				return;
			}
			
			poolItemName = PoolBoss.GetPrefabShortName(name);
		}
		
		/// <summary>
		/// This will get called instead by other scripts if you already know the name
		/// </summary>
		/// <param name="itemName"></param>
		public void SetPoolItemName(string itemName) {
			poolItemName = itemName;
		}
		
		public string ItemName {
			get {
				if (string.IsNullOrEmpty(poolItemName)) {
					FindPoolItemName();
				}
				
				return poolItemName;
			}
		}
	}
}
/*! \endcond */
