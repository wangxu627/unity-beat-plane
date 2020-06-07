using DarkTonic.CoreGameKit;
using UnityEngine;

public class KW_Player : MonoBehaviour {
	public string customEventName = ""; 
	public Texture stableShip;
	public Texture leftShip;
	public Texture rightShip;
	
	public GameObject ProjectilePrefab;
	
	private const float MOVE_SPEED = 100f;
	private Transform trans;
	private Renderer rend;
	
	// Use this for initialization
	void Awake() {
		this.useGUILayout = false;
		this.trans = this.transform;
		this.rend = this.GetComponent<Renderer>();
	}
	
	void OnBecameInvisible() {
		
	}
	
	// Update is called once per frame
	void Update () {
		var moveAmt = Input.GetAxis("Horizontal") * MOVE_SPEED * Time.deltaTime;
		
		if (moveAmt == 0) {
			this.rend.materials[0].mainTexture = stableShip;
		} else if (moveAmt > 0) {
			this.rend.materials[0].mainTexture = rightShip;
		} else {
			this.rend.materials[0].mainTexture = leftShip;
		}
		
		var pos = this.trans.position;
		pos.x += moveAmt;
		this.trans.position = pos;
		
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
			var spawnPos = this.trans.position;
			spawnPos.z += 15;
			
			if (!string.IsNullOrEmpty(customEventName) && LevelSettings.CustomEventExists(customEventName)) {
				LevelSettings.FireCustomEvent(customEventName, this.trans);
			}
			PoolBoss.SpawnOutsidePool(ProjectilePrefab.transform, spawnPos, ProjectilePrefab.transform.rotation); 
		}
	}
}
