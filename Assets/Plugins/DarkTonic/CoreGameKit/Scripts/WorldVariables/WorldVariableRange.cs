/*! \cond PRIVATE */
using System;

// ReSharper disable once CheckNamespace

namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WorldVariableRange {
        // ReSharper disable InconsistentNaming
        public int _modValueIntMin;
        public int _modValueIntMax;

        public float _modValueFloatMin;
        public float _modValueFloatMax;
        public string _statName;

        // ReSharper disable once MemberInitializerValueIgnored
        public WorldVariableTracker.VariableType _varTypeToUse = WorldVariableTracker.VariableType._integer;
        // ReSharper restore InconsistentNaming


        public WorldVariableRange(string statName, WorldVariableTracker.VariableType vType) {
            _statName = statName;
            _varTypeToUse = vType;
        }
    }
}
/*! \endcond */