using System.Collections.Generic;
using System.Text;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WavePrefabPoolGroup))]
// ReSharper disable once CheckNamespace
public class WavePrefabPoolGroupInspector : Editor {
	private bool _isDirty;
	private WavePrefabPoolGroup _settings;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        _settings = (WavePrefabPoolGroup)target;

        WorldVariableTracker.ClearInGamePlayerStats();

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/PrefabPools.htm");

        if (DTInspectorUtility.IsPrefabInProjectView(_settings)) {
            DTInspectorUtility.ShowSubGameObjectNotPrefabMessage();
            return;
        }

        _isDirty = false;

		DTInspectorUtility.BeginGroupedControls();

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

		DTInspectorUtility.EndGroupedControls();

		if (GUI.changed || _isDirty) {
			EditorUtility.SetDirty(target);	// or it won't save the data!!
		}
    }

	private void CreatePrefabPool() {
		var newPrefabPoolName = _settings.newPrefabPoolName;
		
		if (string.IsNullOrEmpty(newPrefabPoolName)) {
			DTInspectorUtility.ShowAlert("You must enter a name for your new Prefab Pool.");
			return;
		}
		
		var spawnPos = _settings.transform.position;
		
		var newPool = Instantiate(LevelSettings.Instance.PrefabPoolTrans.gameObject, spawnPos, Quaternion.identity) as GameObject;
		// ReSharper disable once PossibleNullReferenceException
		newPool.name = newPrefabPoolName;
		
		var poolsHolder = _settings.transform;
		
		var dupe = poolsHolder.GetChildTransform(newPrefabPoolName);
		if (dupe != null) {
			DTInspectorUtility.ShowAlert("You already have a Prefab Pool named '" + newPrefabPoolName + "', please choose another name.");
			
			DestroyImmediate(newPool);
			return;
		}
		
		UndoHelper.CreateObjectForUndo(newPool.gameObject, "create Prefab Pool");
		newPool.transform.parent = poolsHolder.transform;
	}
}
