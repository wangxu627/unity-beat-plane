using System.Globalization;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldVariable))]
// ReSharper disable once CheckNamespace
public class WorldVariableInspector : Editor {
    public override void OnInspectorGUI() {
        EditorGUI.indentLevel = 0;

        var stat = (WorldVariable)target;

        LevelSettings.Instance = null; // clear cached version
        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/WorldVariables.htm");

        if (DTInspectorUtility.IsPrefabInProjectView(stat)) {
            DTInspectorUtility.ShowSubGameObjectNotPrefabMessage();
            return;
        }

        var isDirty = false;

        var newName = EditorGUILayout.TextField("Name", stat.transform.name);
        if (newName != stat.transform.name) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat.gameObject, "change Name");
            stat.transform.name = newName;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Variable Type");
        GUILayout.FlexibleSpace();
        GUI.contentColor = DTInspectorUtility.BrightButtonColor;
        GUILayout.Label(WorldVariableTracker.GetVariableTypeFriendlyString(stat.varType));

        if (Application.isPlaying) {
            var sValue = "";

            switch (stat.varType) {
                case WorldVariableTracker.VariableType._integer:
                    var _int = WorldVariableTracker.GetExistingWorldVariableIntValue(stat.name, stat.startingValue);
                    sValue = _int.HasValue ? _int.Value.ToString() : "";
                    break;
                case WorldVariableTracker.VariableType._float:
                    var _float = WorldVariableTracker.GetExistingWorldVariableFloatValue(stat.name, stat.startingValue);
                    sValue = _float.HasValue ? _float.Value.ToString(CultureInfo.InvariantCulture) : "";
                    break;
                default:
                    Debug.Log("add code for varType: " + stat.varType);
                    break;
            }

            GUILayout.Label("Value:");

            GUILayout.Label(sValue);
        }

        GUILayout.Space(110);
        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();

