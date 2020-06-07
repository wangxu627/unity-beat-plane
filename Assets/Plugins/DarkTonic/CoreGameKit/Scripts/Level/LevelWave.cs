/*! \cond PRIVATE */
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class LevelWave {
        // ReSharper disable InconsistentNaming
        public LevelSettings.WaveType waveType = LevelSettings.WaveType.Timed;
        public LevelSettings.SkipWaveMode skipWaveType = LevelSettings.SkipWaveMode.None;
        public WorldVariableCollection skipWavePassCriteria = new WorldVariableCollection();
        public bool pauseGlobalWavesWhenCompleted;
        public string waveName = "UNNAMED";
        public string waveClass = "None";
		public string waveDescription = "Your wave description goes here.";
		public LevelWaveMusicSettings musicSettings = new LevelWaveMusicSettings();
        public int WaveDuration = LevelSettings.DefaultWaveDuration;
        public KillerInt waveDurationFlex = new KillerInt(LevelSettings.DefaultWaveDuration, 0, 5000);
        public bool endEarlyIfAllDestroyed;
        public bool waveBeatBonusesEnabled;
        public bool useCompletionEvents;
        public bool useSpawnBonusPrefab;
        public WaveSpecifics.SpawnOrigin bonusPrefabSource = WaveSpecifics.SpawnOrigin.Specific;
        public int bonusPrefabPoolIndex = 0;
        public string bonusPrefabPoolName = null;
        public Transform bonusPrefabSpecific;
        public KillerInt bonusPrefabSpawnPercent = new KillerInt(100, 0, 100);
        public KillerInt bonusPrefabQty = new KillerInt(1, 0, 100);
        public string bonusPrefabCategoryName;

        public LevelSettings.WaveSpawnerUseMode spawnerUseMode = LevelSettings.WaveSpawnerUseMode.AllAbove;
        public int spawnersToUseMin = 1;
        public int spawnersToUseMax = 1;
        public bool isDummyWave;
        public bool useTriggeredSpawners;
        public List<TrigSpawnerWaveWaiter> trigSpawnerWavesToAwait = new List<TrigSpawnerWaveWaiter>(); 

        public List<CGKCustomEventToFire> completionCustomEvents = new List<CGKCustomEventToFire>();

        public WorldVariableCollection waveDefeatVariableModifiers = new WorldVariableCollection();
        public bool isExpanded = true;
        public int sequencedWaveNumber = 0;
        public int randomWaveNumber = 0; // assigned and only used for random sorting.
        // ReSharper restore InconsistentNaming
    }
}
/*! \endcond */