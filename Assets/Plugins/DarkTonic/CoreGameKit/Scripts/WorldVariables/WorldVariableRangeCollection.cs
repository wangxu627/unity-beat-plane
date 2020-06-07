/*! \cond PRIVATE */
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WorldVariableRangeCollection {
        // ReSharper disable InconsistentNaming
        public List<WorldVariableRange> statMods = new List<WorldVariableRange>();
        // ReSharper restore InconsistentNaming

        public void DeleteByIndex(int index) {
            statMods.RemoveAt(index);
        }

        public bool HasKey(string key) {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < statMods.Count; i++) {
                if (statMods[i]._statName == key) {
                    return true;
                }
            }

            return false;
        }
    }
}
/*! \endcond */