        if (Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Change Value", GUILayout.Width(100));
            GUILayout.FlexibleSpace();
            switch (stat.varType) {
                case WorldVariableTracker.VariableType._integer:
                    stat.prospectiveValue = EditorGUILayout.IntField("", stat.prospectiveValue, GUILayout.Width(120));
                    break;
                case WorldVariableTracker.VariableType._float:
                    stat.prospectiveFloatValue = EditorGUILayout.FloatField("", stat.prospectiveFloatValue, GUILayout.Width(120));
                    break;
                default:
                    Debug.LogError("Add code for varType: " + stat.varType.ToString());
                    break;
            }

            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Change Value", EditorStyles.toolbarButton, GUILayout.Width(80))) {
                var variable = WorldVariableTracker.GetWorldVariable(stat.name);

                switch (stat.varType) {
                    case WorldVariableTracker.VariableType._integer:
                        variable.CurrentIntValue = stat.prospectiveValue;
                        break;
                    case WorldVariableTracker.VariableType._float:
                        variable.CurrentFloatValue = stat.prospectiveFloatValue;
                        break;
                    default:
                        Debug.LogError("Add code for varType: " + stat.varType.ToString());
                        break;
                }
            }
            GUI.contentColor = Color.white;

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
        }

        var newPersist = (WorldVariable.StatPersistanceMode)EditorGUILayout.EnumPopup("Persistence mode", stat.persistanceMode);
        if (newPersist != stat.persistanceMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Persistence Mode");
            stat.persistanceMode = newPersist;
        }

        var newChange = (WorldVariable.VariableChangeMode)EditorGUILayout.EnumPopup("Modifications allowed", stat.changeMode);
        if (newChange != stat.changeMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Modifications allowed");
            stat.changeMode = newChange;
        }

        switch (stat.varType) {
            case WorldVariableTracker.VariableType._float: {
                    var newStart = EditorGUILayout.FloatField("Starting value", stat.startingValueFloat);
                    if (newStart != stat.startingValueFloat) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Starting value");
                        stat.startingValueFloat = newStart;
                    }
                    break;
                }
            case WorldVariableTracker.VariableType._integer: {
                    var newStart = EditorGUILayout.IntField("Starting value", stat.startingValue);
                    if (newStart != stat.startingValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Starting value");
                        stat.startingValue = newStart;
                    }
                    break;
                }
        }

        var newNeg = EditorGUILayout.Toggle("Allow negative?", stat.allowNegative);
        if (newNeg != stat.allowNegative) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "toggle Allow negative");
            stat.allowNegative = newNeg;
        }

        DTInspectorUtility.StartGroupHeader();
        var newTopLimit = GUILayout.Toggle(stat.hasMaxValue, "Has max value?");
        if (newTopLimit != stat.hasMaxValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "toggle Has max value");
            stat.hasMaxValue = newTopLimit;
        }
        EditorGUILayout.EndVertical();

        if (stat.hasMaxValue) {
            EditorGUI.indentLevel = 0;
            switch (stat.varType) {
                case WorldVariableTracker.VariableType._integer:
                    var newMax = EditorGUILayout.IntField("Max Value", stat.intMaxValue);
                    if (newMax != stat.intMaxValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Max Value");
                        stat.intMaxValue = newMax;
                    }
                    break;
                case WorldVariableTracker.VariableType._float:
                    var newFloatMax = EditorGUILayout.FloatField("Max Value", stat.floatMaxValue);
                    if (newFloatMax != stat.floatMaxValue) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change Max Value");
                        stat.floatMaxValue = newFloatMax;
                    }
                    break;
                default:
                    Debug.Log("add code for varType: " + stat.varType);
                    break;
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;

        DTInspectorUtility.AddSpaceForNonU5();

        DTInspectorUtility.StartGroupHeader();
        var newCanEnd = GUILayout.Toggle(stat.canEndGame, "Triggers game over?");
        if (newCanEnd != stat.canEndGame) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "toggle Triggers game over");
            stat.canEndGame = newCanEnd;
        }
        EditorGUILayout.EndVertical();
        if (stat.canEndGame) {
            EditorGUI.indentLevel = 0;
            var newMin = EditorGUILayout.IntField("G.O. min value", stat.endGameMinValue);
            if (newMin != stat.endGameMinValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change G.O. min value");
                stat.endGameMinValue = newMin;
            }

            var newMax = EditorGUILayout.IntField("G.O. max value", stat.endGameMaxValue);
            if (newMax != stat.endGameMaxValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "change G.O. max value");
                stat.endGameMaxValue = newMax;
            }
        }
        EditorGUILayout.EndVertical();

        DTInspectorUtility.StartGroupHeader();
        var newFire = GUILayout.Toggle(stat.fireEventsOnChange, "Custom Events");
        if (newFire != stat.fireEventsOnChange) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "toggle Custom Events");
            stat.fireEventsOnChange = newFire;
        }
        EditorGUILayout.EndVertical();
        if (stat.fireEventsOnChange) {
            EditorGUI.indentLevel = 0;

            DTInspectorUtility.ShowColorWarningBox("When variable value changes, fire the Custom Events below");

            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTInspectorUtility.AddButtonColor;
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Add", "Click to add a Custom Event"), EditorStyles.toolbarButton, GUILayout.Width(50))) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "Add Custom Event");
                stat.changeCustomEventsToFire.Add(new CGKCustomEventToFire());
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();

            if (stat.changeCustomEventsToFire.Count == 0) {
                DTInspectorUtility.ShowColorWarningBox("You have no Custom Events selected to fire.");
            }

            DTInspectorUtility.VerticalSpace(2);

            int? indexToDelete = null;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < stat.changeCustomEventsToFire.Count; i++) {
                var anEvent = stat.changeCustomEventsToFire[i].CustomEventName;

                var buttonClicked = DTInspectorUtility.FunctionButtons.None;
                anEvent = DTInspectorUtility.SelectCustomEventForVariable(ref isDirty, anEvent,
                    stat, "Custom Event", ref buttonClicked);
                if (buttonClicked == DTInspectorUtility.FunctionButtons.Remove) {
                    indexToDelete = i;
                }

                if (anEvent == stat.changeCustomEventsToFire[i].CustomEventName) {
                    continue;
                }

                stat.changeCustomEventsToFire[i].CustomEventName = anEvent;
            }

            if (indexToDelete.HasValue) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "Remove Custom Event");
                stat.changeCustomEventsToFire.RemoveAt(indexToDelete.Value);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;
        var listenerWasEmpty = stat.listenerPrefab == null;
        var newListener = (WorldVariableListener)EditorGUILayout.ObjectField("Listener", stat.listenerPrefab, typeof(WorldVariableListener), true);
        if (newListener != stat.listenerPrefab) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, stat, "assign Listener");
            stat.listenerPrefab = newListener;
            if (listenerWasEmpty && stat.listenerPrefab != null) {
                // just assigned.
                var listener = stat.listenerPrefab.GetComponent<WorldVariableListener>();
                if (listener == null) {
                    DTInspectorUtility.ShowAlert("You cannot assign a listener that doesn't have a WorldVariableListener script in it.");
                    stat.listenerPrefab = null;
                } else {
                    listener.variableName = stat.transform.name;
                    listener.vType = stat.varType;
                }
            }
        }

        if (GUI.changed || isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }
}
