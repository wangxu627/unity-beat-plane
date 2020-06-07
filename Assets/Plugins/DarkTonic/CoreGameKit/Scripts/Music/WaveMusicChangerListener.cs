using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in the Music Changer. Always make a subclass so you can have different Listeners for different Music Changers.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Wave Music Changer Listener")]
    // ReSharper disable once CheckNamespace
    public class WaveMusicChangerListener : MonoBehaviour {
        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<WaveMusicChanger>();
            if (src != null) {
                src.listener = this;
            }
        }

        /// <summary>
        /// This method gets called when the music will change to a different song.
        /// </summary>
        /// <param name="musicSettings">The settings for the new music.</param>
        public virtual void MusicChanging(LevelWaveMusicSettings musicSettings) {
            // your code here.
        }
    }
}