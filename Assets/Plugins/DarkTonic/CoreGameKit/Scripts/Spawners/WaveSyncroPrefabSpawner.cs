using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used for Syncro Spawners. These are spawners that use sequential or randomly ordered Global Waves set up in Level Settings.
    /// </summary>
    // ReSharper disable once CheckNamespace
    public class WaveSyncroPrefabSpawner : MonoBehaviour {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public List<WaveSpecifics> waveSpecs = new List<WaveSpecifics>();
        public bool isExpanded = true;

        public LevelSettings.ActiveItemMode activeMode = LevelSettings.ActiveItemMode.Always;
        public WorldVariableRangeCollection activeItemCriteria = new WorldVariableRangeCollection();
        public int randomSortKey = 0;

		public bool spawnOutsidePool = false;
        public TriggeredSpawner.GameOverBehavior gameOverBehavior = TriggeredSpawner.GameOverBehavior.Disable;
        public TriggeredSpawner.WavePauseBehavior wavePauseBehavior = TriggeredSpawner.WavePauseBehavior.Disable;
        public WaveSyncroSpawnerListener listener;

        public SpawnLayerTagMode spawnLayerMode = SpawnLayerTagMode.UseSpawnPrefabSettings;
        public SpawnLayerTagMode spawnTagMode = SpawnLayerTagMode.UseSpawnPrefabSettings;
        public int spawnCustomLayer = 0;
		public bool applyLayerRecursively = false;
		public string spawnCustomTag = "Untagged";
        public bool useLevelFilter;
        public int levelFilter = 0;
        public bool useCopyWave;
        public bool isSpawnerSelectedAsTarget;
        // ReSharper restore InconsistentNaming

        private int _currentWaveSize;
        private int _itemsToCompleteWave;
        private float _currentWaveLength;
        private bool _waveFinishedSpawning;
        private bool _levelSettingsNotifiedOfCompletion;
        private int _countSpawned;
        private float _singleSpawnTime;
        private float _lastSpawnTime;
        private WaveSpecifics _currentWave;
        private float _waveStartTime;
        private Transform _trans;
        private GameObject _go;
        private readonly List<Transform> _spawnedWaveMembers = new List<Transform>();
        private float? _repeatTimer;
        private float _repeatWaitTime;
        private int _waveRepetitionNumber;
        private int _waveRepetitionNumberWithReset;
        private bool _spawnerValid;
        private WavePrefabPool _wavePool;
        private int _instanceId;
        private bool _settingUpWave;
        private Transform _lastPrefabKilled;

        private float _currentRandomLimitDistance;

        public enum SpawnLayerTagMode {
            UseSpawnPrefabSettings,
            UseSpawnerSettings,
            Custom
        }
        /*! \endcond */
        #region MonoBehavior events

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _go = gameObject;
            _trans = transform;
            _waveFinishedSpawning = true;
            _levelSettingsNotifiedOfCompletion = false;
            _repeatTimer = null;
            _spawnerValid = true;
            _waveRepetitionNumber = 0;
            _waveRepetitionNumberWithReset = 0;
            _instanceId = GetInstanceID();

            CheckForDuplicateWaveLevelSettings();
            _go.DestroyChildrenWithoutMarker();
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < waveSpecs.Count; i++) {
                var wave = waveSpecs[i];
                CheckForValidVariablesForWave(wave);
            }

            CheckForValidWorldVariables();
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            if (GameIsOverForSpawner || SpawnerIsPaused || _currentWave == null || !_spawnerValid || _settingUpWave) {
                return;
            }

            CheckForWaveRepeat();

            if (_waveFinishedSpawning                                   // wave is finished and in wait mode
                || (Time.time - _waveStartTime < WaveDelay) // shouldn't spawn yet because of Wave Delay setting.
                || (Time.time - _lastSpawnTime <= _singleSpawnTime && _singleSpawnTime > Time.deltaTime))  // not time to spawn the next item yet.
            { 

                return;
            }

            var numberToSpawn = 1;
            if (_singleSpawnTime < Time.deltaTime) {
                if (_singleSpawnTime == 0) {
                    numberToSpawn = _currentWaveSize;
                } else {
                    numberToSpawn = (int)Math.Ceiling(Time.deltaTime / _singleSpawnTime);
                }
            }

            for (var i = 0; i < numberToSpawn; i++) {
                if (CanSpawnOne()) {
                    SpawnOne();
                }

                if (_countSpawned >= _currentWaveSize) {
                    if (LevelSettings.IsLoggingOn) {
                        Debug.Log(string.Format("Spawner '{0}' finished spawning wave# {1} on level# {2}.",
                            name,
                            _currentWave.SpawnWaveNumber + 1,
                            _currentWave.SpawnLevelNumber + 1));
                    }
                    _waveFinishedSpawning = true;

                    if (listener != null) {
                        listener.WaveFinishedSpawning(_currentWave);
                    }
                }

                _lastSpawnTime = Time.time;
            }
        }

        #endregion

        #region public methods

        /*! \cond PRIVATE */
        public void DeleteLevel(int level) {
            var deadWaves = new List<WaveSpecifics>();

            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber == level) {
                    deadWaves.Add(wrongWave);
                }
            }

            foreach (var dead in deadWaves) {
                waveSpecs.Remove(dead);
            }

            if (deadWaves.Count > 0) {
                Debug.Log(string.Format("Deleted {0} matching wave(s) in spawner '{1}'", deadWaves.Count, name));
            }

            var adjusted = 0;
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber <= level) {
                    continue;
                }
                wrongWave.SpawnLevelNumber--;
                adjusted++;
            }

			if (useLevelFilter) {
				if (levelFilter > 0) {
					levelFilter--;
				}
			}

            LogAdjustments(adjusted);
        }

        public void ShiftUpLevel(int level) {
            var wavesToShiftForward = new List<WaveSpecifics>();

            // move previous level waves + 1
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level - 1) {
                    continue;
                }

                wavesToShiftForward.Add(wrongWave);
            }

            var wavesToShiftBackward = new List<WaveSpecifics>();
            // move this level waves -1 
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level) {
                    continue;
                }

                wavesToShiftBackward.Add(wrongWave);
            }

            foreach (var wave in wavesToShiftBackward) {
                wave.SpawnLevelNumber--;
            }
            foreach (var wave in wavesToShiftForward) {
                wave.SpawnLevelNumber++;
            }

            LogAdjustments(wavesToShiftForward.Count + wavesToShiftBackward.Count);
        }

        public void ShiftDownLevel(int level) {
            var wavesToShiftForward = new List<WaveSpecifics>();

            // move this level waves + 1
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level) {
                    continue;
                }

                wavesToShiftForward.Add(wrongWave);
            }

            var wavesToShiftBackward = new List<WaveSpecifics>();
            // move next level waves -1
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level + 1) {
                    continue;
                }

                wavesToShiftBackward.Add(wrongWave);
            }

            foreach (var wave in wavesToShiftBackward) {
                wave.SpawnLevelNumber--;
            }
            foreach (var wave in wavesToShiftForward) {
                wave.SpawnLevelNumber++;
            }

            LogAdjustments(wavesToShiftForward.Count + wavesToShiftBackward.Count);
        }

        public void ShiftUpWave(int level, int waveNum) {
			var waveToShiftUp = FindWave(level, waveNum);
			var wavesToShiftUp = new List<WaveSpecifics>();

			if (waveToShiftUp != null) { 
				// move same level, higher waves back one.
				foreach (var wrongWave in waveSpecs) {
					if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber != waveNum) {
						continue;
					}

					wavesToShiftUp.Add(wrongWave);
				}
 			}

			var wavesToShiftDown = new List<WaveSpecifics>();
			var waveToShiftDown = FindWave(level, waveNum - 1);
			if (waveToShiftDown != null) { 
				// move same level, higher waves forward one.
				foreach (var wrongWave in waveSpecs) {
					if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber != waveNum - 1) {
						continue;
					}

					wavesToShiftDown.Add(wrongWave);
				}
			}

			foreach(var wave in wavesToShiftUp) {
				wave.SpawnWaveNumber--;
			}
			foreach (var wave in wavesToShiftDown) {
				wave.SpawnWaveNumber++;
			}

			LogAdjustments(wavesToShiftUp.Count + wavesToShiftDown.Count);
		}

		public void ShiftDownWave(int level, int waveNum) {
			var waveToShiftDown = FindWave(level, waveNum);
			var wavesToShiftDown = new List<WaveSpecifics>();
			
			if (waveToShiftDown != null) { 
				// move same level, higher waves back one.
				foreach (var wrongWave in waveSpecs) {
					if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber != waveNum) {
						continue;
					}
					
					wavesToShiftDown.Add(wrongWave);
				}
			}
			
			var wavesToShiftUp = new List<WaveSpecifics>();
			var waveToShiftUp = FindWave(level, waveNum + 1);
			if (waveToShiftUp != null) { 
				// move same level, higher waves forward one.
				foreach (var wrongWave in waveSpecs) {
					if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber != waveNum + 1) {
						continue;
					}
					
					wavesToShiftUp.Add(wrongWave);
				}
			}
			
			foreach (var wave in wavesToShiftDown) {
				wave.SpawnWaveNumber++;
			}
			foreach(var wave in wavesToShiftUp) {
				wave.SpawnWaveNumber--;
			}

			LogAdjustments(wavesToShiftUp.Count + wavesToShiftDown.Count);
		}

		public void DeleteWave(int level, int wav) {
			var matchingWave = FindWave(level, wav);
			if (matchingWave != null) {
				waveSpecs.Remove(matchingWave);
                Debug.Log(string.Format("Deleted matching wave in spawner '{0}'", name));
            }

            var adjustments = 0;

            // move same level, higher waves back one.
            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber <= wav) {
                    continue;
                }
                wrongWave.SpawnWaveNumber--;
                adjustments++;
            }

            LogAdjustments(adjustments);
        }

        public WaveSpecifics FindWave(int levelToMatch, int waveToMatch) {
            foreach (var wave in waveSpecs) {
                if (wave.SpawnLevelNumber != levelToMatch || wave.SpawnWaveNumber != waveToMatch) {
                    continue;
                }

                // found the match, get outa here!!
                return wave;
            }

            return null;
        }

        public void InsertLevel(int level) {
            var adjustments = 0;

            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber < level) {
                    continue;
                }
                wrongWave.SpawnLevelNumber++;
                adjustments++;
            }

            LogAdjustments(adjustments);
        }

        public void InsertWave(int newWaveNumber, int level) {
            var adjustments = 0;

            foreach (var wrongWave in waveSpecs) {
                if (wrongWave.SpawnLevelNumber != level || wrongWave.SpawnWaveNumber < newWaveNumber) {
                    continue;
                }
                wrongWave.SpawnWaveNumber++;
                adjustments++;
            }

            LogAdjustments(adjustments);
        }

        public bool IsUsingPrefabPool(Transform poolTrans) {
            foreach (var wave in waveSpecs) {
                if (wave.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool && wave.prefabPoolName == poolTrans.name) {
                    return true;
                }
            }

            return false;
        }

        public void RemoveSpawnedMember(Transform transMember) {
            if (_spawnedWaveMembers.Count == 0) {
                return;
            }

            if (_spawnedWaveMembers.Contains(transMember)) {
                _spawnedWaveMembers.Remove(transMember);
                LevelSettings.RemoveWaveSpawnedItem(transMember);
                _itemsToCompleteWave--;

                if (_spawnedWaveMembers.Count == 0 && _waveFinishedSpawning) {
                    _lastPrefabKilled = transMember;
                }
            }

            CheckForWaveRepeat();
        }

        public string CheckForDuplicateWaveLevelSettings() {
            var dupeMsg = string.Empty;

            var waveLevelCombos = new List<string>();
            foreach (var wave in waveSpecs) {
                var combo = wave.SpawnLevelNumber + ":" + wave.SpawnWaveNumber;
                if (waveLevelCombos.Contains(combo)) {
                    if (Application.isPlaying) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}' contains more than one wave setting for level: {1} and wave: {2}. This is not allowed. Spawner aborting until this is fixed.",
                                name, wave.SpawnLevelNumber + 1, wave.SpawnWaveNumber + 1));
                    } else {
                        dupeMsg = string.Format(
                        "This spawner contains more than one wave setting for level: {0} and wave: {1}. This is not allowed. Spawner disabled until this is fixed.",
                        wave.SpawnLevelNumber + 1, wave.SpawnWaveNumber + 1);
                    }

                    _spawnerValid = false;
                    break;
                }

                waveLevelCombos.Add(combo);
            }

            return dupeMsg;
        }
        /*! \endcond */

        /// <summary> 
        /// Calling this method will spawn one of the current wave for this Level and Wave, if this Spawner is configured to use that Level and Wave.
        /// </summary>
        /// <returns>The Transform of the spawned item.</returns>
        public Transform SpawnOneItem() {
            return SpawnOne(true);
        }

        /// <summary>
        /// Returns a list of Spawned Wave Members
        /// </summary>
        public List<Transform> SpawnedWaveMembers {
            get {
                return _spawnedWaveMembers;
            }
        }

        /*! \cond PRIVATE */
        public bool WaveChange(bool isRestart) {
            _lastPrefabKilled = null;

            if (!_spawnerValid) {
                return false;
            }

            var setupNew = SetupNextWave(true, isRestart);
            if (!setupNew) {
                return false;
            }
            if (listener != null) {
                listener.WaveStart(_currentWave);
            }

            return true;
        }

        public void WaveRepeat() {
            if (_currentWave.waveRepeatBonusesEnabled && _currentWave.waveRepeatVariableModifiers.statMods.Count > 0) {
                // ReSharper disable once TooWideLocalVariableScope
                WorldVariableModifier mod;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _currentWave.waveRepeatVariableModifiers.statMods.Count; i++) {
                    mod = _currentWave.waveRepeatVariableModifiers.statMods[i];
                    WorldVariableTracker.ModifyPlayerStat(mod, _trans);
                }
            }

            if (_currentWave.waveRepeatFireEvents) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _currentWave.waveRepeatCustomEvents.Count; i++) {
                    var anEvent = _currentWave.waveRepeatCustomEvents[i].CustomEventName;

                    LevelSettings.FireCustomEventIfValid(anEvent, _trans);
                }
            }

            if (!SetupNextWave(false, false)) {
                return;
            }
            if (listener != null) {
                listener.WaveRepeat(_currentWave);
            }
        }

        public void SpawnWaveVisual(WaveSpecifics wave) {
            if (Application.isPlaying) {
                // let's not lock up the CPU!
                return;
            }

            var isSphere = wave.spawnSource != WaveSpecifics.SpawnOrigin.Specific || wave.prefabToSpawn == null;

            var nbrToSpawn = SpawnerUtility.GetMaxVisualizeItems(wave.MinToSpwn);

            for (var i = 0; i < nbrToSpawn; i++) {
                var spawnPosition = GetSpawnPositionForVisualization(wave, transform.position, i);

                var rotation = GetSpawnRotationForVisualization(wave, transform, i);

                Transform spawned;

                if (isSphere) {
                    spawned = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    spawned.transform.position = spawnPosition;
                    spawned.transform.rotation = rotation;
                } else {
                    // ReSharper disable once PossibleNullReferenceException
                    spawned = ((Transform)Instantiate(wave.prefabToSpawn, spawnPosition, rotation));
                }

                spawned.parent = transform;
                spawned.gameObject.AddComponent<VisualizationMarker>();

                AfterSpawnForVisualization(wave, spawned.transform);
            }
        }

        // if a wave is restarted before over.
        public void ResetWaveIncrementCounter() {
            _waveRepetitionNumber = 0;
            _waveRepetitionNumberWithReset = 0;
            _spawnedWaveMembers.Clear();
            _currentWave = null; // stops wave from spawning any more
        }
        /*! \endcond */
        #endregion

        #region overridable methods

        /// <summary>
        /// Fires immediately after the item spawns. Used by post-spawn nudge. Override if you need custom logic here.
        /// </summary>
        /// <param name="spawnedTrans">The spawned Transform.</param>
        protected virtual void AfterSpawn(Transform spawnedTrans) {
            if (_currentWave.enablePostSpawnNudge) {
                spawnedTrans.Translate(Vector3.forward * _currentWave.postSpawnNudgeFwd.Value);
                spawnedTrans.Translate(Vector3.right * _currentWave.postSpawnNudgeRgt.Value);
                spawnedTrans.Translate(Vector3.down * _currentWave.postSpawnNudgeDwn.Value);
            }

            switch (spawnLayerMode) {
                case SpawnLayerTagMode.UseSpawnerSettings:
                    if (applyLayerRecursively) {
						spawnedTrans.SetLayerOnAllChildren(_go.layer);
					} else {
						spawnedTrans.gameObject.layer = _go.layer;
					}
                    break;
                case SpawnLayerTagMode.Custom:
					if (applyLayerRecursively) {
						spawnedTrans.SetLayerOnAllChildren(spawnCustomLayer);
					} else {
						spawnedTrans.gameObject.layer = spawnCustomLayer;
					}
                    break;
            }

            switch (spawnTagMode) {
                case SpawnLayerTagMode.UseSpawnerSettings:
                    spawnedTrans.gameObject.tag = _go.tag;
                    break;
                case SpawnLayerTagMode.Custom:
                    spawnedTrans.gameObject.tag = spawnCustomTag;
                    break;
            }

            if (listener != null) {
                listener.ItemSpawned(spawnedTrans);
            }
        }

        /// <summary>
        /// Returns true always unless the Distance Limit feature is used. Subclass this to override it if needed.
        /// </summary>
        /// <returns>boolean value.</returns>
        protected virtual bool CanSpawnOne() {
            if (!_currentWave.enableLimits) {
                return true;
            }

            var allSpawnedAreFarEnoughAway = SpawnUtility.SpawnedMembersAreAllBeyondDistance(_trans, _spawnedWaveMembers,
                _currentWave.doNotSpawnIfMbrCloserThan.Value + _currentRandomLimitDistance);

            return allSpawnedAreFarEnoughAway;
        }

        /// <summary>
        /// This returns the item to spawn. Override this to apply custom logic if needed.
        /// </summary>
        /// <param name="wave">The wave specifics.</param>
        /// <returns>The Transform to spawn.</returns>
        protected virtual Transform GetSpawnable(WaveSpecifics wave) {
            if (wave == null) {
                return null;
            }

            switch (wave.spawnSource) {
                case WaveSpecifics.SpawnOrigin.Specific:
                    return wave.prefabToSpawn;
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    return _wavePool.GetRandomWeightedTransform();
            }

            return null;
        }

        /// <summary>
        /// Returns the position to spawn an item in. Override for your custom logic.
        /// </summary>
        /// <param name="pos">Spawner position.</param>
        /// <param name="itemSpawnedIndex">Index (counter) of item to spawn. Incremental Settings use this.</param>
        /// <returns>The modified position to spawn in.</returns>
        protected virtual Vector3 GetSpawnPosition(Vector3 pos, int itemSpawnedIndex) {
            switch (_currentWave.positionXmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.x = _currentWave.customPosX.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (_currentWave.otherObjectX != null) {
                        pos.x = _currentWave.otherObjectX.position.x;
                    }
                    break;
            }

            switch (_currentWave.positionYmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.y = _currentWave.customPosY.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (_currentWave.otherObjectY != null) {
                        pos.y = _currentWave.otherObjectY.position.y;
                    }
                    break;
            }

            switch (_currentWave.positionZmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.z = _currentWave.customPosZ.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (_currentWave.otherObjectZ != null) {
                        pos.z = _currentWave.otherObjectZ.position.z;
                    }
                    break;
            }

            var addVector = Vector3.zero;

            addVector += _currentWave.WaveOffset;

            if (_currentWave.enableRandomizations) {
                addVector.x += Random.Range(-_currentWave.randomDistX.Value, _currentWave.randomDistX.Value);
                addVector.y += Random.Range(-_currentWave.randomDistY.Value, _currentWave.randomDistY.Value);
                addVector.z += Random.Range(-_currentWave.randomDistZ.Value, _currentWave.randomDistZ.Value);
            }

            if (!_currentWave.enableIncrements || itemSpawnedIndex <= 0) {
                return pos + addVector;
            }
            addVector.x += (_currentWave.incrementPositionX.Value * itemSpawnedIndex);
            addVector.y += (_currentWave.incrementPositionY.Value * itemSpawnedIndex);
            addVector.z += (_currentWave.incrementPositionZ.Value * itemSpawnedIndex);

            return pos + addVector;
        }

        /// <summary>
        /// Returns the rotation to spawn the item in. Override if you need custom logic for this.
        /// </summary>
        /// <param name="prefabToSpawn">The prefab to spawn.</param>
        /// <param name="itemSpawnedIndex">Index (counter) of item to spawn. Incremental Settings use this.</param>
        /// <returns>The modified rotation to spawn in.</returns>
        protected virtual Quaternion GetSpawnRotation(Transform prefabToSpawn, int itemSpawnedIndex) {
            var euler = Vector3.zero;

            switch (_currentWave.curRotationMode) {
                case WaveSpecifics.RotationMode.UsePrefabRotation:
                    euler = prefabToSpawn.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.UseSpawnerRotation:
                    euler = _trans.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.CustomRotation:
                    euler = _currentWave.customRotation;
                    break;
            }

            if (_currentWave.enableRandomizations && _currentWave.randomXRotation) {
                euler.x = Random.Range(_currentWave.randomXRotMin.Value, _currentWave.randomXRotMax.Value);
            } else if (_currentWave.enableIncrements && itemSpawnedIndex > 0) {
                if (_currentWave.enableKeepCenter) {
                    euler.x += (itemSpawnedIndex * _currentWave.incrementRotX.Value -
                                (_currentWaveSize * _currentWave.incrementRotX.Value * .5f));
                } else {
                    euler.x += (itemSpawnedIndex * _currentWave.incrementRotX.Value);
                }
            }

            if (_currentWave.enableRandomizations && _currentWave.randomYRotation) {
                euler.y = Random.Range(_currentWave.randomYRotMin.Value, _currentWave.randomYRotMax.Value);
            } else if (_currentWave.enableIncrements && itemSpawnedIndex > 0) {
                if (_currentWave.enableKeepCenter) {
                    euler.y += (itemSpawnedIndex * _currentWave.incrementRotY.Value -
                                (_currentWaveSize * _currentWave.incrementRotY.Value * .5f));
                } else {
                    euler.y += (itemSpawnedIndex * _currentWave.incrementRotY.Value);
                }
            }

            if (_currentWave.enableRandomizations && _currentWave.randomZRotation) {
                euler.z = Random.Range(_currentWave.randomZRotMin.Value, _currentWave.randomZRotMax.Value);
            } else if (_currentWave.enableIncrements && itemSpawnedIndex > 0) {
                if (_currentWave.enableKeepCenter) {
                    euler.z += (itemSpawnedIndex * _currentWave.incrementRotZ.Value -
                                (_currentWaveSize * _currentWave.incrementRotZ.Value * .5f));
                } else {
                    euler.z += (itemSpawnedIndex * _currentWave.incrementRotZ.Value);
                }
            }

            return Quaternion.Euler(euler);
        }

        /// <summary>
        /// Override this method to call a Network Instantiate method or other.
        /// </summary>
        /// <param name="prefabToSpawn">The prefab to spawn.</param>
        /// <param name="spawnPosition">The position to spawn in.</param>
        /// <param name="rotation">The rotation to spawn with.</param>
        /// <returns>Spawned item Transform</returns>
        protected virtual Transform SpawnWaveItem(Transform prefabToSpawn, Vector3 spawnPosition, Quaternion rotation) {
            if (spawnOutsidePool) {
				return PoolBoss.SpawnOutsidePool(prefabToSpawn, spawnPosition, GetSpawnRotation(prefabToSpawn, _countSpawned));
			}

			return PoolBoss.SpawnInPool(prefabToSpawn, spawnPosition, GetSpawnRotation(prefabToSpawn, _countSpawned));
        }

        #endregion

        #region private methods

        private void SpawnBonusPrefabIfSelected(Vector3 spawnPosition) { // only gets called by non-final waves
            if (_currentWave.bonusRepeatToUseItem == LevelSettings.RepeatToUseItem.All) {
                SpawnBonusPrefabIfAny(spawnPosition);
            }
        }

        private void SpawnBonusPrefabIfAny(Vector3 spawnPosition) {
            var waveBeat = _currentWave;

            if (LevelSettings.PreviousWaveInfo.waveType != LevelSettings.WaveType.Elimination) {
                return;
            }

            if (!waveBeat.useSpawnBonusPrefab) {
                return;
            }

            if (Random.Range(0, 100) >= waveBeat.bonusPrefabSpawnPercent.Value) {
                return;
            }

            switch (waveBeat.bonusPrefabSource) {
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    var bonusPrefabPool = LevelSettings.GetFirstMatchingPrefabPool(waveBeat.bonusPrefabPoolName);
                    if (bonusPrefabPool == null) {
                        LevelSettings.LogIfNew("Could not find Prefab Pool '" + waveBeat.bonusPrefabPoolName + "' for Bonus Prefab on Level: " + LevelSettings.CurrentDisplayLevel + ", Wave: " + LevelSettings.CurrentDisplayWave);
                        return;
                    }

                    var numToSpawn = waveBeat.bonusPrefabQty.Value;
                    for (var i = 0; i < numToSpawn; i++) {
                        var prefabToSpawn = bonusPrefabPool.GetRandomWeightedTransform();
                        if (prefabToSpawn == null) {
                            LevelSettings.LogIfNew("Prefab Pool '" + bonusPrefabPool.name + "' has no items left to spawn for Bonus prefab.");
                            break;
                        }
                        var spawned = PoolBoss.SpawnInPool(prefabToSpawn, spawnPosition, Quaternion.identity);
                        if (spawned == null) {
                            break;
                        }
                    }

                    break;
                case WaveSpecifics.SpawnOrigin.Specific:
                    if (waveBeat.bonusPrefabSpecific == null) {
                        LevelSettings.LogIfNew("Bonus Prefab for Level: " + LevelSettings.CurrentDisplayLevel + ", Wave: " + LevelSettings.CurrentDisplayWave + " is unassigned.");
                        return;
                    }

                    var numberToSpawn = waveBeat.bonusPrefabQty.Value;
                    for (var i = 0; i < numberToSpawn; i++) {
                        var spawned = PoolBoss.SpawnInPool(waveBeat.bonusPrefabSpecific, spawnPosition, Quaternion.identity);
                        if (spawned == null) {
                            break;
                        }
                    }
                    break;
            }
        }

        private void AddSpawnTracker(Transform spawnedTrans) {
            var wrongTracker = spawnedTrans.GetComponent<TrigSpawnTracker>();
            if (wrongTracker != null) {
                GameObject.DestroyImmediate(wrongTracker);
            }

            var tracker = spawnedTrans.GetComponent<SpawnTracker>();
            if (tracker == null) {
                spawnedTrans.gameObject.AddComponent(typeof(SpawnTracker));
                tracker = spawnedTrans.GetComponent<SpawnTracker>();
            }

            tracker.SourceSpawner = this;
        }

        private void CheckForValidVariablesForWave(WaveSpecifics wave) {
            // examine all KillerInts
            wave.MinToSpwn.LogIfInvalid(_trans, "Min To Spawn", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.MaxToSpwn.LogIfInvalid(_trans, "Max To Spawn", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repetitionsToDo.LogIfInvalid(_trans, "Repetitions", wave.SpawnLevelNumber, wave.SpawnWaveNumber);

            if (wave.positionXmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosX.LogIfInvalid(_trans, "Custom X Position", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            }
            if (wave.positionYmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosY.LogIfInvalid(_trans, "Custom Y Position", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            }
            if (wave.positionZmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosZ.LogIfInvalid(_trans, "Custom Z Position", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            }

            // examine all KillerFloats
            wave.WaveDelaySec.LogIfInvalid(_trans, "Delay Wave (sec)", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.TimeToSpawnEntireWave.LogIfInvalid(_trans, "Time To Spawn All", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.repeatPauseMinimum.LogIfInvalid(_trans, "Repeat Pause Min", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatPauseMaximum.LogIfInvalid(_trans, "Repeat Pause Max", wave.SpawnLevelNumber, wave.SpawnWaveNumber);

            wave.repeatItemInc.LogIfInvalid(_trans, "Spawn Increase", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatItemMinLmt.LogIfInvalid(_trans, "Spawn Min Limit", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatItemLmt.LogIfInvalid(_trans, "Spawn Max Limit", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatTimeInc.LogIfInvalid(_trans, "Time Increase", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatTimeMinLmt.LogIfInvalid(_trans, "Time Min Limit", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.repeatTimeLmt.LogIfInvalid(_trans, "Time Max Limit", wave.SpawnLevelNumber, wave.SpawnWaveNumber);

            wave.doNotSpawnIfMbrCloserThan.LogIfInvalid(_trans, "Spawn Limit Min. Distance", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.doNotSpawnRandomDist.LogIfInvalid(_trans, "Spawn Limit Random Distance", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.randomDistX.LogIfInvalid(_trans, "Rand. Distance X", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomDistY.LogIfInvalid(_trans, "Rand. Distance Y", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomDistZ.LogIfInvalid(_trans, "Rand. Distance Z", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomXRotMin.LogIfInvalid(_trans, "Rand. X Rot. Min", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomXRotMax.LogIfInvalid(_trans, "Rand. X Rot. Max", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomYRotMin.LogIfInvalid(_trans, "Rand. Y Rot. Min", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomYRotMax.LogIfInvalid(_trans, "Rand. Y Rot. Max", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomYRotMin.LogIfInvalid(_trans, "Rand. Z Rot. Min", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.randomYRotMax.LogIfInvalid(_trans, "Rand. Z Rot. Max", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.incrementPositionX.LogIfInvalid(_trans, "Incremental Distance X", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.incrementPositionY.LogIfInvalid(_trans, "Incremental Distance Y", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.incrementPositionZ.LogIfInvalid(_trans, "Incremental Distance Z", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.incrementRotX.LogIfInvalid(_trans, "Incremental Rotation X", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.incrementRotY.LogIfInvalid(_trans, "Incremental Rotation Y", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.incrementRotZ.LogIfInvalid(_trans, "Incremental Rotation Z", wave.SpawnLevelNumber,
                wave.SpawnWaveNumber);
            wave.postSpawnNudgeFwd.LogIfInvalid(_trans, "Nudge Forward", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.postSpawnNudgeDwn.LogIfInvalid(_trans, "Nudge Down", wave.SpawnLevelNumber, wave.SpawnWaveNumber);
            wave.postSpawnNudgeRgt.LogIfInvalid(_trans, "Nudge Right", wave.SpawnLevelNumber, wave.SpawnWaveNumber);

            if (wave.curWaveRepeatMode != WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove &&
                wave.curWaveRepeatMode != WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow) {
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < wave.repeatPassCriteria.statMods.Count; i++) {
                var crit = wave.repeatPassCriteria.statMods[i];

                if (WorldVariableTracker.IsBlankVariableName(crit._statName)) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit with no World Variable selected. Please select one.",
                            _trans.name,
                            wave.SpawnLevelNumber + 1,
                            wave.SpawnWaveNumber + 1));
                    _spawnerValid = false;
                } else if (!WorldVariableTracker.VariableExistsInScene(crit._statName)) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit using World Variable '{3}', which doesn't exist in the scene.",
                            _trans.name,
                            wave.SpawnLevelNumber + 1,
                            wave.SpawnWaveNumber + 1,
                            crit._statName));
                    _spawnerValid = false;
                } else {
                    switch (crit._varTypeToUse) {
                        case WorldVariableTracker.VariableType._integer:
                            if (crit._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (!WorldVariableTracker.VariableExistsInScene(crit._modValueIntAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            crit._modValueIntAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit criteria with no World Variable selected. Please select one.",
                                                _trans.name,
                                                wave.SpawnLevelNumber + 1,
                                                wave.SpawnWaveNumber + 1));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit using the value of World Variable '{3}', which doesn't exist in the Scene.",
                                                _trans.name,
                                                wave.SpawnLevelNumber + 1,
                                                wave.SpawnWaveNumber + 1,
                                                crit._modValueIntAmt.worldVariableName));
                                    }
                                    _spawnerValid = false;
                                }
                            }

                            break;
                        case WorldVariableTracker.VariableType._float:
                            if (crit._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(crit._modValueFloatAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            crit._modValueFloatAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit criteria with no World Variable selected. Please select one.",
                                                _trans.name,
                                                wave.SpawnLevelNumber + 1,
                                                wave.SpawnWaveNumber + 1));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', Level {1} Wave {2} has a Repeat Item Limit using the value of World Variable '{3}', which doesn't exist in the Scene.",
                                                _trans.name,
                                                wave.SpawnLevelNumber + 1,
                                                wave.SpawnWaveNumber + 1,
                                                crit._modValueFloatAmt.worldVariableName));
                                    }
                                    _spawnerValid = false;
                                }
                            }

                            break;
                        default:
                            LevelSettings.LogIfNew("Add code for varType: " + crit._varTypeToUse.ToString());
                            break;
                    }
                }
            }
        }

        private void CheckForValidWorldVariables() {
            if (activeMode == LevelSettings.ActiveItemMode.IfWorldVariableInRange ||
                activeMode == LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                    var crit = activeItemCriteria.statMods[i];

                    if (WorldVariableTracker.IsBlankVariableName(crit._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}' has an Active Item Limit criteria with no World Variable selected. Please select one.",
                                _trans.name));
                        _spawnerValid = false;
                    } else if (!WorldVariableTracker.VariableExistsInScene(crit._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}' has an Active Item Limit criteria criteria of World Variable '{1}', which doesn't exist in the scene.",
                                _trans.name,
                                crit._statName));
                        _spawnerValid = false;
                    }
                }
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < waveSpecs.Count; i++) {
                var wave = waveSpecs[i];

                if (!wave.waveRepeatBonusesEnabled) {
                    continue;
                }

                // ReSharper disable ForCanBeConvertedToForeach
                for (var b = 0; b < wave.waveRepeatVariableModifiers.statMods.Count; b++) {
                    // ReSharper restore ForCanBeConvertedToForeach
                    var beatMod = wave.waveRepeatVariableModifiers.statMods[b];

                    if (WorldVariableTracker.IsBlankVariableName(beatMod._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}', Level {1} Wave {2} specifies a Wave Repeat Bonus with no World Variable selected. Please select one.",
                                _trans.name,
                                (wave.SpawnLevelNumber + 1),
                                (wave.SpawnWaveNumber + 1)));
                        _spawnerValid = false;
                    } else if (!WorldVariableTracker.VariableExistsInScene(beatMod._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}', Level {1} Wave {2} specifies a Wave Repeat Bonus of World Variable '{3}', which doesn't exist in the scene.",
                                _trans.name,
                                (wave.SpawnLevelNumber + 1),
                                (wave.SpawnWaveNumber + 1),
                                beatMod._statName));
                        _spawnerValid = false;
                    } else {
                        switch (beatMod._varTypeToUse) {
                            case WorldVariableTracker.VariableType._integer:
                                if (beatMod._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                    if (
                                        !WorldVariableTracker.VariableExistsInScene(
                                            beatMod._modValueIntAmt.worldVariableName)) {
                                        if (
                                            LevelSettings.IllegalVariableNames.Contains(
                                                beatMod._modValueIntAmt.worldVariableName)) {
                                            LevelSettings.LogIfNew(
                                                string.Format(
                                                    "Spawner '{0}', Level {1} Wave {2} wants to award Wave Repeat Bonus if World Variable '{3}' is above the value of an unspecified World Variable. Please select one.",
                                                    _trans.name,
                                                    (wave.SpawnLevelNumber + 1),
                                                    (wave.SpawnWaveNumber + 1),
                                                    beatMod._statName));
                                        } else {
                                            LevelSettings.LogIfNew(
                                                string.Format(
                                                    "Spawner '{0}', Level {1} Wave {2} wants to award Wave Repeat Bonus if World Variable '{3}' is above the value of World Variable '{4}', but the latter is not in the Scene.",
                                                    _trans.name,
                                                    (wave.SpawnLevelNumber + 1),
                                                    (wave.SpawnWaveNumber + 1),
                                                    beatMod._statName,
                                                    beatMod._modValueIntAmt.worldVariableName));
                                        }
                                        _spawnerValid = false;
                                    }
                                }

                                break;
                            case WorldVariableTracker.VariableType._float:
                                if (beatMod._modValueFloatAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                    if (
                                        !WorldVariableTracker.VariableExistsInScene(
                                            beatMod._modValueFloatAmt.worldVariableName)) {
                                        if (
                                            LevelSettings.IllegalVariableNames.Contains(
                                                beatMod._modValueFloatAmt.worldVariableName)) {
                                            LevelSettings.LogIfNew(
                                                string.Format(
                                                    "Spawner '{0}', Level {1} Wave {2} wants to award Wave Repeat Bonus if World Variable '{3}' is above the value of an unspecified World Variable. Please select one.",
                                                    _trans.name,
                                                    (wave.SpawnLevelNumber + 1),
                                                    (wave.SpawnWaveNumber + 1),
                                                    beatMod._statName));
                                        } else {
                                            LevelSettings.LogIfNew(
                                                string.Format(
                                                    "Spawner '{0}', Level {1} Wave {2} wants to award Wave Repeat Bonus if World Variable '{3}' is above the value of World Variable '{4}', but the latter is not in the Scene.",
                                                    _trans.name,
                                                    (wave.SpawnLevelNumber + 1),
                                                    (wave.SpawnWaveNumber + 1),
                                                    beatMod._statName,
                                                    beatMod._modValueFloatAmt.worldVariableName));
                                        }
                                        _spawnerValid = false;
                                    }
                                }

                                break;
                            default:
                                LevelSettings.LogIfNew("Add code for varType: " + beatMod._varTypeToUse.ToString());
                                break;
                        }
                    }
                }

            }
        }

        private void LogAdjustments(int adjustments) {
            if (adjustments > 0) {
                Debug.Log(string.Format("Adjusted {0} wave(s) in spawner '{1}' to match new Level/Wave numbers",
                    adjustments, name));
            }
        }

        private bool WillRepeatWave() {
            var allPassed = true;

            switch (_currentWave.curWaveRepeatMode) {
                case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
                case WaveSpecifics.RepeatWaveMode.Endless:
                    return true;
                case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _currentWave.repeatPassCriteria.statMods.Count; i++) {
                        var stat = _currentWave.repeatPassCriteria.statMods[i];

                        if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                            LevelSettings.LogIfNew(
                                string.Format(
                                    "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                    _trans.name,
                                    stat._statName));
                            continue;
                        }

                        var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                        if (variable == null) {
                            continue;
                        }
                        var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? variable.CurrentIntValue
                            : variable.CurrentFloatValue;
                        var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? stat._modValueIntAmt.Value
                            : stat._modValueFloatAmt.Value;

                        if (varVal >= compareVal) {
                            continue;
                        }

                        allPassed = false;
                        break;
                    }

                    if (!allPassed) {
                        return true;
                    }

                    break;
                case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _currentWave.repeatPassCriteria.statMods.Count; i++) {
                        var stat = _currentWave.repeatPassCriteria.statMods[i];

                        if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                            LevelSettings.LogIfNew(
                                string.Format(
                                    "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                    _trans.name,
                                    stat._statName));
                            continue;
                        }

                        var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                        if (variable == null) {
                            continue;
                        }
                        var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? variable.CurrentIntValue
                            : variable.CurrentFloatValue;
                        var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? stat._modValueIntAmt.Value
                            : stat._modValueFloatAmt.Value;

                        if (varVal <= compareVal) {
                            continue;
                        }

                        allPassed = false;
                        break;
                    }

                    if (!allPassed) {
                        return true;
                    } 

                    break;
            }

            return false;
        }

        private void MaybeRepeatWave() {
            var allPassed = true;

            if (LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Elimination) {
                switch (_currentWave.curWaveRepeatMode) {
                    case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
                    case WaveSpecifics.RepeatWaveMode.Endless:
                        SpawnBonusPrefabIfSelected(_lastPrefabKilled.position);
                        WaveRepeat();
                        break;
                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _currentWave.repeatPassCriteria.statMods.Count; i++) {
                            var stat = _currentWave.repeatPassCriteria.statMods[i];

                            if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                                LevelSettings.LogIfNew(
                                    string.Format(
                                        "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                        _trans.name,
                                        stat._statName));
                                continue;
                            }

                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                continue;
                            }
                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;
                            var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntAmt.Value
                                : stat._modValueFloatAmt.Value;

                            if (varVal >= compareVal) {
                                continue;
                            }

                            allPassed = false;
                            break;
                        }

                        if (!allPassed) {
                            SpawnBonusPrefabIfSelected(_lastPrefabKilled.position);
                            WaveRepeat();
                        } else {
                            SpawnBonusPrefabIfAny(_lastPrefabKilled.position);
                            LevelSettings.EliminationSpawnerCompleted(_instanceId, _lastPrefabKilled);
                            // since this never happens above due to infinite repetitions
                        }
                        break;
                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _currentWave.repeatPassCriteria.statMods.Count; i++) {
                            var stat = _currentWave.repeatPassCriteria.statMods[i];

                            if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                                LevelSettings.LogIfNew(
                                    string.Format(
                                        "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                        _trans.name,
                                        stat._statName));
                                continue;
                            }

                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                continue;
                            }
                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;
                            var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntAmt.Value
                                : stat._modValueFloatAmt.Value;

                            if (varVal <= compareVal) {
                                continue;
                            }

                            allPassed = false;
                            break;
                        }

                        if (!allPassed) {
                            SpawnBonusPrefabIfSelected(_lastPrefabKilled.position);
                            WaveRepeat();
                        } else {
                            SpawnBonusPrefabIfAny(_lastPrefabKilled.position);
                            LevelSettings.EliminationSpawnerCompleted(_instanceId, _lastPrefabKilled);
                            // since this never happens above due to infinite repetitions
                        }
                        break;
                }
            } else if (LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Timed) {
                switch (_currentWave.curTimedRepeatWaveMode) {
                    case WaveSpecifics.TimedRepeatWaveMode.EliminationStyle:
                        SpawnBonusPrefabIfSelected(_lastPrefabKilled.position);
                        WaveRepeat();
                        break;
                    case WaveSpecifics.TimedRepeatWaveMode.StrictTimeStyle:
                        SpawnBonusPrefabIfSelected(_lastPrefabKilled.position);
                        WaveRepeat();
                        break;
                }
            }
        }

        private bool SetupNextWave(bool scanForWave, bool isRestart) {
            _repeatTimer = null;

            if (activeMode == LevelSettings.ActiveItemMode.Never) {
                // even in repeating waves.
                return false;
            }

            if (isRestart && _currentWave == null) {
                return false; // can't restart because the current wave isn't configured in this Spawner.
            }

            var shouldInit = scanForWave || isRestart;

            if (scanForWave && !isRestart) {
                // find wave
                _settingUpWave = true;
                _currentWave = FindWave(LevelSettings.CurrentLevel, LevelSettings.CurrentWaveInfo.sequencedWaveNumber);

                // validate for all things that could go wrong!
                if (_currentWave == null || !_currentWave.enableWave) {
                    return false;
                }

                // check "active mode" for conditions
                switch (activeMode) {
                    case LevelSettings.ActiveItemMode.Never:
                        return false;
                    case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
                        if (activeItemCriteria.statMods.Count == 0) {
                            return false;
                        }
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                            var stat = activeItemCriteria.statMods[i];
                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                return false;
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
                                    "The Min cannot be greater than the Max for Active Item Limit in Syncro Spawner '" +
                                    _trans.name + "'.");
                                return false;
                            }

                            if (varVal < min || varVal > max) {
                                return false;
                            }
                        }

                        break;
                    case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                        if (activeItemCriteria.statMods.Count == 0) {
                            return false;
                        }
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                            var stat = activeItemCriteria.statMods[i];
                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                return false;
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
                                    "The Min cannot be greater than the Max for Active Item Limit in Syncro Spawner '" +
                                    _trans.name + "'.");
                                return false;
                            }

                            if (varVal >= min && varVal <= max) {
                                return false;
                            }
                        }

                        break;
                }

                if (_currentWave.MinToSpwn.Value == 0 || _currentWave.MaxToSpwn.Value == 0) {
                    return false;
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (scanForWave &&
                    _currentWave.WaveDelaySec.Value + _currentWave.TimeToSpawnEntireWave.Value >=
                    LevelSettings.CurrentWaveInfo.waveDurationFlex.Value &&
                    LevelSettings.CurrentWaveInfo.waveType == LevelSettings.WaveType.Timed) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Wave TimeToSpawnWholeWave plus Wave DelaySeconds must be less than the current LevelSettings wave duration, occured in spawner: {0}, wave# {1}, level {2}.",
                            name,
                            _currentWave.SpawnWaveNumber + 1,
                            _currentWave.SpawnLevelNumber + 1));
                    return false;
                }

                if (_currentWave.MinToSpwn.Value > _currentWave.MaxToSpwn.Value) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Wave MinToSpawn cannot be greater than Wave MaxToSpawn, occured in spawner: {0}, wave# {1}, level {2}.",
                            name,
                            _currentWave.SpawnWaveNumber + 1,
                            _currentWave.SpawnLevelNumber + 1));
                    return false;
                }

                if (_currentWave.repeatWaveUntilNew &&
                    _currentWave.repeatPauseMinimum.Value > _currentWave.repeatPauseMaximum.Value) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Wave Repeat Pause Min cannot be greater than Wave Repeat Pause Max, occurred in spawner: {0}, wave# {1}, level {2}.",
                            name,
                            _currentWave.SpawnWaveNumber + 1,
                            _currentWave.SpawnLevelNumber + 1));
                    return false;
                }
            }

            if (LevelSettings.IsLoggingOn) {
                var waveStatus = isRestart ? "Restarting" : string.Empty;
                if (string.IsNullOrEmpty(waveStatus)) {
                    waveStatus = scanForWave ? "Starting" : "Repeating";
                }

                Debug.Log(string.Format("{0} matching wave from spawner: {1}, wave# {2}, level {3}.",
                    waveStatus,
                    name,
                    _currentWave.SpawnWaveNumber + 1,
                    _currentWave.SpawnLevelNumber + 1));
            }

            if (_currentWave.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool) {
                var poolTrans = LevelSettings.GetFirstMatchingPrefabPool(_currentWave.prefabPoolName);
                if (poolTrans == null) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Spawner '{0}' wave# {1}, level {2} is trying to use a Prefab Pool that can't be found.",
                            name,
                            _currentWave.SpawnWaveNumber + 1,
                            _currentWave.SpawnLevelNumber + 1));
                    _spawnerValid = false;
                    _currentWave = null;
                    return false;
                }

                _wavePool = poolTrans;
            } else {
                _wavePool = null;
            }

            _settingUpWave = false;

            CheckForValidVariablesForWave(_currentWave);

            _spawnedWaveMembers.Clear();

            _currentWaveSize = Random.Range(_currentWave.MinToSpwn.Value, _currentWave.MaxToSpwn.Value);
            _currentWaveLength = _currentWave.TimeToSpawnEntireWave.Value;
            var origCurrentWaveSize = _currentWaveSize;

            _itemsToCompleteWave = (int)(_currentWaveSize * _currentWave.waveCompletePercentage * .01f);

            if (_currentWave.repeatWaveUntilNew) {
                if (shouldInit &&
                    (LevelSettings.CurrentWaveInfo.waveType != LevelSettings.WaveType.Elimination ||
                     _currentWave.curWaveRepeatMode == WaveSpecifics.RepeatWaveMode.Endless)) {
                    // only the first time!
                    _currentWave.repetitionsToDo.Value = int.MaxValue;
                }

                if (_currentWave.repeatItemInc.Value != 0) {
                    _currentWaveSize += _waveRepetitionNumberWithReset * _currentWave.repeatItemInc.Value;

                    var resetQty = false;

                    if (_currentWaveSize < _currentWave.repeatItemMinLmt.Value) {
                        if (_currentWave.resetOnItemLimitReached) {
                            resetQty = true;
                        } else {
                            _currentWaveSize = _currentWave.repeatItemMinLmt.Value;
                        }
                    } else if (_currentWaveSize > _currentWave.repeatItemLmt.Value) {
                        if (_currentWave.resetOnItemLimitReached) {
                            resetQty = true;
                        } else {
                            _currentWaveSize = _currentWave.repeatItemLmt.Value;
                        }
                    }

                    if (resetQty) {
                        _currentWaveSize= origCurrentWaveSize;
                        _waveRepetitionNumberWithReset = 0;
                    }
                }

                if (_currentWave.repeatTimeInc.Value != 0) {
                    _currentWaveLength += _waveRepetitionNumberWithReset * _currentWave.repeatTimeInc.Value;

                    var resetTime = false;

                    if (_currentWaveLength < _currentWave.repeatTimeMinLmt.Value) {
                        if (_currentWave.resetOnTimeLimitReached) {
                            resetTime = true;
                        } else {
                            _currentWaveLength = _currentWave.repeatTimeMinLmt.Value;
                        }
                    } else if (_currentWaveLength > _currentWave.repeatTimeLmt.Value) {
                        if (_currentWave.resetOnTimeLimitReached) {
                            resetTime = true;
                        } else {
                            _currentWaveLength = _currentWave.repeatTimeLmt.Value;
                        }
                    }

                    if (resetTime) {
                        _currentWaveLength = _currentWave.TimeToSpawnEntireWave.Value;
                        _waveRepetitionNumberWithReset = 0;
                    }
                }
            }

            _currentWaveLength = Math.Max(0f, _currentWaveLength);

            if (shouldInit) {
                // not on wave repeat!
                _waveRepetitionNumber = 0;
                _waveRepetitionNumberWithReset = 0;
            }

            _waveStartTime = Time.time;
            _waveFinishedSpawning = false;
            _levelSettingsNotifiedOfCompletion = false;
            _countSpawned = 0;
            _singleSpawnTime = _currentWaveLength / _currentWaveSize;

            var delay = WaveDelay;

            _lastSpawnTime = _waveStartTime + delay - _singleSpawnTime; // this should make the first item spawn immediately.

            if (_currentWave.enableLimits) {
                _currentRandomLimitDistance = Random.Range(-_currentWave.doNotSpawnRandomDist.Value,
                    _currentWave.doNotSpawnRandomDist.Value);
            }

            return true;
        }

        private float WaveDelay {
            get {
                var delay = _currentWave.WaveDelaySec.Value;
                if (_waveRepetitionNumber > 0 && !_currentWave.doesRepeatUseWaveDelay) {
                    delay = 0;
                }
                return delay;
            }
        }

        private Transform SpawnOne(bool fromExternalScript = false) {
            if (fromExternalScript && _currentWave == null) {
                return null; // no active spawner for this wave.
            }

            var prefabToSpawn = GetSpawnable(_currentWave);

            if (_currentWave.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool && prefabToSpawn == null) {
                return null;
            }

            if (prefabToSpawn == null) {
                if (fromExternalScript) {
                    LevelSettings.LogIfNew("Cannot 'spawn one' from spawner: " + _trans.name +
                                           " because it has either no settings or selected prefab to spawn for the current wave.");
                    return null;
                }

                LevelSettings.LogIfNew(string.Format(
                    "Spawner '{0}' has no prefab to spawn for wave# {1} on level# {2}.",
                    name,
                    _currentWave.SpawnWaveNumber + 1,
                    _currentWave.SpawnLevelNumber + 1));

                _spawnerValid = false;
                return null;
            }

            var spawnPosition = GetSpawnPosition(_trans.position, _countSpawned);

            var spawnedPrefab = SpawnWaveItem(prefabToSpawn, spawnPosition,
                GetSpawnRotation(prefabToSpawn, _countSpawned));

            if (spawnedPrefab == null) {
                LevelSettings.LogIfNew("Could not spawn: " + prefabToSpawn);
                // in case you might want to increase your pool size so this doesn't happen. If not, comment out this line.
                if (listener != null) {
                    listener.ItemFailedToSpawn(prefabToSpawn);
                }
                return null;
            } else {
                SpawnUtility.RecordSpawnerObjectIfKillable(spawnedPrefab, _go);
            }

            AddSpawnTracker(spawnedPrefab);
            AfterSpawn(spawnedPrefab);

            _spawnedWaveMembers.Add(spawnedPrefab);
            LevelSettings.AddWaveSpawnedItem(spawnedPrefab);
            _countSpawned++;

            if (_currentWave.enableLimits) {
                _currentRandomLimitDistance = Random.Range(-_currentWave.doNotSpawnRandomDist.Value,
                    _currentWave.doNotSpawnRandomDist.Value);
            }

            return spawnedPrefab;
        }

        private void CheckForWaveRepeat() {
            if (GameIsOverForSpawner || _currentWave == null || !_spawnerValid || !_waveFinishedSpawning || !_currentWave.IsValid) {
                return;
            }

			if (_currentWave.repeatWaveUntilNew && LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Timed) {
                if (!_repeatTimer.HasValue) {
                    _repeatTimer = Time.time;
                    _repeatWaitTime = Random.Range(_currentWave.repeatPauseMinimum.Value, _currentWave.repeatPauseMaximum.Value);
                } else if (Time.time - _repeatTimer.Value > _repeatWaitTime) {
                    _waveRepetitionNumber++;
                   _waveRepetitionNumberWithReset++;

                    MaybeRepeatWave();
                }
                return;
            }

            var hasNoneLeft = _spawnedWaveMembers.Count == 0; // also used for "elimination style" with timed waves

            if (LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Elimination && _currentWave.waveCompletePercentage < 100) {
                if (_itemsToCompleteWave <= 0) {
                    // end wave early
                    hasNoneLeft = true;
                }
            }

            if (!hasNoneLeft) {
                return;
            }

            if (LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Elimination &&
                ((_currentWave.curWaveRepeatMode == WaveSpecifics.RepeatWaveMode.NumberOfRepetitions &&
                  _waveRepetitionNumber + 1 >= _currentWave.repetitionsToDo.Value) || !_currentWave.repeatWaveUntilNew)) {

                if (_levelSettingsNotifiedOfCompletion) {
                    return;
                }
                _levelSettingsNotifiedOfCompletion = true;

                if (listener != null) {
                    listener.EliminationWaveCompleted(_currentWave);
                }

                SpawnBonusPrefabIfAny(_lastPrefabKilled.position);
                LevelSettings.EliminationSpawnerCompleted(_instanceId, _lastPrefabKilled);
            } else if (_currentWave.repeatWaveUntilNew) {
                if (!_repeatTimer.HasValue) {
                    if (listener != null) {
                        listener.EliminationWaveCompleted(_currentWave);
                    }

                    _repeatTimer = Time.time;
                    var willRepeatWave = WillRepeatWave();
                    if (LevelSettings.PreviousWaveInfo.waveType == LevelSettings.WaveType.Elimination && willRepeatWave) {
                        _repeatWaitTime = Random.Range(_currentWave.repeatPauseMinimum.Value, _currentWave.repeatPauseMaximum.Value);
                    } else {
                        _repeatWaitTime = 0;
                    }
                } else if (Time.time - _repeatTimer.Value > _repeatWaitTime) {
                    _waveRepetitionNumber++;
                    _waveRepetitionNumberWithReset++;

                    MaybeRepeatWave();
                }
            } else if (!_currentWave.repeatWaveUntilNew) {
                SpawnBonusPrefabIfAny(_lastPrefabKilled.position);
                LevelSettings.EliminationSpawnerCompleted(_instanceId, _lastPrefabKilled);
            }
        }

        private static Vector3 GetSpawnPositionForVisualization(WaveSpecifics wave, Vector3 pos, int itemSpawnedIndex) {
            switch (wave.positionXmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.x = wave.customPosX.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectX != null) {
                        pos.x = wave.otherObjectX.position.x;
                    }
                    break;
            }

            switch (wave.positionYmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.y = wave.customPosY.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectY != null) {
                        pos.y = wave.otherObjectY.position.y;
                    }
                    break;
            }

            switch (wave.positionZmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.z = wave.customPosZ.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectZ != null) {
                        pos.z = wave.otherObjectZ.position.z;
                    }
                    break;
            }

            var addVector = Vector3.zero;

            addVector += wave.WaveOffset;

            if (wave.enableRandomizations) {
                addVector.x += Random.Range(-wave.randomDistX.Value, wave.randomDistX.Value);
                addVector.y += Random.Range(-wave.randomDistY.Value, wave.randomDistY.Value);
                addVector.z += Random.Range(-wave.randomDistZ.Value, wave.randomDistZ.Value);
            }

            if (!wave.enableIncrements || itemSpawnedIndex <= 0) {
                return pos + addVector;
            }
            addVector.x += (wave.incrementPositionX.Value * itemSpawnedIndex);
            addVector.y += (wave.incrementPositionY.Value * itemSpawnedIndex);
            addVector.z += (wave.incrementPositionZ.Value * itemSpawnedIndex);

            return pos + addVector;
        }

        private static Quaternion GetSpawnRotationForVisualization(WaveSpecifics wave, Transform spawner,
            int itemSpawnedIndex) {
            var euler = Vector3.zero;

            switch (wave.curRotationMode) {
                case WaveSpecifics.RotationMode.UsePrefabRotation:
                    break;
                case WaveSpecifics.RotationMode.UseSpawnerRotation:
                    euler = spawner.transform.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.CustomRotation:
                    euler = wave.customRotation;
                    break;
            }

            if (wave.enableRandomizations && wave.randomXRotation) {
                euler.x = Random.Range(wave.randomXRotMin.Value, wave.randomXRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.x += (itemSpawnedIndex * wave.incrementRotX.Value -
                                (wave.MinToSpwn.Value * wave.incrementRotX.Value * .5f));
                } else {
                    euler.x += (itemSpawnedIndex * wave.incrementRotX.Value);
                }
            }

            if (wave.enableRandomizations && wave.randomYRotation) {
                euler.y = Random.Range(wave.randomYRotMin.Value, wave.randomYRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.y += (itemSpawnedIndex * wave.incrementRotY.Value -
                                (wave.MinToSpwn.Value * wave.incrementRotY.Value * .5f));
                } else {
                    euler.y += (itemSpawnedIndex * wave.incrementRotY.Value);
                }
            }

            if (wave.enableRandomizations && wave.randomZRotation) {
                euler.z = Random.Range(wave.randomZRotMin.Value, wave.randomZRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.z += (itemSpawnedIndex * wave.incrementRotZ.Value -
                                (wave.MinToSpwn.Value * wave.incrementRotZ.Value * .5f));
                } else {
                    euler.z += (itemSpawnedIndex * wave.incrementRotZ.Value);
                }
            }

            return Quaternion.Euler(euler);
        }

        private void AfterSpawnForVisualization(WaveSpecifics wave, Transform spawnedTrans) {
            if (wave.enablePostSpawnNudge) {
                spawnedTrans.Translate(Vector3.forward * wave.postSpawnNudgeFwd.Value);
                spawnedTrans.Translate(Vector3.right * wave.postSpawnNudgeRgt.Value);
                spawnedTrans.Translate(Vector3.down * wave.postSpawnNudgeDwn.Value);
            }

            switch (spawnLayerMode) {
                case SpawnLayerTagMode.UseSpawnerSettings:
                    spawnedTrans.gameObject.layer = gameObject.layer;
                    break;
                case SpawnLayerTagMode.Custom:
                    spawnedTrans.gameObject.layer = spawnCustomLayer;
                    break;
            }

            switch (spawnTagMode) {
                case SpawnLayerTagMode.UseSpawnerSettings:
                    spawnedTrans.gameObject.tag = gameObject.tag;
                    break;
                case SpawnLayerTagMode.Custom:
                    spawnedTrans.gameObject.tag = spawnCustomTag;
                    break;
            }
        }

        #endregion

        #region Properties
        /// <summary>
        /// This property will return true if the Game Over Behavior setting makes this spawner disabled.
        /// </summary>
        public bool GameIsOverForSpawner {
            get { return LevelSettings.IsGameOver && gameOverBehavior == TriggeredSpawner.GameOverBehavior.Disable; }
        }

        /// <summary>
        /// This property will return true if the Wave Pause Behavior setting makes this spawner paused.
        /// </summary>
        public bool SpawnerIsPaused {
            get {
                return LevelSettings.WavesArePaused && wavePauseBehavior == TriggeredSpawner.WavePauseBehavior.Disable;
            }
        }
        #endregion
    }
}