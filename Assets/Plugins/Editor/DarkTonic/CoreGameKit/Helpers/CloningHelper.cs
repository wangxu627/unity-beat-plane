using System.Collections.Generic;
using DarkTonic.CoreGameKit;

// ReSharper disable once CheckNamespace
public static class CloningHelper {
    public static LevelSpecifics CloneLevel(LevelSpecifics level) {
        var clone = new LevelSpecifics {
            levelName = level.levelName,
            waveOrder = level.waveOrder,
            WaveSettings = new List<LevelWave>(),
            isExpanded = level.isExpanded
        };

        foreach (var wave in level.WaveSettings) {
            clone.WaveSettings.Add(CloneLevelWave(wave));
        }

        return clone;
    }

    public static LevelWave CloneLevelWave(LevelWave levelWave) {
        var clone = new LevelWave {
            pauseGlobalWavesWhenCompleted = levelWave.pauseGlobalWavesWhenCompleted,
            waveType = levelWave.waveType,
            skipWaveType = levelWave.skipWaveType,
            skipWavePassCriteria = levelWave.skipWavePassCriteria,
            waveName = levelWave.waveName,
            musicSettings = levelWave.musicSettings,
            WaveDuration = levelWave.WaveDuration,
            endEarlyIfAllDestroyed = levelWave.endEarlyIfAllDestroyed,
            waveBeatBonusesEnabled = levelWave.waveBeatBonusesEnabled,
            useCompletionEvents = levelWave.useCompletionEvents,
            completionCustomEvents = levelWave.completionCustomEvents,
            waveDefeatVariableModifiers = levelWave.waveDefeatVariableModifiers,
            isExpanded = levelWave.isExpanded,
            useSpawnBonusPrefab = levelWave.useSpawnBonusPrefab,
            bonusPrefabCategoryName = levelWave.bonusPrefabCategoryName,
            bonusPrefabPoolIndex = levelWave.bonusPrefabPoolIndex,
            bonusPrefabPoolName = levelWave.bonusPrefabPoolName,
            bonusPrefabSource = levelWave.bonusPrefabSource,
            bonusPrefabSpecific = levelWave.bonusPrefabSpecific,
            spawnerUseMode = levelWave.spawnerUseMode,
            spawnersToUseMax = levelWave.spawnersToUseMax,
            spawnersToUseMin = levelWave.spawnersToUseMin,
            waveClass = levelWave.waveClass,
            waveDescription = levelWave.waveDescription,
            waveDurationFlex = levelWave.waveDurationFlex,
            useTriggeredSpawners = levelWave.useTriggeredSpawners,
            trigSpawnerWavesToAwait = levelWave.trigSpawnerWavesToAwait
        }; 

        KillerVariablesHelper.CloneKillerInt(clone.bonusPrefabSpawnPercent, levelWave.bonusPrefabSpawnPercent);
        KillerVariablesHelper.CloneKillerInt(clone.bonusPrefabQty, levelWave.bonusPrefabQty);
        KillerVariablesHelper.CloneKillerInt(clone.waveDurationFlex, levelWave.waveDurationFlex);

        return clone;
    }

