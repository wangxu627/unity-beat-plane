using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimedDespawner))]
// ReSharper disable once CheckNamespace
public class TimedDespawnerInspector : Editor {
	public override void OnInspectorGUI() {
		EditorGUI.indentLevel = 0;
		
		var settings = (TimedDespawner)target;
		LevelSettings.Instance = null; // clear cached version

        DTInspectorUtility.DrawTexture(CoreGameKitInspectorResources.LogoTexture);
        DTInspectorUtility.HelpHeader("http://www.dtdevtools.com/docs/coregamekit/Despawners.htm#TimedDespawner");

        var isDirty = false;

		EditorGUILayout.Separator(); 
		
		var newStartTimer = EditorGUILayout.Toggle("Start Timer On Awake", settings.StartTimerOnSpawn);
		if (newStartTimer != settings.StartTimerOnSpawn) {
			UndoHelper.RecordObjectPropertyForUndo(ref isDirty, settings, "toggle Start Timer On Awake");
			settings.StartTimerOnSpawn = newStartTimer;
		}
		
		var newLifeSeconds = EditorGUILayout.Slider("Despawn Timer (sec)", settings.LifeSeconds, .1f, 50f);
		if (newLifeSeconds != settings.LifeSeconds) {
			UndoHelper.RecordObjectPropertyForUndo(ref isDirty, settings, "change Despawn Timer");
			settings.LifeSeconds = newLifeSeconds;
		}

		var hadNoListener = settings.listener == null;
		var newListener = (TimedDespawnerListener) EditorGUILayout.ObjectField("Listener", settings.listener, typeof(TimedDespawnerListener), true);
		if (newListener != settings.listener) {
			UndoHelper.RecordObjectPropertyForUndo(ref isDirty, settings, "assign Listener");
			settings.listener = newListener;

			if (hadNoListener && settings.listener != null) {
				settings.listener.sourceDespawnerName = settings.transform.name;
			}
		}
		
		if (GUI.changed || isDirty) {
  			EditorUtility.SetDirty(target);	// or it won't save the data!!
		}

		//DrawDefaultInspector();
    }

}
