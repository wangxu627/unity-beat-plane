using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TriggeredDespawner), true)]
// ReSharper disable once CheckNamespace
public class TriggeredDespawnerInspector : Editor {
	private TriggeredDespawner _settings;
	private bool _isDirty;

	public override void OnInspectorGUI() {
		_settings = (TriggeredDespawner)target;
		LevelSettings.Instance = null; // clear cached version

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/Despawners.htm#TriggeredDespawner");

        _isDirty = false;

		EditorGUI.indentLevel = 0;

        var hadNoListener = _settings.listener == null;
        var newListener = (TriggeredDespawnerListener)EditorGUILayout.ObjectField("Listener", _settings.listener, typeof(TriggeredDespawnerListener), true);
        if (newListener != _settings.listener) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "assign Listener");
            _settings.listener = newListener;

            if (hadNoListener && _settings.listener != null) {
                _settings.listener.sourceDespawnerName = _settings.transform.name;
            }
        }

	    var changedList = new List<bool>
	    {
	        RenderEventTypeControls(_settings.invisibleSpec, "Invisible Event", TriggeredSpawner.EventType.Invisible),
	        RenderEventTypeControls(_settings.mouseOverSpec, "Mouse Over (Legacy) Event",
	            TriggeredSpawner.EventType.MouseOver_Legacy),
	        RenderEventTypeControls(_settings.mouseClickSpec, "Mouse Click (Legacy) Event",
	            TriggeredSpawner.EventType.MouseClick_Legacy),
	        RenderEventTypeControls(_settings.collisionSpec, "Collision Enter Event",
	            TriggeredSpawner.EventType.OnCollision),
	        RenderEventTypeControls(_settings.triggerEnterSpec, "Trigger Enter Event",
	            TriggeredSpawner.EventType.OnTriggerEnter),
	        RenderEventTypeControls(_settings.triggerExitSpec, "Trigger Exit Event",
	            TriggeredSpawner.EventType.OnTriggerExit),
	        RenderEventTypeControls(_settings.onClickSpec, "NGUI OnClick Event", TriggeredSpawner.EventType.OnClick_NGUI),
	        RenderEventTypeControls(_settings.collision2dSpec, "2D Collision Enter Event",
	            TriggeredSpawner.EventType.OnCollision2D),
	        RenderEventTypeControls(_settings.triggerEnter2dSpec, "2D Trigger Enter Event",
	            TriggeredSpawner.EventType.OnTriggerEnter2D),
	        RenderEventTypeControls(_settings.triggerExit2dSpec, "2D Trigger Exit Event",
	            TriggeredSpawner.EventType.OnTriggerExit2D)
	    };

		if (GUI.changed || _isDirty || changedList.Contains(true)) {
  			EditorUtility.SetDirty(target);	// or it won't save the data!!
		}

		//DrawDefaultInspector();
    }
	
	private bool RenderEventTypeControls(EventDespawnSpecifics despawnSettings, string toggleText, TriggeredSpawner.EventType eventType) {
        DTInspectorUtility.VerticalSpace(2);
        EditorGUI.indentLevel = 0;

        var state = despawnSettings.eventEnabled;
	    var text = " " + toggleText;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (!state) {
            GUI.backgroundColor = DTInspectorUtility.InactiveHeaderColor;
        } else {
            GUI.backgroundColor = DTInspectorUtility.ActiveHeaderColor;
        }

		DTInspectorUtility.StartGroupHeader(0);

		var newTog = EditorGUILayout.BeginToggleGroup(text, despawnSettings.eventEnabled);
		if (newTog != despawnSettings.eventEnabled) {
			UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle " + toggleText + " enabled");
			despawnSettings.eventEnabled = newTog;
		}


	    if (!despawnSettings.eventEnabled) {
			EditorGUILayout.EndToggleGroup();
			DTInspectorUtility.EndGroupHeader();
			return _isDirty;
	    }
	    
        DTInspectorUtility.BeginGroupedControls();

        if (TriggeredSpawner.eventsWithTagLayerFilters.Contains(eventType)) {
	        DTInspectorUtility.StartGroupHeader(1);
            var newUseLayerFilter = EditorGUILayout.BeginToggleGroup(" Layer filters", despawnSettings.useLayerFilter);
	        if (newUseLayerFilter != despawnSettings.useLayerFilter) {
	            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "toggle Layer filters");
	            despawnSettings.useLayerFilter = newUseLayerFilter;
	        }
            DTInspectorUtility.EndGroupHeader();
            if (despawnSettings.useLayerFilter) {
	            for (var i = 0; i < despawnSettings.matchingLayers.Count; i++) {
	                var newMatch = EditorGUILayout.LayerField("Layer Match " + (i + 1), despawnSettings.matchingLayers[i]);
	                if (newMatch == despawnSettings.matchingLayers[i])
	                {
	                    continue;
	                }
	                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Layer Match");
	                despawnSettings.matchingLayers[i] = newMatch;
	            }
	            EditorGUILayout.BeginHorizontal();
	            GUILayout.Space(12);
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
	            if (GUILayout.Button(new GUIContent("Add", "Click to add a Layer Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
	                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Layer Match");
						
	                despawnSettings.matchingLayers.Add(0);
	            }
	            GUILayout.Space(10);
	            if (despawnSettings.matchingLayers.Count > 1) {
	                if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Layer Match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
	                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Layer Match");

	                    despawnSettings.matchingLayers.RemoveAt(despawnSettings.matchingLayers.Count - 1);
	                }
	            }
	            GUI.contentColor = Color.white;
	            EditorGUILayout.EndHorizontal();
	        }
	        EditorGUILayout.EndToggleGroup();

            DTInspectorUtility.AddSpaceForNonU5();

            DTInspectorUtility.StartGroupHeader(1);
            despawnSettings.useTagFilter = EditorGUILayout.BeginToggleGroup(" Tag filter", despawnSettings.useTagFilter);
            DTInspectorUtility.EndGroupHeader();
            if (despawnSettings.useTagFilter) {
                for (var i = 0; i < despawnSettings.matchingTags.Count; i++) {
	                var newMatch = EditorGUILayout.TagField("Tag Match " + (i + 1), despawnSettings.matchingTags[i]);
	                if (newMatch == despawnSettings.matchingTags[i])
	                {
	                    continue;
	                }
	                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "change Tag Match");
	                despawnSettings.matchingTags[i] = newMatch;
	            }
	            EditorGUILayout.BeginHorizontal();
	            GUILayout.Space(12);
                GUI.contentColor = DTInspectorUtility.BrightButtonColor;
	            if (GUILayout.Button(new GUIContent("Add", "Click to add a Tag Match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
	                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "add Tag Match");
	                despawnSettings.matchingTags.Add("Untagged");
	            }
	            GUILayout.Space(10);
	            if (despawnSettings.matchingTags.Count > 1) {
	                if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last Tag Match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
	                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _settings, "remove Tag Match");
	                    despawnSettings.matchingTags.RemoveAt(despawnSettings.matchingLayers.Count - 1);
	                }
	            }
	            GUI.contentColor = Color.white;
	            EditorGUILayout.EndHorizontal();
	        }
	        EditorGUILayout.EndToggleGroup();
	    } else {
	        EditorGUI.indentLevel = 0;
	        DTInspectorUtility.ShowColorWarningBox("No additional properties for this event type.");
	    }

        DTInspectorUtility.EndGroupedControls();

		EditorGUILayout.EndToggleGroup();
		DTInspectorUtility.EndGroupHeader();

	    return _isDirty;
	}

}
