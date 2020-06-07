/*! \cond PRIVATE */
using System.Collections;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Music/Wave Music Changer")]
    [RequireComponent(typeof(AudioSource))]
    // ReSharper disable once CheckNamespace
    public class WaveMusicChanger : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        public WaveMusicChangerListener listener;
        // ReSharper restore InconsistentNaming

        private static WaveMusicChangerListener _statListener;
        private static AudioSource _statAudio;
        private static bool _isValid;
        private static bool _isFading;
        private static float _fadeStartTime;
        private static float _fadeStartVolume;
        private static float _fadeTotalTime;
        private static float _lastVolume;

        private static readonly YieldInstruction LoopDelay = new WaitForSeconds(.1f);

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _statAudio = GetComponent<AudioSource>();
            _statListener = listener;
            _isFading = false;

            if (_statAudio != null) {
                _isValid = true;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            if (_isValid) {
                StartCoroutine(CoUpdate());
            }
        }

        private static IEnumerator CoUpdate() {
            while (true) {
                yield return LoopDelay; // fading interval

                if (!_isFading || !_statAudio.isPlaying) {
                    continue; // nothing to do.
                }

                _statAudio.volume = _fadeStartVolume * (_fadeTotalTime - (Time.time - _fadeStartTime)) / _fadeTotalTime;

                var volDelta = _lastVolume - _statAudio.volume;

                if (_statAudio.volume <= volDelta) {
                    _isFading = false;
                    _statAudio.Stop();
                }

                _lastVolume = _statAudio.volume;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static void WaveUp(LevelWaveMusicSettings newWave) {
            PlayMusic(newWave);
        }

        private static void PlayMusic(LevelWaveMusicSettings musicSettings) {
            if (!_isValid) {
                LevelSettings.LogIfNew(
                    "WaveMusicChanger is not attached to any prefab with an AudioSource component. Music in Core GameKit LevelSettings will not play.");
                return;
            }

            if (_statListener != null) {
                _statListener.MusicChanging(musicSettings);
            }

            _isFading = false;

            switch (musicSettings.WaveMusicMode) {
                case LevelSettings.WaveMusicMode.PlayNew:
                    _statAudio.Stop();
                    _statAudio.clip = musicSettings.WaveMusic;
                    _statAudio.volume = musicSettings.WaveMusicVolume;
                    _statAudio.Play();
                    break;
                case LevelSettings.WaveMusicMode.Silence:
                    _isFading = true;
                    _fadeStartTime = Time.time;
                    _fadeStartVolume = _statAudio.volume;
                    _fadeTotalTime = musicSettings.FadeTime;
                    break;
                case LevelSettings.WaveMusicMode.KeepPreviousMusic:
                    _statAudio.volume = musicSettings.WaveMusicVolume;
                    break;
            }
        }

        public static void PlayGameOverMusic(LevelWaveMusicSettings musicSettings) {
            PlayMusic(musicSettings);
        }

        /// <summary>
        /// Mutes the music.
        /// </summary>
        public static void MuteMusic() {
            if (_statAudio.clip != null && _statAudio.isPlaying) {
                _statAudio.mute = true;
            }
        }

        /// <summary>
        /// Unmutes the music.
        /// </summary>
        public static void UnmuteMusic() {
            if (_statAudio.clip != null && _statAudio.isPlaying) {
                _statAudio.mute = false;
            }
        }
    }
}
/*! \endcond */