using UnityEngine;
using System.Collections;

public class KW_UFO : MonoBehaviour {
	public float speedMultiplier = 1f;
	
	private Transform trans;
	private float speed;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
		this.speed = 7 * Time.deltaTime * this.speedMultiplier;
	}
	
	// Update is called once per frame
	void Update () {
		this.trans.Translate(Vector3.forward * this.speed);
	}
}
