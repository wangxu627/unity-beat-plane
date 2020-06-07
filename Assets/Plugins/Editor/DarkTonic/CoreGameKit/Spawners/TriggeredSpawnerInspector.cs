using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
using UnityEngine.UI;
#endif

[CustomEditor(typeof(TriggeredSpawner), true)]
// ReSharper disable once CheckNamespace
public class TriggeredSpawnerInspector : Editor {
    private TriggeredSpawner _settings;
    //private List<string> _allStats;
    private bool _levelSettingsInScene;
    //private List<string> _customEventNames;
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    private bool _hasSlider;
    private bool _hasButton;
    private bool _hasRect;
    //private TriggeredWaveSpecifics _waveToVisualize;
    private TriggeredWaveSpecifics _changedWave;
    //private List<TriggeredWaveSpecifics> _allWaves = new List<TriggeredWaveSpecifics>();

    // ReSharper restore ConvertToConstant.Local
    // ReSharper restore FieldCanBeMadeReadOnly.Local

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        _settings = (TriggeredSpawner)target;

        WorldVariableTracker.ClearInGamePlayerStats();

        //_allStats = KillerVariablesHelper.AllStatNames;

        LevelSettings.Instance = null; // clear cached version

        var ls = LevelSettings.Instance;

        _levelSettingsInScene = ls != null;

        if (_levelSettingsInScene) {
            // ReSharper disable once PossibleNullReferenceException
            //_customEventNames = ls.CustomEventNames;
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

        //_changedWave = null;
        //_waveToVisualize = null;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (_hasRect || _hasButton || _hasSlider || showNewUIEvents) { }

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);

        DTInspectorUtility.ShowRedErrorBox("Triggered Spawner is now deprecated. Use the button below to copy your settings to Triggered Spawner V2 (which it will create here), apply changes (if prefab), then remove this script.");
        DTInspectorUtility.ShowColorWarningBox("Or (global option) select the menu item Window -> Core GameKit -> Upgrade Triggered Spawners to V2, which will do it for all Triggered Spawner scripts in the current Scene. Then select the menu item under that to 'Remove all V1 Triggered Spawners' and clean up the unused scripts.");

        if (!Application.isPlaying) {
            if (GUILayout.Button("Copy to V2", GUILayout.Width(100))) {
                CopyToV2();
            }
        }

        //        EditorGUI.indentLevel = 0;
        //        _isDirty = false;

        //        if (!SpawnUtility.IsActive(_settings.gameObject)) {
        //            DTInspectorUtility.RedBoldMessage("Despawned and inactive!");
        //        } else if (_settings.activeMode == LevelSettings.ActiveItemMode.Never) {
        //            DTInspectorUtility.RedBoldMessage("Spawner disabled by Active Mode setting");
        //        } else if (Application.isPlaying) {
        //            if (_settings.GameIsOverForSpawner) {
        //                DTInspectorUtility.RedBoldMessage("Spawner disabled by Game Over Behavior setting");
        //            } else if (_settings.SpawnerIsPaused) {
        //                DTInspectorUtility.RedBoldMessage("Spawner paused by Wave Pause Behavior setting");
        //            }
        //        }

        //        var waveActivated = false;
        //        DTInspectorUtility.StartGroupHeader();
        //        var newActive = (LevelSettings.ActiveItemMode)EditorGUILayout.EnumPopup("Active Mode", _settings.activeMode);
        //        if (newActive != _settings.activeMode) {
        //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Mode");
        //            _settings.activeMode = newActive;

        //            if (!Application.isPlaying) {
        //                _settings.gameObject.DestroyChildrenImmediateWithMarker();
        //            }

        //            // ReSharper disable once ConvertIfToOrExpression
        //            if (_settings.activeMode != LevelSettings.ActiveItemMode.Never) {
        //                waveActivated = true;
        //            }
        //        }
        //        EditorGUILayout.EndVertical();

        //        switch (_settings.activeMode) {
        //            case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
        //            case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
        //                var missingStatNames = new List<string>();
        //                missingStatNames.AddRange(_allStats);
        //                missingStatNames.RemoveAll(delegate(string obj) {
        //                    return _settings.activeItemCriteria.HasKey(obj);
        //                });

        //                var newStat = EditorGUILayout.Popup("Add Active Limit", 0, missingStatNames.ToArray());
        //                if (newStat != 0) {
        //                    AddActiveLimit(missingStatNames[newStat], _settings);
        //                }

        //                if (_settings.activeItemCriteria.statMods.Count == 0) {
        //                    DTInspectorUtility.ShowRedErrorBox("You have no Active Limits. Spawner will never be Active.");
        //                } else {
        //                    EditorGUILayout.Separator();

        //                    int? indexToDelete = null;

        //                    for (var j = 0; j < _settings.activeItemCriteria.statMods.Count; j++) {
        //                        var modifier = _settings.activeItemCriteria.statMods[j];
        //                        EditorGUILayout.BeginHorizontal();
        //                        GUILayout.Space(15);
        //                        var statName = modifier._statName;
        //                        GUILayout.Label(statName);

        //                        GUILayout.FlexibleSpace();
        //                        GUILayout.Label("Min");

        //                        switch (modifier._varTypeToUse) {
        //                            case WorldVariableTracker.VariableType._integer:
        //                                var newMin = EditorGUILayout.IntField(modifier._modValueIntMin, GUILayout.MaxWidth(60));
        //                                if (newMin != modifier._modValueIntMin) {
        //                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Min");
        //                                    modifier._modValueIntMin = newMin;
        //                                }

        //                                GUILayout.Label("Max");
        //                                var newMax = EditorGUILayout.IntField(modifier._modValueIntMax, GUILayout.MaxWidth(60));
        //                                if (newMax != modifier._modValueIntMax) {
        //                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Max");
        //                                    modifier._modValueIntMax = newMax;
        //                                }
        //                                break;
        //                            case WorldVariableTracker.VariableType._float:
        //                                var newMinFloat = EditorGUILayout.FloatField(modifier._modValueFloatMin, GUILayout.MaxWidth(60));
        //                                if (newMinFloat != modifier._modValueFloatMin) {
        //                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Min");
        //                                    modifier._modValueFloatMin = newMinFloat;
        //                                }

        //                                GUILayout.Label("Max");
        //                                var newMaxFloat = EditorGUILayout.FloatField(modifier._modValueFloatMax, GUILayout.MaxWidth(60));
        //                                if (newMaxFloat != modifier._modValueFloatMax) {
        //                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Modifier Max");
        //                                    modifier._modValueFloatMax = newMaxFloat;
        //                                }
        //                                break;
        //                            default:
        //                                Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
        //                                break;
        //                        }
        //                        GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
        //                        if (GUILayout.Button(new GUIContent("Delete", "Remove this limit"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(64))) {
        //                            indexToDelete = j;
        //                        }
        //                        GUI.backgroundColor = Color.white;
        //                        GUILayout.Space(5);
        //                        EditorGUILayout.EndHorizontal();

        //                        KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

        //                        var min = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMin : modifier._modValueFloatMin;
        //                        var max = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMax : modifier._modValueFloatMax;

        //                        if (min > max) {
        //                            DTInspectorUtility.ShowRedErrorBox(modifier._statName + " Min cannot exceed Max, please fix!");
        //                        }
        //                    }

        //                    DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
        //                    if (indexToDelete.HasValue) {
        //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Modifier");
        //                        _settings.activeItemCriteria.DeleteByIndex(indexToDelete.Value);
        //                    }

        //                    EditorGUILayout.Separator();
        //                }

        //                break;
        //        }
        //        EditorGUILayout.EndVertical();

        //		var newGO = (TriggeredSpawner.GameOverBehavior)EditorGUILayout.EnumPopup("Game Over Behavior", _settings.gameOverBehavior);
        //		if (newGO != _settings.gameOverBehavior) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Game Over Behavior");
        //			_settings.gameOverBehavior = newGO;
        //		}

        //		var newPause = (TriggeredSpawner.WavePauseBehavior)EditorGUILayout.EnumPopup("Wave Pause Behavior", _settings.wavePauseBehavior);
        //		if (newPause != _settings.wavePauseBehavior) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Pause Behavior");
        //			_settings.wavePauseBehavior = newPause;
        //		}

        //        var newUI = (TriggeredSpawner.Unity_UIVersion)EditorGUILayout.EnumPopup("Unity UI Version", _settings.unityUIMode);
        //        if (newUI != _settings.unityUIMode) {
        //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Unity UI Version");
        //            _settings.unityUIMode = newUI;
        //        }

        //        var childSpawnerCount = TriggeredSpawner.GetChildSpawners(_settings.transform).Count;

        //        var newSource = (TriggeredSpawner.SpawnerEventSource)EditorGUILayout.EnumPopup("Trigger Source", _settings.eventSourceType);
        //        if (newSource != _settings.eventSourceType) {
        //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Trigger Source");
        //            _settings.eventSourceType = newSource;
        //        }

        //        if (_settings.eventSourceType == TriggeredSpawner.SpawnerEventSource.ReceiveFromParent && _settings.transform.parent == null) {
        //            DTInspectorUtility.ShowRedErrorBox("Illegal Trigger Source - this prefab has no parent.");
        //        }

        //        if (childSpawnerCount > 0) {
        //            var newTransmit = EditorGUILayout.Toggle("Propagate Triggers", _settings.transmitEventsToChildren);
        //            if (newTransmit != _settings.transmitEventsToChildren) {
        //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Propagate Triggers");
        //                _settings.transmitEventsToChildren = newTransmit;
        //            }
        //        } else {
        //            DTInspectorUtility.ShowColorWarningBox("Cannot propagate events with no child spawners");
        //        }

        //		var newOutside = EditorGUILayout.Toggle(new GUIContent("Spawn Outside Pool", "If this is checked, everything spawned from this will not reside under Pool Boss in the Hierarchy, but instead with no parent Game Object."), _settings.spawnOutsidePool);
        //		if (newOutside != _settings.spawnOutsidePool) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawn Outside Pool");
        //			_settings.spawnOutsidePool = newOutside;
        //		}

        //        EditorGUI.indentLevel = 0;
        //        var newLogMissing = EditorGUILayout.Toggle("Log Missing Events", _settings.logMissingEvents);
        //        if (newLogMissing != _settings.logMissingEvents) {
        //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Log Missing Events");
        //            _settings.logMissingEvents = newLogMissing;
        //        }

        //		var hadNoListener = _settings.listener == null;
        //		var newListener = (TriggeredSpawnerListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(TriggeredSpawnerListener), true);
        //		if (newListener != _settings.listener) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
        //			_settings.listener = newListener;
        //			if (hadNoListener && _settings.listener != null) {
        //				_settings.listener.sourceSpawnerName = _settings.transform.name;
        //			}
        //		}

        //        var unusedEvents = GetUnusedEventTypes();

        //		DTInspectorUtility.AddSpaceForNonU5();
        //		DTInspectorUtility.StartGroupHeader();
        //		var newUseLayer = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Layer Mode", _settings.spawnLayerMode);
        //		if (newUseLayer != _settings.spawnLayerMode) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Layer Mode");
        //			_settings.spawnLayerMode = newUseLayer;
        //		}
        //		EditorGUILayout.EndVertical();

