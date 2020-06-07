using UnityEngine;
using System.Collections;

public class KW_Projectile : MonoBehaviour {
	private Transform trans;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		var moveAmt = 140 * Time.deltaTime;
		
		var pos = this.trans.position;
		pos.z += moveAmt;
		this.trans.position = pos;
	}
}
