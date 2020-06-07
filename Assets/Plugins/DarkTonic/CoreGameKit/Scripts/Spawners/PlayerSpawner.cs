/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Spawners/Player Spawner")]
    // ReSharper disable once CheckNamespace
    public class PlayerSpawner : MonoBehaviour {
        public Transform PlayerPrefab;
        public Transform RespawnParticlePrefab;
        public Vector3 RespawnParticleOffset = Vector3.zero;
        public float RespawnDelay = 1f;
        // ReSharper disable InconsistentNaming
        public Vector3 spawnPosition;
        public bool followPlayer = false;
        // ReSharper restore InconsistentNaming

        private Transform _player;
        private float? _nextSpawnTime;
        private Vector3 _playerPosition;
        private bool _isDisabled;

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            if (PlayerPrefab == null) {
                LevelSettings.LogIfNew("No Player Prefab is assigned to PlayerSpawner. PlayerSpawn disabled.");
                _isDisabled = true;
                return;
            }
            if (RespawnDelay < 0) {
                LevelSettings.LogIfNew("Respawn Delay must be a positive number. PlayerSpawn disabled.");
                _isDisabled = true;
                return;
            }

            _nextSpawnTime = null;
            _playerPosition = spawnPosition;

            var pl = GameObject.Find(PlayerPrefab.name);
            _player = pl == null ? null : pl.transform;

            if (_player == null && PoolBoss.IsReady) {
                SpawnPlayer();
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void FixedUpdate() {
            if (_isDisabled) {
                return;
            }

            if (_player == null || !SpawnUtility.IsActive(_player.gameObject)) {
                if (!_nextSpawnTime.HasValue) {
                    _nextSpawnTime = Time.time + RespawnDelay;
                } else if (Time.time >= _nextSpawnTime.Value && !LevelSettings.IsGameOver && PoolBoss.IsReady) {
                    SpawnPlayer();
                    _nextSpawnTime = null;
                }
            } else if (followPlayer) {
                UpdateSpawnPosition(_player.position);
            }
        }

        private void SpawnPlayer() {
            _player = PoolBoss.SpawnOutsidePool(PlayerPrefab, _playerPosition, PlayerPrefab.transform.rotation);

            var spawnPos = _playerPosition + RespawnParticleOffset;
            if (RespawnParticlePrefab != null) {
                PoolBoss.SpawnInPool(RespawnParticlePrefab, spawnPos, RespawnParticlePrefab.transform.rotation);
            }
        }

        public void UpdateSpawnPosition(Vector3 newPosition) {
            _playerPosition = newPosition;
        }
    }
}
/*! \endcond */