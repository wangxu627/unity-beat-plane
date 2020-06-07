/*! \cond PRIVATE */
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class TriggeredWaveSpecifics {
        // ReSharper disable InconsistentNaming
        public bool isExpanded = true;
        public bool enableWave = false;
        public KillerInt NumberToSpwn = new KillerInt(1, 0, 1024);
        public KillerInt MaxToSpawn = new KillerInt(1, 0, 1024);
        public KillerFloat WaveDelaySec = new KillerFloat(0f, 0f, float.MaxValue);
        public bool doesRepeatUseWaveDelay;
        public KillerFloat TimeToSpawnEntireWave = new KillerFloat(0f, 0f, float.MaxValue);
        public Transform prefabToSpawn;
		public string prefabToSpawnCategoryName;
		public bool visualizeWave = true;
        public WaveSpecifics.SpawnOrigin spawnSource = WaveSpecifics.SpawnOrigin.Specific;
        public int prefabPoolIndex = 0;
        public string prefabPoolName = null;

        public bool enableRepeatWave = false;
        public TriggeredSpawnerV2.RepeatWaitFor repeatWaitsForType = TriggeredSpawnerV2.RepeatWaitFor.ItemsDoneSpawning;
        public WaveSpecifics.RepeatWaveMode curWaveRepeatMode = WaveSpecifics.RepeatWaveMode.NumberOfRepetitions;
        public KillerFloat repeatWavePauseSec = new KillerFloat(-1f, .1f, float.MaxValue);
        public KillerInt maxRepeat = new KillerInt(2, 2, int.MaxValue);
        public KillerInt repeatItemInc = new KillerInt(0, -100, 100);
        public KillerInt repeatItemMinLmt = new KillerInt(1, 0, int.MaxValue);
        public KillerInt repeatItemLmt = new KillerInt(100, 0, int.MaxValue);
        public bool resetOnItemLimitReached;

        public KillerFloat repeatTimeInc = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat repeatTimeMinLmt = new KillerFloat(1f, 0, float.MaxValue);
        public KillerFloat repeatTimeLmt = new KillerFloat(100f, 0, float.MaxValue);
        public bool resetOnTimeLimitReached;

        public bool useWaveSpawnBonusForRepeats = false;
        public bool useWaveSpawnBonusForBeginning = true;
        
        public bool waveSpawnFireEvents = false;
        public List<CGKCustomEventToFire> waveSpawnCustomEvents = new List<CGKCustomEventToFire>();

        public bool waveRepeatFireEvents = false;
        public List<CGKCustomEventToFire> waveRepeatCustomEvents = new List<CGKCustomEventToFire>();

        public WorldVariableCollection repeatPassCriteria = new WorldVariableCollection();
        public bool willDespawnOnEvent = false;

        public WaveSpecifics.WaveOffsetChoiceMode offsetChoiceMode = WaveSpecifics.WaveOffsetChoiceMode.RandomlyChosen;
        public List<Vector3> waveOffsetList = new List<Vector3>();

        public bool waveSpawnBonusesEnabled = false;
        public WorldVariableCollection waveSpawnVariableModifiers = new WorldVariableCollection();

        public bool waveElimBonusesEnabled = false;
        public WorldVariableCollection waveElimVariableModifiers = new WorldVariableCollection();
        public bool waveElimFireEvents = false;
        public List<CGKCustomEventToFire> waveElimCustomEvents = new List<CGKCustomEventToFire>();

        public bool useSpawnBonusPrefab = false;
        public WaveSpecifics.SpawnOrigin bonusPrefabSource = WaveSpecifics.SpawnOrigin.Specific;
        public int bonusPrefabPoolIndex = 0;
        public string bonusPrefabPoolName = null;
        public Transform bonusPrefabSpecific;
        public KillerInt bonusPrefabSpawnPercent = new KillerInt(100, 0, 100);
        public KillerInt bonusPrefabQty = new KillerInt(1, 0, 100);
        public string bonusPrefabCategoryName;

        public bool useLayerFilter = false;
        public bool useTagFilter = false;
        public List<string> matchingTags = new List<string>() { "Untagged" };
        public List<int> matchingLayers = new List<int>() { 0 };

        public bool positionExpanded = true;
        public WaveSpecifics.PositionMode positionXmode = WaveSpecifics.PositionMode.SpawnerPosition;
        public WaveSpecifics.PositionMode positionYmode = WaveSpecifics.PositionMode.SpawnerPosition;
        public WaveSpecifics.PositionMode positionZmode = WaveSpecifics.PositionMode.SpawnerPosition;
        public KillerFloat customPosX = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat customPosY = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat customPosZ = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public Transform otherObjectX;
        public Transform otherObjectY;
        public Transform otherObjectZ;

        public WaveSpecifics.RotationMode curRotationMode = WaveSpecifics.RotationMode.UsePrefabRotation;
        public Vector3 customRotation = Vector3.zero;
        public Vector3 keepCenterRotation = Vector3.zero;

        public WaveSpecifics.SpawnerRotationMode curSpawnerRotMode = WaveSpecifics.SpawnerRotationMode.KeepRotation;

        public bool eventOriginIgnoreX = false;
        public bool eventOriginIgnoreY = false;
        public bool eventOriginIgnoreZ = false;

        // for custom events only
        public bool customEventActive = false;
        public bool isCustomEvent = false;
        public string customEventName = string.Empty;
        public Vector3 customEventLookRotation = Vector3.zero;

        public bool enableRandomizations;
        public bool randomXRotation;
        public bool randomYRotation;
        public bool randomZRotation;
        public KillerFloat randomDistX = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
        public KillerFloat randomDistY = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
        public KillerFloat randomDistZ = new KillerFloat(0f, 0f, TriggeredSpawner.MaxDistance);
        public KillerFloat randomXRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomXRotMax = new KillerFloat(360f, 0f, 360f);
        public KillerFloat randomYRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomYRotMax = new KillerFloat(360f, 0f, 360f);
        public KillerFloat randomZRotMin = new KillerFloat(0f, 0f, 360f);
        public KillerFloat randomZRotMax = new KillerFloat(360f, 0f, 360f);

        public bool enableIncrements;
        public bool enableKeepCenter;
        public KillerFloat incrementPositionX = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementPositionY = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementPositionZ = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat incrementRotX = new KillerFloat(0f, -180f, 180f);
        public KillerFloat incrementRotY = new KillerFloat(0f, -180f, 180f);
        public KillerFloat incrementRotZ = new KillerFloat(0f, -180f, 180f);

        public bool enablePostSpawnNudge = false;
        public KillerFloat postSpawnNudgeFwd = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat postSpawnNudgeRgt = new KillerFloat(0f, float.MinValue, float.MaxValue);
        public KillerFloat postSpawnNudgeDwn = new KillerFloat(0f, float.MinValue, float.MaxValue);

        // optional wave end triggers
        public bool stopWaveOnOppositeEvent = false;

        // retrigger limit settings
        public bool disableAfterFirstTrigger = false;
        public TriggeredSpawner.RetriggerLimitMode retriggerLimitMode = TriggeredSpawner.RetriggerLimitMode.None;
        public KillerInt limitPerXFrm = new KillerInt(1, 1, int.MaxValue);
        public KillerFloat limitPerXSec = new KillerFloat(0.1f, .1f, float.MaxValue);

        public float triggerStayForTime;

        public int trigLastFrame = -10000;
        public float trigLastTime = -10000f;

        private int waveOffsetIndex;
        // ReSharper restore InconsistentNaming

        public enum SpawnSource {
            Specific,
            PrefabPool
        }

        public bool IsValid {
            get {
                if (!enableWave) {
                    return false;
                }

                return true;
            }
        }

        public Vector3 WaveOffset {
            get {
                if (waveOffsetList.Count == 0) {
                    waveOffsetList.Add(Vector3.zero);
                    return Vector3.zero;
                }

                var index = 0;

                switch (offsetChoiceMode) {
                    case WaveSpecifics.WaveOffsetChoiceMode.RandomlyChosen:
                        index = UnityEngine.Random.Range(0, waveOffsetList.Count);
                        break;
                    case WaveSpecifics.WaveOffsetChoiceMode.UseInOrder:
                        index = waveOffsetIndex;
                        waveOffsetIndex++;
                        if (waveOffsetIndex >= waveOffsetList.Count) {
                            waveOffsetIndex = 0;
                        }
                        break;
                }

                return waveOffsetList[index];
            }
        }
    }
}
/*! \endcond */