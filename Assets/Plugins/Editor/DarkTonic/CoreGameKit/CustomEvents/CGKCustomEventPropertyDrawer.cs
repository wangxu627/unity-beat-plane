using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CoreCustomEventAttribute))]
// ReSharper disable once CheckNamespace
public class CGKCustomEventPropertyDrawer : PropertyDrawer {
    // ReSharper disable once InconsistentNaming
    public int index;
    // ReSharper disable once InconsistentNaming
    public bool typeIn;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!typeIn) {
            return base.GetPropertyHeight(property, label);
        }
        return base.GetPropertyHeight(property, label) + 16;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        var ls = LevelSettings.Instance;
        // ReSharper disable once RedundantAssignment
        var groupName = "[Type In]";

        if (ls == null) {
            index = -1;
            typeIn = false;
            property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
            return;
        }

        index = ls.CustomEventNames.IndexOf(property.stringValue);

        if (typeIn || index == -1) {
            index = 0;
            typeIn = true;
            position.height -= 16;
        }

        index = EditorGUI.Popup(position, label.text, index, LevelSettings.Instance.CustomEventNames.ToArray());
        groupName = LevelSettings.Instance.CustomEventNames[index];

        switch (groupName) {
            case "[Type In]":
                typeIn = true;
                position.yMin += 16;
                position.height += 16;
                EditorGUI.BeginChangeCheck();
                property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
                EditorGUI.EndChangeCheck();
                break;
            default:
                typeIn = false;
                property.stringValue = groupName;
                break;
        }
    }
}