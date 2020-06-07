/*! \cond PRIVATE */
using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [Serializable]
    // ReSharper disable once CheckNamespace
    public class WorldVariable : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        public bool isExpanded = true;
        public WorldVariableTracker.VariableType varType = WorldVariableTracker.VariableType._integer;
        public bool allowNegative = false;
        public bool canEndGame = false;
        public bool hasMaxValue = false;

        public int startingValue = 0;
        public int endGameMinValue = 0;
        public int endGameMaxValue = 0;
        public int prospectiveValue = 0;
        public int intMaxValue = 100;

        public float startingValueFloat = 0f;
        public float endGameMinValueFloat = 0f;
        public float endGameMaxValueFloat = 0f;
        public float prospectiveFloatValue = 0f;
        public float floatMaxValue = 100f;

        public StatPersistanceMode persistanceMode = StatPersistanceMode.ResetToStartingValue;
        public WorldVariableListener listenerPrefab;
        public VariableChangeMode changeMode = VariableChangeMode.Any;

        public bool fireEventsOnChange = false;
        public List<CGKCustomEventToFire> changeCustomEventsToFire = new List<CGKCustomEventToFire>();
        
        // ReSharper restore InconsistentNaming

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            useGUILayout = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            if (listenerPrefab == null) {
                return;
            }
            var variable = WorldVariableTracker.GetWorldVariable(name);
            if (variable == null) {
                return;
            }

            var curVal = variable.CurrentIntValue;
            listenerPrefab.UpdateValue(curVal, curVal);
        }

        public enum StatPersistanceMode {
            ResetToStartingValue,
            KeepFromPrevious
        }

        public enum VariableChangeMode {
            OnlyIncrease,
            OnlyDecrease,
            Any
        }
    }
}
/*! \endcond */