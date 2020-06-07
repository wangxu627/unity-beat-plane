/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Despawners/Triggered Despawner")]
    // ReSharper disable once CheckNamespace
    public class TriggeredDespawner : MonoBehaviour {
        private Transform _trans;

        // ReSharper disable InconsistentNaming
        public EventDespawnSpecifics invisibleSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics mouseOverSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics mouseClickSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics collisionSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics triggerEnterSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics triggerExitSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics collision2dSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics triggerEnter2dSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics triggerExit2dSpec = new EventDespawnSpecifics();
        public EventDespawnSpecifics onClickSpec = new EventDespawnSpecifics();
        public TriggeredDespawnerListener listener;
        // ReSharper restore InconsistentNaming

        private bool _isDespawning;

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _trans = transform;
            SpawnedOrAwake();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnSpawned() {
            SpawnedOrAwake();
        }

        protected virtual void SpawnedOrAwake() {
            _isDespawning = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnBecameInvisible() {
            if (invisibleSpec.eventEnabled) {
				Despawn(TriggeredSpawner.EventType.Invisible);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnMouseEnter() {
            if (mouseOverSpec.eventEnabled) {
				Despawn(TriggeredSpawner.EventType.MouseOver_Legacy);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnMouseDown() {
            if (mouseClickSpec.eventEnabled) {
				Despawn(TriggeredSpawner.EventType.MouseClick_Legacy);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnClick() {
            if (onClickSpec.eventEnabled) {
				Despawn(TriggeredSpawner.EventType.OnClick_NGUI);
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter(Collision collision) {
            // check filters for matches if turned on
            if (!collisionSpec.eventEnabled) {
                return;
            }

            if (collisionSpec.useLayerFilter && !collisionSpec.matchingLayers.Contains(collision.gameObject.layer)) {
                return;
            }

            if (collisionSpec.useTagFilter && !collisionSpec.matchingTags.Contains(collision.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnCollision);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerEnter(Collider other) {
            // check filters for matches if turned on
            if (!triggerEnterSpec.eventEnabled) {
                return;
            }

            if (triggerEnterSpec.useLayerFilter && !triggerEnterSpec.matchingLayers.Contains(other.gameObject.layer)) {
                return;
            }

            if (triggerEnterSpec.useTagFilter && !triggerEnterSpec.matchingTags.Contains(other.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnTriggerEnter);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerExit(Collider other) {
            // check filters for matches if turned on
            if (!triggerExitSpec.eventEnabled) {
                return;
            }

            if (triggerExitSpec.useLayerFilter && !triggerExitSpec.matchingLayers.Contains(other.gameObject.layer)) {
                return;
            }

            if (triggerExitSpec.useTagFilter && !triggerExitSpec.matchingTags.Contains(other.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnTriggerExit);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter2D(Collision2D collision) {
            // check filters for matches if turned on
            if (!collision2dSpec.eventEnabled) {
                return;
            }

            if (collision2dSpec.useLayerFilter && !collision2dSpec.matchingLayers.Contains(collision.gameObject.layer)) {
                return;
            }

            if (collision2dSpec.useTagFilter && !collision2dSpec.matchingTags.Contains(collision.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnCollision2D);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerEnter2D(Collider2D other) {
            // check filters for matches if turned on
            if (!triggerEnter2dSpec.eventEnabled) {
                return;
            }

            if (triggerEnter2dSpec.useLayerFilter && !triggerEnter2dSpec.matchingLayers.Contains(other.gameObject.layer)) {
                return;
            }

            if (triggerEnter2dSpec.useTagFilter && !triggerEnter2dSpec.matchingTags.Contains(other.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnTriggerEnter2D);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerExit2D(Collider2D other) {
            // check filters for matches if turned on
            if (!triggerExit2dSpec.eventEnabled) {
                return;
            }

            if (triggerExit2dSpec.useLayerFilter && !triggerExit2dSpec.matchingLayers.Contains(other.gameObject.layer)) {
                return;
            }

            if (triggerExit2dSpec.useTagFilter && !triggerExit2dSpec.matchingTags.Contains(other.gameObject.tag)) {
                return;
            }

			Despawn(TriggeredSpawner.EventType.OnTriggerExit2D);
        }

		private void Despawn(TriggeredSpawner.EventType eType) {
            if (LevelSettings.AppIsShuttingDown || _isDespawning ) {                
                return;
            }

            _isDespawning = true;

            if (listener != null) {
                listener.Despawning(eType, _trans);
            }

            PoolBoss.Despawn(_trans);
        }
    }
}
/*! \endcond */