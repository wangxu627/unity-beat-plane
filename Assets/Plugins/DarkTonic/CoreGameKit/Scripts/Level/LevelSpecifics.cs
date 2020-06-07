/*! \cond PRIVATE */
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class LevelSpecifics {
        // ReSharper disable InconsistentNaming
        public string levelName = "UNNAMED";
        public LevelSettings.WaveOrder waveOrder = LevelSettings.WaveOrder.SpecifiedOrder;
        public List<LevelWave> WaveSettings = new List<LevelWave>();
        public bool isExpanded = true;
        // ReSharper restore InconsistentNaming
    }
}
/*! \endcond */
