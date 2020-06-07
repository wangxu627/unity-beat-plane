using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a Triggered Spawner. Always make a subclass so you can have different Listeners for different Triggered Spawners.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Triggered Spawner Listener")]
    // ReSharper disable once CheckNamespace
	public class TriggeredSpawnerListener : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public string sourceSpawnerName = string.Empty;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<TriggeredSpawner>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceSpawnerName = name;
        }

        /// <summary>
        /// This method gets calls when a spawner gets ready to call events on child spawners. Override if you need custom logic.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="transmitterTrans">The parent spawner's Transform.</param>
        /// <param name="receiverSpawnerCount">The count of all child spawners of the parent.</param>
		public virtual void EventPropagating(TriggeredSpawner.EventType eType, Transform transmitterTrans,
            int receiverSpawnerCount) {
            // your code here.
        }

        /// <summary>
        /// This method gets calls when a child spawner gets notified to call its wave from a parent spawner. Override if you need custom logic.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="transmitterTrans">The parent spawner's Transform.</param>
		public virtual void PropagatedEventReceived(TriggeredSpawner.EventType eType, Transform transmitterTrans) {
            // your code here. 
        }

        /// <summary>
        /// This method gets called if the wave ends early.
        /// </summary>
        /// <param name="eType">The event type.</param>
		public virtual void WaveEndedEarly(TriggeredSpawner.EventType eType) {
            // your code here. 
        }

        /// <summary>
        /// This method gets called if a parent spawner's wave ends early, to cancel the child spawners' waves as well.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="customEventName">The custom event name, if any.</param>
        /// <param name="transmitterTrans">The parent spawner's Transform.</param>
        /// <param name="receiverSpawnerCount">The count of all child spawners of the parent.</param>
		public virtual void PropagatedWaveEndedEarly(TriggeredSpawner.EventType eType, string customEventName,
            Transform transmitterTrans, int receiverSpawnerCount) {
            // your code here. 
        }

        /// <summary>
        /// This method gets called if a wave item fails to spawn.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="failedPrefabTrans">The prefab that failed to spawn.</param>
		public virtual void ItemFailedToSpawn(TriggeredSpawner.EventType eType, Transform failedPrefabTrans) {
            // your code here. The transform is not spawned. This is just a reference
        }

        /// <summary>
        /// This method gets called immediately after a wave item spawns.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="spawnedTrans">The spawned item.</param>
		public virtual void ItemSpawned(TriggeredSpawner.EventType eType, Transform spawnedTrans) {
            // do something to the Transform.
        }

        /// <summary>
        /// This method gets called after the last item in a wave has finished spawning.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="spec">The wave specifics.</param>
		public virtual void WaveFinishedSpawning(TriggeredSpawner.EventType eType, TriggeredWaveSpecifics spec) {
            // please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called after the last item in a wave has been despawned.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="spec">The wave specifics.</param>
        public virtual void WaveEliminated(TriggeredSpawner.EventType eType, TriggeredWaveSpecifics spec) {
            // please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called when the wave is about to start spawning.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="spec">The wave specifics.</param>
		public virtual void WaveStart(TriggeredSpawner.EventType eType, TriggeredWaveSpecifics spec) {
            // please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called each time the wave is about to repeat.
        /// </summary>
        /// <param name="eType">The event type.</param>
        /// <param name="spec">The wave specifics.</param>
		public virtual void WaveRepeat(TriggeredSpawner.EventType eType, TriggeredWaveSpecifics spec) {
            // please do not manipulate values in the "spec". It is for your read-only information
        }

        /// <summary>
        /// This method gets called when a spawner is about to despawn (optional).
        /// </summary>
        /// <param name="transDespawning">The Transform of the spawner.</param>
        public virtual void SpawnerDespawning(Transform transDespawning) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Custom Event is received by the Triggered Spawner. This only happens if the Triggered Spawner has configured a wave for that Custom Event.
        /// </summary>
        /// <param name="customEventName">the Custom Event name.</param>
        /// <param name="eventOrigin">The position of the Custom Event's origin (initiator).</param>
        public virtual void CustomEventReceived(string customEventName, Vector3 eventOrigin) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the Triggered Spawner is spawned.
        /// </summary>
        public virtual void Spawned(MonoBehaviour spawner) {
            // your code here, cast to TriggeredSpawnerV2 if you need it.
        }

        /// <summary>
        /// This method gets called when the Triggered Spawner is about to despawn.
        /// </summary>
        public virtual void Despawned(MonoBehaviour spawner) {
            // your code here, cast to TriggeredSpawnerV2 if you need it.
        }

    }
}