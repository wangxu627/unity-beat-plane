using DarkTonic.CoreGameKit;
using UnityEngine;

public class KW_Instructions : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 130), "This scene has 6 waves of various settings. Left / right arrow keys and mouse click to fire. Feel free to tweak the settings. Wave 1 will repeat until you have 1000 score. " +
			"Wave 3 will be skipped if you have 3000 score or more. Notice the music changing in wave 3, and the 'prefab pool' in wave 7 which allows for spawning of mutiple different prefabs from the same spawner wave. " +
			"Wave 7 repeats forever and changes the items in its prefab pool depending on your score! " +
			"We have included a KillableListenerSubclass as an example which listens for events on the Player prefab. It's in the Main Camera prefab. Also note that the player gets 5000 Experience Points for completing each of the first two levels. "
			+ "Score and the other World Variables displayed onscreen are using Legacy Unity GUI. For an example of Unity 4.6 uGUI, install the optional package. "
		    + "For support, check the readme file for links! Happy gaming!");
 		  
		var scoreVar = WorldVariableTracker.GetWorldVariable("Score");
		var experienceVar = WorldVariableTracker.GetWorldVariable("Experience Points");
		var healthVar = WorldVariableTracker.GetWorldVariable("Health");
		var livesVar = WorldVariableTracker.GetWorldVariable("Lives");
		
		if (scoreVar != null) {
			GUI.Label(new Rect(10, 150, 100, 20), "Score: " + scoreVar.CurrentIntValue);
		}
		if (experienceVar != null) {
			GUI.Label(new Rect(10, 170, 100, 20), "Exp: " + experienceVar.CurrentIntValue);
		}
		if (healthVar != null) {
			GUI.Label(new Rect(10, 190, 100, 20), "Health: " + healthVar.CurrentIntValue);
		}
		if (livesVar != null) {
			GUI.Label(new Rect(10, 210, 100, 20), "lives: " + livesVar.CurrentIntValue);
		} 
	}
}
