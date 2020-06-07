using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class PoolBossEditorUtility {
    private static PoolBoss _poolBoss;
    private static int _categoryNum;
    private static int _weightToCreate = 5;

    public static PoolBoss PoBoss {
        get {
            if (_poolBoss != null) {
                return _poolBoss;
            }

            _poolBoss = PoolBoss.Instance;

            return _poolBoss;
        }
    }

    public static bool PrefabIsInPoolBoss(Transform transPrefab) {
        var boss = PoBoss;

        if (boss == null) {
            return false;
        }

        if (transPrefab == null) {
            return true;
        }

        var match = boss.poolItems.Find(delegate(PoolBossItem obj) {
            return obj.prefabTransform != null && obj.prefabTransform.name == transPrefab.name;
        });

        return match != null;
    }

    public static void DisplayPrefab(ref bool isDirty, Object editorObject, ref Transform prefabInstance, ref string catName, string prefabName, bool willDisplayPrefab = true) {
        if (willDisplayPrefab) { 
            var newPrefab = (Transform)EditorGUILayout.ObjectField(prefabName, prefabInstance, typeof(Transform), true);
            if (newPrefab != prefabInstance) {
			    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change " + prefabName);
                prefabInstance = newPrefab;
            }
        }

#if UNITY_2018_2_OR_NEWER
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstance) as Transform;
#else
        var prefab = PrefabUtility.GetPrefabParent(prefabInstance) as Transform;
#endif
        if (prefab == null) {
            prefab = prefabInstance; // there is no parent because it was already the one from Hierarchy (prefab, not instance)
        }

        if (Application.isPlaying || PoBoss == null || PrefabIsInPoolBoss(prefab)) {
            return;
        }
		 
        DTInspectorUtility.StartGroupHeader(); 
        DTInspectorUtility.ShowRedErrorBox("This prefab is not configured in Pool Boss. Add it with the controls below or go to Pool Boss and add it manually.");

		var categories = new List<string>(PoBoss._categories.Count);
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < PoBoss._categories.Count; i++) {
            categories.Add(PoBoss._categories[i].CatName);
        }

        var existingCat = categories.IndexOf(catName);
        if (existingCat < 0) {
            existingCat = 0;
			isDirty = true;
			catName = categories[0];
        }

        _categoryNum = EditorGUILayout.Popup("Category", existingCat, categories.ToArray());
        if (_categoryNum != existingCat) {
			UndoHelper.RecordObjectPropertyForUndo(ref isDirty, editorObject, "change category");
            catName = categories[_categoryNum];
        }

        _weightToCreate = EditorGUILayout.IntField("Preload Qty", _weightToCreate);

        EditorGUILayout.BeginHorizontal();

        var oldColor = GUI.contentColor;

        GUILayout.Space(10);
        GUI.contentColor = DTInspectorUtility.AddButtonColor;
        if (GUILayout.Button("Create Pool Boss Item", EditorStyles.toolbarButton, GUILayout.Width(130))) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, PoBoss, "create Pool Boss item");
            PoBoss.poolItems.Add(new PoolBossItem() {
                prefabTransform = prefab,
                instancesToPreload = _weightToCreate,
                categoryName = catName
            });
        }
        GUILayout.Space(10);
        GUI.contentColor = DTInspectorUtility.BrightButtonColor;
        if (GUILayout.Button("Go to Pool Boss", EditorStyles.toolbarButton, GUILayout.Width(130))) {
            Selection.activeGameObject = PoBoss.gameObject;
        }

        GUI.contentColor = oldColor;
        EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndVertical();
    }
}
