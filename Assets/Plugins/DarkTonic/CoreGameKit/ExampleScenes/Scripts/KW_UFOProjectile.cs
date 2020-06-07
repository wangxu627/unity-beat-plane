using UnityEngine;
using System.Collections;

public class KW_UFOProjectile : MonoBehaviour {
	private Transform trans;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		this.trans.Rotate(Vector3.down * 300 * Time.deltaTime);
		
		var pos = this.trans.position;
		pos.z -= 120 * Time.deltaTime;
		this.trans.position = pos;
	}
}