        //		if (_settings.spawnLayerMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom) {
        //			EditorGUI.indentLevel = 0;

        //			var newCustomLayer = EditorGUILayout.LayerField("Custom Spawn Layer", _settings.spawnCustomLayer);
        //			if (newCustomLayer != _settings.spawnCustomLayer) {
        //				UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Layer");
        //				_settings.spawnCustomLayer = newCustomLayer;
        //			}
        //		}

        //		if (_settings.spawnLayerMode != WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnPrefabSettings) {
        //			var newRecurse = EditorGUILayout.Toggle("Apply Layer Recursively", _settings.applyLayerRecursively);
        //			if (newRecurse != _settings.applyLayerRecursively) {
        //				UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Apply Layer Recursively");
        //				_settings.applyLayerRecursively = newRecurse;
        //			}
        //		}

        //		EditorGUILayout.EndVertical();
        //		DTInspectorUtility.AddSpaceForNonU5();

        //		DTInspectorUtility.StartGroupHeader();
        //		EditorGUI.indentLevel = 0;
        //		var newUseTag = (WaveSyncroPrefabSpawner.SpawnLayerTagMode)EditorGUILayout.EnumPopup("Spawn Tag Mode", _settings.spawnTagMode);
        //		if (newUseTag != _settings.spawnTagMode) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Tag Mode");
        //			_settings.spawnTagMode = newUseTag;
        //		}
        //		EditorGUILayout.EndVertical();

        //		if (_settings.spawnTagMode == WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom) {
        //			EditorGUI.indentLevel = 0;
        //			var newCustomTag = EditorGUILayout.TagField("Custom Spawn Tag", _settings.spawnCustomTag);
        //			if (newCustomTag != _settings.spawnCustomTag) {
        //				UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Spawn Tag");
        //				_settings.spawnCustomTag = newCustomTag;
        //			}
        //		}
        //		EditorGUILayout.EndVertical();
        //		DTInspectorUtility.AddSpaceForNonU5();

        //        var newEventindex = EditorGUILayout.Popup("Event To Activate", 0, unusedEvents.ToArray());

        //        if (newEventindex > 0) {
        //			_isDirty = true;
        //            ActivateEvent(newEventindex, unusedEvents);
        //        }

        //		GUI.contentColor = DTInspectorUtility.BrightButtonColor;
        //		if (GUILayout.Button("Collapse All Events", EditorStyles.toolbarButton, GUILayout.Width(120))) {
        //			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Collapse All Sections");

        //			_settings.enableWave.isExpanded = false;
        //			_settings.disableWave.isExpanded = false;
        //			_settings.visibleWave.isExpanded = false;
        //			_settings.invisibleWave.isExpanded = false;
        //			_settings.mouseOverWave.isExpanded = false;
        //			_settings.mouseClickWave.isExpanded = false;
        //			_settings.collisionWave.isExpanded = false;
        //			_settings.triggerEnterWave.isExpanded = false;
        //			_settings.triggerExitWave.isExpanded = false;
        //			_settings.spawnedWave.isExpanded = false;
        //			_settings.despawnedWave.isExpanded = false;
        //			_settings.codeTriggeredWave1.isExpanded = false;
        //			_settings.codeTriggeredWave2.isExpanded = false;
        //			_settings.clickWave.isExpanded = false;
        //			_settings.collision2dWave.isExpanded = false;
        //			_settings.triggerEnter2dWave.isExpanded = false;
        //			_settings.triggerExit2dWave.isExpanded = false;
        //			_settings.unitySliderChangedWave.isExpanded = false;
        //			_settings.unityButtonClickedWave.isExpanded = false;
        //			_settings.unityPointerDownWave.isExpanded = false;
        //			_settings.unityPointerUpWave.isExpanded = false;
        //			_settings.unityPointerEnterWave.isExpanded = false;
        //			_settings.unityPointerExitWave.isExpanded = false;
        //			_settings.unityDragWave.isExpanded = false;
        //			_settings.unityDropWave.isExpanded = false;
        //			_settings.unityScrollWave.isExpanded = false;
        //			_settings.unityUpdateSelectedWave.isExpanded = false;
        //			_settings.unitySelectWave.isExpanded = false;
        //			_settings.unityDeselectWave.isExpanded = false;
        //			_settings.unityMoveWave.isExpanded = false;
        //			_settings.unityInitializePotentialDragWave.isExpanded = false;
        //			_settings.unityBeginDragWave.isExpanded = false;
        //			_settings.unityEndDragWave.isExpanded = false;
        //			_settings.unitySubmitWave.isExpanded = false;
        //			_settings.unityCancelWave.isExpanded = false;

        //			for (var i = 0; i < _settings.userDefinedEventWaves.Count; i++) {
        //				_settings.userDefinedEventWaves[i].isExpanded = false;
        //			}
        //		}
        //		GUI.contentColor = Color.white;

        //        DTInspectorUtility.VerticalSpace(3);

        //        _allWaves.Clear();

        //        if (_settings.enableWave.enableWave) {
        //            RenderTriggeredWave(_settings.enableWave, "Enabled Event", TriggeredSpawner.EventType.OnEnabled);
        //        }
        //        if (_settings.disableWave.enableWave) {
        //            RenderTriggeredWave(_settings.disableWave, "Disabled Event", TriggeredSpawner.EventType.OnDisabled);
        //        }
        //        if (_settings.visibleWave.enableWave) {
        //            RenderTriggeredWave(_settings.visibleWave, "Visible Event", TriggeredSpawner.EventType.Visible);
        //        }
        //        if (_settings.invisibleWave.enableWave) {
        //            RenderTriggeredWave(_settings.invisibleWave, "Invisible Event", TriggeredSpawner.EventType.Invisible);
        //        }
        //        if (_settings.mouseOverWave.enableWave) {
        //            RenderTriggeredWave(_settings.mouseOverWave, "Mouse Over (Legacy) Event", TriggeredSpawner.EventType.MouseOver_Legacy);
        //        }
        //        if (_settings.mouseClickWave.enableWave) {
        //            RenderTriggeredWave(_settings.mouseClickWave, "Mouse Click (Legacy) Event", TriggeredSpawner.EventType.MouseClick_Legacy);
        //        }

		//#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        //        if (showNewUIEvents) {
        //            if (_hasSlider && _settings.unitySliderChangedWave.enableWave) {
        //                RenderTriggeredWave(_settings.unitySliderChangedWave, "Slider Changed (uGUI) Event", TriggeredSpawner.EventType.SliderChanged_uGUI);
        //            }
        //            if (_hasButton && _settings.unityButtonClickedWave.enableWave) {
        //                RenderTriggeredWave(_settings.unityButtonClickedWave, "Button Click (uGUI) Event", TriggeredSpawner.EventType.ButtonClicked_uGUI);
        //            }

        //            if (_hasRect) {
        //                if (_settings.unityPointerDownWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityPointerDownWave, "Pointer Down (uGUI) Event", TriggeredSpawner.EventType.PointerDown_uGUI);
        //                }
        //                if (_settings.unityPointerUpWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityPointerUpWave, "Pointer Up (uGUI) Event", TriggeredSpawner.EventType.PointerUp_uGUI);
        //                }
        //                if (_settings.unityPointerEnterWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityPointerEnterWave, "Pointer Enter (uGUI) Event", TriggeredSpawner.EventType.PointerEnter_uGUI);
        //                }
        //                if (_settings.unityPointerExitWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityPointerExitWave, "Pointer Exit (uGUI) Event", TriggeredSpawner.EventType.PointerExit_uGUI);
        //                }
        //                if (_settings.unityDragWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityDragWave, "Drag (uGUI) Event", TriggeredSpawner.EventType.Drag_uGUI);
        //                }
        //                if (_settings.unityDropWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityDropWave, "Drop (uGUI) Event", TriggeredSpawner.EventType.Drop_uGUI);
        //                }
        //                if (_settings.unityScrollWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityScrollWave, "Scroll (uGUI) Event", TriggeredSpawner.EventType.Scroll_uGUI);
        //                }
        //                if (_settings.unityUpdateSelectedWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityUpdateSelectedWave, "Update Selected (uGUI) Event", TriggeredSpawner.EventType.UpdateSelected_uGUI);
        //                }
        //                if (_settings.unitySelectWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unitySelectWave, "Select (uGUI) Event", TriggeredSpawner.EventType.Select_uGUI);
        //                }
        //                if (_settings.unityDeselectWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityDeselectWave, "Deselect (uGUI) Event", TriggeredSpawner.EventType.Deselect_uGUI);
        //                }
        //                if (_settings.unityMoveWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityMoveWave, "Move (uGUI) Event", TriggeredSpawner.EventType.Move_uGUI);
        //                }
        //                if (_settings.unityInitializePotentialDragWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityInitializePotentialDragWave, "Init. Potential Drag (uGUI) Event", TriggeredSpawner.EventType.InitializePotentialDrag_uGUI);
        //                }
        //                if (_settings.unityBeginDragWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityBeginDragWave, "Begin Drag (uGUI) Event", TriggeredSpawner.EventType.BeginDrag_uGUI);
        //                }
        //                if (_settings.unityEndDragWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityEndDragWave, "End Drag (uGUI) Event", TriggeredSpawner.EventType.EndDrag_uGUI);
        //                }
        //                if (_settings.unitySubmitWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unitySubmitWave, "Submit (uGUI) Event", TriggeredSpawner.EventType.Submit_uGUI);
        //                }
        //                if (_settings.unityCancelWave.enableWave) {
        //                    RenderTriggeredWave(_settings.unityCancelWave, "Cancel (uGUI) Event", TriggeredSpawner.EventType.Cancel_uGUI);
        //                }
        //            }
        //        }
        //#endif

        //        if (_settings.collisionWave.enableWave) {
        //            RenderTriggeredWave(_settings.collisionWave, "Collision Enter Event", TriggeredSpawner.EventType.OnCollision);
        //        }
        //        if (_settings.triggerEnterWave.enableWave) {
        //            RenderTriggeredWave(_settings.triggerEnterWave, "Trigger Enter Event", TriggeredSpawner.EventType.OnTriggerEnter);
        //        }
        //        if (_settings.triggerExitWave.enableWave) {
        //            RenderTriggeredWave(_settings.triggerExitWave, "Trigger Exit Event", TriggeredSpawner.EventType.OnTriggerExit);
        //        }

        //        // Unity 4.3 Events
        //        if (_settings.collision2dWave.enableWave) {
        //            RenderTriggeredWave(_settings.collision2dWave, "2D Collision Enter Event", TriggeredSpawner.EventType.OnCollision2D);
        //        }

        //        if (_settings.triggerEnter2dWave.enableWave) {
        //            RenderTriggeredWave(_settings.triggerEnter2dWave, "2D Trigger Enter Event", TriggeredSpawner.EventType.OnTriggerEnter2D);
        //        }

        //        if (_settings.triggerExit2dWave.enableWave) {
        //            RenderTriggeredWave(_settings.triggerExit2dWave, "2D Trigger Exit Event", TriggeredSpawner.EventType.OnTriggerExit2D);
        //        }

