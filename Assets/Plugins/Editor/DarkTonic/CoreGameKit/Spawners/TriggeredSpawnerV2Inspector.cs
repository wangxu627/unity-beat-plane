using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
using UnityEngine.UI;
#endif

[CustomEditor(typeof(TriggeredSpawnerV2), true)]
// ReSharper disable once CheckNamespace
public class TriggeredSpawnerV2Inspector : Editor
{
    private TriggeredSpawnerV2 _settings;
    private List<string> _allStats;
    private bool _isDirty;
    private bool _levelSettingsInScene;
    private List<string> _customEventNames;
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    private bool _hasSlider;
    private bool _hasButton;
    private bool _hasRect;
    private TriggeredWaveSpecifics _waveToVisualize;
    private TriggeredWaveSpecifics _changedWave;
    private List<TriggeredWaveSpecifics> _allWaves = new List<TriggeredWaveSpecifics>();

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI()
    {
        _settings = (TriggeredSpawnerV2)target;

        WorldVariableTracker.ClearInGamePlayerStats();

        _allStats = KillerVariablesHelper.AllStatNames;

        LevelSettings.Instance = null; // clear cached version

        var ls = LevelSettings.Instance;

        _levelSettingsInScene = ls != null;

        if (_levelSettingsInScene)
        {
            // ReSharper disable once PossibleNullReferenceException
            _customEventNames = ls.CustomEventNames;
        }

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        var showNewUIEvents = _settings.unityUIMode == TriggeredSpawner.Unity_UIVersion.uGUI;
        _hasSlider = _settings.GetComponent<Slider>() != null;
        _hasButton = _settings.GetComponent<Button>() != null;
        _hasRect = _settings.GetComponent<RectTransform>() != null;
#else
        // ReSharper disable once ConvertToConstant.Local
        var showNewUIEvents = false;
        _hasSlider = false;
        _hasButton = false;
        _hasRect = false;
#endif

        _changedWave = null;
        _waveToVisualize = null;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (_hasRect || _hasButton || _hasSlider || showNewUIEvents) { }

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/TriggeredSpawners.htm");

        EditorGUI.indentLevel = 0;
        _isDirty = false;

        if (!SpawnUtility.IsActive(_settings.gameObject))
        {
            DTInspectorUtility.RedBoldMessage("Despawned and inactive!");
        }
        else if (_settings.activeMode == LevelSettings.ActiveItemMode.Never)
        {
            DTInspectorUtility.RedBoldMessage("Spawner disabled by Active Mode setting");
        }
        else if (Application.isPlaying)
        {
            if (_settings.GameIsOverForSpawner)
            {
                DTInspectorUtility.RedBoldMessage("Spawner disabled by Game Over Behavior setting");
            }
            else if (_settings.SpawnerIsPaused)
            {
                DTInspectorUtility.RedBoldMessage("Spawner paused by Wave Pause Behavior setting");
            }
        }

        var waveActivated = false;
        DTInspectorUtility.StartGroupHeader();
        var newActive = (LevelSettings.ActiveItemMode)EditorGUILayout.EnumPopup("Active Mode", _settings.activeMode);
        if (newActive != _settings.activeMode)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Mode");
            _settings.activeMode = newActive;

            if (!Application.isPlaying)
            {
                _settings.gameObject.DestroyChildrenImmediateWithMarker();
            }

            // ReSharper disable once ConvertIfToOrExpression
            if (_settings.activeMode != LevelSettings.ActiveItemMode.Never)
            {
                waveActivated = true;
            }
        }
        EditorGUILayout.EndVertical();

