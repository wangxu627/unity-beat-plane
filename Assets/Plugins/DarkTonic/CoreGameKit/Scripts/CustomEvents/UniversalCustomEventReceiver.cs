/*! \cond PRIVATE */
using UnityEngine;
using DarkTonic.CoreGameKit;

namespace DarkTonic.CoreGameKit {
	/// <summary>
	/// What happens when the Custom Event fires is in the ReceiveEvent method below. 
	/// </summary>
	// ReSharper disable once CheckNamespace
	public class UniversalCustomEventReceiver : MonoBehaviour, ICgkEventReceiver {
	    public enum EventReceivedBehavior
	    {
	        None,
	        DealXDamage,
	        Despawn,
	        Destroy
	    }

	    [CoreCustomEvent]
	    public string CustomEvent = LevelSettings.NoEventName;

	    public EventReceivedBehavior eventReceivedBehavior = EventReceivedBehavior.None;

	    [Tooltip("Only used if Event Received Behavior is set to Deal X Damage")]
	    public int damageToDeal = 1;

	    private Transform _trans;
	    private Killable _kill;

	    #region MonoBehaviour events

	    // ReSharper disable once UnusedMember.Local
	    private void OnEnable() {
	        RegisterReceiver();
	    }

	    // ReSharper disable once UnusedMember.Local
	    private void OnDisable() {
	        UnregisterReceiver();
	    }

	    // ReSharper disable once UnusedMember.Local
	    private void Start() {
	        CheckForIllegalCustomEvents();
	    }

	    #endregion

	    #region ICgkEventReceiver methods
	    public void CheckForIllegalCustomEvents() {
	        if (CustomEvent != LevelSettings.NoEventName && !LevelSettings.CustomEventExists(CustomEvent)) {
	            LevelSettings.LogIfNew("Transform '" + name + "' is set up to receive or fire Custom Event '" +
	                CustomEvent + "', which does not exist in Core GameKit.");
	        }
	    }

	    public void ReceiveEvent(string customEventName, Vector3 eventOrigin) {
	        switch (eventReceivedBehavior) {
	            case EventReceivedBehavior.None:
	                break;
	            case EventReceivedBehavior.Despawn:
	                PoolBoss.Despawn(Trans);
	                break;
	            case EventReceivedBehavior.DealXDamage:
	                if (Kill == null) {
	                    LogNoKillable();
	                    break;
	                }

	                Kill.TakeDamage(1);

	                break;
	            case EventReceivedBehavior.Destroy:
	                if (Kill == null) {
	                    LogNoKillable();
	                    break;
	                }

	                Kill.DestroyKillable();

	                break;
	        }
	    }

	    public bool SubscribesToEvent(string customEventName) {
	        return CustomEvent == customEventName;
	    }

	    public void RegisterReceiver() {
	        if (CustomEvent != LevelSettings.NoEventName) {
	            LevelSettings.AddCustomEventReceiver(this, Trans);
	        }
	    }

	    public void UnregisterReceiver() {
	        if (CustomEvent != LevelSettings.NoEventName) {
	            LevelSettings.RemoveCustomEventReceiver(this);
	        }
	    }
	    #endregion

	    #region Helper methods

	    private void LogNoKillable() {
	        Debug.Log("Event Received Behavior of Deal X Damage cannot be used on game object '" + name + "' because it has no Killable component.");
	    }

	    #endregion

	    #region Properties
	    public Killable Kill {
	        get {
	            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
	            if (_kill == null) {
	                _kill = GetComponent<Killable>();
	            }

	            return _kill;
	        }
	    }

	    public Transform Trans {
	        get {
	            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
	            if (_trans == null) {
	                _trans = GetComponent<Transform>();
	            }

	            return _trans;
	        }
	    }

	    #endregion
	}
}
/*! \endcond */