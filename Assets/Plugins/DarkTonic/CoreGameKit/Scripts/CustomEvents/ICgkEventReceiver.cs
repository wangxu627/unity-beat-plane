/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    // ReSharper disable once CheckNamespace
    public interface ICgkEventReceiver {
        // this interface is used to "listen" to custom events that Core GameKit transmits.
        /// <summary>
        /// This checks for events that are not found in Core GameKit. It's a good idea to call this in Start (Awake is too early), and save yourself some troubleshooting time! Optional
        /// </summary>
        void CheckForIllegalCustomEvents();

        /// <summary>
        /// This receives the event when it's fired.
        /// </summary>
        void ReceiveEvent(string customEventName, Vector3 eventOrigin);

        /// <summary>
        /// This returns a bool of whether the specified custom event is subscribed to in this class
        /// </summary>
        bool SubscribesToEvent(string customEventName);

        /// <summary>
        /// Registers the receiver with Core GameKit. Call this in OnEnable
        /// </summary>
        void RegisterReceiver();

        /// <summary>
        /// Unregisters the receiver with Core GameKit. Call this in OnDisable
        /// </summary>
        void UnregisterReceiver();
    }
}
/*! \endcond */