        switch (_settings.activeMode)
        {
            case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
            case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                var missingStatNames = new List<string>();
                missingStatNames.AddRange(_allStats);
                missingStatNames.RemoveAll(delegate (string obj)
                {
                    return _settings.activeItemCriteria.HasKey(obj);
                });

                var newStat = EditorGUILayout.Popup("Add Active Limit", 0, missingStatNames.ToArray());
                if (newStat != 0)
                {
                    AddActiveLimit(missingStatNames[newStat], _settings);
                }

                if (_settings.activeItemCriteria.statMods.Count == 0)
                {
                    DTInspectorUtility.ShowRedErrorBox("You have no Active Limits. Spawner will never be Active.");
                }
                else
                {
                    EditorGUILayout.Separator();

                    int? indexToDelete = null;

                    for (var j = 0; j < _settings.activeItemCriteria.statMods.Count; j++)
                    {
                        var modifier = _settings.activeItemCriteria.statMods[j];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        var statName = modifier._statName;
                        GUILayout.Label(statName);

                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Min");

                        switch (modifier._varTypeToUse)
                        {
                            case WorldVariableTracker.VariableType._integer:
                                var newMin = EditorGUILayout.IntField(modifier._modValueIntMin, GUILayout.MaxWidth(60));
                                if (newMin != modifier._modValueIntMin)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Min");
                                    modifier._modValueIntMin = newMin;
                                }

                                GUILayout.Label("Max");
                                var newMax = EditorGUILayout.IntField(modifier._modValueIntMax, GUILayout.MaxWidth(60));
                                if (newMax != modifier._modValueIntMax)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Max");
                                    modifier._modValueIntMax = newMax;
                                }
                                break;
                            case WorldVariableTracker.VariableType._float:
                                var newMinFloat = EditorGUILayout.FloatField(modifier._modValueFloatMin, GUILayout.MaxWidth(60));
                                if (newMinFloat != modifier._modValueFloatMin)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Min");
                                    modifier._modValueFloatMin = newMinFloat;
                                }

                                GUILayout.Label("Max");
                                var newMaxFloat = EditorGUILayout.FloatField(modifier._modValueFloatMax, GUILayout.MaxWidth(60));
                                if (newMaxFloat != modifier._modValueFloatMax)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Max");
                                    modifier._modValueFloatMax = newMaxFloat;
                                }
                                break;
                            default:
                                Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                break;
                        }
                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                        if (GUILayout.Button(new GUIContent("Delete", "Remove this limit"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(64)))
                        {
                            indexToDelete = j;
                        }
                        GUI.backgroundColor = Color.white;
                        GUILayout.Space(5);
                        EditorGUILayout.EndHorizontal();

                        KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

                        var min = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMin : modifier._modValueFloatMin;
                        var max = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMax : modifier._modValueFloatMax;

                        if (min > max)
                        {
                            DTInspectorUtility.ShowRedErrorBox(modifier._statName + " Min cannot exceed Max, please fix!");
                        }
                    }

                    DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Modifier");
                        _settings.activeItemCriteria.DeleteByIndex(indexToDelete.Value);
                    }

                    EditorGUILayout.Separator();
                }

                break;
        }
        EditorGUILayout.EndVertical();

        var newGO = (TriggeredSpawner.GameOverBehavior)EditorGUILayout.EnumPopup("Game Over Behavior", _settings.gameOverBehavior);
        if (newGO != _settings.gameOverBehavior)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Game Over Behavior");
            _settings.gameOverBehavior = newGO;
        }

        var newPause = (TriggeredSpawner.WavePauseBehavior)EditorGUILayout.EnumPopup("Wave Pause Behavior", _settings.wavePauseBehavior);
        if (newPause != _settings.wavePauseBehavior)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Pause Behavior");
            _settings.wavePauseBehavior = newPause;
        }

        var newUI = (TriggeredSpawner.Unity_UIVersion)EditorGUILayout.EnumPopup("Unity UI Version", _settings.unityUIMode);
        if (newUI != _settings.unityUIMode)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Unity UI Version");
            _settings.unityUIMode = newUI;
        }

        var childSpawnerCount = TriggeredSpawner.GetChildSpawners(_settings.transform).Count;

        var newSource = (TriggeredSpawner.SpawnerEventSource)EditorGUILayout.EnumPopup("Trigger Source", _settings.eventSourceType);
        if (newSource != _settings.eventSourceType)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Trigger Source");
            _settings.eventSourceType = newSource;
        }

        if (_settings.eventSourceType == TriggeredSpawner.SpawnerEventSource.ReceiveFromParent && _settings.transform.parent == null)
        {
            DTInspectorUtility.ShowRedErrorBox("Illegal Trigger Source - this prefab has no parent.");
        }

        if (childSpawnerCount > 0)
        {
            var newTransmit = EditorGUILayout.Toggle("Propagate Triggers", _settings.transmitEventsToChildren);
            if (newTransmit != _settings.transmitEventsToChildren)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Propagate Triggers");
                _settings.transmitEventsToChildren = newTransmit;
            }
        }
        else
        {
            DTInspectorUtility.ShowColorWarningBox("Cannot propagate events with no child spawners");
        }

        var newOutside = EditorGUILayout.Toggle(new GUIContent("Spawn Outside Pool", "If this is checked, everything spawned from this will not reside under Pool Boss in the Hierarchy, but instead with no parent Game Object."), _settings.spawnOutsidePool);
        if (newOutside != _settings.spawnOutsidePool)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawn Outside Pool");
            _settings.spawnOutsidePool = newOutside;
        }

        EditorGUI.indentLevel = 0;
        var newLogMissing = EditorGUILayout.Toggle("Log Missing Events", _settings.logMissingEvents);
        if (newLogMissing != _settings.logMissingEvents)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Log Missing Events");
            _settings.logMissingEvents = newLogMissing;
        }

        var hadNoListener = _settings.listener == null;
        var newListener = (TriggeredSpawnerListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(TriggeredSpawnerListener), true);
        if (newListener != _settings.listener)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;
            if (hadNoListener && _settings.listener != null)
            {
                _settings.listener.sourceSpawnerName = _settings.transform.name;
            }
        }

        var unusedEvents = GetUnusedEventTypes();

        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader();
        var newUseLayer = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Layer Mode", _settings.spawnLayerMode);
        if (newUseLayer != _settings.spawnLayerMode)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Layer Mode");
            _settings.spawnLayerMode = newUseLayer;
        }
        EditorGUILayout.EndVertical();

        if (_settings.spawnLayerMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom)
        {
            EditorGUI.indentLevel = 0;

            var newCustomLayer = EditorGUILayout.LayerField("Custom Spawn Layer", _settings.spawnCustomLayer);
            if (newCustomLayer != _settings.spawnCustomLayer)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Layer");
                _settings.spawnCustomLayer = newCustomLayer;
            }
        }

        if (_settings.spawnLayerMode != WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnPrefabSettings)
        {
            var newRecurse = EditorGUILayout.Toggle("Apply Layer Recursively", _settings.applyLayerRecursively);
            if (newRecurse != _settings.applyLayerRecursively)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Apply Layer Recursively");
                _settings.applyLayerRecursively = newRecurse;
            }
        }

        EditorGUILayout.EndVertical();
        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        EditorGUI.indentLevel = 0;
        var newUseTag = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Tag Mode", _settings.spawnTagMode);
        if (newUseTag != _settings.spawnTagMode)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Tag Mode");
            _settings.spawnTagMode = newUseTag;
        }
        EditorGUILayout.EndVertical();

        if (_settings.spawnTagMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom)
        {
            EditorGUI.indentLevel = 0;
            var newCustomTag = EditorGUILayout.TagField("Custom Spawn Tag", _settings.spawnCustomTag);
            if (newCustomTag != _settings.spawnCustomTag)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Tag");
                _settings.spawnCustomTag = newCustomTag;
            }
        }
        EditorGUILayout.EndVertical();
        DTInspectorUtility.AddSpaceForNonU5();

        var newEventindex = EditorGUILayout.Popup("Event To Activate", 0, unusedEvents.ToArray());

        if (newEventindex > 0)
        {
            _isDirty = true;
            ActivateEvent(newEventindex, unusedEvents);
        }

        var hasEvent = false;
        if (_settings.enableWaves.Count > 0) { hasEvent = true; }
        if (_settings.disableWaves.Count > 0) { hasEvent = true; }
        if (_settings.visibleWaves.Count > 0) { hasEvent = true; }
        if (_settings.invisibleWaves.Count > 0) { hasEvent = true; }
        if (_settings.mouseOverWaves.Count > 0) { hasEvent = true; }
        if (_settings.mouseClickWaves.Count > 0) { hasEvent = true; }
        if (_settings.collisionWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerEnterWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerStayWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerExitWaves.Count > 0) { hasEvent = true; }
        if (_settings.spawnedWaves.Count > 0) { hasEvent = true; }
        if (_settings.despawnedWaves.Count > 0) { hasEvent = true; }
        if (_settings.codeTriggeredWaves1.Count > 0) { hasEvent = true; }
        if (_settings.codeTriggeredWaves2.Count > 0) { hasEvent = true; }
        if (_settings.clickWaves.Count > 0) { hasEvent = true; }
        if (_settings.collision2dWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerEnter2dWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerStay2dWaves.Count > 0) { hasEvent = true; }
        if (_settings.triggerExit2dWaves.Count > 0) { hasEvent = true; }
        if (_settings.unitySliderChangedWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityButtonClickedWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityPointerDownWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityPointerUpWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityPointerEnterWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityPointerExitWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityDragWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityDropWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityScrollWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityUpdateSelectedWaves.Count > 0) { hasEvent = true; }
        if (_settings.unitySelectWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityDeselectWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityMoveWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityInitializePotentialDragWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityBeginDragWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityEndDragWaves.Count > 0) { hasEvent = true; }
        if (_settings.unitySubmitWaves.Count > 0) { hasEvent = true; }
        if (_settings.unityCancelWaves.Count > 0) { hasEvent = true; }
        if (_settings.userDefinedEventWaves.Count > 0) { hasEvent = true; }
        
        if (hasEvent)         {
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Collapse All Events", EditorStyles.toolbarButton, GUILayout.Width(120)))             {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Collapse All Sections");

                CollapseFirstWaveIfAny(_settings.enableWaves);
                CollapseFirstWaveIfAny(_settings.disableWaves);
                CollapseFirstWaveIfAny(_settings.visibleWaves);
                CollapseFirstWaveIfAny(_settings.invisibleWaves);
                CollapseFirstWaveIfAny(_settings.mouseOverWaves);
                CollapseFirstWaveIfAny(_settings.mouseClickWaves);
                CollapseFirstWaveIfAny(_settings.collisionWaves);
                CollapseFirstWaveIfAny(_settings.triggerEnterWaves);
                CollapseFirstWaveIfAny(_settings.triggerStayWaves);
                CollapseFirstWaveIfAny(_settings.triggerExitWaves);
                CollapseFirstWaveIfAny(_settings.spawnedWaves);
                CollapseFirstWaveIfAny(_settings.despawnedWaves);
                CollapseFirstWaveIfAny(_settings.codeTriggeredWaves1);
                CollapseFirstWaveIfAny(_settings.codeTriggeredWaves2);
                CollapseFirstWaveIfAny(_settings.clickWaves);
                CollapseFirstWaveIfAny(_settings.collision2dWaves);
                CollapseFirstWaveIfAny(_settings.triggerEnter2dWaves);
                CollapseFirstWaveIfAny(_settings.triggerStay2dWaves);
                CollapseFirstWaveIfAny(_settings.triggerExit2dWaves);
                CollapseFirstWaveIfAny(_settings.unitySliderChangedWaves);
                CollapseFirstWaveIfAny(_settings.unityButtonClickedWaves);
                CollapseFirstWaveIfAny(_settings.unityPointerDownWaves);
                CollapseFirstWaveIfAny(_settings.unityPointerUpWaves);
                CollapseFirstWaveIfAny(_settings.unityPointerEnterWaves);
                CollapseFirstWaveIfAny(_settings.unityPointerExitWaves);
                CollapseFirstWaveIfAny(_settings.unityDragWaves);
                CollapseFirstWaveIfAny(_settings.unityDropWaves);
                CollapseFirstWaveIfAny(_settings.unityScrollWaves);
                CollapseFirstWaveIfAny(_settings.unityUpdateSelectedWaves);
                CollapseFirstWaveIfAny(_settings.unitySelectWaves);
                CollapseFirstWaveIfAny(_settings.unityDeselectWaves);
                CollapseFirstWaveIfAny(_settings.unityMoveWaves);
                CollapseFirstWaveIfAny(_settings.unityInitializePotentialDragWaves);
                CollapseFirstWaveIfAny(_settings.unityBeginDragWaves);
                CollapseFirstWaveIfAny(_settings.unityEndDragWaves);
                CollapseFirstWaveIfAny(_settings.unitySubmitWaves);
                CollapseFirstWaveIfAny(_settings.unityCancelWaves);

                for (var i = 0; i < _settings.userDefinedEventWaves.Count; i++)
                {
                    _settings.userDefinedEventWaves[i].isExpanded = false;
                }
            }
        } else
        {
            DTInspectorUtility.ShowLargeBarAlertBox("You have no Waves set up. This Spawner will do nothing. Add a Wave by selecting an event from the 'Event To Activate' dropdown above.");
        }
        GUI.contentColor = Color.white;

        DTInspectorUtility.VerticalSpace(3);

        _allWaves.Clear();

        if (IsFirstWaveEnabled(_settings.enableWaves))
        {
            RenderTriggeredWave(ref _settings.enableWaves, GetFirstWaveIfAny(_settings.enableWaves), "Enabled Event", TriggeredSpawner.EventType.OnEnabled);
        }
        if (IsFirstWaveEnabled(_settings.disableWaves))
        {
            RenderTriggeredWave(ref _settings.disableWaves, GetFirstWaveIfAny(_settings.disableWaves), "Disabled Event", TriggeredSpawner.EventType.OnDisabled);
        }
        if (IsFirstWaveEnabled(_settings.visibleWaves))
        {
            RenderTriggeredWave(ref _settings.visibleWaves, GetFirstWaveIfAny(_settings.visibleWaves), "Visible Event", TriggeredSpawner.EventType.Visible);
        }
        if (IsFirstWaveEnabled(_settings.invisibleWaves))
        {
            RenderTriggeredWave(ref _settings.invisibleWaves, GetFirstWaveIfAny(_settings.invisibleWaves), "Invisible Event", TriggeredSpawner.EventType.Invisible);
        }
        if (IsFirstWaveEnabled(_settings.mouseOverWaves))
        {
            RenderTriggeredWave(ref _settings.mouseOverWaves, GetFirstWaveIfAny(_settings.mouseOverWaves), "Mouse Over (Legacy) Event", TriggeredSpawner.EventType.MouseOver_Legacy);
        }
        if (IsFirstWaveEnabled(_settings.mouseClickWaves))
        {
            RenderTriggeredWave(ref _settings.mouseClickWaves, GetFirstWaveIfAny(_settings.mouseClickWaves), "Mouse Click (Legacy) Event", TriggeredSpawner.EventType.MouseClick_Legacy);
        }

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        if (showNewUIEvents)
        {
            if (_hasSlider && IsFirstWaveEnabled(_settings.unitySliderChangedWaves))
            {
                RenderTriggeredWave(ref _settings.unitySliderChangedWaves, GetFirstWaveIfAny(_settings.unitySliderChangedWaves), "Slider Changed (uGUI) Event", TriggeredSpawner.EventType.SliderChanged_uGUI);
            }
            if (_hasButton && IsFirstWaveEnabled(_settings.unityButtonClickedWaves))
            {
                RenderTriggeredWave(ref _settings.unityButtonClickedWaves, GetFirstWaveIfAny(_settings.unityButtonClickedWaves), "Button Click (uGUI) Event", TriggeredSpawner.EventType.ButtonClicked_uGUI);
            }

            if (_hasRect)
            {
                if (IsFirstWaveEnabled(_settings.unityPointerDownWaves))
                {
                    RenderTriggeredWave(ref _settings.unityPointerDownWaves, GetFirstWaveIfAny(_settings.unityPointerDownWaves), "Pointer Down (uGUI) Event", TriggeredSpawner.EventType.PointerDown_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityPointerUpWaves))
                {
                    RenderTriggeredWave(ref _settings.unityPointerUpWaves, GetFirstWaveIfAny(_settings.unityPointerUpWaves), "Pointer Up (uGUI) Event", TriggeredSpawner.EventType.PointerUp_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityPointerEnterWaves))
                {
                    RenderTriggeredWave(ref _settings.unityPointerEnterWaves, GetFirstWaveIfAny(_settings.unityPointerEnterWaves), "Pointer Enter (uGUI) Event", TriggeredSpawner.EventType.PointerEnter_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityPointerExitWaves))
                {
                    RenderTriggeredWave(ref _settings.unityPointerExitWaves, GetFirstWaveIfAny(_settings.unityPointerExitWaves), "Pointer Exit (uGUI) Event", TriggeredSpawner.EventType.PointerExit_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityDragWaves))
                {
                    RenderTriggeredWave(ref _settings.unityDragWaves, GetFirstWaveIfAny(_settings.unityDragWaves), "Drag (uGUI) Event", TriggeredSpawner.EventType.Drag_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityDropWaves))
                {
                    RenderTriggeredWave(ref _settings.unityDropWaves, GetFirstWaveIfAny(_settings.unityDropWaves), "Drop (uGUI) Event", TriggeredSpawner.EventType.Drop_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityScrollWaves))
                {
                    RenderTriggeredWave(ref _settings.unityScrollWaves, GetFirstWaveIfAny(_settings.unityScrollWaves), "Scroll (uGUI) Event", TriggeredSpawner.EventType.Scroll_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityUpdateSelectedWaves))
                {
                    RenderTriggeredWave(ref _settings.unityUpdateSelectedWaves, GetFirstWaveIfAny(_settings.unityUpdateSelectedWaves), "Update Selected (uGUI) Event", TriggeredSpawner.EventType.UpdateSelected_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unitySelectWaves))
                {
                    RenderTriggeredWave(ref _settings.unitySelectWaves, GetFirstWaveIfAny(_settings.unitySelectWaves), "Select (uGUI) Event", TriggeredSpawner.EventType.Select_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityDeselectWaves))
                {
                    RenderTriggeredWave(ref _settings.unityDeselectWaves, GetFirstWaveIfAny(_settings.unityDeselectWaves), "Deselect (uGUI) Event", TriggeredSpawner.EventType.Deselect_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityMoveWaves))
                {
                    RenderTriggeredWave(ref _settings.unityMoveWaves, GetFirstWaveIfAny(_settings.unityMoveWaves), "Move (uGUI) Event", TriggeredSpawner.EventType.Move_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityInitializePotentialDragWaves))
                {
                    RenderTriggeredWave(ref _settings.unityInitializePotentialDragWaves, GetFirstWaveIfAny(_settings.unityInitializePotentialDragWaves), "Init. Potential Drag (uGUI) Event", TriggeredSpawner.EventType.InitializePotentialDrag_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityBeginDragWaves))
                {
                    RenderTriggeredWave(ref _settings.unityBeginDragWaves, GetFirstWaveIfAny(_settings.unityBeginDragWaves), "Begin Drag (uGUI) Event", TriggeredSpawner.EventType.BeginDrag_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityEndDragWaves))
                {
                    RenderTriggeredWave(ref _settings.unityEndDragWaves, GetFirstWaveIfAny(_settings.unityEndDragWaves), "End Drag (uGUI) Event", TriggeredSpawner.EventType.EndDrag_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unitySubmitWaves))
                {
                    RenderTriggeredWave(ref _settings.unitySubmitWaves, GetFirstWaveIfAny(_settings.unitySubmitWaves), "Submit (uGUI) Event", TriggeredSpawner.EventType.Submit_uGUI);
                }
                if (IsFirstWaveEnabled(_settings.unityCancelWaves))
                {
                    RenderTriggeredWave(ref _settings.unityCancelWaves, GetFirstWaveIfAny(_settings.unityCancelWaves), "Cancel (uGUI) Event", TriggeredSpawner.EventType.Cancel_uGUI);
                }
            }
        }
