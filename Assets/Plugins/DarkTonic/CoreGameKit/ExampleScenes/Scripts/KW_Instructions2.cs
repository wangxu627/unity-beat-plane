using UnityEngine;
using System.Collections;

public class KW_Instructions2 : MonoBehaviour {
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 760, 100), "Make sure to play this one with 'Maximize on Play' on or it will be impossible to play. " +
			"This scene has triggered spawners of different types. Left / right arrow keys and mouse click to fire. " +
			"You will see triggered projectile waves, triggered attack waves and spawners of spawners. " +
			"Notice the decreasing attack on repeat of the Triggered Spawners on the first enemy. Also notice that when destroying the motherships, all fighters it launched die with it. " +
			"The player prefab gets 2 seconds of invincibility after it's spawned each time, courtesy of the Killable component. We're not using the PlayerSpawner script in this Scene. Instead we're using the Respawn section of the Killable script on the Player. All with no code on your end!");
	}
}
