using System;
using System.Collections.Generic;

/*! \cond PRIVATE */
// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class CgkCustomEvent {
        public string EventName;
        public string ProspectiveName;
		public bool IsEditing;
		// ReSharper disable InconsistentNaming
        public bool eventExpanded = true;
        public LevelSettings.EventReceiveMode eventRcvMode = LevelSettings.EventReceiveMode.Always;
        public LevelSettings.EventReceiveFilter eventRcvFilterMode = LevelSettings.EventReceiveFilter.All;
		public KillerInt filterModeQty = new KillerInt(1, 1, 10000);
        public KillerFloat distanceThreshold = new KillerFloat(10, 0.1f, float.MaxValue);
        public int frameLastFired = -1;
        public CgkCustomEventsFireDuringFrame customEventsDuringFrame = null;

		public string categoryName = LevelSettings.NoCategory;
        // ReSharper restore InconsistentNaming

        public CgkCustomEvent(string eventName) {
            EventName = eventName;
            ProspectiveName = eventName;
        }
    }

    [Serializable]
    public class CgkCustomEventsFireDuringFrame {
        public int FrameNumber;
        public HashSet<int> CustomEventHashes;
    }
}
/*! \endcond */