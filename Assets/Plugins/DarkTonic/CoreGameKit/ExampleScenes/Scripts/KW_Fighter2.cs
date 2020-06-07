using UnityEngine;
using System.Collections;

public class KW_Fighter2 : MonoBehaviour {
	private Transform trans;
	private float zMovement;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
		this.zMovement = 30.5f;
	}
	
	// Update is called once per frame
	void Update () {
		this.trans.Translate(Vector3.forward * this.zMovement * Time.deltaTime);
	}
}