        //        // code triggered event
        //        if (_settings.codeTriggeredWave1.enableWave) {
        //            RenderTriggeredWave(_settings.codeTriggeredWave1, "Code-Triggered Event 1", TriggeredSpawner.EventType.CodeTriggered1);
        //        }
        //        if (_settings.codeTriggeredWave2.enableWave) {
        //            RenderTriggeredWave(_settings.codeTriggeredWave2, "Code-Triggered Event 2", TriggeredSpawner.EventType.CodeTriggered2);
        //        }

        //        // Pool Boss & Pool Manager events (same for both).
        //        if (_settings.spawnedWave.enableWave) {
        //            RenderTriggeredWave(_settings.spawnedWave, "Spawned Event", TriggeredSpawner.EventType.OnSpawned);
        //        }
        //        if (_settings.despawnedWave.enableWave) {
        //            RenderTriggeredWave(_settings.despawnedWave, "Despawned Event", TriggeredSpawner.EventType.OnDespawned);
        //        }

        //        // NGUI events
        //        if (_settings.clickWave.enableWave) {
        //            RenderTriggeredWave(_settings.clickWave, "NGUI OnClick Event", TriggeredSpawner.EventType.OnClick_NGUI);
        //        }

        //        for (var i = 0; i < _settings.userDefinedEventWaves.Count; i++) {
        //            var aWave = _settings.userDefinedEventWaves[i];
        //            RenderTriggeredWave(aWave, "Custom Event", TriggeredSpawner.EventType.CustomEvent, i);
        //        }

        //        if (!Application.isPlaying && !DTInspectorUtility.IsPrefabInProjectView(_settings)) {
        //            if (_waveToVisualize != null) {
        //				// turn off other waves!
        //                // ReSharper disable once ForCanBeConvertedToForeach
        //                for (var w = 0; w < _allWaves.Count; w++) {
        //                    if (_allWaves[w] != _waveToVisualize) {
        //                        _allWaves[w].visualizeWave = false;
        //                    }
        //                }
        //            }

        //            TriggeredWaveSpecifics wave = null;
        //            if (_changedWave != null) {
        //				wave = _changedWave;
        //            }

        //            var hasUnrenderedVisualWave = false;
        //            if (wave == null) {
        //				// ReSharper disable once ForCanBeConvertedToForeach
        //                for (var w = 0; w < _allWaves.Count; w++) {
        //                    var oneWave = _allWaves[w];
        //                    if (!oneWave.visualizeWave) {
        //                        continue;
        //                    }

        //                    if (_settings.transform.childCount != 0 || oneWave.NumberToSpwn.Value <= 0) {
        //                        continue;
        //                    }

        //                    hasUnrenderedVisualWave = true;
        //                    break;
        //                }
        //            }

        //            if (waveActivated || hasUnrenderedVisualWave) {
        //                // ReSharper disable once ForCanBeConvertedToForeach
        //                for (var w = 0; w < _allWaves.Count; w++) {
        //                    if (!_allWaves[w].visualizeWave) {
        //                        continue;
        //                    }

        //                    wave = _allWaves[w];
        //                    break;
        //                }
        //            }

        //            if (wave != null) {
        //                if (wave.visualizeWave) {
        //                    _settings.gameObject.DestroyChildrenImmediateWithMarker();
        //                    _settings.SpawnWaveVisual(wave);
        //                }
        //            }
        //        }

        if (GUI.changed) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    //    private void RenderTriggeredWave(TriggeredWaveSpecifics waveSetting, string toggleText, TriggeredSpawner.EventType eventType, int? itemIndex = null) {
    //        _allWaves.Add(waveSetting);

    //        var disabledText = string.Empty;

    //        if (eventType == TriggeredSpawner.EventType.CustomEvent && !string.IsNullOrEmpty(waveSetting.customEventName)) {
    //            toggleText += ": " + waveSetting.customEventName;
    //        }

    //        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never) {
    //            disabledText = " - DISABLED";
    //        }

    //        toggleText += disabledText;

    //        EditorGUI.indentLevel = 0;

    //        if (_settings.activeMode == LevelSettings.ActiveItemMode.Never) {
    //            DTInspectorUtility.StartGroupHeader(1);
    //            EditorGUILayout.LabelField(toggleText);
    //            DTInspectorUtility.EndGroupHeader();
    //            return;
    //        }

    //        if (eventType == TriggeredSpawner.EventType.CustomEvent) {
    //            var state = waveSetting.isExpanded;
    //            var text = toggleText;

    //            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
    //            if (!state) {
    //                GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
    //            } else {
    //                GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
    //            }

    //            GUILayout.BeginHorizontal();

    //            text = "<b><size=11>" + text + "</size></b>";

    //			if (state) {
    //                text = "\u25BC " + text;
    //            } else {
    //                text = "\u25BA " + text;
    //            }
    //            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
    //                state = !state;
    //            }

    //            GUILayout.Space(2f);

    //            if (state != waveSetting.isExpanded) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Custom Event active");
    //                waveSetting.isExpanded = state;
    //            }

    //			var buttonPressed = DTInspectorUtility.AddCustomEventIcons(false, false, false, true);

    //            switch (buttonPressed) {
    //                case DTInspectorUtility.FunctionButtons.Remove:
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Custom Event");
    //                    // ReSharper disable once PossibleInvalidOperationException
    //                    _settings.userDefinedEventWaves.RemoveAt(itemIndex.Value);
    //                    waveSetting.customEventActive = false;
    //                    break;
    //				case DTInspectorUtility.FunctionButtons.Fire:
    //					_settings.ReceiveEvent(waveSetting.customEventName, _settings.transform.position);
    //					break;
    //            }
    //            GUILayout.Space(4);

    //            EditorGUILayout.EndHorizontal();
    //        } else {

    //            var state = waveSetting.isExpanded;
    //            var text = toggleText;

    //            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
    //            if (!state) {
    //                GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
    //            } else {
    //                GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
    //            }

    //            GUILayout.BeginHorizontal();

    //            text = "<b><size=11>" + text + "</size></b>";
    //            if (state) {
    //                text = "\u25BC " + text;
    //            } else {
    //                text = "\u25BA " + text;
    //            }
    //            if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
    //                state = !state;
    //            }

    //            GUILayout.Space(2f);


    //            if (state != waveSetting.isExpanded) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "expand " + eventType + " event");
    //                waveSetting.isExpanded = state;
    //            }

    //			var buttonPressed = DTInspectorUtility.AddCustomEventIcons(false, false, false, true);

    //            switch (buttonPressed) {
    //                case DTInspectorUtility.FunctionButtons.Remove:
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete " + eventType);
    //                    waveSetting.enableWave = false;
    //                    break;
    //				case DTInspectorUtility.FunctionButtons.Fire:
    //					_settings.PropagateEventTrigger(eventType, null, true);
    //                	break;
    //            }
    //            GUILayout.Space(4);

    //            EditorGUILayout.EndHorizontal();
    //        }

    //        if (!waveSetting.isExpanded) {
    //            DTInspectorUtility.VerticalSpace(2);
    //            return;
    //        }

    //        DTInspectorUtility.BeginGroupedControls();

    //        if (eventType == TriggeredSpawner.EventType.CustomEvent) {
    //            if (_levelSettingsInScene) {
    //                var existingIndex = _customEventNames.IndexOf(waveSetting.customEventName);

    //                int? customEventIndex = null;

    //                EditorGUI.indentLevel = 0;

    //                var noEvent = false;
    //                var noMatch = false;

    //                if (existingIndex >= 1) {
    //                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
    //                    if (existingIndex == 1) {
    //                        noEvent = true;
    //                    }
    //                } else if (existingIndex == -1 && waveSetting.customEventName == LevelSettings.NoEventName) {
    //                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
    //                } else { // non-match
    //                    noMatch = true;
    //                    var newEventName = EditorGUILayout.TextField("Custom Event Name", waveSetting.customEventName);
    //                    if (newEventName != waveSetting.customEventName) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Event Name");
    //                        waveSetting.customEventName = newEventName;
    //                    }

    //                    var newIndex = EditorGUILayout.Popup("All Custom Events", -1, _customEventNames.ToArray());
    //                    if (newIndex >= 0) {
    //                        customEventIndex = newIndex;
    //                    }
    //                }

    //                if (noEvent) {
    //                    DTInspectorUtility.ShowRedErrorBox("No Custom Event specified. This section will do nothing.");
    //                } else if (noMatch) {
    //                    DTInspectorUtility.ShowRedErrorBox("Custom Event found no match. Type in or choose one.");
    //                }

    //                if (customEventIndex.HasValue) {
    //                    if (existingIndex != customEventIndex.Value) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Event");
    //                    }
    //                    switch (customEventIndex.Value) {
    //                        case -1:
    //                            waveSetting.customEventName = LevelSettings.NoEventName;
    //                            break;
    //                        default:
    //                            waveSetting.customEventName = _customEventNames[customEventIndex.Value];
    //                            break;
    //                    }
    //                }
    //            } else {
    //                var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", waveSetting.customEventName);
    //                if (newCustomEvent != waveSetting.customEventName) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Custom Event Name");
    //                    waveSetting.customEventName = newCustomEvent;
    //                }
    //            }
    //        }

    //        var poolNames = PoolNames;

    //        if (!waveSetting.enableWave) {
    //            return;
    //        }

    //        var newVis = EditorGUILayout.Toggle("Visualize Wave", waveSetting.visualizeWave);
    //        if (newVis != waveSetting.visualizeWave) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Visualize Wave");
    //            waveSetting.visualizeWave = newVis;
    //            if (!newVis) {
    //                _settings.gameObject.DestroyChildrenImmediateWithMarker();
    //            } else {
    //				_changedWave = waveSetting;
    //                _waveToVisualize = waveSetting;
    //            }
    //        }

    //        var newSource = (WaveSpecifics.SpawnOrigin)EditorGUILayout.EnumPopup("Prefab Type", waveSetting.spawnSource);
    //        if (newSource != waveSetting.spawnSource) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Type");
    //            waveSetting.spawnSource = newSource;
    // 			_changedWave = waveSetting;
    //        } 
    //        switch (waveSetting.spawnSource) {
    //            case WaveSpecifics.SpawnOrigin.Specific:
    //				PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _settings, ref waveSetting.prefabToSpawn, ref waveSetting.prefabToSpawnCategoryName, "Prefab To Spawn");

    //				if (_isDirty) {
    //					_changedWave = waveSetting;
    //				}

    //                if (waveSetting.prefabToSpawn == null) {
    //                    DTInspectorUtility.ShowRedErrorBox("Please specify a prefab to spawn.");
    //                }
    //                break;
    //            case WaveSpecifics.SpawnOrigin.PrefabPool:
    //                if (poolNames != null) {
    //                    var pool = LevelSettings.GetFirstMatchingPrefabPool(waveSetting.prefabPoolName);
    //                    var noPoolSelected = false;
    //                    var illegalPool = false;
    //                    var noPools = false;

    //                    if (pool == null) {
    //                        if (string.IsNullOrEmpty(waveSetting.prefabPoolName)) {
    //                            noPoolSelected = true;
    //                        } else {
    //                            illegalPool = true;
    //                        }
    //                        waveSetting.prefabPoolIndex = 0;
    //                    } else {
    //                        waveSetting.prefabPoolIndex = poolNames.IndexOf(waveSetting.prefabPoolName);
    //                    }

