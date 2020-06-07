using UnityEngine;
using System.Collections;

public class KW_Fighter1 : MonoBehaviour {
	public bool isStrafeLeft;

	private Transform trans;
	private bool strafeFinished;
	private float distToStrafe;
	private float startX; 
	
	void Awake() {
		AwakeOrSpawned();
	}
	
	void OnSpawned() { // used by Core GameKit Pooling & also Pool Manager Pooling!
		AwakeOrSpawned();
	}

	private void AwakeOrSpawned() {
		this.useGUILayout = false;
		this.trans = this.transform;
		this.strafeFinished = false;
		this.startX = this.trans.position.x;
		this.distToStrafe = Random.Range(40, 80);
	}
	
	// Update is called once per frame
	void Update () {
		var pos = this.trans.position;
		
		if (!this.strafeFinished) {
			if (isStrafeLeft) {
				pos.x -= 100 * Time.deltaTime;
			} else {
				pos.x += 100 * Time.deltaTime;
			}

			pos.z += 30f * Time.deltaTime;
			
			if (Mathf.Abs(pos.x - this.startX) > distToStrafe) {
				this.strafeFinished = true;
			}
		} else {
			pos.z -= 70 * Time.deltaTime;
		}

		this.trans.position = pos; 
		
	}
}
