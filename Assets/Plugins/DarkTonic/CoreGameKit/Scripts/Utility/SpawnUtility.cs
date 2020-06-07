using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used as a wrapper for Pool Boss, but if you prefer to use Pool Manager, you can change that hookup here.
    /// </summary>
    // ReSharper disable once CheckNamespace
    public static class SpawnUtility {
		/// <summary>
		/// Call this method to damage all instances of a prefab using Pool Boss. 
		/// </summary>
		/// <param name="transToDamage">Transform to damage</param>
		/// <param name="damagePoints">How many points of damage to deal to each</param>
		public static void DamageAllOfPrefab(Transform transToDamage, int damagePoints) {
			PoolBoss.DamageAllOfPrefab(transToDamage, damagePoints);
		}

		/// <summary>
        /// Call this method to despawn all instances of a prefab using Pool Boss. 
        /// </summary>
        /// <param name="transToDespawn">Transform to despawn</param>
        public static void DespawnAllOfPrefab(Transform transToDespawn) {
            PoolBoss.DespawnAllOfPrefab(transToDespawn);
        }

        /// <summary>
        /// Call this method to kill all instances of a prefab using Pool Boss. Only prefabs with a Killable component can be killed.  
        /// </summary>
        /// <param name="transToKill">Transform to kill</param>
        public static void KillAllOfPrefab(Transform transToKill) {
            PoolBoss.KillAllOfPrefab(transToKill);
        }

        /// <summary>
        /// Call this method to despawn all instances of all prefabs that use Pool Boss. 
        /// </summary>
        public static void DespawnAllPrefabs() {
            PoolBoss.DespawnAllPrefabs();
        }

		/// <summary>
		/// Call this method to damage all instances of a prefab using Pool Boss. 
		/// </summary>
		/// <param name="damagePoints">How many points of damage to deal to each</param>
		public static void DamageAllPrefabs(int damagePoints) {
			PoolBoss.DamageAllPrefabs(damagePoints);
		}

        /// <summary>
        /// Call this method to kill all instances of all prefabs that use Pool Boss. Only prefabs with a Killable component can be killed.  
        /// </summary>
        public static void KillAllPrefabs() {
            PoolBoss.KillAllPrefabs();
        }

        /// <summary>
        /// Call this method to despawn all instances of all prefabs in a certain Pool Boss category. 
        /// </summary>
		/// <param name="category">Category to affect</param>
		public static void DespawnAllPrefabsInCategory(string category) {
            PoolBoss.DespawnAllPrefabsInCategory(category);
        }

		/// <summary>
		/// Call this method to damage all instances of all prefabs in a certain Pool Boss category. Only prefabs with a Killable component can be damaged.  
		/// </summary>
		/// <param name="category">Category to affect</param>
		/// <param name="category">Amount of damage to deal to each</param>
		public static void DamageAllPrefabsInCategory(string category, int damagePoints) {
			PoolBoss.DamageAllPrefabsInCategory(category, damagePoints);
		}

        /// <summary>
        /// Call this method to kill all instances of all prefabs in a certain Pool Boss category. Only prefabs with a Killable component can be killed.  
        /// </summary>
		/// <param name="category">Category to affect</param>
		public static void KillAllPrefabsInCategory(string category) {
            PoolBoss.KillAllPrefabsInCategory(category);
        }

        /*! \cond PRIVATE */
        public static bool SpawnedMembersAreAllBeyondDistance(Transform spawnerTrans, List<Transform> members,
            float minDist) {
            var allMembersBeyondDistance = true;

            var spawnerPos = spawnerTrans.position;
            var sqrDist = minDist * minDist;

            foreach (var t in members) {
                if (t == null || !IsActive(t.gameObject)) {
                    // .active will work with Pool Manager.
                    continue;
                }

                if (Vector3.SqrMagnitude(spawnerPos - t.transform.position) < sqrDist) {
                    allMembersBeyondDistance = false;
                }
            }

            return allMembersBeyondDistance;
        }

        public static void RecordSpawnerObjectIfKillable(Transform spawnedObject, GameObject spawnerObject) {
            var spawnedKill = spawnedObject.GetComponent<Killable>();
            if (spawnedKill != null) {
                spawnedKill.RecordSpawner(spawnerObject);
            }
        }
        /*! \endcond */

        /// <summary>
        /// This method will tell you if a GameObject is either despawned or destroyed.
        /// </summary>
        /// <param name="objectToCheck">The GameObject you're asking about.</param>
        /// <returns>True or false</returns>
        public static bool IsDespawnedOrDestroyed(GameObject objectToCheck) {
            return objectToCheck == null || !IsActive(objectToCheck);
        }

        /// <summary>
        /// This is a cross-Unity-version method to tell you if a GameObject is active in the Scene.
        /// </summary>
        /// <param name="go">The GameObject you're asking about.</param>
        /// <returns>True or false</returns>
        public static bool IsActive(GameObject go) {
            return go.activeSelf;
        }

        /// <summary>
        /// This is a cross-Unity-version method to set a GameObject to active in the Scene.
        /// </summary>
        /// <param name="go">The GameObject you're setting to active or inactive</param>
        /// <param name="isActive">True to set the object to active, false to set it to inactive.</param>
        public static void SetActive(GameObject go, bool isActive) {
            go.SetActive(isActive);
        }
    }
}