    //                    if (poolNames.Count > 1) {
    //                        EditorGUILayout.BeginHorizontal();
    //                        var newPool = EditorGUILayout.Popup("Prefab Pool", waveSetting.prefabPoolIndex, poolNames.ToArray());
    //                        if (newPool != waveSetting.prefabPoolIndex) {
    //                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Prefab Pool");
    //                            waveSetting.prefabPoolIndex = newPool;
    //                        }

    //                        if (waveSetting.prefabPoolIndex > 0) {
    //                            var matchingPool = LevelSettings.GetFirstMatchingPrefabPool(poolNames[waveSetting.prefabPoolIndex]);
    //                            if (matchingPool != null) {
    //                                waveSetting.prefabPoolName = matchingPool.name;
    //                            }
    //                        } else {
    //                            waveSetting.prefabPoolName = string.Empty;
    //                        }

    //                        if (newPool > 0) {
    //                            if (DTInspectorUtility.AddControlButtons("Prefab Pool") ==
    //                                DTInspectorUtility.FunctionButtons.Edit) {
    //                                Selection.activeGameObject = pool.gameObject;
    //                            }
    //                        }
    //                        EditorGUILayout.EndHorizontal();
    //                    } else {
    //                        noPools = true;
    //                    }

    //                    if (noPools) {
    //                        DTInspectorUtility.ShowRedErrorBox("You have no Prefab Pools. Create one first.");
    //                    } else if (noPoolSelected) {
    //                        DTInspectorUtility.ShowRedErrorBox("No Prefab Pool selected.");
    //                    } else if (illegalPool) {
    //                        DTInspectorUtility.ShowRedErrorBox("Prefab Pool '" + waveSetting.prefabPoolName + "' not found. Select one.");
    //                    }
    //                } else {
    //                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.NoPrefabPoolsContainerAlert);
    //                    DTInspectorUtility.ShowRedErrorBox(LevelSettings.RevertLevelSettingsAlert);
    //                }

    //                break;
    //        }

    //        var oldInt = waveSetting.NumberToSpwn.Value;
    //        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.NumberToSpwn, "Min To Spawn", _settings);
    //        if (oldInt != waveSetting.NumberToSpwn.Value) {
    //            _changedWave = waveSetting;
    //        }

    //        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.MaxToSpawn, "Max To Spawn", _settings);

