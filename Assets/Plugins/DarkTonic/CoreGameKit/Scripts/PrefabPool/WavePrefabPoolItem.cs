/*! \cond PRIVATE */
using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WavePrefabPoolItem {
        // ReSharper disable InconsistentNaming
        public Transform prefabToSpawn;
		public string prefabPoolBossCategory;
		public LevelSettings.ActiveItemMode activeMode = LevelSettings.ActiveItemMode.Always;
        public WorldVariableRangeCollection activeItemCriteria = new WorldVariableRangeCollection();
        public KillerInt thisWeight = new KillerInt(1, 0, 256);
        public bool isExpanded = true;
        // ReSharper restore InconsistentNaming
    }
}
/*! \endcond */