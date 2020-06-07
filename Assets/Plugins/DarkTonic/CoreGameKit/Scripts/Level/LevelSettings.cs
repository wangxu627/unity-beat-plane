using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif 

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to set up global settings and configure levels and waves for Syncro Spawners.
    /// </summary>
    // ReSharper disable once CheckNamespace
    [CoreScriptOrder(-80)]
    public class LevelSettings : MonoBehaviour {
        #region Variables, constants and enums

        /*! \cond PRIVATE */
        public const int DefaultWaveDuration = 5;
        public const string DynamicEventName = "[Type In]";
        public const string NoEventName = "[None]";
        public const string EmptyValue = "[Empty]";
        public const string NoCategory = "[Uncategorized]";
        public const string KillerPoolingContainerTransName = "PoolBoss";
        public const string PrefabPoolsContainerTransName = "PrefabPools";
        public const string SpawnerContainerTransName = "Spawners";
        public const string WorldVariablesContainerTransName = "WorldVariables";
        public const string DropDownNoneOption = "-None-";
        public const string RevertLevelSettingsAlert = "Please revert your LevelSettings prefab.";

        public const string NoSpawnContainerAlert =
            "You have no '" + SpawnerContainerTransName + "' prefab under LevelSettings. " + RevertLevelSettingsAlert;

        public const string NoPrefabPoolsContainerAlert =
            "You have no '" + PrefabPoolsContainerTransName + "' prefab under LevelSettings. " +
            RevertLevelSettingsAlert;

        public const string NoWorldVariablesContainerAlert =
            "You have no '" + WorldVariablesContainerTransName + "' prefab under LevelSettings. " +
            RevertLevelSettingsAlert;

        // reduce this to check for spawner activations more often. This is set to ~10x a second.
        private const float WaveCheckInterval = .1f;
        private const int MaxComponents = 20;

        // ReSharper disable InconsistentNaming
        public bool useMusicSettings = true;
        public bool showLevelSettings = true;
        public bool showCustomEvents = true;
        public bool gameStatsExpanded = false;
        public string newEventName = "my event";
        public LevelSettingsListener listener;
        public Transform RedSpawnerTrans;
        public Transform GreenSpawnerTrans;
        public Transform PrefabPoolTrans;
        public string newSpawnerName = "spawnerName";
        public bool newPrefabPoolExpanded = true;
        public string newPrefabPoolName = "EnemiesPool";
        public SpawnerType newSpawnerType = SpawnerType.Green;
        public LevelWaveMusicSettings gameOverMusicSettings = new LevelWaveMusicSettings();
        public bool spawnersExpanded = true;
        public bool createSpawnerExpanded = true;
        public bool createPrefabPoolsExpanded = true;
        public bool killerPoolingExpanded = true;
        public bool disableSyncroSpawners = false;
        public bool startFirstWaveImmediately = true;
        public WaveRestartBehavior waveRestartMode = WaveRestartBehavior.LeaveSpawned;
        public bool enableWaveWarp;
        public KillerInt startLevelNumber = new KillerInt(1, 1, int.MaxValue);
        public KillerInt startWaveNumber = new KillerInt(1, 1, int.MaxValue);
        public bool persistBetweenScenes = false;
        public bool isLoggingOn = false;
        public List<LevelSpecifics> LevelTimes = new List<LevelSpecifics>();
        public bool useWaves = true;
        public bool showCustomWaveClasses;
        public List<string> customWaveClasses = new List<string>();
        public LevelLoopMode repeatLevelMode = LevelLoopMode.Win;
        public bool useWaveNameFilter;
        public string waveNameFilterText;
        public List<CgkCustomEvent> customEvents = new List<CgkCustomEvent>();
        public List<CGKCustomEventCategory> customEventCategories = new List<CGKCustomEventCategory> {
                new CGKCustomEventCategory()
            };
        public string newCustomEventCategoryName = "New Category";
        public string addToCustomEventCategoryName = "New Category";

        public bool initializationSettingsExpanded = false;
        public List<CGKCustomEventToFire> initializationCustomEvents = new List<CGKCustomEventToFire>();
        public bool waveDurationsCopied;

        // ReSharper restore InconsistentNaming

        public static readonly List<string> IllegalVariableNames = new List<string>()
        {
            DropDownNoneOption,
            string.Empty
        };

        private static LevelSettings _lsInstance;
        private static Dictionary<int, List<LevelWave>> _waveSettingsByLevel = new Dictionary<int, List<LevelWave>>();
        private static int _currentLevel;
        private static int _currentLevelWave;
		private static int _displayCurrentLevel;
		private static int _displayCurrentWave;
        private static bool _gameIsOver;
        private static bool _hasPlayerWon;
        private static bool _wavesArePaused;
		private static float _lastWavePauseTime = 0f;
        private static float _wavePausedTime = 0f;
        private static LevelWave _previousWave;
        private static readonly List<TrigSpawnerWaveWaiter> TriggeredWavesToAwait = new List<TrigSpawnerWaveWaiter>();

        private static readonly Dictionary<int, WaveSyncroPrefabSpawner> EliminationSpawnersUnkilled =
            new Dictionary<int, WaveSyncroPrefabSpawner>();

        private static bool _skippingWaveForRestart;
        private static bool _skipCurrentWave;
        private static readonly List<Transform> SpawnedItemsRemaining = new List<Transform>();
        private static int _waveTimeRemaining;
        private static readonly Dictionary<string, float> RecentErrorsByTime = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private static readonly List<RespawnTimer> PrefabsToRespawn = new List<RespawnTimer>();

        private static readonly Dictionary<int, object> CustomEventParams = new Dictionary<int, object>();
        private static readonly Dictionary<int, Transform> CustomEventParamSenders = new Dictionary<int, Transform>();
        private static readonly List<CustomEventCandidate> ValidReceivers = new List<CustomEventCandidate>(10);

        private static readonly Dictionary<string, Dictionary<ICgkEventReceiver, Transform>> ReceiversByEventName =
            new Dictionary<string, Dictionary<ICgkEventReceiver, Transform>>(StringComparer.OrdinalIgnoreCase);

        private static Transform _trans;

        private readonly List<WaveSyncroPrefabSpawner> _syncroSpawners = new List<WaveSyncroPrefabSpawner>();
        private bool _initCustomEventsFired;
        private bool _isValid;
        private float _lastWaveChangeTime;
        private bool _hasFirstWaveBeenStarted;

        // ReSharper disable once InconsistentNaming
        public int _frames;
        public static readonly YieldInstruction EndOfFrameDelay = new WaitForEndOfFrame();
        private readonly YieldInstruction _loopDelay = new WaitForSeconds(WaveCheckInterval);

        public enum RepeatToUseItem {
            All,
            Final
        }

        public enum WaveSpawnerUseMode {
            AllAbove,
            RandomSubset
        }

        public enum LevelLoopMode {
            Win,
            RepeatAllLevelsFromBeginning
        }

        public enum EventReceiveMode {
            Always,
            WhenDistanceLessThan,
            WhenDistanceMoreThan,
            Never,
            OnSameGameObject,
            OnChildGameObject,
            OnParentGameObject,
            OnSameOrChildGameObject,
            OnSameOrParentGameObject
        }

        public enum EventReceiveFilter {
            All,
            Closest,
            Random
        }

        public enum WaveOrder {
            SpecifiedOrder,
            RandomOrder
        }

        public enum WaveRestartBehavior {
            LeaveSpawned,
            DestroySpawned,
            DespawnSpawned
        }

        public enum VariableSource {
            Variable,
            Value
        }

        public enum WaveMusicMode {
            KeepPreviousMusic,
            PlayNew,
            Silence
        }

        public enum ActiveItemMode {
            Always,
            Never,
            IfWorldVariableInRange,
            IfWorldVariableOutsideRange
        }

        public enum SkipWaveMode {
            None,
            Always,
            IfWorldVariableValueAbove,
            IfWorldVariableValueBelow,
        }

        public enum WaveType {
            Timed,
            Elimination
        }

        public enum SpawnerType {
            Green,
            Red
        }

        public enum RotationType {
            Identity,
            CustomEuler,
            SpawnerRotation
        }

        public enum SpawnPositionMode {
            UseVector3,
            UseThisObjectPosition,
            UseOtherObjectPosition
        }
        /*! \endcond */

        #endregion

        #region classes and structs

        /*! \cond PRIVATE */
        public struct RespawnTimer {
            public float TimeToRespawn;
            public Transform PrefabToRespawn;
            public Vector3 Position;
            public bool HasRespawnCustomEvents;

            public RespawnTimer(float timeToWait, Transform prefab, Vector3 position, bool hasRespawnCustomEvents) {
                TimeToRespawn = Time.realtimeSinceStartup + timeToWait;
                PrefabToRespawn = prefab;
                Position = position;
                HasRespawnCustomEvents = hasRespawnCustomEvents;
            }
        }

        public struct CustomEventCandidate {
            public float DistanceAway;
            public ICgkEventReceiver Receiver;
            public Transform Trans;
            public int RandomId;

            public CustomEventCandidate(float distance, ICgkEventReceiver rec, Transform trans, int randomId) {
                DistanceAway = distance;
                Receiver = rec;
                Trans = trans;
                RandomId = randomId;
            }
        }
        /*! \endcond */
        #endregion

        #region MonoBehavior Events

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            var ls = FindObjectsOfType(typeof(LevelSettings));
            if (ls.Length > 1) {
                Destroy(gameObject);
                return;
            }

            useGUILayout = false;
            _trans = transform;

            _hasFirstWaveBeenStarted = false;
            _isValid = true;
            _wavesArePaused = false;
            var iLevel = 0;
            _currentLevel = 0;
            _currentLevelWave = 0;
			_displayCurrentLevel = 0;
			_displayCurrentWave = 0;
			_previousWave = null;
            _skipCurrentWave = false;
            _skippingWaveForRestart = false;

            if (persistBetweenScenes) {
                DontDestroyOnLoad(gameObject);
            }

#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += LevelWasLoaded;
#endif

            if (useWaves) {
                if (LevelTimes.Count == 0) {
                    LogIfNew("NO LEVEL / WAVE TIMES DEFINED. ABORTING.");
                    _isValid = false;
                    return;
                }
                if (LevelTimes[0].WaveSettings.Count == 0) {
                    LogIfNew("NO LEVEL 1 / WAVE 1 TIME DEFINED! ABORTING.");
                    _isValid = false;
                    return;
                }
            }

            var levelSettingScripts = FindObjectsOfType(typeof(LevelSettings));
            if (levelSettingScripts.Length > 1) {
                LogIfNew(
                    "You have more than one LevelWaveSettings prefab in your scene. Please delete all but one. Aborting.");
                _isValid = false;
                return;
            }

            _waveSettingsByLevel = new Dictionary<int, List<LevelWave>>();

            // ReSharper disable once TooWideLocalVariableScope
            List<LevelWave> waveLs;

            for (var i = 0; i < LevelTimes.Count; i++) {
                var level = LevelTimes[i];

                if (level.WaveSettings.Count == 0) {
                    LogIfNew("NO WAVES DEFINED FOR LEVEL: " + (iLevel + 1));
                    _isValid = false;
                    continue;
                }

                waveLs = new List<LevelWave>();
                LevelWave newLevelWave;

                var w = 0;

                foreach (var waveSetting in level.WaveSettings) {
                    if (waveSetting.waveDurationFlex.Value <= 0) {
                        LogIfNew("WAVE DURATION CANNOT BE ZERO OR LESS - OCCURRED IN LEVEL " + (i + 1) + ".");
                        _isValid = false;
                        return;
                    }

                    newLevelWave = new LevelWave {
                        waveType = waveSetting.waveType,
                        WaveDuration = waveSetting.WaveDuration,
                        waveDurationFlex = waveSetting.waveDurationFlex,
                        musicSettings = new LevelWaveMusicSettings {
                            WaveMusicMode = waveSetting.musicSettings.WaveMusicMode,
                            WaveMusicVolume = waveSetting.musicSettings.WaveMusicVolume,
                            WaveMusic = waveSetting.musicSettings.WaveMusic,
                            FadeTime = waveSetting.musicSettings.FadeTime
                        },
                        waveName = waveSetting.waveName,
                        waveDefeatVariableModifiers = waveSetting.waveDefeatVariableModifiers,
                        useCompletionEvents = waveSetting.useCompletionEvents,
                        completionCustomEvents = waveSetting.completionCustomEvents,
                        waveBeatBonusesEnabled = waveSetting.waveBeatBonusesEnabled,
                        skipWaveType = waveSetting.skipWaveType,
                        skipWavePassCriteria = waveSetting.skipWavePassCriteria,
                        sequencedWaveNumber = w,
                        endEarlyIfAllDestroyed = waveSetting.endEarlyIfAllDestroyed,
                        pauseGlobalWavesWhenCompleted = waveSetting.pauseGlobalWavesWhenCompleted,
                        useSpawnBonusPrefab = waveSetting.useSpawnBonusPrefab,
                        bonusPrefabSource = waveSetting.bonusPrefabSource,
                        bonusPrefabPoolIndex = waveSetting.bonusPrefabPoolIndex,
                        bonusPrefabPoolName = waveSetting.bonusPrefabPoolName,
                        bonusPrefabSpecific = waveSetting.bonusPrefabSpecific,
                        bonusPrefabCategoryName = waveSetting.bonusPrefabCategoryName,
                        bonusPrefabSpawnPercent = waveSetting.bonusPrefabSpawnPercent,
                        bonusPrefabQty = waveSetting.bonusPrefabQty,
                        spawnerUseMode = waveSetting.spawnerUseMode,
                        spawnersToUseMin = waveSetting.spawnersToUseMin,
                        spawnersToUseMax = waveSetting.spawnersToUseMax,
                        waveClass = waveSetting.waveClass,
                        useTriggeredSpawners = waveSetting.useTriggeredSpawners,
                        trigSpawnerWavesToAwait = waveSetting.trigSpawnerWavesToAwait
                    };

                    if (waveSetting.waveType == WaveType.Elimination) {
                        newLevelWave.WaveDuration = 500; // super long to recognize this problem if it occurs.
                        newLevelWave.waveDurationFlex.selfValue = 500;
                    }

                    waveLs.Add(newLevelWave);
                    w++;
                }

                var sequencedWaves = new List<LevelWave>();

                switch (level.waveOrder) {
                    case WaveOrder.SpecifiedOrder:
                        sequencedWaves.AddRange(waveLs);
                        break;
                    case WaveOrder.RandomOrder:
                        while (waveLs.Count > 0) {
                            var randIndex = Random.Range(0, waveLs.Count);
                            sequencedWaves.Add(waveLs[randIndex]);
                            waveLs.RemoveAt(randIndex);
                        }
                        break;
                }

                if (i == LevelTimes.Count - 1) {
                    // extra bogus wave so that the real last wave will get run
                    newLevelWave = new LevelWave() {
                        musicSettings = new LevelWaveMusicSettings() {
                            WaveMusicMode = WaveMusicMode.KeepPreviousMusic,
                            WaveMusic = null
                        },
                        WaveDuration = 1,
                        sequencedWaveNumber = w,
                        isDummyWave = true
                    };
                    newLevelWave.waveDurationFlex.selfValue = 1;

                    sequencedWaves.Add(newLevelWave);
                }

                _waveSettingsByLevel.Add(iLevel, sequencedWaves);

                iLevel++;
            }

            // ReSharper disable once TooWideLocalVariableScope
            WaveSyncroPrefabSpawner spawner;

            foreach (var gObj in GetAllSpawners) {
                spawner = gObj.GetComponent<WaveSyncroPrefabSpawner>();

                _syncroSpawners.Add(spawner);
            }

            _waveTimeRemaining = 0;
            SpawnedItemsRemaining.Clear();

            _gameIsOver = false;
            _hasPlayerWon = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void OnApplicationQuit() {
            AppIsShuttingDown = true; // very important!! Dont' take this out, false debug info will show up.
            WorldVariableTracker.FlushAll();
        }

        private void OnApplicationPause(bool paused) { // for Android existing via back button
            if (!paused) {
                return;
            }

            WorldVariableTracker.FlushAll();
        }

#if UNITY_5_4_OR_NEWER
        private static void LevelWasLoaded(Scene scene, LoadSceneMode mode) {
            WorldVariableTracker.FlushAll();
        }
#else
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void OnLevelWasLoaded(int level) {
            WorldVariableTracker.FlushAll();
        }
#endif

        // ReSharper disable once UnusedMember.Local
        private void OnDisable() {
            WorldVariableTracker.FlushAll();
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            if (!CheckForValidVariables()) {
                _isValid = false;
            }

            if (!startFirstWaveImmediately) {
                _wavesArePaused = true;
            }

            if (_isValid) {
                StartCoroutine(CoUpdate());
            }
        }

        #endregion

        #region Helper Methods

        private bool CheckForValidVariables() {
            if (!useWaves) {
                return true; // don't bother checking
            }

            // check for valid custom start level
            if (enableWaveWarp) {
                var startLevelNum = startLevelNumber.Value;
                var startWaveNum = startWaveNumber.Value;

                if (startLevelNum > _waveSettingsByLevel.Count) {
                    LogIfNew(
                        string.Format(
                            "Illegal Start Level# specified in Level Settings. There are only {0} level(s). Aborting.",
                            _waveSettingsByLevel.Count));
                    return false;
                }

                var waveCount = _waveSettingsByLevel[startLevelNum - 1].Count;
                if (startLevelNum == LevelTimes.Count) {
                    waveCount--; // -1 for the fake final wave on the final level
                }
                if (startWaveNum > waveCount) {
                    LogIfNew(
                        string.Format(
                            "Illegal Start Wave# specified in Level Settings. Level {0} only has {1} wave(s). Aborting.",
                            startLevelNum,
                            waveCount));
                    return false;
                }
            }

            for (var i = 0; i < _waveSettingsByLevel.Count; i++) {
                var wavesForLevel = _waveSettingsByLevel[i];
                for (var w = 0; w < wavesForLevel.Count; w++) {
                    // check "skip wave states".
                    var wave = wavesForLevel[w];
                    if (wave.skipWaveType == SkipWaveMode.IfWorldVariableValueAbove || wave.skipWaveType == SkipWaveMode.IfWorldVariableValueBelow) {
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var skip = 0; skip < wave.skipWavePassCriteria.statMods.Count; skip++) {
                            var skipCrit = wave.skipWavePassCriteria.statMods[skip];

                            if (WorldVariableTracker.IsBlankVariableName(skipCrit._statName)) {
                                LogIfNew(
                                    string.Format(
                                        "Level {0} Wave {1} specifies a Skip Wave criteria with no World Variable selected. Please select one.",
                                        (i + 1),
                                        (w + 1)));
                                _isValid = false;
                            } else if (!WorldVariableTracker.VariableExistsInScene(skipCrit._statName)) {
                                LogIfNew(
                                    string.Format(
                                        "Level {0} Wave {1} specifies a Skip Wave criteria of World Variable '{2}', which doesn't exist in the scene.",
                                        (i + 1),
                                        (w + 1),
                                        skipCrit._statName));
                                _isValid = false;
                            } else {
                                switch (skipCrit._varTypeToUse) {
                                    case WorldVariableTracker.VariableType._integer:
                                        if (skipCrit._modValueIntAmt.variableSource == VariableSource.Variable) {
                                            if (
                                                !WorldVariableTracker.VariableExistsInScene(
                                                    skipCrit._modValueIntAmt.worldVariableName)) {
                                                if (
                                                    IllegalVariableNames.Contains(
                                                        skipCrit._modValueIntAmt.worldVariableName)) {
                                                    LogIfNew(
                                                        string.Format(
                                                            "Level {0} Wave {1} wants to skip wave if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                            (i + 1),
                                                            (w + 1),
                                                            skipCrit._statName));
                                                } else {
                                                    LogIfNew(
                                                        string.Format(
                                                            "Level {0} Wave {1} wants to skip wave if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                            (i + 1),
                                                            (w + 1),
                                                            skipCrit._statName,
                                                            skipCrit._modValueIntAmt.worldVariableName));
                                                }
                                                _isValid = false;
                                            }
                                        }

                                        break;
                                    case WorldVariableTracker.VariableType._float:
                                        if (skipCrit._modValueFloatAmt.variableSource == VariableSource.Variable) {
                                            if (
                                                !WorldVariableTracker.VariableExistsInScene(
                                                    skipCrit._modValueFloatAmt.worldVariableName)) {
                                                if (
                                                    IllegalVariableNames.Contains(
                                                        skipCrit._modValueFloatAmt.worldVariableName)) {
                                                    LogIfNew(
                                                        string.Format(
                                                            "Level {0} Wave {1} wants to skip wave if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                            (i + 1),
                                                            (w + 1),
                                                            skipCrit._statName));
                                                } else {
                                                    LogIfNew(
                                                        string.Format(
                                                            "Level {0} Wave {1} wants to skip wave if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                            (i + 1),
                                                            (w + 1),
                                                            skipCrit._statName,
                                                            skipCrit._modValueFloatAmt.worldVariableName));
                                                }
                                                _isValid = false;
                                            }
                                        }

                                        break;
                                    default:
                                        LogIfNew("Add code for varType: " + skipCrit._varTypeToUse.ToString());
                                        break;
                                }
                            }
                        }
                    }

                    CheckWaveCompletionVars(wave, i, w);
                }
            }

            return true;
        }

        private IEnumerator CoUpdate() {
            while (true) {
                yield return _loopDelay;

                if (!PoolBoss.IsReady) {
                    continue; // wait until Pool Boss has finished Instantiating all the pools.
                }

                // Global waves and respawns should only be launched from the server.
                if (!PoolBoss.IsServer) {
                    continue;
                }

                if (initializationSettingsExpanded && !_initCustomEventsFired) {
                    // fire only once, after joining room
                    _initCustomEventsFired = true;

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < initializationCustomEvents.Count; i++) {
                        var anEvent = initializationCustomEvents[i].CustomEventName;

                        FireCustomEventIfValid(anEvent, _trans);
                    }
                }

                // respawn timers
                if (PrefabsToRespawn.Count > 0) {
                    var respawnedIndexes = new List<int>();

                    for (var i = 0; i < PrefabsToRespawn.Count; i++) {
                        var p = PrefabsToRespawn[i];
                        if (Time.realtimeSinceStartup < p.TimeToRespawn) {
                            continue;
                        }

                        var spawned = PoolBoss.SpawnInPool(p.PrefabToRespawn, p.Position, p.PrefabToRespawn.rotation);
                        if (spawned == null) {
                            continue;
                        }

                        respawnedIndexes.Add(i);

                        if (!p.HasRespawnCustomEvents) {
                            continue;
                        }

                        var spawnedKill = spawned.GetComponent<Killable>();
                        spawnedKill.FireRespawnEvents();
                    }

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < respawnedIndexes.Count; i++) {
                        PrefabsToRespawn.RemoveAt(respawnedIndexes[i]);
                    }
                }

                if ((_gameIsOver && !_skippingWaveForRestart) || _wavesArePaused || !useWaves) { 
                    // yes we need to check if game not over and restart was called.
                    continue;
                }

                //check if level or wave is done.
                if (_hasFirstWaveBeenStarted && !_skipCurrentWave) {
                    var timeToCompare = ActiveWaveInfo.waveDurationFlex.Value;
                    var waveType = ActiveWaveInfo.waveType;

                    switch (waveType) {
                        case WaveType.Timed:
							var tempTime = (int)(timeToCompare - (Time.realtimeSinceStartup - _lastWaveChangeTime) + _wavePausedTime);
                            // ReSharper disable once RedundantCheckBeforeAssignment
                            if (tempTime != TimeRemainingInCurrentWave) {
                                TimeRemainingInCurrentWave = tempTime;
                            }

                            var allDead = ActiveWaveInfo.endEarlyIfAllDestroyed &&
                                          EliminationSpawnersUnkilled.Count == 0;

							if (!allDead && Time.realtimeSinceStartup - _lastWaveChangeTime - _wavePausedTime < timeToCompare) {
                                continue;
                            }

                            EndCurrentWaveNormally();

                            break;
                        case WaveType.Elimination:
                            if (EliminationSpawnersUnkilled.Count > 0) {
                                continue;
                            }

                            if (TriggeredWavesToAwait.Count > 0) {
                                continue;
                            }

                            EndCurrentWaveNormally();
                            break;
                    }
                }

                if (_skipCurrentWave) {
                    if (_skippingWaveForRestart) {
                        IsGameOver = false;
                        _skippingWaveForRestart = false;
                    }

                    if (listener != null) {
                        listener.WaveEndedEarly(PreviousWaveInfo);
                    }
                }

                bool waveSkipped;


                do {
                    var waveInfo = CurrentWaveInfo;

                    if (!disableSyncroSpawners) {
                        // notify all synchro spawners
                        waveSkipped = SpawnOrSkipNewWave(waveInfo);
                        if (waveSkipped) {
                            if (isLoggingOn) {
                                Debug.Log("Wave skipped - wave# is: " + (_currentLevelWave + 1) + " on Level: " +
                                          (_currentLevel + 1));
                            }
                        } else {
                            waveSkipped = false;
                        }
                    } else {
                        waveSkipped = false;
                    }

                    LevelWaveMusicSettings musicSpec = null;

                    // change music maybe
                    if (_currentLevel > 0 && _currentLevelWave == 0) {
                        if (isLoggingOn) {
                            Debug.Log("Level up - new level# is: " + (_currentLevel + 1) +
							          " . Wave 1 starting, occurred at time: " + Time.realtimeSinceStartup);
                        }

                        musicSpec = waveInfo.musicSettings;
                    } else if (_currentLevel > 0 || _currentLevelWave > 0) {
                        if (isLoggingOn) {
                            Debug.Log("Wave up - new wave# is: " + (_currentLevelWave + 1) + " on Level: " +
							          (_currentLevel + 1) + ". Occured at time: " + Time.realtimeSinceStartup);
                        }

                        musicSpec = waveInfo.musicSettings;
                    } else if (_currentLevel == 0 && _currentLevelWave == 0) {
                        musicSpec = waveInfo.musicSettings;
                    }

                    _previousWave = CurrentWaveInfo;
                    _currentLevelWave++;

                    if (_currentLevelWave >= WaveLengths.Count) {
                        _currentLevelWave = 0;
                        var completedLevel = _currentLevel;
                        _currentLevel++;

                        if (listener != null) {
                            listener.LevelEnded(completedLevel + 1);
                        }

                        if (!_gameIsOver && _currentLevel >= _waveSettingsByLevel.Count) {
                            switch (repeatLevelMode) {
                                case LevelLoopMode.RepeatAllLevelsFromBeginning:
                                    if (IsLoggingOn) {
                                        Debug.Log("Levels restarting from beginning!");
                                    }
                                    musicSpec = null;

                                    if (LevelTimes[completedLevel].waveOrder == WaveOrder.RandomOrder) {
                                        var allWaves = _waveSettingsByLevel[completedLevel];
                                        // ReSharper disable once ForCanBeConvertedToForeach
                                        for (var i = 0; i < allWaves.Count; i++) {
                                            var aWave = allWaves[i];
                                            aWave.randomWaveNumber = Random.Range(0, allWaves.Count);
                                        }
                                        allWaves.Sort(delegate (LevelWave x, LevelWave y) {
                                            return x.randomWaveNumber.CompareTo(y.randomWaveNumber);
                                        });
                                    }
                                    GotoWave(1, 1);
                                    break;
                                case LevelLoopMode.Win:
                                    musicSpec = gameOverMusicSettings;
                                    Win();
                                    IsGameOver = true;
                                    break;
                            }
                        }
                    }

                    PlayMusicIfSet(musicSpec);
                } while (waveSkipped);

                _lastWaveChangeTime = Time.realtimeSinceStartup;
                _hasFirstWaveBeenStarted = true;
                _skipCurrentWave = false;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void CheckWaveCompletionVars(LevelWave wave, int i, int w) {
            // check "wave completion bonuses".
            if (!wave.waveBeatBonusesEnabled) {
                return;
            }

            if (wave.waveBeatBonusesEnabled) {
                if (wave.waveType == WaveType.Elimination) {
                    wave.bonusPrefabSpawnPercent.LogIfInvalid(this.transform, "Bonus Prefab Spawn Percent");
                    wave.bonusPrefabQty.LogIfInvalid(this.transform, "Bonus Prefab Spawn Qty");
                }
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var b = 0; b < wave.waveDefeatVariableModifiers.statMods.Count; b++) {
                var beatMod = wave.waveDefeatVariableModifiers.statMods[b];

                if (WorldVariableTracker.IsBlankVariableName(beatMod._statName)) {
                    LogIfNew(
                        string.Format(
                            "Level {0} Wave {1} specifies a Wave Completion Bonus with no World Variable selected. Please select one.",
                            (i + 1),
                            (w + 1)));
                    _isValid = false;
                } else if (!WorldVariableTracker.VariableExistsInScene(beatMod._statName)) {
                    LogIfNew(
                        string.Format(
                            "Level {0} Wave {1} specifies a Wave Completion Bonus of World Variable '{2}', which doesn't exist in the scene.",
                            (i + 1),
                            (w + 1),
                            beatMod._statName));
                    _isValid = false;
                } else {
                    switch (beatMod._varTypeToUse) {
                        case WorldVariableTracker.VariableType._integer:
                            if (beatMod._modValueIntAmt.variableSource == VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(
                                        beatMod._modValueIntAmt.worldVariableName)) {
                                    if (
                                        IllegalVariableNames.Contains(
                                            beatMod._modValueIntAmt.worldVariableName)) {
                                        LogIfNew(
                                            string.Format(
                                                "Level {0} Wave {1} wants to award Wave Completion Bonus if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                (i + 1),
                                                (w + 1),
                                                beatMod._statName));
                                    } else {
                                        LogIfNew(
                                            string.Format(
                                                "Level {0} Wave {1} wants to award Wave Completion Bonus if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                (i + 1),
                                                (w + 1),
                                                beatMod._statName,
                                                beatMod._modValueIntAmt.worldVariableName));
                                    }
                                    _isValid = false;
                                }
                            }

                            break;
                        case WorldVariableTracker.VariableType._float:
                            if (beatMod._modValueFloatAmt.variableSource == VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(
                                        beatMod._modValueFloatAmt.worldVariableName)) {
                                    if (
                                        IllegalVariableNames.Contains(
                                            beatMod._modValueFloatAmt.worldVariableName)) {
                                        LogIfNew(
                                            string.Format(
                                                "Level {0} Wave {1} wants to award Wave Completion Bonus if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                (i + 1),
                                                (w + 1),
                                                beatMod._statName));
                                    } else {
                                        LogIfNew(
                                            string.Format(
                                                "Level {0} Wave {1} wants to award Wave Completion Bonus if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                (i + 1),
                                                (w + 1),
                                                beatMod._statName,
                                                beatMod._modValueFloatAmt.worldVariableName));
                                    }
                                    _isValid = false;
                                }
                            }

                            break;
                        default:
                            LogIfNew("Add code for varType: " + beatMod._varTypeToUse.ToString());
                            break;
                    }
                }
            }
        }

        private void EndCurrentWaveNormally() {
            // check for wave end bonuses
            if (ActiveWaveInfo.waveBeatBonusesEnabled && ActiveWaveInfo.waveDefeatVariableModifiers.statMods.Count > 0) {
                if (listener != null) {
                    listener.WaveCompleteBonusesStart(ActiveWaveInfo.waveDefeatVariableModifiers.statMods);
                }

                // ReSharper disable once TooWideLocalVariableScope
                WorldVariableModifier mod;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < ActiveWaveInfo.waveDefeatVariableModifiers.statMods.Count; i++) {
                    mod = ActiveWaveInfo.waveDefeatVariableModifiers.statMods[i];
                    WorldVariableTracker.ModifyPlayerStat(mod, _trans);
                }
            }

            if (ActiveWaveInfo.useCompletionEvents && ActiveWaveInfo.completionCustomEvents.Count > 0) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < ActiveWaveInfo.completionCustomEvents.Count; i++) {
                    var anEvent = ActiveWaveInfo.completionCustomEvents[i].CustomEventName;

                    FireCustomEventIfValid(anEvent, _trans);
                }
            }

            if (listener != null) {
                listener.WaveEnded(PreviousWaveInfo);
            }

            if (ActiveWaveInfo.pauseGlobalWavesWhenCompleted) {
                PauseWave();
            }
        }

        private static bool SkipWaveOrNot(LevelWave waveInfo, bool valueAbove) {
            var skipThisWave = true;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < waveInfo.skipWavePassCriteria.statMods.Count; i++) {
                var stat = waveInfo.skipWavePassCriteria.statMods[i];

                var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                if (variable == null) {
                    skipThisWave = false;
                    break;
                }
                var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                    ? variable.CurrentIntValue
                    : variable.CurrentFloatValue;
                var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                    ? stat._modValueIntAmt.Value
                    : stat._modValueFloatAmt.Value;

                if (valueAbove) {
                    if (!(varVal < compareVal)) {
                        continue;
                    }
                    skipThisWave = false;
                    break;
                } else {
                    if (!(varVal > compareVal)) {
                        continue;
                    }
                    skipThisWave = false;
                    break;
                }
            }

            return skipThisWave;
        }

        private void SpawnNewWave(LevelWave waveInfo, bool isRestartWave) {
			EliminationSpawnersUnkilled.Clear();
            SpawnedItemsRemaining.Clear();
            WaveRemainingItemsChanged();

            _wavePausedTime = 0;
            TriggeredWavesToAwait.Clear();

            var spawnersToActivate = _syncroSpawners.Count;

            if (waveInfo.spawnerUseMode == WaveSpawnerUseMode.RandomSubset) {
                for (var i = 0; i < _syncroSpawners.Count; i++) {
                    _syncroSpawners[i].randomSortKey = Random.Range(int.MinValue, int.MaxValue);
                }

                spawnersToActivate = Random.Range(waveInfo.spawnersToUseMin, waveInfo.spawnersToUseMax);
            } else {
                for (var i = 0; i < _syncroSpawners.Count; i++) {
                    _syncroSpawners[i].randomSortKey = i;
                }
            }

            _syncroSpawners.Sort(delegate (WaveSyncroPrefabSpawner x, WaveSyncroPrefabSpawner y) {
                return x.randomSortKey.CompareTo(y.randomSortKey);
            });

            var spawnersActivated = 0;

            for (var i = 0; i < _syncroSpawners.Count && spawnersActivated < spawnersToActivate; i++) {
                var syncro = _syncroSpawners[i];

                if (!syncro.WaveChange(isRestartWave)) {
                    // returns true if wave found.
                    continue;
                }

                // wave valid for spawner
                spawnersActivated++;

                switch (waveInfo.waveType) {
                    case WaveType.Elimination:
                        EliminationSpawnersUnkilled.Add(syncro.GetInstanceID(), syncro);
                        AddTriggeredSpawnerAwaitersIfAny(waveInfo);
                        break;
                    case WaveType.Timed:
                        EliminationSpawnersUnkilled.Add(syncro.GetInstanceID(), syncro);
                        TimeRemainingInCurrentWave = CurrentWaveInfo.waveDurationFlex.Value;
                        break;
                }
            }

            if (listener != null) {
                var waveOrder = LevelTimes[CurrentLevel].waveOrder;

                switch (waveOrder) {
                    case WaveOrder.SpecifiedOrder:
                        if (CurrentWaveInfo.sequencedWaveNumber == 0) {
                            listener.LevelStarted(CurrentLevel);
                        }
                        break;
                    case WaveOrder.RandomOrder:
                        if (CurrentWaveInfo.randomWaveNumber == 0) {
                            listener.LevelStarted(CurrentLevel);
                        }
                        break;
                }

                listener.WaveStarted(CurrentWaveInfo);
            }
        }

        private void AddTriggeredSpawnerAwaitersIfAny(LevelWave wave) {
            if (!wave.useTriggeredSpawners) {
                return;
            }

            if (wave.trigSpawnerWavesToAwait.Count == 0) {
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < wave.trigSpawnerWavesToAwait.Count; i++) {
                var aTrig = wave.trigSpawnerWavesToAwait[i];
                if (aTrig.TrigSpawner == null) {
                    continue; // spawner despawned
                }

                // valid wave.
                TriggeredWavesToAwait.Add(aTrig);
            }
        }

        // Return true to skip wave, false means we started spawning the wave.
        private bool SpawnOrSkipNewWave(LevelWave waveInfo) {
            var skipThisWave = true;

            if (enableWaveWarp) {
                // check for Custom Start Wave and skip all before it
                if (CurrentLevel < startLevelNumber.Value - 1) {
                    return true; // skip
                }
                if (CurrentLevel == startLevelNumber.Value - 1 && CurrentLevelWave < startWaveNumber.Value - 1) {
                    return true; // skip
                }
                enableWaveWarp = false; // should only happen once after you pass the warped wave.
            }

            if (waveInfo.skipWavePassCriteria.statMods.Count == 0 || waveInfo.skipWaveType == SkipWaveMode.None) {
                skipThisWave = false;
            }

            if (skipThisWave) {
                switch (waveInfo.skipWaveType) {
                    case SkipWaveMode.Always:
                        break;
                    case SkipWaveMode.IfWorldVariableValueAbove:
                        if (!SkipWaveOrNot(waveInfo, true)) {
                            skipThisWave = false;
                        }
                        break;
                    case SkipWaveMode.IfWorldVariableValueBelow:
                        if (!SkipWaveOrNot(waveInfo, false)) {
                            skipThisWave = false;
                        }
                        break;
                }
            }

            if (skipThisWave) {
                if (listener != null) {
                    listener.WaveSkipped(waveInfo);
                }
                return true;
            }

			_displayCurrentWave = _currentLevelWave;
			_displayCurrentLevel = _currentLevel;

			SpawnNewWave(waveInfo, false);
            return false;
        }

        private static void Win() {
            if (IsLoggingOn) {
                Debug.Log("Player has WON!");
            }

            HasPlayerWon = true;
        }

        #endregion

        #region Public Static Methods

        /*! \cond PRIVATE */
        public static LevelSettings Instance {
            get {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_lsInstance == null) {
                    _lsInstance = (LevelSettings)FindObjectOfType(typeof(LevelSettings));
                }

                return _lsInstance;
            }
            // ReSharper disable once ValueParameterNotUsed
            set { _lsInstance = null; }
        }

        public static void AddWaveSpawnedItem(Transform spawnedTrans) {
            if (SpawnedItemsRemaining.Contains(spawnedTrans)) {
                return;
            }

            SpawnedItemsRemaining.Add(spawnedTrans);
            WaveRemainingItemsChanged();
        }

        public static void EliminationSpawnerCompleted(int instanceId, Transform lastPrefabKilled) {
            var spawnersBefore = EliminationSpawnersUnkilled.Count;
            EliminationSpawnersUnkilled.Remove(instanceId);

            if (spawnersBefore != EliminationSpawnersUnkilled.Count && EliminationSpawnersUnkilled.Count == 0) {
                SpawnBonusPrefabIfAny(lastPrefabKilled.position);
            }
        }

        public static void TriggeredSpawnerWaveEliminated(TriggeredSpawner.EventType eType, TriggeredSpawnerV2 spawner, TriggeredWaveSpecifics wave) {
            if (TriggeredWavesToAwait.Count == 0) {
                return;
            }

            var waveMatch = TriggeredWavesToAwait.Find(delegate(TrigSpawnerWaveWaiter waiter) {
                return waiter.TrigSpawner == spawner && waiter.EventType == eType && (eType != TriggeredSpawner.EventType.CustomEvent || waiter.CustomEventName == wave.customEventName);
            });

            if (waveMatch == null) {
                return;
            }

            TriggeredWavesToAwait.Remove(waveMatch);
        }
        /*! \endcond */

        /// <summary>
        /// Call this method to continue the game when the game has ended. Continues from the same global wave the player died on. If the player already won, you should not call this.
        /// </summary>
        public static void ContinueGame() {
            if (HasPlayerWon) {
                LogIfNew("The player has already won, so there are no more waves to continue from. You may wish to call RestartGame instead. Aborting.");
                return;
            }

            WorldVariableTracker.ForceReInit();
            IsGameOver = false;
        }

        /// <summary>
        /// Call this method to restart the game when the game has ended. Goes back to level #1, wave #1 for Syncro Spawners.
        /// </summary>
        public static void RestartGame() {
            if (!Instance.useWaves) {
                return;
            }

            WorldVariableTracker.ForceReInit();

            _skippingWaveForRestart = true;

            foreach (var spawner in Instance._syncroSpawners) {
                spawner.ResetWaveIncrementCounter();
            }

            GotoWave(1, 1);
        }

        /// <summary>
        /// Call this method to immediately finish the current wave for Syncro Spawners.
        /// </summary>
        public static void EndWave() {
            _skipCurrentWave = true;
        }

        /// <summary>
        /// Call this method to immediately finish the current wave for Syncro Spawners and go to a different level / wave you specify.
        /// </summary>
        /// <param name="levelNum">The level number to skip to.</param>
        /// <param name="waveNum">The wave number to skip to.</param>
		public static void GotoWave(int levelNum, int waveNum) {
            if (levelNum < 1 || waveNum < 1) {
                LogIfNew("GotoWave cannot take a levelNum or waveNum less than 1. Aborting.");
                return;
            }

            if (IsGameOver && !_skippingWaveForRestart) {
                LogIfNew("The game is over, so you cannot GotoWave. You can want to call RestartGame() instead. Aborting", true);
                return;
            }

            _skipCurrentWave = true;
            _currentLevel = levelNum - 1;
            _currentLevelWave = waveNum - 1;

			_displayCurrentLevel = _currentLevel;
			_displayCurrentWave = _currentLevelWave;
        }

        /*! \cond PRIVATE */
        public static WavePrefabPool GetFirstMatchingPrefabPool(string poolName) {
            if (string.IsNullOrEmpty(poolName)) {
                return null;
            }

            var poolsHolder = GetPoolsHolder;

            if (poolsHolder == null) {
                return null;
            }

            var oChild = poolsHolder.GetChildTransform(poolName);

            if (oChild == null) {
                return null;
            }

            return oChild.GetComponent<WavePrefabPool>();
        }
        /*! \endcond */

        /// <summary>
        /// This returns a list of all Prefab Pools
        /// </summary>
        /// <returns>A list of all Prefab Pool names.</returns>
        public static List<string> GetSortedPrefabPoolNames() {
            var poolsHolder = GetPoolsHolder;

            if (poolsHolder == null) {
                return null;
            }

            var pools = new List<string>();

            for (var i = 0; i < poolsHolder.childCount; i++) {
                var oChild = poolsHolder.GetChild(i);
                pools.Add(oChild.name);
            }

            pools.Sort();

            pools.Insert(0, "-None-");

            return pools;
        }

        /*! \cond PRIVATE */
        public static void LogIfNew(string message, bool logAsWarning = false) {
            if (RecentErrorsByTime.ContainsKey(message)) {
                var item = RecentErrorsByTime[message];
				if (Time.realtimeSinceStartup - 1f > item) {
                    // it's been over 1 second. Log again
                    RecentErrorsByTime.Remove(message);
                } else {
                    return;
                }
            }

			RecentErrorsByTime.Add(message, Time.realtimeSinceStartup);

            if (logAsWarning) {
                Debug.LogWarning(message);
            } else {
                Debug.LogError(message);
            }
        }
        /*! \endcond */

        /// <summary>
        /// Use this method to pause the current wave for Syncro Spawners.
        /// </summary>
        public static void PauseWave() {
            if (_wavesArePaused) {
				return; // can't pause if already paused.
			}

			_wavesArePaused = true;
			_lastWavePauseTime = Time.realtimeSinceStartup;

            if (IsLoggingOn) {
                Debug.LogWarning("Waves paused!");
            }
        }

        /// <summary>
        /// Use this method to restart the current wave for Syncro Spawners. This puts the repeat wave counter back to zero as well. Also, this unpauses the wave if paused.
        /// </summary>
        public static void RestartCurrentWave() {
            if (!PoolBoss.IsServer) {
                return;
            }

            if (IsGameOver) {
                LogIfNew("Cannot restart current wave because game is over for Core GameKit.");
                return; // no wave
            }

            // destroy spawns from current wave if any
            var restartMode = Instance.waveRestartMode;
            if (restartMode != WaveRestartBehavior.LeaveSpawned) {
                var i = SpawnedItemsRemaining.Count + 1;

                while (SpawnedItemsRemaining.Count > 0) {
                    var item = SpawnedItemsRemaining[0];
                    Killable maybeKillable = null;
                    if (Instance.waveRestartMode == WaveRestartBehavior.DestroySpawned) {
                        maybeKillable = item.GetComponent<Killable>();
                    }

                    if (maybeKillable != null) {
                        maybeKillable.DestroyKillable();
                    } else {
                        PoolBoss.Despawn(SpawnedItemsRemaining[0]);
                    }

                    i--;
                    if (i < 0) {
                        break; // just in case. Don't want endless loops.
                    }
                }
            }

            UnpauseWave();
            Instance.SpawnNewWave(ActiveWaveInfo, true);

            if (Instance.listener != null) {
                Instance.listener.WaveRestarted(ActiveWaveInfo);
            }
        }

        /*! \cond PRIVATE */
        public static void RemoveWaveSpawnedItem(Transform spawnedTrans) {
            if (!SpawnedItemsRemaining.Contains(spawnedTrans)) {
                return;
            }

            SpawnedItemsRemaining.Remove(spawnedTrans);
            WaveRemainingItemsChanged();
        }

        public static void TrackTimedRespawn(float delay, Transform prefabTrans, Vector3 pos, bool hasRespawnCustomEvents) {
            PrefabsToRespawn.Add(new RespawnTimer(delay, prefabTrans, pos, hasRespawnCustomEvents));
        }
        /*! \endcond */

        /// <summary>
        /// Use this method to unpause the current wave for Syncro Spawners.
        /// </summary>
        public static void UnpauseWave() {
            if (!_wavesArePaused) {
				return; // can't unpause if not paused.
			}

			_wavesArePaused = false;

            _wavePausedTime = Time.realtimeSinceStartup - _lastWavePauseTime;

            if (IsLoggingOn) {
				Debug.LogWarning("Waves unpaused!");
			}
        }

        /// <summary>
        /// This method lets you start at a custom level and wave number. You must call this no later than Start for it to work properly.
        /// </summary>
        /// <param name="levelNumber">The level number to start on.</param>
        /// <param name="waveNumber">The wave number to start on.</param>
        public static void WarpToLevel(int levelNumber, int waveNumber) {
            Instance.enableWaveWarp = true;
            Instance.startLevelNumber.Value = (levelNumber - 1);
            Instance.startWaveNumber.Value = waveNumber - 1;
        }

        #endregion

        #region Private Static Methods

        private static void PlayMusicIfSet(LevelWaveMusicSettings musicSpec) {
            if (Instance.useMusicSettings && Instance.useWaves && musicSpec != null) {
                WaveMusicChanger.WaveUp(musicSpec);
            }
        }

        private static void WaveRemainingItemsChanged() {
            if (Listener != null) {
                Listener.WaveItemsRemainingChanged(WaveRemainingItemCount);
            }
        }

        private static void SpawnBonusPrefabIfAny(Vector3 spawnPosition) {
            var waveBeat = PreviousWaveInfo;
            if (!waveBeat.useSpawnBonusPrefab) {
                return;
            }

            if (Random.Range(0, 100) >= waveBeat.bonusPrefabSpawnPercent.Value) {
                return;
            }

            switch (waveBeat.bonusPrefabSource) {
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    var bonusPrefabPool = GetFirstMatchingPrefabPool(waveBeat.bonusPrefabPoolName);
                    if (bonusPrefabPool == null) {
                        LogIfNew("Could not find Prefab Pool '" + waveBeat.bonusPrefabPoolName + "' for Bonus Prefab on Level: " + CurrentDisplayLevel + ", Wave: " + CurrentDisplayWave);
                        return;
                    }

                    var numToSpawn = waveBeat.bonusPrefabQty.Value;
                    for (var i = 0; i < numToSpawn; i++) {
                        var prefabToSpawn = bonusPrefabPool.GetRandomWeightedTransform();
                        if (prefabToSpawn == null) {
                            LogIfNew("Prefab Pool '" + bonusPrefabPool.name + "' has no items left to spawn for Bonus prefab.");
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
                        LogIfNew("Bonus Prefab for Level: " + CurrentDisplayLevel + ", Wave: " + CurrentDisplayWave + " is unassigned.");
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

        #endregion


        #region Public Properties

        /*! \cond PRIVATE */
        public static bool AppIsShuttingDown { get; set; }
        /*! \endcond */

        /// <summary>
        /// This property returns the current wave info for Syncro Spawners
        /// </summary>
        public static LevelWave ActiveWaveInfo {
            // This is the only one you would read from code. "CurrentWaveInfo" is to be used by spawners only.
            get {
                LevelWave wave;
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_previousWave != null) {
                    wave = _previousWave;
                } else {
                    wave = CurrentWaveInfo;
                }

                return wave;
            }
        }

		/// <summary>
		/// Gets the current level number for display purposes.
		/// </summary>
		/// <value>The current display level.</value>
		public static int CurrentDisplayLevel {
			get {
				return _displayCurrentLevel + 1;
			}
		}

		/// <summary>
		/// Gets the current wave number for display purposes.
		/// </summary>
		/// <value>The current display wave.</value>
		public static int CurrentDisplayWave {
			get {
				return _displayCurrentWave + 1;
			}
		}

        /// <summary>
        /// This property returns the current level number (zero-based) for Syncro Spawners.
        /// </summary>
        public static int CurrentLevel {
            get { return _currentLevel; }
        }

        /// <summary>
        /// This property returns the current wave number (zero-based) in the current level for Syncro Spawners.
        /// </summary>
        public static int CurrentLevelWave {
            get { return _currentLevelWave; }
        }

        /// <summary>
        /// This property returns the current level number (zero-based) for Syncro Spawners.
        /// </summary>
        public int LevelNumber {
            get { return CurrentLevel; }
        }

        /// <summary>
        /// This property returns the current wave number (zero-based) in the current level for Syncro Spawners.
        /// </summary>
        public int WaveNumber {
            get { return CurrentLevelWave; }
        }

        /*! \cond PRIVATE */
        public static LevelWave CurrentWaveInfo {
            get {
                if (WaveLengths.Count == 0) {
                    LogIfNew("Not possible to restart wave. There are no waves set up in LevelSettings.");
                    return null;
                }

                var waveInfo = WaveLengths[_currentLevelWave];
                return waveInfo;
            }
        }
        /*! \endcond */

        /// <summary>
        /// A list of the Transforms for all Prefab Pools
        /// </summary>
        public static List<Transform> GetAllPrefabPools {
            get {
                var holder = GetPoolsHolder;

                if (holder == null) {
                    LogIfNew(NoPrefabPoolsContainerAlert);
                    return null;
                }

                var pools = new List<Transform>();
                for (var i = 0; i < holder.childCount; i++) {
                    pools.Add(holder.GetChild(i));
                }

                return pools;
            }
        }

        /// <summary>
        /// A list of the Transforms for all Syncro Spawners
        /// </summary>
        public static List<Transform> GetAllSpawners {
            get {
                var spawnContainer = Instance.transform.GetChildTransform(SpawnerContainerTransName);

                if (spawnContainer == null) {
                    LogIfNew(NoSpawnContainerAlert);
                    return null;
                }

                var spawners = new List<Transform>();
                for (var i = 0; i < spawnContainer.childCount; i++) {
                    spawners.Add(spawnContainer.GetChild(i));
                }

                return spawners;
            }
        }

        public static List<WaveSyncroPrefabSpawner> GetAllSpawnerScripts {
            get {
                var spawnContainer = Instance.transform.GetChildTransform(SpawnerContainerTransName);

                if (spawnContainer == null) {
                    LogIfNew(NoSpawnContainerAlert);
                    return null;
                }

                var spawners = new List<WaveSyncroPrefabSpawner>();
                for (var i = 0; i < spawnContainer.childCount; i++) {
                    spawners.Add(spawnContainer.GetChild(i).GetComponent<WaveSyncroPrefabSpawner>());
                }

                return spawners;
            }
        }

        /*! \cond PRIVATE */
        public static List<Transform> GetAllWorldVariables {
            get {
                var holder = GetWorldVariablesHolder;

                if (holder == null) {
                    LogIfNew(NoWorldVariablesContainerAlert);
                    return null;
                }

                var vars = new List<Transform>();
                for (var i = 0; i < holder.childCount; i++) {
                    vars.Add(holder.GetChild(i));
                }

                return vars;
            }
        }
        /*! \endcond */

        /// <summary>
        /// Use this property to read or set "IsGameOver". If game is over, game over behavior will come into play, Syncro Spawners will stop spawning and waves will not advance.
        /// </summary>
        public static bool IsGameOver {
            get { return _gameIsOver; }
            set {
                var wasGameOver = _gameIsOver;
                _gameIsOver = value;

                if (!_gameIsOver) {
                    return;
                }
                if (!wasGameOver) {
                    if (Listener != null) {
                        Listener.GameOver(HasPlayerWon);

                        if (!HasPlayerWon) {
                            if (IsLoggingOn) {
                                Debug.Log("Player has LOST!");
                            }
                            Listener.Lose();
                        }
                    }
                }

                var musicSpec = Instance.gameOverMusicSettings;

                PlayMusicIfSet(musicSpec);
            }
        }

        /// <summary>
        /// This property returns whether there is another wave after the current one.
        /// </summary>
        public static bool HasNextWave {
            get {
                if (_currentLevel < _waveSettingsByLevel.Count - 1) {
                    return true;
                }

                return _currentLevelWave < WaveLengths.Count - 1;
            }
        }

        /*! \cond PRIVATE */
        public static bool HasPlayerWon {
            get { return _hasPlayerWon; }
            set {
                _hasPlayerWon = value;

                if (value && Listener != null) {
                    Listener.Win();
                }
            }
        }

        /// <summary>
        /// This property returns whether or not logging is turned on in Level Settings.
        /// </summary>
        public static bool IsLoggingOn {
            get { return Instance != null && Instance.isLoggingOn; }
        }
        /*! \endcond */

        /// <summary>
        /// This property returns the number of the last level you have set up (zero-based).
        /// </summary>
        public static int LastLevel {
            get { return _waveSettingsByLevel.Count; }
        }

        /*! \cond PRIVATE */
        public static LevelSettingsListener Listener {
            get {
                if (AppIsShuttingDown) {
                    return null;
                }

                if (Instance != null) {
                    return Instance.listener;
                } else {
                    return null;
                }
            }
        }

        public static LevelWave PreviousWaveInfo {
            get { return _previousWave; }
        }
        /*! \endcond */

        /// <summary>
        /// This property returns a list of all Syncro Spawners in the Scene.
        /// </summary>
        public static List<WaveSyncroPrefabSpawner> SyncroSpawners {
            get { return Instance._syncroSpawners; }
        }

        /// <summary>
        /// This property returns a random Syncro Spawner in the Scene.
        /// </summary>
        public static WaveSyncroPrefabSpawner RandomSyncroSpawner {
            get {
                var spawners = Instance._syncroSpawners;
                if (spawners.Count == 0) {
                    return null;
                }

                var randIndex = Random.Range(0, spawners.Count);
                return spawners[randIndex];
            }
        }

        /// <summary>
        /// This property returns the number of seconds remaining in the current wave for Syncro Spawners. -1 is returned for elimination waves.
        /// </summary>
        public static int TimeRemainingInCurrentWave {
            get {
                var wave = ActiveWaveInfo;

                switch (wave.waveType) {
                    case WaveType.Elimination:
                        return -1;
                    case WaveType.Timed:
                        return _waveTimeRemaining;
                }

                return -1;
            }
            set {
                _waveTimeRemaining = value;

                if (ActiveWaveInfo.waveType == WaveType.Timed && Listener != null) {
                    Listener.WaveTimeRemainingChanged(_waveTimeRemaining);
                }
            }
        }

        /// <summary>
        /// This property returns a list of all wave settings in the current Level.
        /// </summary>
        public static List<LevelWave> WaveLengths {
            get {
                if (!_waveSettingsByLevel.ContainsKey(_currentLevel)) {
                    return new List<LevelWave>();
                }
                return _waveSettingsByLevel[_currentLevel];
            }
        }

        /// <summary>
        /// This property will return whether the current wave is paused for Syncro Spawners.
        /// </summary>
        public static bool WavesArePaused {
            get { return _wavesArePaused; }
        }

        /// <summary>
        /// This property will return the number of Spawners that haven't had their waves eliminated yet.
        /// </summary>
        public static int EliminationSpawnersRemaining {
            get { return EliminationSpawnersUnkilled.Count; }
        }

        #endregion

        #region Private properties

        /*! \cond PRIVATE */
        private static Transform GetPoolsHolder {
            get {
                var lev = Instance;
                if (lev == null) {
                    return null;
                }

                return lev.transform.GetChildTransform(PrefabPoolsContainerTransName);
            }
        }

        private static Transform GetWorldVariablesHolder {
            get {
                var lev = Instance;
                if (lev == null) {
                    return null;
                }

                return lev.transform.GetChildTransform(WorldVariablesContainerTransName);
            }
        }
        /*! \endcond */

        /// <summary>
        /// The number of items left to eliminate in a wave.
        /// </summary>
        public static int WaveRemainingItemCount {
            get { return SpawnedItemsRemaining.Count; }
        }

        #endregion

        #region Custom Events

        /// <summary>
        /// This method is used to keep track of enabled CustomEventReceivers automatically. This is called when then CustomEventReceiver prefab is enabled. Only call this if you write classes that inherit from ICgkEventReceiver.
        /// </summary>
        /// <param name="receiver">The receiver object to register.</param>
        /// <param name="receiverTrans">The Transform of the receiver.</param>
        public static void AddCustomEventReceiver(ICgkEventReceiver receiver, Transform receiverTrans) {
            if (AppIsShuttingDown) {
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Instance.customEvents.Count; i++) {
                var anEvent = Instance.customEvents[i];
                if (!receiver.SubscribesToEvent(anEvent.EventName)) {
                    continue;
                }

                if (!ReceiversByEventName.ContainsKey(anEvent.EventName)) {
                    ReceiversByEventName.Add(anEvent.EventName, new Dictionary<ICgkEventReceiver, Transform>
                    {
                        {receiver, receiverTrans}
                    });
                } else {
                    var dict = ReceiversByEventName[anEvent.EventName];
                    if (dict.ContainsKey(receiver)) {
                        continue;
                    }

                    dict.Add(receiver, receiverTrans);
                }
            }
        }

        /// <summary>
        /// This method is used to keep track of enabled CustomEventReceivers automatically. This is called when then CustomEventReceiver prefab is disabled.
        /// </summary>
        /// <param name="receiver">The receiver to remove from the tracking list.</param>
        public static void RemoveCustomEventReceiver(ICgkEventReceiver receiver) {
            if (AppIsShuttingDown || Instance == null) {
                if (Instance == null) {
                    // remove it from all events if it's trying to die from Scene reload.
                    foreach (var key in ReceiversByEventName.Keys) {
                        ReceiversByEventName[key].Remove(receiver);
                    }
                }
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Instance.customEvents.Count; i++) {
                var anEvent = Instance.customEvents[i];
                if (!receiver.SubscribesToEvent(anEvent.EventName)) {
                    continue;
                }

                var dict = ReceiversByEventName[anEvent.EventName];
                dict.Remove(receiver);
            }
        }

        /// <summary>
        /// Returns a list of all Receivers for a Custom Event
        /// </summary>
        /// <param name="customEventName">The name of the Custom Event</param>
        /// <returns>A list of Transforms for all receivers.</returns>
        public static List<Transform> ReceiversForEvent(string customEventName) {
            var receivers = new List<Transform>();

            if (!ReceiversByEventName.ContainsKey(customEventName)) {
                return receivers;
            }

            var dict = ReceiversByEventName[customEventName];

            foreach (var receiver in dict.Keys) {
                if (receiver.SubscribesToEvent(customEventName)) {
                    receivers.Add(dict[receiver]);
                }
            }

            return receivers;
        }

		/// <summary>
		/// Fire a custom event if the name is valid (not "NONE", "TYPE IN", etc);
		/// </summary>
		/// <param name="anEvent">The name of the Custom Event</param>
		/// <param name="eventOrigin">The origin position of the Custom Event</param>
		public static void FireCustomEventIfValid(string anEvent, Transform eventOrigin) {
			if (anEvent == DynamicEventName || anEvent == NoEventName || anEvent == EmptyValue) {
				return;
			}
			
			FireCustomEvent(anEvent, eventOrigin);
		}

        /// <summary>
        /// Fires a CGK custom event but with a parameter
        /// </summary>
        /// <param name="customEventName">The name of the custom event.</param>
        /// <param name="originObject">The Transform origin of the event.</param>
        /// <param name="eventParam">The parameter for the event</param>
        /// <param name="logDupe">Whether or not to log an error with duplicate event firing.</param>
        public static void FireCustomEventWithParam(string customEventName, Transform originObject, object eventParam, bool logDupe = true) {
            // generate the hash for param storage
            int paramKey = GetCustomEventHash(customEventName, originObject.position);

            // make sure we dont have an active custom event from the same name and position
            if (!CustomEventParams.ContainsKey(paramKey)) {
                // store the param and sender in the param dictionaries
                CustomEventParams.Add(paramKey, eventParam);
                CustomEventParamSenders.Add(paramKey, originObject);

                // fire the event using normal FireCustomEvent
                LevelSettings.FireCustomEvent(customEventName, originObject, logDupe);

                // remove the param from the dictionaries
                CustomEventParams.Remove(paramKey);
                CustomEventParamSenders.Remove(paramKey);
            } else {
                if (logDupe) {
                    Debug.LogWarning("Already fired Custom Event '" + customEventName + "' this frame. Cannot be fired twice in the same frame.");
                }
            }
        }

        /// <summary>
        /// Calling this method will fire a Custom Event at the originPoint position. All CustomEventReceivers with the named event specified will do whatever action is assigned to them. If there is a distance criteria applied to receivers, it will be applied.
        /// </summary>
        /// <param name="customEventName">The name of the custom event.</param>
        /// <param name="originObject">The Transform origin of the event.</param>
        /// <param name="logDupe">Whether or not to log an error with duplicate event firing.</param>
        public static void FireCustomEvent(string customEventName, Transform originObject, bool logDupe = true) {
            if (AppIsShuttingDown) {
                return;
            }

            if (customEventName == DynamicEventName || customEventName == NoEventName || customEventName == EmptyValue) {
                return;
            }

            if (originObject == null) {
                Debug.LogError("Custom Event '" + customEventName + "' cannot be fired without an originObject passed in.");
                return;
            }

            if (!CustomEventExists(customEventName)) {
                Debug.LogError("Custom Event '" + customEventName + "' was not found in Core GameKit.");
                return;
            }

            var customEvent = GetCustomEventByName(customEventName);

            if (customEvent == null) {
                // for warming
                return;
            }

            var eventHash = 0;

            var allowFiringSameFrameFromDiffObjects = false;
            // check for "fired too recently" conditions
			switch (customEvent.eventRcvMode) {
				case EventReceiveMode.Never:
				case EventReceiveMode.Always:
				case EventReceiveMode.WhenDistanceLessThan:
				case EventReceiveMode.WhenDistanceMoreThan:
					if (customEvent.frameLastFired >= Time.frameCount) {
						if (logDupe) {
							Debug.LogWarning("Already fired Custom Event '" + customEventName + "' this frame. Cannot be fired twice in the same frame.");
						}
						return;
					}
					break;
				case EventReceiveMode.OnChildGameObject:
				case EventReceiveMode.OnParentGameObject:
				case EventReceiveMode.OnSameGameObject:
				case EventReceiveMode.OnSameOrChildGameObject:
				case EventReceiveMode.OnSameOrParentGameObject:
			        allowFiringSameFrameFromDiffObjects = true;
                    eventHash = GetCustomEventHash(customEventName, originObject.position);
                    // check if it fired this frame on this object already

                    if (customEvent.customEventsDuringFrame != null) {
			            if (customEvent.customEventsDuringFrame.FrameNumber < Time.frameCount) {
			                customEvent.customEventsDuringFrame = null; // get rid of old frame data if any
			            }
                        if (customEvent.customEventsDuringFrame != null && customEvent.customEventsDuringFrame.CustomEventHashes.Contains(eventHash)) {
                            if (logDupe) {
                                Debug.LogWarning("Already fired Custom Event '" + customEventName + "' this frame from the same game object '" + originObject.name + "'. Cannot be fired twice in the same frame from the same object.");
                            }
                            return;
                        }
                    }

					break;
			}

            // not too soon to fire Custom Event again, so record "last fired time"
            if (allowFiringSameFrameFromDiffObjects) {
                //record in hashset
                if (customEvent.customEventsDuringFrame == null) {
                    customEvent.customEventsDuringFrame = new CgkCustomEventsFireDuringFrame {
                        FrameNumber = Time.frameCount,
                        CustomEventHashes = new HashSet<int> {
                            eventHash
                        }
                    };
                } else {
                    customEvent.customEventsDuringFrame.CustomEventHashes.Add(eventHash);
                }
            } else {
                customEvent.frameLastFired = Time.frameCount;
            }

            if (Instance.isLoggingOn) {
                Debug.Log("Firing Custom Event: " + customEventName);
            }

            if (!ReceiversByEventName.ContainsKey(customEventName)) {
                // no receivers
                return;
            }

            var originPoint = originObject.position;

            float? sqrDist = null;

            var dict = ReceiversByEventName[customEventName];

            List<ICgkEventReceiver> validReceivers = null;

            switch (customEvent.eventRcvMode) {
                case EventReceiveMode.Never:
                    if (Instance.isLoggingOn) {
                        Debug.LogWarning("Custom Event '" + customEventName + "' not being transmitted because it is set to 'Never transmit'.");
                    }
                    return; // no transmission.
                case EventReceiveMode.OnChildGameObject:
                    validReceivers = GetChildReceivers(originObject, customEventName, false);
                    break;
                case EventReceiveMode.OnParentGameObject:
                    validReceivers = GetParentReceivers(originObject, customEventName, false);
                    break;
                case EventReceiveMode.OnSameOrChildGameObject:
                    validReceivers = GetChildReceivers(originObject, customEventName, true);
                    break;
                case EventReceiveMode.OnSameOrParentGameObject:
                    validReceivers = GetParentReceivers(originObject, customEventName, true);
                    break;
                case EventReceiveMode.WhenDistanceLessThan:
                case EventReceiveMode.WhenDistanceMoreThan:
                    sqrDist = customEvent.distanceThreshold.Value * customEvent.distanceThreshold.Value;
                    break;
            }

            if (validReceivers == null) {
                validReceivers = new List<ICgkEventReceiver>();

                // only used for "OnXGameObject" Send To Receiver modes
                foreach (var receiver in dict.Keys) {
                    switch (customEvent.eventRcvMode) {
                        case EventReceiveMode.WhenDistanceLessThan:
                            var dist1 = (dict[receiver].position - originPoint).sqrMagnitude;
                            if (dist1 > sqrDist) {
                                continue;
                            }
                            break;
                        case EventReceiveMode.WhenDistanceMoreThan:
                            var dist2 = (dict[receiver].position - originPoint).sqrMagnitude;
                            if (dist2 < sqrDist) {
                                continue;
                            }
                            break;
                        case EventReceiveMode.OnSameGameObject:
                            if (originObject != dict[receiver]) {
                                continue; // not same Transform
                            }
                            break;
                    }

                    validReceivers.Add(receiver);
                }
            }

            var mustSortAndFilter = customEvent.eventRcvFilterMode != EventReceiveFilter.All &&
                customEvent.filterModeQty.Value < validReceivers.Count && validReceivers.Count > 1;

            if (!mustSortAndFilter) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < validReceivers.Count; i++) {
                    validReceivers[i].ReceiveEvent(customEventName, originPoint);
                }
                return;
            }

            // further filter by "random" or "closest"
            ValidReceivers.Clear();

            // ReSharper disable TooWideLocalVariableScope
            Transform receiverTrans;
            int randId;
            float dist;
            // ReSharper restore TooWideLocalVariableScope

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < validReceivers.Count; i++) {
                var receiver = validReceivers[i];
                receiverTrans = dict[receiver];
                dist = 0f;
                randId = 0;

                switch (customEvent.eventRcvFilterMode) {
                    case EventReceiveFilter.Random:
                        randId = UnityEngine.Random.Range(0, 1000);
                        break;
                    case EventReceiveFilter.Closest:
                        dist = (receiverTrans.position - originPoint).sqrMagnitude;
                        break;
                }

                ValidReceivers.Add(new CustomEventCandidate(dist, receiver, receiverTrans, randId));
            }

            int firstDeadIndex;
            int countToRemove;

            // filter out based on fields above
            switch (customEvent.eventRcvFilterMode) {
                case EventReceiveFilter.Closest:
                    ValidReceivers.Sort(delegate (CustomEventCandidate x, CustomEventCandidate y) {
                        return x.DistanceAway.CompareTo(y.DistanceAway);
                    });

                    firstDeadIndex = customEvent.filterModeQty.Value;
                    countToRemove = ValidReceivers.Count - firstDeadIndex;
                    ValidReceivers.RemoveRange(firstDeadIndex, countToRemove);

                    break;
                case EventReceiveFilter.Random:
                    ValidReceivers.Sort(delegate (CustomEventCandidate x, CustomEventCandidate y) {
                        return x.RandomId.CompareTo(y.RandomId);
                    });

                    firstDeadIndex = customEvent.filterModeQty.Value;
                    countToRemove = ValidReceivers.Count - firstDeadIndex;
                    ValidReceivers.RemoveRange(firstDeadIndex, countToRemove);
                    break;
            }

            // filter done, fire events!
            for (var i = 0; i < ValidReceivers.Count; i++) {
                ValidReceivers[i].Receiver.ReceiveEvent(customEventName, originPoint);
            }
        }

        private static CgkCustomEvent GetCustomEventByName(string customEventName) {
            var matches = Instance.customEvents.FindAll(delegate (CgkCustomEvent obj) {
                return obj.EventName ==
                       customEventName;
            });

            return matches.Count > 0 ? matches[0] : null;
        }

        /// <summary>
        /// Calling this method will return whether or not the specified Custom Event exists.
        /// </summary>
        /// <param name="customEventName">The Custom Event name.</param>
        public static bool CustomEventExists(string customEventName) {
            if (AppIsShuttingDown) {
                return true;
            }

            return GetCustomEventByName(customEventName) != null;
        }

        /*! \cond PRIVATE */
        private static List<ICgkEventReceiver> GetChildReceivers(Transform origin, string eventName, bool includeSelf) {
#if UNITY_5 || UNITY_2017_1_OR_NEWER
            var components = origin.GetComponentsInChildren<ICgkEventReceiver>().ToList();

            components.RemoveAll(delegate (ICgkEventReceiver rec) {
                return !rec.SubscribesToEvent(eventName);
            });

            if (includeSelf) {
                return components;
            }

            return FilterOutSelf(components, origin);
#else
			var components = origin.GetComponentsInChildren<MonoBehaviour>().ToList();
			var receivers = GetReceiversFromMonos(components, eventName, includeSelf, origin);
			
			return receivers;
#endif
        }

        private static List<ICgkEventReceiver> GetParentReceivers(Transform origin, string eventName, bool includeSelf) {
#if UNITY_5 || UNITY_2017_1_OR_NEWER
            var components = origin.GetComponentsInParent<ICgkEventReceiver>().ToList();

            components.RemoveAll(delegate (ICgkEventReceiver rec) {
                return !rec.SubscribesToEvent(eventName);
            });

            if (includeSelf) {
                return components;
            }

            return FilterOutSelf(components, origin);
#else
			var components = origin.GetComponentsInParent<MonoBehaviour>().ToList();
			var receivers = GetReceiversFromMonos(components, eventName, includeSelf, origin);
			
			return receivers;
#endif
        }

        private static List<ICgkEventReceiver> FilterOutSelf(List<ICgkEventReceiver> sourceList, Transform origin) {
            var matchOriginComponents = new List<ICgkEventReceiver>();

            foreach (var component in sourceList) {
                var mono = component as MonoBehaviour;
                if (mono == null || mono.transform != origin) {
                    continue;
                }

                matchOriginComponents.Add(component);
            }

            var failsafe = 0;
            while (matchOriginComponents.Count > 0 && failsafe < MaxComponents) {
                sourceList.Remove(matchOriginComponents[0]);
                matchOriginComponents.RemoveAt(0);
                failsafe++;
            }

            return sourceList;
        }

