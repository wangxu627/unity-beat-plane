using UnityEngine;
using System.Collections;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
	/// <summary>
	/// This class is used to configure a Timed Despawner
	/// </summary>
	[AddComponentMenu("Dark Tonic/Core GameKit/Despawners/Timed Despawner")]
	// ReSharper disable once CheckNamespace
	public class TimedDespawner : MonoBehaviour {
		/*! \cond PRIVATE */
		public float LifeSeconds = 5;
		public bool StartTimerOnSpawn = true;
		// ReSharper disable InconsistentNaming
		public TimedDespawnerListener listener;
		// ReSharper restore InconsistentNaming
		/*! \endcond */
		
		private Transform _trans;
		private YieldInstruction _timerDelay;
		
		// ReSharper disable once UnusedMember.Local
		private void Awake() {
			_trans = transform;
			_timerDelay = new WaitForSeconds(LifeSeconds);
			AwakeOrSpawn();
		}
		
		// ReSharper disable once UnusedMember.Local
		private void OnSpawned() {
			// used by Core GameKit Pooling & also Pool Manager Pooling!
			AwakeOrSpawn();
		}
		
		private void AwakeOrSpawn() {
			if (StartTimerOnSpawn) {
				StartTimer();
			}
		}
		
		/// <summary>
		/// Call this method to start the Timer if it's not set to start automatically.
		/// </summary>
		public void StartTimer() {
			StartCoroutine(WaitUntilTimeUp());
		}
		
		private IEnumerator WaitUntilTimeUp() {
			yield return _timerDelay;
			
			if (listener != null) {
				listener.Despawning(_trans);
			}
			PoolBoss.Despawn(_trans);
			
		}
		
		/// <summary>
		/// Call this method to change the despawn timer
		/// </summary>
		/// <param name="time"> The new despawn timer value.</param>
		public void ChangeTimerDelay(float time)
		{
			_timerDelay = new WaitForSeconds(time);
		}
	}
}