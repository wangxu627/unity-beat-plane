using System;
using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using RelationsInspector.Backend.CoreGameKit;

[CustomEditor(typeof(LevelSettings))]
// ReSharper disable once CheckNamespace
public class LevelSettingsInspector : Editor {
    private LevelSettings _settings;
    private bool _isDirty;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        EditorGUI.indentLevel = 0;

        _settings = (LevelSettings)target;

        WorldVariableTracker.ClearInGamePlayerStats();

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);

        _isDirty = false;

        if (DTInspectorUtility.IsPrefabInProjectView(_settings)) {
            DTInspectorUtility.ShowTopGameObjectNotPrefabMessage();

            EditorGUILayout.Separator();

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6);
            if (GUILayout.Button("Create LevelWaveSettings Prefab", EditorStyles.toolbarButton, GUILayout.Width(180))) {
                CreateLevelSettingsPrefab();
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
            return;
        }

        var allStats = KillerVariablesHelper.AllStatNames;
        var poolNames = LevelSettings.GetSortedPrefabPoolNames();

        var playerStatsHolder = _settings.transform.GetChildTransform(LevelSettings.WorldVariablesContainerTransName);
        if (playerStatsHolder == null) { 
            Debug.LogError("You have no child prefab of LevelSettings called '" + LevelSettings.WorldVariablesContainerTransName + "'. " + LevelSettings.RevertLevelSettingsAlert);
            DTInspectorUtility.ShowRedErrorBox("Please check the console. You have a breaking error.");
            return;
        }
		 
		DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm");

        EditorGUI.indentLevel = 0;

        DTInspectorUtility.StartGroupHeader();

        var newUseWaves = EditorGUILayout.BeginToggleGroup(" Use Global Waves", _settings.useWaves);
        if (newUseWaves != _settings.useWaves) {
            if (Application.isPlaying) {
                DTInspectorUtility.ShowAlert("Cannot change this setting at runtime.");
            } else {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Global Waves");
                _settings.useWaves = newUseWaves;
            }
        }
        DTInspectorUtility.EndGroupHeader();

        if (_settings.useWaves) {
            EditorGUI.indentLevel = 0;

            DTInspectorUtility.StartGroupHeader(1);
            var newUseMusic = GUILayout.Toggle(_settings.useMusicSettings, " Use Music Settings");
            if (newUseMusic != _settings.useMusicSettings) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Music Settings");
                _settings.useMusicSettings = newUseMusic;
            }
            EditorGUILayout.EndVertical();

            if (_settings.useMusicSettings) {
                EditorGUI.indentLevel = 0;

                var newGoMusic = (LevelSettings.WaveMusicMode)EditorGUILayout.EnumPopup("G.O. Music Mode", _settings.gameOverMusicSettings.WaveMusicMode);
                if (newGoMusic != _settings.gameOverMusicSettings.WaveMusicMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change G.O. Music Mode");
                    _settings.gameOverMusicSettings.WaveMusicMode = newGoMusic;
                }
                if (_settings.gameOverMusicSettings.WaveMusicMode == LevelSettings.WaveMusicMode.PlayNew) {
                    var newWaveMusic = (AudioClip)EditorGUILayout.ObjectField("G.O. Music", _settings.gameOverMusicSettings.WaveMusic, typeof(AudioClip), true);
                    if (newWaveMusic != _settings.gameOverMusicSettings.WaveMusic) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign G.O. Music");
                        _settings.gameOverMusicSettings.WaveMusic = newWaveMusic;
                    }
                }
                if (_settings.gameOverMusicSettings.WaveMusicMode != LevelSettings.WaveMusicMode.Silence) {
                    var newMusicVol = EditorGUILayout.Slider("G.O. Music Volume", _settings.gameOverMusicSettings.WaveMusicVolume, 0f, 1f);
                    if (newMusicVol != _settings.gameOverMusicSettings.WaveMusicVolume) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change G.O. Music Volume");
                        _settings.gameOverMusicSettings.WaveMusicVolume = newMusicVol;
                    }
                } else {
                    var newFadeTime = EditorGUILayout.Slider("Silence Fade Time", _settings.gameOverMusicSettings.FadeTime, 0f, 15f);
                    if (newFadeTime != _settings.gameOverMusicSettings.FadeTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Silence Fade Time");
                        _settings.gameOverMusicSettings.FadeTime = newFadeTime;
                    }
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel = 0;

            DTInspectorUtility.AddSpaceForNonU5();
            DTInspectorUtility.StartGroupHeader(1);
            var newEnableWarp = GUILayout.Toggle(_settings.enableWaveWarp, " Custom Start Wave?");
            if (newEnableWarp != _settings.enableWaveWarp) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Start Wave?");
                _settings.enableWaveWarp = newEnableWarp;
            }
            EditorGUILayout.EndVertical();

            if (_settings.enableWaveWarp) {
                EditorGUI.indentLevel = 0;

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _settings.startLevelNumber, "Custom Start Level#", _settings);
                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, _settings.startWaveNumber, "Custom Start Wave#", _settings);
            }
            EditorGUILayout.EndVertical();
            DTInspectorUtility.ResetColors();

            var newDisableSyncro = EditorGUILayout.Toggle("Syncro Spawners Off", _settings.disableSyncroSpawners);
            if (newDisableSyncro != _settings.disableSyncroSpawners) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Syncro Spawners Off");
                _settings.disableSyncroSpawners = newDisableSyncro;
            }

            var newStart = EditorGUILayout.Toggle("Auto Start Waves", _settings.startFirstWaveImmediately);
            if (newStart != _settings.startFirstWaveImmediately) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Auto Start Waves");
                _settings.startFirstWaveImmediately = newStart;
            }

            var newDestroy = (LevelSettings.WaveRestartBehavior)EditorGUILayout.EnumPopup("Wave Restart Mode", _settings.waveRestartMode);
            if (newDestroy != _settings.waveRestartMode) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Restart Mode");
                _settings.waveRestartMode = newDestroy;
            }
        }

        EditorGUILayout.EndToggleGroup();

        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        var newUse = EditorGUILayout.BeginToggleGroup(" Use Initialization Options", _settings.initializationSettingsExpanded);
        if (newUse != _settings.initializationSettingsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Initialization Options");
            _settings.initializationSettingsExpanded = newUse;
        }

        if (_settings.initializationSettingsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            DTInspectorUtility.ShowColorWarningBox("When LevelSettings has finished initializing, fire the Custom Events below");

            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Initialization Custom Event");
                _settings.initializationCustomEvents.Add(new CGKCustomEventToFire());
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();

            if (_settings.initializationCustomEvents.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
            }

            DTInspectorUtility.VerticalSpace(2);

            int? indexToDelete = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _settings.initializationCustomEvents.Count; i++) {
                var anEvent = _settings.initializationCustomEvents[i].CustomEventName;

                var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _settings, "Custom Event", ref buttonClicked);

                if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove) {
                    indexToDelete = i;
                }

                if (anEvent == _settings.initializationCustomEvents[i].CustomEventName) {
                    continue;
                }

                _settings.initializationCustomEvents[i].CustomEventName = anEvent;
            }

            if (indexToDelete.HasValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Initialization Custom Event");
                _settings.initializationCustomEvents.RemoveAt(indexToDelete.Value);
            }

            DTInspectorUtility.EndGroupedControls();
        }
        EditorGUILayout.EndToggleGroup();
        DTInspectorUtility.EndGroupHeader();

        EditorGUI.indentLevel = 0;

        if (!Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wave Visualizations", GUILayout.Width(120));
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Refresh All", "Refresh all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var syncros = FindObjectsOfType(typeof(WaveSyncroPrefabSpawner));
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var j = 0; j < syncros.Length; j++) {
                    var aSpawner = (WaveSyncroPrefabSpawner)syncros[j];
                    aSpawner.gameObject.DestroyChildrenImmediateWithMarker();

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var w = 0; w < aSpawner.waveSpecs.Count; w++) {
                        var aWave = aSpawner.waveSpecs[w];
                        if (!aWave.visualizeWave) {
                            continue;
                        }

                        aSpawner.SpawnWaveVisual(aWave);
                        break;
                    }
                }

                var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawnerV2));
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var t = 0; t < trigSpawners.Length; t++) {
                    var trigSpawner = (TriggeredSpawnerV2)trigSpawners[t];

                    // ReSharper disable once ForCanBeConvertedToForeach
                    var allWaves = new List<TriggeredWaveSpecifics>(trigSpawner.AllWaves.Keys.Count);
                    foreach (var key in trigSpawner.AllWaves.Keys) {
                        allWaves.AddRange(trigSpawner.AllWaves[key]);
                    }

                    for (var w = 0; w < allWaves.Count; w++) {
                        var wave = allWaves[w];
                        if (!wave.enableWave || !wave.visualizeWave) {
                            continue;
                        }

                        trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();
                        trigSpawner.SpawnWaveVisual(allWaves[w]);
                        break;
                    }
                }
            }

            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Hide All", "Hide all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var visuals = FindObjectsOfType(typeof(VisualizationMarker));
                var totalItems = visuals.Length;
                var i = 0;
                while (visuals.Length > 0 && i < totalItems) {
                    DestroyImmediate(((VisualizationMarker)visuals[i]).gameObject);
                    i++;
                }
            }

            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Disable All", "Disable all wave visualizations"), EditorStyles.toolbarButton, GUILayout.Width(70))) {
                var visuals = FindObjectsOfType(typeof(VisualizationMarker));
                var totalItems = visuals.Length;
                var i = 0;

                while (visuals.Length > 0 && i < totalItems) {
                    DestroyImmediate(((VisualizationMarker)visuals[i]).gameObject);
                    i++;
                }

                var syncros = FindObjectsOfType(typeof(WaveSyncroPrefabSpawner));

                for (var j = 0; j < syncros.Length; j++) {
                    var aSpawner = (WaveSyncroPrefabSpawner)syncros[j];
                    var hasChanged = false;

                    for (var w = 0; w < aSpawner.waveSpecs.Count; w++) {
                        var aWave = aSpawner.waveSpecs[w];
                        if (!aWave.enableWave || !aWave.visualizeWave) {
                            continue;
                        }

                        aWave.visualizeWave = false;
                        hasChanged = true;
                    }

                    if (hasChanged) {
                        EditorUtility.SetDirty(aSpawner);
                    }
                }

                var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawnerV2));
                // ReSharper disable ForCanBeConvertedToForeach
                for (var j = 0; j < trigSpawners.Length; j++) {
                    var aSpawner = (TriggeredSpawnerV2)trigSpawners[j];

                    var isChanged = false;
                    var allWaves = new List<TriggeredWaveSpecifics>(aSpawner.AllWaves.Keys.Count);
                    foreach (var key in aSpawner.AllWaves.Keys) {
                        allWaves.AddRange(aSpawner.AllWaves[key]);
                    }

                    for (var z = 0; z < allWaves.Count; z++) {
                        var wave = allWaves[z];
                        if (!wave.visualizeWave) {
                            continue;
                        }

                        wave.visualizeWave = false;
                        isChanged = true;
                    }

                    if (isChanged) {
                        EditorUtility.SetDirty(aSpawner);
                    }
                }
            }

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        var newPersist = EditorGUILayout.Toggle("Persist Between Scenes", _settings.persistBetweenScenes);
        if (newPersist != _settings.persistBetweenScenes) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Persist Between Scenes");
            _settings.persistBetweenScenes = newPersist;
        }

        var newLogging = EditorGUILayout.Toggle("Log Messages", _settings.isLoggingOn);
        if (newLogging != _settings.isLoggingOn) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Log Messages");
            _settings.isLoggingOn = newLogging;
        }

        var hadNoListener = _settings.listener == null;
        var newListener = (LevelSettingsListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(LevelSettingsListener), true);
        if (newListener != _settings.listener) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;
            if (hadNoListener && _settings.listener != null) {
                _settings.listener.sourceTransName = _settings.transform.name;
            }
        }

        if (Application.isPlaying && PoolBoss.IsServer) {
            DTInspectorUtility.StartGroupHeader(1, false);
            EditorGUILayout.LabelField("Game Status Panel", EditorStyles.boldLabel);
            //EditorGUILayout.EndVertical();
            if (LevelSettings.IsGameOver) {
                GUI.backgroundColor = Color.red;
                DTInspectorUtility.ShowRedErrorBox("Game Status: GAME OVER");
            } else {
                GUI.backgroundColor = Color.green;
                DTInspectorUtility.ShowColorWarningBox("Game Status: NOT OVER");
            }

            if (_settings.useWaves) {
                if (LevelSettings.WavesArePaused) {
                    GUI.backgroundColor = Color.red;

                    DTInspectorUtility.ShowRedErrorBox("Wave Status: Paused");
                } else {
                    GUI.backgroundColor = Color.green;
                    EditorGUILayout.BeginHorizontal();
                    var level = LevelSettings.CurrentDisplayLevel;
                    var wave = LevelSettings.CurrentDisplayWave;

                    DTInspectorUtility.ShowColorWarningBox("Playing Level: [" + level + "] Wave: [" + wave + "]");
                    EditorGUILayout.EndHorizontal();
                }

                GUI.backgroundColor = Color.yellow;
                if (LevelSettings.ActiveWaveInfo.waveType == LevelSettings.WaveType.Elimination) {
                    DTInspectorUtility.ShowColorWarningBox("Elimination Spawners Remaining: [" +
                                                            LevelSettings.EliminationSpawnersRemaining + "]");
                } else {
                    DTInspectorUtility.ShowColorWarningBox("Timed Wave, Remaining: [" + LevelSettings.TimeRemainingInCurrentWave + "]");
                }
            }

            GUI.backgroundColor = Color.green;
            EditorGUILayout.BeginHorizontal();

            if (_settings.useWaves) {
                if (LevelSettings.WavesArePaused) {
                    if (GUILayout.Button("Unpause", EditorStyles.miniButton, GUILayout.Width(70))) {
                        LevelSettings.UnpauseWave();
                    }
                } else if (!LevelSettings.IsGameOver) {
                    if (GUILayout.Button("Pause", EditorStyles.miniButton, GUILayout.Width(70))) {
                        LevelSettings.PauseWave();
                    }
                }
            }

            var hasNextWave = LevelSettings.HasNextWave;

            if (!LevelSettings.WavesArePaused && hasNextWave && !LevelSettings.IsGameOver) {
                GUILayout.Space(4);

                if (GUILayout.Button("Next Wave", EditorStyles.miniButton, GUILayout.Width(70))) {
                    LevelSettings.EndWave();
                }
            }
            if (LevelSettings.IsGameOver) {
                GUILayout.Space(4);
                if (GUILayout.Button("Continue game", EditorStyles.miniButton, GUILayout.Width(90))) {
                    LevelSettings.ContinueGame();
                }

                GUILayout.Space(4);
                if (GUILayout.Button("Restart game", EditorStyles.miniButton, GUILayout.Width(80))) {
                    LevelSettings.RestartGame();
                }
            }

            EditorGUILayout.EndHorizontal();

            DTInspectorUtility.AddSpaceForNonU5();

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndVertical();
        }

        GUI.contentColor = DTInspectorUtility.BrightButtonColor;
        if (GUILayout.Button("Collapse All Sections", EditorStyles.toolbarButton, GUILayout.Width(140)))
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Collapse All Sections");
            _settings.killerPoolingExpanded = false;
            _settings.createPrefabPoolsExpanded = false;
            _settings.spawnersExpanded = false;
            _settings.gameStatsExpanded = false;
            _settings.showLevelSettings = false;
            _settings.showCustomEvents = false;
        }
        GUI.contentColor = Color.white;

        DTInspectorUtility.VerticalSpace(4);

        // Pool Boss section

        var state = _settings.killerPoolingExpanded;
        var text = "Pool Boss";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);


        EditorGUI.indentLevel = 0;
        if (state != _settings.killerPoolingExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Pool Boss");
            _settings.killerPoolingExpanded = state;
        }

        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#PoolBoss");

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        var poolingHolder = _settings.transform.GetChildTransform(LevelSettings.KillerPoolingContainerTransName);
        if (poolingHolder == null) {
            Debug.LogError("You have no child prefab of LevelSettings called '" + LevelSettings.KillerPoolingContainerTransName + "'. " + LevelSettings.RevertLevelSettingsAlert);
            return;
        }
        if (_settings.killerPoolingExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            var kp = poolingHolder.GetComponent<PoolBoss>();
            if (kp == null) {
                Debug.LogError("You have no PoolBoss script on your " + LevelSettings.KillerPoolingContainerTransName + " subprefab. " + LevelSettings.RevertLevelSettingsAlert);
                return;
            }

            DTInspectorUtility.ShowColorWarningBox(string.Format("You have {0} Pool Item(s) set up. Click the button below to configure Pooling.", kp.poolItems.Count));

            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            GUILayout.Space(6);
            if (GUILayout.Button("Go to Pool Boss", EditorStyles.toolbarButton, GUILayout.Width(120))) {
                Selection.activeGameObject = poolingHolder.gameObject;
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
            DTInspectorUtility.EndGroupedControls();
        }
        // end Pool Boss section

        // create Prefab Pools section
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.createPrefabPoolsExpanded;
        text = "Prefab Pools";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.createPrefabPoolsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Prefab Pools");
            _settings.createPrefabPoolsExpanded = state;
        }
        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#PrefabPools");

        EditorGUILayout.EndHorizontal();

        if (_settings.createPrefabPoolsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUI.indentLevel = 0;

            // Add expand/collapse buttons if there are items in the list

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            // A little space between button groups
            GUILayout.Space(6);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            DTInspectorUtility.StartGroupHeader();
            EditorGUI.indentLevel = 1;
            var newExp = DTInspectorUtility.Foldout(_settings.newPrefabPoolExpanded, "Create New Prefab Pools");
            if (newExp != _settings.newPrefabPoolExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Create New Prefab Pools");
                _settings.newPrefabPoolExpanded = newExp;
            }
            EditorGUILayout.EndVertical();

            if (_settings.newPrefabPoolExpanded) {
                EditorGUI.indentLevel = 0;
                var newPoolName = EditorGUILayout.TextField("New Pool Name", _settings.newPrefabPoolName);
                if (newPoolName != _settings.newPrefabPoolName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Pool Name");
                    _settings.newPrefabPoolName = newPoolName;
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(6);
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button("Create Prefab Pool", EditorStyles.toolbarButton, GUILayout.MaxWidth(110))) {
                    CreatePrefabPool();
                }
                GUI.contentColor = Color.white;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            DTInspectorUtility.VerticalSpace(2);

            var pools = LevelSettings.GetAllPrefabPools;
            if (pools.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no Prefab Pools.");
            }

            foreach (var pool in pools) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();

                var poolScript = pool.GetComponent<WavePrefabPool>();

                var itemName = pool.name + " (" + poolScript.poolItems.Count + " ";
                itemName += (poolScript.poolItems.Count == 1 ? "item" : "items") + ")";
                GUILayout.Label(itemName);

                GUILayout.FlexibleSpace();

                var buttonPressed = DTInspectorUtility.AddControlButtons("Prefab Pool");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = pool.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (pools.Count > 0) {
                DTInspectorUtility.VerticalSpace(2);
            }

            DTInspectorUtility.EndGroupedControls();
        }
        GUI.color = Color.white;
        // end create prefab pools section

        // create spawners section
        EditorGUI.indentLevel = 0;

        DTInspectorUtility.VerticalSpace(2);
        state = _settings.spawnersExpanded;
        text = "Syncro Spawners";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);
        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#SyncroSpawners");
        EditorGUILayout.EndHorizontal();


        if (state != _settings.spawnersExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Syncro Spawners");
            _settings.spawnersExpanded = state;
        }

        if (_settings.spawnersExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUI.indentLevel = 0;

            // Add expand/collapse buttons if there are items in the list

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            // A little space between button groups
            GUILayout.Space(6);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            // end create spawners section
            GUI.color = Color.white;

            var spawners = LevelSettings.GetAllSpawners;

            if (_settings.useWaves) {
                DTInspectorUtility.StartGroupHeader();
                EditorGUI.indentLevel = 1;
                var newExp = DTInspectorUtility.Foldout(_settings.createSpawnerExpanded, "Create New");
                if (newExp != _settings.createSpawnerExpanded)                 {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand _settings.createSpawnerExpanded");
                    _settings.createSpawnerExpanded = newExp;
                }
                EditorGUILayout.EndVertical();

                if (_settings.createSpawnerExpanded)                 {
                    EditorGUI.indentLevel = 0;
                    var newName = EditorGUILayout.TextField("New Spawner Name", _settings.newSpawnerName);
                    if (newName != _settings.newSpawnerName)                     {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Spawner Name");
                        _settings.newSpawnerName = newName;
                    }

                    var newType = (LevelSettings.SpawnerType)EditorGUILayout.EnumPopup("New Spawner Color", _settings.newSpawnerType);
                    if (newType != _settings.newSpawnerType)                     {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Spawner Color");
                        _settings.newSpawnerType = newType;
                    }

                    EditorGUILayout.BeginHorizontal(EditorStyles.boldLabel);
                    GUILayout.Space(6);
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    if (GUILayout.Button("Create Spawner", EditorStyles.toolbarButton, GUILayout.MaxWidth(110)))                     {
                        CreateSpawner();
                    }
                    GUI.contentColor = Color.white;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                DTInspectorUtility.VerticalSpace(2);
            }

            if (!_settings.useWaves)             {
                DTInspectorUtility.ShowLargeBarAlertBox("Syncro Spawners disabled. Check 'Use Global Waves' up top to use.");
            }
            else if (spawners.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no Syncro Spawners.");
            }

            GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
            foreach (var spawner in spawners) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(spawner.name);
                GUILayout.FlexibleSpace();
                var buttonPressed = DTInspectorUtility.AddControlButtons("Spawner");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = spawner.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.white;

            DTInspectorUtility.EndGroupedControls();
        }

        GUI.color = Color.white;


        // Player stats
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.gameStatsExpanded;
        text = "World Variables";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.gameStatsExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle World Variables");
            _settings.gameStatsExpanded = state;
        }

        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#WorldVariables");

        EditorGUILayout.EndHorizontal();

        if (_settings.gameStatsExpanded) {
            DTInspectorUtility.BeginGroupedControls();
            // BUTTONS...
            GUI.color = Color.white;

            var variables = LevelSettings.GetAllWorldVariables;
            if (variables.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You currently have no World Variables.");
            }

            foreach (var worldVar in variables) {
                DTInspectorUtility.StartGroupHeader(1, false);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(worldVar.name);

                GUILayout.FlexibleSpace();

                var variable = worldVar.GetComponent<WorldVariable>();
                GUI.contentColor = DTInspectorUtility.BrightTextColor;
                GUILayout.Label(WorldVariableTracker.GetVariableTypeFriendlyString(variable.varType));
                GUI.contentColor = Color.white;

                var buttonPressed = DTInspectorUtility.AddControlButtons("World Variable");
                if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                    Selection.activeGameObject = worldVar.gameObject;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            GUI.backgroundColor = Color.white;

            DTInspectorUtility.VerticalSpace(3);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6);
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("World Variable Panel", EditorStyles.toolbarButton, GUILayout.MaxWidth(130))) {
                Selection.objects = new Object[] {
                    playerStatsHolder.gameObject
                };
                return;
            }
            GUI.contentColor = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            DTInspectorUtility.EndGroupedControls();
        }
        // end Player  stats
        GUI.color = Color.white;

        _settings._frames++;
        _isDirty = true;

        // level waves
        DTInspectorUtility.VerticalSpace(2);
        state = _settings.showLevelSettings;
        text = "Levels & Waves";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.showLevelSettings) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Level Waves");
            _settings.showLevelSettings = state;
        }

        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#LevelsWaves");

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_settings.showLevelSettings) {
            if (_settings.useWaves) {
                DTInspectorUtility.BeginGroupedControls();
                EditorGUI.indentLevel = 0;  // Space will handle this for the header

                DTInspectorUtility.StartGroupHeader(0);
                var newShow = GUILayout.Toggle(_settings.showCustomWaveClasses, " Show Custom Wave Classes");
                if (newShow != _settings.showCustomWaveClasses) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Show Custom Wave Classes");
                    _settings.showCustomWaveClasses = newShow;
                }

                EditorGUILayout.EndVertical();
                if (_settings.showCustomWaveClasses) {
                    if (_settings.customWaveClasses.Count == 0) {
                        DTInspectorUtility.ShowLargeBarAlertBox("You have no Custom Wave Classes set up.");
                    }

                    int? classToDelete = null;

                    for (var i = 0; i < _settings.customWaveClasses.Count; i++) {
                        var waveClass = _settings.customWaveClasses[i];

                        DTInspectorUtility.StartGroupHeader(1, false);
                        EditorGUILayout.BeginHorizontal();

                        var newName = EditorGUILayout.TextField("Wave Class", waveClass);
                        if (newName != waveClass) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Rename Custom Wave Class");
                            _settings.customWaveClasses[i] = newName;
                        }

                        var oldBG = GUI.backgroundColor;
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Click to delete Custom Wave Class"), EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                            classToDelete = i;
                        }
                        GUI.backgroundColor = oldBG;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }

                    if (classToDelete.HasValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Delete Custom Wave Class");
                        _settings.customWaveClasses.RemoveAt(classToDelete.Value);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    GUILayout.Space(6);
                    if (GUILayout.Button(new GUIContent("Add", "New Custom Wave Class"), EditorStyles.toolbarButton, GUILayout.Width(32))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add New Custom Wave Class");
                        _settings.customWaveClasses.Add("New Wave Class (rename)");
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (_settings.LevelTimes.Count > 0) {
                    var newRepeat = (LevelSettings.LevelLoopMode)EditorGUILayout.EnumPopup("Last Level Completed", _settings.repeatLevelMode);
                    if (newRepeat != _settings.repeatLevelMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Last Level Completed");
                        _settings.repeatLevelMode = newRepeat;
                    }
                }

                var useWaveNameFilter = EditorGUILayout.Toggle("Use Wave Name Filter", _settings.useWaveNameFilter);
                if (useWaveNameFilter != _settings.useWaveNameFilter) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Wave Name Filter");
                    _settings.useWaveNameFilter = useWaveNameFilter;
                }
                if (_settings.useWaveNameFilter) {
                    EditorGUI.indentLevel = 1;
                    var newFilter = EditorGUILayout.TextField("Wave Name Filter", _settings.waveNameFilterText);
                    if (newFilter != _settings.waveNameFilterText) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Name Filter");
                        _settings.waveNameFilterText = newFilter;
                    }
                }
                EditorGUI.indentLevel = 0;

                DTInspectorUtility.VerticalSpace(2);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Level Wave Settings", EditorStyles.boldLabel);

                // BUTTONS...
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

                // Add expand/collapse buttons if there are items in the list
                if (_settings.LevelTimes.Count > 0) {
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    const string collapseIcon = "Collapse";
                    var content = new GUIContent(collapseIcon, "Click to collapse all");
                    var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton);

                    const string expandIcon = "Expand";
                    content = new GUIContent(expandIcon, "Click to expand all");
                    var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton);
                    if (masterExpand) {
                        ExpandCollapseAll(true);
                    }
                    if (masterCollapse) {
                        ExpandCollapseAll(false);
                    }
                    GUI.contentColor = Color.white;
                } else {
                    GUILayout.FlexibleSpace();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();

                // ReSharper disable TooWideLocalVariableScope
                // ReSharper disable RedundantAssignment
                var levelButtonPressed = DTInspectorUtility.FunctionButtons.None;
                var waveButtonPressed = DTInspectorUtility.FunctionButtons.None;
                // ReSharper restore RedundantAssignment
                // ReSharper restore TooWideLocalVariableScope

                EditorGUI.indentLevel = 0;

                if (_settings.LevelTimes.Count == 0) {
                    DTInspectorUtility.ShowColorWarningBox("You have no Levels set up.");

                    EditorGUILayout.BeginHorizontal(GUILayout.Width(50));
                    GUILayout.Space(4);
                    var addText = string.Format("Click to add level{0}.", _settings.LevelTimes.Count > 0 ? " at the end" : "");

                    // Main Add button
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    if (GUILayout.Button(new GUIContent("Add", addText), EditorStyles.toolbarButton)) {
                        _isDirty = true;
                        CreateNewLevelAfter();
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }

                var levelToDelete = -1;
                var levelToInsertAt = -1;
                int? levelToShiftUp = null;
                int? levelToShiftDown = null;
                int? levelToCopy = null;
                var waveToInsertAt = -1;
                var waveToDelete = -1;
                int? waveToShiftUp = null;
                int? waveToShiftDown = null;
                int? waveToCopy = null;

                var needsDurationUpgrade = false;
                for (var l = 0; l < _settings.LevelTimes.Count; l++) {
                    var levelSetting = _settings.LevelTimes[l];
                    for (var w = 0; w < levelSetting.WaveSettings.Count; w++) {
                        var wave = levelSetting.WaveSettings[w];
                        if (wave.waveType == LevelSettings.WaveType.Timed && wave.WaveDuration != wave.waveDurationFlex.Value
                            && wave.WaveDuration != LevelSettings.DefaultWaveDuration && !_settings.waveDurationsCopied) {

                            needsDurationUpgrade = true;
                            break;
                        }
                    }
                }
                if (needsDurationUpgrade) {
                    DTInspectorUtility.ShowLargeBarAlertBox("It appears you may need to copy the Duration from the old setting. Click the button below to do so.");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(4);
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button(new GUIContent("Copy Durations From Old Setting", "Click this to copy the now invisible old field for duration into this new field."), EditorStyles.toolbarButton, GUILayout.Width(190))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Copy Durations From Old Setting");

                        for (var l = 0; l < _settings.LevelTimes.Count; l++) {
                            var levelSetting = _settings.LevelTimes[l];
                            for (var w = 0; w < levelSetting.WaveSettings.Count; w++) {
                                var wave = levelSetting.WaveSettings[w];
                                wave.waveDurationFlex.selfValue = wave.WaveDuration;
                            }
                        }
                        _settings.waveDurationsCopied = true;
                    }
                    GUI.contentColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }

                for (var l = 0; l < _settings.LevelTimes.Count; l++) {
                    EditorGUI.indentLevel = 0;
                    var levelSetting = _settings.LevelTimes[l];

                    DTInspectorUtility.StartGroupHeader();
                    EditorGUILayout.BeginHorizontal();
                    // Display foldout with current state
                    EditorGUI.indentLevel = 1;
                    var levelDisplayName = string.Format("Level {0} Waves & Settings", (l + 1));
                    if (!levelSetting.isExpanded && !string.IsNullOrEmpty(levelSetting.levelName)) {
                        levelDisplayName += " (" + levelSetting.levelName + ")";
                    }

                    state = DTInspectorUtility.Foldout(levelSetting.isExpanded, levelDisplayName);
                    if (state != levelSetting.isExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Level Waves & Settings");
                        levelSetting.isExpanded = state;
                    }

                    GUILayout.FlexibleSpace();


                    levelButtonPressed = DTInspectorUtility.AddFoldOutListItemButtons(l, _settings.LevelTimes.Count, "level", true, "Click to show all prefabs spawned in this Level", true, true, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel = 0;

                    if (levelSetting.isExpanded) {
                        var newName = EditorGUILayout.TextField("Level Name", levelSetting.levelName);
                        if (newName != levelSetting.levelName) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Level Name");
                            levelSetting.levelName = newName;
                        }

                        var newOrder = (LevelSettings.WaveOrder)EditorGUILayout.EnumPopup("Wave Sequence", levelSetting.waveOrder);
                        if (newOrder != levelSetting.waveOrder) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Sequence");
                            levelSetting.waveOrder = newOrder;
                        }

                        var allWaveCount = levelSetting.WaveSettings.Count;
                        var filteredWaves = new List<LevelWave>(allWaveCount);
                        if (_settings.useWaveNameFilter && !string.IsNullOrEmpty(_settings.waveNameFilterText)) {
                            filteredWaves.AddRange(levelSetting.WaveSettings.FindAll(delegate (LevelWave lev) {
                                return lev.waveName.ToLower().Contains(_settings.waveNameFilterText);
                            }));
                        } else {
                            filteredWaves.AddRange(levelSetting.WaveSettings);
                        }
                        var filteredOut = allWaveCount - filteredWaves.Count;

                        if (filteredOut > 0) {
                            DTInspectorUtility.ShowColorWarningBox(filteredOut + " waves filtered out with Wave Name Filter (above).");
                        }
                        for (var w = 0; w < filteredWaves.Count; w++) {
                            var showVisualize = false;

                            var waveSetting = filteredWaves[w];

                            DTInspectorUtility.StartGroupHeader(1);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUI.indentLevel = 1;

                            var waveDisplayName = "Wave " + (w + 1);
                            if (!waveSetting.isExpanded && !string.IsNullOrEmpty(waveSetting.waveName)) {
                                waveDisplayName += " (" + waveSetting.waveName + ")";
                            }
                            // Display foldout with current state
                            var innerExpanded = DTInspectorUtility.Foldout(waveSetting.isExpanded, waveDisplayName);
                            if (innerExpanded != waveSetting.isExpanded) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Wave");
                                waveSetting.isExpanded = innerExpanded;
                            }

                            if (GUILayout.Button(new GUIContent("Visualize", "Visualize Waves of All Spawners"),
                                EditorStyles.toolbarButton, GUILayout.Width(64))) {
                                showVisualize = true;
                            }

                            waveButtonPressed = DTInspectorUtility.AddFoldOutListItemButtons(w, levelSetting.WaveSettings.Count, "wave", true, "Click to show all prefabs spawned in this Wave", true, true, true);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();

                            if (waveSetting.isExpanded) {
                                EditorGUI.indentLevel = 0;
                                if (waveSetting.skipWaveType == LevelSettings.SkipWaveMode.Always) {
                                    DTInspectorUtility.ShowColorWarningBox("This wave is set to be skipped.");
                                }

                                if (string.IsNullOrEmpty(waveSetting.waveName)) {
                                    waveSetting.waveName = "UNNAMED";
                                }

                                if (_settings.showCustomWaveClasses) {
                                    if (_settings.customWaveClasses.Count == 0) {
                                        DTInspectorUtility.ShowLargeBarAlertBox("Set up some Custom Wave Classes above first.");
                                    } else {
                                        var waveClassIndex = _settings.customWaveClasses.IndexOf(waveSetting.waveClass);
                                        if (waveClassIndex < 0) {
                                            waveClassIndex = 0;
                                            _isDirty = true;
                                            waveSetting.waveClass = _settings.customWaveClasses[0];
                                        }

                                        var newClassIndex = EditorGUILayout.Popup("Custom Wave Class", waveClassIndex, _settings.customWaveClasses.ToArray());
                                        if (newClassIndex != waveClassIndex) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Wave Class");
                                            waveSetting.waveClass = _settings.customWaveClasses[newClassIndex];
                                        }
                                    }
                                }

                                var newWaveName = EditorGUILayout.TextField("Wave Name", waveSetting.waveName);
                                if (newWaveName != waveSetting.waveName) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Name");
                                    waveSetting.waveName = newWaveName;
                                }

                                var newWaveDesc = EditorGUILayout.TextField("Wave Description", waveSetting.waveDescription);
                                if (newWaveDesc != waveSetting.waveDescription) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Description");
                                    waveSetting.waveDescription = newWaveDesc;
                                }

                                var newWaveType = (LevelSettings.WaveType)EditorGUILayout.EnumPopup("Wave Type", waveSetting.waveType);
                                if (newWaveType != waveSetting.waveType) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Type");
                                    waveSetting.waveType = newWaveType;
                                }

                                if (waveSetting.waveType == LevelSettings.WaveType.Timed) {
                                    KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.waveDurationFlex, "Duration (sec)", _settings);

                                    var newEnd = EditorGUILayout.Toggle("End When All Destroyed", waveSetting.endEarlyIfAllDestroyed);
                                    if (newEnd != waveSetting.endEarlyIfAllDestroyed) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle End Early When All Destroyed");
                                        waveSetting.endEarlyIfAllDestroyed = newEnd;
                                    }
                                }

                                switch (waveSetting.skipWaveType) {
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueAbove:
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueBelow:
                                        EditorGUILayout.Separator();
                                        break;
                                }

                                var newSkipType = (LevelSettings.SkipWaveMode)EditorGUILayout.EnumPopup("Skip Wave Type", waveSetting.skipWaveType);
                                if (newSkipType != waveSetting.skipWaveType) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Skip Wave Type");
                                    waveSetting.skipWaveType = newSkipType;
                                }

                                switch (waveSetting.skipWaveType) {
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueAbove:
                                    case LevelSettings.SkipWaveMode.IfWorldVariableValueBelow:
                                        var missingStatNames = new List<string>();
                                        missingStatNames.AddRange(allStats);
                                        missingStatNames.RemoveAll(delegate (string obj) {
                                            return waveSetting.skipWavePassCriteria.HasKey(obj);
                                        });

                                        var newStat = EditorGUILayout.Popup("Add Skip Wave Limit", 0, missingStatNames.ToArray());
                                        if (newStat != 0) {
                                            AddWaveSkipLimit(missingStatNames[newStat], waveSetting);
                                        }

                                        if (waveSetting.skipWavePassCriteria.statMods.Count == 0) {
                                            DTInspectorUtility.ShowRedErrorBox("You have no Skip Wave Limits. Wave will never be skipped.");
                                        } else {
                                            EditorGUILayout.Separator();

                                            int? indexToDelete = null;

                                            for (var i = 0; i < waveSetting.skipWavePassCriteria.statMods.Count; i++) {
                                                var modifier = waveSetting.skipWavePassCriteria.statMods[i];

                                                var buttonPressed = DTInspectorUtility.FunctionButtons.None;

                                                switch (modifier._varTypeToUse) {
                                                    case WorldVariableTracker.VariableType._integer:
                                                        buttonPressed = KillerVariablesHelper.DisplayKillerIntLimit(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
                                                        break;
                                                    case WorldVariableTracker.VariableType._float:
                                                        buttonPressed = KillerVariablesHelper.DisplayKillerFloatLimit(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _settings, true, true);
                                                        break;
                                                    default:
                                                        Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                                        break;
                                                }

                                                KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                                                if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
                                                    indexToDelete = i;
                                                }
                                            }

                                            DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                                            if (indexToDelete.HasValue) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Skip Wave Limit");
                                                waveSetting.skipWavePassCriteria.DeleteByIndex(indexToDelete.Value);
                                            }

                                            EditorGUILayout.Separator();
                                        }

                                        break;
                                }

                                if (_settings.useMusicSettings) {
                                    if (l > 0 || w > 0) {
                                        var newMusicMode = (LevelSettings.WaveMusicMode)EditorGUILayout.EnumPopup("Music Mode", waveSetting.musicSettings.WaveMusicMode);
                                        if (newMusicMode != waveSetting.musicSettings.WaveMusicMode) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Music Mode");
                                            waveSetting.musicSettings.WaveMusicMode = newMusicMode;
                                        }
                                    }

                                    if (waveSetting.musicSettings.WaveMusicMode == LevelSettings.WaveMusicMode.PlayNew) {
                                        var newWavMusic = (AudioClip)EditorGUILayout.ObjectField("Music", waveSetting.musicSettings.WaveMusic, typeof(AudioClip), true);
                                        if (newWavMusic != waveSetting.musicSettings.WaveMusic) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Music");
                                            waveSetting.musicSettings.WaveMusic = newWavMusic;
                                        }
                                    }
                                    if (waveSetting.musicSettings.WaveMusicMode != LevelSettings.WaveMusicMode.Silence) {
                                        var newVol = EditorGUILayout.Slider("Music Volume", waveSetting.musicSettings.WaveMusicVolume, 0f, 1f);
                                        if (newVol != waveSetting.musicSettings.WaveMusicVolume) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Music Volume");
                                            waveSetting.musicSettings.WaveMusicVolume = newVol;
                                        }
                                    } else {
                                        var newFadeTime = EditorGUILayout.Slider("Silence Fade Time", waveSetting.musicSettings.FadeTime, 0f, 15f);
                                        if (newFadeTime != waveSetting.musicSettings.FadeTime) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Silence Fade Time");
                                            waveSetting.musicSettings.FadeTime = newFadeTime;
                                        }
                                    }
                                }

                                DTInspectorUtility.VerticalSpace(2);
                                DTInspectorUtility.StartGroupHeader(0, false);
                                var spawnersUsed = FindMatchingSpawners(l, w);

                                var unusedSpawners = LevelSettings.GetAllSpawnerScripts;
                                unusedSpawners.RemoveAll(
                                    delegate (WaveSyncroPrefabSpawner spawner) {
                                        return spawnersUsed.Contains(spawner);
                                    });
                                var unusedSpawnerNames = new List<string>(unusedSpawners.Count + 1);
                                unusedSpawnerNames.Add("-None-");

                                foreach (var spawner in unusedSpawners) {
                                    unusedSpawnerNames.Add(spawner.name);
                                }

                                if (!Application.isPlaying) {
                                    GUI.backgroundColor = DTInspectorUtility.SecondaryGroupBoxColor;
                                    var newSpawnerIndex = EditorGUILayout.Popup("Add Wave For Spawner", 0, unusedSpawnerNames.ToArray());
                                    if (newSpawnerIndex > 0) {
                                        var newWaveSpawner = unusedSpawners[newSpawnerIndex - 1];
                                        var isDirty = true;
                                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, newWaveSpawner, "Add Wave For Spawner");

                                        newWaveSpawner.waveSpecs.Add(new WaveSpecifics {
                                            SpawnLevelNumber = l,
                                            SpawnWaveNumber = w
                                        });
                                    }
                                }

                                if (spawnersUsed.Count == 0) {
                                    DTInspectorUtility.ShowLargeBarAlertBox("You have no Spawners set up for this Wave.");
                                } else {
                                    GUI.contentColor = DTInspectorUtility.BrightTextColor;
                                    GUILayout.Label("Spawners: " + spawnersUsed.Count, EditorStyles.boldLabel);
                                    GUI.contentColor = Color.white;
                                }


                                int? spawnerWaveIndexToDelete = null;

                                var s = 0;
                                foreach (var spawner in spawnersUsed) {
                                    DTInspectorUtility.StartGroupHeader(1, false);
                                    EditorGUILayout.BeginHorizontal();
                                    GUILayout.Label(spawner.name);
                                    GUILayout.FlexibleSpace();

                                    var buttonPressed = DTInspectorUtility.AddControlButtons("Spawner");
                                    if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                                        Selection.activeGameObject = spawner.gameObject;
                                    }

                                    if (!Application.isPlaying) {
                                        var oldBg = GUI.backgroundColor;
                                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                                        if (GUILayout.Button(new GUIContent("Delete", "Click to Demove Spawner Wave"),
                                            EditorStyles.miniButton, GUILayout.Width(45))) {
                                            spawnerWaveIndexToDelete = s;
                                        }
                                        GUI.backgroundColor = oldBg;
                                    }

                                    EditorGUILayout.EndHorizontal();
                                    EditorGUILayout.EndVertical();
                                    s++;
                                }

                                if (spawnerWaveIndexToDelete.HasValue) {
                                    var spawnerName = spawnersUsed[spawnerWaveIndexToDelete.Value].name;
                                    var matchingSpawner = LevelSettings.GetAllSpawners.Find(delegate (Transform spawnerTrans) {
                                        return spawnerTrans.name == spawnerName;
                                    });

                                    if (matchingSpawner != null) {
                                        var spawner = matchingSpawner.GetComponent<WaveSyncroPrefabSpawner>();
                                        var waveToKill = spawner.waveSpecs.Find(delegate (WaveSpecifics wave) {
                                            return wave.SpawnLevelNumber == l && wave.SpawnWaveNumber == w;
                                        });

                                        if (waveToKill != null) {
                                            var dirty = true;
                                            UndoHelper.RecordObjectPropertyForUndo(ref dirty, spawner, "Delete Spawner Wave");

                                            spawner.waveSpecs.Remove(waveToKill);
                                        }
                                    }
                                }

                                if (spawnersUsed.Count > 1) {
                                    var newUsing = (LevelSettings.WaveSpawnerUseMode)EditorGUILayout.EnumPopup("Spawners To Use", waveSetting.spawnerUseMode);
                                    if (newUsing != waveSetting.spawnerUseMode) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawners To Use");
                                        waveSetting.spawnerUseMode = newUsing;
                                    }

                                    if (waveSetting.spawnerUseMode == LevelSettings.WaveSpawnerUseMode.RandomSubset) {
                                        var newMin = EditorGUILayout.IntSlider("Use Spawners Min", waveSetting.spawnersToUseMin, 1, spawnersUsed.Count);
                                        if (newMin != waveSetting.spawnersToUseMin) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Use Spawners Min");
                                            waveSetting.spawnersToUseMin = newMin;
                                        }

                                        var newMax = EditorGUILayout.IntSlider("Use Spawners Max", waveSetting.spawnersToUseMax, 1, spawnersUsed.Count);
                                        if (newMax != waveSetting.spawnersToUseMax) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Use Spawners Max");
                                            waveSetting.spawnersToUseMax = newMax;
                                        }

                                        if (waveSetting.spawnersToUseMin > waveSetting.spawnersToUseMax) {
                                            _isDirty = true;
                                            waveSetting.spawnersToUseMax = waveSetting.spawnersToUseMin;
                                        }

                                        if (waveSetting.spawnersToUseMax < waveSetting.spawnersToUseMin) {
                                            _isDirty = true;
                                            waveSetting.spawnersToUseMin = waveSetting.spawnersToUseMax;
                                        }
                                    }
                                }

                                EditorGUILayout.EndVertical();
                                DTInspectorUtility.AddSpaceForNonU5();

                                if (waveSetting.waveType == LevelSettings.WaveType.Elimination) {
                                    DTInspectorUtility.StartGroupHeader(0, false);

                                    var newTrig = EditorGUILayout.BeginToggleGroup(" Use Triggered Spawners",
                                        waveSetting.useTriggeredSpawners);
                                    if (newTrig != waveSetting.useTriggeredSpawners) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                            "toggle Use Triggered Spawners");
                                        waveSetting.useTriggeredSpawners = newTrig;
                                    }

                                    if (waveSetting.useTriggeredSpawners) {
                                        DTInspectorUtility.BeginGroupedControls();

                                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(4);
                                        if (GUILayout.Button(new GUIContent("Add", "Click to add a Triggered Spawner"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Triggered Spawner");
                                            waveSetting.trigSpawnerWavesToAwait.Add(new TrigSpawnerWaveWaiter());
                                        }
                                        GUI.contentColor = Color.white;
                                        EditorGUILayout.EndHorizontal();

                                        if (waveSetting.trigSpawnerWavesToAwait.Count == 0) {
                                            DTInspectorUtility.ShowColorWarningBox("You have no Triggered Spawners added here.");
                                        } else {
                                            EditorGUILayout.LabelField("Triggered Spawners: " + waveSetting.trigSpawnerWavesToAwait.Count, EditorStyles.boldLabel);

                                            int? spawnerIndexToDelete = null;

                                            for (var t = 0; t < waveSetting.trigSpawnerWavesToAwait.Count; t++) {
                                                var aSpawn = waveSetting.trigSpawnerWavesToAwait[t];
                                                DTInspectorUtility.StartGroupHeader(1, false);
                                                EditorGUILayout.BeginHorizontal();
                                                var newSpawnerScript = (TriggeredSpawnerV2)EditorGUILayout.ObjectField("", aSpawn.TrigSpawner, typeof(TriggeredSpawnerV2), true, GUILayout.Width(100));
                                                if (newSpawnerScript != aSpawn.TrigSpawner) {
                                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawner G.O.");
                                                    aSpawn.TrigSpawner = newSpawnerScript;
                                                }

                                                var isInvalidEvent = false;
                                                if (aSpawn.TrigSpawner != null) {
                                                    var activeWaves = aSpawn.TrigSpawner.ActiveEliminationWaves;
                                                    var waveEventTypes = new List<TriggeredSpawner.EventType>(activeWaves.Count);
                                                    var waveNames = new List<string>(activeWaves.Count);
                                                    var customEventNames = new List<string>(activeWaves.Count);

                                                    foreach (var waiter in activeWaves) {
                                                        waveEventTypes.Add(waiter.EventType);
                                                        waveNames.Add(GetFriendlyEventName(waiter.EventType, waiter.Wave));
                                                        customEventNames.Add(waiter.Wave.customEventName);
                                                    }

                                                    var existingIndex = -1;
                                                    if (string.IsNullOrEmpty(aSpawn.CustomEventName)) {
                                                        existingIndex = waveEventTypes.IndexOf(aSpawn.EventType);
                                                    } else {
                                                        var customEventMatchIndex = customEventNames.IndexOf(aSpawn.CustomEventName);
                                                        existingIndex = customEventMatchIndex;
                                                    }

                                                    if (activeWaves.Count > 0) {
                                                        EditorGUILayout.LabelField("Event", GUILayout.Width(38));

                                                        var newEventIndex = EditorGUILayout.Popup("", existingIndex,
                                                            waveNames.ToArray(), GUILayout.Width(90));
                                                        if (newEventIndex != existingIndex) {
                                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawner Event Type");

                                                            aSpawn.EventType = waveEventTypes[newEventIndex];
                                                            aSpawn.CustomEventName = customEventNames[newEventIndex];
                                                        }
                                                    } else {
                                                        EditorGUILayout.LabelField("No Events Used", EditorStyles.boldLabel, GUILayout.Width(110));
                                                        isInvalidEvent = true;
                                                    }

                                                    GUILayout.FlexibleSpace();
                                                    var settingsIcon = new GUIContent(CoreGameKitInspectorResources.SettingsTexture, "Click to go to Spawner");
                                                    if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton,
                                                        GUILayout.Width(24), GUILayout.Height(16))) {
                                                        Selection.activeObject = aSpawn.TrigSpawner.transform;
                                                    }
                                                } else {
                                                    GUILayout.FlexibleSpace();
                                                }

                                                var oldBG = GUI.backgroundColor;
                                                GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                                                if (GUILayout.Button(new GUIContent("Delete", "Click to delete Triggered Spawner"), EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                                                    spawnerIndexToDelete = t;
                                                }
                                                GUI.backgroundColor = oldBG;

                                                EditorGUILayout.EndHorizontal();
                                                if (aSpawn.TrigSpawner == null) {
                                                    DTInspectorUtility.ShowRedErrorBox("No Spawner Selected. Invalid.");
                                                } else if (isInvalidEvent) {
                                                    DTInspectorUtility.ShowRedErrorBox("No Event Selected. Invalid.");
                                                }

                                                EditorGUILayout.EndVertical();
                                            }

                                            if (spawnerIndexToDelete.HasValue) {
                                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Delete Triggered Spawner");
                                                waveSetting.trigSpawnerWavesToAwait.RemoveAt(spawnerIndexToDelete.Value);
                                            }
                                        }

                                        DTInspectorUtility.EndGroupedControls();
                                    }

                                    EditorGUILayout.EndToggleGroup();
                                    EditorGUILayout.EndVertical();
                                }

                                DTInspectorUtility.VerticalSpace(2);
                                EditorGUILayout.LabelField("Wave Completed Options", EditorStyles.boldLabel);

                                var newPause = EditorGUILayout.Toggle("Pause Global Waves", waveSetting.pauseGlobalWavesWhenCompleted);
                                if (newPause != waveSetting.pauseGlobalWavesWhenCompleted) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Pause Global Waves");
                                    waveSetting.pauseGlobalWavesWhenCompleted = newPause;
                                }

                                if (waveSetting.waveType == LevelSettings.WaveType.Elimination) {
                                    DTInspectorUtility.AddSpaceForNonU5(2);
                                    DTInspectorUtility.StartGroupHeader(0, false);
                                    EditorGUI.indentLevel = 0;
                                    // beat level Custom Events to fire
                                    var newLastSpawn = EditorGUILayout.BeginToggleGroup(" Wave Elimination Bonus Prefab",
                                        waveSetting.useSpawnBonusPrefab);
                                    if (newLastSpawn != waveSetting.useSpawnBonusPrefab) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Elimination Bonus Prefab");
                                        waveSetting.useSpawnBonusPrefab = newLastSpawn;
                                    }
                                    EditorGUILayout.EndVertical();

                                    if (waveSetting.useSpawnBonusPrefab) {
                                        DTInspectorUtility.BeginGroupedControls();

                                        var newBonusSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Bonus Prefab Type", waveSetting.bonusPrefabSource);
                                        if (newBonusSource != waveSetting.bonusPrefabSource) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Bonus Prefab Type");
                                            waveSetting.bonusPrefabSource = newBonusSource;
                                        }

                                        var hasBonusPrefab = true;
                                        switch (waveSetting.bonusPrefabSource) {
                                            case WaveSpecifics.SpawnOrigin.PrefabPool:
                                                if (poolNames != null) {
                                                    var pool = LevelSettings.GetFirstMatchingPrefabPool(waveSetting.bonusPrefabPoolName);
                                                    var noDeathPool = false;
                                                    var illegalDeathPref = false;
                                                    var noPrefabPools = false;

                                                    if (pool == null) {
                                                        if (string.IsNullOrEmpty(waveSetting.bonusPrefabPoolName)) {
                                                            noDeathPool = true;
                                                        } else {
                                                            illegalDeathPref = true;
                                                        }
                                                        waveSetting.bonusPrefabPoolIndex = 0;
                                                    } else {
                                                        waveSetting.bonusPrefabPoolIndex = poolNames.IndexOf(waveSetting.bonusPrefabPoolName);
                                                    }

                                                    if (poolNames.Count > 1) {
                                                        EditorGUILayout.BeginHorizontal();
                                                        var newDeathPool = EditorGUILayout.Popup("Bonus Prefab Pool", waveSetting.bonusPrefabPoolIndex, poolNames.ToArray());
                                                        if (newDeathPool != waveSetting.bonusPrefabPoolIndex) {
                                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Bonus Prefab Pool");
                                                            waveSetting.bonusPrefabPoolIndex = newDeathPool;
                                                        }

                                                        if (waveSetting.bonusPrefabPoolIndex > 0) {
                                                            var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[waveSetting.bonusPrefabPoolIndex]);
                                                            if (matchingPool != null) {
                                                                waveSetting.bonusPrefabPoolName = matchingPool.name;
                                                            }
                                                        } else {
                                                            waveSetting.bonusPrefabPoolName = string.Empty;
                                                        }

                                                        if (newDeathPool > 0) {
                                                            if (DTInspectorUtility.AddControlButtons("Prefab Pool") == DTInspectorUtility.FunctionButtons.Edit) {
                                                                Selection.activeGameObject = pool.gameObject;
                                                            }
                                                        }
                                                        EditorGUILayout.EndHorizontal();

                                                    } else {
                                                        noPrefabPools = true;
                                                    }

                                                    if (noPrefabPools) {
                                                        DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                                        hasBonusPrefab = false;
                                                    } else if (noDeathPool) {
                                                        DTInspectorUtility.ShowRedErrorBox("No Bonus Prefab Pool selected.");
                                                        hasBonusPrefab = false;
                                                    } else if (illegalDeathPref) {
                                                        DTInspectorUtility.ShowRedErrorBox("Bonus Prefab Pool '" + waveSetting.bonusPrefabPoolName + "' not found. Select one.");
                                                        hasBonusPrefab = false;
                                                    }
                                                } else {
                                                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                                                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                                                    hasBonusPrefab = false;
                                                }
                                                break;
                                            case WaveSpecifics.SpawnOrigin.Specific:
                                                PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _settings, ref waveSetting.bonusPrefabSpecific, ref waveSetting.bonusPrefabCategoryName, "Bonus Prefab");

                                                if (waveSetting.bonusPrefabSpecific == null) {
                                                    DTInspectorUtility.ShowColorWarningBox("You have no Bonus prefab assigned. Nothing will spawn when this wave is completed.");
                                                    hasBonusPrefab = false;
                                                }

                                                break;
                                        }

                                        if (hasBonusPrefab) {
                                            KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.bonusPrefabSpawnPercent, "Spawn % Chance", _settings);

                                            KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.bonusPrefabQty, "Spawn Quantity", _settings);
                                        }

                                        DTInspectorUtility.EndGroupedControls();
                                    }
                                    EditorGUILayout.EndToggleGroup();
                                }

                                DTInspectorUtility.AddSpaceForNonU5(2);
                                DTInspectorUtility.StartGroupHeader(0, false);
                                // beat level variable modifiers
                                var newBonusesEnabled = EditorGUILayout.BeginToggleGroup(" Wave Completion Bonus", waveSetting.waveBeatBonusesEnabled);
                                if (newBonusesEnabled != waveSetting.waveBeatBonusesEnabled) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Completion Bonus");
                                    waveSetting.waveBeatBonusesEnabled = newBonusesEnabled;
                                }
                                EditorGUILayout.EndVertical();

                                if (waveSetting.waveBeatBonusesEnabled) {
                                    DTInspectorUtility.BeginGroupedControls();
                                    EditorGUI.indentLevel = 1;

                                    var missingBonusStatNames = new List<string>();
                                    missingBonusStatNames.AddRange(allStats);
                                    missingBonusStatNames.RemoveAll(delegate (string obj) {
                                        return
                                            waveSetting
                                                .waveDefeatVariableModifiers
                                                .HasKey(obj);
                                    });

                                    var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0,
                                        missingBonusStatNames.ToArray());
                                    if (newBonusStat != 0) {
                                        AddBonusStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
                                    }

                                    if (waveSetting.waveDefeatVariableModifiers.statMods.Count == 0) {
                                        if (waveSetting.waveBeatBonusesEnabled) {
                                            DTInspectorUtility.ShowColorWarningBox(
                                                "You currently are using no modifiers for this wave.");
                                        }
                                    } else {
                                        EditorGUILayout.Separator();

                                        int? indexToDelete = null;

                                        for (var i = 0; i < waveSetting.waveDefeatVariableModifiers.statMods.Count; i++) {
                                            var modifier = waveSetting.waveDefeatVariableModifiers.statMods[i];

                                            var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                                            switch (modifier._varTypeToUse) {
                                                case WorldVariableTracker.VariableType._integer:
                                                    buttonPressed = KillerVariablesHelper.DisplayKillerInt(
                                                        ref _isDirty, modifier._modValueIntAmt, modifier._statName,
                                                        _settings, true, true);
                                                    break;
                                                case WorldVariableTracker.VariableType._float:
                                                    buttonPressed =
                                                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty,
                                                            modifier._modValueFloatAmt, modifier._statName, _settings,
                                                            true, true);
                                                    break;
                                                default:
                                                    Debug.LogError("Add code for varType: " +
                                                                   modifier._varTypeToUse.ToString());
                                                    break;
                                            }

                                            KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                                            if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
                                                indexToDelete = i;
                                            }
                                        }

                                        if (indexToDelete.HasValue) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                                "delete Variable Modifier");
                                            waveSetting.waveDefeatVariableModifiers.DeleteByIndex(indexToDelete.Value);
                                        }
                                    }
                                    DTInspectorUtility.EndGroupedControls();
                                }
                                EditorGUILayout.EndToggleGroup();

                                DTInspectorUtility.AddSpaceForNonU5(2);
                                DTInspectorUtility.StartGroupHeader(0, false);
                                EditorGUI.indentLevel = 0;
                                // beat level Custom Events to fire
                                var newExp = EditorGUILayout.BeginToggleGroup(" Wave Completion Custom Events", waveSetting.useCompletionEvents);
                                if (newExp != waveSetting.useCompletionEvents) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Completion Completion Custom Events");
                                    waveSetting.useCompletionEvents = newExp;
                                }
                                EditorGUILayout.EndVertical();

                                if (waveSetting.useCompletionEvents) {
                                    DTInspectorUtility.BeginGroupedControls();
                                    DTInspectorUtility.ShowColorWarningBox("When wave completed, fire the Custom Events below");
                                    EditorGUILayout.BeginHorizontal();
                                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                                    GUILayout.Space(6);
                                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Completion Custom Event");
                                        waveSetting.completionCustomEvents.Add(new CGKCustomEventToFire());
                                    }
                                    GUI.contentColor = Color.white;

                                    EditorGUILayout.EndHorizontal();

                                    if (waveSetting.completionCustomEvents.Count == 0) {
                                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                                    }

                                    if (waveSetting.completionCustomEvents.Count > 0) {
                                        DTInspectorUtility.VerticalSpace(2);
                                    }

                                    int? indexToDelete = null;

                                    // ReSharper disable once ForCanBeConvertedToForeach
                                    for (var i = 0; i < waveSetting.completionCustomEvents.Count; i++) {
                                        var anEvent = waveSetting.completionCustomEvents[i].CustomEventName;

                                        var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent, _settings, "Custom Event", ref buttonClicked);

                                        if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove) {
                                            indexToDelete = i;
                                        }

                                        if (anEvent == waveSetting.completionCustomEvents[i].CustomEventName) {
                                            continue;
                                        }

                                        waveSetting.completionCustomEvents[i].CustomEventName = anEvent;
                                    }

                                    if (indexToDelete.HasValue) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Last Wave Completion Custom Event");
                                        waveSetting.completionCustomEvents.RemoveAt(indexToDelete.Value);
                                    }

                                    DTInspectorUtility.EndGroupedControls();
                                }
                                EditorGUILayout.EndToggleGroup();

                            }

                            if (showVisualize) {
                                var allSpawners = LevelSettings.GetAllSpawners;
                                // ReSharper disable once ForCanBeConvertedToForeach
                                for (var i = 0; i < allSpawners.Count; i++) {
                                    var aSpawner = allSpawners[i];
                                    aSpawner.gameObject.DestroyChildrenImmediateWithMarker();

                                    var spawn = aSpawner.GetComponent<WaveSyncroPrefabSpawner>();
                                    // ReSharper disable ForCanBeConvertedToForeach
                                    for (var wave = 0; wave < spawn.waveSpecs.Count; wave++) {
                                        // ReSharper restore ForCanBeConvertedToForeach
                                        spawn.waveSpecs[wave].visualizeWave = false;
                                    }
                                }

                                var spawnersUsed = FindMatchingSpawners(l, w);
                                foreach (var spawner in spawnersUsed) {
                                    // ReSharper disable once ForCanBeConvertedToForeach
                                    for (var lw = 0; lw < spawner.waveSpecs.Count; lw++) {
                                        var aWave = spawner.waveSpecs[lw];
                                        // ReSharper disable once InvertIf
                                        if (aWave.SpawnLevelNumber == l && aWave.SpawnWaveNumber == w) {
                                            aWave.visualizeWave = true;
                                            //Debug.Log(spawner.name + " : " + l + " : " + w);
                                            spawner.SpawnWaveVisual(aWave);
                                        }
                                    }
                                }
                            }

                            switch (waveButtonPressed) {
                                case DTInspectorUtility.FunctionButtons.ShiftUp:
                                    waveToShiftUp = w;
                                    break;
                                case DTInspectorUtility.FunctionButtons.ShiftDown:
                                    waveToShiftDown = w;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Remove:
                                    if (levelSetting.WaveSettings.Count <= 1) {
                                        DTInspectorUtility.ShowAlert("You cannot delete the only Wave in a Level. Delete the Level if you like.");
                                    } else {
                                        waveToDelete = w;
                                    }

                                    _isDirty = true;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Add:
                                    waveToInsertAt = w;
                                    _isDirty = true;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Copy:
                                    waveToCopy = w;
                                    break;
                                case DTInspectorUtility.FunctionButtons.ShowRelations:
                                    DTInspectorUtility.ShowLevelAndWaveSpawnedPrefabs(l + 1, w + 1);
                                    break;
                            }

                            EditorGUILayout.EndVertical();
                            DTInspectorUtility.AddSpaceForNonU5();
                        }

                        if (waveToDelete >= 0) {
                            if (DTInspectorUtility.ConfirmDialog("Delete wave? This cannot be undone.")) {
                                DeleteWave(levelSetting, waveToDelete, l);
                                _isDirty = true;
                            }
                        }
                        if (waveToInsertAt > -1) {
                            if (DTInspectorUtility.ConfirmDialog("Add wave? This cannot be undone.")) {
                                InsertWaveAfter(levelSetting, waveToInsertAt, l);
                                _isDirty = true;
                            }
                        }
                        if (waveToCopy.HasValue) {
                            if (DTInspectorUtility.ConfirmDialog("Clone wave? This cannot be undone.")) {
                                CloneWave(levelSetting, waveToCopy.Value, l);
                                _isDirty = true;
                            }
                        }
                        if (waveToShiftUp.HasValue) {
                            if (DTInspectorUtility.ConfirmDialog("Shift wave up? This cannot be undone.")) {
                                ShiftUpWave(levelSetting, waveToShiftUp.Value, l);
                                _isDirty = true;
                            }
                        }
                        if (waveToShiftDown.HasValue) {
                            if (DTInspectorUtility.ConfirmDialog("Shift wave down? This cannot be undone.")) {
                                ShiftDownWave(levelSetting, waveToShiftDown.Value, l);
                                _isDirty = true;
                            }
                        }
                    }

                    switch (levelButtonPressed) {
                        case DTInspectorUtility.FunctionButtons.Remove:
                            if (DTInspectorUtility.ConfirmDialog("Delete level? This cannot be undone.")) {
                                levelToDelete = l;
                                _isDirty = true;
                            }
                            break;
                        case DTInspectorUtility.FunctionButtons.Add:
                            _isDirty = true;
                            levelToInsertAt = l;
                            break;
                        case DTInspectorUtility.FunctionButtons.ShowRelations:
                            DTInspectorUtility.ShowLevelAndWaveSpawnedPrefabs(l + 1, 0);
                            break;
                        case DTInspectorUtility.FunctionButtons.ShiftUp:
                            levelToShiftUp = l;
                            break;
                        case DTInspectorUtility.FunctionButtons.ShiftDown:
                            levelToShiftDown = l;
                            break;
                        case DTInspectorUtility.FunctionButtons.Copy:
                            levelToCopy = l;
                            break;
                    }

                    EditorGUILayout.EndVertical();

                    if (!levelSetting.isExpanded) {
                        continue;
                    }

                    DTInspectorUtility.VerticalSpace(0);
                    DTInspectorUtility.AddSpaceForNonU5(3);
                }

                if (levelToDelete > -1) {
                    DeleteLevel(levelToDelete);
                }

                if (levelToInsertAt > -1) {
                    CreateNewLevelAfter(levelToInsertAt);
                }

                if (levelToShiftUp.HasValue) {
                    ShiftUpLevel(levelToShiftUp.Value);
                }
                if (levelToShiftDown.HasValue) {
                    ShiftDownLevel(levelToShiftDown.Value);
                }
                if (levelToCopy.HasValue) {
                    CloneLevel(levelToCopy.Value);
                }

                DTInspectorUtility.EndGroupedControls();
            } else {
                DTInspectorUtility.BeginGroupedControls();
                DTInspectorUtility.ShowLargeBarAlertBox("Levels & Waves disabled. Check 'Use Global Waves' up top to use.");
                DTInspectorUtility.EndGroupedControls();
            }
        }

        // level waves
        EditorGUI.indentLevel = 0;
        DTInspectorUtility.VerticalSpace(2);

        state = _settings.showCustomEvents;
        text = "Custom Events";

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);

        if (state != _settings.showCustomEvents) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Events");
            _settings.showCustomEvents = state;
        }

        DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/LevelWaveSettings.htm#CustomEvents");

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        if (_settings.showCustomEvents) {
            var catNames = new List<string>(_settings.customEventCategories.Count);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _settings.customEventCategories.Count; i++) {
                catNames.Add(_settings.customEventCategories[i].CatName);
            }

            var selCatIndex = catNames.IndexOf(_settings.addToCustomEventCategoryName);

            if (selCatIndex == -1) {
                selCatIndex = 0;
                _isDirty = true;
            }

            var defaultCat = catNames[selCatIndex];

            DTInspectorUtility.BeginGroupedControls();
            DTInspectorUtility.StartGroupHeader(0, false);
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            var newEvent = EditorGUILayout.TextField("New Event Name", _settings.newEventName);
            if (newEvent != _settings.newEventName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Event Name");
                _settings.newEventName = newEvent;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Create New Event", EditorStyles.toolbarButton, GUILayout.Width(100))) {
                CreateCustomEvent(_settings.newEventName, defaultCat);
            }
            GUILayout.Space(6);
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;

            GUI.contentColor = Color.white;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            DTInspectorUtility.AddSpaceForNonU5(2);

            DTInspectorUtility.StartGroupHeader(0, false);
            DTInspectorUtility.ResetColors();
            var newCat = EditorGUILayout.TextField("New Category Name", _settings.newCustomEventCategoryName);
            if (newCat != _settings.newCustomEventCategoryName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change New Category Name");
                _settings.newCustomEventCategoryName = newCat;
            }
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            GUILayout.Space(4);
            if (GUILayout.Button("Create New Category", EditorStyles.toolbarButton, GUILayout.Width(130))) {
                CreateCategory();
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            DTInspectorUtility.ResetColors();

            DTInspectorUtility.AddSpaceForNonU5(2);
            GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;

            var newIndex = EditorGUILayout.Popup("Default Event Category", selCatIndex, catNames.ToArray());
            if (newIndex != selCatIndex) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Default Event Category");
                _settings.addToCustomEventCategoryName = catNames[newIndex];
            }

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(6);

            var hasExpanded = false;
            foreach (var t in _settings.customEvents) {
                if (string.IsNullOrEmpty(t.categoryName)) {
                    t.categoryName = defaultCat;
                }

                if (!t.eventExpanded) {
                    continue;
                }
                hasExpanded = true;
                break;
            }

            var buttonText = hasExpanded ? "Collapse All" : "Expand All";

            if (GUILayout.Button(buttonText, EditorStyles.toolbarButton, GUILayout.Width(80))) {
                ExpandCollapseCustomEvents(!hasExpanded);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            int? indexToShiftUp = null;
            int? indexToShiftDown = null;
            CgkCustomEvent eventRenaming = null;
            CgkCustomEvent eventToDelete = null;
            CgkCustomEvent eventEditing = null;
            CgkCustomEvent eventToVisualize = null;
            CgkCustomEvent eventToHide = null;
            CGKCustomEventCategory catEditing = null;
            CGKCustomEventCategory catRenaming = null;
            CGKCustomEventCategory catToDelete = null;

            DTInspectorUtility.StartGroupHeader(1, true);

            for (var c = 0; c < _settings.customEventCategories.Count; c++) {
                var cat = _settings.customEventCategories[c];

                EditorGUI.indentLevel = 0;

                var matchingItems = new List<CgkCustomEvent>();
                matchingItems.AddRange(_settings.customEvents);
                matchingItems.RemoveAll(delegate (CgkCustomEvent x) {
                    return x.categoryName != cat.CatName;
                });

                var hasItems = matchingItems.Count > 0;

                EditorGUILayout.BeginHorizontal();

                if (!cat.IsEditing || Application.isPlaying) {
                    var catName = cat.CatName;

                    catName += ": " + matchingItems.Count + " item" + ((matchingItems.Count != 1) ? "s" : "");

                    var state2 = cat.IsExpanded;
                    var text2 = catName;

                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (!state2) {
                        GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
                    } else {
                        GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
                    }

                    text2 = "<b><size=11>" + text2 + "</size></b>";

                    if (state2) {
                        text2 = "\u25BC " + text2;
                    } else {
                        text2 = "\u25BA " + text2;
                    }
                    if (!GUILayout.Toggle(true, text2, "dragtab", GUILayout.MinWidth(20f))) {
                        state2 = !state2;
                    }

                    GUILayout.Space(2f);

                    if (state2 != cat.IsExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                            "toggle expand Custom Event Category");
                        cat.IsExpanded = state2;
                    }

                    var catItemsCollapsed = true;

                    for (var i = 0; i < matchingItems.Count; i++) {
                        var item = matchingItems[i];

                        if (!item.eventExpanded) {
                            continue;
                        }
                        catItemsCollapsed = false;
                        break;
                    }

                    GUI.backgroundColor = Color.white;

                    var tooltip = catItemsCollapsed
                        ? "Click to expand all items in this category"
                        : "Click to collapse all items in this category";
                    var btnText = catItemsCollapsed ? "Expand" : "Collapse";

                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button(new GUIContent(btnText, tooltip), EditorStyles.toolbarButton,
                        GUILayout.Width(60), GUILayout.Height(16))) {
                        ExpandCollapseCategory(cat.CatName, catItemsCollapsed);
                    }
                    GUI.contentColor = Color.white;

                    if (!Application.isPlaying) {
                        if (c > 0) {
                            // the up arrow.
                            var upArrow = CoreGameKitInspectorResources.UpArrowTexture;
                            if (GUILayout.Button(new GUIContent(upArrow, "Click to shift Category up"),
                                EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16))) {
                                indexToShiftUp = c;
                            }
                        } else {
                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                        }

                        if (c < _settings.customEventCategories.Count - 1) {
                            // The down arrow will move things towards the end of the List
                            var dnArrow = CoreGameKitInspectorResources.DownArrowTexture;
                            if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift Category down"),
                                EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16))) {
                                indexToShiftDown = c;
                            }
                        } else {
                            GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                        }

                        var settingsIcon = new GUIContent(CoreGameKitInspectorResources.SettingsTexture,
                            "Click to edit Category");

                        GUI.backgroundColor = Color.white;
                        if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24),
                            GUILayout.Height(16))) {
                            catEditing = cat;
                        }
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Click to delete Category"),
                            EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                            catToDelete = cat;
                        }

                        GUILayout.Space(2);
                    } else {
                        GUILayout.Space(4);
                    }

                } else {
                    GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
                    var tex = EditorGUILayout.TextField("", cat.ProspectiveName);
                    if (tex != cat.ProspectiveName) {
                        cat.ProspectiveName = tex;
                        _isDirty = true;
                    }

                    var buttonPressed = DTInspectorUtility.AddCancelSaveButtons("Custom Event Category");

                    switch (buttonPressed) {
                        case DTInspectorUtility.FunctionButtons.Cancel:
                            cat.IsEditing = false;
                            cat.ProspectiveName = cat.CatName;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.Save:
                            catRenaming = cat;
                            break;
                    }

                    GUILayout.Space(4);
                }

                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                if (cat.IsEditing) {
                    DTInspectorUtility.VerticalSpace(2);
                }

                matchingItems.Sort(delegate (CgkCustomEvent x, CgkCustomEvent y) {
                    // ReSharper disable PossibleNullReferenceException
                    return string.Compare(x.EventName, y.EventName, StringComparison.Ordinal);
                    // ReSharper restore PossibleNullReferenceException
                });

                if (!hasItems) {
                    DTInspectorUtility.BeginGroupedControls();
                    DTInspectorUtility.ShowLargeBarAlertBox("This Category is empty. Add / move some items or you may delete it.");
                    DTInspectorUtility.EndGroupedControls();
                }

                GUI.contentColor = Color.white;

                if (cat.IsExpanded) {
                    for (var i = 0; i < matchingItems.Count; i++) {
                        EditorGUI.indentLevel = 1;
                        var anEvent = matchingItems[i];

                        DTInspectorUtility.AddSpaceForNonU5(2);
                        DTInspectorUtility.StartGroupHeader();

                        EditorGUILayout.BeginHorizontal();

                        if (!anEvent.IsEditing || Application.isPlaying) {
                            var eName = anEvent.EventName;

                            var exp = DTInspectorUtility.Foldout(anEvent.eventExpanded, eName);
                            if (exp != anEvent.eventExpanded) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                    "toggle expand Custom Event");
                                anEvent.eventExpanded = exp;
                            }
                            GUILayout.FlexibleSpace();

                            if (!Application.isPlaying) {
                                GUI.backgroundColor = DTInspectorUtility.BrightButtonColor;
                                var newCatIndex = catNames.IndexOf(anEvent.categoryName);
                                var newEventCat = EditorGUILayout.Popup(newCatIndex, catNames.ToArray(),
                                    GUILayout.Width(130));
                                if (newEventCat != newCatIndex) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                        "change Custom Event Category");
                                    anEvent.categoryName = catNames[newEventCat];
                                }
                                GUI.backgroundColor = Color.white;
                            }

                            if (Application.isPlaying) {
                                var receivers = LevelSettings.ReceiversForEvent(anEvent.EventName);

                                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                                if (receivers.Count > 0) {
                                    if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                        var matches = new List<GameObject>(receivers.Count);

                                        foreach (var t in receivers) {
                                            matches.Add(t.gameObject);
                                        }
                                        Selection.objects = matches.ToArray();
                                    }
                                }

                                if (GUILayout.Button("Fire!", EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                    LevelSettings.FireCustomEvent(anEvent.EventName, _settings.transform);
                                }

                                GUI.contentColor = DTInspectorUtility.BrightTextColor;
                                GUILayout.Label(string.Format("Receivers: {0}", receivers.Count));
                                GUI.contentColor = Color.white;
                            } else {
                                var buttonPressed = DTInspectorUtility.AddCustomEventIcons(anEvent.IsEditing, true, !anEvent.IsEditing, true);

                                switch (buttonPressed) {
                                    case DTInspectorUtility.FunctionButtons.Remove:
                                        eventToDelete = anEvent;
                                        break;
                                    case DTInspectorUtility.FunctionButtons.Visualize:
                                        eventToVisualize = anEvent;
                                        break;
                                    case DTInspectorUtility.FunctionButtons.Hide:
                                        eventToHide = anEvent;
                                        break;
                                    case DTInspectorUtility.FunctionButtons.Edit:
                                        eventEditing = anEvent;
                                        break;
                                }
                            }
                        } else {
                            var oldColor = GUI.backgroundColor;
                            GUI.backgroundColor = DTInspectorUtility.BrightTextColor;
                            var newName = GUILayout.TextField(anEvent.ProspectiveName, GUILayout.Width(160));
                            if (newName != anEvent.ProspectiveName) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Proposed Event Name");
                                anEvent.ProspectiveName = newName;
                            }
                            GUI.backgroundColor = oldColor;

                            GUILayout.FlexibleSpace();

                            var buttonPressed2 = DTInspectorUtility.AddCustomEventIcons(true, true, false, true);

                            switch (buttonPressed2) {
                                case DTInspectorUtility.FunctionButtons.Rename:
                                    eventRenaming = anEvent;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Cancel:
                                    anEvent.IsEditing = false;
                                    _isDirty = true;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Remove:
                                    eventToDelete = anEvent;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Visualize:
                                    eventToVisualize = anEvent;
                                    break;
                                case DTInspectorUtility.FunctionButtons.Hide:
                                    eventToHide = anEvent;
                                    break;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        if (!anEvent.eventExpanded) {
                            EditorGUILayout.EndVertical();
                            DTInspectorUtility.AddSpaceForNonU5();
                            continue;
                        }
                        EditorGUI.indentLevel = 0;
                        var rcvMode = (LevelSettings.EventReceiveMode)EditorGUILayout.EnumPopup("Send To Receivers", anEvent.eventRcvMode);
                        if (rcvMode != anEvent.eventRcvMode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Send To Receivers");
                            anEvent.eventRcvMode = rcvMode;
                        }

                        if (rcvMode == LevelSettings.EventReceiveMode.WhenDistanceLessThan || rcvMode == LevelSettings.EventReceiveMode.WhenDistanceMoreThan) {
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, anEvent.distanceThreshold, "Distance Threshold", _settings);
                        }

                        if (rcvMode != LevelSettings.EventReceiveMode.Never) {
                            var rcvFilter = (LevelSettings.EventReceiveFilter)EditorGUILayout.EnumPopup("Valid Receivers", anEvent.eventRcvFilterMode);
                            if (rcvFilter != anEvent.eventRcvFilterMode) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Valid Receivers");
                                anEvent.eventRcvFilterMode = rcvFilter;
                            }
                        }

                        switch (anEvent.eventRcvFilterMode) {
                            case LevelSettings.EventReceiveFilter.Closest:
                            case LevelSettings.EventReceiveFilter.Random:
                                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, anEvent.filterModeQty, "Valid Qty", _settings);
                                break;
                        }

                        EditorGUILayout.EndVertical();
                        DTInspectorUtility.AddSpaceForNonU5();
                    }
                }

                if (c < _settings.customEventCategories.Count - 1) {
                    DTInspectorUtility.VerticalSpace(3);
                }
            }

            DTInspectorUtility.EndGroupHeader();


            if (eventToDelete != null) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Delete Custom Event");
                _settings.customEvents.Remove(eventToDelete);
            }
            if (eventRenaming != null) {
                RenameEvent(eventRenaming);
            }
            if (eventEditing != null) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Edit Custom Event");
                eventEditing.IsEditing = true;
                _isDirty = true;
            }

            if (eventToVisualize != null) {
                VisualizeEvent(eventToVisualize);
            }
            if (eventToHide != null) {
                HideEvent(eventToHide);
            }

            if (indexToShiftUp.HasValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift up Category");
                var item = _settings.customEventCategories[indexToShiftUp.Value];
                _settings.customEventCategories.Insert(indexToShiftUp.Value - 1, item);
                _settings.customEventCategories.RemoveAt(indexToShiftUp.Value + 1);
                _isDirty = true;
            }

            if (indexToShiftDown.HasValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift down Category");
                var index = indexToShiftDown.Value + 1;
                var item = _settings.customEventCategories[index];
                _settings.customEventCategories.Insert(index - 1, item);
                _settings.customEventCategories.RemoveAt(index + 1);
                _isDirty = true;
            }

            if (catToDelete != null) {
                if (_settings.customEvents.FindAll(delegate (CgkCustomEvent x) {
                    return x.categoryName == catToDelete.CatName;
                }).Count > 0) {
                    DTInspectorUtility.ShowAlert("You cannot delete a Category with Custom Events in it. Move or delete the items first.");
                } else if (_settings.customEventCategories.Count <= 1) {
                    DTInspectorUtility.ShowAlert("You cannot delete the last Category.");
                } else {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Delete Category");
                    _settings.customEventCategories.Remove(catToDelete);
                    _isDirty = true;
                }
            }

            if (catRenaming != null) {
                // ReSharper disable once ForCanBeConvertedToForeach
                var isValidName = true;

                if (string.IsNullOrEmpty(catRenaming.ProspectiveName)) {
                    isValidName = false;
                    DTInspectorUtility.ShowAlert("You cannot have a blank Category name.");
                }

                if (isValidName) {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var c = 0; c < _settings.customEventCategories.Count; c++) {
                        var cat = _settings.customEventCategories[c];
                        // ReSharper disable once InvertIf
                        if (cat != catRenaming && cat.CatName == catRenaming.ProspectiveName) {
                            isValidName = false;
                            DTInspectorUtility.ShowAlert("You already have a Category named '" + catRenaming.ProspectiveName + "'. Category names must be unique.");
                        }
                    }

                    if (isValidName) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Undo change Category name.");

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < _settings.customEvents.Count; i++) {
                            var item = _settings.customEvents[i];
                            if (item.categoryName == catRenaming.CatName) {
                                item.categoryName = catRenaming.ProspectiveName;
                            }
                        }

                        catRenaming.CatName = catRenaming.ProspectiveName;
                        catRenaming.IsEditing = false;
                        _isDirty = true;
                    }
                }
            }

            if (catEditing != null) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var c = 0; c < _settings.customEventCategories.Count; c++) {
                    var cat = _settings.customEventCategories[c];
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (catEditing == cat) {
                        cat.IsEditing = true;
                    } else {
                        cat.IsEditing = false;
                    }

                    _isDirty = true;
                }
            }

            if (eventEditing != null) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var c = 0; c < _settings.customEvents.Count; c++) {
                    var evt = _settings.customEvents[c];
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (eventEditing == evt) {
                        evt.IsEditing = true;
                    } else {
                        evt.IsEditing = false;
                    }

                    _isDirty = true;
                }
            }

            if (eventRenaming != null) {
                // ReSharper disable once ForCanBeConvertedToForeach
                var isValidName = true;

                if (string.IsNullOrEmpty(eventRenaming.ProspectiveName)) {
                    isValidName = false;
                    DTInspectorUtility.ShowAlert("You cannot have a blank Custom Event name.");
                }

                if (isValidName) {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var c = 0; c < _settings.customEvents.Count; c++) {
                        var evt = _settings.customEvents[c];
                        // ReSharper disable once InvertIf
                        if (evt != eventRenaming && evt.EventName == eventRenaming.ProspectiveName) {
                            isValidName = false;
                            DTInspectorUtility.ShowAlert("You already have a Custom Event named '" + eventRenaming.ProspectiveName + "'. Custom Event names must be unique.");
                        }
                    }

                    if (isValidName) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                            "Undo change Custom Event name.");

                        eventRenaming.EventName = eventRenaming.ProspectiveName;
                        eventRenaming.IsEditing = false;
                        _isDirty = true;
                    }
                }
            }

            DTInspectorUtility.EndGroupedControls();
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target); // or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void ExpandCollapseAll(bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand / collapse all Level Wave Settings");

        foreach (var level in _settings.LevelTimes) {
            level.isExpanded = isExpand;
            foreach (var wave in level.WaveSettings) {
                wave.isExpanded = isExpand;
            }
        }
    }

    private void CreateSpawner() {
        var newSpawnerName = _settings.newSpawnerName;

        if (string.IsNullOrEmpty(newSpawnerName)) {
            DTInspectorUtility.ShowAlert("You must enter a name for your new Spawner.");
            return;
        }

        Transform spawnerTrans = null;

        switch (_settings.newSpawnerType) {
            case LevelSettings.SpawnerType.Green:
                spawnerTrans = _settings.GreenSpawnerTrans;
                break;
            case LevelSettings.SpawnerType.Red:
                spawnerTrans = _settings.RedSpawnerTrans;
                break;
        }

        var spawnPos = _settings.transform.position;
        spawnPos.x += Random.Range(-10, 10);
        spawnPos.z += Random.Range(-10, 10);

        // ReSharper disable once PossibleNullReferenceException
        var newSpawner = Instantiate(spawnerTrans.gameObject, spawnPos, Quaternion.identity) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        UndoHelper.CreateObjectForUndo(newSpawner.gameObject, "create Spawner");
        newSpawner.name = newSpawnerName;

        var spawnersHolder = _settings.transform.GetChildTransform(LevelSettings.SpawnerContainerTransName);
        if (spawnersHolder == null) {
            DTInspectorUtility.ShowAlert(LevelSettings.NoSpawnContainerAlert);

            DestroyImmediate(newSpawner);

            return;
        }

        newSpawner.transform.parent = spawnersHolder.transform;
    }

    private void CreatePrefabPool() {
        var newPrefabPoolName = _settings.newPrefabPoolName;

        if (string.IsNullOrEmpty(newPrefabPoolName)) {
            DTInspectorUtility.ShowAlert("You must enter a name for your new Prefab Pool.");
            return;
        }

        var spawnPos = _settings.transform.position;

        var newPool = Instantiate(_settings.PrefabPoolTrans.gameObject, spawnPos, Quaternion.identity) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        newPool.name = newPrefabPoolName;

        var poolsHolder = _settings.transform.GetChildTransform(LevelSettings.PrefabPoolsContainerTransName);
        if (poolsHolder == null) {
            DTInspectorUtility.ShowAlert(LevelSettings.NoPrefabPoolsContainerAlert);

            DestroyImmediate(newPool);
            return;
        }

        var dupe = poolsHolder.GetChildTransform(newPrefabPoolName);
        if (dupe != null) {
            DTInspectorUtility.ShowAlert("You already have a Prefab Pool named '" + newPrefabPoolName + "', please choose another name.");

            DestroyImmediate(newPool);
            return;
        }

        UndoHelper.CreateObjectForUndo(newPool.gameObject, "create Prefab Pool");
        newPool.transform.parent = poolsHolder.transform;
    }

    private static void InsertWaveAfter(LevelSpecifics spec, int waveToInsertAt, int level) {
        var spawners = LevelSettings.GetAllSpawners;

        var newWave = new LevelWave();

        waveToInsertAt++;
        spec.WaveSettings.Insert(waveToInsertAt, newWave);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertWave(waveToInsertAt, level);
        }
    }

    private void ShiftUpLevel(int levelNum) {
        var waveToShiftUp = _settings.LevelTimes[levelNum];
        _settings.LevelTimes.RemoveAt(levelNum);
        _settings.LevelTimes.Insert(levelNum - 1, waveToShiftUp);

        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        foreach (var script in spawnerScripts) {
            script.ShiftUpLevel(levelNum);
        }
    }

    private void ShiftDownLevel(int levelNum) {
        var waveToShiftDown = _settings.LevelTimes[levelNum];
        _settings.LevelTimes.RemoveAt(levelNum);
        _settings.LevelTimes.Insert(levelNum + 1, waveToShiftDown);

        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        foreach (var script in spawnerScripts) {
            script.ShiftDownLevel(levelNum);
        }
    }

    private static void ShiftUpWave(LevelSpecifics spec, int waveNum, int level) {
        var waveToShiftUp = spec.WaveSettings[waveNum];
        spec.WaveSettings.RemoveAt(waveNum);
        spec.WaveSettings.Insert(waveNum - 1, waveToShiftUp);

        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        foreach (var script in spawnerScripts) {
            script.ShiftUpWave(level, waveNum);
        }
    }

    private static void ShiftDownWave(LevelSpecifics spec, int waveNum, int level) {
        var waveToShiftDown = spec.WaveSettings[waveNum];
        spec.WaveSettings.RemoveAt(waveNum);
        spec.WaveSettings.Insert(waveNum + 1, waveToShiftDown);

        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        foreach (var script in spawnerScripts) {
            script.ShiftDownWave(level, waveNum);
        }
    }

    private static void CloneWave(LevelSpecifics spec, int waveToInsertAt, int level) {
        var spawners = LevelSettings.GetAllSpawners;

        var newWave = CloningHelper.CloneLevelWave(spec.WaveSettings[waveToInsertAt]);

        waveToInsertAt++;
        spec.WaveSettings.Insert(waveToInsertAt, newWave);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertWave(waveToInsertAt, level);
        }
    }

    private void CloneLevel(int levelNum) {
        var levelToClone = _settings.LevelTimes[levelNum];
        var newLevel = CloningHelper.CloneLevel(levelToClone);

        _settings.LevelTimes.Insert(levelNum + 1, newLevel);

        var spawners = LevelSettings.GetAllSpawners;

        for (var i = 0; i < newLevel.WaveSettings.Count; i++) {
            foreach (var spawner in spawners) {
                var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
                var waveInSpawner = spawnerScript.FindWave(levelNum, i);
                if (waveInSpawner == null) {
                    continue;
                }

                var clonedWave = CloningHelper.CloneWave(waveInSpawner);
                clonedWave.SpawnLevelNumber = levelNum + 1;

                spawnerScript.waveSpecs.Add(clonedWave);
            }
        }
    }

    private void DeleteLevel(int levelToDelete) {
        var spawners = LevelSettings.GetAllSpawners;

        _settings.LevelTimes.RemoveAt(levelToDelete);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.DeleteLevel(levelToDelete);
        }
    }

    private void CreateNewLevelAfter(int? index = null) {
        var spawners = LevelSettings.GetAllSpawners;

        var newLevel = new LevelSpecifics();
        var newWave = new LevelWave();
        newLevel.WaveSettings.Add(newWave);

        int newLevelIndex;

        if (index == null) {
            newLevelIndex = _settings.LevelTimes.Count;
        } else {
            newLevelIndex = index.Value + 1;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Level");

        _settings.LevelTimes.Insert(newLevelIndex, newLevel);

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            spawnerScript.InsertLevel(newLevelIndex);
        }
    }

    private static void DeleteWave(LevelSpecifics spec, int waveToDelete, int levelNumber) {
        var spawners = LevelSettings.GetAllSpawners;

        var spawnerScripts = new List<WaveSyncroPrefabSpawner>();
        foreach (var s in spawners) {
            spawnerScripts.Add(s.GetComponent<WaveSyncroPrefabSpawner>());
        }

        spec.WaveSettings.RemoveAt(waveToDelete);

        foreach (var script in spawnerScripts) {
            script.DeleteWave(levelNumber, waveToDelete);
        }
    }

    private void AddWaveSkipLimit(string modifierName, LevelWave spec) {
        if (spec.skipWavePassCriteria.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This wave already has a Skip Wave Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Skip Wave Limit");

        spec.skipWavePassCriteria.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
    }

    private static List<WaveSyncroPrefabSpawner> FindMatchingSpawners(int level, int wave) {
        var spawners = LevelSettings.GetAllSpawners;

        var matchingSpawners = new List<WaveSyncroPrefabSpawner>();

        foreach (var spawner in spawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            var matchingWave = spawnerScript.FindWave(level, wave);
            if (matchingWave == null) {
                continue;
            }

            matchingSpawners.Add(spawnerScript);
        }

        return matchingSpawners;
    }

    private void AddBonusStatModifier(string modifierName, LevelWave waveSpec) {
        if (waveSpec.waveDefeatVariableModifiers.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Completion Bonus modifier");

        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

        waveSpec.waveDefeatVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    }

    private void CreateLevelSettingsPrefab() {
        // ReSharper disable once RedundantCast
        var go = Instantiate(_settings.gameObject) as GameObject;
        // ReSharper disable once PossibleNullReferenceException
        go.name = "LevelWaveSettings";
        go.transform.position = Vector3.zero;
    }

    private void CreateCategory() {
        if (string.IsNullOrEmpty(_settings.newCustomEventCategoryName)) {
            DTInspectorUtility.ShowAlert("You cannot have a blank Category name.");
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var c = 0; c < _settings.customEventCategories.Count; c++) {
            var cat = _settings.customEventCategories[c];
            // ReSharper disable once InvertIf
            if (cat.CatName == _settings.newCustomEventCategoryName) {
                DTInspectorUtility.ShowAlert("You already have a Category named '" + _settings.newCustomEventCategoryName + "'. Category names must be unique.");
                return;
            }
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Create New Category");

        var newCat = new CGKCustomEventCategory {
            CatName = _settings.newCustomEventCategoryName,
            ProspectiveName = _settings.newCustomEventCategoryName
        };

        _settings.customEventCategories.Add(newCat);
    }

    private void RenameEvent(CgkCustomEvent cEvent) {
        var match = _settings.customEvents.FindAll(delegate (CgkCustomEvent obj) {
            return obj.EventName == cEvent.ProspectiveName;
        });

        if (match.Count > 0) {
            DTInspectorUtility.ShowAlert("You already have a Custom Event named '" + cEvent.ProspectiveName + "'. Please choose a different name.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Rename Custom Event");
        cEvent.EventName = cEvent.ProspectiveName;
    }

    private void CreateCustomEvent(string newEventName, string defaultCategory) {
        if (_settings.customEvents.FindAll(delegate (CgkCustomEvent obj) {
            return obj.EventName == newEventName;
        }).Count > 0) {
            DTInspectorUtility.ShowAlert("You already have a Custom Event named '" + newEventName + "'. Please choose a different name.");
            return;
        }

        var newEvent = new CgkCustomEvent(newEventName);
        newEvent.categoryName = defaultCategory;

        _settings.customEvents.Add(newEvent);
    }

    private void VisualizeEvent(CgkCustomEvent cEvent) {
        var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawnerV2));

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var t = 0; t < trigSpawners.Length; t++) {
            var trigSpawner = (TriggeredSpawnerV2)trigSpawners[t];

            var matchingWave = trigSpawner.userDefinedEventWaves.Find(delegate (TriggeredWaveSpecifics obj) {
                return obj.customEventName == cEvent.EventName;
            });

            if (matchingWave == null || !matchingWave.enableWave) {
                continue;
            }

            var isChanged = false;

            var allWaves = new List<TriggeredWaveSpecifics>(trigSpawner.AllWaves.Keys.Count);
            foreach (var key in trigSpawner.AllWaves.Keys) {
                allWaves.AddRange(trigSpawner.AllWaves[key]);
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var w = 0; w < allWaves.Count; w++) {
                var wave = allWaves[w];
                if (!wave.enableWave || !wave.visualizeWave || wave == matchingWave) {
                    continue;
                }

                isChanged = true;
                wave.visualizeWave = false;
                break;
            }

            if (!matchingWave.visualizeWave) {
                isChanged = true;
                matchingWave.visualizeWave = true;
            }

            trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();
            trigSpawner.SpawnWaveVisual(matchingWave);

            if (isChanged) {
                EditorUtility.SetDirty(trigSpawner);
            }
        }
    }

    private void HideEvent(CgkCustomEvent cEvent) {
        var trigSpawners = FindObjectsOfType(typeof(TriggeredSpawnerV2));

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var t = 0; t < trigSpawners.Length; t++) {
            var trigSpawner = (TriggeredSpawnerV2)trigSpawners[t];

            var matchingWave = trigSpawner.userDefinedEventWaves.Find(delegate (TriggeredWaveSpecifics obj) {
                return obj.customEventName == cEvent.EventName && obj.enableWave && obj.visualizeWave;
            });

            if (matchingWave == null) {
                continue;
            }

            matchingWave.visualizeWave = false;
            trigSpawner.gameObject.DestroyChildrenImmediateWithMarker();

            EditorUtility.SetDirty(trigSpawner);
        }
    }

    private void ExpandCollapseCustomEvents(bool shouldExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Expand / Collapse All Custom Events");

        foreach (var t in _settings.customEvents) {
            t.eventExpanded = shouldExpand;
        }
    }

    private void ExpandCollapseCategory(string category, bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand / collapse all items in Category");

        foreach (var item in _settings.customEvents) {
            if (item.categoryName != category) {
                continue;
            }

            item.eventExpanded = isExpand;
        }
    }

    private string GetFriendlyEventName(TriggeredSpawner.EventType eType, TriggeredWaveSpecifics wave) {
        switch (eType) {
            case TriggeredSpawner.EventType.OnEnabled:
                return "Enabled";
            case TriggeredSpawner.EventType.OnDisabled:
                return "Disabled";
            case TriggeredSpawner.EventType.Visible:
                return "Visible";
            case TriggeredSpawner.EventType.Invisible:
                return "Invisible";
            case TriggeredSpawner.EventType.MouseOver_Legacy:
                return "Mouse Over (Legacy)";
            case TriggeredSpawner.EventType.MouseClick_Legacy:
                return "Mouse Click (Legacy)";
            case TriggeredSpawner.EventType.SliderChanged_uGUI:
                return "Slider Changed (uGUI)";
            case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                return "Button Click (uGUI)";
            case TriggeredSpawner.EventType.PointerDown_uGUI:
                return "Pointer Down (uGUI)";
            case TriggeredSpawner.EventType.PointerUp_uGUI:
                return "Pointer Up (uGUI)";
            case TriggeredSpawner.EventType.PointerEnter_uGUI:
                return "Pointer Enter (uGUI)";
            case TriggeredSpawner.EventType.PointerExit_uGUI:
                return "Pointer Exit (uGUI)";
            case TriggeredSpawner.EventType.Drag_uGUI:
                return "Drag (uGUI)";
            case TriggeredSpawner.EventType.Drop_uGUI:
                return "Drop (uGUI)";
            case TriggeredSpawner.EventType.Scroll_uGUI:
                return "Scroll (uGUI)";
            case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                return "Update Selected (uGUI)";
            case TriggeredSpawner.EventType.Select_uGUI:
                return "Select (uGUI)";
            case TriggeredSpawner.EventType.Deselect_uGUI:
                return "Deselect (uGUI)";
            case TriggeredSpawner.EventType.Move_uGUI:
                return "Move (uGUI)";
            case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                return "Init. Potential Drag (uGUI)";
            case TriggeredSpawner.EventType.BeginDrag_uGUI:
                return "Begin Drag (uGUI)";
            case TriggeredSpawner.EventType.EndDrag_uGUI:
                return "End Drag (uGUI)";
            case TriggeredSpawner.EventType.Submit_uGUI:
                return "Submit (uGUI)";
            case TriggeredSpawner.EventType.Cancel_uGUI:
                return "Cancel (uGUI)";
            case TriggeredSpawner.EventType.OnCollision:
                return "Collision Enter";
            case TriggeredSpawner.EventType.OnTriggerEnter:
                return "Trigger Enter";
            case TriggeredSpawner.EventType.OnTriggerExit:
                return "Trigger Exit";
            case TriggeredSpawner.EventType.OnCollision2D:
                return "2D Collision Enter";
            case TriggeredSpawner.EventType.OnTriggerEnter2D:
                return "2D Trigger Enter";
            case TriggeredSpawner.EventType.OnTriggerExit2D:
                return "2D Trigger Exit";
            case TriggeredSpawner.EventType.CodeTriggered1:
                return "Code-Triggered 1";
            case TriggeredSpawner.EventType.CodeTriggered2:
                return "Code-Triggered 2";
            case TriggeredSpawner.EventType.OnSpawned:
                return "Spawned";
            case TriggeredSpawner.EventType.OnDespawned:
                return "Despawned";
            case TriggeredSpawner.EventType.OnClick_NGUI:
                return "NGUI OnClick";
            case TriggeredSpawner.EventType.CustomEvent:
                return "Custom Event: " + wave.customEventName;
            default:
                return "-UNKNOWN-";
        }
    }
}
