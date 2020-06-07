/*! \cond PRIVATE */
using System;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class KillerVariable {
        // ReSharper disable InconsistentNaming
        public LevelSettings.VariableSource variableSource = LevelSettings.VariableSource.Value;
        public string worldVariableName = string.Empty;
        public ModMode curModMode = ModMode.Add;
        // ReSharper restore InconsistentNaming

        public enum ModMode {
            Set,
            Add,
            Sub,
            Mult
        }
    }
}
/*! \endcond */