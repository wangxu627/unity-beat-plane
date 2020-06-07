using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a Killable. Always make a subclass so you can have different Listeners for different Killables.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Killable Listener")]
    // ReSharper disable once CheckNamespace
    public class KillableListener : MonoBehaviour {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public string sourceKillableName;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<Killable>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceKillableName = name;
        }

        /// <summary>
        /// This method gets called when the Killable's spawner gets destroyed.
        /// </summary>
        public virtual void SpawnerDestroyed() {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the Killable is about to despawn (before Despawned). Will not get automatically called by PoolBoss.Despawn.
        /// </summary>
        public virtual void Despawning(TriggeredSpawner.EventType eType) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the Killable's spawner is despawning (after Despawning).
        /// </summary>
        public virtual void Despawned() {
            // your code here
        }

        /// <summary>
        /// This method gets called when the Killable is taking damage.
        /// </summary>
        public virtual void TakingDamage(int pointsDamage, Killable enemyHitBy) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the Killable is prevented taking damage by invicibility.
        /// </summary>
        public virtual void DamagePrevented(int pointsDamage, Killable enemyHitBy) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when damage prefabs are spawned.
        /// </summary>
        public virtual void DamagePrefabSpawned(Transform damagePrefab) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when damage prefabs fail to spawn.
        /// </summary>
        public virtual void DamagePrefabFailedToSpawn(Transform damagePrefab) {
            // your code here.  
        }

        /// <summary>
        /// This method gets called when the delay before death begins.
        /// </summary>
        public virtual void DeathDelayStarted(float delayTime) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when death prefabs are spawned.
        /// </summary>
        public virtual void DeathPrefabSpawned(Transform deathPrefab) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when death prefabs fail to spawn.
        /// </summary>
        public virtual void DeathPrefabFailedToSpawn(Transform deathPrefab) {
            // your code here.  
        }

        /// <summary>
        /// This method gets called when Damage World Variables are about to be modified. You can change the modifiers here if you like, permanently.
        /// </summary>
        public virtual void ModifyingDamageWorldVariables(List<WorldVariableModifier> variableModifiers) {
            // your code here. You can change the variable modifiers before they get used if you want.
        }

        /// <summary>
        /// This method gets called when Death World Variables are about to be modified. You can change the modifiers here if you like, permanently.
        /// </summary>
        public virtual void ModifyingDeathWorldVariables(List<WorldVariableModifier> variableModifiers) {
            // your code here. You can change the variable modifiers before they get used if you want.
        }

        /// <summary>
        /// This method gets called when the death timer starts.
        /// </summary>
        public virtual void WaitingToDestroyKillable(Killable deadKillable) {
            // your code here;
        }

        /// <summary>
        /// This method gets called when the Killable is destroyed.
        /// </summary>
        public virtual void DestroyingKillable(Killable deadKillable) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the Scenario is about to be decided. You can add logic to decide the Scenario here.
        /// </summary>
        public virtual string DeterminingScenario(Killable deadKillable, string scenario) {
            // if you wish to use logic to change the Scenario, do it here. Example below.

            // if (yourLogicHere == true) {
            //   scenario = "ReachedTower";
            // }

            return scenario;
        }

        /// <summary>
        /// This method gets called when the Killable is spawned.
        /// </summary>
        public virtual void Spawned(Killable newKillable) {
            // your code here
        }

		/// <summary>
		/// This method gets called when the Killable starts already in a Scene and Spawned would not be called.
		/// </summary>
		public virtual void StartedInScene(Killable newKillable) {
			// your code here
		}
	}
}