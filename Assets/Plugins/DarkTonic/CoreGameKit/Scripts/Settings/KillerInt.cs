using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to hold any integer field in Core GameKit's Inspector's. You can either type an int value or choose a WorldVariable.
    /// </summary>
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class KillerInt : KillerVariable {
        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public int selfValue;
        public int minimum;
        public int maximum;
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        private bool _isValid = true;

        /*! \cond PRIVATE */
        public KillerInt(int startingValue)
            : this(startingValue, int.MinValue, int.MaxValue) {
        }

        public KillerInt(int startingValue, int min, int max) {
            selfValue = startingValue;
            minimum = min;
            maximum = max;
        }

        public int LogIfInvalid(Transform trans, string fieldName, int? levelNum = null, int? waveNum = null,
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
        /// This will get or set the value of a Killer Int, which is either the value of the selected World Variable or the entered int. If this field is set to a World Variable, you cannot set it.
        /// </summary>
        public int Value {
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
					 
						varVal = variable.CurrentIntValue;
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
        public static int DefaultValue {
            get { return default(int); }
        }
        /*! \endcond */
    }
}