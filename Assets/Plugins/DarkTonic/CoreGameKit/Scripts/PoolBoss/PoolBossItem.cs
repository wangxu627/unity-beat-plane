/*! \cond PRIVATE */
using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class PoolBossItem {
        // ReSharper disable InconsistentNaming
        public Transform prefabTransform;
        public int instancesToPreload = 1;
        public bool isExpanded = true;
        public bool logMessages;
        public bool allowInstantiateMore;
        public bool enableNavMeshAgentOnSpawn = false;
        public int itemHardLimit = 10;
        public bool allowRecycle;
        public string categoryName = PoolBoss.NoCategory;
        // ReSharper restore InconsistentNaming

        public PoolBossItem Clone() {
            var clone = new PoolBossItem {
                prefabTransform = prefabTransform,
                instancesToPreload = instancesToPreload,
                isExpanded = isExpanded,
                logMessages = logMessages,
                allowInstantiateMore = allowInstantiateMore,
                itemHardLimit = itemHardLimit,
                allowRecycle = allowRecycle,
                categoryName = categoryName
            };

            return clone;
        }
    }
}
/*! \endcond */