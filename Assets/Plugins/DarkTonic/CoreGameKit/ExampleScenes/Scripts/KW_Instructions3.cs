using DarkTonic.CoreGameKit;
using UnityEngine;

public class KW_Instructions3 : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 100), "Click on enemies to do 1 damage to them. This level has one repeated wave. After each wave, the enemies get an additional hit point so it gets harder and harder! This is done with the Wave Repeat Bonus setting of the World Variable 'Enemy Toughness' which is assigned to the enemy's Hit Points. Pool Boss is set up correctly in this Scene with 2 prefabs.");
		
		var scoreVar = WorldVariableTracker.GetWorldVariable("Score");
		var experienceVar = WorldVariableTracker.GetWorldVariable("Experience Points");
		var healthVar = WorldVariableTracker.GetWorldVariable("Health");
		var livesVar = WorldVariableTracker.GetWorldVariable("Lives");
		
		if (scoreVar != null) {
			GUI.Label(new Rect(10, 120, 100, 30), "Score: " + scoreVar.CurrentIntValue);
		}
		if (experienceVar != null) {
			GUI.Label(new Rect(10, 160, 100, 30), "Exp: " + experienceVar.CurrentIntValue);
		}
		if (healthVar != null) {
			GUI.Label(new Rect(10, 190, 100, 30), "Health: " + healthVar.CurrentIntValue);
		}
		if (livesVar != null) {
			GUI.Label(new Rect(10, 220, 100, 30), "lives: " + livesVar.CurrentIntValue);
		}
	}
}