    public static WaveSpecifics CloneWave(WaveSpecifics sourceWave) {
        var clone = new WaveSpecifics {
            isExpanded = sourceWave.isExpanded,
            enableWave = sourceWave.enableWave,
            visualizeWave = sourceWave.visualizeWave,
            SpawnLevelNumber = sourceWave.SpawnLevelNumber,
            SpawnWaveNumber = sourceWave.SpawnWaveNumber,
            prefabToSpawn = sourceWave.prefabToSpawn,
            spawnSource = sourceWave.spawnSource,
            prefabPoolIndex = sourceWave.prefabPoolIndex,
            prefabPoolName = sourceWave.prefabPoolName,
            repeatWaveUntilNew = sourceWave.repeatWaveUntilNew,
            waveCompletePercentage = sourceWave.waveCompletePercentage,

            curWaveRepeatMode = sourceWave.curWaveRepeatMode,
            curTimedRepeatWaveMode = sourceWave.curTimedRepeatWaveMode,
            resetOnItemLimitReached = sourceWave.resetOnItemLimitReached,
            resetOnTimeLimitReached = sourceWave.resetOnTimeLimitReached,
            repeatPassCriteria = sourceWave.repeatPassCriteria,

            waveRepeatBonusesEnabled = sourceWave.waveRepeatBonusesEnabled,
            waveRepeatVariableModifiers = sourceWave.waveRepeatVariableModifiers,
            waveRepeatFireEvents = sourceWave.waveRepeatFireEvents,
            waveRepeatCustomEvents = sourceWave.waveRepeatCustomEvents,

            positionExpanded = sourceWave.positionExpanded,
            positionXmode = sourceWave.positionXmode,
            positionYmode = sourceWave.positionYmode,
            positionZmode = sourceWave.positionZmode,

            curRotationMode = sourceWave.curRotationMode,
            customRotation = sourceWave.customRotation,

            enableLimits = sourceWave.enableLimits,

            enableRandomizations = sourceWave.enableRandomizations,
            randomXRotation = sourceWave.randomXRotation,
            randomYRotation = sourceWave.randomYRotation,
            randomZRotation = sourceWave.randomZRotation,

            enableIncrements = sourceWave.enableIncrements,
            enableKeepCenter = sourceWave.enableKeepCenter,

            waveOffsetList = sourceWave.waveOffsetList,

            enablePostSpawnNudge = sourceWave.enablePostSpawnNudge,

            useSpawnBonusPrefab = sourceWave.useSpawnBonusPrefab,
            bonusPrefabSource = sourceWave.bonusPrefabSource,
            bonusPrefabPoolIndex = sourceWave.bonusPrefabPoolIndex,
            bonusPrefabPoolName = sourceWave.bonusPrefabPoolName,
            bonusPrefabSpecific = sourceWave.bonusPrefabSpecific,
            bonusPrefabCategoryName = sourceWave.bonusPrefabCategoryName,
            bonusRepeatToUseItem = sourceWave.bonusRepeatToUseItem
        };

        KillerVariablesHelper.CloneKillerInt(clone.MinToSpwn, sourceWave.MinToSpwn);
        KillerVariablesHelper.CloneKillerInt(clone.MaxToSpwn, sourceWave.MaxToSpwn);
        KillerVariablesHelper.CloneKillerFloat(clone.WaveDelaySec, sourceWave.WaveDelaySec);
        KillerVariablesHelper.CloneKillerFloat(clone.TimeToSpawnEntireWave, sourceWave.TimeToSpawnEntireWave);
        KillerVariablesHelper.CloneKillerFloat(clone.repeatPauseMinimum, sourceWave.repeatPauseMinimum);
        KillerVariablesHelper.CloneKillerFloat(clone.repeatPauseMaximum, sourceWave.repeatPauseMaximum);
        KillerVariablesHelper.CloneKillerInt(clone.repeatItemInc, sourceWave.repeatItemInc);
        KillerVariablesHelper.CloneKillerInt(clone.repeatItemMinLmt, sourceWave.repeatItemMinLmt);
        KillerVariablesHelper.CloneKillerInt(clone.repeatItemLmt, sourceWave.repeatItemLmt);
        KillerVariablesHelper.CloneKillerFloat(clone.repeatTimeInc, sourceWave.repeatTimeInc);
        KillerVariablesHelper.CloneKillerFloat(clone.repeatTimeMinLmt, sourceWave.repeatTimeMinLmt);
        KillerVariablesHelper.CloneKillerFloat(clone.repeatTimeLmt, sourceWave.repeatTimeLmt);
        KillerVariablesHelper.CloneKillerInt(clone.repetitionsToDo, sourceWave.repetitionsToDo);
        KillerVariablesHelper.CloneKillerFloat(clone.customPosX, sourceWave.customPosX);
        KillerVariablesHelper.CloneKillerFloat(clone.customPosY, sourceWave.customPosY);
        KillerVariablesHelper.CloneKillerFloat(clone.customPosZ, sourceWave.customPosZ);
        KillerVariablesHelper.CloneKillerFloat(clone.doNotSpawnIfMbrCloserThan, sourceWave.doNotSpawnIfMbrCloserThan);
        KillerVariablesHelper.CloneKillerFloat(clone.doNotSpawnRandomDist, sourceWave.doNotSpawnRandomDist);
        KillerVariablesHelper.CloneKillerFloat(clone.randomDistX, sourceWave.randomDistX);
        KillerVariablesHelper.CloneKillerFloat(clone.randomDistY, sourceWave.randomDistY);
        KillerVariablesHelper.CloneKillerFloat(clone.randomDistZ, sourceWave.randomDistZ);
        KillerVariablesHelper.CloneKillerFloat(clone.randomXRotMin, sourceWave.randomXRotMin);
        KillerVariablesHelper.CloneKillerFloat(clone.randomXRotMax, sourceWave.randomXRotMax);
        KillerVariablesHelper.CloneKillerFloat(clone.randomYRotMin, sourceWave.randomYRotMin);
        KillerVariablesHelper.CloneKillerFloat(clone.randomYRotMax, sourceWave.randomYRotMax);
        KillerVariablesHelper.CloneKillerFloat(clone.randomZRotMin, sourceWave.randomZRotMin);
        KillerVariablesHelper.CloneKillerFloat(clone.randomZRotMax, sourceWave.randomZRotMax);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementPositionX, sourceWave.incrementPositionX);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementPositionY, sourceWave.incrementPositionY);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementPositionZ, sourceWave.incrementPositionZ);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementRotX, sourceWave.incrementRotX);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementRotY, sourceWave.incrementRotY);
        KillerVariablesHelper.CloneKillerFloat(clone.incrementRotZ, sourceWave.incrementRotZ);
        KillerVariablesHelper.CloneKillerFloat(clone.postSpawnNudgeFwd, sourceWave.postSpawnNudgeFwd);
        KillerVariablesHelper.CloneKillerFloat(clone.postSpawnNudgeRgt, sourceWave.postSpawnNudgeRgt);
        KillerVariablesHelper.CloneKillerFloat(clone.postSpawnNudgeDwn, sourceWave.postSpawnNudgeDwn);
        KillerVariablesHelper.CloneKillerInt(clone.bonusPrefabSpawnPercent, sourceWave.bonusPrefabSpawnPercent);
        KillerVariablesHelper.CloneKillerInt(clone.bonusPrefabQty, sourceWave.bonusPrefabQty);

        return clone;
    }

    public static WavePrefabPoolItem CloneWavePrefabPoolItem(WavePrefabPoolItem sourceItem) {
        var clone = new WavePrefabPoolItem {
            prefabToSpawn = sourceItem.prefabToSpawn,
            activeMode = sourceItem.activeMode,
            activeItemCriteria = sourceItem.activeItemCriteria,
            isExpanded = sourceItem.isExpanded
        };

        KillerVariablesHelper.CloneKillerInt(clone.thisWeight, sourceItem.thisWeight);

        return clone;
    }
}
