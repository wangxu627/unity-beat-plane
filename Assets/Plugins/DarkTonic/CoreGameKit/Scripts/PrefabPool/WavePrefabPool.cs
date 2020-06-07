using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used for Prefab Pool setup, to give randomness and weight to the groups of prefabs in a single spawner wave (or Killable spawn).
    /// </summary>
    // ReSharper disable once CheckNamespace
    public class WavePrefabPool : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public bool isExpanded = true;
        public bool exhaustiveList = true;
        public PoolDispersalMode dispersalMode = PoolDispersalMode.Randomized;
        public WavePrefabPoolListener listener;
        public List<WavePrefabPoolItem> poolItems;
        // ReSharper restore InconsistentNaming

        private bool _isValid;
        private readonly List<int> _poolItemIndexes = new List<int>();

        public enum PoolDispersalMode {
            Randomized,
            OriginalPoolOrder
        }
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            useGUILayout = false;
            FillPool();
        }

        private void FillPool() {
            // fill weighted pool
            for (var item = 0; item < poolItems.Count; item++) {
                var poolItem = poolItems[item];

                var includeItem = true;

                switch (poolItem.activeMode) {
                    case LevelSettings.ActiveItemMode.Always:
                        break;
                    case LevelSettings.ActiveItemMode.Never:
                        continue;
                    case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
                        if (poolItem.activeItemCriteria.statMods.Count == 0) {
                            includeItem = false;
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < poolItem.activeItemCriteria.statMods.Count; i++) {
                            var stat = poolItem.activeItemCriteria.statMods[i];
                            if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                                Debug.LogError(
                                    string.Format(
                                        "Prefab Pool '{0}' could not find World Variable '{1}', which is used in its Active Item Criteria.",
                                        transform.name,
                                        stat._statName));
                                includeItem = false;
                                break;
                            }

                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                includeItem = false;
                                break;
                            }
                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;

                            var min = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMin
                                : stat._modValueFloatMin;
                            var max = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMax
                                : stat._modValueFloatMax;

                            if (min > max) {
                                LevelSettings.LogIfNew(
                                    "The Min cannot be greater than the Max for Active Item Limit in Prefab Pool '" +
                                    transform.name + "'. Skipping item '" + poolItem.prefabToSpawn.name + "'.");
                                includeItem = false;
                                break;
                            }

                            if (!(varVal < min) && !(varVal > max)) {
                                continue;
                            }
                            includeItem = false;
                            break;
                        }

                        break;
                    case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                        if (poolItem.activeItemCriteria.statMods.Count == 0) {
                            includeItem = false;
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < poolItem.activeItemCriteria.statMods.Count; i++) {
                            var stat = poolItem.activeItemCriteria.statMods[i];
                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                includeItem = false;
                                break;
                            }
                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;

                            var min = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMin
                                : stat._modValueFloatMin;
                            var max = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMax
                                : stat._modValueFloatMax;

                            if (min > max) {
                                LevelSettings.LogIfNew(
                                    "The Min cannot be greater than the Max for Active Item Limit in Prefab Pool '" +
                                    transform.name + "'. Skipping item '" + poolItem.prefabToSpawn.name + "'.");
                                includeItem = false;
                                break;
                            }

                            if (!(varVal >= min) || !(varVal <= max)) {
                                continue;
                            }
                            includeItem = false;
                            break;
                        }

                        break;
                }

                if (!includeItem) {
                    continue;
                }

                for (var i = 0; i < poolItem.thisWeight.Value; i++) {
                    _poolItemIndexes.Add(item);
                }
            }

            if (_poolItemIndexes.Count == 0) {
                LevelSettings.LogIfNew("The Prefab Pool '" + name +
                                       "' has no active Prefab Pool items. Please add some or delete the Prefab pool before continuing. Disabling Core GameKit.");
                LevelSettings.IsGameOver = true;
                return;
            }

            _isValid = true;
        }

        /// <summary>
        /// This returns a random (unless you've set the Pool to non-random) Transform to you for your own use to spawn. All spawners and Killable use this.
        /// </summary>
        /// <returns></returns>
        public Transform GetRandomWeightedTransform() {
            if (!_isValid) {
                return null;
            }

            var index = 0; // for non-random
            if (dispersalMode == PoolDispersalMode.Randomized) {
                index = Random.Range(0, _poolItemIndexes.Count);
            }

            var prefabIndex = _poolItemIndexes[index];

            if (exhaustiveList || dispersalMode == PoolDispersalMode.OriginalPoolOrder) {
                _poolItemIndexes.RemoveAt(index);

                if (_poolItemIndexes.Count == 0) {
                    // refill
                    if (LevelSettings.IsLoggingOn) {
                        Debug.Log(string.Format("Prefab Pool '{0}' refilling exhaustion list.",
                            name));
                    }

                    if (listener != null) {
                        listener.PoolRefilling();
                    }

                    FillPool();
                }
            }

            var spawnable = poolItems[prefabIndex].prefabToSpawn;

            if (LevelSettings.IsLoggingOn) {
                Debug.Log(string.Format("Prefab Pool '{0}' spawning random item '{1}'.",
                    name,
                    spawnable.name));
            }

            if (listener != null) {
                listener.PrefabGrabbedFromPool(spawnable);
            }

            return spawnable;
        }

		/// <summary>
		/// Calling this method will clear and refill the pool, taking into account any World Variables you or other weights you may have changed since the last fill.
		/// </summary>
		public void ResetPool() {
			_poolItemIndexes.Clear();
			FillPool();
		}

        /*! \cond PRIVATE */
        public int PoolInstancesOfIndex(int index) {
            return _poolItemIndexes.FindAll(delegate(int obj) {
                return obj.Equals(index);
            }).Count;
        }
        /*! \endcond */
    }
}