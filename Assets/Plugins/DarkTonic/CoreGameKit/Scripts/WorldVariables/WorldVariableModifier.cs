/*! \cond PRIVATE */
using System;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WorldVariableModifier {
        // ReSharper disable InconsistentNaming
        public string _statName;
        public KillerInt _modValueIntAmt = new KillerInt(0, int.MinValue, int.MaxValue);
        public KillerFloat _modValueFloatAmt = new KillerFloat(0f, float.MinValue, float.MaxValue);
        // ReSharper disable once MemberInitializerValueIgnored
        public WorldVariableTracker.VariableType _varTypeToUse = WorldVariableTracker.VariableType._integer;
        // ReSharper restore InconsistentNaming

        public WorldVariableModifier(string statName, WorldVariableTracker.VariableType vType) {
            _statName = statName;
            _varTypeToUse = vType;
        }
    }
}
/*! \endcond */