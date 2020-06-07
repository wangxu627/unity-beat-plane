using UnityEngine;
using System.Collections;

public class KW_Shot3 : MonoBehaviour {
	private Transform trans;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		this.trans.Translate(Vector3.forward * 70 * Time.deltaTime);
	}
}
