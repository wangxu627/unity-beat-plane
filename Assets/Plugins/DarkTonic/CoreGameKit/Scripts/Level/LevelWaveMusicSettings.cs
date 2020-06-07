/*! \cond PRIVATE */
using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class LevelWaveMusicSettings {
        public LevelSettings.WaveMusicMode WaveMusicMode = LevelSettings.WaveMusicMode.PlayNew;
        public AudioClip WaveMusic;
        public float WaveMusicVolume = 1f;
        public float FadeTime = 2f;
    }
}
/*! \endcond */