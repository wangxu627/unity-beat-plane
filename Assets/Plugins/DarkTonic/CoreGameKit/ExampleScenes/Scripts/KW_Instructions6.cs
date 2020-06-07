using UnityEngine;
using System.Collections;

public class KW_Instructions6 : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 120), "This scene has 6 waves of various settings. Left / right arrow keys and mouse click to fire. Feel free to tweak the settings. Wave 1 will repeat until you have 1000 score. " +
			"Wave 3 will be skipped if you have 3000 score or more. Notice the music changing in wave 3, and the 'prefab pool' in wave 7 which allows for spawning of mutiple different prefabs from the same spawner wave. " +
			"Wave 7 repeats forever and changes the items in its prefab pool depending on your score! " +
			"I have included a KillableListenerSubclass as an example which listens for events on the Player prefab. It's in the Main Camera prefab. Also note that the player gets 5000 Experience Points for completing each of the first two levels. "
			+ "Score and the other World Variables displayed onscreen are done with Unity 4.6 UI. "
		    + "For support, check the readme file for links! Happy gaming!");
	}
}