#endif

        if (IsFirstWaveEnabled(_settings.collisionWaves))
        {
            RenderTriggeredWave(ref _settings.collisionWaves, GetFirstWaveIfAny(_settings.collisionWaves), "Collision Enter Event", TriggeredSpawner.EventType.OnCollision);
        }
        if (IsFirstWaveEnabled(_settings.triggerEnterWaves))
        {
            RenderTriggeredWave(ref _settings.triggerEnterWaves, GetFirstWaveIfAny(_settings.triggerEnterWaves), "Trigger Enter Event", TriggeredSpawner.EventType.OnTriggerEnter);
        }
        if (IsFirstWaveEnabled(_settings.triggerStayWaves))
        {
            RenderTriggeredWave(ref _settings.triggerStayWaves, GetFirstWaveIfAny(_settings.triggerStayWaves), "Trigger Stay Event", TriggeredSpawner.EventType.OnTriggerStay);
        }
        if (IsFirstWaveEnabled(_settings.triggerExitWaves))
        {
            RenderTriggeredWave(ref _settings.triggerExitWaves, GetFirstWaveIfAny(_settings.triggerExitWaves), "Trigger Exit Event", TriggeredSpawner.EventType.OnTriggerExit);
        }

        // Unity 4.3 Events
        if (IsFirstWaveEnabled(_settings.collision2dWaves))
        {
            RenderTriggeredWave(ref _settings.collision2dWaves, GetFirstWaveIfAny(_settings.collision2dWaves), "2D Collision Enter Event", TriggeredSpawner.EventType.OnCollision2D);
        }

        if (IsFirstWaveEnabled(_settings.triggerEnter2dWaves))
        {
            RenderTriggeredWave(ref _settings.triggerEnter2dWaves, GetFirstWaveIfAny(_settings.triggerEnter2dWaves), "2D Trigger Enter Event", TriggeredSpawner.EventType.OnTriggerEnter2D);
        }

        if (IsFirstWaveEnabled(_settings.triggerStay2dWaves))
        {
            RenderTriggeredWave(ref _settings.triggerStay2dWaves, GetFirstWaveIfAny(_settings.triggerStay2dWaves), "2D Trigger Stay Event", TriggeredSpawner.EventType.OnTriggerStay2D);
        }

        if (IsFirstWaveEnabled(_settings.triggerExit2dWaves))
        {
            RenderTriggeredWave(ref _settings.triggerExit2dWaves, GetFirstWaveIfAny(_settings.triggerExit2dWaves), "2D Trigger Exit Event", TriggeredSpawner.EventType.OnTriggerExit2D);
        }

        // code triggered event
        if (IsFirstWaveEnabled(_settings.codeTriggeredWaves1))
        {
            RenderTriggeredWave(ref _settings.codeTriggeredWaves1, GetFirstWaveIfAny(_settings.codeTriggeredWaves1), "Code-Triggered Event 1", TriggeredSpawner.EventType.CodeTriggered1);
        }
        if (IsFirstWaveEnabled(_settings.codeTriggeredWaves2))
        {
            RenderTriggeredWave(ref _settings.codeTriggeredWaves2, GetFirstWaveIfAny(_settings.codeTriggeredWaves2), "Code-Triggered Event 2", TriggeredSpawner.EventType.CodeTriggered2);
        }

        // Pool Boss & Pool Manager events (same for both).
        if (IsFirstWaveEnabled(_settings.spawnedWaves))
        {
            RenderTriggeredWave(ref _settings.spawnedWaves, GetFirstWaveIfAny(_settings.spawnedWaves), "Spawned Event", TriggeredSpawner.EventType.OnSpawned);
        }
        if (IsFirstWaveEnabled(_settings.despawnedWaves))
        {
            RenderTriggeredWave(ref _settings.despawnedWaves, GetFirstWaveIfAny(_settings.despawnedWaves), "Despawned Event", TriggeredSpawner.EventType.OnDespawned);
        }

        // NGUI events
        if (IsFirstWaveEnabled(_settings.clickWaves))
        {
            RenderTriggeredWave(ref _settings.clickWaves, GetFirstWaveIfAny(_settings.clickWaves), "NGUI OnClick Event", TriggeredSpawner.EventType.OnClick_NGUI);
        }

        for (var i = 0; i < _settings.userDefinedEventWaves.Count; i++)
        {
            var aWave = _settings.userDefinedEventWaves[i];
            RenderTriggeredWave(ref _settings.userDefinedEventWaves, aWave, "Custom Event", TriggeredSpawner.EventType.CustomEvent, i);
        }

        if (!Application.isPlaying && !DTInspectorUtility.IsPrefabInProjectView(_settings))
        {
            if (_waveToVisualize != null)
            {
                // turn off other waves!
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var w = 0; w < _allWaves.Count; w++)
                {
                    if (_allWaves[w] != _waveToVisualize)
                    {
                        _allWaves[w].visualizeWave = false;
                    }
                }
            }

            TriggeredWaveSpecifics wave = null;
            if (_changedWave != null)
            {
                wave = _changedWave;
            }

            var hasUnrenderedVisualWave = false;
            if (wave == null)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var w = 0; w < _allWaves.Count; w++)
                {
                    var oneWave = _allWaves[w];
                    if (!oneWave.visualizeWave)
                    {
                        continue;
                    }

                    if (_settings.transform.childCount != 0 || oneWave.NumberToSpwn.Value <= 0)
                    {
                        continue;
                    }

                    hasUnrenderedVisualWave = true;
                    break;
                }
            }

            if (waveActivated || hasUnrenderedVisualWave)
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var w = 0; w < _allWaves.Count; w++)
                {
                    if (!_allWaves[w].visualizeWave)
                    {
                        continue;
                    }

                    wave = _allWaves[w];
                    break;
                }
            }

            if (wave != null)
            {
                if (wave.visualizeWave)
                {
                    _settings.gameObject.DestroyChildrenImmediateWithMarker();
                    _settings.SpawnWaveVisual(wave);
                }
            }
        }

        if (GUI.changed || _isDirty)
        {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void RenderTriggeredWave(ref List<TriggeredWaveSpecifics> waveList, TriggeredWaveSpecifics waveSetting, string toggleText, TriggeredSpawner.EventType eventType, int? itemIndex = null)
    {
        _allWaves.Add(waveSetting);

        var disabledText = string.Empty;

        if (eventType == TriggeredSpawner.EventType.CustomEvent && !string.IsNullOrEmpty(waveSetting.customEventName))
        {
            toggleText += ": " + waveSetting.customEventName;
        }

        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never)
        {
            disabledText = " - DISABLED";
        }

        toggleText += disabledText;

        EditorGUI.indentLevel = 0;

        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never)
        {
            DTInspectorUtility.StartGroupHeader(1);
            EditorGUILayout.LabelField(toggleText);
            DTInspectorUtility.EndGroupHeader();
            return;
        }

        if (eventType == TriggeredSpawner.EventType.CustomEvent)
        {
            var state = waveSetting.isExpanded;
            var text = toggleText;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!state)
            {
                GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
            }
            else
            {
                GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
            }

            GUILayout.BeginHorizontal();

            text = "<b><size=11>" + text + "</size></b>";

            if (state)
            {
                text = "\u25BC " + text;
            }
            else
            {
                text = "\u25BA " + text;
            }
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
            {
                state = !state;
            }

            GUILayout.Space(2f);

            if (state != waveSetting.isExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Event active");
                waveSetting.isExpanded = state;
            }

            var buttonPressed = DTInspectorUtility.AddCustomEventIcons(false, false, false, true);
            DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/TriggeredSpawners.htm#WaveSettings");

            switch (buttonPressed)
            {
                case DTInspectorUtility.FunctionButtons.Remove:
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Custom Event");
                    // ReSharper disable once PossibleInvalidOperationException
                    _settings.userDefinedEventWaves.RemoveAt(itemIndex.Value);
                    waveSetting.customEventActive = false;
                    break;
                case DTInspectorUtility.FunctionButtons.Fire:
                    _settings.ReceiveEvent(waveSetting.customEventName, _settings.transform.position);
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {

            var state = waveSetting.isExpanded;
            var text = toggleText;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!state)
            {
                GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
            }
            else
            {
                GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
            }

            GUILayout.BeginHorizontal();

            text = "<b><size=11>" + text + "</size></b>";
            if (state)
            {
                text = "\u25BC " + text;
            }
            else
            {
                text = "\u25BA " + text;
            }
            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
            {
                state = !state;
            }

            GUILayout.Space(2f);


            if (state != waveSetting.isExpanded)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "expand " + eventType + " event");
                waveSetting.isExpanded = state;
            }

            var buttonPressed = DTInspectorUtility.AddCustomEventIcons(false, false, false, true);

            switch (buttonPressed)
            {
                case DTInspectorUtility.FunctionButtons.Remove:
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete " + eventType);

                    waveList = new List<TriggeredWaveSpecifics>(0);
                    break;
                case DTInspectorUtility.FunctionButtons.Fire:
                    _settings.PropagateEventTrigger(eventType, null, true);
                    break;
            }
            DTInspectorUtility.AddHelpIcon("http://www.dtdevtools.com/docs/coregamekit/TriggeredSpawners.htm#WaveSettings");

            EditorGUILayout.EndHorizontal();
        }

        if (!waveSetting.isExpanded)
        {
            DTInspectorUtility.VerticalSpace(2);
            return;
        }

        DTInspectorUtility.BeginGroupedControls();

        if (eventType == TriggeredSpawner.EventType.CustomEvent)
        {
            if (_levelSettingsInScene)
            {
                var existingIndex = _customEventNames.IndexOf(waveSetting.customEventName);

                int? customEventIndex = null;

                EditorGUI.indentLevel = 0;

                var noEvent = false;
                var noMatch = false;

                if (existingIndex >= 1)
                {
                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
                    if (existingIndex == 1)
                    {
                        noEvent = true;
                    }
                }
                else if (existingIndex == -1 && waveSetting.customEventName == LevelSettings.NoEventName)
                {
                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
                }
                else
                { // non-match
                    noMatch = true;
                    var newEventName = EditorGUILayout.TextField("Custom Event Name", waveSetting.customEventName);
                    if (newEventName != waveSetting.customEventName)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Event Name");
                        waveSetting.customEventName = newEventName;
                    }

                    var newIndex = EditorGUILayout.Popup("All Custom Events", -1, _customEventNames.ToArray());
                    if (newIndex >= 0)
                    {
                        customEventIndex = newIndex;
                    }
                }

                if (noEvent)
                {
                    DTInspectorUtility.ShowRedErrorBox("No Custom Event specified. This section will do nothing.");
                }
                else if (noMatch)
                {
                    DTInspectorUtility.ShowRedErrorBox("Custom Event found no match. Type in or choose one.");
                }

                if (customEventIndex.HasValue)
                {
                    if (existingIndex != customEventIndex.Value)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Event");
                    }
                    switch (customEventIndex.Value)
                    {
                        case -1:
                            waveSetting.customEventName = LevelSettings.NoEventName;
                            break;
                        default:
                            waveSetting.customEventName = _customEventNames[customEventIndex.Value];
                            break;
                    }
                }
            }
            else
            {
                var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", waveSetting.customEventName);
                if (newCustomEvent != waveSetting.customEventName)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Custom Event Name");
                    waveSetting.customEventName = newCustomEvent;
                }
            }
        }

        var poolNames = PoolNames;

        if (!waveSetting.enableWave)
        {
            return;
        }

        var newVis = EditorGUILayout.Toggle("Visualize Wave", waveSetting.visualizeWave);
        if (newVis != waveSetting.visualizeWave)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Visualize Wave");
            waveSetting.visualizeWave = newVis;
            if (!newVis)
            {
                _settings.gameObject.DestroyChildrenImmediateWithMarker();
            }
            else
            {
                _changedWave = waveSetting;
                _waveToVisualize = waveSetting;
            }
        }

        if (eventType == TriggeredSpawner.EventType.OnTriggerStay || eventType == TriggeredSpawner.EventType.OnTriggerStay2D)
        {
            var newStay = EditorGUILayout.Slider("After Stay (sec)", waveSetting.triggerStayForTime, 0.1f, 10000f);
            if (newStay != waveSetting.triggerStayForTime)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change After Stay (sec)");
                waveSetting.triggerStayForTime = newStay;
            }
        }

        var newElim = (TriggeredSpawnerV2.RepeatWaitFor)EditorGUILayout.EnumPopup("Wave Completed When", waveSetting.repeatWaitsForType);
        if (newElim != waveSetting.repeatWaitsForType)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Completed When");
            waveSetting.repeatWaitsForType = newElim;
        }

        var newSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Prefab Type", waveSetting.spawnSource);
        if (newSource != waveSetting.spawnSource)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Type");
            waveSetting.spawnSource = newSource;
            _changedWave = waveSetting;
        }
        switch (waveSetting.spawnSource)
        {
            case WaveSpecifics.SpawnOrigin.Specific:
                PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _settings, ref waveSetting.prefabToSpawn, ref waveSetting.prefabToSpawnCategoryName, "Prefab To Spawn");

                if (_isDirty)
                {
                    _changedWave = waveSetting;
                }

                if (waveSetting.prefabToSpawn == null)
                {
                    DTInspectorUtility.ShowRedErrorBox("Please specify a prefab to spawn.");
                }
                break;
            case WaveSpecifics.SpawnOrigin.PrefabPool:
                if (poolNames != null)
                {
                    var pool = LevelSettings.GetFirstMatchingPrefabPool(waveSetting.prefabPoolName);
                    var noPoolSelected = false;
                    var illegalPool = false;
                    var noPools = false;

                    if (pool == null)
                    {
                        if (string.IsNullOrEmpty(waveSetting.prefabPoolName))
                        {
                            noPoolSelected = true;
                        }
                        else
                        {
                            illegalPool = true;
                        }
                        waveSetting.prefabPoolIndex = 0;
                    }
                    else
                    {
                        waveSetting.prefabPoolIndex = poolNames.IndexOf(waveSetting.prefabPoolName);
                    }

                    if (poolNames.Count > 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var newPool = EditorGUILayout.Popup("Prefab Pool", waveSetting.prefabPoolIndex, poolNames.ToArray());
                        if (newPool != waveSetting.prefabPoolIndex)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Pool");
                            waveSetting.prefabPoolIndex = newPool;
                        }

                        if (waveSetting.prefabPoolIndex > 0)
                        {
                            var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[waveSetting.prefabPoolIndex]);
                            if (matchingPool != null)
                            {
                                waveSetting.prefabPoolName = matchingPool.name;
                            }
                        }
                        else
                        {
                            waveSetting.prefabPoolName = string.Empty;
                        }

                        if (newPool > 0)
                        {
                            if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
                                DTInspectorUtility.FunctionButtons.Edit)
                            {
                                Selection.activeGameObject = pool.gameObject;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        noPools = true;
                    }

                    if (noPools)
                    {
                        DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
                    }
                    else if (noPoolSelected)
                    {
                        DTInspectorUtility.ShowRedErrorBox("No Prefab Pool selected.");
                    }
                    else if (illegalPool)
                    {
                        DTInspectorUtility.ShowRedErrorBox("Prefab Pool '" + waveSetting.prefabPoolName + "' not found. Select one.");
                    }
                }
                else
                {
                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
                }

                break;
        }

        var oldInt = waveSetting.NumberToSpwn.Value;
        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.NumberToSpwn, "Min To Spawn", _settings);
        if (oldInt != waveSetting.NumberToSpwn.Value)
        {
            _changedWave = waveSetting;
        }

        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.MaxToSpawn, "Max To Spawn", _settings);

        if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType))
        {
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.TimeToSpawnEntireWave, "Time To Spawn All", _settings);
        }

        if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType))
        {
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.WaveDelaySec, "Delay Wave (sec)", _settings);
        }

        if (waveSetting.enableRepeatWave)
        {
            var newUseDelayOnRepeat = EditorGUILayout.Toggle("Use Delay Wave on Repeats", waveSetting.doesRepeatUseWaveDelay);
            if (newUseDelayOnRepeat != waveSetting.doesRepeatUseWaveDelay)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Delay Wave on Repeats");
                waveSetting.doesRepeatUseWaveDelay = newUseDelayOnRepeat;
            }
        }

        bool newDisable;

        switch (eventType)
        {
            case TriggeredSpawner.EventType.Visible:
                newDisable = EditorGUILayout.Toggle("Stop On Invisible", waveSetting.stopWaveOnOppositeEvent);
                if (newDisable != waveSetting.stopWaveOnOppositeEvent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Invisible");
                    waveSetting.stopWaveOnOppositeEvent = newDisable;
                }
                break;
            case TriggeredSpawner.EventType.OnTriggerEnter:
            case TriggeredSpawner.EventType.OnTriggerStay:
                newDisable = EditorGUILayout.Toggle("Stop When Trigger Exit", waveSetting.stopWaveOnOppositeEvent);
                if (newDisable != waveSetting.stopWaveOnOppositeEvent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Exit");
                    waveSetting.stopWaveOnOppositeEvent = newDisable;
                }
                break;
            case TriggeredSpawner.EventType.OnTriggerExit:
                newDisable = EditorGUILayout.Toggle("Stop When Trigger Enter", waveSetting.stopWaveOnOppositeEvent);
                if (newDisable != waveSetting.stopWaveOnOppositeEvent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Enter");
                    waveSetting.stopWaveOnOppositeEvent = newDisable;
                }
                break;
            case TriggeredSpawner.EventType.OnTriggerEnter2D:
            case TriggeredSpawner.EventType.OnTriggerStay2D:
                newDisable = EditorGUILayout.Toggle("Stop When Trigger Exit 2D", waveSetting.stopWaveOnOppositeEvent);
                if (newDisable != waveSetting.stopWaveOnOppositeEvent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Exit 2D");
                    waveSetting.stopWaveOnOppositeEvent = newDisable;
                }
                break;
            case TriggeredSpawner.EventType.OnTriggerExit2D:
                newDisable = EditorGUILayout.Toggle("Stop When Trigger Enter 2D", waveSetting.stopWaveOnOppositeEvent);
                if (newDisable != waveSetting.stopWaveOnOppositeEvent)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Enter 2D");
                    waveSetting.stopWaveOnOppositeEvent = newDisable;
                }
                break;
        }

        newDisable = EditorGUILayout.Toggle("Disable Event After", waveSetting.disableAfterFirstTrigger);
        if (newDisable != waveSetting.disableAfterFirstTrigger)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Disable Event After");
            waveSetting.disableAfterFirstTrigger = newDisable;
        }

        if (TriggeredSpawner.eventsThatCanTriggerDespawn.Contains(eventType))
        {
            var newWillDespawn = EditorGUILayout.Toggle("Despawn This", waveSetting.willDespawnOnEvent);
            if (newWillDespawn != waveSetting.willDespawnOnEvent)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Despawn This");
                waveSetting.willDespawnOnEvent = newWillDespawn;
            }
        }

        if (TriggeredSpawner.eventsWithTagLayerFilters.Contains(eventType))
        {
            DTInspectorUtility.StartGroupHeader(1);
            var newLayer = EditorGUILayout.BeginToggleGroup(" Layer Filter", waveSetting.useLayerFilter);
            if (newLayer != waveSetting.useLayerFilter)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Layer Filter");
                waveSetting.useLayerFilter = newLayer;
            }
            DTInspectorUtility.EndGroupHeader();

            if (waveSetting.useLayerFilter)
            {
                for (var i = 0; i < waveSetting.matchingLayers.Count; i++)
                {
                    var newMatch = EditorGUILayout.LayerField("Layer Match " + (i + 1), waveSetting.matchingLayers[i]);
                    if (newMatch == waveSetting.matchingLayers[i])
                    {
                        continue;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Layer Match");
                    waveSetting.matchingLayers[i] = newMatch;
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a Layer Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Layer Match");
                    waveSetting.matchingLayers.Add(0);
                }
                GUILayout.Space(10);
                if (waveSetting.matchingLayers.Count > 1)
                {
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Layer Match"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Layer Match");
                        waveSetting.matchingLayers.RemoveAt(waveSetting.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
            DTInspectorUtility.AddSpaceForNonU5();

            DTInspectorUtility.StartGroupHeader(1);
            var newTag = EditorGUILayout.BeginToggleGroup(" Tag Filter", waveSetting.useTagFilter);
            if (newTag != waveSetting.useTagFilter)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Tag Filter");
                waveSetting.useTagFilter = newTag;
            }
            DTInspectorUtility.EndGroupHeader();
            if (waveSetting.useTagFilter)
            {
                for (var i = 0; i < waveSetting.matchingTags.Count; i++)
                {
                    var newMatch = EditorGUILayout.TagField("Tag Match " + (i + 1), waveSetting.matchingTags[i]);
                    if (newMatch == waveSetting.matchingTags[i])
                    {
                        continue;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Tag Match");
                    waveSetting.matchingTags[i] = newMatch;
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(12);
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a Tag Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Tag Match");
                    waveSetting.matchingTags.Add("Untagged");
                }
                GUILayout.Space(10);
                if (waveSetting.matchingTags.Count > 1)
                {
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Tag Match"), EditorStyles.toolbarButton, GUILayout.Width(60)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Tag Match");
                        waveSetting.matchingTags.RemoveAt(waveSetting.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
        }

        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        EditorGUI.indentLevel = 1;
        var newEx = DTInspectorUtility.Foldout(waveSetting.positionExpanded, " Position Settings");
        if (newEx != waveSetting.positionExpanded)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Position Settings");
            waveSetting.positionExpanded = newEx;
        }
        EditorGUILayout.EndVertical();
        EditorGUI.indentLevel = 0;

        // ReSharper disable once TooWideLocalVariableScope
        float oldFloat;

        if (waveSetting.positionExpanded)
        {
            var newX = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("X Position Mode", waveSetting.positionXmode);
            if (newX != waveSetting.positionXmode)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change X Position Mode");
                waveSetting.positionXmode = newX;
                _changedWave = waveSetting;
            }

            Transform otherObj;

            switch (waveSetting.positionXmode)
            {
                case WaveSpecifics.PositionMode.CustomPosition:
                    oldFloat = waveSetting.customPosX.Value;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosX, "X Position", _settings);
                    if (oldFloat != waveSetting.customPosX.Value)
                    {
                        _changedWave = waveSetting;
                    }
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectX, typeof(Transform), true);
                    if (waveSetting.otherObjectX != otherObj)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                        waveSetting.otherObjectX = otherObj;
                    }
                    if (waveSetting.otherObjectX == null)
                    {
                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                    }
                    DTInspectorUtility.VerticalSpace(3);
                    break;
            }

            var newY = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Y Position Mode", waveSetting.positionYmode);
            if (newY != waveSetting.positionYmode)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Y Position Mode");
                waveSetting.positionYmode = newY;
                _changedWave = waveSetting;
            }

            switch (waveSetting.positionYmode)
            {
                case WaveSpecifics.PositionMode.CustomPosition:
                    oldFloat = waveSetting.customPosY.Value;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosY, "Y Position", _settings);
                    if (oldFloat != waveSetting.customPosY.Value)
                    {
                        _changedWave = waveSetting;
                    }
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectY, typeof(Transform), true);
                    if (waveSetting.otherObjectY != otherObj)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                        waveSetting.otherObjectY = otherObj;
                    }
                    if (waveSetting.otherObjectY == null)
                    {
                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                    }
                    DTInspectorUtility.VerticalSpace(3);
                    break;
            }

            var newZ = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Z Position Mode", waveSetting.positionZmode);
            if (newZ != waveSetting.positionZmode)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Z Position Mode");
                waveSetting.positionZmode = newZ;
                _changedWave = waveSetting;
            }

            switch (waveSetting.positionZmode)
            {
                case WaveSpecifics.PositionMode.CustomPosition:
                    oldFloat = waveSetting.customPosZ.Value;
                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosZ, "Z Position", _settings);
                    if (oldFloat != waveSetting.customPosZ.Value)
                    {
                        _changedWave = waveSetting;
                    }
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectZ, typeof(Transform), true);
                    if (waveSetting.otherObjectZ != otherObj)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
                        waveSetting.otherObjectZ = otherObj;
                    }
                    if (waveSetting.otherObjectZ == null)
                    {
                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
                    }
                    break;
            }

            if (waveSetting.waveOffsetList.Count == 0)
            {
                waveSetting.waveOffsetList.Add(new Vector3());
                _isDirty = true;
            }

            DTInspectorUtility.StartGroupHeader();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wave Offsets");

            if (!Application.isPlaying)
            {
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                if (GUILayout.Button(new GUIContent("Add", "Add a new Wave Offset"),
                    EditorStyles.toolbarButton, GUILayout.MaxWidth(50)))
                {
                    waveSetting.waveOffsetList.Add(new Vector3());
                    _isDirty = true;
                    _changedWave = waveSetting;
                }
                GUI.contentColor = Color.white;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (!Application.isPlaying)
            {
                var newMode = (WaveSpecifics.WaveOffsetChoiceMode)EditorGUILayout.EnumPopup("Offset Selection", waveSetting.offsetChoiceMode);
                if (newMode != waveSetting.offsetChoiceMode)
                {
                    _isDirty = true;
                    waveSetting.offsetChoiceMode = newMode;
                }
            }

            int? itemToDelete = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < waveSetting.waveOffsetList.Count; i++)
            {
                var anOffset = waveSetting.waveOffsetList[i];
                EditorGUILayout.BeginHorizontal();

                var newOffset = EditorGUILayout.Vector3Field("Wave Offset #" + (i + 1), anOffset);

                var btn = DTInspectorUtility.FunctionButtons.None;

                if (!Application.isPlaying)
                {
                    btn = DTInspectorUtility.AddCustomEventIcons(false, false, false, false, "Wave Offset");
                }

                EditorGUILayout.EndHorizontal();

                if (btn == DTInspectorUtility.FunctionButtons.Remove)
                {
                    itemToDelete = i;
                }

                if (newOffset == anOffset)
                {
                    continue;
                }

                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Offset");
                waveSetting.waveOffsetList[i] = newOffset;
                _changedWave = waveSetting;
            }
            EditorGUILayout.EndVertical();

            if (itemToDelete.HasValue)
            {
                waveSetting.waveOffsetList.RemoveAt(itemToDelete.Value);
                _isDirty = true;
                _changedWave = waveSetting;
            }
        }

        EditorGUILayout.EndVertical();
        //DTInspectorUtility.ResetColors();

        if (waveSetting.isCustomEvent)
        {
            var newLookAt = (WaveSpecifics.SpawnerRotationMode)EditorGUILayout.EnumPopup("Spawner Rotation Mode", waveSetting.curSpawnerRotMode);
            if (newLookAt != waveSetting.curSpawnerRotMode)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawner Rotation Mode");
                waveSetting.curSpawnerRotMode = newLookAt;
            }
        }

        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        var newRotation = (WaveSpecifics.RotationMode)EditorGUILayout.EnumPopup("Spawn Rotation Mode", waveSetting.curRotationMode);
        if (newRotation != waveSetting.curRotationMode)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Rotation Mode");
            waveSetting.curRotationMode = newRotation;
            _changedWave = waveSetting;
        }
        EditorGUILayout.EndVertical();

        if (waveSetting.curRotationMode == WaveSpecifics.RotationMode.LookAtCustomEventOrigin)
        {
            if (!waveSetting.isCustomEvent)
            {
                DTInspectorUtility.ShowRedErrorBox("Look At Custom Event Origin rotation mode is only valid for Custom Events.");
            }
            else
            {
                EditorGUI.indentLevel = 0;

                var ignoreX = EditorGUILayout.Toggle("Ignore Origin X", waveSetting.eventOriginIgnoreX);
                if (ignoreX != waveSetting.eventOriginIgnoreX)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin X");
                    waveSetting.eventOriginIgnoreX = ignoreX;
                }

                var ignoreY = EditorGUILayout.Toggle("Ignore Origin Y", waveSetting.eventOriginIgnoreY);
                if (ignoreY != waveSetting.eventOriginIgnoreY)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin Y");
                    waveSetting.eventOriginIgnoreY = ignoreY;
                }

                var ignoreZ = EditorGUILayout.Toggle("Ignore Origin Z", waveSetting.eventOriginIgnoreZ);
                if (ignoreZ != waveSetting.eventOriginIgnoreZ)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin Z");
                    waveSetting.eventOriginIgnoreZ = ignoreZ;
                }
            }
        }

        EditorGUI.indentLevel = 0;
        if (waveSetting.curRotationMode == WaveSpecifics.RotationMode.CustomRotation)
        {
            var newCust = EditorGUILayout.Vector3Field("Custom Rotation Euler", waveSetting.customRotation);
            if (newCust != waveSetting.customRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Rotation Euler");
                waveSetting.customRotation = newCust;
                _changedWave = waveSetting;
            }
        }
        EditorGUILayout.EndVertical();
        DTInspectorUtility.ResetColors();

        if (!waveSetting.disableAfterFirstTrigger)
        {
            if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType))
            {
                DTInspectorUtility.AddSpaceForNonU5();
                DTInspectorUtility.StartGroupHeader(1);
                var newRetrigger = (TriggeredSpawner.RetriggerLimitMode)EditorGUILayout.EnumPopup("Retrigger Limit Mode", waveSetting.retriggerLimitMode);
                if (newRetrigger != waveSetting.retriggerLimitMode)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Retrigger Limit Mode");
                    waveSetting.retriggerLimitMode = newRetrigger;
                }
                EditorGUILayout.EndVertical();

                switch (waveSetting.retriggerLimitMode)
                {
                    case TriggeredSpawner.RetriggerLimitMode.FrameBased:
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.limitPerXFrm, "Min Frames Between", _settings);
                        break;
                    case TriggeredSpawner.RetriggerLimitMode.TimeBased:
                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.limitPerXSec, "Min Seconds Between", _settings);
                        break;
                }
                EditorGUILayout.EndVertical();
            }
        }

        // repeat wave spawn variable modifiers
        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        var newBonusesEnabled = EditorGUILayout.BeginToggleGroup(" Wave Spawn Bonus & Events", waveSetting.waveSpawnBonusesEnabled);
        if (newBonusesEnabled != waveSetting.waveSpawnBonusesEnabled)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Spawn Bonus & Events");
            waveSetting.waveSpawnBonusesEnabled = newBonusesEnabled;
        }
        DTInspectorUtility.EndGroupHeader();

        if (waveSetting.waveSpawnBonusesEnabled)
        {
            EditorGUI.indentLevel = 0;

            var missingBonusStatNames = new List<string>();
            missingBonusStatNames.AddRange(_allStats);
            missingBonusStatNames.RemoveAll(delegate (string obj)
            {
                return waveSetting.waveSpawnVariableModifiers.HasKey(obj);
            });

            var newCheck = EditorGUILayout.Toggle("Use On First Spawn", waveSetting.useWaveSpawnBonusForBeginning);
            if (newCheck != waveSetting.useWaveSpawnBonusForBeginning)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Use On First Spawn");
                waveSetting.useWaveSpawnBonusForBeginning = newCheck;
            }

            var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingBonusStatNames.ToArray());
            if (newBonusStat != 0)
            {
                AddBonusStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
            }

            if (waveSetting.waveSpawnVariableModifiers.statMods.Count == 0)
            {
                if (waveSetting.waveSpawnBonusesEnabled)
                {
                    DTInspectorUtility.ShowRedErrorBox("You currently are using no modifiers for this wave.");
                }
            }
            else
            {
                EditorGUILayout.Separator();

                int? indexToDelete = null;

                for (var i = 0; i < waveSetting.waveSpawnVariableModifiers.statMods.Count; i++)
                {
                    var modifier = waveSetting.waveSpawnVariableModifiers.statMods[i];

                    var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                    switch (modifier._varTypeToUse)
                    {
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

                    if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                    {
                        indexToDelete = i;
                    }
                }

                if (indexToDelete.HasValue)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Variable Modifier");
                    waveSetting.waveSpawnVariableModifiers.DeleteByIndex(indexToDelete.Value);
                }

                EditorGUILayout.Separator();
            }

            DTInspectorUtility.StartGroupHeader(0, false);
            var newExp = EditorGUILayout.Toggle("Spawn Cust. Events", waveSetting.waveSpawnFireEvents);
            if (newExp != waveSetting.waveSpawnFireEvents)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Cust. Events");
                waveSetting.waveSpawnFireEvents = newExp;
            }

            if (waveSetting.waveSpawnFireEvents)
            {
                DTInspectorUtility.ShowColorWarningBox("When wave starts, fire the Custom Events below");

                EditorGUILayout.BeginHorizontal();
                GUI.contentColor = DTInspectorUtility.AddButtonColor;
                GUILayout.Space(10);
                if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Spawn Custom Event");
                    waveSetting.waveSpawnCustomEvents.Add(new CGKCustomEventToFire());
                }
                GUI.contentColor = Color.white;

                EditorGUILayout.EndHorizontal();

                if (waveSetting.waveSpawnCustomEvents.Count == 0)
                {
                    DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                }

                DTInspectorUtility.VerticalSpace(2);

                int? indexToDelete = null;

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < waveSetting.waveSpawnCustomEvents.Count; i++)
                {
                    var anEvent = waveSetting.waveSpawnCustomEvents[i].CustomEventName;

                    var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                    anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
                        _settings, "Custom Event", ref buttonClicked);

                    if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                    {
                        indexToDelete = i;
                    }

                    if (anEvent == waveSetting.waveSpawnCustomEvents[i].CustomEventName)
                    {
                        continue;
                    }

                    waveSetting.waveSpawnCustomEvents[i].CustomEventName = anEvent;
                }

                if (indexToDelete.HasValue)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Wave Spawn Custom Event");
                    waveSetting.waveSpawnCustomEvents.RemoveAt(indexToDelete.Value);
                }
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.EndToggleGroup();

        if (waveSetting.repeatWaitsForType == TriggeredSpawnerV2.RepeatWaitFor.ItemsEliminated)
        {
            // repeat wave elimination variable modifiers
            DTInspectorUtility.AddSpaceForNonU5();
            DTInspectorUtility.StartGroupHeader(1);
            var newElimBonus = EditorGUILayout.BeginToggleGroup(" Wave Elimination Bonus & Events", waveSetting.waveElimBonusesEnabled);
            if (newElimBonus != waveSetting.waveElimBonusesEnabled)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Elimination Bonus & Events");
                waveSetting.waveElimBonusesEnabled = newElimBonus;
            }
            DTInspectorUtility.EndGroupHeader();

            if (waveSetting.waveElimBonusesEnabled)
            {
                EditorGUI.indentLevel = 0;

                var missingBonusStatNames = new List<string>();
                missingBonusStatNames.AddRange(_allStats);
                missingBonusStatNames.RemoveAll(delegate (string obj)
                {
                    return waveSetting.waveElimVariableModifiers.HasKey(obj);
                });

                var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingBonusStatNames.ToArray());
                if (newBonusStat != 0)
                {
                    AddElimStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
                }

                if (waveSetting.waveElimVariableModifiers.statMods.Count == 0)
                {
                    if (waveSetting.waveElimBonusesEnabled)
                    {
                        DTInspectorUtility.ShowRedErrorBox("You currently are using no modifiers for this wave.");
                    }
                }
                else
                {
                    EditorGUILayout.Separator();

                    int? indexToDelete = null;

                    for (var i = 0; i < waveSetting.waveElimVariableModifiers.statMods.Count; i++)
                    {
                        var modifier = waveSetting.waveElimVariableModifiers.statMods[i];

                        var buttonPressed = DTInspectorUtility.FunctionButtons.None;
                        switch (modifier._varTypeToUse)
                        {
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

                        if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Variable Modifier");
                        waveSetting.waveElimVariableModifiers.DeleteByIndex(indexToDelete.Value);
                    }

                    EditorGUILayout.Separator();
                }

                DTInspectorUtility.StartGroupHeader(0, false);
                var newExp = EditorGUILayout.Toggle("Elimination Cust. Events", waveSetting.waveElimFireEvents);
                if (newExp != waveSetting.waveElimFireEvents)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Elimination Cust. Events");
                    waveSetting.waveElimFireEvents = newExp;
                }

                if (waveSetting.waveElimFireEvents)
                {
                    DTInspectorUtility.ShowColorWarningBox("When wave is eliminated, fire the Custom Events below");

                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Spawn Custom Event");
                        waveSetting.waveElimCustomEvents.Add(new CGKCustomEventToFire());
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (waveSetting.waveElimCustomEvents.Count == 0)
                    {
                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                    }

                    DTInspectorUtility.VerticalSpace(2);

                    int? indexToDelete = null;

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < waveSetting.waveElimCustomEvents.Count; i++)
                    {
                        var anEvent = waveSetting.waveElimCustomEvents[i].CustomEventName;

                        var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
                            _settings, "Custom Event", ref buttonClicked);
                        if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }

                        if (anEvent == waveSetting.waveElimCustomEvents[i].CustomEventName)
                        {
                            continue;
                        }

                        waveSetting.waveElimCustomEvents[i].CustomEventName = anEvent;
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Wave Elimination Custom Event");
                        waveSetting.waveElimCustomEvents.RemoveAt(indexToDelete.Value);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndToggleGroup();
        }

        if (waveSetting.repeatWaitsForType == TriggeredSpawnerV2.RepeatWaitFor.ItemsEliminated) {
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

        if (TriggeredSpawner.eventsThatCanRepeatWave.Contains(eventType))
        {
            DTInspectorUtility.AddSpaceForNonU5();
            DTInspectorUtility.StartGroupHeader(1);
            var newRepeat = EditorGUILayout.BeginToggleGroup(" Repeat Wave", waveSetting.enableRepeatWave);
            if (newRepeat != waveSetting.enableRepeatWave)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Wave");
                waveSetting.enableRepeatWave = newRepeat;
            }
            DTInspectorUtility.EndGroupHeader();
            if (waveSetting.enableRepeatWave)
            {
                var newRepeatMode = (WaveSpecifics.RepeatWaveMode)EditorGUILayout.EnumPopup("Repeat Mode", waveSetting.curWaveRepeatMode);
                if (newRepeatMode != waveSetting.curWaveRepeatMode)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Repeat Mode");
                    waveSetting.curWaveRepeatMode = newRepeatMode;
                }

                switch (waveSetting.curWaveRepeatMode)
                {
                    case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.maxRepeat, "Wave Repetitions", _settings);
                        break;
                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
                        var missingStatNames = new List<string>();
                        missingStatNames.AddRange(_allStats);
                        missingStatNames.RemoveAll(delegate (string obj)
                        {
                            return waveSetting.repeatPassCriteria.HasKey(obj);
                        });

                        var newStat = EditorGUILayout.Popup("Add Variable Limit", 0, missingStatNames.ToArray());
                        if (newStat != 0)
                        {
                            AddStatModifier(missingStatNames[newStat], waveSetting);
                        }

                        if (waveSetting.repeatPassCriteria.statMods.Count == 0)
                        {
                            DTInspectorUtility.ShowRedErrorBox("You have no Variable Limits. Wave will not repeat.");
                        }
                        else
                        {
                            EditorGUILayout.Separator();

                            int? indexToDelete = null;

                            for (var i = 0; i < waveSetting.repeatPassCriteria.statMods.Count; i++)
                            {
                                var modifier = waveSetting.repeatPassCriteria.statMods[i];
                                var buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
                                if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove)
                                {
                                    indexToDelete = i;
                                }

                                KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);
                            }

                            DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                            if (indexToDelete.HasValue)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Modifier");
                                waveSetting.repeatPassCriteria.DeleteByIndex(indexToDelete.Value);
                            }

                            EditorGUILayout.Separator();
                        }

                        break;
                }

                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatWavePauseSec, "Pause Before Repeat", _settings);

                DTInspectorUtility.VerticalSpace(3);
                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemInc, "Spawn Increase", _settings);

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemMinLmt, "Spawn Min Limit", _settings);
                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemLmt, "Spawn Max Limit", _settings);

                var reset = EditorGUILayout.Toggle("Reset On Spawn Lmt Passed", waveSetting.resetOnItemLimitReached);
                if (reset != waveSetting.resetOnItemLimitReached)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Spawn Lmt Passed");
                    waveSetting.resetOnItemLimitReached = reset;
                }

                DTInspectorUtility.VerticalSpace(3);
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeInc, "Time Increase", _settings);
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeMinLmt, "Time Min Limit", _settings);
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeLmt, "Time Max Limit", _settings);
                reset = EditorGUILayout.Toggle("Reset On Time Lmt Passed", waveSetting.resetOnTimeLimitReached);
                if (reset != waveSetting.resetOnTimeLimitReached)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Time Lmt Passed");
                    waveSetting.resetOnTimeLimitReached = reset;
                }

                if (waveSetting.waveSpawnBonusesEnabled)
                {
                    EditorGUI.indentLevel = 0;
                    var newUseRepeatBonus = EditorGUILayout.Toggle("Use Wave Spawn Bonus", waveSetting.useWaveSpawnBonusForRepeats);
                    if (newUseRepeatBonus != waveSetting.useWaveSpawnBonusForRepeats)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Wave Spawn Bonus");
                        waveSetting.useWaveSpawnBonusForRepeats = newUseRepeatBonus;
                    }
                }


                DTInspectorUtility.AddSpaceForNonU5();

                DTInspectorUtility.StartGroupHeader(0, true);
                var newExp = EditorGUILayout.Toggle("Repeat Cust. Events", waveSetting.waveRepeatFireEvents);
                if (newExp != waveSetting.waveRepeatFireEvents)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Cust. Events");
                    waveSetting.waveRepeatFireEvents = newExp;
                }

                EditorGUILayout.EndVertical();

                if (waveSetting.waveRepeatFireEvents)
                {
                    DTInspectorUtility.ShowColorWarningBox("When wave repeats, fire the Custom Events below");

                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50)))
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Repeat Custom Event");
                        waveSetting.waveRepeatCustomEvents.Add(new CGKCustomEventToFire());
                    }
                    GUI.contentColor = Color.white;

                    EditorGUILayout.EndHorizontal();

                    if (waveSetting.waveRepeatCustomEvents.Count == 0)
                    {
                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                    }

                    DTInspectorUtility.VerticalSpace(2);

                    int? indexToDelete = null;

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < waveSetting.waveRepeatCustomEvents.Count; i++)
                    {
                        var anEvent = waveSetting.waveRepeatCustomEvents[i].CustomEventName;

                        var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
                            _settings, "Custom Event", ref buttonClicked);
                        if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove)
                        {
                            indexToDelete = i;
                        }

                        if (anEvent == waveSetting.waveRepeatCustomEvents[i].CustomEventName)
                        {
                            continue;
                        }

                        waveSetting.waveRepeatCustomEvents[i].CustomEventName = anEvent;
                    }

                    if (indexToDelete.HasValue)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove Wave Repeat Custom Event");
                        waveSetting.waveRepeatCustomEvents.RemoveAt(indexToDelete.Value);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndToggleGroup();
        }

        // show randomizations
        const string variantTag = " Randomization";

        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        var newRand = EditorGUILayout.BeginToggleGroup(variantTag, waveSetting.enableRandomizations);
        if (newRand != waveSetting.enableRandomizations)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Randomization");
            waveSetting.enableRandomizations = newRand;
            _changedWave = waveSetting;
        }
        DTInspectorUtility.EndGroupHeader();
        if (waveSetting.enableRandomizations)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Random Rotation");

            var newRandX = GUILayout.Toggle(waveSetting.randomXRotation, "X");
            if (newRandX != waveSetting.randomXRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random X Rotation");
                waveSetting.randomXRotation = newRandX;
                _changedWave = waveSetting;
            }
            GUILayout.Space(10);
            var newRandY = GUILayout.Toggle(waveSetting.randomYRotation, "Y");
            if (newRandY != waveSetting.randomYRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Y Rotation");
                waveSetting.randomYRotation = newRandY;
                _changedWave = waveSetting;
            }
            GUILayout.Space(10);
            var newRandZ = GUILayout.Toggle(waveSetting.randomZRotation, "Z");
            if (newRandZ != waveSetting.randomZRotation)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Z Rotation");
                waveSetting.randomZRotation = newRandZ;
                _changedWave = waveSetting;
            }
            EditorGUILayout.EndHorizontal();

            if (waveSetting.randomXRotation)
            {
                oldFloat = waveSetting.randomXRotMin.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMin, "Rand. X Rot. Min", _settings);
                if (oldFloat != waveSetting.randomXRotMin.Value)
                {
                    _changedWave = waveSetting;
                }

                oldFloat = waveSetting.randomXRotMax.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMax, "Rand. X Rot. Max", _settings);
                if (oldFloat != waveSetting.randomXRotMax.Value)
                {
                    _changedWave = waveSetting;
                }
            }
            if (waveSetting.randomYRotation)
            {
                oldFloat = waveSetting.randomYRotMin.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMin, "Rand. Y Rot. Min", _settings);
                if (oldFloat != waveSetting.randomYRotMin.Value)
                {
                    _changedWave = waveSetting;
                }

                oldFloat = waveSetting.randomYRotMax.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMax, "Rand. Y Rot. Max", _settings);
                if (oldFloat != waveSetting.randomYRotMax.Value)
                {
                    _changedWave = waveSetting;
                }
            }
            if (waveSetting.randomZRotation)
            {
                oldFloat = waveSetting.randomZRotMin.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMin, "Rand. Z Rot. Min", _settings);
                if (oldFloat != waveSetting.randomZRotMin.Value)
                {
                    _changedWave = waveSetting;
                }

                oldFloat = waveSetting.randomZRotMax.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMax, "Rand. Z Rot. Max", _settings);
                if (oldFloat != waveSetting.randomZRotMax.Value)
                {
                    _changedWave = waveSetting;
                }
            }

            EditorGUILayout.Separator();

            oldFloat = waveSetting.randomDistX.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistX, "Rand. Distance X", _settings);
            if (oldFloat != waveSetting.randomDistX.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.randomDistY.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistY, "Rand. Distance Y", _settings);
            if (oldFloat != waveSetting.randomDistY.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.randomDistZ.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistZ, "Rand. Distance Z", _settings);
            if (oldFloat != waveSetting.randomDistZ.Value)
            {
                _changedWave = waveSetting;
            }
        }
        EditorGUILayout.EndToggleGroup();


        // show increments
        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        var incTag = " Incremental Settings";
        var newIncrements = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enableIncrements);
        if (newIncrements != waveSetting.enableIncrements)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Incremental Settings");
            waveSetting.enableIncrements = newIncrements;
            _changedWave = waveSetting;
        }
        DTInspectorUtility.EndGroupHeader();
        if (waveSetting.enableIncrements)
        {
            oldFloat = waveSetting.incrementPositionX.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionX, "Distance X", _settings);
            if (oldFloat != waveSetting.incrementPositionX.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.incrementPositionY.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionY, "Distance Y", _settings);
            if (oldFloat != waveSetting.incrementPositionY.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.incrementPositionZ.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionZ, "Distance Z", _settings);
            if (oldFloat != waveSetting.incrementPositionZ.Value)
            {
                _changedWave = waveSetting;
            }

            EditorGUILayout.Separator();

            if (waveSetting.enableRandomizations && waveSetting.randomXRotation)
            {
                DTInspectorUtility.ShowColorWarningBox("Rotation X - cannot be used with Random Rotation X.");
            }
            else
            {
                oldFloat = waveSetting.incrementRotX.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotX, "Rotation X", _settings);
                if (oldFloat != waveSetting.incrementRotX.Value)
                {
                    _changedWave = waveSetting;
                }
            }

            if (waveSetting.enableRandomizations && waveSetting.randomYRotation)
            {
                DTInspectorUtility.ShowColorWarningBox("Rotation Y - cannot be used with Random Rotation Y.");
            }
            else
            {
                oldFloat = waveSetting.incrementRotY.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotY, "Rotation Y", _settings);
                if (oldFloat != waveSetting.incrementRotY.Value)
                {
                    _changedWave = waveSetting;
                }
            }

            if (waveSetting.enableRandomizations && waveSetting.randomZRotation)
            {
                DTInspectorUtility.ShowColorWarningBox("Rotation Z - cannot be used with Random Rotation Z.");
            }
            else
            {
                oldFloat = waveSetting.incrementRotZ.Value;
                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotZ, "Rotation Z", _settings);
                if (oldFloat != waveSetting.incrementRotZ.Value)
                {
                    _changedWave = waveSetting;
                }
            }

            var newIncKc = EditorGUILayout.Toggle("Keep Center", waveSetting.enableKeepCenter);
            if (newIncKc != waveSetting.enableKeepCenter)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Keep Center");
                waveSetting.enableKeepCenter = newIncKc;
                _changedWave = waveSetting;
            }
        }
        EditorGUILayout.EndToggleGroup();


        // show increments
        incTag = " Post-spawn Nudge Settings";
        DTInspectorUtility.AddSpaceForNonU5();
        DTInspectorUtility.StartGroupHeader(1);
        var newPostEnabled = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enablePostSpawnNudge);
        if (newPostEnabled != waveSetting.enablePostSpawnNudge)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Post-spawn Nudge Settings");
            waveSetting.enablePostSpawnNudge = newPostEnabled;
            _changedWave = waveSetting;
        }
        DTInspectorUtility.EndGroupHeader();
        if (waveSetting.enablePostSpawnNudge)
        {
            oldFloat = waveSetting.postSpawnNudgeFwd.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeFwd, "Nudge Forward", _settings);
            if (oldFloat != waveSetting.postSpawnNudgeFwd.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.postSpawnNudgeRgt.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeRgt, "Nudge Right", _settings);
            if (oldFloat != waveSetting.postSpawnNudgeRgt.Value)
            {
                _changedWave = waveSetting;
            }

            oldFloat = waveSetting.postSpawnNudgeDwn.Value;
            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeDwn, "Nudge Down", _settings);
            if (oldFloat != waveSetting.postSpawnNudgeDwn.Value)
            {
                _changedWave = waveSetting;
            }
        }
        EditorGUILayout.EndToggleGroup();

        DTInspectorUtility.EndGroupedControls();

        DTInspectorUtility.VerticalSpace(3);
    }

    private List<string> GetUnusedEventTypes()
    {
        var unusedEvents = new List<string> { "-None-" };
        if (!IsFirstWaveEnabled(_settings.enableWaves))
        {
            unusedEvents.Add("Enabled");
        }
        if (!IsFirstWaveEnabled(_settings.disableWaves))
        {
            unusedEvents.Add("Disabled");
        }
        if (!IsFirstWaveEnabled(_settings.visibleWaves))
        {
            unusedEvents.Add("Visible");
        }
        if (!IsFirstWaveEnabled(_settings.invisibleWaves))
        {
            unusedEvents.Add("Invisible");
        }

        if (_settings.unityUIMode == TriggeredSpawner.Unity_UIVersion.Legacy)
        {
            if (!IsFirstWaveEnabled(_settings.mouseOverWaves))
            {
                unusedEvents.Add("Mouse Over (Legacy)");
            }
            if (!IsFirstWaveEnabled(_settings.mouseClickWaves))
            {
                unusedEvents.Add("Mouse Click (Legacy)");
            }
        }

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        if (_settings.unityUIMode == TriggeredSpawner.Unity_UIVersion.uGUI)
        {
            if (_hasSlider && !IsFirstWaveEnabled(_settings.unitySliderChangedWaves))
            {
                unusedEvents.Add("Slider Changed (uGUI)");
            }
            if (_hasButton && !IsFirstWaveEnabled(_settings.unityButtonClickedWaves))
            {
                unusedEvents.Add("Button Click (uGUI)");
            }
            if (_hasRect)
            {
                if (!IsFirstWaveEnabled(_settings.unityPointerDownWaves))
                {
                    unusedEvents.Add("Pointer Down (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityPointerUpWaves))
                {
                    unusedEvents.Add("Pointer Up (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityPointerEnterWaves))
                {
                    unusedEvents.Add("Pointer Enter (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityPointerExitWaves))
                {
                    unusedEvents.Add("Pointer Exit (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityDragWaves))
                {
                    unusedEvents.Add("Drag (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityDropWaves))
                {
                    unusedEvents.Add("Drop (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityScrollWaves))
                {
                    unusedEvents.Add("Scroll (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityUpdateSelectedWaves))
                {
                    unusedEvents.Add("Update Selected (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unitySelectWaves))
                {
                    unusedEvents.Add("Select (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityDeselectWaves))
                {
                    unusedEvents.Add("Deselect (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityMoveWaves))
                {
                    unusedEvents.Add("Move (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityInitializePotentialDragWaves))
                {
                    unusedEvents.Add("Init. Potential Drag (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityBeginDragWaves))
                {
                    unusedEvents.Add("Begin Drag (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityEndDragWaves))
                {
                    unusedEvents.Add("End Drag (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unitySubmitWaves))
                {
                    unusedEvents.Add("Submit (uGUI)");
                }
                if (!IsFirstWaveEnabled(_settings.unityCancelWaves))
                {
                    unusedEvents.Add("Cancel (uGUI)");
                }
            }
        }
#endif

        if (!IsFirstWaveEnabled(_settings.collisionWaves))
        {
            unusedEvents.Add("Collision Enter");
        }
        if (!IsFirstWaveEnabled(_settings.triggerEnterWaves))
        {
            unusedEvents.Add("Trigger Enter");
        }
        if (!IsFirstWaveEnabled(_settings.triggerStayWaves))
        {
            unusedEvents.Add("Trigger Stay");
        }
        if (!IsFirstWaveEnabled(_settings.triggerExitWaves))
        {
            unusedEvents.Add("Trigger Exit");
        }
        if (!IsFirstWaveEnabled(_settings.collision2dWaves))
        {
            unusedEvents.Add("2D Collision Enter");
        }
        if (!IsFirstWaveEnabled(_settings.triggerStay2dWaves))
        {
            unusedEvents.Add("2D Trigger Stay");
        }
        if (!IsFirstWaveEnabled(_settings.triggerEnter2dWaves))
        {
            unusedEvents.Add("2D Trigger Enter");
        }
        if (!IsFirstWaveEnabled(_settings.triggerExit2dWaves))
        {
            unusedEvents.Add("2D Trigger Exit");
        }
        if (!IsFirstWaveEnabled(_settings.codeTriggeredWaves1))
        {
            unusedEvents.Add("Code-Triggered 1");
        }
        if (!IsFirstWaveEnabled(_settings.codeTriggeredWaves2))
        {
            unusedEvents.Add("Code-Triggered 2");
        }
        if (!IsFirstWaveEnabled(_settings.spawnedWaves))
        {
            unusedEvents.Add("Spawned");
        }
        if (!IsFirstWaveEnabled(_settings.despawnedWaves))
        {
            unusedEvents.Add("Despawned");
        }
        if (!IsFirstWaveEnabled(_settings.clickWaves))
        {
            unusedEvents.Add("NGUI OnClick");
        }

        unusedEvents.Add("Custom Event");

        return unusedEvents;
    }

    private void AddActiveEventToList(ref List<TriggeredWaveSpecifics> eventList)
    {
        if (eventList == null)
        {
            return;
        }

        eventList = new List<TriggeredWaveSpecifics>(1) {
            new TriggeredWaveSpecifics {
                enableWave = true
            }
        };
    }

    private void ActivateEvent(int index, List<string> unusedEvents)
    {
        var item = unusedEvents[index];

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "activate Event");

        switch (item)
        {
            case "Code-Triggered 1":
                AddActiveEventToList(ref _settings.codeTriggeredWaves1);
                break;
            case "Code-Triggered 2":
                AddActiveEventToList(ref _settings.codeTriggeredWaves2);
                break;
            case "Invisible":
                AddActiveEventToList(ref _settings.invisibleWaves);
                break;
            case "Mouse Click (Legacy)":
                AddActiveEventToList(ref _settings.mouseClickWaves);
                break;
            case "Mouse Over (Legacy)":
                AddActiveEventToList(ref _settings.mouseOverWaves);
                break;
            case "NGUI OnClick":
                AddActiveEventToList(ref _settings.clickWaves);
                break;
            case "Collision Enter":
                AddActiveEventToList(ref _settings.collisionWaves);
                break;
            case "Despawned":
                AddActiveEventToList(ref _settings.despawnedWaves);
                break;
            case "Disabled":
                AddActiveEventToList(ref _settings.disableWaves);
                break;
            case "Enabled":
                AddActiveEventToList(ref _settings.enableWaves);
                break;
            case "Spawned":
                AddActiveEventToList(ref _settings.spawnedWaves);
                break;
            case "Trigger Enter":
                AddActiveEventToList(ref _settings.triggerEnterWaves);
                break;
            case "Trigger Stay":
                AddActiveEventToList(ref _settings.triggerStayWaves);
                break;
            case "Trigger Exit":
                AddActiveEventToList(ref _settings.triggerExitWaves);
                break;
            case "Visible":
                AddActiveEventToList(ref _settings.visibleWaves);
                break;
            case "2D Collision Enter":
                AddActiveEventToList(ref _settings.collision2dWaves);
                break;
            case "2D Trigger Enter":
                AddActiveEventToList(ref _settings.triggerEnter2dWaves);
                break;
            case "2D Trigger Stay":
                AddActiveEventToList(ref _settings.triggerStay2dWaves);
                break;
            case "2D Trigger Exit":
                AddActiveEventToList(ref _settings.triggerExit2dWaves);
                break;
            case "Slider Changed (uGUI)":
                AddActiveEventToList(ref _settings.unitySliderChangedWaves);
                break;
            case "Button Click (uGUI)":
                AddActiveEventToList(ref _settings.unityButtonClickedWaves);
                break;
            case "Pointer Down (uGUI)":
                AddActiveEventToList(ref _settings.unityPointerDownWaves);
                break;
            case "Pointer Up (uGUI)":
                AddActiveEventToList(ref _settings.unityPointerUpWaves);
                break;
            case "Pointer Enter (uGUI)":
                AddActiveEventToList(ref _settings.unityPointerEnterWaves);
                break;
            case "Pointer Exit (uGUI)":
                AddActiveEventToList(ref _settings.unityPointerExitWaves);
                break;
            case "Drag (uGUI)":
                AddActiveEventToList(ref _settings.unityDragWaves);
                break;
            case "Drop (uGUI)":
                AddActiveEventToList(ref _settings.unityDropWaves);
                break;
            case "Scroll (uGUI)":
                AddActiveEventToList(ref _settings.unityScrollWaves);
                break;
            case "Update Selected (uGUI)":
                AddActiveEventToList(ref _settings.unityUpdateSelectedWaves);
                break;
            case "Select (uGUI)":
                AddActiveEventToList(ref _settings.unitySelectWaves);
                break;
            case "Deselect (uGUI)":
                AddActiveEventToList(ref _settings.unityDeselectWaves);
                break;
            case "Move (uGUI)":
                AddActiveEventToList(ref _settings.unityMoveWaves);
                break;
            case "Init. Potential Drag (uGUI)":
                AddActiveEventToList(ref _settings.unityInitializePotentialDragWaves);
                break;
            case "Begin Drag (uGUI)":
                AddActiveEventToList(ref _settings.unityBeginDragWaves);
                break;
            case "End Drag (uGUI)":
                AddActiveEventToList(ref _settings.unityEndDragWaves);
                break;
            case "Submit (uGUI)":
                AddActiveEventToList(ref _settings.unitySubmitWaves);
                break;
            case "Cancel (uGUI)":
                AddActiveEventToList(ref _settings.unityCancelWaves);
                break;
            case "Custom Event":
                CreateCustomEvent(false);
                break;
        }
    }

    private void AddStatModifier(string modifierName, TriggeredWaveSpecifics spec)
    {
        if (spec.repeatPassCriteria.HasKey(modifierName))
        {
            DTInspectorUtility.ShowAlert("This wave already has a Variable Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Variable Limit");

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        spec.repeatPassCriteria.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
    }

    private TriggeredWaveSpecifics GetFirstWaveIfAny(List<TriggeredWaveSpecifics> waveList)
    {
        if (waveList.Count == 0)
        {
            return null;
        }

        return waveList[0];
    }

    private bool IsFirstWaveEnabled(List<TriggeredWaveSpecifics> waveList)
    {
        var firstWave = GetFirstWaveIfAny(waveList);

        if (firstWave == null)
        {
            return false;
        }

        return firstWave.enableWave;
    }

    private void CollapseFirstWaveIfAny(List<TriggeredWaveSpecifics> waveList)
    {
        var firstWave = GetFirstWaveIfAny(waveList);

        if (firstWave == null)
        {
            return;
        }

        firstWave.isExpanded = false;
    }

    private void AddActiveLimit(string modifierName, TriggeredSpawnerV2 spec)
    {
        if (spec.activeItemCriteria.HasKey(modifierName))
        {
            DTInspectorUtility.ShowAlert("This item already has a Active Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Active Limit");

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        spec.activeItemCriteria.statMods.Add(new WorldVariableRange(modifierName, myVar.varType));
    }

    private void AddBonusStatModifier(string modifierName, TriggeredWaveSpecifics waveSpec)
    {
        if (waveSpec.waveSpawnVariableModifiers.HasKey(modifierName))
        {
            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Repeat Bonus modifier");

        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

        waveSpec.waveSpawnVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    }

    private void AddElimStatModifier(string modifierName, TriggeredWaveSpecifics waveSpec)
    {
        if (waveSpec.waveElimVariableModifiers.HasKey(modifierName))
        {
            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Elimination Bonus modifier");

        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

        waveSpec.waveElimVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    }

    private static List<string> PoolNames {
        get {
            return LevelSettings.GetSortedPrefabPoolNames();
        }
    }

    private void CreateCustomEvent(bool recordUndo)
    {
        var newWave = new TriggeredWaveSpecifics { customEventActive = true, isCustomEvent = true, enableWave = true };

        if (recordUndo)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Custom Event");
        }

        _settings.userDefinedEventWaves.Add(newWave);
    }
}