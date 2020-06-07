using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
	/// <summary>
    /// This class is used for various Spawner methods, such as activate and deactivate wave.
    /// </summary>
    // ReSharper disable once CheckNamespace
    public static class SpawnerUtility {
		private const int MaxVisualizeItems = 100;

		/// <summary>
        /// Use this method to activate a wave by Level and Wave # in a Syncro Spawner.
        /// </summary>
        /// <param name="transSpawner">The Transform of the Syncro Spawner.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="waveNumber">The wave number.</param>
        public static void ActivateWave(Transform transSpawner, int levelNumber, int waveNumber) {
            var spawner = transSpawner.GetComponent<WaveSyncroPrefabSpawner>();
            ActivateWave(spawner, levelNumber, waveNumber);
        }

        /// <summary>
        /// Use this method to activate a wave by Level and Wave # in a Syncro Spawner.
        /// </summary>
        /// <param name="spawner">The Spawner script of the Syncro Spawner.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="waveNumber">The wave number.</param>
        public static void ActivateWave(WaveSyncroPrefabSpawner spawner, int levelNumber, int waveNumber) {
            ChangeSpawnerWaveStatus(spawner, levelNumber, waveNumber, true);
        }

        /// <summary>
        /// Use this method to deactivate a wave by Level and Wave # in a Syncro Spawner.
        /// </summary>
        /// <param name="transSpawner">The Transform of the Syncro Spawner.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="waveNumber">The wave number.</param>
        public static void DeactivateWave(Transform transSpawner, int levelNumber, int waveNumber) {
            var spawner = transSpawner.GetComponent<WaveSyncroPrefabSpawner>();
            DeactivateWave(spawner, levelNumber, waveNumber);
        }

        /// <summary>
        /// Use this method to deactivate a wave by Level and Wave # in a Syncro Spawner.
        /// </summary>
        /// <param name="spawner">The Spawner script of the Syncro Spawner.</param>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="waveNumber">The wave number.</param>
        public static void DeactivateWave(WaveSyncroPrefabSpawner spawner, int levelNumber, int waveNumber) {
            ChangeSpawnerWaveStatus(spawner, levelNumber, waveNumber, false);
        }

        private static void ChangeSpawnerWaveStatus(WaveSyncroPrefabSpawner spawner, int levelNumber, int waveNumber,
            bool isActivate) {
            var statusText = isActivate ? "activate" : "deactivate";

            if (spawner == null) {
                LevelSettings.LogIfNew(string.Format("Spawner was NULL. Cannot {0} wave# {1} in level# {2}",
                    statusText,
                    waveNumber,
                    levelNumber));
                return;
            }

            foreach (var wave in spawner.waveSpecs) {
                if (wave.SpawnLevelNumber + 1 != levelNumber || wave.SpawnWaveNumber + 1 != waveNumber) {
                    continue;
                }
                if (LevelSettings.IsLoggingOn) {
                    Debug.Log(string.Format("Logging '{0}' in spawner '{1}' for wave# {2}, level# {3}.",
                        statusText,
                        spawner.name,
                        waveNumber,
                        levelNumber));
                }
                wave.enableWave = isActivate;
                return;
            }

            LevelSettings.LogIfNew(
                string.Format("Could not locate a wave matching wave# {0}, level# {1}, in spawner '{2}'.",
                    waveNumber, levelNumber, spawner.name));
        }

        /*! \cond PRIVATE */
        public static void DestroyChildrenWithoutMarker(this GameObject go) {
            var children = new List<GameObject>();

            foreach (Transform tran in go.transform) {
                if (tran.GetComponent<VisualizationMarker>() != null) {
                    children.Add(tran.gameObject);
                }
            }

			var failsafe = 0;
			while (children.Count > 0 && failsafe < 200) {
				var child = children[0];
				GameObject.Destroy(child);
				if (children[0] == child) {
					children.RemoveAt(0);
				}
				failsafe++;
			}
        }

		public static int GetMaxVisualizeItems(KillerInt kInt) {
			var val = kInt.Value;
			if (val > MaxVisualizeItems) {
				val = MaxVisualizeItems;
			}
			return val;
		}
        /*! \endcond */
    }
}