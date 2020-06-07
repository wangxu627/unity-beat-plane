using System.Collections.Generic;
using System.Text;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WavePrefabPool))]
// ReSharper disable once CheckNamespace
public class WavePrefabPoolInspector : Editor {
    private Transform _poolTrans;
    private WavePrefabPool _settings;
    private bool _isDirty;

    private static void FindMatchingSpawners(Transform poolTrans, bool selectThem) {
        LevelSettings.Instance = null; // clear cached version

        var syncroSpawners = LevelSettings.GetAllSpawners;
        var triggeredSpawners = FindObjectsOfType(typeof(TriggeredSpawnerV2));
        var killables = FindObjectsOfType(typeof(Killable));

        var matchSpawners = new List<GameObject>();

        var sb = new StringBuilder();

        foreach (var spawner in syncroSpawners) {
            var spawnerScript = spawner.GetComponent<WaveSyncroPrefabSpawner>();
            if (!spawnerScript.IsUsingPrefabPool(poolTrans)) {
                continue;
            }

            matchSpawners.Add(spawner.gameObject);
            if (sb.Length > 0) {
                sb.Append(", ");
            }
            sb.Append("'" + spawnerScript.name + "'");
        }

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (TriggeredSpawnerV2 trig in triggeredSpawners) {
            if (!trig.IsUsingPrefabPool(poolTrans)) {
                continue;
            }

            matchSpawners.Add(trig.gameObject);
            if (sb.Length > 0) {
                sb.Append(", ");
            }
            sb.Append("'" + trig.name + "'");
        }

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (Killable kill in killables) {
            if (!kill.IsUsingPrefabPool(poolTrans)) {
                continue;
            }

            matchSpawners.Add(kill.gameObject);
            if (sb.Length > 0) {
                sb.Append(", ");
            }
            sb.Append("'" + kill.name + "'");
        }

        if (sb.Length == 0) {
            sb.Append("~None~");
        }

        Debug.Log(string.Format("--- Found {0} matching Objects(s) for Prefab Pool: ({1}) ---",
            matchSpawners.Count,
            sb));

        if (!selectThem) {
            return;
        }
        if (matchSpawners.Count > 0) {
            Selection.objects = matchSpawners.ToArray();
        } else {
            Debug.Log("No Objects in the Scene use this Prefab Pool.");
        }
    }

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        _settings = (WavePrefabPool)target;
        _poolTrans = _settings.transform;

        WorldVariableTracker.ClearInGamePlayerStats();

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/PrefabPools.htm");

        _isDirty = false;

        var allStats = KillerVariablesHelper.AllStatNames;

        var myParent = _settings.transform.parent;
        LevelSettings levelSettings = null;

        if (myParent != null) {
            var levelSettingObj = myParent.parent;
            if (levelSettingObj != null) {
                levelSettings = levelSettingObj.GetComponent<LevelSettings>();
            }
        }

        if (levelSettings == null) {
            return;
        }

        EditorGUI.indentLevel = 0;
        var newSeq = (WavePrefabPool.PoolDispersalMode)EditorGUILayout.EnumPopup("Spawn Sequence", _settings.dispersalMode);
        if (newSeq != _settings.dispersalMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Spawn Sequence");
            _settings.dispersalMode = newSeq;
        }
        if (_settings.dispersalMode == WavePrefabPool.PoolDispersalMode.Randomized) {
            var newExhaust = EditorGUILayout.Toggle("Exhaust before repeat", _settings.exhaustiveList);
            if (newExhaust != _settings.exhaustiveList) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Exhaust before repeat");
                _settings.exhaustiveList = newExhaust;
            }
        }

        var hadNoListener = _settings.listener == null;
        var newListener = (WavePrefabPoolListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(WavePrefabPoolListener), true);
        if (newListener != _settings.listener) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;
            if (hadNoListener && _settings.listener != null) {
                _settings.listener.sourcePrefabPoolName = _settings.transform.name;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scene Objects Using");

        GUI.contentColor = DTInspectorUtility.BrightButtonColor;
        if (GUILayout.Button("List", EditorStyles.toolbarButton, GUILayout.MinWidth(55))) {
            FindMatchingSpawners(_poolTrans, false);
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.MinWidth(55))) {
            FindMatchingSpawners(_poolTrans, true);
        }
        GUILayout.FlexibleSpace();

        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        DTInspectorUtility.VerticalSpace(4);

        if (!Application.isPlaying) {
            EditorGUILayout.BeginVertical();
            var anEvent = Event.current;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUI.color = DTInspectorUtility.DragAreaColor;
            var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
            GUI.Box(dragArea, "Drag prefabs here in bulk to add them to the Pool!");
            GUI.color = Color.white;

            switch (anEvent.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(anEvent.mousePosition)) {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (anEvent.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences) {
                            AddPoolItem(dragged);
                        }
                    }
                    Event.current.Use();
                    break;
            }
            GUILayout.Space(4);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            DTInspectorUtility.VerticalSpace(4);
        }

        EditorGUI.indentLevel = 0;

        var state = _settings.isExpanded;
        var text = string.Format("Prefab Pool Items ({0})", _settings.poolItems.Count);

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

        GUILayout.BeginHorizontal();

