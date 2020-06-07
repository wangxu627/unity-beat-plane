using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to hold any float field in Core GameKit's Inspector's. You can either type a float value or choose a WorldVariable.
    /// </summary>
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class KillerFloat : KillerVariable {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public float selfValue;
        public float minimum;
        public float maximum;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        private bool _isValid = true;

        /*! \cond PRIVATE */
        public KillerFloat(float startingValue)
            : this(startingValue, float.MinValue, float.MaxValue) {
        }

        public KillerFloat(float startingValue, float min, float max) {
            selfValue = startingValue;
            minimum = min;
            maximum = max;
        }

        public float LogIfInvalid(Transform trans, string fieldName, int? levelNum = null, int? waveNum = null,
            string trigEventName = null) {
            var val = Value; // trigger Valid or not evaluation

            if (_isValid) {
                return val;
            }

            WorldVariableTracker.LogIfInvalidWorldVariable(worldVariableName, trans, fieldName, levelNum, waveNum,
                trigEventName);

            return val;
        }
        /*! \endcond */

        /// <summary>
        /// This will get or set the value of a Killer Float, which is either the value of the selected World Variable or the entered float. If this field is set to a World Variable, you cannot set it.
        /// </summary>
        public float Value {
            get {
                var varVal = DefaultValue;
                _isValid = true;

                switch (variableSource) {
                    case LevelSettings.VariableSource.Value:
                        varVal = selfValue;
                        break;
                    case LevelSettings.VariableSource.Variable:
                        if (LevelSettings.IllegalVariableNames.Contains(worldVariableName)) {
                            _isValid = false;
                            break;
                        }
                        var variable = WorldVariableTracker.GetWorldVariable(worldVariableName);
                        if (variable == null) {
                            _isValid = false;
                            break;
                        }

                        varVal = variable.CurrentFloatValue;
                        break;
                    default:
                        LevelSettings.LogIfNew("Unknown VariableSource: " + variableSource.ToString());
                        break;
                }

                return Math.Min(varVal, maximum);
            }
            set {
                switch (variableSource) {
                    case LevelSettings.VariableSource.Value:
                        var newVal = Math.Min(value, maximum);
                        newVal = Math.Max(newVal, minimum);
                        selfValue = newVal;
                        break;
                    default:
                        LevelSettings.LogIfNew("Cannot set KillerInt with source of: " + variableSource.ToString());
                        break;
                }
            }
        }

        /*! \cond PRIVATE */
        public static float DefaultValue {
            get { return default(float); }
        }
        /*! \endcond */
    }
}