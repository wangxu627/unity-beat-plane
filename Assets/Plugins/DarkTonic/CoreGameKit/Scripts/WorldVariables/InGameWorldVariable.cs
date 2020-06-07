using UnityEngine;
using System;
#if UNITY_WEBPLAYER || UNITY_WP8 || UNITY_METRO || UNITY_WIIU
	// can't compile this class as is
#else
using PlayerPrefs = PreviewLabs.PlayerPrefs;
#endif

// ReSharper disable once CheckNamespace

namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class represents a World Variable in the current Scene at runtime. You can get, set or modify the value.
    /// </summary>
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class InGameWorldVariable {
        private const string PlayerPrefStatToken = "~KWStat_{0}~";

        private readonly string _statName;
        private readonly WorldVariable _sourceStat;
        private readonly WorldVariableTracker.VariableType _varType;

        private string _tokenizedPrefsKey = string.Empty;

        /*! \cond PRIVATE */
        public InGameWorldVariable(WorldVariable srcStat, string statName, WorldVariableTracker.VariableType varType) {
            _sourceStat = srcStat;
            _statName = statName;
            _varType = varType;
        }
        /*! \endcond */

        /*! \cond PRIVATE */
        public WorldVariableTracker.VariableType VariableType {
            get { return _varType; }
        }
        /*! \endcond */

        /// <summary>
        /// This property returns the integer value that the World Variable currently holds.
        /// </summary>
        public int CurrentIntValue {
            get {
                if (!PlayerPrefs.HasKey(TokenizedPrefsKey)) {
                    PlayerPrefs.SetInt(TokenizedPrefsKey, _sourceStat.startingValue);
                }

                return PlayerPrefs.GetInt(TokenizedPrefsKey);
            }
            set {
                if (_sourceStat.hasMaxValue && value > _sourceStat.intMaxValue) {
                    value = _sourceStat.intMaxValue;
                }

                if (value < 0 && !_sourceStat.allowNegative) {
                    value = 0;
                }

                var oldVal = CurrentIntValue;

                PlayerPrefs.SetInt(TokenizedPrefsKey, value);
                 
                UpdateIntValue(value, oldVal);

                EndGameIfIntInRange();
            }
        }

        private void FireCustomEvents() {
            if (!_sourceStat.fireEventsOnChange) {
                return;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _sourceStat.changeCustomEventsToFire.Count; i++) {
                var anEvent = _sourceStat.changeCustomEventsToFire[i].CustomEventName;

                LevelSettings.FireCustomEventIfValid(anEvent, LevelSettings.Instance.transform);
            }
        }

        /// <summary>
        /// This property returns the float value that the World Variable currently holds.
        /// </summary>
        public float CurrentFloatValue {
            get {
                if (!PlayerPrefs.HasKey(TokenizedPrefsKey)) {
                    PlayerPrefs.SetFloat(TokenizedPrefsKey, _sourceStat.startingValueFloat);
                }

                return PlayerPrefs.GetFloat(TokenizedPrefsKey);
            }
            set {
                if (_sourceStat.hasMaxValue && value > _sourceStat.floatMaxValue) {
                    value = _sourceStat.floatMaxValue;
                }

                if (value < 0f && !_sourceStat.allowNegative) {
                    value = 0f;
                }

                var oldVal = CurrentFloatValue;

                PlayerPrefs.SetFloat(TokenizedPrefsKey, value);

                UpdateFloatValue(value, oldVal);

                EndGameIfFloatInRange();
            }
        }

        /*! \cond PRIVATE */
        public void ModifyVariable(WorldVariableModifier mod) {
            switch (mod._varTypeToUse) {
                case WorldVariableTracker.VariableType._integer:
                    var modVal = mod._modValueIntAmt.Value;

                    switch (mod._modValueIntAmt.curModMode) {
                        case KillerVariable.ModMode.Add:
                            AddToIntValue(modVal);
                            break;
                        case KillerVariable.ModMode.Set:
                            SetIntValueIfAllowed(modVal);
                            AddToIntValue(0); // need to trigger game over if G.O. value reached
                            break;
                        case KillerVariable.ModMode.Sub:
                            AddToIntValue(-modVal);
                            break;
                        case KillerVariable.ModMode.Mult:
                            MultiplyByIntValue(modVal);
                            break;
                        default:
                            LevelSettings.LogIfNew("Add code for modMode: " + mod._modValueIntAmt.curModMode.ToString());
                            break;
                    }

                    break;
                case WorldVariableTracker.VariableType._float:
                    var modFloatVal = mod._modValueFloatAmt.Value;

                    switch (mod._modValueFloatAmt.curModMode) {
                        case KillerVariable.ModMode.Add:
                            AddToFloatValue(modFloatVal);
                            break;
                        case KillerVariable.ModMode.Set:
                            SetFloatValueIfAllowed(modFloatVal);
                            AddToFloatValue(0f);
                            break;
                        case KillerVariable.ModMode.Sub:
                            AddToFloatValue(-modFloatVal);
                            break;
                        case KillerVariable.ModMode.Mult:
                            MultiplyByFloatValue(modFloatVal);
                            break;
                        default:
                            LevelSettings.LogIfNew("Add code for modMode: " + mod._modValueIntAmt.curModMode.ToString());
                            break;
                    }

                    break;
                default:
                    LevelSettings.LogIfNew("Add code for varType: " + mod._varTypeToUse.ToString());
                    break;
            }
        }
        /*! \endcond */

        private void EndGameIfIntInRange() {
            if (!_sourceStat.canEndGame) {
                return;
            }
            if (CurrentIntValue >= _sourceStat.endGameMinValue && CurrentIntValue <= _sourceStat.endGameMaxValue) {
                LevelSettings.IsGameOver = true;
            }
        }

        private void EndGameIfFloatInRange() {
            if (!_sourceStat.canEndGame) {
                return;
            }
            if (CurrentFloatValue >= _sourceStat.endGameMinValue && CurrentFloatValue <= _sourceStat.endGameMaxValue) {
                LevelSettings.IsGameOver = true;
            }
        }

        /*! \cond PRIVATE */
        public void SetIntValueIfAllowed(int newVal) {
            switch (_sourceStat.changeMode) {
                case WorldVariable.VariableChangeMode.Any:
                    break;
                case WorldVariable.VariableChangeMode.OnlyDecrease:
                    if (newVal > CurrentIntValue) {
                        return; // not allowed
                    }
                    break;
                case WorldVariable.VariableChangeMode.OnlyIncrease:
                    if (newVal < CurrentIntValue) {
                        return; // not allowed
                    }
                    break;
            }

            CurrentIntValue = newVal;
        }

        public void SetFloatValueIfAllowed(float newVal) {
            switch (_sourceStat.changeMode) {
                case WorldVariable.VariableChangeMode.Any:
                    break;
                case WorldVariable.VariableChangeMode.OnlyDecrease:
                    if (newVal > CurrentFloatValue) {
                        return; // not allowed
                    }
                    break;
                case WorldVariable.VariableChangeMode.OnlyIncrease:
                    if (newVal < CurrentFloatValue) {
                        return; // not allowed
                    }
                    break;
            }

            CurrentFloatValue = newVal;
        }
        /*! \endcond */

        private void UpdateFloatValue(float val, float oldVal) {
            FireCustomEvents();

            if (_sourceStat.listenerPrefab != null) {
                _sourceStat.listenerPrefab.UpdateFloatValue(val, oldVal);
            }
        }

        private void UpdateIntValue(int val, int oldVal) {
            FireCustomEvents();

            if (_sourceStat.listenerPrefab != null) {
                _sourceStat.listenerPrefab.UpdateValue(val, oldVal);
            }
        }

        private void AddToIntValue(int valueToAdd) {
            var oldVal = CurrentIntValue;
            SetIntValueIfAllowed(CurrentIntValue + valueToAdd);

            EndGameIfIntInRange();

            UpdateIntValue(CurrentIntValue, oldVal);
        }

        private void MultiplyByIntValue(int valueToMultiplyBy) {
            var oldVal = CurrentIntValue;
            SetIntValueIfAllowed(CurrentIntValue * valueToMultiplyBy);

            EndGameIfIntInRange();

            UpdateIntValue(CurrentIntValue, oldVal);
        }

        private void AddToFloatValue(float valueToAdd) {
            var oldVal = CurrentFloatValue;
            SetFloatValueIfAllowed(CurrentFloatValue + valueToAdd);

            EndGameIfFloatInRange();

            UpdateFloatValue(CurrentFloatValue, oldVal);
        }

        private void MultiplyByFloatValue(float valueToMultiplyBy) {
            var oldVal = CurrentFloatValue;
            SetFloatValueIfAllowed(CurrentFloatValue * valueToMultiplyBy);

            EndGameIfFloatInRange();

            UpdateFloatValue(CurrentFloatValue, oldVal);
        }

        /*! \cond PRIVATE */
        private string TokenizedPrefsKey {
            get {
                if (_tokenizedPrefsKey == string.Empty) {
                    _tokenizedPrefsKey = GetTokenPrefsKey(_statName);
                }

                return _tokenizedPrefsKey;
            }
        }

        public static string GetTokenPrefsKey(string myStatName) {
            return string.Format(PlayerPrefStatToken, myStatName);
        }
    }
}