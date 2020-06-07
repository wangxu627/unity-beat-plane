using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveSyncroPrefabSpawner), true)]
// ReSharper disable once CheckNamespace
public class WaveSyncroPrefabSpawnerInspector : Editor {
    private LevelSettings _levSettings;
    private WaveSyncroPrefabSpawner _settings;
    private bool _isDirty;


    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        _settings = (WaveSyncroPrefabSpawner)target;

        WorldVariableTracker.ClearInGamePlayerStats();

        _isDirty = false;

        var myParent = _settings.transform.parent;
        Transform levelSettingObj = null;
        LevelSettings levelSettings = null;

        LevelSettings.Instance = null; // clear cached version

        if (myParent != null) {
            levelSettingObj = myParent.parent;
            if (levelSettingObj != null) {
                levelSettings = levelSettingObj.GetComponent<LevelSettings>();
            }
        }

        if (myParent == null || levelSettingObj == null || levelSettings == null) {
            DrawDefaultInspector();
            return;
        }

        var allStats = KillerVariablesHelper.AllStatNames;

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/SyncroSpawners.htm");

        EditorGUI.indentLevel = 0;

        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never) {
            DTInspectorUtility.RedBoldMessage("Spawner disabled by Active Mode setting");
        } else if (Application.isPlaying) {
            if (_settings.GameIsOverForSpawner) {
                DTInspectorUtility.RedBoldMessage("Spawner disabled by Game Over Behavior setting");
            } else if (_settings.SpawnerIsPaused) {
                DTInspectorUtility.RedBoldMessage("Spawner paused by Wave Pause Behavior setting");
            }
        }

        DTInspectorUtility.StartGroupHeader();

        var waveActivated = false;

        var newActive = (LevelSettings.ActiveItemMode)EditorGUILayout.EnumPopup("Active Mode", _settings.activeMode);
        if (newActive != _settings.activeMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Mode");
            _settings.activeMode = newActive;
            if (!Application.isPlaying) {
                _settings.gameObject.DestroyChildrenImmediateWithMarker();
            }

            if (newActive != LevelSettings.ActiveItemMode.Never) {
                waveActivated = true;
            }
        }
        EditorGUILayout.EndVertical();

        switch (_settings.activeMode) {
            case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
            case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                var missingStatNames = new List<string>();
                missingStatNames.AddRange(allStats);
                missingStatNames.RemoveAll(delegate (string obj) {
                    return _settings.activeItemCriteria.HasKey(obj);
                });

                var newStat = EditorGUILayout.Popup("Add Active Limit", 0, missingStatNames.ToArray());
                if (newStat != 0) {
                    AddActiveLimit(missingStatNames[newStat]);
                }

                if (_settings.activeItemCriteria.statMods.Count == 0) {
                    DTInspectorUtility.ShowRedErrorBox("You have no Active Limits. Spawner will never be Active.");
                } else {
                    EditorGUILayout.Separator();

                    int? indexToDelete = null;

                    for (var j = 0; j < _settings.activeItemCriteria.statMods.Count; j++) {
                        var modifier = _settings.activeItemCriteria.statMods[j];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        var statName = modifier._statName;
                        GUILayout.Label(statName);

                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Min");
                        switch (modifier._varTypeToUse) {
                            case WorldVariableTracker.VariableType._integer:
                                var newMin = EditorGUILayout.IntField(modifier._modValueIntMin, GUILayout.MaxWidth(60));
                                if (newMin != modifier._modValueIntMin) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Limit Min");
                                    modifier._modValueIntMin = newMin;
                                }
                                GUILayout.Label("Max");

                                var newMax = EditorGUILayout.IntField(modifier._modValueIntMax, GUILayout.MaxWidth(60));
                                if (newMax != modifier._modValueIntMax) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Limit Max");
                                    modifier._modValueIntMax = newMax;
                                }
                                break;
                            case WorldVariableTracker.VariableType._float:
                                var newMinFloat = EditorGUILayout.FloatField(modifier._modValueFloatMin, GUILayout.MaxWidth(60));
                                if (newMinFloat != modifier._modValueFloatMin) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Limit Min");
                                    modifier._modValueFloatMin = newMinFloat;
                                }
                                GUILayout.Label("Max");

                                var newMaxFloat = EditorGUILayout.FloatField(modifier._modValueFloatMax, GUILayout.MaxWidth(60));
                                if (newMaxFloat != modifier._modValueFloatMax) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Limit Max");
                                    modifier._modValueFloatMax = newMaxFloat;
                                }
                                break;
                            default:
                                Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                break;
                        }
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Remove this Limit"), EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                            indexToDelete = j;
                        }

                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(5);
                        EditorGUILayout.EndHorizontal();

                        KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                        var min = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMin : modifier._modValueFloatMin;
                        var max = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMax : modifier._modValueFloatMax;

                        if (min > max) {
                            DTInspectorUtility.ShowRedErrorBox(modifier._statName + " Min cannot exceed Max, please fix!");
                        }
                    }

                    DTInspectorUtility.ShowColorWarningBox("  Limits are inclusive: i.e. 'Above' means >=");
                    if (indexToDelete.HasValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Limit");
                        _settings.activeItemCriteria.DeleteByIndex(indexToDelete.Value);
                    }
                }

                break;
        }
        EditorGUILayout.EndVertical();

        var newGO = (TriggeredSpawner.GameOverBehavior)EditorGUILayout.EnumPopup("Game Over Behavior", _settings.gameOverBehavior);
        if (newGO != _settings.gameOverBehavior) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Game Over Behavior");
            _settings.gameOverBehavior = newGO;
        }

        var newPause = (TriggeredSpawner.WavePauseBehavior)EditorGUILayout.EnumPopup("Wave Pause Behavior", _settings.wavePauseBehavior);
        if (newPause != _settings.wavePauseBehavior) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Pause Behavior");
            _settings.wavePauseBehavior = newPause;
        }

        var newOutside = EditorGUILayout.Toggle(new GUIContent("Spawn Outside Pool", "If this is checked, everything spawned from this will not reside under Pool Boss in the Hierarchy, but instead with no parent Game Object."), _settings.spawnOutsidePool);
        if (newOutside != _settings.spawnOutsidePool) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawn Outside Pool");
            _settings.spawnOutsidePool = newOutside;
        }

        EditorGUI.indentLevel = 0;
        var hadNoListener = _settings.listener == null;
        var newListener = (WaveSyncroSpawnerListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(WaveSyncroSpawnerListener), true);
        if (newListener != _settings.listener) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;
            if (hadNoListener && _settings.listener != null) {
                _settings.listener.sourceSpawnerName = _settings.transform.name;
            }
        }

        DTInspectorUtility.StartGroupHeader();
        var newUseLayer = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Layer Mode", _settings.spawnLayerMode);
        if (newUseLayer != _settings.spawnLayerMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Layer Mode");
            _settings.spawnLayerMode = newUseLayer;
        }
        EditorGUILayout.EndVertical();

        if (_settings.spawnLayerMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom) {
            EditorGUI.indentLevel = 0;

            var newCustomLayer = EditorGUILayout.LayerField("Custom Spawn Layer", _settings.spawnCustomLayer);
            if (newCustomLayer != _settings.spawnCustomLayer) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Layer");
                _settings.spawnCustomLayer = newCustomLayer;
            }
        }

        if (_settings.spawnLayerMode != WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnPrefabSettings) {
            var newRecurse = EditorGUILayout.Toggle("Apply Layer Recursively", _settings.applyLayerRecursively);
            if (newRecurse != _settings.applyLayerRecursively) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Apply Layer Recursively");
                _settings.applyLayerRecursively = newRecurse;
            }
        }

        EditorGUILayout.EndVertical();

        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        EditorGUI.indentLevel = 0;
        var newUseTag = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Tag Mode", _settings.spawnTagMode);
        if (newUseTag != _settings.spawnTagMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Tag Mode");
            _settings.spawnTagMode = newUseTag;
        }
        EditorGUILayout.EndVertical();

        if (_settings.spawnTagMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom) {
            EditorGUI.indentLevel = 0;
            var newCustomTag = EditorGUILayout.TagField("Custom Spawn Tag", _settings.spawnCustomTag);
            if (newCustomTag != _settings.spawnCustomTag) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Tag");
                _settings.spawnCustomTag = newCustomTag;
            }
        }
        EditorGUILayout.EndVertical();


        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        var useFilter = EditorGUILayout.Toggle("Use Filter", _settings.useLevelFilter);
        if (useFilter != _settings.useLevelFilter) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Filter");
            _settings.useLevelFilter = useFilter;
        }
        EditorGUILayout.EndVertical();


        var filteredWaves = new List<WaveSpecifics>();
        filteredWaves.AddRange(_settings.waveSpecs);

        var filteredOut = 0;
        var allFilteredOut = false;

        if (_settings.useLevelFilter) {
            var origNum = filteredWaves.Count;
            filteredWaves.RemoveAll(delegate (WaveSpecifics spec) {
                return spec.SpawnLevelNumber != _settings.levelFilter;
            });
            filteredOut = origNum - filteredWaves.Count;
            if (filteredOut == origNum) {
                allFilteredOut = true;
            }
        }

        if (_settings.useLevelFilter) {
            var newfilter = EditorGUILayout.IntPopup("Level Filter", _settings.levelFilter + 1, LevelNames, LevelIndexes) - 1;

            if (newfilter != _settings.levelFilter) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Level Filter");
                _settings.levelFilter = newfilter;
            }

            DTInspectorUtility.ShowColorWarningBox(filteredOut + " wave(s) filtered out.");
        }

        EditorGUILayout.EndVertical();

        DTInspectorUtility.AddSpaceForNonU5();

        if (!Application.isPlaying) {
            DTInspectorUtility.StartGroupHeader();
            var useCopy = EditorGUILayout.Toggle("Copy Waves To Spawners", _settings.useCopyWave);
            if (useCopy != _settings.useCopyWave) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Copy Wave To Spawners");
                _settings.useCopyWave = useCopy;
            }
            EditorGUILayout.EndVertical();
            if (_settings.useCopyWave) {
                var spawnerTranforms = LevelSettings.GetAllSpawners;
                var otherSpawners = new List<WaveSyncroPrefabSpawner>(spawnerTranforms.Count);
                var selectedSpawners = new List<WaveSyncroPrefabSpawner>(otherSpawners.Count);

                for (var i = 0; i < spawnerTranforms.Count; i++) {
                    var spawner = spawnerTranforms[i].GetComponent<WaveSyncroPrefabSpawner>();
                    if (spawner.name == _settings.name) {
                        continue;
                    }
                    if (spawner.isSpawnerSelectedAsTarget) {
                        selectedSpawners.Add(spawner);
                    }
                    otherSpawners.Add(spawner);
                }

                var selectedWaves = new List<WaveSpecifics>(filteredWaves.Count);

                for (var w = 0; w < filteredWaves.Count; w++) {
                    var aWave = filteredWaves[w];
                    if (aWave.isSelectedToCopyFrom) {
                        selectedWaves.Add(aWave);
                    }
                }

                if (otherSpawners.Count == 0) {
                    DTInspectorUtility.ShowLargeBarAlertBox("You have no other Syncro Spawners, so nowhere to copy to.");
                } else {
                    DTInspectorUtility.ShowColorWarningBox(
                        "Select Source Waves from Wave Settings (checkboxes are on the left) & Target Spawners, then click Copy or Move.");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        "Source Waves (" + selectedWaves.Count + "/" + filteredWaves.Count + ")", EditorStyles.boldLabel,
                        GUILayout.Width(180));
                    GUILayout.FlexibleSpace();
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button("Select All", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Select All Source Waves");

                        for (var i = 0; i < filteredWaves.Count; i++) {
                            filteredWaves[i].isSelectedToCopyFrom = true;
                        }
                    }
                    GUILayout.Space(4);
                    if (GUILayout.Button("Clear All", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Clear All Source Waves");

                        for (var i = 0; i < filteredWaves.Count; i++) {
                            filteredWaves[i].isSelectedToCopyFrom = false;
                        }
                    }
                    GUILayout.Space(2);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    EditorGUILayout.LabelField(
                        "Target Spawners (" + selectedSpawners.Count + "/" + (otherSpawners.Count) + ")",
                        EditorStyles.boldLabel, GUILayout.Width(180));
                    GUILayout.FlexibleSpace();
                    if (selectedSpawners.Count > 0 && selectedWaves.Count > 0) {
                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                        if (GUILayout.Button("Copy", EditorStyles.toolbarButton, GUILayout.Width(40))) {
                            CopyWavesToTarget(selectedWaves, selectedSpawners, false);
                        }
                        GUILayout.Space(4);
                        if (GUILayout.Button("Move", EditorStyles.toolbarButton, GUILayout.Width(40))) {
                            CopyWavesToTarget(selectedWaves, selectedSpawners, true);
                        }
                        GUILayout.Space(8);
                    }
                    GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                    if (GUILayout.Button("Select All", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectsForUndo(otherSpawners.ToArray(), "Select All Target Spawners");
                        for (var i = 0; i < otherSpawners.Count; i++) {
                            var aSpawner = otherSpawners[i];
                            aSpawner.isSpawnerSelectedAsTarget = true;
                            EditorUtility.SetDirty(aSpawner);
                        }
                    }
                    GUILayout.Space(4);
                    if (GUILayout.Button("Clear All", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectsForUndo(otherSpawners.ToArray(), "Clear All Target Spawners");
                        for (var i = 0; i < otherSpawners.Count; i++) {
                            var aSpawner = otherSpawners[i];
                            aSpawner.isSpawnerSelectedAsTarget = false;
                            EditorUtility.SetDirty(aSpawner);
                        }
                    }

                    GUILayout.Space(2);
                    GUI.contentColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }

                for (var i = 0; i < otherSpawners.Count; i++) {
                    var spawner = otherSpawners[i];

                    DTInspectorUtility.StartGroupHeader(1, false);

                    EditorGUILayout.BeginHorizontal();
                    var newSelected = GUILayout.Toggle(spawner.isSpawnerSelectedAsTarget, " " + spawner.name);
                    if (newSelected != spawner.isSpawnerSelectedAsTarget) {
                        var spawnerDirty = true;
                        UndoHelper.RecordObjectPropertyForUndo(ref spawnerDirty, spawner, "toggle Spawner");
                        spawner.isSpawnerSelectedAsTarget = newSelected;
                        EditorUtility.SetDirty(spawner);
                    }

                    var buttonPressed = DTInspectorUtility.AddControlButtons("World Variable");
                    if (buttonPressed == DTInspectorUtility.FunctionButtons.Edit) {
                        Selection.activeGameObject = spawner.gameObject;
                    }

                    EditorGUILayout.EndHorizontal();
                    if (spawner.isSpawnerSelectedAsTarget) {
                        var spawnerExistingMatchWaves = "";

                        for (var s = 0; s < spawner.waveSpecs.Count; s++) {
                            var spawnerWave = spawner.waveSpecs[s];
                            for (var w = 0; w < selectedWaves.Count; w++) {
                                var aWave = selectedWaves[w];
                                if (aWave.SpawnLevelNumber == spawnerWave.SpawnLevelNumber &&
                                    aWave.SpawnWaveNumber == spawnerWave.SpawnWaveNumber) {
                                    if (spawnerExistingMatchWaves.Length > 0) {
                                        spawnerExistingMatchWaves += ", ";
                                    }
                                    spawnerExistingMatchWaves += (spawnerWave.SpawnLevelNumber + 1) + "/" +
                                                                 (spawnerWave.SpawnWaveNumber + 1);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(spawnerExistingMatchWaves)) {
                            DTInspectorUtility.ShowLargeBarAlertBox("Spawner already has settings for Level/Wave: " +
                                                                    spawnerExistingMatchWaves +
                                                                    ". You will overwrite if you copy or move.");
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Separator();


        EditorGUI.indentLevel = 0;

        var dupeMsg = _settings.CheckForDuplicateWaveLevelSettings();
        if (!string.IsNullOrEmpty(dupeMsg)) {
            DTInspectorUtility.ShowRedErrorBox(dupeMsg);
        }

        var disabledText = "";
        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never) {
            disabledText = " --DISABLED--";
        }

        var newExpanded = _settings.isExpanded;
        var text = string.Format("Wave Settings ({0}){1}", filteredWaves.Count, disabledText);

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!newExpanded) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

        text = "<b><size=11>" + text + "</size></b>";

        if (newExpanded) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            newExpanded = !newExpanded;
        }

        GUILayout.Space(2f);

        if (newExpanded != _settings.isExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Wave Settings");
            _settings.isExpanded = newExpanded;
        }
        // BUTTONS...
        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(16));

        // ReSharper disable once RedundantAssignment

        if (_settings.activeMode != LevelSettings.ActiveItemMode.Never) {
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            GUI.backgroundColor = Color.white;

            // Add expand/collapse buttons if there are items in the list
            if (_settings.waveSpecs.Count > 0) {
                var content = new GUIContent("Collapse", "Click to collapse all");
                var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));

                content = new GUIContent("Expand", "Click to expand all");
                var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));
                if (masterExpand) {
                    ExpandCollapseAll(true);
                }
                if (masterCollapse) {
                    ExpandCollapseAll(false);
                }
                if (GUILayout.Button(new GUIContent("Sort Level/Wave"), EditorStyles.toolbarButton, GUILayout.Height(16))) {
                    SortLevelWave();
                }
            } else {
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(50));
            // A little space between button groups

            var addText = string.Format("Click to add Wave{0}.", _settings.waveSpecs.Count > 0 ? " before the first" : "");
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;

            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            // Main Add button
            if (GUILayout.Button(new GUIContent("Add", addText), EditorStyles.toolbarButton, GUILayout.Height(16))) {
                if (levelSettings.LevelTimes.Count == 0) {
                    DTInspectorUtility.ShowAlert("You will not have any Level or Wave #'s to select in your Spawner Wave Settings until you add a Level in LevelSettings. Please do that first.");
                } else {
                    var newWave = new WaveSpecifics();
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave");
                    _settings.waveSpecs.Add(newWave);
                }
            }

            GUI.contentColor = Color.white;
            DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/SyncroSpawners.htm#WaveSettings");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            if (_settings.isExpanded) {
                DTInspectorUtility.BeginGroupedControls();
                EditorGUI.indentLevel = 0;

                if (_settings.waveSpecs.Count == 0) {
                    DTInspectorUtility.ShowLargeBarAlertBox("You have zero Wave Settings. Your spawner won't spawn anything.");
                }
                if (allFilteredOut) {
                    DTInspectorUtility.ShowLargeBarAlertBox("You have filtered all Waves out with Level Filter above.");
                }

                var waveToInsertAt = -1;
                WaveSpecifics waveToDelete = null;
                WaveSpecifics waveSetting;
                int? waveToMoveUp = null;
                int? waveToMoveDown = null;
                int? waveToClone = null;

                // get list of prefab pools.
                var poolNames = LevelSettings.GetSortedPrefabPoolNames();

                int? waveToVisualize = null;
                int? waveToEnable = null;
                int? changedWaveNum = null;

                for (var w = 0; w < filteredWaves.Count; w++) {
                    EditorGUI.indentLevel = 1;
                    waveSetting = filteredWaves[w];
                    var levelWave = GetLevelWaveFromWaveSpec(waveSetting);

                    DTInspectorUtility.StartGroupHeader();
                    EditorGUILayout.BeginHorizontal();

                    if (_settings.useCopyWave && !Application.isPlaying) {
                        var newSel = GUILayout.Toggle(waveSetting.isSelectedToCopyFrom, "");
                        if (newSel != waveSetting.isSelectedToCopyFrom) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle select Wave");
                            waveSetting.isSelectedToCopyFrom = newSel;
                        }
                    }

                    var sDisabled = "";
                    if (!waveSetting.isExpanded && !waveSetting.enableWave) {
                        sDisabled = " DISABLED ";
                    }

                    newExpanded = DTInspectorUtility.Foldout(waveSetting.isExpanded,
                      string.Format("Wave Setting #{0} ({1}/{2}){3}", (w + 1),
                              waveSetting.SpawnLevelNumber + 1,
                              waveSetting.SpawnWaveNumber + 1,
                              sDisabled));
                    if (newExpanded != waveSetting.isExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Wave Setting");
                        waveSetting.isExpanded = newExpanded;
                    }

                    GUILayout.FlexibleSpace();
                    var waveButtonPressed = DTInspectorUtility.AddFoldOutListItemButtons(w, _settings.waveSpecs.Count, "Wave", false, null, false, true, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    switch (waveButtonPressed) {
                        case DTInspectorUtility.FunctionButtons.Remove:
                            waveToDelete = waveSetting;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.Add:
                            waveToInsertAt = w;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.ShiftDown:
                            waveToMoveDown = w;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.ShiftUp:
                            waveToMoveUp = w;
                            _isDirty = true;
                            break;
                        case DTInspectorUtility.FunctionButtons.Copy:
                            waveToClone = w;
                            _isDirty = true;
                            break;
                    }

                    if (!waveSetting.isExpanded) {
                        EditorGUILayout.EndVertical();
                        continue;
                    }
                    EditorGUI.indentLevel = 0;

                    var newEnabled = EditorGUILayout.BeginToggleGroup(" Enable Wave", waveSetting.enableWave);
                    if (newEnabled != waveSetting.enableWave) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Enable Wave");
                        waveSetting.enableWave = newEnabled;
                        if (!Application.isPlaying) {
                            _settings.gameObject.DestroyChildrenImmediateWithMarker();
                        }

                        if (newEnabled) {
                            waveToEnable = w;
                        }
                    }

                    var newVis = EditorGUILayout.Toggle("Visualize Wave", waveSetting.visualizeWave);
                    if (newVis != waveSetting.visualizeWave) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Visualize Wave");
                        waveSetting.visualizeWave = newVis;
                        if (!newVis) {
                            _settings.gameObject.DestroyChildrenImmediateWithMarker();
                        } else {
                            changedWaveNum = w;
                            waveToVisualize = w;
                        }
                    }

                    var oldLevelNumber = waveSetting.SpawnLevelNumber;

                    var newLevel = EditorGUILayout.IntPopup("Level#", waveSetting.SpawnLevelNumber + 1, LevelNames, LevelIndexes) - 1;
                    if (newLevel != waveSetting.SpawnLevelNumber) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Level#");
                        waveSetting.SpawnLevelNumber = newLevel;

                        if (oldLevelNumber != waveSetting.SpawnLevelNumber) {
                            waveSetting.SpawnWaveNumber = 0;
                        }
                    }

                    var newWave = EditorGUILayout.IntPopup("Wave#", waveSetting.SpawnWaveNumber + 1,
                        WaveNamesForLevel(waveSetting.SpawnLevelNumber), WaveIndexesForLevel(waveSetting.SpawnLevelNumber)) - 1;
                    if (newWave != waveSetting.SpawnWaveNumber) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave#");
                        waveSetting.SpawnWaveNumber = newWave;
                    }

                    var oldInt = waveSetting.MinToSpwn.Value;

                    KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.MinToSpwn, "Min To Spawn", _settings);
                    if (oldInt != waveSetting.MinToSpwn.Value) {
                        changedWaveNum = w;
                    }

                    KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.MaxToSpwn, "Max To Spawn", _settings);

                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.TimeToSpawnEntireWave, "Time To Spawn All", _settings);

                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.WaveDelaySec, "Delay Wave (sec)", _settings);

                    if (waveSetting.repeatWaveUntilNew) {
                        var newUseDelayOnRepeat = EditorGUILayout.Toggle("Use Wave Delay on Repeats", waveSetting.doesRepeatUseWaveDelay);
                        if (newUseDelayOnRepeat != waveSetting.doesRepeatUseWaveDelay) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Wave Delay on Repeats");
                            waveSetting.doesRepeatUseWaveDelay = newUseDelayOnRepeat;
                        }
                    }

                    if (levelWave.waveType == LevelSettings.WaveType.Elimination) {
                        var newComplete = EditorGUILayout.IntSlider("Wave Completion %", waveSetting.waveCompletePercentage, 1, 100);
                        if (newComplete != waveSetting.waveCompletePercentage) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Wave Completion %");
                            waveSetting.waveCompletePercentage = newComplete;
                        }
                    }

                    DTInspectorUtility.StartGroupHeader(1);

                    var newSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Prefab Type", waveSetting.spawnSource);
                    if (newSource != waveSetting.spawnSource) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Type");
                        waveSetting.spawnSource = newSource;
                        changedWaveNum = w;
                    }
                    EditorGUILayout.EndVertical();
                    switch (waveSetting.spawnSource) {
                        case WaveSpecifics.SpawnOrigin.Specific:
                            var wasDirty = _isDirty;
                            PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _settings, ref waveSetting.prefabToSpawn, ref waveSetting.prefabToSpawnCategoryName, "Prefab To Spawn");

                            if (!wasDirty && _isDirty) {
                                changedWaveNum = w;
                            }

                            if (!_isDirty && changedWaveNum.HasValue) {
                                changedWaveNum = w;
                            }

                            if (waveSetting.prefabToSpawn == null) {
                                DTInspectorUtility.ShowRedErrorBox("Please specify a prefab to spawn.");
                            }
                            break;
                        case WaveSpecifics.SpawnOrigin.PrefabPool:
                            if (poolNames != null) {
                                var pool = LevelSettings.GetFirstMatchingPrefabPool(waveSetting.prefabPoolName);
                                var noPoolSelected = false;
                                var illegalPool = false;
                                var noPools = false;

                                if (pool == null) {
                                    if (string.IsNullOrEmpty(waveSetting.prefabPoolName)) {
                                        noPoolSelected = true;
                                    } else {
                                        illegalPool = true;
                                    }
                                    waveSetting.prefabPoolIndex = 0;
                                } else {
                                    waveSetting.prefabPoolIndex = poolNames.IndexOf(waveSetting.prefabPoolName);
                                }

                                if (poolNames.Count > 1) {
                                    EditorGUILayout.BeginHorizontal();
                                    var newPoolIndex = EditorGUILayout.Popup("Prefab Pool", waveSetting.prefabPoolIndex, poolNames.ToArray());
                                    if (newPoolIndex != waveSetting.prefabPoolIndex) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Pool");
                                        waveSetting.prefabPoolIndex = newPoolIndex;
                                    }

                                    if (waveSetting.prefabPoolIndex > 0) {
                                        var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[waveSetting.prefabPoolIndex]);
                                        if (matchingPool != null) {
                                            waveSetting.prefabPoolName = matchingPool.name;
                                        }
                                    } else {
                                        waveSetting.prefabPoolName = string.Empty;
                                    }

                                    if (newPoolIndex > 0) {
                                        if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                            DTInspectorUtility.FunctionButtons.Edit) {
                                            Selection.activeGameObject = pool.gameObject;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                } else {
                                    noPools = true;
                                }

                                if (noPools) {
                                    DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                                } else if (noPoolSelected) {
                                    DTInspectorUtility.ShowRedErrorBox("No Prefab Pool selected.");
                                } else if (illegalPool) {
                                    DTInspectorUtility.ShowRedErrorBox("Prefab Pool '" + waveSetting.prefabPoolName + "' not found. Select one.");
                                }
                            } else {
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                                DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                            }

                            break;
                    }
                    EditorGUILayout.EndVertical();
                    DTInspectorUtility.AddSpaceForNonU5();

                    DTInspectorUtility.StartGroupHeader(1);

                    EditorGUI.indentLevel = 1;
                    var newEx = DTInspectorUtility.Foldout(waveSetting.positionExpanded, " Position Settings");
                    if (newEx != waveSetting.positionExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Position Settings");
                        waveSetting.positionExpanded = newEx;
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel = 0;

                    if (waveSetting.positionExpanded) {
                        var newX = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("X Position Mode", waveSetting.positionXmode);
                        if (newX != waveSetting.positionXmode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change X Position Mode");
                            waveSetting.positionXmode = newX;
                            changedWaveNum = w;
                        }

                        Transform otherObj;

                        switch (waveSetting.positionXmode) {
                            case WaveSpecifics.PositionMode.CustomPosition:
                                var oldX = waveSetting.customPosX.Value;
                                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosX, "X Position", _settings);
                                if (oldX != waveSetting.customPosX.Value) {
                                    changedWaveNum = w;
                                }
                                break;
                            case WaveSpecifics.PositionMode.OtherObjectPosition:
                                otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectX, typeof(Transform), true);
                                if (waveSetting.otherObjectX != otherObj) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                                    waveSetting.otherObjectX = otherObj;
                                }
                                if (waveSetting.otherObjectX == null) {
                                    DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                                }
                                DTInspectorUtility.VerticalSpace(4);
                                break;
                        }

                        var newY = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Y Position Mode", waveSetting.positionYmode);
                        if (newY != waveSetting.positionYmode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Y Position Mode");
                            waveSetting.positionYmode = newY;
                            changedWaveNum = w;
                        }

                        switch (waveSetting.positionYmode) {
                            case WaveSpecifics.PositionMode.CustomPosition:
                                var oldY = waveSetting.customPosY.Value;
                                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosY, "Y Position", _settings);
                                if (oldY != waveSetting.customPosY.Value) {
                                    changedWaveNum = w;
                                }
                                break;
                            case WaveSpecifics.PositionMode.OtherObjectPosition:
                                otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectY, typeof(Transform), true);
                                if (waveSetting.otherObjectY != otherObj) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                                    waveSetting.otherObjectY = otherObj;
                                }
                                if (waveSetting.otherObjectY == null) {
                                    DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                                }

                                DTInspectorUtility.VerticalSpace(4);
                                break;
                        }

                        var newZ = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Z Position Mode", waveSetting.positionZmode);
                        if (newZ != waveSetting.positionZmode) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Z Position Mode");
                            waveSetting.positionZmode = newZ;
                            changedWaveNum = w;
                        }

                        switch (waveSetting.positionZmode) {
                            case WaveSpecifics.PositionMode.CustomPosition:
                                var oldZ = waveSetting.customPosZ.Value;
                                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosZ, "Z Position", _settings);
                                if (oldZ != waveSetting.customPosZ.Value) {
                                    changedWaveNum = w;
                                }
                                break;
                            case WaveSpecifics.PositionMode.OtherObjectPosition:
                                otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectZ, typeof(Transform), true);
                                if (waveSetting.otherObjectZ != otherObj) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                                    waveSetting.otherObjectZ = otherObj;
                                }
                                if (waveSetting.otherObjectZ == null) {
                                    DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                                }
                                break;
                        }

                        if (waveSetting.waveOffsetList.Count == 0) {
                            waveSetting.waveOffsetList.Add(new Vector3());
                            _isDirty = true;
                            changedWaveNum = w;
                        }

                        DTInspectorUtility.StartGroupHeader();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Wave Offsets");

                        if (!Application.isPlaying) {
                            GUI.contentColor = DTInspectorUtility.AddButtonColor;
                            if (GUILayout.Button(new GUIContent("Add", "Add a new Wave Offset"),
                                EditorStyles.toolbarButton, GUILayout.MaxWidth(50))) {
                                waveSetting.waveOffsetList.Add(new Vector3());
                                _isDirty = true;
                                changedWaveNum = w;
                            }
                            GUI.contentColor = Color.white;
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();

                        if (!Application.isPlaying) {
                            var newMode = (WaveSpecifics.WaveOffsetChoiceMode)EditorGUILayout.EnumPopup("Offset Selection", waveSetting.offsetChoiceMode);
                            if (newMode != waveSetting.offsetChoiceMode) {
                                _isDirty = true;
                                waveSetting.offsetChoiceMode = newMode;
                            }
                        }

                        int? itemToDelete = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < waveSetting.waveOffsetList.Count; i++) {
                            var anOffset = waveSetting.waveOffsetList[i];
                            EditorGUILayout.BeginHorizontal();

                            var newOffset = EditorGUILayout.Vector3Field("Wave Offset #" + (i + 1), anOffset);

                            var btn = DTInspectorUtility.FunctionButtons.None;

                            if (!Application.isPlaying) {
                                btn = DTInspectorUtility.AddCustomEventIcons(false, false, false, false, "Wave Offset");
                            }

                            EditorGUILayout.EndHorizontal();

                            if (btn == DTInspectorUtility.FunctionButtons.Remove) {
                                itemToDelete = i;
                            }

                            if (newOffset == anOffset) {
                                continue;
                            }

                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Offset");
                            waveSetting.waveOffsetList[i] = newOffset;
                            changedWaveNum = w;
                        }
                        EditorGUILayout.EndVertical();

                        if (itemToDelete.HasValue) {
                            waveSetting.waveOffsetList.RemoveAt(itemToDelete.Value);
                            _isDirty = true;
                            changedWaveNum = w;
                        }
                    }

                    EditorGUILayout.EndVertical();
                    DTInspectorUtility.AddSpaceForNonU5();

                    DTInspectorUtility.StartGroupHeader(1);
                    var newRotation = (WaveSpecifics.RotationMode)EditorGUILayout.EnumPopup("Spawn Rotation Mode", waveSetting.curRotationMode);
                    if (newRotation != waveSetting.curRotationMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Rotation Mode");
                        waveSetting.curRotationMode = newRotation;
                        changedWaveNum = w;
                    }
                    EditorGUILayout.EndVertical();

                    if (waveSetting.curRotationMode == WaveSpecifics.RotationMode.CustomRotation) {
                        var newCust = EditorGUILayout.Vector3Field("Custom Rotation Euler", waveSetting.customRotation);
                        if (newCust != waveSetting.customRotation) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Rotation Euler");
                            waveSetting.customRotation = newCust;
                            changedWaveNum = w;
                        }
                    }
                    EditorGUILayout.EndVertical();
                    DTInspectorUtility.AddSpaceForNonU5();

                    DTInspectorUtility.StartGroupHeader(1);
                    newExpanded = EditorGUILayout.BeginToggleGroup(" Spawn Limit Controls", waveSetting.enableLimits);
                    if (newExpanded != waveSetting.enableLimits) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Limit Controls");
                        waveSetting.enableLimits = newExpanded;
                    }
                    DTInspectorUtility.EndGroupHeader();
                    if (waveSetting.enableLimits) {
                        DTInspectorUtility.ShowColorWarningBox("Stop spawning until all spawns from wave satisfy:");

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.doNotSpawnIfMbrCloserThan, "Min. Distance", _settings);

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.doNotSpawnRandomDist, "Random Distance", _settings);
                    }
                    EditorGUILayout.EndToggleGroup();
                    DTInspectorUtility.AddSpaceForNonU5();

                    if (levelWave.waveType == LevelSettings.WaveType.Elimination) {
                        DTInspectorUtility.AddSpaceForNonU5(2);
                        DTInspectorUtility.StartGroupHeader(1);
                        EditorGUI.indentLevel = 0;
                        // beat level Custom Events to fire
                        var newLastSpawn = EditorGUILayout.BeginToggleGroup(" Wave Elimination Bonus Prefab",
                            waveSetting.useSpawnBonusPrefab);
                        if (newLastSpawn != waveSetting.useSpawnBonusPrefab) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Elimination Bonus Prefab");
                            waveSetting.useSpawnBonusPrefab = newLastSpawn;
                        }
                        DTInspectorUtility.EndGroupHeader();

                        if (waveSetting.useSpawnBonusPrefab) {
                            if (waveSetting.repeatWaveUntilNew) {
                                var newRepeatUsage = (LevelSettings.RepeatToUseItem) EditorGUILayout.EnumPopup("Use On Wave Repeats", waveSetting.bonusRepeatToUseItem);
                                if (newRepeatUsage != waveSetting.bonusRepeatToUseItem) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings,
                                        "change Use On Wave Repeats");
                                    waveSetting.bonusRepeatToUseItem = newRepeatUsage;
                                }
                            }

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
                        }
                        EditorGUILayout.EndToggleGroup();
                    }

                    DTInspectorUtility.StartGroupHeader(1);

                    newExpanded = EditorGUILayout.BeginToggleGroup(" Repeat Wave", waveSetting.repeatWaveUntilNew);
                    if (newExpanded != waveSetting.repeatWaveUntilNew) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Wave");
                        waveSetting.repeatWaveUntilNew = newExpanded;
                    }
                    DTInspectorUtility.EndGroupHeader();
                    if (waveSetting.repeatWaveUntilNew) {
                        if (levelWave.waveType == LevelSettings.WaveType.Elimination) {
                            var newRepeatMode = (WaveSpecifics.RepeatWaveMode)EditorGUILayout.EnumPopup("Repeat Mode", waveSetting.curWaveRepeatMode);
                            if (newRepeatMode != waveSetting.curWaveRepeatMode) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Repeat Mode");
                                waveSetting.curWaveRepeatMode = newRepeatMode;
                            }
                        } else {
                            // only one mode for non-elimination waves.
                            var newRepeatMode = (WaveSpecifics.TimedRepeatWaveMode)EditorGUILayout.EnumPopup("Timed Repeat Mode", waveSetting.curTimedRepeatWaveMode);
                            if (newRepeatMode != waveSetting.curTimedRepeatWaveMode) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Timed Repeat Mode");
                                waveSetting.curTimedRepeatWaveMode = newRepeatMode;
                            }
                        }

                        switch (waveSetting.curWaveRepeatMode) {
                            case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
                                if (levelWave.waveType == LevelSettings.WaveType.Elimination) {
                                    KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repetitionsToDo, "Repetitions", _settings);
                                }
                                break;
                            case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
                            case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
                                if (levelWave.waveType != LevelSettings.WaveType.Elimination) {
                                    break;
                                }

                                var missingStatNames = new List<string>();
                                missingStatNames.AddRange(allStats);
                                missingStatNames.RemoveAll(delegate (string obj) {
                                    return waveSetting.repeatPassCriteria.HasKey(obj);
                                });

                                var newStat = EditorGUILayout.Popup("Add Variable Limit", 0, missingStatNames.ToArray());
                                if (newStat != 0) {
                                    AddStatModifier(missingStatNames[newStat], waveSetting);
                                }

                                if (waveSetting.repeatPassCriteria.statMods.Count == 0) {
                                    DTInspectorUtility.ShowRedErrorBox("You have no Variable Limits. Wave will not repeat.");
                                } else {
                                    EditorGUILayout.Separator();

                                    int? indexToDelete = null;

                                    for (var i = 0; i < waveSetting.repeatPassCriteria.statMods.Count; i++) {
                                        var modifier = waveSetting.repeatPassCriteria.statMods[i];
                                        var buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
                                        if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
                                            indexToDelete = i;
                                        }
                                    }

                                    DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                                    if (indexToDelete.HasValue) {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Limit");
                                        waveSetting.repeatPassCriteria.DeleteByIndex(indexToDelete.Value);
                                    }

                                    EditorGUILayout.Separator();
                                }
                                break;
                        }

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatPauseMinimum, "Repeat Pause Min", _settings);

                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatPauseMaximum, "Repeat Pause Max", _settings);

                        DTInspectorUtility.VerticalSpace(3);
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemInc, "Spawn Increase", _settings);
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemMinLmt, "Spawn Min Limit", _settings);
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemLmt, "Spawn Max Limit", _settings);

                        var reset = EditorGUILayout.Toggle("Reset On Spawn Lmt Passed", waveSetting.resetOnItemLimitReached);
                        if (reset != waveSetting.resetOnItemLimitReached) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Spawn Lmt Passed");
                            waveSetting.resetOnItemLimitReached = reset;
                        }

                        DTInspectorUtility.VerticalSpace(3);
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeInc, "Time Increase", _settings);
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeMinLmt, "Time Min Limit", _settings);
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeLmt, "Time Max Limit", _settings);
                        reset = EditorGUILayout.Toggle("Reset On Time Lmt Passed", waveSetting.resetOnTimeLimitReached);
                        if (reset != waveSetting.resetOnTimeLimitReached) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Time Lmt Passed");
                            waveSetting.resetOnTimeLimitReached = reset;
                        }


                        // repeat wave variable modifiers
                        DTInspectorUtility.StartGroupHeader(0, true);
                        var newBonusesEnabled = EditorGUILayout.Toggle("Repeat Bonus", waveSetting.waveRepeatBonusesEnabled);
                        if (newBonusesEnabled != waveSetting.waveRepeatBonusesEnabled) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Bonus");
                            waveSetting.waveRepeatBonusesEnabled = newBonusesEnabled;
                        }

                        EditorGUILayout.EndVertical();

                        if (waveSetting.waveRepeatBonusesEnabled) {
                            EditorGUI.indentLevel = 1;

                            var missingBonusStatNames = new List<string>();
                            missingBonusStatNames.AddRange(allStats);
                            missingBonusStatNames.RemoveAll(delegate (string obj) {
                                return waveSetting.waveRepeatVariableModifiers.HasKey(obj);
                            });

                            var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingBonusStatNames.ToArray());
                            if (newBonusStat != 0) {
                                AddBonusStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
                            }

                            if (waveSetting.waveRepeatVariableModifiers.statMods.Count == 0) {
                                if (waveSetting.waveRepeatBonusesEnabled) {
                                    DTInspectorUtility.ShowColorWarningBox("You currently are using no modifiers for this wave.");
                                }
                            } else {
                                EditorGUILayout.Separator();

                                int? indexToDelete = null;

                                EditorGUI.indentLevel = 0;
                                for (var i = 0; i < waveSetting.waveRepeatVariableModifiers.statMods.Count; i++) {
                                    var modifier = waveSetting.waveRepeatVariableModifiers.statMods[i];

                                    var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                                    switch (modifier._varTypeToUse) {
                                        case WorldVariableTracker.VariableType._integer:
                                            buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
                                            break;
                                        case WorldVariableTracker.VariableType._float:
                                            buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _settings, true, true);
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

                                if (indexToDelete.HasValue) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Variable Modifier");
                                    waveSetting.waveRepeatVariableModifiers.DeleteByIndex(indexToDelete.Value);
                                }

                                EditorGUILayout.Separator();
                            }
                        }
                        EditorGUILayout.EndVertical();

                        DTInspectorUtility.AddSpaceForNonU5();

                        DTInspectorUtility.StartGroupHeader(0, true);
                        var newExp = EditorGUILayout.Toggle("Repeat Cust. Events", waveSetting.waveRepeatFireEvents);
                        if (newExp != waveSetting.waveRepeatFireEvents) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Cust. Events");
                            waveSetting.waveRepeatFireEvents = newExp;
                        }

                        EditorGUILayout.EndVertical();

                        if (waveSetting.waveRepeatFireEvents) {
                            DTInspectorUtility.ShowColorWarningBox("When wave repeats, fire the Custom Events below");

                            EditorGUILayout.BeginHorizontal();
                            GUI.contentColor = DTInspectorUtility.AddButtonColor;
                            GUILayout.Space(10);
                            if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Repeat Custom Event");
                                waveSetting.waveRepeatCustomEvents.Add(new CGKCustomEventToFire());
                            }
                            GUI.contentColor = Color.white;

                            EditorGUILayout.EndHorizontal();

                            if (waveSetting.waveRepeatCustomEvents.Count == 0) {
                                DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                            }

                            DTInspectorUtility.VerticalSpace(2);

                            int? indexToDelete = null;

                            // ReSharper disable once ForCanBeConvertedToForeach
                            for (var i = 0; i < waveSetting.waveRepeatCustomEvents.Count; i++) {
                                var anEvent = waveSetting.waveRepeatCustomEvents[i].CustomEventName;

                                var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                                anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
                                    _settings, "Custom Event", ref buttonClicked);
                                if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove) {
                                    indexToDelete = i;
                                }

                                if (anEvent == waveSetting.waveRepeatCustomEvents[i].CustomEventName) {
                                    continue;
                                }

                                waveSetting.waveRepeatCustomEvents[i].CustomEventName = anEvent;
                            }

                            if (indexToDelete.HasValue) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Wave Repeat Custom Event");
                                waveSetting.waveRepeatCustomEvents.RemoveAt(indexToDelete.Value);
                            }
                        }
                        EditorGUILayout.EndVertical();
                        if (waveSetting.waveRepeatCustomEvents.Count > 0) {
                            DTInspectorUtility.VerticalSpace(2);
                        }
                    }
                    EditorGUILayout.EndToggleGroup();
                    DTInspectorUtility.AddSpaceForNonU5();

                    EditorGUI.indentLevel = 0;
                    // show randomizations
                    const string variantTag = " Randomization";

                    DTInspectorUtility.StartGroupHeader(1);
                    newExpanded = EditorGUILayout.BeginToggleGroup(variantTag, waveSetting.enableRandomizations);
                    if (newExpanded != waveSetting.enableRandomizations) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Randomization");
                        waveSetting.enableRandomizations = newExpanded;
                        changedWaveNum = w;
                    }
                    DTInspectorUtility.EndGroupHeader();
                    if (waveSetting.enableRandomizations) {
                        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
                        EditorGUILayout.LabelField("Random Rotation");

                        var newRandX = GUILayout.Toggle(waveSetting.randomXRotation, "X");
                        if (newRandX != waveSetting.randomXRotation) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Rotation X");
                            waveSetting.randomXRotation = newRandX;
                            changedWaveNum = w;
                        }
                        GUILayout.Space(10);

                        var newRandY = GUILayout.Toggle(waveSetting.randomYRotation, "Y");
                        if (newRandY != waveSetting.randomYRotation) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Rotation Y");
                            waveSetting.randomYRotation = newRandY;
                            changedWaveNum = w;
                        }
                        GUILayout.Space(10);

                        var newRandZ = GUILayout.Toggle(waveSetting.randomZRotation, "Z");
                        if (newRandZ != waveSetting.randomZRotation) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Rotation Z");
                            waveSetting.randomZRotation = newRandZ;
                            changedWaveNum = w;
                        }
                        EditorGUILayout.EndHorizontal();

                        if (waveSetting.randomXRotation) {
                            var randX = waveSetting.randomXRotMin.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMin, "Rand. X Rot. Min", _settings);
                            if (randX != waveSetting.randomXRotMin.Value) {
                                changedWaveNum = w;
                            }

                            randX = waveSetting.randomXRotMax.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMax, "Rand. X Rot. Max", _settings);
                            if (randX != waveSetting.randomXRotMax.Value) {
                                changedWaveNum = w;
                            }
                        }

                        if (waveSetting.randomYRotation) {
                            var randY = waveSetting.randomYRotMin.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMin, "Rand. Y Rot. Min", _settings);
                            if (randY != waveSetting.randomYRotMin.Value) {
                                changedWaveNum = w;
                            }

                            randY = waveSetting.randomYRotMax.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMax, "Rand. Y Rot. Max", _settings);
                            if (randY != waveSetting.randomYRotMax.Value) {
                                changedWaveNum = w;
                            }
                        }

                        if (waveSetting.randomZRotation) {
                            var randZ = waveSetting.randomZRotMin.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMin, "Rand. Z Rot. Min", _settings);
                            if (randZ != waveSetting.randomZRotMin.Value) {
                                changedWaveNum = w;
                            }

                            randZ = waveSetting.randomZRotMax.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMax, "Rand. Z Rot. Max", _settings);
                            if (randZ != waveSetting.randomZRotMax.Value) {
                                changedWaveNum = w;
                            }
                        }

                        EditorGUILayout.Separator();

                        var rndX = waveSetting.randomDistX.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistX, "Rand. Distance X", _settings);
                        if (rndX != waveSetting.randomDistX.Value) {
                            changedWaveNum = w;
                        }

                        var rndY = waveSetting.randomDistY.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistY, "Rand. Distance Y", _settings);
                        if (rndY != waveSetting.randomDistY.Value) {
                            changedWaveNum = w;
                        }

                        var rndZ = waveSetting.randomDistZ.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistZ, "Rand. Distance Z", _settings);
                        if (rndZ != waveSetting.randomDistZ.Value) {
                            changedWaveNum = w;
                        }
                    }
                    EditorGUILayout.EndToggleGroup();
                    DTInspectorUtility.AddSpaceForNonU5();

                    // show increments
                    var incTag = " Incremental Settings";
                    DTInspectorUtility.StartGroupHeader(1);
                    newExpanded = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enableIncrements);
                    if (newExpanded != waveSetting.enableIncrements) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Incremental Settings");
                        waveSetting.enableIncrements = newExpanded;
                        changedWaveNum = w;
                    }
                    DTInspectorUtility.EndGroupHeader();
                    if (waveSetting.enableIncrements) {
                        var oldX = waveSetting.incrementPositionX.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionX, "Distance X", _settings);
                        if (oldX != waveSetting.incrementPositionX.Value) {
                            changedWaveNum = w;
                        }

                        var oldY = waveSetting.incrementPositionY.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionY, "Distance Y", _settings);
                        if (oldY != waveSetting.incrementPositionY.Value) {
                            changedWaveNum = w;
                        }

                        var oldZ = waveSetting.incrementPositionZ.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionZ, "Distance Z", _settings);
                        if (oldZ != waveSetting.incrementPositionZ.Value) {
                            changedWaveNum = w;
                        }

                        EditorGUILayout.Separator();

                        if (waveSetting.enableRandomizations && waveSetting.randomXRotation) {
                            DTInspectorUtility.ShowColorWarningBox("Rotation X - cannot be used with Random Rotation X.");
                        } else {
                            var rotX = waveSetting.incrementRotX.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotX, "Rotation X", _settings);
                            if (rotX != waveSetting.incrementRotX.Value) {
                                changedWaveNum = w;
                            }
                        }

                        if (waveSetting.enableRandomizations && waveSetting.randomYRotation) {
                            DTInspectorUtility.ShowColorWarningBox("Rotation Y - cannot be used with Random Rotation Y.");
                        } else {
                            var rotY = waveSetting.incrementRotY.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotY, "Rotation Y", _settings);
                            if (rotY != waveSetting.incrementRotY.Value) {
                                changedWaveNum = w;
                            }
                        }

                        if (waveSetting.enableRandomizations && waveSetting.randomZRotation) {
                            DTInspectorUtility.ShowColorWarningBox("Rotation Z - cannot be used with Random Rotation Z.");
                        } else {
                            var rotZ = waveSetting.incrementRotZ.Value;
                            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotZ, "Rotation Z", _settings);
                            if (rotZ != waveSetting.incrementRotZ.Value) {
                                changedWaveNum = w;
                            }
                        }

                        var newIncKc = EditorGUILayout.Toggle("Keep Center", waveSetting.enableKeepCenter);
                        if (newIncKc != waveSetting.enableKeepCenter) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Keep Center");
                            waveSetting.enableKeepCenter = newIncKc;
                            changedWaveNum = w;
                        }
                    }
                    EditorGUILayout.EndToggleGroup();

                    DTInspectorUtility.AddSpaceForNonU5();

                    // show increments
                    incTag = " Post-spawn Nudge Settings";
                    DTInspectorUtility.StartGroupHeader(1);
                    newExpanded = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enablePostSpawnNudge);
                    if (newExpanded != waveSetting.enablePostSpawnNudge) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Post-spawn Nudge Settings");
                        waveSetting.enablePostSpawnNudge = newExpanded;
                        changedWaveNum = w;
                    }
                    DTInspectorUtility.EndGroupHeader();
                    if (waveSetting.enablePostSpawnNudge) {
                        var oldF = waveSetting.postSpawnNudgeFwd.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeFwd, "Nudge Forward", _settings);
                        if (oldF != waveSetting.postSpawnNudgeFwd.Value) {
                            changedWaveNum = w;
                        }

                        var oldR = waveSetting.postSpawnNudgeRgt.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeRgt, "Nudge Right", _settings);
                        if (oldR != waveSetting.postSpawnNudgeRgt.Value) {
                            changedWaveNum = w;
                        }

                        var oldD = waveSetting.postSpawnNudgeDwn.Value;
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeDwn, "Nudge Down", _settings);
                        if (oldD != waveSetting.postSpawnNudgeDwn.Value) {
                            changedWaveNum = w;
                        }
                    }
                    EditorGUILayout.EndToggleGroup();

                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.EndVertical();

                    DTInspectorUtility.VerticalSpace(3);
                }

                if (!Application.isPlaying && !DTInspectorUtility.IsPrefabInProjectView(_settings)) {
                    if (waveToVisualize.HasValue) {
                        for (var w = 0; w < _settings.waveSpecs.Count; w++) {
                            if (w != waveToVisualize.Value) {
                                _settings.waveSpecs[w].visualizeWave = false;
                            }
                        }
                    }

                    WaveSpecifics wave = null;
                    if (changedWaveNum.HasValue) {
                        wave = _settings.waveSpecs[changedWaveNum.Value];
                    }
                    if (waveToEnable.HasValue) {
                        wave = _settings.waveSpecs[waveToEnable.Value];
                    }

                    var hasUnrenderedVisualWave = false;
                    if (wave == null) {
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var w = 0; w < _settings.waveSpecs.Count; w++) {
                            var oneWave = _settings.waveSpecs[w];
                            if (!oneWave.visualizeWave) {
                                continue;
                            }

                            if (_settings.transform.childCount != 0 || !oneWave.enableWave || oneWave.MinToSpwn.Value <= 0) {
                                continue;
                            }

                            hasUnrenderedVisualWave = true;
                            break;
                        }
                    }

                    if (waveActivated || hasUnrenderedVisualWave) {
                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var w = 0; w < _settings.waveSpecs.Count; w++) {
                            if (!_settings.waveSpecs[w].visualizeWave) {
                                continue;
                            }
                            wave = _settings.waveSpecs[w];
                            break;
                        }
                    }

                    if (wave != null) {
                        if (wave.visualizeWave) {
                            _settings.gameObject.DestroyChildrenImmediateWithMarker();
                            _settings.SpawnWaveVisual(wave);
                        }
                    }
                }

                if (waveToDelete != null) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Wave");
                    _settings.waveSpecs.Remove(waveToDelete);

                    if (!Application.isPlaying) {
                        _settings.gameObject.DestroyChildrenImmediateWithMarker();
                    }
                }

                if (waveToInsertAt > -1) {
                    if (levelSettings.LevelTimes.Count == 0) {
                        DTInspectorUtility.ShowAlert("You will not have any Level or Wave #'s to select in your Spawner Wave Settings until you add a Level in LevelSettings. Please do that first.");
                    } else {
                        var newWave = new WaveSpecifics();
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave");
                        _settings.waveSpecs.Insert(waveToInsertAt + 1, newWave);
                    }
                }

                if (waveToMoveUp.HasValue) {
                    var item = _settings.waveSpecs[waveToMoveUp.Value];
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift up Wave");
                    _settings.waveSpecs.Insert(waveToMoveUp.Value - 1, item);
                    _settings.waveSpecs.RemoveAt(waveToMoveUp.Value + 1);
                }

                if (waveToMoveDown.HasValue) {
                    var index = waveToMoveDown.Value + 1;

                    var item = _settings.waveSpecs[index];
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift down Wave");
                    _settings.waveSpecs.Insert(index - 1, item);
                    _settings.waveSpecs.RemoveAt(index + 1);
                }

                if (waveToClone.HasValue) {
                    var index = waveToClone.Value;

                    var newItem = CloningHelper.CloneWave(_settings.waveSpecs[index]);
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "clone Wave");
                    _settings.waveSpecs.Insert(index, newItem);
                }

                DTInspectorUtility.EndGroupedControls();
            }
        } else {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void CopyWavesToTarget(List<WaveSpecifics> sourceWaves, List<WaveSyncroPrefabSpawner> targetSpawners, bool deleteFromSource) {
        var undoActionName = deleteFromSource ? "Move Waves To Spawners" : "Copy Waves To Spawners";
        UndoHelper.RecordObjectsForUndo(targetSpawners.ToArray(), undoActionName);

        for (var w = 0; w < sourceWaves.Count; w++) {
            var wave = sourceWaves[w];
            for (var s = 0; s < targetSpawners.Count; s++) {
                var spawner = targetSpawners[s];
                var existingMatchWave = spawner.waveSpecs.Find(delegate (WaveSpecifics spec) {
                    return spec.SpawnLevelNumber == wave.SpawnLevelNumber && spec.SpawnWaveNumber == wave.SpawnWaveNumber;
                });
                if (existingMatchWave != null) {
                    spawner.waveSpecs.Remove(existingMatchWave);
                }
                var copiedWave = CloningHelper.CloneWave(wave);
                spawner.waveSpecs.Add(copiedWave);

                EditorUtility.SetDirty(spawner);
            }
        }

        Debug.Log("Copied " + sourceWaves.Count + " Wave(s) to " + targetSpawners.Count + " Spawner(s).");
    }

    private void AddStatModifier(string modifierName, WaveSpecifics spec) {
        if (spec.repeatPassCriteria.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This wave already has a Variable Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Variable Limit");

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        spec.repeatPassCriteria.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
    }

    private void ExpandCollapseAll(bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand / collapse Wave Settings");

        foreach (var wave in _settings.waveSpecs) {
            wave.isExpanded = isExpand;
        }
    }

    private static string[] LevelNames {
        get {
            var names = new string[LevelSettings.Instance.LevelTimes.Count];
            for (var i = 0; i < LevelSettings.Instance.LevelTimes.Count; i++) {
                names[i] = (i + 1).ToString();
            }

            return names;
        }
    }

    private static int[] LevelIndexes {
        get {
            var indexes = new int[LevelSettings.Instance.LevelTimes.Count];

            for (var i = 0; i < LevelSettings.Instance.LevelTimes.Count; i++) {
                indexes[i] = i + 1;
            }

            return indexes;
        }
    }

    private static string[] WaveNamesForLevel(int levelNumber) {
        if (LevelSettings.Instance.LevelTimes.Count <= levelNumber) {
            return new string[0];
        }

        var level = LevelSettings.Instance.LevelTimes[levelNumber];
        var names = new string[level.WaveSettings.Count];

        for (var i = 0; i < level.WaveSettings.Count; i++) {
            names[i] = (i + 1).ToString();
        }

        return names;
    }

    private static int[] WaveIndexesForLevel(int levelNumber) {
        if (LevelSettings.Instance.LevelTimes.Count <= levelNumber) {
            return new int[0];
        }

        var level = LevelSettings.Instance.LevelTimes[levelNumber];
        var indexes = new int[level.WaveSettings.Count];

        for (var i = 0; i < level.WaveSettings.Count; i++) {
            indexes[i] = i + 1;
        }

        return indexes;
    }

    private static LevelWave GetLevelWaveFromWaveSpec(WaveSpecifics waveSpec) {
        var levelNumber = waveSpec.SpawnLevelNumber;
        var waveNumber = waveSpec.SpawnWaveNumber;

        if (LevelSettings.Instance.LevelTimes.Count <= levelNumber) {
            return null;
        }

        var wave = LevelSettings.Instance.LevelTimes[levelNumber].WaveSettings[waveNumber];
        return wave;
    }

    private void AddActiveLimit(string modifierName) {
        if (_settings.activeItemCriteria.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This item already has a Active Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Active Limit");

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        _settings.activeItemCriteria.statMods.Add(new WorldVariableRange(modifierName, myVar.varType));
    }

    private void SortLevelWave() {
        _settings.waveSpecs.Sort(delegate (WaveSpecifics a, WaveSpecifics b) {
            var levelComp = a.SpawnLevelNumber.CompareTo(b.SpawnLevelNumber);
            if (levelComp == 0) {
                return a.SpawnWaveNumber.CompareTo(b.SpawnWaveNumber);
            }

            return levelComp;
        });
    }

    private void AddBonusStatModifier(string modifierName, WaveSpecifics waveSpec) {
        if (waveSpec.waveRepeatVariableModifiers.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Repeat Bonus modifier");

        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

        waveSpec.waveRepeatVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    }
}