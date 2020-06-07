using UnityEngine;
using System.Collections;

public class KW_MotherShip : MonoBehaviour {
	public float speed = 30;
	
	private Transform trans;
	private bool isStrafeLeft;
	private bool strafeFinished;
	private float distToStrafe;
	private float startX;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		var pos = this.trans.position;
		pos.z -= speed * Time.deltaTime;		

		this.trans.position = pos; 
		
	}
}
