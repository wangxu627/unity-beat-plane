using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
	/// <summary>
	/// This class is used to set up Killable, used for combat objects with attack points and hit points. Also can be used for pickups such as coins and health packs.
	/// </summary>
	[AddComponentMenu("Dark Tonic/Core GameKit/Combat/Killable")]
	// ReSharper disable once CheckNamespace
	public class Killable : MonoBehaviour {
		/*! \cond PRIVATE */
		public const string DestroyedText = "Destroyed";
		public const int MaxHitPoints = 100000;
		public const int MaxAttackPoints = 100000;
		public const int MinAttackPoints = -100000;
		
		#region Members
		
		// ReSharper disable InconsistentNaming
		public TriggeredSpawner.GameOverBehavior gameOverBehavior = TriggeredSpawner.GameOverBehavior.Disable;
		public bool syncHitPointWorldVariable = false;
		public KillerInt hitPoints = new KillerInt(1, 1, MaxHitPoints);
		public KillerInt maxHitPoints = new KillerInt(MaxAttackPoints, MinAttackPoints, MaxAttackPoints);
		public KillerInt atckPoints = new KillerInt(1, MinAttackPoints, MaxAttackPoints);
		public KillableListener listener;
		public string poolBossCategoryName;

		public bool invincibilityExpanded = false;
		public bool isInvincible;
		public bool invincibleWhileChildrenKillablesExist = true;
		public bool disableCollidersWhileChildrenKillablesExist = false;
		
		public bool invincibleOnSpawn = false;
		public KillerFloat invincibleTimeSpawn = new KillerFloat(2f, 0f, float.MaxValue);
		
		public bool invincibleWhenDamaged = false;
		public KillerFloat invincibleDamageTime = new KillerFloat(1f, 0, float.MaxValue);
		
		public SpawnSource invinceHitPrefabSource = SpawnSource.None;
		public int invinceHitPrefabPoolIndex = 0;
		public string invinceHitPrefabPoolName = null;
		public Transform invinceHitPrefabSpecific;
		public string invinceHitPrefabCategoryName;
		public bool invinceHitPrefabRandomizeXRotation = false;
		public bool invinceHitPrefabRandomizeYRotation = false;
		public bool invinceHitPrefabRandomizeZRotation = false;
		
		public bool enableLogging = false;
		public bool filtersExpanded = true;
		
		public bool ignoreKillablesSpawnedByMe = true;
		public bool useLayerFilter = false;
		public bool useTagFilter = false;
		public bool showVisibilitySettings = true;
		
		public bool despawnWhenOffscreen = false;
		public bool despawnOnClick = false;
		public bool despawnOnMouseClick = false;
		public bool despawnIfNotVisible = false;
		public KillerFloat despawnIfNotVisibleForSec = new KillerFloat(5f, .1f, float.MaxValue);
		
		public SpawnSource vanishPrefabSource = SpawnSource.None;
		public int vanishPrefabPoolIndex = 0;
		public string vanishPrefabPoolName = null;
		public Transform vanishPrefabSpecific;
		public string vanishPrefabCategoryName;
		public bool vanishPrefabRandomizeXRotation = false;
		public bool vanishPrefabRandomizeYRotation = false;
		public bool vanishPrefabRandomizeZRotation = false;
		
		public bool ignoreOffscreenHits = false;
		public List<string> matchingTags = new List<string>() { "Untagged" };
		public List<int> matchingLayers = new List<int>() { 0 };
		public DespawnMode despawnMode = DespawnMode.ZeroHitPoints;
		public bool includeNonKillables = false;
		
		public bool damageFireEvents = false;
		public List<CGKCustomEventToFire> damageCustomEvents = new List<CGKCustomEventToFire>();

        public bool dealDamageFireEvents = false;
        public List<CGKCustomEventToFire> dealDamageCustomEvents = new List<CGKCustomEventToFire>();

        public bool deathFireEvents = false;
		public List<CGKCustomEventToFire> deathCustomEvents = new List<CGKCustomEventToFire>();
		
		// death player stat mods
		public bool despawnStatModifiersExpanded = false;
		public WorldVariableCollection playerStatDespawnModifiers = new WorldVariableCollection();
		public List<WorldVariableCollection> alternateModifiers = new List<WorldVariableCollection>();
		
		// deal damage prefab settings
		public bool dealDamagePrefabExpanded = false;
		public SpawnSource dealDamagePrefabSource = SpawnSource.None;
		public int dealDamagePrefabPoolIndex = 0;
		public string dealDamagePrefabPoolName = null;
		public Transform dealDamagePrefabSpecific;
		public string dealDamagePrefabCategoryName;
		public bool dealDamagePrefabOnDeathHit = false;
		public bool dealDamagePrefabRandomizeXRotation = false;
		public bool dealDamagePrefabRandomizeYRotation = false;
		public bool dealDamagePrefabRandomizeZRotation = false;
		
		// damage prefab settings
		public bool damagePrefabExpanded = false;
		public SpawnSource damagePrefabSource = SpawnSource.None;
		public int damagePrefabPoolIndex = 0;
		public string damagePrefabPoolName = null;
		public Transform damagePrefabSpecific;
		public string damagePrefabCategoryName;
		public DamagePrefabSpawnMode damagePrefabSpawnMode = DamagePrefabSpawnMode.None;
		public KillerInt damagePrefabSpawnQuantity = new KillerInt(1, 1, 100);
		public KillerInt damageGroupsize = new KillerInt(1, 1, 100000);
		public Vector3 damagePrefabOffset = Vector3.zero;
		public Vector3 damagePrefabIncrementalOffset = Vector3.zero;
		public bool damagePrefabRandomizeXRotation = false;
		public bool damagePrefabRandomizeYRotation = false;
		public bool damagePrefabRandomizeZRotation = false;
		public bool despawnStatDamageModifiersExpanded = false;
		public bool damagePrefabOnDeathHit = false;
		public WorldVariableCollection playerStatDamageModifiers = new WorldVariableCollection();
		
		public bool damageKnockBackExpanded = false;
		public bool sendDamageKnockback = false;
		public bool receiveKnockbackWhenDamaged = false;
		public bool receiveKnockbackWhenInvince = false;
		public KillerFloat damageKnockUpMeters = new KillerFloat(10f, float.MinValue, float.MaxValue);
		public KillerFloat damageKnockBackFactor = new KillerFloat(3f, float.MinValue, float.MaxValue);
		
		// death prefab settings
		public WaveSpecifics.SpawnOrigin deathPrefabSource = WaveSpecifics.SpawnOrigin.Specific;
		public int deathPrefabPoolIndex = 0;
		public string deathPrefabPoolName = null;
		public bool deathPrefabSettingsExpanded = false;
		public Transform deathPrefabSpecific;
		public string deathPrefabCategoryName;
		public bool deathPrefabKeepSameParent = true;
		public KillerInt deathPrefabSpawnPercent = new KillerInt(100, 0, 100);
		public DeathPrefabSpawnLocation deathPrefabSpawnLocation = DeathPrefabSpawnLocation.DeathPosition;
		public KillerInt deathPrefabQty = new KillerInt(1, 0, 100);
		public Vector3 deathPrefabOffset = Vector3.zero;
		public Vector3 deathPrefabIncrementalOffset = Vector3.zero;
		public RotationMode rotationMode = RotationMode.UseDeathPrefabRotation;
		public bool deathPrefabRandomizeXRotation = false;
		public bool deathPrefabRandomizeYRotation = false;
		public bool deathPrefabRandomizeZRotation = false;
		public bool deathPrefabKeepVelocity = true;
		public Vector3 deathPrefabCustomRotation = Vector3.zero;
		public KillerFloat deathDelay = new KillerFloat(0, 0, 100);
		
		public SpawnerDestroyedBehavior spawnerDestroyedAction = SpawnerDestroyedBehavior.DoNothing;
		
		public Killable parentKillableForParentDestroyed = null;
		public SpawnerDestroyedBehavior parentDestroyedAction = SpawnerDestroyedBehavior.DoNothing;
		public DeathDespawnBehavior deathDespawnBehavior = DeathDespawnBehavior.ReturnToPool;
		
		public bool timerDeathEnabled;
		public KillerFloat timerDeathSeconds = new KillerFloat(1f, 0.1f, float.MaxValue);
		public SpawnerDestroyedBehavior timeUpAction = SpawnerDestroyedBehavior.Die;
		
		public bool distanceDeathEnabled = false;
		public KillerFloat tooFarDistance = new KillerFloat(1f, 0.1f, float.MaxValue);
		public SpawnerDestroyedBehavior distanceDeathAction = SpawnerDestroyedBehavior.Die;
		
		public int currentHitPoints;
		
		public bool isVisible;
		
		public bool showRespawnSettings = false;
		public RespawnType respawnType = RespawnType.None;
		public int timesToRespawn = 1;
		public KillerFloat respawnDelay = new KillerFloat(0, 0, 100);
		public bool respawnFireEvents = false;
		public List<CGKCustomEventToFire> respawnCustomEvents = new List<CGKCustomEventToFire>();
		
		// ReSharper restore InconsistentNaming
		/*! \endcond */
		
		private Vector3 _respawnLocation = Vector3.zero;

        private VisualizationMarker _visualizationMarker;
        private int _timesRespawned;
		private Vector3 _spawnPoint;
		private GameObject _spawnedFromObject;
		private int? _spawnedFromGOInstanceId;
		private WavePrefabPool _deathPrefabWavePool;
		private Transform _trans;
		private GameObject _go;
		private int? _instanceId;
		private CharacterController _charCtrl;
		private Rigidbody _body;
		private Killable _parentKillable;
		private Collider _collider;
		private bool _willSkipDeathDelay;
		private string _deathScenarioName;
		
		private Rigidbody2D _body2D;
		private Collider2D _collider2D;
		
		private int _damageTaken;
		private int _damagePrefabsSpawned;
		private WavePrefabPool _damagePrefabWavePool;
		private WavePrefabPool _dealDamagePrefabWavePool;
		private WavePrefabPool _vanishPrefabWavePool;
		private WavePrefabPool _invinceHitPrefabWavePool;
		private bool _becameVisible;
		private float _spawnTime;
		private bool _isDespawning;
		private bool _isTemporarilyInvincible;
		private bool _spawnerSet;
		private bool _spawnLocationSet;
		private bool _waitingToDestroy;
		private bool _deathDelayGoing;
		private readonly List<Killable> _childrenToDestroy = new List<Killable>();
		
		private readonly List<Killable> _childKillables = new List<Killable>();
		
		#endregion
		
		
		#region enums
		/*! \cond PRIVATE */
		public enum DeathPrefabSpawnLocation {
			DeathPosition,
			RespawnPosition
		}

		public enum DeathDespawnBehavior {
			ReturnToPool,
			Disable
		}
		
		public enum RespawnType {
			None = 0,
			Infinite = 1,
			SetNumber = 2
		}
		
		public enum SpawnerDestroyedBehavior {
			DoNothing,
			Despawn,
			Die
		}
		
		public enum SpawnSource {
			None,
			Specific,
			PrefabPool
		}
		
		public enum DamagePrefabSpawnMode {
			None,
			PerHit,
			PerHitPointLost,
			PerGroupHitPointsLost
		}
		
		public enum RotationMode {
			CustomRotation,
			InheritExistingRotation,
			UseDeathPrefabRotation
		}
		
		public enum DespawnMode {
			None = -1,
			ZeroHitPoints = 0,
			LostAnyHitPoints = 1,
			CollisionOrTrigger = 2
		}
		
		/*! \endcond */
		#endregion
		
		#region MonoBehavior events and associated virtuals
		
		// ReSharper disable once UnusedMember.Local
		private void Awake() {
			_timesRespawned = 0;
			ResetSpawnerInfo();
		}
		
		// ReSharper disable once UnusedMember.Local
		private void Start() {
			SpawnedOrAwake(false);
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnSpawned() {
			// used by Core GameKit Pooling & also Pool Manager Pooling!
			SpawnedOrAwake();
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnDespawned() {
			_spawnerSet = false;
			
			// reset velocity
			ResetVelocity();
			ResetSpawnerInfo();
			
			// add code here to fire when despawned
			Despawned();
		}
		
		/// <summary>
		/// This method is automatically called just before the Killable is Despawned
		/// </summary>
		protected virtual void Despawned() {
			// add code to subclass if needing functionality here		
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnClick() {
			_OnClick();
		}
		
		/// <summary>
		/// This method gets called when a Killable is clicked
		/// </summary>
		protected virtual void _OnClick() {
			if (despawnOnClick) {
				DestroyKillable();
			}
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnMouseDown() {
			_OnMouseDown();
		}
		
		/// <summary>
		/// This method gets called when a Killable has a mouse button clicked down.
		/// </summary>
		protected virtual void _OnMouseDown() {
			if (despawnOnMouseClick) {
				DestroyKillable();
			}
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnBecameVisible() {
			BecameVisible();
		}
		
		/// <summary>
		/// This gets called when the Killable becomes visible.
		/// </summary>
		public virtual void BecameVisible() {
			if (isVisible) {
				return; // to fix Unity error.
			}
			
			isVisible = true;
			_becameVisible = true;
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnBecameInvisible() {
			BecameInvisible();
		}
		
		/// <summary>
		/// This gets called when the Killable becomes invisible
		/// </summary>
		public virtual void BecameInvisible() {
			isVisible = false;
			
			if (despawnWhenOffscreen) {
				Despawn(TriggeredSpawner.EventType.Invisible);
			}
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnCollisionEnter2D(Collision2D coll) {
            if (IsVisualizationClone) {
                return;
            }

            CollisionEnter2D(coll);
		}
		
		/// <summary>
		/// This gets called when the Killable has a 2D collision
		/// </summary>
		/// <param name="collision">The collision object.</param>
		public virtual void CollisionEnter2D(Collision2D collision) {
			var othGo = collision.gameObject;
			
			if (!IsValidHit(othGo.layer, othGo.tag)) {
				return;
			}
			
			var enemy = GetOtherKillable(othGo);
			
			CheckForAttackPoints(enemy, othGo);
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnTriggerEnter2D(Collider2D other) {
            if (IsVisualizationClone) {
                return;
            }

            TriggerEnter2D(other);
		}
		
		/// <summary>
		/// This gets called when the Killable has a 2D "trigger" collision
		/// </summary>
		/// <param name="other">The collider2D object.</param>
		public virtual void TriggerEnter2D(Collider2D other) {
			var othGo = other.gameObject;
			
			if (!IsValidHit(othGo.layer, othGo.tag)) {
				return;
			}
			
			var enemy = GetOtherKillable(othGo);
			
			CheckForAttackPoints(enemy, othGo);
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnCollisionEnter(Collision collision) {
            if (IsVisualizationClone) {
                return;
            }

            CollisionEnter(collision);
		}
		
		/// <summary>
		/// This gets called when the Killable has a collision
		/// </summary>
		/// <param name="collision">The collision object.</param>
		public virtual void CollisionEnter(Collision collision) {
			var othGo = collision.gameObject;
			
			if (!IsValidHit(othGo.layer, othGo.tag)) {
				return;
			}
			
			var enemy = GetOtherKillable(othGo);
			
			CheckForAttackPoints(enemy, othGo);
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnTriggerEnter(Collider other) {
		    if (IsVisualizationClone) {
		        return;
		    }

            TriggerEnter(other);
		}
		
		/// <summary>
		/// This gets called when the Killable has a "trigger" collision
		/// </summary>
		/// <param name="other">The collider object</param>
		public virtual void TriggerEnter(Collider other) {
			if (!IsValidHit(other.gameObject.layer, other.gameObject.tag)) {
				return;
			}
			
			var enemy = GetOtherKillable(other.gameObject);
			
			CheckForAttackPoints(enemy, other.gameObject);
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (IsVisualizationClone) {
                return;
            }

            ControllerColliderHit(hit.gameObject);
		}
		
		/// <summary>
		/// This gets called when the Killable has a collision from a CharacterController. Do not call this.
		/// </summary>
		/// <param name="hit">The game object being hit.</param>
		/// <param name="calledFromOtherKillable">The object calling this method</param>
		public virtual void ControllerColliderHit(GameObject hit, bool calledFromOtherKillable = false) {
			if (calledFromOtherKillable && CharController != null) {
				// we don't need to be called from a Char Controller if we are one. Abort to exit potential endless loop.
				return;
			}
			
			var enemy = GetOtherKillable(hit);
			
			if (enemy != null && !calledFromOtherKillable) {
				// for Character Controllers, the hit object will not register a hit, so we call it manually.
				enemy.ControllerColliderHit(GameObj, true);
			}
			
			if (!IsValidHit(hit.layer, hit.tag)) {
				return;
			}
			
			CheckForAttackPoints(enemy, hit);
		}
		 
		// ReSharper disable once UnusedMember.Local
		protected virtual void LateUpdate() {
			if (_waitingToDestroy) {
				DestroyRecordedChildren();
				
				if (deathDelay.Value > 0f && !_willSkipDeathDelay) {
					if (!_deathDelayGoing) {
						StartCoroutine(WaitThenDestroy(_deathScenarioName));
					}
				} else {
					if (!PerformDeath(_deathScenarioName)) {
						_waitingToDestroy = false; // stop endless death prefab spawning loop 
					};
				}
				
				return;
			}
			
			switch (spawnerDestroyedAction) {
				case SpawnerDestroyedBehavior.DoNothing:
					break;
				case SpawnerDestroyedBehavior.Despawn:
					if (_spawnerSet && SpawnUtility.IsDespawnedOrDestroyed(_spawnedFromObject)) {
						SpawnerDestroyed();
						Despawn(TriggeredSpawner.EventType.SpawnerDestroyed);
					}
					break;
				case SpawnerDestroyedBehavior.Die:
					if (_spawnerSet && SpawnUtility.IsDespawnedOrDestroyed(_spawnedFromObject)) {
						SpawnerDestroyed();
						DestroyKillable();
					}
					break;
			}
			
			// check for distance-based death
			if (distanceDeathEnabled && Vector3.Distance(_spawnPoint, Trans.position) > tooFarDistance.Value) {
				switch (distanceDeathAction) {
				case SpawnerDestroyedBehavior.DoNothing:
					break;
				case SpawnerDestroyedBehavior.Despawn:
					Despawn(TriggeredSpawner.EventType.DistanceDeath);
					break;
				case SpawnerDestroyedBehavior.Die:
					DestroyKillable();
					break;
				}
				
				return;
			}
			
			// check for death timer.
			if (timerDeathEnabled && Time.time - _spawnTime > timerDeathSeconds.Value) {
				switch (timeUpAction) {
				case SpawnerDestroyedBehavior.DoNothing:
					break;
				case SpawnerDestroyedBehavior.Despawn:
					Despawn(TriggeredSpawner.EventType.DeathTimer);
					break;
				case SpawnerDestroyedBehavior.Die:
					DestroyKillable();
					break;
				}
				
				return;
			}
			
			// check for "not visible too long"
			if (!despawnIfNotVisible || _becameVisible) {
				return;
			}
			
			if (Time.time - _spawnTime > despawnIfNotVisibleForSec.Value) {
				Despawn(TriggeredSpawner.EventType.Invisible);
			}
		}
		
		#endregion
		
		#region Public Methods
		
		/// <summary>
		/// Call this method to add attack points to the Killable.
		/// </summary>
		/// <param name="pointsToAdd">The number of attack points to add.</param>
		public void AddAttackPoints(int pointsToAdd) {
			atckPoints.Value += pointsToAdd;
			if (atckPoints.Value < 0) {
				atckPoints.Value = 0;
			}
		}
		
		/// <summary>
		/// Call this method to add hit points to the Killable.
		/// </summary>
		/// <param name="pointsToAdd">The number of hit points to add to "current hit points".</param>
		public void AddHitPoints(int pointsToAdd) {
			hitPoints.Value += pointsToAdd;
			if (hitPoints.Value < 0) {
				hitPoints.Value = 0;
			}
			
			currentHitPoints += pointsToAdd;
			if (currentHitPoints < 0) {
				currentHitPoints = 0;
			}
		}

        /// <summary>
        /// Gets called internally, you should never need to call this.
        /// </summary>
	    public void FireRespawnEvents() {
	        if (!respawnFireEvents) {
	            return;
	        }

	        // ReSharper disable once ForCanBeConvertedToForeach
	        for (var i = 0; i < respawnCustomEvents.Count; i++) {
	            var anEvent = respawnCustomEvents[i].CustomEventName;

	            LevelSettings.FireCustomEventIfValid(anEvent, Trans);
	        }
	    }

	    /// <summary>
        /// Use this method to start the death timer. Timer will start from "now".
        /// </summary>
        public void StartDeathTimer() {
			_spawnTime = Time.time;
			timerDeathEnabled = true;
		}
		
		/// <summary>
		/// Use this method to stop the death timer.
		/// </summary>
		public void StopDeathTimer() {
			_spawnTime = -1f;
			timerDeathEnabled = false;
		}
		
		/// <summary>
		/// Call this method to make your Killable invincible for X seconds.
		/// </summary>
		/// <param name="seconds">Number of seconds to make your Killable invincible.</param>
		public void TemporaryInvincibility(float seconds) {
			if (_isTemporarilyInvincible) {
				// already invincible.
				return;
			}
			StartCoroutine(SetSpawnInvincibleForSeconds(seconds));
		}
		
		/*! \cond PRIVATE */
		public bool IsUsingPrefabPool(Transform poolTrans) {
			var poolName = poolTrans.name;
			
			if (damagePrefabSource == SpawnSource.PrefabPool && damagePrefabPoolName == poolName) {
				return true;
			}
			
			return false;
		}
		
		public void RegisterChildKillable(Killable kill) {
			if (_childKillables.Contains(kill)) {
				return;
			}
			
			_childKillables.Add(kill);
			
			if (invincibleWhileChildrenKillablesExist && disableCollidersWhileChildrenKillablesExist) {
				DisableColliders();
			}
			
			// Diagnostic code to uncomment if things are going wrong.
			//Debug.Log("ADD - children of '" + name + "': " + childKillables.Count);
		}
		
		public void RecordSpawner(GameObject spawnerObject) {
			_spawnedFromObject = spawnerObject;
			_spawnerSet = true;
		}
		
		public virtual void UnregisterChildKillable(Killable kill) {
			_childKillables.Remove(kill);
			
			deathDespawnBehavior = DeathDespawnBehavior.Disable;
			
			if (_childKillables.Count == 0 && invincibleWhileChildrenKillablesExist &&
			    disableCollidersWhileChildrenKillablesExist) {
				EnableColliders();
			}
			
			// Diagnostic code to uncomment if things are going wrong.
			//Debug.Log("REMOVE - children of '" + name + "': " + childKillables.Count);
		}
		
		public void RecordChildToDie(Killable kilChild) {
			if (_childrenToDestroy.Contains(kilChild)) {
				return;
			}
			
			_childrenToDestroy.Add(kilChild);
		}
		/*! \endcond */
		
		#endregion
		
		#region Helper Methods
		
		private void CheckForValidVariables() {
			// examine all KillerInts
			hitPoints.LogIfInvalid(Trans, "Killable Start Hit Points");
			maxHitPoints.LogIfInvalid(Trans, "Killable Max Hit Points");
			atckPoints.LogIfInvalid(Trans, "Killable Start Attack Points");

			if (damagePrefabSpawnMode != DamagePrefabSpawnMode.None) {
				damagePrefabSpawnQuantity.LogIfInvalid(Trans, "Killable Damage Prefab Spawn Quantity");

				if (damagePrefabSpawnMode == DamagePrefabSpawnMode.PerGroupHitPointsLost) {
					damageGroupsize.LogIfInvalid(Trans, "Killable Group H.P. Amount");
				}
			}

			deathPrefabSpawnPercent.LogIfInvalid(Trans, "Killable Spawn % Chance");
			deathPrefabQty.LogIfInvalid(Trans, "Killable Death Prefab Spawn Quantity");
			deathDelay.LogIfInvalid(Trans, "Killable Death Delay");
			respawnDelay.LogIfInvalid(Trans, "Killable Respawn Delay");
			damageKnockUpMeters.LogIfInvalid(Trans, "Killable Knock Up Force");
			damageKnockBackFactor.LogIfInvalid(Trans, "Killable Knock Back Force");
			
			// examine all KillerFloats
			despawnIfNotVisibleForSec.LogIfInvalid(Trans, "Killable Not Visible Max Time");
			if (timerDeathEnabled) {
				timerDeathSeconds.LogIfInvalid(Trans, "Killable Timer Death Seconds");
			}
			
			if (invincibleOnSpawn) {
				invincibleTimeSpawn.LogIfInvalid(Trans, "Killable Invincibility Time (sec)");
			}
			
			if (invincibleWhenDamaged) {
				invincibleDamageTime.LogIfInvalid(Trans, "Killable Invincible After Damage Time");
			}
			
			// check damage mod scenarios  
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < playerStatDamageModifiers.statMods.Count; i++) {
				var mod = playerStatDamageModifiers.statMods[i];
				ValidateWorldVariableModifier(mod);
			}
			
			// check mod scenarios
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < playerStatDespawnModifiers.statMods.Count; i++) {
				var mod = playerStatDespawnModifiers.statMods[i];
				ValidateWorldVariableModifier(mod);
			}
			
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var c = 0; c < alternateModifiers.Count; c++) {
				var alt = alternateModifiers[c];
				// ReSharper disable ForCanBeConvertedToForeach
				for (var i = 0; i < alt.statMods.Count; i++) {
					// ReSharper restore ForCanBeConvertedToForeach
					var mod = alt.statMods[i];
					ValidateWorldVariableModifier(mod);
				}
			}
		}
		
		private void CheckForAttackPoints(Killable enemy, GameObject goHit) {
			var attackPoints = 0;
			
			if (enemy == null) {
				if (!includeNonKillables) {
					LogIfEnabled("Not taking any damage because you've collided with non-Killable object '" + goHit.name +
					             "'.");
					return;
				}
			} else {
				if (ignoreKillablesSpawnedByMe) {
					if (enemy.SpawnedFromObjectId == KillableId) {
						LogIfEnabled("Not taking any damage because you've collided with a Killable named '" +
						             goHit.name +
						             "' spawned by this Killable.");
						return;
					}
				}
				
				attackPoints = enemy.atckPoints.Value;
			}
			
			TakeDamage(attackPoints, enemy);
		}
		
		private void DestroyRecordedChildren() {
			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < _childrenToDestroy.Count; i++) {
				var aDeadChild = _childrenToDestroy[i];
				
				switch (aDeadChild.parentDestroyedAction) {
				case SpawnerDestroyedBehavior.DoNothing:
					break;
				case SpawnerDestroyedBehavior.Despawn:
					aDeadChild.Despawn(TriggeredSpawner.EventType.ParentDestroyed);
					break;
				case SpawnerDestroyedBehavior.Die:
					aDeadChild.DestroyKillable();
					break;
				}
			}
		}
		
		private void DisableColliders() {
			if (Colidr != null) {
				Colidr.enabled = false;
			}
			
			if (Colidr2D != null) {
				Colidr2D.enabled = false;
			}
		}
		
		private void EnableColliders() {
			if (Colidr != null) {
				Colidr.enabled = true;
			}
			
			if (Colidr2D != null) {
				Colidr2D.enabled = true;
			}
		}
		
		private static Killable GetOtherKillable(GameObject other) {
			var enemy = other.GetComponent<Killable>();
			if (enemy != null) {
				return enemy;
			}
			var childKill = other.GetComponent<KillableChildCollision>();
			if (childKill != null) {
				enemy = childKill.killable;
			}
			
			return enemy;
		}
		
		private bool IsValidHit(int hitLayer, string hitTag) {
			if (PoolBoss.Instance.IsPhotonNetworked) {
				if (!IsMyMultiplayerPrefab) { // only take damage on the owner's client
					return false;
				}
			}
			
			if (GameIsOverForKillable) {
				LogIfEnabled("Invalid hit because game is over for Killable. Modify Game Over Behavior to get around ");
				return false;
			}
			
			// check filters for matches if turned on
			if (useLayerFilter && !matchingLayers.Contains(hitLayer)) {
				LogIfEnabled("Invalid hit because layer of other object ('" + LayerMask.LayerToName(hitLayer) + "') is not in the Layer Filter.");
				return false;
			}
			
			if (useTagFilter && !matchingTags.Contains(hitTag)) {
				LogIfEnabled("Invalid hit because tag of other object ('" + hitTag + "') is not in the Tag Filter.");
				return false;
			}
			
			// ReSharper disable once InvertIf
			if (!isVisible && ignoreOffscreenHits) {
				LogIfEnabled(
					"Invalid hit because Killable is set to ignore offscreen hits and is invisible or offscreen right now. Consider using the KillableChildVisibility script if the Renderer is in a child object.");
				return false;
			}
			
			return true;
		}
		
		private void LogIfEnabled(string msg) {
			if (!enableLogging) {
				return;
			}
			
			Debug.Log("Killable '" + Trans.name + "' log: " + msg);
		}
		
		/// <summary>
		/// This method is used to change World Variable values during damage or destruction. This can be overridden in a subclass to do other things.
		/// </summary>
		/// <param name="modCollection">The collection of World Variable modifications.</param>
		/// <param name="isDamage">True if from damage, false otherwise.</param>
		/// <param name="fireAnyway">Optional switch to ignore result of "ShouldFire" methods</param>
		public virtual void ModifyWorldVariables(WorldVariableCollection modCollection, bool isDamage, bool fireAnyway = false) {
			if (modCollection.statMods.Count > 0) {
				if (isDamage) {
					if (!fireAnyway && !ShouldFireDamageVariableModifiers()) {
						return;
					}
					
					if (listener != null) {
						ModifyDamageWorldVariables(modCollection.statMods);
					}
				} else {
					if (!fireAnyway && !ShouldFireDeathVariableModifiers()) {
						return;
					}
					
					if (listener != null) {
						ModifyDeathWorldVariables(modCollection.statMods);
					}
				}
			}
			
			foreach (var modifier in modCollection.statMods) {
				WorldVariableTracker.ModifyPlayerStat(modifier, Trans);
			}
		}
		
		/*! \cond PRIVATE */
		protected bool PerformDeath(string scenarioName, bool skipDespawn = false) {
			scenarioName = DetermineScenario(scenarioName);
			
			if (listener != null) {
				listener.DestroyingKillable(this);
				scenarioName = listener.DeterminingScenario(this, scenarioName);
			}
			
			if (deathPrefabSource == WaveSpecifics.SpawnOrigin.Specific && deathPrefabSpecific == null) {
				// no death prefab.
			} else {
				SpawnDeathPrefabs();
			}
			
			if (deathFireEvents) {
				// ReSharper disable once ForCanBeConvertedToForeach
				for (var i = 0; i < deathCustomEvents.Count; i++) {
					var anEvent = deathCustomEvents[i].CustomEventName;
					
					LevelSettings.FireCustomEventIfValid(anEvent, Trans);
				}
			}
			
			// modify world variables
			if (scenarioName == DestroyedText) {
				ModifyWorldVariables(playerStatDespawnModifiers, false);
			} else {
				var scenario = alternateModifiers.Find(delegate(WorldVariableCollection obj) {
					return
						obj.scenarioName ==
							scenarioName;
				});
				
				if (scenario == null) {
					LevelSettings.LogIfNew("Scenario: '" + scenarioName + "' not found in Killable '" + Trans.name +
					                       "'. No World Variables modified by destruction.");
				} else {
					ModifyWorldVariables(scenario, false);
				}
			}

			var isSuccess = true;

			if (!skipDespawn) {
				isSuccess = Despawn(TriggeredSpawner.EventType.LostHitPoints);
			}

			return isSuccess;
		}
		/*! \endcond */

		private void ResetVelocity() {
			if (!IsGravBody) {
				return;
			}
			
			if (Body != null) {
				Body.velocity = Vector3.zero;
				Body.angularVelocity = Vector3.zero;
			}
			
			if (Body2D == null || Body2D.isKinematic) {
				return;
			}
			Body2D.velocity = Vector3.zero;
			Body2D.angularVelocity = 0f;
		}
		
		private void ResetSpawnerInfo() {
			_spawnedFromObject = null;
			_spawnerSet = false;
			_spawnedFromGOInstanceId = null;
		}
		
		private bool SpawnDamagePrefabsIfPerHit(int damagePoints) {
			if (damagePrefabSpawnMode != DamagePrefabSpawnMode.PerHit) {
				return false;
			}
			
			SpawnDamagePrefabs(damagePoints);
			
			return true;
		}
		
		private void SpawnDamagePrefabs(int damagePoints) {
			if (IsInvincible() && damagePrefabSpawnMode != DamagePrefabSpawnMode.PerHit) { // do not spawn damage prefabs when invincible unless it's "per hit"
				return;
			}
			
			var numberToSpawn = 0;
			
			switch (damagePrefabSpawnMode) {
			case DamagePrefabSpawnMode.None:
				return;
			case DamagePrefabSpawnMode.PerHit:
				numberToSpawn = 1;
				break;
			case DamagePrefabSpawnMode.PerHitPointLost:
				numberToSpawn = Math.Min(hitPoints.Value, damagePoints);
				break;
			case DamagePrefabSpawnMode.PerGroupHitPointsLost:
				_damageTaken += damagePoints;
				var numberOfGroups = (int)Math.Floor(_damageTaken / (float)damageGroupsize.Value);
				numberToSpawn = numberOfGroups - _damagePrefabsSpawned;
				break;
			}
			
			if (numberToSpawn == 0) {
				return;
			}
			
			numberToSpawn *= damagePrefabSpawnQuantity.Value;
			
			var spawnPos = Trans.position + damagePrefabOffset;
			
			for (var i = 0; i < numberToSpawn; i++) {
				var prefabToSpawn = CurrentDamagePrefab;
				if (damagePrefabSource == SpawnSource.None ||
				    (damagePrefabSource != SpawnSource.None && prefabToSpawn == null)) {
					// empty element in Prefab Pool
					continue;
				}
				
				if (i > 0) {
					spawnPos += damagePrefabIncrementalOffset;
				}
				
				var spawnedDamagePrefab = SpawnPrefab(prefabToSpawn, spawnPos);
				if (spawnedDamagePrefab == null) {
					DamagePrefabFailedSpawn(prefabToSpawn);
				} else {
					SpawnUtility.RecordSpawnerObjectIfKillable(spawnedDamagePrefab, GameObj);
					
					// affect the spawned object.
					var euler = prefabToSpawn.rotation.eulerAngles;
					
					if (damagePrefabRandomizeXRotation) {
						euler.x = UnityEngine.Random.Range(0f, 360f);
					}
					if (damagePrefabRandomizeYRotation) {
						euler.y = UnityEngine.Random.Range(0f, 360f);
					}
					if (damagePrefabRandomizeZRotation) {
						euler.z = UnityEngine.Random.Range(0f, 360f);
					}
					
					spawnedDamagePrefab.rotation = Quaternion.Euler(euler);
					
					DamagePrefabSpawned(spawnedDamagePrefab);
				}
			}
			
			// clean up
			_damagePrefabsSpawned += numberToSpawn;
		}
		
		private bool SpawnDealDamagePrefabsIfTakingDamage(int damagePoints, Killable enemy) {
			if (IsInvincible() || damagePoints <= 0) { // do not spawn deal damage prefabs when invincible or no damage
				return false;
			}
			
			if (enemy == null) {
				return false;
			}

            if (enemy.dealDamageFireEvents) { // fire the "deal damage" custom events from that enemy, if any
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < enemy.dealDamageCustomEvents.Count; i++) {
                    var anEvent = enemy.dealDamageCustomEvents[i].CustomEventName;

                    LevelSettings.FireCustomEventIfValid(anEvent, Trans);
                }
            }

            var prefabToSpawn = enemy.CurrentDealDamagePrefab;
			if (prefabToSpawn == null) {
				// empty element, spawn nothing
				return false;
			}
			
			var spawnedDealDamagePrefab = SpawnPrefab(prefabToSpawn, Trans.position);
			
            // ReSharper disable once InvertIf
			if (spawnedDealDamagePrefab != null) {
				SpawnUtility.RecordSpawnerObjectIfKillable(spawnedDealDamagePrefab, GameObj);
				
				// affect the spawned object.
				var euler = prefabToSpawn.rotation.eulerAngles;
				
				if (enemy.dealDamagePrefabRandomizeXRotation) {
					euler.x = UnityEngine.Random.Range(0f, 360f);
				}
				if (enemy.dealDamagePrefabRandomizeYRotation) {
					euler.y = UnityEngine.Random.Range(0f, 360f);
				}
				if (enemy.dealDamagePrefabRandomizeZRotation) {
					euler.z = UnityEngine.Random.Range(0f, 360f);
				}
				
				spawnedDealDamagePrefab.rotation = Quaternion.Euler(euler);
			}
			
			return true;
		}
		
		private void SpawnDeathPrefabs() {
			if (UnityEngine.Random.Range(0, 100) >= deathPrefabSpawnPercent.Value) {
				return;
			}

			var spawnPos = DeathPrefabSpawnPosition;
			spawnPos += deathPrefabOffset;
			
			for (var i = 0; i < deathPrefabQty.Value; i++) {
				var deathPre = CurrentDeathPrefab;
				
				if (deathPrefabSource == WaveSpecifics.SpawnOrigin.PrefabPool && deathPre == null) {
					continue; // nothing to spawn
				}
				
				if (i > 0) {
					spawnPos += deathPrefabIncrementalOffset;
				}
				
				var spawnRotation = deathPre.transform.rotation;
				switch (rotationMode) {
					case RotationMode.InheritExistingRotation:
						spawnRotation = Trans.rotation;
						break;
					case RotationMode.CustomRotation:
						spawnRotation = Quaternion.Euler(deathPrefabCustomRotation);
						break;
				}


				var euler = spawnRotation.eulerAngles;
				
				if (deathPrefabRandomizeXRotation) {
					euler.x = UnityEngine.Random.Range(0f, 360f);
				}
				if (deathPrefabRandomizeYRotation) {
					euler.y = UnityEngine.Random.Range(0f, 360f);
				}
				if (deathPrefabRandomizeZRotation) {
					euler.z = UnityEngine.Random.Range(0f, 360f);
				}

				spawnRotation = Quaternion.Euler(euler);

				var theParent = deathPrefabKeepSameParent ? Trans.parent : null;
				var spawnedDeathPrefab = SpawnDeathPrefab(deathPre, spawnPos, spawnRotation, theParent);
				
				if (spawnedDeathPrefab != null) {
					DeathPrefabSpawned(spawnedDeathPrefab);
					
					SpawnUtility.RecordSpawnerObjectIfKillable(spawnedDeathPrefab, GameObj);
					
					if (!deathPrefabKeepVelocity) {
						continue;
					}
					var spawnedBody = spawnedDeathPrefab.GetComponent<Rigidbody>();
					if (spawnedBody != null && !spawnedBody.isKinematic && Body != null && !Body.isKinematic) {
						spawnedBody.velocity = Body.velocity;
					} else {
						var spawnedBody2D = spawnedDeathPrefab.GetComponent<Rigidbody2D>();
						if (spawnedBody2D != null && !spawnedBody2D.isKinematic && Body2D != null && !Body2D.isKinematic) {
							spawnedBody2D.velocity = Body2D.velocity;
						}
					}
				} else {
					DeathPrefabFailedSpawn(deathPre);
				}
			}
		}
		
		private void SpawnInvinceHitPrefab() {
			var prefabToSpawn = CurrentInvinceHitPrefab;
			if (prefabToSpawn == null) {
				// empty element, spawn nothing
				return;
			}
			
			var spawnedInvinceHitPrefab = SpawnPrefab(prefabToSpawn, Trans.position);
			// ReSharper disable once InvertIf
			if (spawnedInvinceHitPrefab != null) {
				SpawnUtility.RecordSpawnerObjectIfKillable(spawnedInvinceHitPrefab, GameObj);
				
				// affect the spawned object.
				var euler = prefabToSpawn.rotation.eulerAngles;
				
				if (invinceHitPrefabRandomizeXRotation) {
					euler.x = UnityEngine.Random.Range(0f, 360f);
				}
				if (invinceHitPrefabRandomizeXRotation) {
					euler.y = UnityEngine.Random.Range(0f, 360f);
				}
				if (invinceHitPrefabRandomizeXRotation) {
					euler.z = UnityEngine.Random.Range(0f, 360f);
				}
				
				spawnedInvinceHitPrefab.rotation = Quaternion.Euler(euler);
			}
		}
		
		// ReSharper disable once MemberCanBeMadeStatic.Local
		private void SpawnVanishPrefab() {
			var prefabToSpawn = CurrentVanishPrefab;
			if (prefabToSpawn == null) {
				// empty element, spawn nothing
				return;
			}
			
			var spawnedVanishPrefab = SpawnPrefab(prefabToSpawn, Trans.position);
			// ReSharper disable once InvertIf
			if (spawnedVanishPrefab != null) {
				SpawnUtility.RecordSpawnerObjectIfKillable(spawnedVanishPrefab, GameObj);
				
				// affect the spawned object.
				var euler = prefabToSpawn.rotation.eulerAngles;
				
				if (vanishPrefabRandomizeXRotation) {
					euler.x = UnityEngine.Random.Range(0f, 360f);
				}
				if (vanishPrefabRandomizeYRotation) {
					euler.y = UnityEngine.Random.Range(0f, 360f);
				}
				if (vanishPrefabRandomizeZRotation) {
					euler.z = UnityEngine.Random.Range(0f, 360f);
				}
				
				spawnedVanishPrefab.rotation = Quaternion.Euler(euler);
			}
		}
		
		private void ValidateWorldVariableModifier(WorldVariableModifier mod) {
			if (WorldVariableTracker.IsBlankVariableName(mod._statName)) {
				LevelSettings.LogIfNew(
					string.Format(
					"Killable '{0}' specifies a World Variable Modifier with no World Variable name. Please delete and re-add.",
					Trans.name));
			} else if (!WorldVariableTracker.VariableExistsInScene(mod._statName)) {
				LevelSettings.LogIfNew(
					string.Format(
					"Killable '{0}' specifies a World Variable Modifier with World Variable '{1}', which doesn't exist in the scene.",
					Trans.name,
					mod._statName));
			} else {
				switch (mod._varTypeToUse) {
				case WorldVariableTracker.VariableType._integer:
					if (mod._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
						if (!WorldVariableTracker.VariableExistsInScene(mod._modValueIntAmt.worldVariableName)) {
							if (LevelSettings.IllegalVariableNames.Contains(mod._modValueIntAmt.worldVariableName)) {
								LevelSettings.LogIfNew(
									string.Format(
									"Killable '{0}' wants to modify World Variable '{1}' using the value of an unspecified World Variable. Please specify one.",
									Trans.name,
									mod._statName));
							} else {
								LevelSettings.LogIfNew(
									string.Format(
									"Killable '{0}' wants to modify World Variable '{1}' using the value of World Variable '{2}', but the latter is not in the Scene.",
									Trans.name,
									mod._statName,
									mod._modValueIntAmt.worldVariableName));
							}
						}
					}
					
					break;
				case WorldVariableTracker.VariableType._float:
					
					break;
				default:
					LevelSettings.LogIfNew("Add code for varType: " + mod._varTypeToUse.ToString());
					break;
				}
			}
		}
		
		#endregion
		
		#region Virtual methods
		
		#region Pooling methods
		
		
		/// <summary>
		/// Override this in a subclass if you want to use a network Destroy or something like that
		/// </summary>
		public virtual bool DespawnPrefab() {
			return PoolBoss.Despawn(Trans);
		}
		
		
		/// <summary>
		/// Override this in a subclass if you want to use a network Instantiate or something like that
		/// </summary>
		/// <param name="prefabToSpawn">The prefab to spawn.</param>
		/// <param name="spawnPos">The position to spawn in.</param>
		/// <returns>The Transform of the spawned prefab.</returns>
		public virtual Transform SpawnPrefab(Transform prefabToSpawn, Vector3 spawnPos) {
			return PoolBoss.SpawnInPool(prefabToSpawn, spawnPos, Quaternion.identity);
		}
		
		/// <summary>
		/// Override this in a subclass if you want to use a network Instantiate or something like that
		/// </summary>
		/// <param name="deathPre">The death prefab.</param>
		/// <param name="spawnPos">The position to spawn it in.</param>
		/// <param name="spawnRotation">The rotation to spawn it in.</param>
		/// <param name="theParent">The object to parent the spawned death prefab to.</param>
		/// <returns>The Transform of the spawned death prefab.</returns>
		public virtual Transform SpawnDeathPrefab(Transform deathPre, Vector3 spawnPos, Quaternion spawnRotation,
		                                          Transform theParent) {
			return PoolBoss.Spawn(deathPre, spawnPos, spawnRotation, theParent);
		}
		
		#endregion
		
		/// <summary>
		/// This method will be called when a Damage Prefab fails to spawn.
		/// </summary>
		/// <param name="prefabToSpawn">The prefab that failed spawn.</param>
		protected virtual void DamagePrefabFailedSpawn(Transform prefabToSpawn) {
			if (listener != null) {
				listener.DamagePrefabFailedToSpawn(prefabToSpawn);
			}
		}
		
		/// <summary>
		/// This method will be called when a Damage Prefab spawns.
		/// </summary>
		/// <param name="spawnedDamagePrefab">The prefab that spawned.</param>
		protected virtual void DamagePrefabSpawned(Transform spawnedDamagePrefab) {
			if (listener != null) {
				listener.DamagePrefabSpawned(spawnedDamagePrefab);
			}
		}
		
		/// <summary>
		/// This method is called when damage is prevents by Invincibility
		/// </summary>
		/// <param name="pointsDamage">The number of points prevented.</param>
		/// <param name="enemyHitBy">The enemy that tried to inflict damage.</param>
		protected virtual void DamagePrevented(int pointsDamage, Killable enemyHitBy) {
			listener.DamagePrevented(pointsDamage, enemyHitBy);
		}
		
		/// <summary>
		///  This method gets called when the death delay starts (good to override and play a death animation).
		/// </summary>
		protected virtual void DeathDelayStarted() {
			if (listener != null) {
				listener.DeathDelayStarted(deathDelay.Value);
			}
		}
		
		/// <summary>
		/// This method gets called when a Death Prefab fails to spawn.
		/// </summary>
		/// <param name="deathPre">The Death Prefab that failed to spawn.</param>
		protected virtual void DeathPrefabFailedSpawn(Transform deathPre) {
			if (listener != null) {
				listener.DeathPrefabFailedToSpawn(deathPre);
			}
		}
		
		/// <summary>
		/// This method gets called when a Death Prefab spawns.
		/// </summary>
		/// <param name="spawnedDeathPrefab">The spawned Death Prefab.</param>
		protected virtual void DeathPrefabSpawned(Transform spawnedDeathPrefab) {
			if (listener != null) {
				listener.DeathPrefabSpawned(spawnedDeathPrefab);
			}
		}
		
		/// <summary>
		/// Call this method to despawn the Killable. This is not the same as DestroyKillable. This will not spawn a death prefab and will not modify World Variables.
		/// </summary>
		/// <param name="eType">The event type.</param>
		public virtual bool Despawn(TriggeredSpawner.EventType eType) {
			if (LevelSettings.AppIsShuttingDown || _isDespawning) {
				return false;
			}
			
			_isDespawning = true;
			
			if (listener != null) {
				listener.Despawning(eType);
			}
			
			return DespawnOrRespawn();
		}
		
		/// <summary>
		/// Despawns or respawns depending on the setup option chosen. Except when Despawn Behavior is set to "Disable", in which case this game object is disabled instead.
		/// </summary>
		public virtual bool DespawnOrRespawn() {
			EnableColliders();
			
			// possibly move this into OnDespawned if it causes problems
			_childKillables.Clear();
			
			ResetSpawnerInfo();

			var isSuccess = true;

			if (deathDespawnBehavior == DeathDespawnBehavior.Disable) {
				if (_parentKillable != null) {
					_parentKillable.UnregisterChildKillable(this);
				}
				
				SpawnUtility.SetActive(GameObj, false);
				return true;
			}
			
			if (respawnType == RespawnType.None || GameIsOverForKillable) {
				if (!DespawnThis()) {
					isSuccess = false;
				}
			} else if (_timesRespawned >= timesToRespawn && respawnType != RespawnType.Infinite) {
				_timesRespawned = 0;
				_spawnLocationSet = false;
				if (!DespawnThis()) {
					isSuccess = false;
				}
			} else {
				_timesRespawned++;
				
				// reset velocity
				ResetVelocity();
				
				if (respawnDelay.Value <= 0f) {
				    FireRespawnEvents();
					
					SpawnedOrAwake(false);
				} else {
					LevelSettings.TrackTimedRespawn(respawnDelay.Value, Trans, Trans.position, respawnFireEvents);
					if (!DespawnThis()) {
						isSuccess = false;
					}
				}
			}

			return isSuccess;
		}
		
		/// <summary>
		/// This handles just despawning the item, when it's decided that you don't want to just immediately respawn it.
		/// </summary>
		public virtual bool DespawnThis() {

			if (_parentKillable != null) {
				_parentKillable.UnregisterChildKillable(this);
			}
			
			if (!IsDead) {
				SpawnVanishPrefab();
			}
			
			var isSuccess = DespawnPrefab();
			
			if (listener != null) {
				listener.Despawned();
			}

			return isSuccess;
		}
		
		/// <summary>
		/// This method can be called to immediately destroy the Killable (death delay will be ignored)
		/// </summary>
		/// <param name="scenarioName"></param>
		public void DestroyImmediately(string scenarioName = DestroyedText) {
			DestroyKillable(scenarioName, true);
		}
		
		/// <summary>
		/// Call this method when you want the Killable to die. The death prefab (if any) will be spawned and World Variable Modifiers will be executed.
		/// </summary>
		/// <param name="scenarioName">(optional) pass the name of an alternate scenario if you wish to use a different set of World Variable Modifiers from that scenario.</param>
		/// <param name="skipDeathDelay">You can pass true to skip the death delay and "destroy now"</param>
		public virtual void DestroyKillable(string scenarioName = DestroyedText, bool skipDeathDelay = false) {
			if (_waitingToDestroy) {
				return; // already on it's way out! Don't destroy twice.
			}
			
			// set variables for later destruction in Update method (so all other collisions get registered as well)
			_waitingToDestroy = true;
			_willSkipDeathDelay = skipDeathDelay;
			_deathScenarioName = scenarioName;
		}
		
		/// <summary>
		/// This method you can override in a subclass to add logic to decide which Scenario to use.
		/// </summary>
		/// <param name="scenarioName">The default scenario is passed here by default (Destroyed).</param>
		/// <returns>The chosen Scenario name.</returns>
		public virtual string DetermineScenario(string scenarioName) {
			return scenarioName;
		}
		
		/// <summary>
		/// Determines whether this instance is invincible.
		/// </summary>
		/// <returns><c>true</c> if this instance is invincible; otherwise, <c>false</c>.</returns>
		public virtual bool IsInvincible() {
			return isInvincible || (invincibleWhileChildrenKillablesExist && _childKillables.Count > 0);
		}
		
		/// <summary>
		/// Determines whether this instance is temporarily invincible.
		/// </summary>
		/// <returns><c>true</c> if this instance is temporarily invincible; otherwise, <c>false</c>.</returns>
		public virtual bool IsTemporarilyInvincible() {
			return _isTemporarilyInvincible;
		}

        /// <summary>
        /// This will knock back (and up) this Killable based on the Knockback settings of Enemy passed in.
        /// </summary>
        /// <param name="enemy">The enemy knocking back this Killable.</param>
        public virtual void Knockback(Killable enemy) {
			if (enemy == null) {
				return;
			}

			if (!enemy.sendDamageKnockback || enemy.atckPoints.Value <= 0) { 
				return; // negative damage (health increase) should not knockback.
            }

			if (IsInvincible()) {
				if (!ReceiveKnockbackWhenInvince) {
					return;
				}
			} else { // was damaged
				if (!ReceiveKnockbackWhenDamaged) {
					return;
				}
			}

			var pushHeight = enemy.damageKnockUpMeters.Value;
			var pushForce = enemy.damageKnockBackFactor.Value;

			var pushDir = (Trans.position - enemy.Trans.position);
			pushDir.y = 0f;

			if (Body != null) {
				Body.velocity = new Vector3(0, 0, 0);
				Body.AddForce(pushDir.normalized * pushForce, ForceMode.VelocityChange);
				Body.AddForce(Vector3.up * pushHeight, ForceMode.VelocityChange);
			} else if (Body2D != null) {
				// Rigidbody 2D
				Body2D.velocity = new Vector2(0, 0);
				var knockback = Vector2.right * pushForce; // knock right

				if (enemy.Trans.position.x > Trans.position.x) {
					knockback *= -1; // knock left
				}

				Body2D.AddForce(knockback, ForceMode2D.Impulse);
				Body2D.AddForce(Vector3.up * pushHeight, ForceMode2D.Impulse);
			} else if (CharController != null) {
			    var move = pushDir.normalized * pushForce;
                move.y = (Vector3.up * pushHeight).y;

                CharController.Move(move);
			}
		}
		
		/// <summary>
		/// This method will be called when the Damage World Variable Modifiers are about to be modified.
		/// </summary>
		/// <param name="mods">The World Variable Modifier collection</param>
		protected virtual void ModifyDamageWorldVariables(List<WorldVariableModifier> mods) {
			listener.ModifyingDamageWorldVariables(mods);
		}
		
		/// <summary>
		/// This method will be called when the Death World Variable Modifiers are about to be modified.
		/// </summary>
		/// <param name="mods">The World Variable Modifier collection</param>
		protected virtual void ModifyDeathWorldVariables(List<WorldVariableModifier> mods) {
			listener.ModifyingDeathWorldVariables(mods);
		}
		
		/// <summary>
		/// This method will be used for "Any Hit" deaths if specified. You can override this to add additional criteria to a subclass.
		/// </summary>
		/// <param name="enemyKillable"></param>
		/// <returns></returns>
		protected virtual bool ShouldDieFromNonDamageHit(Killable enemyKillable) {
			return despawnMode == DespawnMode.CollisionOrTrigger;
		}
		
		/// <summary>
		/// This will determine whether or not to use the "Damage World Variable Modifiers" that are set up whenever the Killable is damaged.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldFireDamageVariableModifiers() {
			return true;
		}
		
		/// <summary>
		/// This will determine whether or not to use the "Death World Variable Modifiers" that are set up whenever the Killable is damaged.
		/// </summary>
		/// <returns></returns>
		protected virtual bool ShouldFireDeathVariableModifiers() {
			return true;
		}
		
		/// <summary>
		/// This method gets called whenever the object is spawned or starts in a Scene (from Awake event)
		/// </summary>
		/// <param name="spawned">True if spawned, false if in the Scene at beginning.</param>
		protected virtual void SpawnedOrAwake(bool spawned = true) {
			if (listener != null) {
				if (spawned) {
					listener.Spawned(this);
				} else {
					listener.StartedInScene(this);
				}
			}

		    if (!spawned && Trans.parent == null) {
		        var theName = PoolBoss.GetPrefabName(Trans); // add the PoolableInfo script if not there when this starts already in the Scene.
                if (string.IsNullOrEmpty(theName)) { } // get rid of warning.
		    }

			KilledBy = null;
			_waitingToDestroy = false;
			_deathDelayGoing = false;

			_childrenToDestroy.Clear();
			
			if (parentDestroyedAction != SpawnerDestroyedBehavior.DoNothing && parentKillableForParentDestroyed != null) {
				parentKillableForParentDestroyed.RecordChildToDie(this);
			}
			
			// anything you want to do each time this is spawned.
			if (_timesRespawned == 0) {
				isVisible = false;
				_becameVisible = false;
			}
			
			_isDespawning = false;
			_spawnTime = Time.time;
			_isTemporarilyInvincible = false;
			_spawnPoint = Trans.position;
			
			if (!_spawnLocationSet) {
				_respawnLocation = Trans.position;
				_spawnLocationSet = true;
			}
			
			// respawning from "respawn" setting.
			if (_timesRespawned > 0) {
				Trans.position = _respawnLocation;
			} else {
				// register child Killables with parent, if any
				var aParent = Trans.parent;
				while (aParent != null) {
					_parentKillable = aParent.GetComponent<Killable>();
					if (_parentKillable == null) {
						aParent = aParent.parent;
						continue;
					}
					
					_parentKillable.RegisterChildKillable(this);
					break;
				}
			}
			
			currentHitPoints = hitPoints.Value;
			
			_damageTaken = 0;
			_damagePrefabsSpawned = 0;
			
			if (deathPrefabPoolName != null && deathPrefabSource == WaveSpecifics.SpawnOrigin.PrefabPool) {
				_deathPrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(deathPrefabPoolName);
				if (_deathPrefabWavePool == null) {
					LevelSettings.LogIfNew("Death Prefab Pool '" + deathPrefabPoolName + "' not found for Killable '" +
					                       name + "'.");
				}
			}
			
			if (damagePrefabSpawnMode != DamagePrefabSpawnMode.None && damagePrefabPoolName != null &&
			    damagePrefabSource == SpawnSource.PrefabPool) {
				_damagePrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(damagePrefabPoolName);
				if (_damagePrefabWavePool == null) {
					LevelSettings.LogIfNew("Damage Prefab Pool '" + _damagePrefabWavePool + "' not found for Killable '" +
					                       name + "'.");
				}
			}
			
			if (dealDamagePrefabSource == SpawnSource.PrefabPool) {
				_dealDamagePrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(dealDamagePrefabPoolName);
				if (_dealDamagePrefabWavePool == null) {
					LevelSettings.LogIfNew("Deal Damage Prefab Pool '" + _dealDamagePrefabWavePool + "' not found for Killable '" +
					                       name + "'.");
				}
			}
			
			if (vanishPrefabSource == SpawnSource.PrefabPool) {
				_vanishPrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(vanishPrefabPoolName);
				if (_vanishPrefabWavePool == null) {
					LevelSettings.LogIfNew("Vanish Prefab Pool '" + vanishPrefabPoolName + "' not found for Killable '" +
					                       name + "'.");
				}
			}
			
			if (invinceHitPrefabSource == SpawnSource.PrefabPool) {
				_invinceHitPrefabWavePool = LevelSettings.GetFirstMatchingPrefabPool(invinceHitPrefabPoolName);
				if (_invinceHitPrefabWavePool == null) {
					LevelSettings.LogIfNew("Invince Hit Prefab Pool '" + invinceHitPrefabPoolName + "' not found for Killable '" +
					                       name + "'.");
				}
			}
			
			if (damagePrefabSpawnMode != DamagePrefabSpawnMode.None && damagePrefabSource == SpawnSource.Specific &&
			    damagePrefabSpecific == null) {
				LevelSettings.LogIfNew(string.Format("Damage Prefab for '{0}' is not assigned.", Trans.name));
			}
			
			CheckForValidVariables();
			
			StopAllCoroutines(); // for respawn purposes.
			
			deathDespawnBehavior = DeathDespawnBehavior.ReturnToPool;
			
			if (invincibleOnSpawn) {
				TemporaryInvincibility(invincibleTimeSpawn.Value);
			}
		}
		
		/// <summary>
		/// This method gets called when the game object that spawned this Killable is destroyed.
		/// </summary>
		protected virtual void SpawnerDestroyed() {
			if (listener != null) {
				listener.SpawnerDestroyed();
			}
		}
		
		/// <summary>
		/// Call this method to inflict X points of damage to a Killable. 
		/// </summary>
		/// <param name="damagePoints">The number of points of damage to inflict.</param>
		public virtual void TakeDamage(int damagePoints) {
			TakeDamage(damagePoints, null);
		}
		
		/// <summary>
		/// Call this method to inflict X points of damage to a Killable. 
		/// </summary>
		/// <param name="damagePoints">The number of points of damage to inflict.</param>
		/// <param name="enemy">The other Killable that collided with this one.</param>
		public virtual void TakeDamage(int damagePoints, Killable enemy) {
			var dmgPrefabsSpawned = false;
			var dealDmgPrefabSpawned = false;
			var varsModded = false;
			var eventsFired = false;
			
			var knockBackSent = false;
		    var enemySendsKnockback = enemy != null && enemy.sendDamageKnockback;

			if (IsInvincible()) {
				SpawnInvinceHitPrefab();
				
				if (damagePoints >= 0) {
					LogIfEnabled("Taking no damage because it's currently invincible!");
				}
				
				if (listener != null) {
					DamagePrevented(damagePoints, enemy);
				}
				
				if (despawnMode == DespawnMode.CollisionOrTrigger) {
					LogIfEnabled("Destroyed anyway because 'HP Death Mode' set to on Collision Or Trigger!");
					DestroyKillable();
				}
				
				// mod variables and spawn dmg prefabs
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				if (!varsModded) {
					ModifyWorldVariables(playerStatDamageModifiers, true);
					varsModded = true;
				}
				
				// ReSharper disable once ConditionIsAlwaysTrueOrFalse
				if (!eventsFired && damageFireEvents) {
					// ReSharper disable once ForCanBeConvertedToForeach
					for (var i = 0; i < damageCustomEvents.Count; i++) {
						var anEvent = damageCustomEvents[i].CustomEventName;
						
						LevelSettings.FireCustomEventIfValid(anEvent, Trans);
					}
					eventsFired = true;
				}
				
				dmgPrefabsSpawned = SpawnDamagePrefabsIfPerHit(damagePoints);
				// end mod variables and spawn dmg prefabs
				
				if (enemySendsKnockback) {
					knockBackSent = true;
					Knockback(enemy);
				}
				
				if (damagePoints >= 0) {
					// allow negative damage to continue
					return;
				}
			}
			
			TakingDamage(damagePoints, enemy);
			
			if (enemySendsKnockback && !knockBackSent) {
				Knockback(enemy);
			}
			
			var newHP = currentHitPoints - damagePoints;
			var isDeathHit = newHP <= 0 || (despawnMode == DespawnMode.LostAnyHitPoints && damagePoints > 0) || (ShouldDieFromNonDamageHit(enemy));
			
			// mod variables and spawn dmg prefabs
			if (!varsModded) {
				ModifyWorldVariables(playerStatDamageModifiers, true);
				varsModded = true;
			}
			
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			if (!eventsFired && damageFireEvents) {
				// ReSharper disable once ForCanBeConvertedToForeach
				for (var i = 0; i < damageCustomEvents.Count; i++) {
					var anEvent = damageCustomEvents[i].CustomEventName;
					
					LevelSettings.FireCustomEventIfValid(anEvent, Trans);
				}
				// ReSharper disable once RedundantAssignment
				eventsFired = true;
			}
			
			var shouldSpawnDmgPrefab = !isDeathHit || damagePrefabOnDeathHit;
			var shouldSpawnDealDmgPrefab = !isDeathHit || (enemy != null && enemy.dealDamagePrefabOnDeathHit);
			
			if (!dmgPrefabsSpawned && shouldSpawnDmgPrefab) {
				dmgPrefabsSpawned = SpawnDamagePrefabsIfPerHit(damagePoints);
			}
			
			if (shouldSpawnDealDmgPrefab) {
				dealDmgPrefabSpawned = SpawnDealDamagePrefabsIfTakingDamage(damagePoints, enemy);
			}
			
			// end mod variables and spawn dmg prefabs
			
			if (damagePoints == 0 && !isDeathHit) {
				return;
			}
			
			if (enableLogging) {
				LogIfEnabled("Taking " + damagePoints + " points damage!");
			}
			
			currentHitPoints = newHP;
			
			if (currentHitPoints < 0) {
				currentHitPoints = 0;
			} else if (currentHitPoints > maxHitPoints.Value) {
				currentHitPoints = maxHitPoints.Value;
			}
			
			// must do this first so you don't turn invincible by the next lines!
			if (!dealDmgPrefabSpawned && isDeathHit && (enemy != null && enemy.dealDamagePrefabOnDeathHit)) {
				SpawnDealDamagePrefabsIfTakingDamage(damagePoints, enemy);
			}
			
			if (invincibleWhenDamaged && currentHitPoints > 0) {
				TemporaryInvincibility(invincibleDamageTime.Value);
			}
			
			if (hitPoints.variableSource == LevelSettings.VariableSource.Variable && syncHitPointWorldVariable) {
				var aVar = WorldVariableTracker.GetWorldVariable(hitPoints.worldVariableName);
				if (aVar != null) {
					aVar.CurrentIntValue = currentHitPoints;
				}
			}
			
			// mod variables and spawn dmg prefabs
			// ReSharper disable once ConditionIsAlwaysTrueOrFalse
			// ReSharper disable HeuristicUnreachableCode
			if (!varsModded) {
				ModifyWorldVariables(playerStatDamageModifiers, true);
				// ReSharper disable once RedundantAssignment
				varsModded = true;
			}
			// ReSharper restore HeuristicUnreachableCode
			
			if (!dmgPrefabsSpawned && shouldSpawnDmgPrefab) {
				SpawnDamagePrefabs(damagePoints);
			}
			
			// end mod variables and spawn dmg prefabs
			
			switch (despawnMode) {
			case DespawnMode.ZeroHitPoints:
				if (currentHitPoints > 0) {
					return;
				}
				break;
			case DespawnMode.None:
				return;
			}
			
			KilledBy = enemy;
			DestroyKillable();
		}
		
		/// <summary>
		/// This method when damage is valid and is about to be inflicted (no invincibility).
		/// </summary>
		/// <param name="damagePoints">Number of damage points to take.</param>
		/// <param name="enemy">The enemy that dealt the damage.</param>
		protected virtual void TakingDamage(int damagePoints, Killable enemy) {
			if (listener != null) {
				listener.TakingDamage(damagePoints, enemy);
			}
		}
		
		#endregion
		
		#region CoRoutines
		private IEnumerator SetSpawnInvincibleForSeconds(float seconds) {
			_isTemporarilyInvincible = true;
			isInvincible = true;
			
			yield return new WaitForSeconds(seconds);
			
			if (!_isTemporarilyInvincible) {
				yield break;
			}
			
			isInvincible = false;
			_isTemporarilyInvincible = false;
		}
		
		private IEnumerator WaitThenDestroy(string scenarioName) {
			_deathDelayGoing = true;

			DeathDelayStarted();
			
			if (deathDelay.Value > 0f) {
				if (listener != null) {
					listener.WaitingToDestroyKillable(this);
				}
				
				yield return new WaitForSeconds(deathDelay.Value);
			}
			
			if (!PerformDeath(scenarioName)) {
				_waitingToDestroy = false; // stop endless death prefab spawning loop 
			}

			_deathDelayGoing = false;
		}
		#endregion
		
		#region Properties
		
		/*! \cond PRIVATE */
		private Transform CurrentDeathPrefab {
			get {
				switch (deathPrefabSource) {
				case WaveSpecifics.SpawnOrigin.Specific:
					return deathPrefabSpecific;
				case WaveSpecifics.SpawnOrigin.PrefabPool:
					return _deathPrefabWavePool.GetRandomWeightedTransform();
				}
				
				return null;
			}
		}
		
		private Transform CurrentDamagePrefab {
			get {
				switch (damagePrefabSource) {
				case SpawnSource.Specific:
					return damagePrefabSpecific;
				case SpawnSource.PrefabPool:
					if (_damagePrefabWavePool == null) {
						return null;
					}
					
					return _damagePrefabWavePool.GetRandomWeightedTransform();
				}
				
				return null;
			}
		}

        private bool IsVisualizationClone {
            get {
                if (_visualizationMarker != null) {
                    return true;
                }

                _visualizationMarker = GetComponent<VisualizationMarker>();
                return _visualizationMarker != null;
            }
        }

        public int? SpawnedFromObjectId {
			get {
				if (SpawnedFromObject != null && !_spawnedFromGOInstanceId.HasValue) {
					_spawnedFromGOInstanceId = SpawnedFromObject.GetInstanceID();
				}
				
				return _spawnedFromGOInstanceId;
			}
		}
		
		private GameObject GameObj {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_go == null) {
					_go = gameObject;
				}
				
				return _go;
			}
		}
		
		private int? KillableId {
			get {
				if (!_instanceId.HasValue) {
					_instanceId = GameObj.GetInstanceID();
				}
				
				return _instanceId;
			}
		}

	    public bool CanReceiveKnockback {
	        get { return IsGravBody || IsCharController; }
        }

		public bool ReceiveKnockbackWhenInvince {
			get { return CanReceiveKnockback && receiveKnockbackWhenInvince; }
		}
		
		public bool ReceiveKnockbackWhenDamaged {
			get { return CanReceiveKnockback && receiveKnockbackWhenDamaged; }
		}
		
		/*! \endcond */
		
		/// <summary>
		/// Returns a cached lazy-lookup of the Rigidbody component
		/// </summary>
		public Rigidbody Body {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_body == null) {
					_body = GetComponent<Rigidbody>();
				}
				
				return _body;
			}
		}
		
		/// <summary>
		/// Returns a list of all Killables that are a child of this one.
		/// </summary>
		public List<Killable> ChildKillables {
			get {
				return _childKillables;
			}
		}
		
		/// <summary>
		/// Returns a cached lazy-lookup of the Collider component
		/// </summary>
		public Collider Colidr {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_collider == null) {
                    _collider = GetComponent<Collider>();
				}
				
				return _collider;
			}
		}
		
		/// <summary>
		/// The current hit points.
		/// </summary>
		public int CurrentHitPoints {
			get { return currentHitPoints; }
			set { currentHitPoints = value; }
		}
		
		/// <summary>
		/// This returns the Vanish prefab for this Killable (spawned when it despawns if not "killed").
		/// </summary>
		public Transform CurrentVanishPrefab {
			get {
				switch (vanishPrefabSource) {
				case SpawnSource.PrefabPool:
					if (_vanishPrefabWavePool == null) {
						return null;
					}
					
					return _vanishPrefabWavePool.GetRandomWeightedTransform();
				case SpawnSource.Specific:
					return vanishPrefabSpecific;
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// This returns the Deal Damage prefab for this Killable (spawned on things it damages only).
		/// </summary>
		public Transform CurrentDealDamagePrefab {
			get {
				switch (dealDamagePrefabSource) {
				case SpawnSource.Specific:
					return dealDamagePrefabSpecific;
				case SpawnSource.PrefabPool:
					if (_dealDamagePrefabWavePool == null) {
						return null;
					}
					
					return _dealDamagePrefabWavePool.GetRandomWeightedTransform();
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// This returns the Invince Hit prefab for this Killable (spawned when it is hit while Invincible).
		/// </summary>
		public Transform CurrentInvinceHitPrefab {
			get {
				switch (invinceHitPrefabSource) {
				case SpawnSource.Specific:
					return invinceHitPrefabSpecific;
				case SpawnSource.PrefabPool:
					if (_invinceHitPrefabWavePool == null) {
						return null;
					}
					
					return _invinceHitPrefabWavePool.GetRandomWeightedTransform();
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// This property will return true if the Game Over Behavior setting makes this Killable disabled.
		/// </summary>
		public bool GameIsOverForKillable {
			get { return LevelSettings.IsGameOver && gameOverBehavior == TriggeredSpawner.GameOverBehavior.Disable; }
		}
		
		/// <summary>
		/// This will return if the Killable has died (maybe of use to other non-CGK scripts).
		/// </summary>
		public bool IsDead {
			get { return _waitingToDestroy; }
		}
		
		/// <summary>
		/// Returns whether the Killable has a gravity rigidbody (3d or 2d)
		/// </summary>
		public bool IsGravBody {
			get { return (Body != null && Body.useGravity) || (Body2D != null && Body2D.gravityScale > 0); }
		}

        /*! \cond PRIVATE */

	    public bool IsCharController {
	        get { return CharController != null; }
	    }

        public bool IsMyMultiplayerPrefab { 
			get {
				return PoolBoss.IsMyMultiplayerPrefab(this); 
			}
		}
		/*! \endcond */

        /// <summary>
        /// This property will return a reference to the Killable that dealt lethal damage to this one.
        /// </summary>
        public Killable KilledBy { get; private set; }
		
		/// <summary>
		/// Gets or sets the respawn position. Defaults to the location last spawned.
		/// </summary>
		/// <value>The respawn position.</value>
		public Vector3 RespawnPosition {
			get { return _respawnLocation; }
			set { _respawnLocation = value; }
		}

		/// <summary>
		/// The game object this Killable was spawned from, if any.
		/// </summary>
		public GameObject SpawnedFromObject {
			get { return _spawnedFromObject; }
		}
		
		/// <summary>
		/// This property returns a cached lazy-lookup of the Transform component.
		/// </summary>
		public Transform Trans {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_trans == null) {
					_trans = transform;
				}
				
				return _trans;
			}
		}
		
		/// <summary>
		/// This property returns the number of times this Killable has been respawned since it was first spawned.
		/// </summary>
		public int TimesRespawned {
			get { return _timesRespawned; }
		}
		
		private Rigidbody2D Body2D {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_body2D == null) {
					_body2D = GetComponent<Rigidbody2D>();
				}
				
				return _body2D;
			}
		}
		
		/*! \cond PRIVATE */
		private Vector3 DeathPrefabSpawnPosition {
			get {
				switch (deathPrefabSpawnLocation) {
					case DeathPrefabSpawnLocation.DeathPosition:
					default:
						return Trans.position;
					case DeathPrefabSpawnLocation.RespawnPosition:
						return RespawnPosition;
				}
			}
		}

		private Collider2D Colidr2D {
			get {
				// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
				if (_collider2D == null) {
					_collider2D = GetComponent<Collider2D>();
				}
				
				return _collider2D;
			}
		}

	    private CharacterController CharController {
	        get {
	            if (_charCtrl != null) {
	                return _charCtrl;
	            }

                _charCtrl = GetComponent<CharacterController>();
	            return _charCtrl;
	        }
        }

		/*! \endcond */
		
		#endregion
	}
}