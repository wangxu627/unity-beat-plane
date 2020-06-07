using UnityEngine;
using System.Text;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
using DarkTonic.CoreGameKit;
	using UnityEngine.UI;
#endif

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
namespace DarkTonic.CoreGameKit {
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("Dark Tonic/Core GameKit/Listeners/World Variable Listener")]
	// ReSharper disable once CheckNamespace
		public class WorldVariableListener : MonoBehaviour {
	    // ReSharper disable InconsistentNaming
		public string variableName = "";
		public WorldVariableTracker.VariableType vType = WorldVariableTracker.VariableType._integer;
		public bool displayVariableName = false;
		public int decimalPlaces = 1;
		public bool useCommaFormatting = true;
		public bool useFixedNumberOfDigits;
		public int fixedDigitCount = 8;
		// ReSharper restore InconsistentNaming
		
		private int _variableValue;   
		private float _variableFloatValue;
		
		private Text _text;
		
	    // ReSharper disable once UnusedMember.Local
		void Awake() {
			_text = GetComponent<Text>();
		}

		void OnEnable() {
			WorldVariableTracker.UpdateAllListeners();
		}

		public virtual void UpdateValue(int newValue, int oldVal) {
			_variableValue = newValue;
			var valFormatted = new StringBuilder(string.Format("{0}{1}", displayVariableName ? variableName + ": " : "", _variableValue.ToString("N0")));
			
			if (!useCommaFormatting) {
				valFormatted = valFormatted.Replace(",", "");
			}
			
			if (_text == null || !SpawnUtility.IsActive(_text.gameObject)) {
				return;
			}

			if (useFixedNumberOfDigits) {
				while(valFormatted.Length < fixedDigitCount) {
					valFormatted.Insert(0, "0");
				}
			}

			_text.text = valFormatted.ToString();
		}

        public virtual void UpdateFloatValue(float newValue, float oldVal) {
			_variableFloatValue = newValue;
			var valFormatted = new StringBuilder(string.Format("{0}{1}", displayVariableName ? variableName + ": " : "", _variableFloatValue.ToString("N" + decimalPlaces)));
			
			if (!useCommaFormatting) {
				valFormatted = valFormatted.Replace(",", "");
			}

			if (useFixedNumberOfDigits) {
				while(valFormatted.Length < fixedDigitCount) {
					valFormatted.Insert(0, "0");
				}
			}

			_text.text = valFormatted.ToString();
		}
	}
}
#else

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used to listen to key events in a World Variable. Always make a subclass so you can have different Listeners for different World Variables.
    /// </summary>
    [AddComponentMenu("Dark Tonic/Core GameKit/Listeners/World Variable Listener")]
    // ReSharper disable once CheckNamespace
    public class WorldVariableListener : MonoBehaviour {
        /*! \cond PRIVATE */        
        // ReSharper disable InconsistentNaming
        public string variableName = "";
        public WorldVariableTracker.VariableType vType = WorldVariableTracker.VariableType._integer;
        public int decimalPlaces = 1;
        public bool useCommaFormatting = true;
		public bool useFixedNumberOfDigits;
		public int fixedDigitCount = 8;
		public int xStart = 50; // ALSO delete this when you get rid of the OnGUI section. You won't need it.
        // ReSharper restore InconsistentNaming
        /*! \endcond */

        private int _variableValue;
        private float _variableFloatValue;

        // ReSharper disable once UnusedMember.Local
        private void Reset() {
            var src = GetComponent<WorldVariable>();
            if (src == null) {
                return;
            }
            src.listenerPrefab = this;
            variableName = name;
        }

        /// <summary>
        /// This method gets called when the value of an integer World Variable is changing.
        /// </summary>
        /// <param name="newValue">The new value of the variable.</param>
        /// <param name="oldValue">The old value of the variable.</param>
        public virtual void UpdateValue(int newValue, int oldValue) {
            _variableValue = newValue;
        }

        /// <summary>
        /// This method gets called when the value of a float World Variable is changing.
        /// </summary>
        /// <param name="newValue">The new value of the variable.</param>
        /// <param name="oldValue">The old value of the variable.</param>
        public virtual void UpdateFloatValue(float newValue, float oldValue) {
            _variableFloatValue = newValue;
        }

        // This is just used for illustrative purposes. You might replace this with code to update a non-Unity GUI text element. If you use NGUI, please install the optional package "NGUI_CoreGameKit" to get an NGUI version of this script, replacing this one.
        // ReSharper disable once UnusedMember.Local
        private void OnGUI() {
            StringBuilder valFormatted = new StringBuilder();
            switch (vType) {
                case WorldVariableTracker.VariableType._integer:
                    valFormatted.Append(_variableValue.ToString("N0"));
                    

					if (!useCommaFormatting) {
                        valFormatted = valFormatted.Replace(",", "");
                    }

					if (useFixedNumberOfDigits) {
						while(valFormatted.Length < fixedDigitCount) {
							valFormatted.Insert(0, "0");
						}
					}

                    GUI.Label(new Rect(xStart, 120, 180, 40), variableName + ": " + valFormatted);
                    break;
                case WorldVariableTracker.VariableType._float:
                    valFormatted.Append(_variableFloatValue.ToString("N" + decimalPlaces));
                    if (!useCommaFormatting) {
                        valFormatted = valFormatted.Replace(",", "");
                    }

					if (useFixedNumberOfDigits) {
						while(valFormatted.Length < fixedDigitCount) {
							valFormatted.Insert(0, "0");
						}
					}

					GUI.Label(new Rect(xStart, 120, 180, 40), variableName + ": " + valFormatted);
                    break;
                default:
                    LevelSettings.LogIfNew("Add code for varType: " + vType.ToString());
                    break;
            }
        }
    }
}

#endif