using DarkTonic.CoreGameKit;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class KW_Instructions5 : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 90), "This scene shows how Composite Killables can be set up with zero code-writing. The mother ship has 2 sub-Killables of 'Fighter2', which each have a sub-Killable of 'Fighter1R'. Fighter2 is set to be invincible while children are alive, and Mothership is set the same. So you have to destroy the Fighter1R, then the Fighter2 (both sides), then the Mothership. This can be used to make uber-bosses and fortresses. A custom event is used to make the rotation of the spawners (Fighter2) look at you when you fire.");

		var scoreVar = WorldVariableTracker.GetWorldVariable("Score");
		var livesVar = WorldVariableTracker.GetWorldVariable("Lives");
		
		if (scoreVar != null) {
			GUI.Label(new Rect(10, 120, 100, 30), "Score: " + scoreVar.CurrentIntValue);
		}
		if (livesVar != null) {
			GUI.Label(new Rect(10, 150, 100, 30), "lives: " + livesVar.CurrentIntValue);
		}
	
	}
}
