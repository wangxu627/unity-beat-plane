using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEngine;

public class KillableListenerSubclass : KillableListener {
	// if you need more than one Listener for of each type (KillableListener etc), create subclasses like this, inheriting from KillableListener
	public override void Despawning(TriggeredSpawner.EventType eType) {
		base.Despawning(eType);
		
		// your code here.
		Debug.Log("KillableListenerSubclass (on MainCamera): Played died! Take some action");
	}
	
	public override void TakingDamage(int pointsDamage, Killable enemyHitBy) {
		base.TakingDamage(pointsDamage, enemyHitBy);
		
		// your code here.
	}

	public override void DamagePrefabSpawned(Transform damagePrefab) {
		base.DamagePrefabSpawned(damagePrefab);
		
		// your code here.
	}
	
	public override void DamagePrefabFailedToSpawn(Transform damagePrefab) {
		base.DamagePrefabFailedToSpawn(damagePrefab);
		
		// your code here.  
	}
	
	public override void DeathPrefabSpawned(Transform deathPrefab) {
		base.DeathPrefabSpawned(deathPrefab);
		
		// your code here.
		Debug.Log("Death prefab spawned for " + this.sourceKillableName);
	}
	
	public override void DeathPrefabFailedToSpawn(Transform deathPrefab) {
		base.DeathPrefabFailedToSpawn(deathPrefab);
		
		// your code here.  
	}

	public override void ModifyingDeathWorldVariables(List<WorldVariableModifier> variableModifiers) {
		base.ModifyingDeathWorldVariables(variableModifiers);
		
		// your code here.
		Debug.Log("Modifying world variations for " + this.sourceKillableName + " destruction");
	}
}
