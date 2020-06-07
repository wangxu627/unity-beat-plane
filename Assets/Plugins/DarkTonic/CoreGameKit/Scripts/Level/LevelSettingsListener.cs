using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in LevelSettings. Always make a subclass so you can have different Listeners for different LevelSettings.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/Level Settings Listener")]
    // ReSharper disable once CheckNamespace
    public class LevelSettingsListener : MonoBehaviour {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public string sourceTransName;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<LevelSettings>();
            if (src == null) {
                return;
            }
            src.listener = this;
            sourceTransName = name;
        }

        /// <summary>
        /// This method gets called when the number of wave items spawned changes (something spawns or despawns).
        /// </summary>
        /// <param name="waveItemsRemaining"></param>
        public virtual void WaveItemsRemainingChanged(int waveItemsRemaining) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the seconds remaining in a timed wave changes.
        /// </summary>
        public virtual void WaveTimeRemainingChanged(int secondsRemaining) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the player wins (last wave completed and game not over).
        /// </summary>
        public virtual void Win() {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the player loses (game ended from lives = 0 or other game-ending trigger).
        /// </summary>
        public virtual void Lose() {
            // your code here.
        }

        /// <summary>
        /// This method gets called when the player wins or loses.
        /// </summary>
        public virtual void GameOver(bool hasWon) {
            // your code here.
        }

		/// <summary>
		/// This method gets called when a Global Level starts.
		/// </summary>
		public virtual void LevelStarted(int levelNum) {
			// your code here.
		}

        /// <summary>
        /// This method gets called when a Global Level ends, just before the next one starts (if any more).
        /// </summary>
        public virtual void LevelEnded(int levelNum) {
            // your code here.
        }


        /// <summary>
        /// This method gets called when a Global Wave begins, before anything has spawned.
        /// </summary>
        public virtual void WaveStarted(LevelWave levelWaveInfo) {
			// your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave ends, just before the next one starts.
        /// </summary>
        public virtual void WaveEnded(LevelWave levelWaveInfo) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave restarts.
        /// </summary>
        public virtual void WaveRestarted(LevelWave levelWaveInf) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave has been completed and the wave complete bonuses are about to be awarded. You can modify those awards permanently here.
        /// </summary>
        public virtual void WaveCompleteBonusesStart(List<WorldVariableModifier> bonusModifiers) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave has been completed and the wave elimination bonuses are about to be awarded. You can modify those awards permanently here.
        /// </summary>
        public virtual void WaveEliminationBonusesStart(List<WorldVariableModifier> elimModifiers) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave ends early.
        /// </summary>
        public virtual void WaveEndedEarly(LevelWave levelWaveInfo) {
            // your code here.
        }

        /// <summary>
        /// This method gets called when a Global Wave is skipped through Skip Wave criteria or other means.
        /// </summary>
        public virtual void WaveSkipped(LevelWave levelWaveInfo) {
            // your code here.
        }
    }
}