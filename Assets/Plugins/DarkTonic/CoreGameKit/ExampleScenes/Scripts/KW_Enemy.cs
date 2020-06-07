using UnityEngine;
using System.Collections;

public class KW_Enemy : MonoBehaviour {
	public float speedMultiplier = 1f;
	public bool randomHoriz = false;
	
	private Transform trans;
	private float speed;
	private float horizSpeed;
	
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
		AwakeOrSpawned();
	}
	
	void OnSpawned() {
		AwakeOrSpawned();
	}
	
	private void AwakeOrSpawned() {
		this.speed = Random.Range(-8, -7) * Time.deltaTime * this.speedMultiplier;
		if (this.randomHoriz) {
			this.horizSpeed = Random.Range(-4, 4) * Time.deltaTime * this.speedMultiplier;
		}
	}
	
	// Update is called once per frame
	void Update () {
		var pos = this.trans.position;
		pos.x += this.horizSpeed;
		pos.z += speed;
		this.trans.position = pos; 
		
		this.trans.Rotate(Vector3.down * 300 * Time.deltaTime);
	}
}