#if UNITY_3_5_7
        if (!state) {
            text += " (Click to expand)";
        }
#else
        text = "<b><size=11>" + text + "</size></b>";
#endif
        if (state) {
            text = "\u25BC " + text;
        } else {
            text = "\u25BA " + text;
        }
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
            state = !state;
        }

        GUILayout.Space(2f);



        if (state != _settings.isExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Prefab Pool Items");
            _settings.isExpanded = state;
        }
        // BUTTONS...
        EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(16));

        DTInspectorUtility.ResetColors();

        var alphaSort = false;

        //DTInspectorUtility.UseLightSkinButtonColor();
        // Add expand/collapse buttons if there are items in the list
        if (_settings.poolItems.Count > 0) {
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            alphaSort = GUILayout.Button("Alpha Sort", EditorStyles.toolbarButton, GUILayout.Height(16));

            const string collapseIcon = "Collapse";
            var content = new GUIContent(collapseIcon, "Click to collapse all");
            var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));

            const string expandIcon = "Expand";
            content = new GUIContent(expandIcon, "Click to expand all");
            var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));
            if (masterExpand) {
                ExpandCollapseAll(_settings, true);
            }
            if (masterCollapse) {
                ExpandCollapseAll(_settings, false);
            }
            GUI.contentColor = Color.white;
        } else {
            GUILayout.FlexibleSpace();
        }

        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));

        var topAdded = false;

        var addText = string.Format("Click to add Pool item{0}.", _settings.poolItems.Count > 0 ? " before the first" : "");

        GUI.contentColor = DTInspectorUtility.AddButtonColor;
        // Main Add button
        if (GUILayout.Button(new GUIContent("Add", addText), EditorStyles.toolbarButton, GUILayout.Height(16))) {
            topAdded = true;
        }
        GUI.contentColor = Color.white;

        GUILayout.Space(4);

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        int? itemToDelete = null;
        int? itemToInsert = null;
        int? itemToShiftUp = null;
        int? itemToShiftDown = null;
        int? itemToClone = null;

        if (_settings.isExpanded) {
            if (_settings.poolItems.Count > 0) {
                DTInspectorUtility.BeginGroupedControls();
            }
            for (var i = 0; i < _settings.poolItems.Count; i++) {
                var item = _settings.poolItems[i];

                DTInspectorUtility.StartGroupHeader(1);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel = 1;

                var sName = "";
                if (!item.isExpanded) {
                    if (item.prefabToSpawn == null) {
                        sName = " " + LevelSettings.EmptyValue;
                    } else {
                        sName = " (" + item.prefabToSpawn.name + ")";
                    }
                }

                var sDisabled = "";
                var itemDisabled = item.activeMode == LevelSettings.ActiveItemMode.Never;
                if (!item.isExpanded && itemDisabled) {
                    sDisabled = " - DISABLED";
                }

                var newItemExpanded = DTInspectorUtility.Foldout(item.isExpanded,
                  string.Format("Pool Item #{0}{1}{2}", (i + 1), sName, sDisabled));
                if (newItemExpanded != item.isExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand Pool Item");
                    item.isExpanded = newItemExpanded;
                }

                GUILayout.FlexibleSpace();

                if (Application.isPlaying) {
                    GUI.contentColor = DTInspectorUtility.BrightTextColor;
                    var itemCount = _settings.PoolInstancesOfIndex(i);
                    GUILayout.Label("Remaining: " + itemCount);
                    GUI.contentColor = Color.white;
                } 

                var poolItemButton = DTInspectorUtility.AddFoldOutListItemButtons(i, _settings.poolItems.Count, "Pool Item", false, null, false, true, true);

                switch (poolItemButton) {
                    case DTInspectorUtility.FunctionButtons.Remove:
                        itemToDelete = i;
                        _isDirty = true;
                        break;
                    case DTInspectorUtility.FunctionButtons.Add:
                        itemToInsert = i;
                        _isDirty = true;
                        break;
                    case DTInspectorUtility.FunctionButtons.ShiftUp:
                        itemToShiftUp = i;
                        _isDirty = true;
                        break;
                    case DTInspectorUtility.FunctionButtons.ShiftDown:
                        itemToShiftDown = i;
                        _isDirty = true;
                        break;
                    case DTInspectorUtility.FunctionButtons.Copy:
                        itemToClone = i;
                        _isDirty = true;
                        break;
                }

                EditorGUI.indentLevel = 0;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                if (itemDisabled) {
                    DTInspectorUtility.ShowColorWarningBox("This item is currently disabled and will never spawn.");
                }

                if (!item.isExpanded) {
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUI.indentLevel = 0;

                if (item.prefabToSpawn == null && !itemDisabled) {
                    DTInspectorUtility.ShowColorWarningBox("Nothing will spawn when this item is chosen from the pool.");
                }

                var newActive = (LevelSettings.ActiveItemMode)EditorGUILayout.EnumPopup("Active Mode", item.activeMode);
                if (newActive != item.activeMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Active Mode");
                    item.activeMode = newActive;
                }

                switch (item.activeMode) {
                    case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
                    case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                        var missingStatNames = new List<string>();
                        missingStatNames.AddRange(allStats);
                        missingStatNames.RemoveAll(delegate(string obj) {
                            return item.activeItemCriteria.HasKey(obj);
                        });

                        var newStat = EditorGUILayout.Popup("Add Active Limit", 0, missingStatNames.ToArray());
                        if (newStat != 0) {
                            AddActiveLimit(missingStatNames[newStat], item);
                        }

                        if (item.activeItemCriteria.statMods.Count == 0) {
                            DTInspectorUtility.ShowRedErrorBox("You have no Active Limits. Item will never be Active.");
                        } else {
                            EditorGUILayout.Separator();

                            int? indexToDelete = null;

                            for (var j = 0; j < item.activeItemCriteria.statMods.Count; j++) {
                                var modifier = item.activeItemCriteria.statMods[j];
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
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Limit Min");
                                            modifier._modValueIntMin = newMin;
                                        }

                                        GUILayout.Label("Max");
                                        var newMax = EditorGUILayout.IntField(modifier._modValueIntMax, GUILayout.MaxWidth(60));
                                        if (newMax != modifier._modValueIntMax) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Limit Max");
                                            modifier._modValueIntMax = newMax;
                                        }
                                        break;
                                    case WorldVariableTracker.VariableType._float:
                                        var newMinFloat = EditorGUILayout.FloatField(modifier._modValueFloatMin, GUILayout.MaxWidth(60));
                                        if (newMinFloat != modifier._modValueFloatMin) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Limit Min");
                                            modifier._modValueFloatMin = newMinFloat;
                                        }

                                        GUILayout.Label("Max");
                                        var newMaxFloat = EditorGUILayout.FloatField(modifier._modValueFloatMax, GUILayout.MaxWidth(60));
                                        if (newMaxFloat != modifier._modValueFloatMax) {
                                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Active Limit Max");
                                            modifier._modValueFloatMax = newMaxFloat;
                                        }
                                        break;
                                    default:
                                        Debug.LogError("Add code for varType: " + modifier._varTypeToUse.ToString());
                                        break;
                                }
                                var oldBG = GUI.backgroundColor;

                                GUI.backgroundColor = DTInspectorUtility.DeleteButtonColor;
                                if (GUILayout.Button(new GUIContent("Delete", "Remove this limit"), EditorStyles.miniButtonMid, GUILayout.MaxWidth(64))) {
                                    indexToDelete = j;
                                }
                                GUI.backgroundColor = oldBG;
                                GUILayout.Space(5);
                                EditorGUILayout.EndHorizontal();

                                var min = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMin : modifier._modValueFloatMin;
                                var max = modifier._varTypeToUse == WorldVariableTracker.VariableType._integer ? modifier._modValueIntMax : modifier._modValueFloatMax;

                                if (min > max) {
                                    DTInspectorUtility.ShowRedErrorBox(modifier._statName + " Min cannot exceed Max, please fix!");
                                }
                            }

                            DTInspectorUtility.ShowColorWarningBox("Limits are inclusive: i.e. 'Above' means >=");
                            if (indexToDelete.HasValue) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "delete Active Limit");
                                item.activeItemCriteria.DeleteByIndex(indexToDelete.Value);
                            }

                            DTInspectorUtility.VerticalSpace(2);
                        }

                        break;
                }

                PoolBossEditorUtility.DisplayPrefab(ref _isDirty, _settings, ref item.prefabToSpawn, ref item.prefabPoolBossCategory, "Prefab Pool Item");

                KillerVariablesHelper.DisplayKillerInt(ref _isDirty, item.thisWeight, "Weight", _settings);
                EditorGUILayout.EndVertical();
                DTInspectorUtility.AddSpaceForNonU5();
            }

            if (_settings.poolItems.Count > 0) {
                DTInspectorUtility.EndGroupedControls();
            }
        }

        if (topAdded) {
            var newItem = new WavePrefabPoolItem();
            var index = 0;
            if (_settings.poolItems.Count > 0) {
                index = _settings.poolItems.Count - 1;
            }

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Prefab Pool Item");
            _settings.poolItems.Insert(index, newItem);
        } else if (itemToDelete.HasValue) {
            if (_settings.poolItems.Count == 1) {
                DTInspectorUtility.ShowAlert("You cannot delete the only Prefab Pool item. Delete the entire Pool from the hierarchy if you wish.");

            } else {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Prefab Pool Item");
                _settings.poolItems.RemoveAt(itemToDelete.Value);
            }
        } else if (itemToInsert.HasValue) {
            var newItem = new WavePrefabPoolItem();
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Prefab Pool Item");
            _settings.poolItems.Insert(itemToInsert.Value + 1, newItem);
        }

        if (itemToClone.HasValue) {
            var newItem = CloningHelper.CloneWavePrefabPoolItem(_settings.poolItems[itemToClone.Value]);
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "clone Prefab Pool Item");
            _settings.poolItems.Insert(itemToClone.Value, newItem);
        }

        if (itemToShiftUp.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift up Prefab Pool Item");
            var item = _settings.poolItems[itemToShiftUp.Value];
            _settings.poolItems.Insert(itemToShiftUp.Value - 1, item);
            _settings.poolItems.RemoveAt(itemToShiftUp.Value + 1);
        }

        if (itemToShiftDown.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "shift down Prefab Pool Item");
            var index = itemToShiftDown.Value + 1;
            var item = _settings.poolItems[index];
            _settings.poolItems.Insert(index - 1, item);
            _settings.poolItems.RemoveAt(index + 1);
        }

        if (alphaSort) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "Alpha Sort Prefab Pool Items");
            _settings.poolItems.Sort(delegate(WavePrefabPoolItem x, WavePrefabPoolItem y) {
                if (x.prefabToSpawn == null) {
                    return -1;
                }
                if (y.prefabToSpawn == null) {
                    return 1;
                }

                return x.prefabToSpawn.name.CompareTo(y.prefabToSpawn.name);
            });
        }

        if (GUI.changed || topAdded || _isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void AddActiveLimit(string modifierName, WavePrefabPoolItem spec) {
        if (spec.activeItemCriteria.HasKey(modifierName)) {
            DTInspectorUtility.ShowAlert("This item already has a Active Limit for World Variable: " + modifierName + ". Please modify the existing one instead.");
            return;
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Active Limit");

        var myVar = WorldVariableTracker.GetWorldVariableScript(modifierName);

        spec.activeItemCriteria.statMods.Add(new WorldVariableRange(modifierName, myVar.varType));
    }

    private void ExpandCollapseAll(WavePrefabPool pool, bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle expand / collapse all");

        foreach (var poolItem in pool.poolItems) {
            poolItem.isExpanded = isExpand;
        }
    }

    private void AddPoolItem(Object o) {
        // ReSharper disable once PossibleNullReferenceException
        var go = (o as GameObject);
        if (go == null) {
            DTInspectorUtility.ShowAlert("You dragged an object which was not a Game Object. Not adding to Prefab Pool.");
            return;
        }

        var newItem = new WavePrefabPoolItem();
        newItem.prefabToSpawn = go.transform;

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Prefab Pool Item");
        _settings.poolItems.Add(newItem);
    }
}
