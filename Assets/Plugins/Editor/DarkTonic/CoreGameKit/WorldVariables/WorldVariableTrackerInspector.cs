using System.Collections.Generic;
using System.Globalization;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldVariableTracker))]
// ReSharper disable once CheckNamespace
public class WorldVariableTrackerInspector : Editor {
    private WorldVariableTracker _holder;
    private List<WorldVariable> _stats;
    private bool _isDirty;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
       EditorGUI.indentLevel = 0;

        _holder = (WorldVariableTracker)target;

        LevelSettings.Instance = null; // clear cached version
        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/WorldVariables.htm");

        _isDirty = false;

        if (DTInspectorUtility.IsPrefabInProjectView(_holder)) {
            DTInspectorUtility.ShowSubGameObjectNotPrefabMessage();
            return;
        }

        _stats = GetPlayerStatsFromChildren(_holder.transform);

        Transform statToRemove = null;

        _stats.Sort(delegate(WorldVariable x, WorldVariable y) {
            return x.name.CompareTo(y.name);
        });

        DTInspectorUtility.StartGroupHeader();
        EditorGUI.indentLevel = 1;
        var newShowNewVar = DTInspectorUtility.Foldout(_holder.showNewVarSection, "Create New");

        EditorGUI.indentLevel = 0;
        if (newShowNewVar != _holder.showNewVarSection) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "toggle Create New");
            _holder.showNewVarSection = newShowNewVar;
        }
        EditorGUILayout.EndVertical();

        if (_holder.showNewVarSection) {
            var newVarName = EditorGUILayout.TextField("New Variable Name", _holder.newVariableName);
            if (newVarName != _holder.newVariableName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "change New Variable Name");
                _holder.newVariableName = newVarName;
            }

            var newVarType = (WorldVariableTracker.VariableType)EditorGUILayout.EnumPopup("New Variable Type", _holder.newVarType);
            if (newVarType != _holder.newVarType) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "change New Variable Type");
                _holder.newVarType = newVarType;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Actions");
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            GUILayout.Space(101);
            if (GUILayout.Button("Create Variable", EditorStyles.toolbarButton, GUILayout.MaxWidth(100))) {
                CreateNewVariable(_holder.newVariableName, _holder.newVarType);
                _isDirty = true;
            }
            GUILayout.FlexibleSpace();
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        EditorGUILayout.LabelField("All World Variables");
        EditorGUILayout.EndVertical();

        var totalInt = _stats.FindAll(delegate(WorldVariable obj) {
            return obj.varType == WorldVariableTracker.VariableType._integer;
        });
        var totalFloat = _stats.FindAll(delegate(WorldVariable obj) {
            return obj.varType == WorldVariableTracker.VariableType._float;
        });

        var showIntVariable = EditorGUILayout.Toggle("Show Integers (" + totalInt.Count + ")", _holder.showIntVars);
        if (showIntVariable != _holder.showIntVars) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "toggle Show Integers");
            _holder.showIntVars = showIntVariable;
        }

        var showFloatVariable = EditorGUILayout.Toggle("Show Floats (" + totalFloat.Count + ")", _holder.showFloatVars);
        if (showFloatVariable != _holder.showFloatVars) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "toggle Show Floats");
            _holder.showFloatVars = showFloatVariable;
        }

        var filteredStats = new List<WorldVariable>();
        filteredStats.AddRange(_stats);
        if (!_holder.showIntVars) {
            filteredStats.RemoveAll(delegate(WorldVariable obj) {
                return obj.varType == WorldVariableTracker.VariableType._integer;
            });
        }
        if (!_holder.showFloatVars) {
            filteredStats.RemoveAll(delegate(WorldVariable obj) {
                return obj.varType == WorldVariableTracker.VariableType._float;
            });
        }

        if (filteredStats.Count == 0) {
            DTInspectorUtility.ShowColorWarningBox("You have no World Variables of the selected type(s).");
        }
        EditorGUILayout.EndVertical();

        DTInspectorUtility.AddSpaceForNonU5();

        var state = _holder.worldVariablesExpanded;
        var text = string.Format("World Variables ({0})", filteredStats.Count);

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

        if (state != _holder.worldVariablesExpanded) {
            _holder.worldVariablesExpanded = state;
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "toggle World Variables");
        }

        // Add expand/collapse buttons if there are items in the list
        if (_stats.Count > 0) {
            GUI.backgroundColor = Color.white;

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            const string collapseIcon = "Collapse";
            var content = new GUIContent(collapseIcon, "Click to collapse all");
            var masterCollapse = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));

            const string expandIcon = "Expand";
            content = new GUIContent(expandIcon, "Click to expand all");
            var masterExpand = GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Height(16));
            if (masterExpand) {
                ExpandCollapseAll(true);
            }
            if (masterCollapse) {
                ExpandCollapseAll(false);
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
        }

        GUILayout.Space(4);
        EditorGUILayout.EndHorizontal();

        if (_holder.worldVariablesExpanded) {
            if (filteredStats.Count > 0) {
                DTInspectorUtility.BeginGroupedControls();
            }
            for (var i = 0; i < filteredStats.Count; i++) {
                var aStat = filteredStats[i];

                var varDirty = false;

                DTInspectorUtility.StartGroupHeader();
                EditorGUI.indentLevel = 1;
                EditorGUILayout.BeginHorizontal();
                var newExpand = DTInspectorUtility.Foldout(aStat.isExpanded, aStat.name);
                if (newExpand != aStat.isExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "toggle expand Variables");
                    aStat.isExpanded = newExpand;
                }

                GUILayout.FlexibleSpace();

                if (Application.isPlaying) {
                    GUI.contentColor = DTInspectorUtility.BrightTextColor;
                    var sValue = "";

                    switch (aStat.varType) {
                        case WorldVariableTracker.VariableType._integer:
                            var _int = WorldVariableTracker.GetExistingWorldVariableIntValue(aStat.name, aStat.startingValue);
                            sValue = _int.HasValue ? _int.Value.ToString() : "";
                            break;
                        case WorldVariableTracker.VariableType._float:
                            var _float = WorldVariableTracker.GetExistingWorldVariableFloatValue(aStat.name, aStat.startingValueFloat);
                            sValue = _float.HasValue ? _float.Value.ToString(CultureInfo.InvariantCulture) : "";
                            break;
                        default:
                            Debug.Log("add code for varType: " + aStat.varType);
                            break;
                    }

                    EditorGUILayout.LabelField("Value: " + sValue, GUILayout.Width(120));
                    GUI.contentColor = Color.white;
                    GUILayout.Space(10);
                }

                GUI.contentColor = DTInspectorUtility.BrightTextColor;
                GUILayout.Label(WorldVariableTracker.GetVariableTypeFriendlyString(aStat.varType));
                switch (aStat.varType) {
                    case WorldVariableTracker.VariableType._float:
                        GUILayout.Space(15);
                        break;
                }
                GUI.contentColor = Color.white;
                var functionPressed = DTInspectorUtility.AddFoldOutListItemButtons(i, _stats.Count, "variable", false, null, false);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                if (aStat.isExpanded) {
                    EditorGUI.indentLevel = 0;

                    var newName = EditorGUILayout.TextField("Name", aStat.transform.name);
                    if (newName != aStat.transform.name) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat.gameObject, "change Name");
                        aStat.transform.name = newName;
                    }

                    if (Application.isPlaying) {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Change Value", GUILayout.Width(100));
                        GUILayout.FlexibleSpace();
                        switch (aStat.varType) {
                            case WorldVariableTracker.VariableType._integer:
                                aStat.prospectiveValue = EditorGUILayout.IntField("", aStat.prospectiveValue, GUILayout.Width(120));
                                break;
                            case WorldVariableTracker.VariableType._float:
                                aStat.prospectiveFloatValue = EditorGUILayout.FloatField("", aStat.prospectiveFloatValue, GUILayout.Width(120));
                                break;
                            default:
                                Debug.LogError("Add code for varType: " + aStat.varType.ToString());
                                break;
                        }

                        GUI.contentColor = DTInspectorUtility.BrightButtonColor;
                        if (GUILayout.Button("Change Value", EditorStyles.toolbarButton, GUILayout.Width(80))) {
                            var variable = WorldVariableTracker.GetWorldVariable(aStat.name);

                            switch (aStat.varType) {
                                case WorldVariableTracker.VariableType._integer:
                                    variable.CurrentIntValue = aStat.prospectiveValue;
                                    break;
                                case WorldVariableTracker.VariableType._float:
                                    variable.CurrentFloatValue = aStat.prospectiveFloatValue;
                                    break;
                                default:
                                    Debug.LogError("Add code for varType: " + aStat.varType.ToString());
                                    break;
                            }
                        }
                        GUI.contentColor = Color.white;

                        GUILayout.Space(10);

                        EditorGUILayout.EndHorizontal();
                    }


                    var newPersist = (WorldVariable.StatPersistanceMode)EditorGUILayout.EnumPopup("Persistence mode", aStat.persistanceMode);
                    if (newPersist != aStat.persistanceMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Persistence mode");
                        aStat.persistanceMode = newPersist;
                    }

                    var newChange = (WorldVariable.VariableChangeMode)EditorGUILayout.EnumPopup("Modifications allowed", aStat.changeMode);
                    if (newChange != aStat.changeMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Modifications allowed");
                        aStat.changeMode = newChange;
                    }

                    switch (aStat.varType) {
                        case WorldVariableTracker.VariableType._integer:
                            var newStart = EditorGUILayout.IntField("Starting value", aStat.startingValue);
                            if (newStart != aStat.startingValue) {
                                UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Starting value");
                                aStat.startingValue = newStart;
                            }
                            break;
                        case WorldVariableTracker.VariableType._float:
                            var newStartFloat = EditorGUILayout.FloatField("Starting value", aStat.startingValueFloat);
                            if (newStartFloat != aStat.startingValueFloat) {
                                UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Starting value");
                                aStat.startingValueFloat = newStartFloat;
                            }
                            break;
                        default:
                            Debug.Log("add code for varType: " + aStat.varType);
                            break;
                    }

                    var newNeg = EditorGUILayout.Toggle("Allow negative?", aStat.allowNegative);
                    if (newNeg != aStat.allowNegative) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "toggle Allow negative");
                        aStat.allowNegative = newNeg;
                    }

                    DTInspectorUtility.StartGroupHeader(1);

                    var newTopLimit = GUILayout.Toggle(aStat.hasMaxValue, "Has max value?");
                    if (newTopLimit != aStat.hasMaxValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "toggle Has max value");
                        aStat.hasMaxValue = newTopLimit;
                    }
                    EditorGUILayout.EndVertical();

                    if (aStat.hasMaxValue) {
                        EditorGUI.indentLevel = 0;
                        switch (aStat.varType) {
                            case WorldVariableTracker.VariableType._integer:
                                var newMax = EditorGUILayout.IntField("Max Value", aStat.intMaxValue);
                                if (newMax != aStat.intMaxValue) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Max Value");
                                    aStat.intMaxValue = newMax;
                                }
                                break;
                            case WorldVariableTracker.VariableType._float:
                                var newFloatMax = EditorGUILayout.FloatField("Max Value", aStat.floatMaxValue);
                                if (newFloatMax != aStat.floatMaxValue) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change Max Value");
                                    aStat.floatMaxValue = newFloatMax;
                                }
                                break;
                            default:
                                Debug.Log("add code for varType: " + aStat.varType);
                                break;
                        }
                    }
                    EditorGUILayout.EndVertical();
                    DTInspectorUtility.AddSpaceForNonU5(1);

                    DTInspectorUtility.StartGroupHeader(1);
                    EditorGUI.indentLevel = 0;
                    var newCanEnd = GUILayout.Toggle(aStat.canEndGame, "Triggers game over?");
                    if (newCanEnd != aStat.canEndGame) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "toggle Triggers game over");
                        aStat.canEndGame = newCanEnd;
                    }
                    EditorGUILayout.EndVertical();
                    if (aStat.canEndGame) {
                        EditorGUI.indentLevel = 0;
                        switch (aStat.varType) {
                            case WorldVariableTracker.VariableType._integer:
                                var newMin = EditorGUILayout.IntField("G.O. min value", aStat.endGameMinValue);
                                if (newMin != aStat.endGameMinValue) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change G.O. min value");
                                    aStat.endGameMinValue = newMin;
                                }

                                var newMax = EditorGUILayout.IntField("G.O. max value", aStat.endGameMaxValue);
                                if (newMax != aStat.endGameMaxValue) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change G.O. max value");
                                    aStat.endGameMaxValue = newMax;
                                }
                                break;
                            case WorldVariableTracker.VariableType._float:
                                var newMinFloat = EditorGUILayout.FloatField("G.O. min value", aStat.endGameMinValueFloat);
                                if (newMinFloat != aStat.endGameMinValueFloat) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change G.O. min value");
                                    aStat.endGameMinValueFloat = newMinFloat;
                                }

                                var newMaxFloat = EditorGUILayout.FloatField("G.O. max value", aStat.endGameMaxValueFloat);
                                if (newMaxFloat != aStat.endGameMaxValueFloat) {
                                    UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "change G.O. max value");
                                    aStat.endGameMaxValueFloat = newMaxFloat;
                                }
                                break;
                            default:
                                Debug.Log("add code for varType: " + aStat.varType);
                                break;
                        }
                    }
                    EditorGUILayout.EndVertical();

                    DTInspectorUtility.AddSpaceForNonU5(1);
                    DTInspectorUtility.StartGroupHeader(1);
                    var newFire = GUILayout.Toggle(aStat.fireEventsOnChange, "Custom Events");
                    if (newFire != aStat.fireEventsOnChange) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, aStat, "toggle Custom Events");
                        aStat.fireEventsOnChange = newFire;
                    }
                    EditorGUILayout.EndVertical();
                    if (aStat.fireEventsOnChange) {
                        EditorGUI.indentLevel = 0;

                        DTInspectorUtility.ShowColorWarningBox("When variable value changes, fire the Custom Events below");

                        EditorGUILayout.BeginHorizontal();
                        GUI.contentColor = DTInspectorUtility.AddButtonColor;
                        GUILayout.Space(10);
                        if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, aStat, "Add Custom Event");
                            aStat.changeCustomEventsToFire.Add(new CGKCustomEventToFire());
                        }
                        GUI.contentColor = Color.white;

                        EditorGUILayout.EndHorizontal();

                        if (aStat.changeCustomEventsToFire.Count == 0) {
                            DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
                        }

                        DTInspectorUtility.VerticalSpace(2);

                        int? indexToDelete = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var j = 0; j < aStat.changeCustomEventsToFire.Count; j++) {
                            var anEvent = aStat.changeCustomEventsToFire[j].CustomEventName;

                            var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                            anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref _isDirty, anEvent,
                                aStat, "Custom Event", ref buttonClicked);
                            if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove) {
                                indexToDelete = j;
                            }

                            if (anEvent == aStat.changeCustomEventsToFire[j].CustomEventName) {
                                continue;
                            }

                            aStat.changeCustomEventsToFire[j].CustomEventName = anEvent;
                        }

                        if (indexToDelete.HasValue) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, aStat, "Remove Custom Event");
                            aStat.changeCustomEventsToFire.RemoveAt(indexToDelete.Value);
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUI.indentLevel = 0;
                    var listenerWasEmpty = aStat.listenerPrefab == null;
                    var newListener = (WorldVariableListener)EditorGUILayout.ObjectField("Listener", aStat.listenerPrefab, typeof(WorldVariableListener), true);
                    if (newListener != aStat.listenerPrefab) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varDirty, aStat, "assign Listener");
                        aStat.listenerPrefab = newListener;
                        if (listenerWasEmpty && aStat.listenerPrefab != null) {
                            // just assigned.
                            var listener = aStat.listenerPrefab.GetComponent<WorldVariableListener>();
                            if (listener == null) {
                                DTInspectorUtility.ShowAlert("You cannot assign a listener that doesn't have a WorldVariableListener script in it.");
                                aStat.listenerPrefab = null;
                            } else {
                                listener.variableName = aStat.transform.name;
                            }
                        }
                    }
                }

                switch (functionPressed) {
                    case DTInspectorUtility.FunctionButtons.Remove:
                        statToRemove = aStat.transform;
                        break;
                }

                if (varDirty) {
                    EditorUtility.SetDirty(aStat);
                }

                EditorGUILayout.EndVertical();

                DTInspectorUtility.AddSpaceForNonU5();
            }

            if (filteredStats.Count > 0) {
                DTInspectorUtility.EndGroupedControls();
            }
        }

        if (statToRemove != null) {
            _isDirty = true;
            RemoveStat(statToRemove);
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private static List<WorldVariable> GetPlayerStatsFromChildren(Transform holder) {
        var stats = new List<WorldVariable>();

        for (var i = 0; i < holder.childCount; i++) {
            var aTrans = holder.GetChild(i);

            var aStat = aTrans.GetComponent<WorldVariable>();
            if (aStat == null) {
                DTInspectorUtility.ShowRedErrorBox("A prefab under 'PlayerStats' named '" + aTrans.name + "' does not have a WorldVariable script. Please delete it.");
                continue;
            }

            stats.Add(aStat);
        }

        return stats;
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void RemoveStat(Transform stat) {
        UndoHelper.DestroyForUndo(stat.gameObject);
    }

    private void CreateNewVariable(string varName, WorldVariableTracker.VariableType varType) {
        varName = varName.Trim();

        var match = _holder.transform.GetChildTransform(varName);
        if (match != null) {
            DTInspectorUtility.ShowAlert("You already have a World Variable named '" + varName + "'. Please choose a unique name.");
            return;
        }

        var newStat = (GameObject)Instantiate(_holder.statPrefab.gameObject, _holder.transform.position, Quaternion.identity);

        UndoHelper.CreateObjectForUndo(newStat, "create World Variable");

        newStat.name = varName;

        var variable = newStat.GetComponent<WorldVariable>();
        variable.varType = varType;

        newStat.transform.parent = _holder.transform;
    }

    private void ExpandCollapseAll(bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _holder, "toggle expand / collapse all World Variables");

        foreach (var variable in _stats) {
            variable.isExpanded = isExpand;
        }
    }
}
