using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBPLAYER || UNITY_WP8 || UNITY_METRO || UNITY_WIIU
// can't compile this class
#else
using PlayerPrefs = PreviewLabs.PlayerPrefs;
#endif

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class can read and write World Variables in the current Scene.
    /// </summary>
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WorldVariableTracker : MonoBehaviour {
        private static Dictionary<string, InGameWorldVariable> _inGamePlayerStats;
        private static bool _doneInitializing;

        /*! \cond PRIVATE */
        // ReSharper disable InconsistentNaming
        public Transform statPrefab;
        public string newVariableName = "NewVariable";
        public bool showNewVarSection = true;
        public VariableType newVarType = VariableType._integer;
        public bool worldVariablesExpanded = true;

        public bool showIntVars = true;
        public bool showFloatVars = true;
        public int frames;

        public enum VariableType {
            _integer,
            _float
        }
        /*! \endcond */

        // ReSharper restore InconsistentNaming

        private static Transform _trans;
        private static WorldVariableTracker _instance;

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            useGUILayout = false;
            Init();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDisable() {
            _inGamePlayerStats = null; // to undo the caching and load the new Scene's variables.
        }

        /*! \cond PRIVATE */
        public static WorldVariableTracker Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }
                _instance = (WorldVariableTracker)FindObjectOfType(typeof(WorldVariableTracker));

                return _instance;
            }
        }
        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            frames++;
        }

        private static void Init() {
            if (_inGamePlayerStats != null) {
                return;
            }

            _doneInitializing = false;

            _inGamePlayerStats = new Dictionary<string, InGameWorldVariable>(StringComparer.OrdinalIgnoreCase);

            if (TrackerTransform == null) {
                return;
            }

            // set up variables for use
            for (var i = 0; i < TrackerTransform.childCount; i++) {
                var oTrans = TrackerTransform.GetChild(i);
                var oStat = oTrans.GetComponent<WorldVariable>();

                if (oStat == null) {
                    LevelSettings.LogIfNew("Transform '" + oTrans.name +
                                           "' under WorldVariables does not contain a WorldVariable script. Please fix this.");
                    continue;
                }

                if (_inGamePlayerStats.ContainsKey(oStat.name)) {
                    LevelSettings.LogIfNew("You have more than one World Variable named '" + oStat.name +
                                           "' in this Scene. Please make sure the names are unique.");
                    continue;
                }

                var newStatTracker = new InGameWorldVariable(oStat, oStat.name, oStat.varType);

                if (Application.isPlaying) {
                    // do not update values when we're in edit mode!
                    switch (oStat.persistanceMode) {
                        case WorldVariable.StatPersistanceMode.ResetToStartingValue:
                            switch (oStat.varType) {
                                case VariableType._integer:
                                    newStatTracker.CurrentIntValue = oStat.startingValue;
                                    break;
                                case VariableType._float:
                                    newStatTracker.CurrentFloatValue = oStat.startingValueFloat;
                                    break;
                            }
                            break;
                        case WorldVariable.StatPersistanceMode.KeepFromPrevious:
                            // set to value in player prefs	
                            break;
                    }

                    if (oStat.listenerPrefab != null) {
                        var variable = GetExistingWorldVariableIntValue(oStat.name, oStat.startingValue);
                        if (variable != null) {
                            oStat.listenerPrefab.UpdateValue(variable.Value, variable.Value);
                        }
                    }
                }

                _inGamePlayerStats.Add(oStat.name, newStatTracker);
            }

            _doneInitializing = true;
        }

        /// <summary>
        /// Check at runtime whether the World Variable exists in this Scene.
        /// </summary>
        /// <param name="statName">World Variable name</param>
        /// <returns>boolean</returns>
        public static bool VariableExistsInScene(string statName) {
            return InGamePlayerStats.ContainsKey(statName);
        }

        /*! \cond PRIVATE */
        public static bool IsBlankVariableName(string statName) {
            return LevelSettings.IllegalVariableNames.Contains(statName);
        }
        /*! \endcond */

		/// <summary>
		/// Will re-initialize all World Variables to their starting value (only for those marked as "Reset To Starting Value"). Called when using LevelSettings.RestartGame and LevelSettings.ContinueGame();
		/// </summary>
		public static void ForceReInit() {
			_inGamePlayerStats = null;
			Init();
		}

        /// <summary>
        /// Modifies a World Variable by name. You can set, add, multiply or subtract the value.
        /// </summary>
        /// <param name='modifier'>Modifier.</param>
        /// <param name='sourceTrans'>Source trans. Optional - this will output in the debug message if the World Variable is not found.</param>
        public static void ModifyPlayerStat(WorldVariableModifier modifier, Transform sourceTrans = null) {
            var statName = modifier._statName;

            if (!InGamePlayerStats.ContainsKey(statName)) {
                LevelSettings.LogIfNew(
                    string.Format(
                        "Transform '{0}' tried to modify a World Variable called '{1}', which was not found in this scene.",
                        sourceTrans == null ? LevelSettings.EmptyValue : sourceTrans.name,
                        statName));

                return;
            }

            var stat = InGamePlayerStats[statName];

            switch (modifier._varTypeToUse) {
                case VariableType._integer:
                case VariableType._float:
                    stat.ModifyVariable(modifier);
                    break;
                default:
                    LevelSettings.LogIfNew("Write code for varType: " + modifier._varTypeToUse.ToString());
                    break;
            }

        }

        /// <summary>
        /// This returns an instance of the WorldVariable at runtime.
        /// </summary>
        /// <param name="statName">The World Variable name.</param>
        /// <returns>InGameWorldVariable object</returns>
        public static InGameWorldVariable GetWorldVariable(string statName) {
            if (InGamePlayerStats.ContainsKey(statName)) {
                return InGamePlayerStats[statName];
            }
            if (statName == LevelSettings.DropDownNoneOption) {
                // don't log here.
            } else {
                LevelSettings.LogIfNew("Could not find World Variable '" + statName + "'.");
            }
            return null;
        }

        /// <summary>
        /// Gets the existing world variable value. This should only be used in startup code. Otherwise grab the variable from GetWorldVariable
        /// </summary>
        /// <returns>
        /// The existing World Variable value in PlayerPrefs.
        /// </returns>
        /// <param name='variableName'>World Variable name.</param>
        /// <param name="startingValue">The value to use if not present.</param>
        public static int? GetExistingWorldVariableIntValue(string variableName, int startingValue) {
            var tokenKey = InGameWorldVariable.GetTokenPrefsKey(variableName);

            // save this if we need it later.
            if (!VariableExistsInScene(variableName)) {
                return null;
            }

            if (!PlayerPrefs.HasKey(tokenKey)) {
                // set it if this is the first time!!
                PlayerPrefs.SetInt(tokenKey, startingValue);
            }

            return PlayerPrefs.GetInt(tokenKey);
        }

        /// <summary>
        /// Gets the existing world variable value. This should only be used in startup code. Otherwise grab the variable from GetWorldVariable
        /// </summary>
        /// <returns>
        /// The existing World Variable value in PlayerPrefs.
        /// </returns>
        /// <param name='variableName'>World Variable name.</param>
        /// <param name="startingValue">The value to use if not present.</param>
        public static float? GetExistingWorldVariableFloatValue(string variableName, float startingValue) {
            var tokenKey = InGameWorldVariable.GetTokenPrefsKey(variableName);

            // save this if we need it later.
            if (!VariableExistsInScene(variableName)) {
                return null;
            }

            if (!PlayerPrefs.HasKey(tokenKey)) {
                // if this is the first time, set it!
                PlayerPrefs.SetFloat(tokenKey, startingValue);
            }

            return PlayerPrefs.GetFloat(tokenKey);
        }

        /*! \cond PRIVATE */
		/// <summary>
		/// Called internall when a World Variable Listener becomes enabled, so that the Listeners will show the current value without having to wait for an "update" of the World Variable.
		/// </summary>
		public static void UpdateAllListeners() {
			if (!_doneInitializing) {
				return;
			}
			
			foreach (var key in InGamePlayerStats.Keys) {
				var variable = InGamePlayerStats[key];
				switch (variable.VariableType) {
				case VariableType._float:
					InGamePlayerStats[key].CurrentFloatValue = InGamePlayerStats[key].CurrentFloatValue;
					break;
				case VariableType._integer:
					InGamePlayerStats[key].CurrentIntValue = InGamePlayerStats[key].CurrentIntValue;
					break;
				}
			}
		}

		public static string GetVariableTypeFriendlyString(VariableType varType) {
            var vType = varType.ToString();
            vType = vType.Substring(1);
            vType = vType.Substring(0, 1).ToUpper() + vType.Substring(1);

            return vType;
        }

        public static WorldVariable GetWorldVariableScript(string varName) {
            var ls = LevelSettings.Instance;
            if (ls == null) {
                return null;
            }

            var statsHolder = ls.transform.Find(LevelSettings.WorldVariablesContainerTransName);
            var varTrans = statsHolder.GetChildTransform(varName);

            if (varTrans == null) {
                return null;
            }

            return varTrans.GetComponent<WorldVariable>();
        }

        public static void LogIfInvalidWorldVariable(string worldVariableName, Transform trans, string fieldName,
            int? levelNum = null, int? waveNum = null, string trigEventName = null) {
            if (LevelSettings.IllegalVariableNames.Contains(worldVariableName)) {
                if (!string.IsNullOrEmpty(trigEventName)) {
                    LevelSettings.LogIfNew(
                        string.Format("The '{0}' field in '{1}' has no Variable assigned for event '{2}'.",
                            fieldName, trans.name, trigEventName));
                } else if (levelNum.HasValue && waveNum.HasValue) {
                    LevelSettings.LogIfNew(
                        string.Format("The '{0}' field in '{1}' has no Variable assigned for Level {2} Wave {3}.",
                            fieldName, trans.name, levelNum.Value + 1, waveNum.Value + 1));
                } else {
                    LevelSettings.LogIfNew(string.Format("The '{0}' field in '{1}' has no Variable assigned.",
                        fieldName, trans.name));
                }
            } else {
                if (!string.IsNullOrEmpty(trigEventName)) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "The '{0}' field in '{1}' is using an invalid Variable '{2} for event '{3}'. That variable is not in the Scene.",
                            fieldName,
                            trans.name,
                            worldVariableName,
                            trigEventName));
                } else if (levelNum.HasValue && waveNum.HasValue) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "The '{0}' field in '{1}' is using an invalid Variable '{2}' for Level {3} Wave {4}. That variable is not in the Scene",
                            fieldName, trans.name, worldVariableName, levelNum.Value + 1, waveNum.Value + 1));
                } else {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "The '{0}' field in '{1}' is using an invalid Variable '{2}' That variable is not in the Scene.",
                            fieldName, trans.name, worldVariableName));
                }
            }
        }
        /*! \endcond */

        /// <summary>
        /// A list of all World Variables in the current Scene
        /// </summary>
        public static Dictionary<string, InGameWorldVariable> InGamePlayerStats {
            // for lazy lookup
            get {
                if (_inGamePlayerStats == null) {
                    Init();
                }

                return _inGamePlayerStats;
            }
        }

        /// <summary>
        /// Deletes the list holding all World Variables. Used by Inspectors only to avoid caching and showing the wrong thing. Do not call this.
        /// </summary>
        public static void ClearInGamePlayerStats() {
            if (Application.isPlaying) {
                return; // should NEVER happen while playing.
            }

            _inGamePlayerStats = null;
        }

        /*! \cond PRIVATE */
        public static Transform TrackerTransform {
            // for lazy lookup
            get {
                if (_trans != null) {
                    return _trans;
                }
                var track = Instance;
                if (track != null) {
                    _trans = track.transform;
                }

                return _trans;
            }
        }

        public static bool IsInitializing {
            get { return !_doneInitializing; }
        }
        /*! \endcond */

        /// <summary>
        /// This method will flush all World Variable values and write them to a text file for later retrieval. Normally not needed. If you find your variable values get lost, call this before changing Scenes. I've never seen it happen myself.
        /// </summary>
        public static void FlushAll() {
#if UNITY_WEBPLAYER || UNITY_WP8 || UNITY_METRO || UNITY_WIIU
    // can't compile this class
#else
            PlayerPrefs.Flush();
#endif
        }
    }
}