    //        if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType)) {
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.TimeToSpawnEntireWave, "Time To Spawn All", _settings);
    //        }

    //        if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType)) {
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.WaveDelaySec, "Delay Wave (sec)", _settings);
    //        }

    //        bool newDisable;

    //        switch (eventType) {
    //            case TriggeredSpawner.EventType.Visible:
    //                newDisable = EditorGUILayout.Toggle("Stop On Invisible", waveSetting.stopWaveOnOppositeEvent);
    //                if (newDisable != waveSetting.stopWaveOnOppositeEvent) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Invisible");
    //                    waveSetting.stopWaveOnOppositeEvent = newDisable;
    //                }
    //                break;
    //            case TriggeredSpawner.EventType.OnTriggerEnter:
    //                newDisable = EditorGUILayout.Toggle("Stop When Trigger Exit", waveSetting.stopWaveOnOppositeEvent);
    //                if (newDisable != waveSetting.stopWaveOnOppositeEvent) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Exit");
    //                    waveSetting.stopWaveOnOppositeEvent = newDisable;
    //                }
    //                break;
    //            case TriggeredSpawner.EventType.OnTriggerExit:
    //                newDisable = EditorGUILayout.Toggle("Stop When Trigger Enter", waveSetting.stopWaveOnOppositeEvent);
    //                if (newDisable != waveSetting.stopWaveOnOppositeEvent) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Enter");
    //                    waveSetting.stopWaveOnOppositeEvent = newDisable;
    //                }
    //                break;
    //            case TriggeredSpawner.EventType.OnTriggerEnter2D:
    //                newDisable = EditorGUILayout.Toggle("Stop When Trigger Exit 2D", waveSetting.stopWaveOnOppositeEvent);
    //                if (newDisable != waveSetting.stopWaveOnOppositeEvent) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Exit 2D");
    //                    waveSetting.stopWaveOnOppositeEvent = newDisable;
    //                }
    //                break;
    //            case TriggeredSpawner.EventType.OnTriggerExit2D:
    //                newDisable = EditorGUILayout.Toggle("Stop When Trigger Enter 2D", waveSetting.stopWaveOnOppositeEvent);
    //                if (newDisable != waveSetting.stopWaveOnOppositeEvent) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Stop On Trigger Enter 2D");
    //                    waveSetting.stopWaveOnOppositeEvent = newDisable;
    //                }
    //                break;
    //        }

    //        newDisable = EditorGUILayout.Toggle("Disable Event After", waveSetting.disableAfterFirstTrigger);
    //        if (newDisable != waveSetting.disableAfterFirstTrigger) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Disable Event After");
    //            waveSetting.disableAfterFirstTrigger = newDisable;
    //        }

    //        if (TriggeredSpawner.eventsThatCanTriggerDespawn.Contains(eventType)) {
    //            var newWillDespawn = EditorGUILayout.Toggle("Despawn This", waveSetting.willDespawnOnEvent);
    //            if (newWillDespawn != waveSetting.willDespawnOnEvent) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Despawn This");
    //                waveSetting.willDespawnOnEvent = newWillDespawn;
    //            }
    //        }

    //        if (TriggeredSpawner.eventsWithTagLayerFilters.Contains(eventType)) {
    //            DTInspectorUtility.StartGroupHeader(1);
    //            var newLayer = EditorGUILayout.BeginToggleGroup(" Layer Filter", waveSetting.useLayerFilter);
    //            if (newLayer != waveSetting.useLayerFilter) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Layer Filter");
    //                waveSetting.useLayerFilter = newLayer;
    //            }
    //            DTInspectorUtility.EndGroupHeader();

    //            if (waveSetting.useLayerFilter) {
    //                for (var i = 0; i < waveSetting.matchingLayers.Count; i++) {
    //                    var newMatch = EditorGUILayout.LayerField("Layer Match " + (i + 1), waveSetting.matchingLayers[i]);
    //                    if (newMatch == waveSetting.matchingLayers[i]) {
    //                        continue;
    //                    }
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Layer Match");
    //                    waveSetting.matchingLayers[i] = newMatch;
    //                }
    //                EditorGUILayout.BeginHorizontal();
    //                GUILayout.Space(12);
    //                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
    //                if (GUILayout.Button(new GUIContent("Add", "Click to add a Layer Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Layer Match");
    //                    waveSetting.matchingLayers.Add(0);
    //                }
    //                GUILayout.Space(10);
    //                if (waveSetting.matchingLayers.Count > 1) {
    //                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Layer Match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Layer Match");
    //                        waveSetting.matchingLayers.RemoveAt(waveSetting.matchingLayers.Count - 1);
    //                    }
    //                }
    //                GUI.contentColor = Color.white;
    //                EditorGUILayout.EndHorizontal();
    //            }
    //            EditorGUILayout.EndToggleGroup();
    //            DTInspectorUtility.AddSpaceForNonU5();

    //            DTInspectorUtility.StartGroupHeader(1);
    //            var newTag = EditorGUILayout.BeginToggleGroup(" Tag Filter", waveSetting.useTagFilter);
    //            if (newTag != waveSetting.useTagFilter) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Tag Filter");
    //                waveSetting.useTagFilter = newTag;
    //            }
    //            DTInspectorUtility.EndGroupHeader();
    //            if (waveSetting.useTagFilter) {
    //                for (var i = 0; i < waveSetting.matchingTags.Count; i++) {
    //                    var newMatch = EditorGUILayout.TagField("Tag Match " + (i + 1), waveSetting.matchingTags[i]);
    //                    if (newMatch == waveSetting.matchingTags[i]) {
    //                        continue;
    //                    }
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Tag Match");
    //                    waveSetting.matchingTags[i] = newMatch;
    //                }
    //                EditorGUILayout.BeginHorizontal();
    //                GUILayout.Space(12);
    //                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
    //                if (GUILayout.Button(new GUIContent("Add", "Click to add a Tag Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Tag Match");
    //                    waveSetting.matchingTags.Add("Untagged");
    //                }
    //                GUILayout.Space(10);
    //                if (waveSetting.matchingTags.Count > 1) {
    //                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Tag Match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Tag Match");
    //                        waveSetting.matchingTags.RemoveAt(waveSetting.matchingLayers.Count - 1);
    //                    }
    //                }
    //                GUI.contentColor = Color.white;
    //                EditorGUILayout.EndHorizontal();
    //            }
    //            EditorGUILayout.EndToggleGroup();
    //        }

    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        EditorGUI.indentLevel = 1;
    //        var newEx = DTInspectorUtility.Foldout(waveSetting.positionExpanded, " Position Settings");
    //        if (newEx != waveSetting.positionExpanded) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Position Settings");
    //            waveSetting.positionExpanded = newEx;
    //        }
    //        EditorGUILayout.EndVertical();
    //        EditorGUI.indentLevel = 0;

    //        // ReSharper disable once TooWideLocalVariableScope
    //        float oldFloat;

    //        if (waveSetting.positionExpanded) {
    //            var newX = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("X Position Mode", waveSetting.positionXmode);
    //            if (newX != waveSetting.positionXmode) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change X Position Mode");
    //                waveSetting.positionXmode = newX;
    //                _changedWave = waveSetting;
    //            }

    //            Transform otherObj;

    //            switch (waveSetting.positionXmode) {
    //                case WaveSpecifics.PositionMode.CustomPosition:
    //                    oldFloat = waveSetting.customPosX.Value;
    //                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosX, "X Position", _settings);
    //                    if (oldFloat != waveSetting.customPosX.Value) {
    //                        _changedWave = waveSetting;
    //                    }
    //                    break;
    //                case WaveSpecifics.PositionMode.OtherObjectPosition:
    //                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectX, typeof(Transform), true);
    //                    if (waveSetting.otherObjectX != otherObj) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
    //                        waveSetting.otherObjectX = otherObj;
    //                    }
    //                    if (waveSetting.otherObjectX == null) {
    //                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
    //                    }
    //                    DTInspectorUtility.VerticalSpace(3);
    //                    break;
    //            }

    //            var newY = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Y Position Mode", waveSetting.positionYmode);
    //            if (newY != waveSetting.positionYmode) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Y Position Mode");
    //                waveSetting.positionYmode = newY;
    //                _changedWave = waveSetting;
    //            }

    //            switch (waveSetting.positionYmode) {
    //                case WaveSpecifics.PositionMode.CustomPosition:
    //                    oldFloat = waveSetting.customPosY.Value;
    //                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosY, "Y Position", _settings);
    //                    if (oldFloat != waveSetting.customPosY.Value) {
    //                        _changedWave = waveSetting;
    //                    }
    //                    break;
    //                case WaveSpecifics.PositionMode.OtherObjectPosition:
    //                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectY, typeof(Transform), true);
    //                    if (waveSetting.otherObjectY != otherObj) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
    //                        waveSetting.otherObjectY = otherObj;
    //                    }
    //                    if (waveSetting.otherObjectY == null) {
    //                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
    //                    }
    //                    DTInspectorUtility.VerticalSpace(3);
    //                    break;
    //            }

    //            var newZ = (WaveSpecifics.PositionMode)EditorGUILayout.EnumPopup("Z Position Mode", waveSetting.positionZmode);
    //            if (newZ != waveSetting.positionZmode) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Z Position Mode");
    //                waveSetting.positionZmode = newZ;
    //                _changedWave = waveSetting;
    //            }

    //            switch (waveSetting.positionZmode) {
    //                case WaveSpecifics.PositionMode.CustomPosition:
    //                    oldFloat = waveSetting.customPosZ.Value;
    //                    KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.customPosZ, "Z Position", _settings);
    //                    if (oldFloat != waveSetting.customPosZ.Value) {
    //                        _changedWave = waveSetting;
    //                    }
    //                    break;
    //                case WaveSpecifics.PositionMode.OtherObjectPosition:
    //                    otherObj = (Transform)EditorGUILayout.ObjectField("Other Object", waveSetting.otherObjectZ, typeof(Transform), true);
    //                    if (waveSetting.otherObjectZ != otherObj) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Other Object");
    //                        waveSetting.otherObjectZ = otherObj;
    //                    }
    //                    if (waveSetting.otherObjectZ == null) {
    //                        DTInspectorUtility.ShowRedErrorBox("You have not specified a Transform. The spawner's position will be used instead.");
    //                    }
    //                    break;
    //            }

    //            if (waveSetting.waveOffsetList.Count == 0) {
    //                waveSetting.waveOffsetList.Add(new Vector3());
    //                _isDirty = true;
    //            }

    //            DTInspectorUtility.StartGroupHeader();
    //            EditorGUILayout.BeginHorizontal();
    //            EditorGUILayout.LabelField("Wave Offsets");

    //            if (!Application.isPlaying) {
    //                GUI.contentColor = DTInspectorUtility.AddButtonColor;
    //                if (GUILayout.Button(new GUIContent("Add", "Add a new Wave Offset"),
    //                    EditorStyles.toolbarButton, GUILayout.MaxWidth(50))) {
    //                    waveSetting.waveOffsetList.Add(new Vector3());
    //                    _isDirty = true;
    //                    _changedWave = waveSetting;
    //                }
    //                GUI.contentColor = Color.white;
    //            }

    //            EditorGUILayout.EndHorizontal();
    //            EditorGUILayout.EndVertical();

    //            if (!Application.isPlaying) {
    //                var newMode = (WaveSpecifics.WaveOffsetChoiceMode)EditorGUILayout.EnumPopup("Offset Selection", waveSetting.offsetChoiceMode);
    //                if (newMode != waveSetting.offsetChoiceMode) {
    //                    _isDirty = true;
    //                    waveSetting.offsetChoiceMode = newMode;
    //                }
    //            }

    //            int? itemToDelete = null;

    //            // ReSharper disable once ForCanBeConvertedToForeach
    //            for (var i = 0; i < waveSetting.waveOffsetList.Count; i++) {
    //                var anOffset = waveSetting.waveOffsetList[i];
    //                EditorGUILayout.BeginHorizontal();

    //                var newOffset = EditorGUILayout.Vector3Field("Wave Offset #" + (i + 1), anOffset);

    //                var btn = DTInspectorUtility.FunctionButtons.None;

    //                if (!Application.isPlaying) {
    //                    btn = DTInspectorUtility.AddCustomEventIcons(false, false, false, false, "Wave Offset");
    //                }

    //                EditorGUILayout.EndHorizontal();

    //                if (btn == DTInspectorUtility.FunctionButtons.Remove) {
    //                    itemToDelete = i;
    //                }

    //                if (newOffset == anOffset) {
    //                    continue;
    //                }

    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Wave Offset");
    //                waveSetting.waveOffsetList[i] = newOffset;
    //                _changedWave = waveSetting;
    //            }
    //            EditorGUILayout.EndVertical();

    //            if (itemToDelete.HasValue) {
    //                waveSetting.waveOffsetList.RemoveAt(itemToDelete.Value);
    //                _isDirty = true;
    //                _changedWave = waveSetting;
    //            }
    //        }

    //        EditorGUILayout.EndVertical();
    //        //DTInspectorUtility.ResetColors();

    //        if (waveSetting.isCustomEvent) {
    //            var newLookAt = (WaveSpecifics.SpawnerRotationMode)EditorGUILayout.EnumPopup("Spawner Rotation Mode", waveSetting.curSpawnerRotMode);
    //            if (newLookAt != waveSetting.curSpawnerRotMode) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawner Rotation Mode");
    //                waveSetting.curSpawnerRotMode = newLookAt;
    //            }
    //        }

    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        var newRotation = (WaveSpecifics.RotationMode)EditorGUILayout.EnumPopup("Spawn Rotation Mode", waveSetting.curRotationMode);
    //        if (newRotation != waveSetting.curRotationMode) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Rotation Mode");
    //            waveSetting.curRotationMode = newRotation;
    //            _changedWave = waveSetting;
    //        }
    //        EditorGUILayout.EndVertical();

    //        if (waveSetting.curRotationMode == WaveSpecifics.RotationMode.LookAtCustomEventOrigin) {
    //            if (!waveSetting.isCustomEvent) {
    //                DTInspectorUtility.ShowRedErrorBox("Look At Custom Event Origin rotation mode is only valid for Custom Events.");
    //            } else {
    //                EditorGUI.indentLevel = 0;

    //                var ignoreX = EditorGUILayout.Toggle("Ignore Origin X", waveSetting.eventOriginIgnoreX);
    //                if (ignoreX != waveSetting.eventOriginIgnoreX) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin X");
    //                    waveSetting.eventOriginIgnoreX = ignoreX;
    //                }

    //                var ignoreY = EditorGUILayout.Toggle("Ignore Origin Y", waveSetting.eventOriginIgnoreY);
    //                if (ignoreY != waveSetting.eventOriginIgnoreY) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin Y");
    //                    waveSetting.eventOriginIgnoreY = ignoreY;
    //                }

    //                var ignoreZ = EditorGUILayout.Toggle("Ignore Origin Z", waveSetting.eventOriginIgnoreZ);
    //                if (ignoreZ != waveSetting.eventOriginIgnoreZ) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Ignore Origin Z");
    //                    waveSetting.eventOriginIgnoreZ = ignoreZ;
    //                }
    //            }
    //        }

    //        EditorGUI.indentLevel = 0;
    //        if (waveSetting.curRotationMode == WaveSpecifics.RotationMode.CustomRotation) {
    //            var newCust = EditorGUILayout.Vector3Field("Custom Rotation Euler", waveSetting.customRotation);
    //            if (newCust != waveSetting.customRotation) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Custom Rotation Euler");
    //                waveSetting.customRotation = newCust;
    //                _changedWave = waveSetting;
    //            }
    //        }
    //        EditorGUILayout.EndVertical();
    //        DTInspectorUtility.ResetColors();

    //        if (!waveSetting.disableAfterFirstTrigger) {
    //            if (!TriggeredSpawner.eventsWithInflexibleWaveLength.Contains(eventType)) {
    //                DTInspectorUtility.AddSpaceForNonU5();
    //                DTInspectorUtility.StartGroupHeader(1);
    //                var newRetrigger = (TriggeredSpawner.RetriggerLimitMode)EditorGUILayout.EnumPopup("Retrigger Limit Mode", waveSetting.retriggerLimitMode);
    //                if (newRetrigger != waveSetting.retriggerLimitMode) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Retrigger Limit Mode");
    //                    waveSetting.retriggerLimitMode = newRetrigger;
    //                }
    //                EditorGUILayout.EndVertical();

    //                switch (waveSetting.retriggerLimitMode) {
    //                    case TriggeredSpawner.RetriggerLimitMode.FrameBased:
    //                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.limitPerXFrm, "Min Frames Between", _settings);
    //                        break;
    //                    case TriggeredSpawner.RetriggerLimitMode.TimeBased:
    //                        KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.limitPerXSec, "Min Seconds Between", _settings);
    //                        break;
    //                }
    //                EditorGUILayout.EndVertical();
    //            }
    //        }

    //        // repeat wave spawn variable modifiers
    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        var newBonusesEnabled = EditorGUILayout.BeginToggleGroup(" Wave Spawn Bonus & Events", waveSetting.waveSpawnBonusesEnabled);
    //        if (newBonusesEnabled != waveSetting.waveSpawnBonusesEnabled) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Wave Spawn Bonus & Events");
    //            waveSetting.waveSpawnBonusesEnabled = newBonusesEnabled;
    //        }
    //        DTInspectorUtility.EndGroupHeader();

    //        if (waveSetting.waveSpawnBonusesEnabled) {
    //            EditorGUI.indentLevel = 0;

    //            var missingBonusStatNames = new List<string>();
    //            missingBonusStatNames.AddRange(_allStats);
    //            missingBonusStatNames.RemoveAll(delegate(string obj) {
    //                return waveSetting.waveSpawnVariableModifiers.HasKey(obj);
    //            });

    //            var newCheck = EditorGUILayout.Toggle("Use On First Spawn", waveSetting.useWaveSpawnBonusForBeginning);
    //            if (newCheck != waveSetting.useWaveSpawnBonusForBeginning) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Use On First Spawn");
    //                waveSetting.useWaveSpawnBonusForBeginning = newCheck;
    //            }

    //            var newBonusStat = EditorGUILayout.Popup("Add Variable Modifer", 0, missingBonusStatNames.ToArray());
    //            if (newBonusStat != 0) {
    //                AddBonusStatModifier(missingBonusStatNames[newBonusStat], waveSetting);
    //            }

    //            if (waveSetting.waveSpawnVariableModifiers.statMods.Count == 0) {
    //                if (waveSetting.waveSpawnBonusesEnabled) {
    //                    DTInspectorUtility.ShowRedErrorBox("You currently are using no modifiers for this wave.");
    //                }
    //            } else {
    //                EditorGUILayout.Separator();

    //                int? indexToDelete = null;

    //                for (var i = 0; i < waveSetting.waveSpawnVariableModifiers.statMods.Count; i++) {
    //                    var modifier = waveSetting.waveSpawnVariableModifiers.statMods[i];

    //                    var buttonPressed = DTInspectorUtility.FunctionButtons.None;
    //                    switch (modifier._varTypeToUse) {
    //                        case WorldVariableTracker.VariableType._integer:
    //                            buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
    //                            break;
    //                        case WorldVariableTracker.VariableType._float:
    //                            buttonPressed = KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, modifier._modValueFloatAmt, modifier._statName, _settings, true, true);
    //                            break;
    //                        default:
    //                            Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
    //                            break;
    //                    }

    //                    KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);

    //                    if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
    //                        indexToDelete = i;
    //                    }
    //                }

    //                if (indexToDelete.HasValue) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Variable Modifier");
    //                    waveSetting.waveSpawnVariableModifiers.DeleteByIndex(indexToDelete.Value);
    //                }

    //                EditorGUILayout.Separator();
    //            }

    //            DTInspectorUtility.StartGroupHeader(0, false);
    //            var newExp = EditorGUILayout.Toggle("Spawn Cust. Events", waveSetting.waveSpawnFireEvents);
    //            if (newExp != waveSetting.waveSpawnFireEvents) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Spawn Cust. Events");
    //                waveSetting.waveSpawnFireEvents = newExp;
    //            }

    //            if (waveSetting.waveSpawnFireEvents) {
    //                DTInspectorUtility.ShowColorWarningBox("When wave starts, fire the Custom Events below");

    //                EditorGUILayout.BeginHorizontal();
    //                GUI.contentColor = DTInspectorUtility.AddButtonColor;
    //                GUILayout.Space(10);
    //                if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Spawn Custom Event");
    //                    waveSetting.waveSpawnCustomEvents.Add(new CGKCustomEventToFire());
    //                }
    //                GUILayout.Space(10);
    //                if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove last Wave Spawn Custom Event");
    //                    waveSetting.waveSpawnCustomEvents.RemoveAt(waveSetting.waveSpawnCustomEvents.Count - 1);
    //                }
    //                GUI.contentColor = Color.white;

    //                EditorGUILayout.EndHorizontal();

    //                if (waveSetting.waveSpawnCustomEvents.Count == 0) {
    //                    DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
    //                }

    //                DTInspectorUtility.VerticalSpace(2);

    //                // ReSharper disable once ForCanBeConvertedToForeach
    //                for (var i = 0; i < waveSetting.waveSpawnCustomEvents.Count; i++) {
    //                    var anEvent = waveSetting.waveSpawnCustomEvents[i].CustomEventName;

    //                    anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
    //                        _settings, "Custom Event");

    //                    if (anEvent == waveSetting.waveSpawnCustomEvents[i].CustomEventName) {
    //                        continue;
    //                    }

    //                    waveSetting.waveSpawnCustomEvents[i].CustomEventName = anEvent;
    //                }
    //            }
    //            EditorGUILayout.EndVertical();

    //        }
    //        EditorGUILayout.EndToggleGroup();


    //        if (TriggeredSpawner.eventsThatCanRepeatWave.Contains(eventType)) {
    //            DTInspectorUtility.AddSpaceForNonU5();
    //            DTInspectorUtility.StartGroupHeader(1);
    //            var newRepeat = EditorGUILayout.BeginToggleGroup(" Repeat Wave", waveSetting.enableRepeatWave);
    //            if (newRepeat != waveSetting.enableRepeatWave) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Wave");
    //                waveSetting.enableRepeatWave = newRepeat;
    //            }
    //            DTInspectorUtility.EndGroupHeader();
    //            if (waveSetting.enableRepeatWave) {
    //                var newRepeatMode = (WaveSpecifics.RepeatWaveMode)EditorGUILayout.EnumPopup("Repeat Mode", waveSetting.curWaveRepeatMode);
    //                if (newRepeatMode != waveSetting.curWaveRepeatMode) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Repeat Mode");
    //                    waveSetting.curWaveRepeatMode = newRepeatMode;
    //                }

    //                switch (waveSetting.curWaveRepeatMode) {
    //                    case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
    //                        KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.maxRepeat, "Wave Repetitions", _settings);
    //                        break;
    //                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
    //                    case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
    //                        var missingStatNames = new List<string>();
    //                        missingStatNames.AddRange(_allStats);
    //                        missingStatNames.RemoveAll(delegate(string obj) {
    //                            return waveSetting.repeatPassCriteria.HasKey(obj);
    //                        });

    //                        var newStat = EditorGUILayout.Popup("Add Variable Limit", 0, missingStatNames.ToArray());
    //                        if (newStat != 0) {
    //                            AddStatModifier(missingStatNames[newStat], waveSetting);
    //                        }

    //                        if (waveSetting.repeatPassCriteria.statMods.Count == 0) {
    //                            DTInspectorUtility.ShowRedErrorBox("You have no Variable Limits. Wave will not repeat.");
    //                        } else {
    //                            EditorGUILayout.Separator();

    //                            int? indexToDelete = null;

    //                            for (var i = 0; i < waveSetting.repeatPassCriteria.statMods.Count; i++) {
    //                                var modifier = waveSetting.repeatPassCriteria.statMods[i];
    //                                var buttonPressed = KillerVariablesHelper.DisplayKillerInt(ref _isDirty, modifier._modValueIntAmt, modifier._statName, _settings, true, true);
    //                                if (buttonPressed == DTInspectorUtility.FunctionButtons.Remove) {
    //                                    indexToDelete = i;
    //                                }

    //                                KillerVariablesHelper.ShowErrorIfMissingVariable(modifier._statName);
    //                            }

    //                            DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
    //                            if (indexToDelete.HasValue) {
    //                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Modifier");
    //                                waveSetting.repeatPassCriteria.DeleteByIndex(indexToDelete.Value);
    //                            }

    //                            EditorGUILayout.Separator();
    //                        }

    //                        break;
    //                }

    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatWavePauseSec, "Pause Before Repeat", _settings);

    //                DTInspectorUtility.VerticalSpace(3);
    //                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemInc, "Spawn Increase", _settings);

    //                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemMinLmt, "Spawn Min Limit", _settings);
    //                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, waveSetting.repeatItemLmt, "Spawn Max Limit", _settings);

    //                var reset = EditorGUILayout.Toggle("Reset On Spawn Lmt Passed", waveSetting.resetOnItemLimitReached);
    //                if (reset != waveSetting.resetOnItemLimitReached) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Spawn Lmt Passed");
    //                    waveSetting.resetOnItemLimitReached = reset;
    //                }

    //                DTInspectorUtility.VerticalSpace(3);
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeInc, "Time Increase", _settings);
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeMinLmt, "Time Min Limit", _settings);
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.repeatTimeLmt, "Time Max Limit", _settings);
    //                reset = EditorGUILayout.Toggle("Reset On Time Lmt Passed", waveSetting.resetOnTimeLimitReached);
    //                if (reset != waveSetting.resetOnTimeLimitReached) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Reset On Time Lmt Passed");
    //                    waveSetting.resetOnTimeLimitReached = reset;
    //                }

    //                if (waveSetting.waveSpawnBonusesEnabled) {
    //                    EditorGUI.indentLevel = 0;
    //                    var newUseRepeatBonus = EditorGUILayout.Toggle("Use Wave Spawn Bonus", waveSetting.useWaveSpawnBonusForRepeats);
    //                    if (newUseRepeatBonus != waveSetting.useWaveSpawnBonusForRepeats) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Use Wave Spawn Bonus");
    //                        waveSetting.useWaveSpawnBonusForRepeats = newUseRepeatBonus;
    //                    }
    //                }


    //                DTInspectorUtility.AddSpaceForNonU5();

    //                DTInspectorUtility.StartGroupHeader(0, false);
    //                var newExp = EditorGUILayout.Toggle("Repeat Cust. Events", waveSetting.waveRepeatFireEvents);
    //                if (newExp != waveSetting.waveRepeatFireEvents) {
    //                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Repeat Cust. Events");
    //                    waveSetting.waveRepeatFireEvents = newExp;
    //                }

    //                if (waveSetting.waveRepeatFireEvents) {
    //                    DTInspectorUtility.ShowColorWarningBox("When wave repeats, fire the Custom Events below");

    //                    EditorGUILayout.BeginHorizontal();
    //                    GUI.contentColor = DTInspectorUtility.AddButtonColor;
    //                    GUILayout.Space(10);
    //                    if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Add Wave Repeat Custom Event");
    //                        waveSetting.waveRepeatCustomEvents.Add(new CGKCustomEventToFire());
    //                    }
    //                    GUILayout.Space(10);
    //                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
    //                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Remove last Wave Repeat Custom Event");
    //                        waveSetting.waveRepeatCustomEvents.RemoveAt(waveSetting.waveRepeatCustomEvents.Count - 1);
    //                    }
    //                    GUI.contentColor = Color.white;

    //                    EditorGUILayout.EndHorizontal();

    //                    if (waveSetting.waveRepeatCustomEvents.Count == 0) {
    //                        DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
    //                    }

    //                    DTInspectorUtility.VerticalSpace(2);

    //                    // ReSharper disable once ForCanBeConvertedToForeach
    //                    for (var i = 0; i < waveSetting.waveRepeatCustomEvents.Count; i++) {
    //                        var anEvent = waveSetting.waveRepeatCustomEvents[i].CustomEventName;

    //                        anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
    //                            _settings, "Custom Event");

    //                        if (anEvent == waveSetting.waveRepeatCustomEvents[i].CustomEventName) {
    //                            continue;
    //                        }

    //                        waveSetting.waveRepeatCustomEvents[i].CustomEventName = anEvent;
    //                    }
    //                }
    //                EditorGUILayout.EndVertical();
    //            }
    //            EditorGUILayout.EndToggleGroup();
    //        }

    //        // show randomizations
    //        const string variantTag = " Randomization";

    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        var newRand = EditorGUILayout.BeginToggleGroup(variantTag, waveSetting.enableRandomizations);
    //        if (newRand != waveSetting.enableRandomizations) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Randomization");
    //            waveSetting.enableRandomizations = newRand;
    //            _changedWave = waveSetting;
    //        }
    //        DTInspectorUtility.EndGroupHeader();
    //        if (waveSetting.enableRandomizations) {
    //            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
    //            EditorGUILayout.LabelField("Random Rotation");

    //            var newRandX = GUILayout.Toggle(waveSetting.randomXRotation, "X");
    //            if (newRandX != waveSetting.randomXRotation) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random X Rotation");
    //                waveSetting.randomXRotation = newRandX;
    //                _changedWave = waveSetting;
    //            }
    //            GUILayout.Space(10);
    //            var newRandY = GUILayout.Toggle(waveSetting.randomYRotation, "Y");
    //            if (newRandY != waveSetting.randomYRotation) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Y Rotation");
    //                waveSetting.randomYRotation = newRandY;
    //                _changedWave = waveSetting;
    //            }
    //            GUILayout.Space(10);
    //            var newRandZ = GUILayout.Toggle(waveSetting.randomZRotation, "Z");
    //            if (newRandZ != waveSetting.randomZRotation) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Random Z Rotation");
    //                waveSetting.randomZRotation = newRandZ;
    //                _changedWave = waveSetting;
    //            }
    //            EditorGUILayout.EndHorizontal();

    //            if (waveSetting.randomXRotation) {
    //                oldFloat = waveSetting.randomXRotMin.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMin, "Rand. X Rot. Min", _settings);
    //                if (oldFloat != waveSetting.randomXRotMin.Value) {
    //                    _changedWave = waveSetting;
    //                }

    //                oldFloat = waveSetting.randomXRotMax.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomXRotMax, "Rand. X Rot. Max", _settings);
    //                if (oldFloat != waveSetting.randomXRotMax.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }
    //            if (waveSetting.randomYRotation) {
    //                oldFloat = waveSetting.randomYRotMin.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMin, "Rand. Y Rot. Min", _settings);
    //                if (oldFloat != waveSetting.randomYRotMin.Value) {
    //                    _changedWave = waveSetting;
    //                }

    //                oldFloat = waveSetting.randomYRotMax.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomYRotMax, "Rand. Y Rot. Max", _settings);
    //                if (oldFloat != waveSetting.randomYRotMax.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }
    //            if (waveSetting.randomZRotation) {
    //                oldFloat = waveSetting.randomZRotMin.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMin, "Rand. Z Rot. Min", _settings);
    //                if (oldFloat != waveSetting.randomZRotMin.Value) {
    //                    _changedWave = waveSetting;
    //                }

    //                oldFloat = waveSetting.randomZRotMax.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomZRotMax, "Rand. Z Rot. Max", _settings);
    //                if (oldFloat != waveSetting.randomZRotMax.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }

    //            EditorGUILayout.Separator();

    //            oldFloat = waveSetting.randomDistX.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistX, "Rand. Distance X", _settings);
    //            if (oldFloat != waveSetting.randomDistX.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.randomDistY.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistY, "Rand. Distance Y", _settings);
    //            if (oldFloat != waveSetting.randomDistY.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.randomDistZ.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.randomDistZ, "Rand. Distance Z", _settings);
    //            if (oldFloat != waveSetting.randomDistZ.Value) {
    //                _changedWave = waveSetting;
    //            }
    //        }
    //        EditorGUILayout.EndToggleGroup();


    //        // show increments
    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        var incTag = " Incremental Settings";
    //        var newIncrements = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enableIncrements);
    //        if (newIncrements != waveSetting.enableIncrements) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Incremental Settings");
    //            waveSetting.enableIncrements = newIncrements;
    //            _changedWave = waveSetting;
    //        }
    //        DTInspectorUtility.EndGroupHeader();
    //        if (waveSetting.enableIncrements) {
    //            oldFloat = waveSetting.incrementPositionX.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionX, "Distance X", _settings);
    //            if (oldFloat != waveSetting.incrementPositionX.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.incrementPositionY.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionY, "Distance Y", _settings);
    //            if (oldFloat != waveSetting.incrementPositionY.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.incrementPositionZ.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementPositionZ, "Distance Z", _settings);
    //            if (oldFloat != waveSetting.incrementPositionZ.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            EditorGUILayout.Separator();

    //            if (waveSetting.enableRandomizations && waveSetting.randomXRotation) {
    //                DTInspectorUtility.ShowColorWarningBox("Rotation X - cannot be used with Random Rotation X.");
    //            } else {
    //                oldFloat = waveSetting.incrementRotX.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotX, "Rotation X", _settings);
    //                if (oldFloat != waveSetting.incrementRotX.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }

    //            if (waveSetting.enableRandomizations && waveSetting.randomYRotation) {
    //                DTInspectorUtility.ShowColorWarningBox("Rotation Y - cannot be used with Random Rotation Y.");
    //            } else {
    //                oldFloat = waveSetting.incrementRotY.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotY, "Rotation Y", _settings);
    //                if (oldFloat != waveSetting.incrementRotY.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }

    //            if (waveSetting.enableRandomizations && waveSetting.randomZRotation) {
    //                DTInspectorUtility.ShowColorWarningBox("Rotation Z - cannot be used with Random Rotation Z.");
    //            } else {
    //                oldFloat = waveSetting.incrementRotZ.Value;
    //                KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.incrementRotZ, "Rotation Z", _settings);
    //                if (oldFloat != waveSetting.incrementRotZ.Value) {
    //                    _changedWave = waveSetting;
    //                }
    //            }

    //            var newIncKc = EditorGUILayout.Toggle("Keep Center", waveSetting.enableKeepCenter);
    //            if (newIncKc != waveSetting.enableKeepCenter) {
    //                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Keep Center");
    //                waveSetting.enableKeepCenter = newIncKc;
    //                _changedWave = waveSetting;
    //            }
    //        }
    //        EditorGUILayout.EndToggleGroup();


    //        // show increments
    //        incTag = " Post-spawn Nudge Settings";
    //        DTInspectorUtility.AddSpaceForNonU5();
    //        DTInspectorUtility.StartGroupHeader(1);
    //        var newPostEnabled = EditorGUILayout.BeginToggleGroup(incTag, waveSetting.enablePostSpawnNudge);
    //        if (newPostEnabled != waveSetting.enablePostSpawnNudge) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Post-spawn Nudge Settings");
    //            waveSetting.enablePostSpawnNudge = newPostEnabled;
    //            _changedWave = waveSetting;
    //        }
    //        DTInspectorUtility.EndGroupHeader();
    //        if (waveSetting.enablePostSpawnNudge) {
    //            oldFloat = waveSetting.postSpawnNudgeFwd.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeFwd, "Nudge Forward", _settings);
    //            if (oldFloat != waveSetting.postSpawnNudgeFwd.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.postSpawnNudgeRgt.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeRgt, "Nudge Right", _settings);
    //            if (oldFloat != waveSetting.postSpawnNudgeRgt.Value) {
    //                _changedWave = waveSetting;
    //            }

    //            oldFloat = waveSetting.postSpawnNudgeDwn.Value;
    //            KillerVariablesHelper.DisplayKillerFloat(ref _isDirty, waveSetting.postSpawnNudgeDwn, "Nudge Down", _settings);
    //            if (oldFloat != waveSetting.postSpawnNudgeDwn.Value) {
    //                _changedWave = waveSetting;
    //            }
    //        }
    //        EditorGUILayout.EndToggleGroup();

    //        DTInspectorUtility.EndGroupedControls();

    //        DTInspectorUtility.VerticalSpace(3);
    //    }

    //    private List<string> GetUnusedEventTypes() {
    //        var unusedEvents = new List<string> { "-None-" };
    //        if (!_settings.enableWave.enableWave) {
    //            unusedEvents.Add("Enabled");
    //        }
    //        if (!_settings.disableWave.enableWave) {
    //            unusedEvents.Add("Disabled");
    //        }
    //        if (!_settings.visibleWave.enableWave) {
    //            unusedEvents.Add("Visible");
    //        }
    //        if (!_settings.invisibleWave.enableWave) {
    //            unusedEvents.Add("Invisible");
    //        }

    //        if (_settings.unityUIMode == TriggeredSpawner.Unity_UIVersion.Legacy) {
    //            if (!_settings.mouseOverWave.enableWave) {
    //                unusedEvents.Add("Mouse Over (Legacy)");
    //            }
    //            if (!_settings.mouseClickWave.enableWave) {
    //                unusedEvents.Add("Mouse Click (Legacy)");
    //            }
    //        }

	//#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
    //        if (_settings.unityUIMode == TriggeredSpawner.Unity_UIVersion.uGUI) {
    //            if (_hasSlider && !_settings.unitySliderChangedWave.enableWave) {
    //                unusedEvents.Add("Slider Changed (uGUI)");
    //            }
    //            if (_hasButton && !_settings.unityButtonClickedWave.enableWave) {
    //                unusedEvents.Add("Button Click (uGUI)");
    //            }
    //            if (_hasRect) {
    //                if (!_settings.unityPointerDownWave.enableWave) {
    //                    unusedEvents.Add("Pointer Down (uGUI)");
    //                }
    //                if (!_settings.unityPointerUpWave.enableWave) {
    //                    unusedEvents.Add("Pointer Up (uGUI)");
    //                }
    //                if (!_settings.unityPointerEnterWave.enableWave) {
    //                    unusedEvents.Add("Pointer Enter (uGUI)");
    //                }
    //                if (!_settings.unityPointerExitWave.enableWave) {
    //                    unusedEvents.Add("Pointer Exit (uGUI)");
    //                }
    //                if (!_settings.unityDragWave.enableWave) {
    //                    unusedEvents.Add("Drag (uGUI)");
    //                }
    //                if (!_settings.unityDropWave.enableWave) {
    //                    unusedEvents.Add("Drop (uGUI)");
    //                }
    //                if (!_settings.unityScrollWave.enableWave) {
    //                    unusedEvents.Add("Scroll (uGUI)");
    //                }
    //                if (!_settings.unityUpdateSelectedWave.enableWave) {
    //                    unusedEvents.Add("Update Selected (uGUI)");
    //                }
    //                if (!_settings.unitySelectWave.enableWave) {
    //                    unusedEvents.Add("Select (uGUI)");
    //                }
    //                if (!_settings.unityDeselectWave.enableWave) {
    //                    unusedEvents.Add("Deselect (uGUI)");
    //                }
    //                if (!_settings.unityMoveWave.enableWave) {
    //                    unusedEvents.Add("Move (uGUI)");
    //                }
    //                if (!_settings.unityInitializePotentialDragWave.enableWave) {
    //                    unusedEvents.Add("Init. Potential Drag (uGUI)");
    //                }
    //                if (!_settings.unityBeginDragWave.enableWave) {
    //                    unusedEvents.Add("Begin Drag (uGUI)");
    //                }
    //                if (!_settings.unityEndDragWave.enableWave) {
    //                    unusedEvents.Add("End Drag (uGUI)");
    //                }
    //                if (!_settings.unitySubmitWave.enableWave) {
    //                    unusedEvents.Add("Submit (uGUI)");
    //                }
    //                if (!_settings.unityCancelWave.enableWave) {
    //                    unusedEvents.Add("Cancel (uGUI)");
    //                }
    //            }
    //        }
    //#endif

    //        if (!_settings.collisionWave.enableWave) {
    //            unusedEvents.Add("Collision Enter");
    //        }
    //        if (!_settings.triggerEnterWave.enableWave) {
    //            unusedEvents.Add("Trigger Enter");
    //        }
    //        if (!_settings.triggerExitWave.enableWave) {
    //            unusedEvents.Add("Trigger Exit");
    //        }
    //        if (!_settings.collision2dWave.enableWave) {
    //            unusedEvents.Add("2D Collision Enter");
    //        }
    //        if (!_settings.triggerEnter2dWave.enableWave) {
    //            unusedEvents.Add("2D Trigger Enter");
    //        }
    //        if (!_settings.triggerExit2dWave.enableWave) {
    //            unusedEvents.Add("2D Trigger Exit");
    //        }
    //        if (!_settings.codeTriggeredWave1.enableWave) {
    //            unusedEvents.Add("Code-Triggered 1");
    //        }
    //        if (!_settings.codeTriggeredWave2.enableWave) {
    //            unusedEvents.Add("Code-Triggered 2");
    //        }
    //        if (!_settings.spawnedWave.enableWave) {
    //            unusedEvents.Add("Spawned");
    //        }
    //        if (!_settings.despawnedWave.enableWave) {
    //            unusedEvents.Add("Despawned");
    //        }
    //        if (!_settings.clickWave.enableWave) {
    //            unusedEvents.Add("NGUI OnClick");
    //        }

    //        unusedEvents.Add("Custom Event");

    //        return unusedEvents;
    //    }

    //    private void ActivateEvent(int index, List<string> unusedEvents) {
    //        var item = unusedEvents[index];

    //        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "activate Event");

    //        switch (item) {
    //            case "Code-Triggered 1":
    //                _settings.codeTriggeredWave1.enableWave = true;
    //                break;
    //            case "Code-Triggered 2":
    //                _settings.codeTriggeredWave2.enableWave = true;
    //                break;
    //            case "Invisible":
    //                _settings.invisibleWave.enableWave = true;
    //                break;
    //            case "Mouse Click (Legacy)":
    //                _settings.mouseClickWave.enableWave = true;
    //                break;
    //            case "Mouse Over (Legacy)":
    //                _settings.mouseOverWave.enableWave = true;
    //                break;
    //            case "NGUI OnClick":
    //                _settings.clickWave.enableWave = true;
    //                break;
    //            case "Collision Enter":
    //                _settings.collisionWave.enableWave = true;
    //                break;
    //            case "Despawned":
    //                _settings.despawnedWave.enableWave = true;
    //                break;
    //            case "Disabled":
    //                _settings.disableWave.enableWave = true;
    //                break;
    //            case "Enabled":
    //                _settings.enableWave.enableWave = true;
    //                break;
    //            case "Spawned":
    //                _settings.spawnedWave.enableWave = true;
    //                break;
    //            case "Trigger Enter":
    //                _settings.triggerEnterWave.enableWave = true;
    //                break;
    //            case "Trigger Exit":
    //                _settings.triggerExitWave.enableWave = true;
    //                break;
    //            case "Visible":
    //                _settings.visibleWave.enableWave = true;
    //                break;
    //            case "2D Collision Enter":
    //                _settings.collision2dWave.enableWave = true;
    //                break;
    //            case "2D Trigger Enter":
    //                _settings.triggerEnter2dWave.enableWave = true;
    //                break;
    //            case "2D Trigger Exit":
    //                _settings.triggerExit2dWave.enableWave = true;
    //                break;
    //            case "Slider Changed (uGUI)":
    //                _settings.unitySliderChangedWave.enableWave = true;
    //                break;
    //            case "Button Click (uGUI)":
    //                _settings.unityButtonClickedWave.enableWave = true;
    //                break;
    //            case "Pointer Down (uGUI)":
    //                _settings.unityPointerDownWave.enableWave = true;
    //                break;
    //            case "Pointer Up (uGUI)":
    //                _settings.unityPointerUpWave.enableWave = true;
    //                break;
    //            case "Pointer Enter (uGUI)":
    //                _settings.unityPointerEnterWave.enableWave = true;
    //                break;
    //            case "Pointer Exit (uGUI)":
    //                _settings.unityPointerExitWave.enableWave = true;
    //                break;
    //            case "Drag (uGUI)":
    //                _settings.unityDragWave.enableWave = true;
    //                break;
    //            case "Drop (uGUI)":
    //                _settings.unityDropWave.enableWave = true;
    //                break;
    //            case "Scroll (uGUI)":
    //                _settings.unityScrollWave.enableWave = true;
    //                break;
    //            case "Update Selected (uGUI)":
    //                _settings.unityUpdateSelectedWave.enableWave = true;
    //                break;
    //            case "Select (uGUI)":
    //                _settings.unitySelectWave.enableWave = true;
    //                break;
    //            case "Deselect (uGUI)":
    //                _settings.unityDeselectWave.enableWave = true;
    //                break;
    //            case "Move (uGUI)":
    //                _settings.unityMoveWave.enableWave = true;
    //                break;
    //            case "Init. Potential Drag (uGUI)":
    //                _settings.unityInitializePotentialDragWave.enableWave = true;
    //                break;
    //            case "Begin Drag (uGUI)":
    //                _settings.unityBeginDragWave.enableWave = true;
    //                break;
    //            case "End Drag (uGUI)":
    //                _settings.unityEndDragWave.enableWave = true;
    //                break;
    //            case "Submit (uGUI)":
    //                _settings.unitySubmitWave.enableWave = true;
    //                break;
    //            case "Cancel (uGUI)":
    //                _settings.unityCancelWave.enableWave = true;
    //                break;
    //            case "Custom Event":
    //                CreateCustomEvent(false);
    //                break;
    //        }
    //    }

    //    private void AddStatModifier(string modifierName, TriggeredWaveSpecifics spec) {
    //        if (spec.repeatPassCriteria.HasKey(modifierName)) {
    //            DTInspectorUtility.ShowAlert("This wave already has a Variable Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
    //            return;
    //        }

    //        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Variable Limit");

    //        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

    //        spec.repeatPassCriteria.statMods.Add(new WorldVariableModifier(modifierName, myVar.varType));
    //    }

    //    private void AddActiveLimit(string modifierName, TriggeredSpawner spec) {
    //        if (spec.activeItemCriteria.HasKey(modifierName)) {
    //            DTInspectorUtility.ShowAlert("This item already has a Active Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
    //            return;
    //        }

    //        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Active Limit");

    //        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

    //        spec.activeItemCriteria.statMods.Add(new WorldVariableRange(modifierName, myVar.varType));
    //    }

    //    private void AddBonusStatModifier(string modifierName, TriggeredWaveSpecifics waveSpec) {
    //        if (waveSpec.waveSpawnVariableModifiers.HasKey(modifierName)) {
    //            DTInspectorUtility.ShowAlert("This Wave already has a modifier for World Variable: " + modifierName + ". Please modify that instead.");
    //            return;
    //        }

    //        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Wave Repeat Bonus modifier");

    //        var vType = WorldVariableTracker.GetWorldVariableScript(modifierName);

    //        waveSpec.waveSpawnVariableModifiers.statMods.Add(new WorldVariableModifier(modifierName, vType.varType));
    //    }

    //    private static List<string> PoolNames {
    //        get {
    //            return LevelSettings.GetSortedPrefabPoolNames();
    //        }
    //    }

    //    private void CreateCustomEvent(bool recordUndo) {
    //        var newWave = new TriggeredWaveSpecifics { customEventActive = true, isCustomEvent = true, enableWave = true };

    //        if (recordUndo) {
    //            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Custom Event");
    //        }

    //        _settings.userDefinedEventWaves.Add(newWave);
    //    }

    private void CopyToV2() {
        var v2 = _settings.GetComponent<TriggeredSpawnerV2>();

        if (v2 == null) {
            _settings.gameObject.AddComponent<TriggeredSpawnerV2>();
            v2 = _settings.GetComponent<TriggeredSpawnerV2>();
        }

        v2.DeleteAllWaves();
        
        CopyWaveIfAny(_settings.enableWave, ref v2.enableWaves);
        CopyWaveIfAny(_settings.disableWave, ref v2.disableWaves);
        CopyWaveIfAny(_settings.visibleWave, ref v2.visibleWaves);
        CopyWaveIfAny(_settings.invisibleWave, ref v2.invisibleWaves);
        CopyWaveIfAny(_settings.mouseOverWave, ref v2.mouseOverWaves);
        CopyWaveIfAny(_settings.mouseClickWave, ref v2.mouseClickWaves);
        CopyWaveIfAny(_settings.collisionWave, ref v2.collisionWaves);
        CopyWaveIfAny(_settings.triggerEnterWave, ref v2.triggerEnterWaves);
        CopyWaveIfAny(_settings.triggerExitWave, ref v2.triggerExitWaves);
        CopyWaveIfAny(_settings.spawnedWave, ref v2.spawnedWaves);
        CopyWaveIfAny(_settings.despawnedWave, ref v2.despawnedWaves);
        CopyWaveIfAny(_settings.codeTriggeredWave1, ref v2.codeTriggeredWaves1);
        CopyWaveIfAny(_settings.codeTriggeredWave2, ref v2.codeTriggeredWaves2);
        CopyWaveIfAny(_settings.clickWave, ref v2.clickWaves);
        CopyWaveIfAny(_settings.collision2dWave, ref v2.collision2dWaves);
        CopyWaveIfAny(_settings.triggerEnter2dWave, ref v2.triggerEnter2dWaves);
        CopyWaveIfAny(_settings.triggerExit2dWave, ref v2.triggerExit2dWaves);
        v2.userDefinedEventWaves.AddRange(_settings.userDefinedEventWaves);
        CopyWaveIfAny(_settings.unitySliderChangedWave, ref v2.unitySliderChangedWaves);
        CopyWaveIfAny(_settings.unityButtonClickedWave, ref v2.unityButtonClickedWaves);
        CopyWaveIfAny(_settings.unityPointerDownWave, ref v2.unityPointerDownWaves);
        CopyWaveIfAny(_settings.unityPointerUpWave, ref v2.unityPointerUpWaves);
        CopyWaveIfAny(_settings.unityPointerEnterWave, ref v2.unityPointerEnterWaves);
        CopyWaveIfAny(_settings.unityPointerExitWave, ref v2.unityPointerExitWaves);
        CopyWaveIfAny(_settings.unityDragWave, ref v2.unityDragWaves);
        CopyWaveIfAny(_settings.unityDropWave, ref v2.unityDropWaves);
        CopyWaveIfAny(_settings.unityScrollWave, ref v2.unityScrollWaves);
        CopyWaveIfAny(_settings.unityUpdateSelectedWave, ref v2.unityUpdateSelectedWaves);
        CopyWaveIfAny(_settings.unitySelectWave, ref v2.unitySelectWaves);
        CopyWaveIfAny(_settings.unityDeselectWave, ref v2.unityDeselectWaves);
        CopyWaveIfAny(_settings.unityMoveWave, ref v2.unityMoveWaves);
        CopyWaveIfAny(_settings.unityInitializePotentialDragWave, ref v2.unityInitializePotentialDragWaves);
        CopyWaveIfAny(_settings.unityBeginDragWave, ref v2.unityBeginDragWaves);
        CopyWaveIfAny(_settings.unityEndDragWave, ref v2.unityEndDragWaves);
        CopyWaveIfAny(_settings.unitySubmitWave, ref v2.unitySubmitWaves);
        CopyWaveIfAny(_settings.unityCancelWave, ref v2.unityCancelWaves);

        // copy non-wave fields too
        v2.activeMode = _settings.activeMode;
        v2.activeItemCriteria = _settings.activeItemCriteria;
        v2.gameOverBehavior = _settings.gameOverBehavior;
        v2.wavePauseBehavior = _settings.wavePauseBehavior;
        v2.unityUIMode = _settings.unityUIMode;
        v2.eventSourceType = _settings.eventSourceType;
        v2.spawnOutsidePool = _settings.spawnOutsidePool;
        v2.logMissingEvents = _settings.logMissingEvents;
        v2.listener = _settings.listener;
        v2.spawnLayerMode = _settings.spawnLayerMode;
        v2.spawnCustomLayer = _settings.spawnCustomLayer;
        v2.applyLayerRecursively = _settings.applyLayerRecursively;
        v2.spawnTagMode = _settings.spawnTagMode;
        v2.spawnCustomTag = _settings.spawnCustomTag;
    }

    private void CopyWaveIfAny(TriggeredWaveSpecifics firstSourceWave, ref List<TriggeredWaveSpecifics> targetWaveList) {
        //It will always be initialized even for unused wave data
        if (firstSourceWave == null) {
            return;
        }

        //Default value set when TriggeredWaveSpecifics class is initialized
        if (!firstSourceWave.enableWave) {
            return;
        }

        targetWaveList = new List<TriggeredWaveSpecifics>(1) {
            firstSourceWave
        };
    }

    //private TriggeredWaveSpecifics GetFirstWaveIfAny(List<TriggeredWaveSpecifics> waveList) {
    //    if (waveList.Count == 0) {
    //        return null;
    //    }

    //    return waveList[0];
    //}
}