#if UNITY_5 || UNITY_2017_1_OR_NEWER
        // not needed
#else
		private static List<ICgkEventReceiver> GetReceiversFromMonos(List<MonoBehaviour> monos, string eventName, bool includeSelf, Transform origin) {
			var receivers = new List<ICgkEventReceiver>();
			
			foreach (var component in monos) {
				var rec = component as ICgkEventReceiver;
				if (rec == null) {
					continue;
				}
				
				if (!rec.SubscribesToEvent(eventName)) {
					continue;
				}    
				
				if (!includeSelf && component.transform == origin) {
					continue;
				}
				
				receivers.Add(rec);
			}
			
			return receivers;
		}
#endif

		/// <summary>
		/// Generates a hash based on the event name and the x, y, z of the origin
		/// </summary>
		/// <param name="customEventName">name of the event to generate a hash for</param>
		/// <param name="eventOrigin">origin of the event to generate a hash for</param>
		/// <returns></returns>
		private static int GetCustomEventHash(string customEventName, Vector3 eventOrigin) {
            // anonymous class to generate the hash.  per documentation anonymous hash will match if all params are the same.
            return new { customEventName, eventOrigin.x, eventOrigin.y, eventOrigin.z }.GetHashCode();
        }

        /// <summary>
        /// Gets the custom event parameter passed to FireCustomEventWithParam, during the same frame only.
        /// </summary>
        /// <returns>The custom event parameter.</returns>
        /// <param name="customEventName">Custom event name.</param>
        /// <param name="eventOrigin">Event origin.</param>
        public static T GetCustomEventParam<T>(string customEventName, Vector3 eventOrigin) {
			// get the event hash
			int paramKey = GetCustomEventHash(customEventName, eventOrigin);
			object eventParam = null;
			
			// if a param exists, retrieve it
			if (CustomEventParams.ContainsKey(paramKey)) {
				eventParam = CustomEventParams[paramKey];
			} 
			
			return (T)Convert.ChangeType(eventParam, typeof(T));
		}

        /// <summary>
        /// Gets the sender of a custom event parameter passed to FireCustomEventWithParam, during the same frame only.
        /// </summary>
        /// <returns>The custom event parameter sender Transform.</returns>
        /// <param name="customEventName">Custom event name.</param>
        /// <param name="eventOrigin">Event origin.</param>
        public static Transform GetCustomEventParamSender(string customEventName, Vector3 eventOrigin) {
            // get the event hash
            int paramKey = GetCustomEventHash(customEventName, eventOrigin);
            Transform eventParamSender = null;

            // if a param sender exists, retrieve it
            if (CustomEventParamSenders.ContainsKey(paramKey)) {
                eventParamSender = CustomEventParamSenders[paramKey];
            }

            return eventParamSender;
        }

        public static Transform WorldVariablePanel {
			get {
				return Instance.transform.GetChildTransform(WorldVariablesContainerTransName);
			}
		}

		public static Transform PrefabPoolsPanel {
			get {
				return Instance.transform.GetChildTransform(PrefabPoolsContainerTransName);
			}
		}

		public static Transform PoolBossPanel {
			get {
				return Instance.transform.GetChildTransform(KillerPoolingContainerTransName);
			}
		}

        /*! \endcond */

        /// <summary>
        /// This will return a list of all the Custom Events you have defined, including the selectors for "type in" and "none".
        /// </summary>
        public List<string> CustomEventNames {
            get {
                var customEventNames = new List<string> { DynamicEventName, NoEventName };


                var custEvents = Instance.customEvents;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < custEvents.Count; i++) {
                    customEventNames.Add(custEvents[i].EventName);
                }

                return customEventNames;
            }
        }

        #endregion
    }
}