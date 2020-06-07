using UnityEngine;
using System.Collections;

public class KW_Instructions4 : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 90), "This scene has triggered spawners of different types. Left / right arrow keys and mouse click to fire. " +
			"When you fire, all enemies within range will attack in your direction, courtesy of the custom event 'PlayerAttack' fired by the player script and Triggered Spawners on the enemies that receive that event.");
		
	}
}
