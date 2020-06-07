using System.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endif

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    /// <summary>
    /// This class is used for Triggered Spawner setup.
    /// </summary>
    [CoreScriptOrder(-100)]
    [AddComponentMenu("Dark Tonic/Core GameKit/Spawners/Triggered Spawner V2")]
    // ReSharper disable once CheckNamespace
    public class TriggeredSpawnerV2 : MonoBehaviour, ICgkEventReceiver {
        /*! \cond PRIVATE */
        public const int MaxDistance = 5000;

        // ReSharper disable InconsistentNaming
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        public TriggeredSpawner.Unity_UIVersion unityUIMode = TriggeredSpawner.Unity_UIVersion.uGUI;
#else
		public TriggeredSpawner.Unity_UIVersion unityUIMode = TriggeredSpawner.Unity_UIVersion.Legacy;
#endif

        #region categorizations of event types

        public static List<TriggeredSpawner.EventType> eventsThatCanRepeatWave = new List<TriggeredSpawner.EventType>()
        {
            TriggeredSpawner.EventType.OnEnabled,
            TriggeredSpawner.EventType.Visible,
            TriggeredSpawner.EventType.OnTriggerEnter,
            TriggeredSpawner.EventType.OnTriggerStay,
            TriggeredSpawner.EventType.OnTriggerExit,
            TriggeredSpawner.EventType.MouseClick_Legacy,
            TriggeredSpawner.EventType.MouseOver_Legacy,
            TriggeredSpawner.EventType.OnCollision,
            TriggeredSpawner.EventType.OnSpawned,
            TriggeredSpawner.EventType.CodeTriggered1,
            TriggeredSpawner.EventType.CodeTriggered2,
            TriggeredSpawner.EventType.OnClick_NGUI,
            TriggeredSpawner.EventType.OnCollision2D,
            TriggeredSpawner.EventType.OnTriggerEnter2D,
            TriggeredSpawner.EventType.OnTriggerStay2D,
            TriggeredSpawner.EventType.OnTriggerExit2D,
            TriggeredSpawner.EventType.CustomEvent,
            TriggeredSpawner.EventType.SliderChanged_uGUI,
            TriggeredSpawner.EventType.ButtonClicked_uGUI,
            TriggeredSpawner.EventType.PointerDown_uGUI,
            TriggeredSpawner.EventType.PointerUp_uGUI,
            TriggeredSpawner.EventType.PointerEnter_uGUI,
            TriggeredSpawner.EventType.PointerExit_uGUI,
            TriggeredSpawner.EventType.Drag_uGUI,
            TriggeredSpawner.EventType.Drop_uGUI,
            TriggeredSpawner.EventType.Scroll_uGUI,
            TriggeredSpawner.EventType.UpdateSelected_uGUI,
            TriggeredSpawner.EventType.Select_uGUI,
            TriggeredSpawner.EventType.Deselect_uGUI,
            TriggeredSpawner.EventType.Move_uGUI,
            TriggeredSpawner.EventType.InitializePotentialDrag_uGUI,
            TriggeredSpawner.EventType.BeginDrag_uGUI,
            TriggeredSpawner.EventType.EndDrag_uGUI,
            TriggeredSpawner.EventType.Submit_uGUI,
            TriggeredSpawner.EventType.Cancel_uGUI
        };

        public static List<TriggeredSpawner.EventType> eventsWithTagLayerFilters = new List<TriggeredSpawner.EventType>()
        {
            TriggeredSpawner.EventType.OnCollision,
            TriggeredSpawner.EventType.OnTriggerEnter,
            TriggeredSpawner.EventType.OnTriggerStay,
            TriggeredSpawner.EventType.OnTriggerExit,
            TriggeredSpawner.EventType.OnCollision2D,
            TriggeredSpawner.EventType.OnTriggerEnter2D,
            TriggeredSpawner.EventType.OnTriggerStay2D,
            TriggeredSpawner.EventType.OnTriggerExit2D
        };

        public static List<TriggeredSpawner.EventType> eventsWithInflexibleWaveLength = new List<TriggeredSpawner.EventType>()
        {
            TriggeredSpawner.EventType.Invisible,
            TriggeredSpawner.EventType.OnDespawned,
            TriggeredSpawner.EventType.OnDisabled
        };

        public static List<TriggeredSpawner.EventType> eventsThatCanTriggerDespawn = new List<TriggeredSpawner.EventType>()
        {
            TriggeredSpawner.EventType.MouseClick_Legacy,
            TriggeredSpawner.EventType.MouseOver_Legacy,
            TriggeredSpawner.EventType.OnCollision,
            TriggeredSpawner.EventType.OnTriggerEnter,
            TriggeredSpawner.EventType.OnTriggerStay,
            TriggeredSpawner.EventType.OnTriggerExit,
            TriggeredSpawner.EventType.OnCollision2D,
            TriggeredSpawner.EventType.OnTriggerEnter2D,
            TriggeredSpawner.EventType.OnTriggerStay2D,
            TriggeredSpawner.EventType.OnTriggerExit2D,
            TriggeredSpawner.EventType.Visible,
            TriggeredSpawner.EventType.OnEnabled,
            TriggeredSpawner.EventType.OnSpawned,
            TriggeredSpawner.EventType.CodeTriggered1,
            TriggeredSpawner.EventType.CodeTriggered2,
            TriggeredSpawner.EventType.OnClick_NGUI,
            TriggeredSpawner.EventType.CustomEvent,
            TriggeredSpawner.EventType.SliderChanged_uGUI,
            TriggeredSpawner.EventType.ButtonClicked_uGUI,
            TriggeredSpawner.EventType.PointerDown_uGUI,
            TriggeredSpawner.EventType.PointerUp_uGUI,
            TriggeredSpawner.EventType.PointerEnter_uGUI,
            TriggeredSpawner.EventType.PointerExit_uGUI,
            TriggeredSpawner.EventType.Drag_uGUI,
            TriggeredSpawner.EventType.Drop_uGUI,
            TriggeredSpawner.EventType.Scroll_uGUI,
            TriggeredSpawner.EventType.UpdateSelected_uGUI,
            TriggeredSpawner.EventType.Select_uGUI,
            TriggeredSpawner.EventType.Deselect_uGUI,
            TriggeredSpawner.EventType.Move_uGUI,
            TriggeredSpawner.EventType.InitializePotentialDrag_uGUI,
            TriggeredSpawner.EventType.BeginDrag_uGUI,
            TriggeredSpawner.EventType.EndDrag_uGUI,
            TriggeredSpawner.EventType.Submit_uGUI,
            TriggeredSpawner.EventType.Cancel_uGUI
        };

        #endregion

        #region public variables

        public bool logMissingEvents = true;
        public LevelSettings.ActiveItemMode activeMode = LevelSettings.ActiveItemMode.Always;
        public WorldVariableRangeCollection activeItemCriteria = new WorldVariableRangeCollection();

        public TriggeredSpawner.GameOverBehavior gameOverBehavior = TriggeredSpawner.GameOverBehavior.Disable;
        public TriggeredSpawner.WavePauseBehavior wavePauseBehavior = TriggeredSpawner.WavePauseBehavior.Disable;
        public TriggeredSpawner.SpawnerEventSource eventSourceType = TriggeredSpawner.SpawnerEventSource.Self;
        public bool transmitEventsToChildren = true;
        public bool spawnOutsidePool = false;

        public WaveSyncroPrefabSpawner.SpawnLayerTagMode spawnLayerMode =
            WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnPrefabSettings;

        public WaveSyncroPrefabSpawner.SpawnLayerTagMode spawnTagMode =
            WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnPrefabSettings;

        public int spawnCustomLayer = 0;
        public string spawnCustomTag = "Untagged";
        public bool applyLayerRecursively = false;

        public TriggeredSpawnerListener listener = null;

        public List<TriggeredWaveSpecifics> enableWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> disableWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> visibleWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> invisibleWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> mouseOverWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> mouseClickWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> collisionWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerEnterWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerStayWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerExitWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> spawnedWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> despawnedWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> codeTriggeredWaves1 = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> codeTriggeredWaves2 = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> clickWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> collision2dWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerEnter2dWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerStay2dWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> triggerExit2dWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> userDefinedEventWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unitySliderChangedWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityButtonClickedWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityPointerDownWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityPointerUpWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityPointerEnterWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityPointerExitWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityDragWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityDropWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityScrollWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityUpdateSelectedWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unitySelectWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityDeselectWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityMoveWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityInitializePotentialDragWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityBeginDragWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityEndDragWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unitySubmitWaves = new List<TriggeredWaveSpecifics>();
        public List<TriggeredWaveSpecifics> unityCancelWaves = new List<TriggeredWaveSpecifics>();

        public enum RepeatWaitFor {
            ItemsDoneSpawning,
            ItemsEliminated
        }
        // ReSharper restore InconsistentNaming

        #endregion

        #region private variables

        private TriggeredWaveMetaData _enableWaveMeta;
        private TriggeredWaveMetaData _disableWaveMeta;
        private TriggeredWaveMetaData _visibleWaveMeta;
        private TriggeredWaveMetaData _invisibleWaveMeta;
        private TriggeredWaveMetaData _mouseOverWaveMeta;
        private TriggeredWaveMetaData _mouseClickWaveMeta;
        private TriggeredWaveMetaData _collisionWaveMeta;
        private TriggeredWaveMetaData _triggerEnterWaveMeta;
        private TriggeredWaveMetaData _triggerStayWaveMeta;
        private TriggeredWaveMetaData _triggerExitWaveMeta;
        private TriggeredWaveMetaData _spawnedWaveMeta;
        private TriggeredWaveMetaData _despawnedWaveMeta;
        private TriggeredWaveMetaData _codeTriggeredWave1Meta;
        private TriggeredWaveMetaData _codeTriggeredWave2Meta;
        private TriggeredWaveMetaData _clickWaveMeta;
        private TriggeredWaveMetaData _collision2DWaveMeta;
        private TriggeredWaveMetaData _triggerEnter2DWaveMeta;
        private TriggeredWaveMetaData _triggerStay2DWaveMeta;
        private TriggeredWaveMetaData _triggerExit2DWaveMeta;
        private readonly List<TriggeredWaveMetaData> _userDefinedEventWaveMeta = new List<TriggeredWaveMetaData>();
        private TriggeredWaveMetaData _unitySliderChangedWaveMeta;
        private TriggeredWaveMetaData _unityButtonClickedWaveMeta;
        private TriggeredWaveMetaData _unityPointerDownWaveMeta;
        private TriggeredWaveMetaData _unityPointerUpWaveMeta;
        private TriggeredWaveMetaData _unityPointerEnterWaveMeta;
        private TriggeredWaveMetaData _unityPointerExitWaveMeta;
        private TriggeredWaveMetaData _unityDragWaveMeta;
        private TriggeredWaveMetaData _unityDropWaveMeta;
        private TriggeredWaveMetaData _unityScrollWaveMeta;
        private TriggeredWaveMetaData _unityUpdateSelectedWaveMeta;
        private TriggeredWaveMetaData _unitySelectWaveMeta;
        private TriggeredWaveMetaData _unityDeselectWaveMeta;
        private TriggeredWaveMetaData _unityMoveWaveMeta;
        private TriggeredWaveMetaData _unityInitializePotentialDragWaveMeta;
        private TriggeredWaveMetaData _unityBeginDragWaveMeta;
        private TriggeredWaveMetaData _unityEndDragWaveMeta;
        private TriggeredWaveMetaData _unitySubmitWaveMeta;
        private TriggeredWaveMetaData _unityCancelWaveMeta;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        // ReSharper disable once RedundantNameQualifier
        private UnityEngine.UI.Button _button;
        private Slider _slider;
#endif

        private Transform _trans;
        private GameObject _go;
        private bool _isVisible;
        private List<TriggeredSpawnerV2> _childSpawners = new List<TriggeredSpawnerV2>();
        private Dictionary<TriggeredSpawner.EventType, List<TriggeredWaveSpecifics>> _allWaves = new Dictionary<TriggeredSpawner.EventType, List<TriggeredWaveSpecifics>>(30);
        private float? _triggerEnterTime;
        private float? _triggerEnter2dTime;

        #endregion

        /*! \endcond */

        // ReSharper disable once UnusedMember.Local
        private void Awake() {
            _go = gameObject;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
            // ReSharper disable once RedundantNameQualifier
            _button = GetComponent<UnityEngine.UI.Button>();
            _slider = GetComponent<Slider>();

            if (IsSetToUGUI) {
                AddUGUIComponents();
            }

#endif

            SpawnedOrAwake();
            _go.DestroyChildrenWithoutMarker();
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            CheckForValidVariablesForWave(GetFirstWaveIfAny(enableWaves), TriggeredSpawner.EventType.OnEnabled);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(disableWaves), TriggeredSpawner.EventType.OnDisabled);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(visibleWaves), TriggeredSpawner.EventType.Visible);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(invisibleWaves), TriggeredSpawner.EventType.Invisible);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(mouseOverWaves), TriggeredSpawner.EventType.MouseOver_Legacy);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(mouseClickWaves), TriggeredSpawner.EventType.MouseClick_Legacy);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(collisionWaves), TriggeredSpawner.EventType.OnCollision);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerEnterWaves), TriggeredSpawner.EventType.OnTriggerEnter);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerStayWaves), TriggeredSpawner.EventType.OnTriggerStay);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerExitWaves), TriggeredSpawner.EventType.OnTriggerExit);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(spawnedWaves), TriggeredSpawner.EventType.OnSpawned);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(despawnedWaves), TriggeredSpawner.EventType.OnDespawned);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(clickWaves), TriggeredSpawner.EventType.OnClick_NGUI);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(codeTriggeredWaves1), TriggeredSpawner.EventType.CodeTriggered1);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(codeTriggeredWaves2), TriggeredSpawner.EventType.CodeTriggered2);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(collision2dWaves), TriggeredSpawner.EventType.OnCollision2D);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerEnter2dWaves), TriggeredSpawner.EventType.OnTriggerEnter2D);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerStay2dWaves), TriggeredSpawner.EventType.OnTriggerStay2D);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(triggerExit2dWaves), TriggeredSpawner.EventType.OnTriggerExit2D);

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
            CheckForValidVariablesForWave(GetFirstWaveIfAny(unitySliderChangedWaves), TriggeredSpawner.EventType.SliderChanged_uGUI);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(unityButtonClickedWaves), TriggeredSpawner.EventType.ButtonClicked_uGUI);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(unityPointerDownWaves), TriggeredSpawner.EventType.PointerDown_uGUI);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(unityPointerUpWaves), TriggeredSpawner.EventType.PointerUp_uGUI);
            CheckForValidVariablesForWave(GetFirstWaveIfAny(unityPointerEnterWaves), TriggeredSpawner.EventType.PointerEnter_uGUI);
#endif

            // check active item mode
            if (activeMode == LevelSettings.ActiveItemMode.IfWorldVariableInRange ||
                activeMode == LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                    var crit = activeItemCriteria.statMods[i];

                    if (WorldVariableTracker.IsBlankVariableName(crit._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}' has an Active Item Limit criteria with no World Variable selected. Please select one.",
                                Trans.name));
                    } else if (!WorldVariableTracker.VariableExistsInScene(crit._statName)) {
                        LevelSettings.LogIfNew(
                            string.Format(
                                "Spawner '{0}' has an Active Item Limit criteria criteria of World Variable '{1}', which doesn't exist in the scene.",
                                Trans.name,
                                crit._statName));
                    }
                }
            }

            CheckForIllegalCustomEvents();
        }

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            if (GameIsOverForSpawner || SpawnerIsPaused || !HasActiveSpawningWave || !SpawnerIsActive) {
                return;
            }

            SpawnFromWaveMeta(_enableWaveMeta, TriggeredSpawner.EventType.OnEnabled);
            SpawnFromWaveMeta(_disableWaveMeta, TriggeredSpawner.EventType.OnDisabled);
            SpawnFromWaveMeta(_visibleWaveMeta, TriggeredSpawner.EventType.Visible);
            SpawnFromWaveMeta(_invisibleWaveMeta, TriggeredSpawner.EventType.Invisible);
            SpawnFromWaveMeta(_mouseOverWaveMeta, TriggeredSpawner.EventType.MouseOver_Legacy);
            SpawnFromWaveMeta(_mouseClickWaveMeta, TriggeredSpawner.EventType.MouseClick_Legacy);
            SpawnFromWaveMeta(_collisionWaveMeta, TriggeredSpawner.EventType.OnCollision);
            SpawnFromWaveMeta(_triggerEnterWaveMeta, TriggeredSpawner.EventType.OnTriggerEnter);
            SpawnFromWaveMeta(_triggerStayWaveMeta, TriggeredSpawner.EventType.OnTriggerStay);
            SpawnFromWaveMeta(_triggerExitWaveMeta, TriggeredSpawner.EventType.OnTriggerExit);
            SpawnFromWaveMeta(_spawnedWaveMeta, TriggeredSpawner.EventType.OnSpawned);
            SpawnFromWaveMeta(_despawnedWaveMeta, TriggeredSpawner.EventType.OnDespawned);
            SpawnFromWaveMeta(_codeTriggeredWave1Meta, TriggeredSpawner.EventType.CodeTriggered1);
            SpawnFromWaveMeta(_codeTriggeredWave2Meta, TriggeredSpawner.EventType.CodeTriggered2);
            SpawnFromWaveMeta(_clickWaveMeta, TriggeredSpawner.EventType.OnClick_NGUI);
            SpawnFromWaveMeta(_collision2DWaveMeta, TriggeredSpawner.EventType.OnCollision2D);
            SpawnFromWaveMeta(_triggerEnter2DWaveMeta, TriggeredSpawner.EventType.OnTriggerEnter2D);
            SpawnFromWaveMeta(_triggerStay2DWaveMeta, TriggeredSpawner.EventType.OnTriggerStay2D);
            SpawnFromWaveMeta(_triggerExit2DWaveMeta, TriggeredSpawner.EventType.OnTriggerExit2D);
            SpawnFromWaveMeta(_unitySliderChangedWaveMeta, TriggeredSpawner.EventType.SliderChanged_uGUI);
            SpawnFromWaveMeta(_unityButtonClickedWaveMeta, TriggeredSpawner.EventType.ButtonClicked_uGUI);
            SpawnFromWaveMeta(_unityPointerDownWaveMeta, TriggeredSpawner.EventType.PointerDown_uGUI);
            SpawnFromWaveMeta(_unityPointerUpWaveMeta, TriggeredSpawner.EventType.PointerUp_uGUI);
            SpawnFromWaveMeta(_unityPointerEnterWaveMeta, TriggeredSpawner.EventType.PointerEnter_uGUI);
            SpawnFromWaveMeta(_unityPointerExitWaveMeta, TriggeredSpawner.EventType.PointerExit_uGUI);
            SpawnFromWaveMeta(_unityDragWaveMeta, TriggeredSpawner.EventType.Drag_uGUI);
            SpawnFromWaveMeta(_unityDropWaveMeta, TriggeredSpawner.EventType.Drop_uGUI);
            SpawnFromWaveMeta(_unityScrollWaveMeta, TriggeredSpawner.EventType.Scroll_uGUI);
            SpawnFromWaveMeta(_unityUpdateSelectedWaveMeta, TriggeredSpawner.EventType.UpdateSelected_uGUI);
            SpawnFromWaveMeta(_unitySelectWaveMeta, TriggeredSpawner.EventType.Select_uGUI);
            SpawnFromWaveMeta(_unityDeselectWaveMeta, TriggeredSpawner.EventType.Deselect_uGUI);
            SpawnFromWaveMeta(_unityMoveWaveMeta, TriggeredSpawner.EventType.Move_uGUI);
            SpawnFromWaveMeta(_unityInitializePotentialDragWaveMeta, TriggeredSpawner.EventType.InitializePotentialDrag_uGUI);
            SpawnFromWaveMeta(_unityBeginDragWaveMeta, TriggeredSpawner.EventType.BeginDrag_uGUI);
            SpawnFromWaveMeta(_unityEndDragWaveMeta, TriggeredSpawner.EventType.EndDrag_uGUI);
            SpawnFromWaveMeta(_unitySubmitWaveMeta, TriggeredSpawner.EventType.Submit_uGUI);
            SpawnFromWaveMeta(_unityCancelWaveMeta, TriggeredSpawner.EventType.Cancel_uGUI);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                var anEvent = _userDefinedEventWaveMeta[i];
                if (!anEvent.waveSpec.customEventActive) {
                    continue;
                }

                SpawnFromWaveMeta(anEvent, TriggeredSpawner.EventType.CustomEvent);
            }
        }

        #region Propogate Events

        private void PropagateEventToChildSpawners(TriggeredSpawner.EventType eType) {
            if (!transmitEventsToChildren) {
                return;
            }

            if (_childSpawners.Count <= 0) {
                return;
            }
            if (listener != null) {
                listener.EventPropagating(eType, Trans, _childSpawners.Count);
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _childSpawners.Count; i++) {
                _childSpawners[i].PropagateEventTrigger(eType, Trans);
            }
        }

        /*! \cond PRIVATE */
        public void PropagateEventTrigger(TriggeredSpawner.EventType eType, Transform transmitterTrans, bool calledFromInspector = false) {
            if (!calledFromInspector && listener != null) {
                listener.PropagatedEventReceived(eType, transmitterTrans);
            }

            switch (eType) {
                case TriggeredSpawner.EventType.CodeTriggered1:
                    ActivateCodeTriggeredEvent1();
                    break;
                case TriggeredSpawner.EventType.CodeTriggered2:
                    ActivateCodeTriggeredEvent2();
                    break;
                case TriggeredSpawner.EventType.Invisible:
                    _OnBecameInvisible(false);
                    break;
                case TriggeredSpawner.EventType.MouseClick_Legacy:
                    _OnMouseDown(false);
                    break;
                case TriggeredSpawner.EventType.MouseOver_Legacy:
                    _OnMouseEnter(false);
                    break;
                case TriggeredSpawner.EventType.OnClick_NGUI:
                    _OnClick(false);
                    break;
                case TriggeredSpawner.EventType.OnCollision:
                    _OnCollisionEnter(false);
                    break;
                case TriggeredSpawner.EventType.OnDespawned:
                    _OnDespawned(false);
                    break;
                case TriggeredSpawner.EventType.OnDisabled:
                    _DisableEvent(false);
                    break;
                case TriggeredSpawner.EventType.OnEnabled:
                    _EnableEvent(false);
                    break;
                case TriggeredSpawner.EventType.OnSpawned:
                    _OnSpawned(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter:
                    _OnTriggerEnter(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay:
                    _OnTriggerStay(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit:
                    _OnTriggerExit(false);
                    break;
                case TriggeredSpawner.EventType.Visible:
                    _OnBecameVisible(false);
                    break;
                case TriggeredSpawner.EventType.OnCollision2D:
                    _OnCollision2dEnter(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter2D:
                    _OnTriggerEnter2D(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay2D:
                    _OnTriggerStay2D(false);
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit2D:
                    _OnTriggerExit2D(false);
                    break;
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
                case TriggeredSpawner.EventType.SliderChanged_uGUI:
                    _SliderChanged(false);
                    break;
                case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                    _ButtonClicked(false);
                    break;
                case TriggeredSpawner.EventType.PointerDown_uGUI:
                    _OnPointerDown(false);
                    break;
                case TriggeredSpawner.EventType.PointerUp_uGUI:
                    _OnPointerUp(false);
                    break;
                case TriggeredSpawner.EventType.PointerEnter_uGUI:
                    _OnPointerEnter(false);
                    break;
                case TriggeredSpawner.EventType.PointerExit_uGUI:
                    _OnPointerExit(false);
                    break;
                case TriggeredSpawner.EventType.Drag_uGUI:
                    _OnDrag(false);
                    break;
                case TriggeredSpawner.EventType.Drop_uGUI:
                    _OnDrop(false);
                    break;
                case TriggeredSpawner.EventType.Scroll_uGUI:
                    _OnScroll(false);
                    break;
                case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                    _OnUpdateSelected(false);
                    break;
                case TriggeredSpawner.EventType.Select_uGUI:
                    _OnSelect(false);
                    break;
                case TriggeredSpawner.EventType.Deselect_uGUI:
                    _OnDeselect(false);
                    break;
                case TriggeredSpawner.EventType.Move_uGUI:
                    _OnMove(false);
                    break;
                case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                    _OnInitializePotentialDrag(false);
                    break;
                case TriggeredSpawner.EventType.BeginDrag_uGUI:
                    _OnBeginDrag(false);
                    break;
                case TriggeredSpawner.EventType.EndDrag_uGUI:
                    _OnEndDrag(false);
                    break;
                case TriggeredSpawner.EventType.Submit_uGUI:
                    _OnSubmit(false);
                    break;
                case TriggeredSpawner.EventType.Cancel_uGUI:
                    _OnCancel(false);
                    break;
#endif
            }
        }
        /*! \endcond */

        #endregion

        #region CodeTriggeredEvents

        /// <summary>
        /// Call this method to active Code-Triggered Event 1.
        /// </summary>
        public void ActivateCodeTriggeredEvent1() {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.CodeTriggered1;

            var codeTriggeredWave1 = GetFirstWaveIfAny(codeTriggeredWaves1);

            if (!IsWaveValid(codeTriggeredWave1, eType, false)) {
                return;
            }

            if (SetupNextWave(codeTriggeredWave1, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _codeTriggeredWave1Meta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_codeTriggeredWave1Meta, eType);

            PropagateEventToChildSpawners(eType);
        }

        /// <summary>
        /// Call this method to active Code-Triggered Event 2.
        /// </summary>
        public void ActivateCodeTriggeredEvent2() {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.CodeTriggered2;

            var codeTriggeredWave2 = GetFirstWaveIfAny(codeTriggeredWaves2);

            if (!IsWaveValid(codeTriggeredWave2, eType, false)) {
                return;
            }

            if (SetupNextWave(codeTriggeredWave2, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _codeTriggeredWave2Meta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_codeTriggeredWave2Meta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnEnable

        // ReSharper disable once UnusedMember.Local
        private void OnEnable() {
#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
            if (_slider != null) {
                _slider.onValueChanged.AddListener(SliderChanged);
            }
            if (_button != null) {
                _button.onClick.AddListener(ButtonClicked);
            }
#endif


            _EnableEvent(true);
        }

        private void _EnableEvent(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnEnabled;

            RegisterReceiver();

            var enableWave = GetFirstWaveIfAny(enableWaves);

            if (!IsWaveValid(enableWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.OnDisabled, string.Empty); // stop "disable" wave.

            if (SetupNextWave(enableWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _enableWaveMeta.waveSpec);
                }
            }

            if (PoolBoss.IsReady) {
                SpawnFromWaveMeta(_enableWaveMeta, eType);

                PropagateEventToChildSpawners(eType);
            }
        }

        #endregion

        #region OnDisable

        // ReSharper disable once UnusedMember.Local
        private void OnDisable() {
            _DisableEvent(true);
        }

        private void _DisableEvent(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnDisabled;

            UnregisterReceiver();

            var disableWave = GetFirstWaveIfAny(disableWaves);

            if (!IsWaveValid(disableWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.OnEnabled, string.Empty); // stop "enable" wave.

            if (SetupNextWave(disableWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _disableWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_disableWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region Pooling events (OnSpawned, OnDespawned) - used by both Pool Boss and Pool Manager.

        // ReSharper disable once UnusedMember.Local
        private void OnSpawned() {
            SpawnedOrAwake();

            if (listener != null) {
                listener.Spawned(this);
            }

            _OnSpawned(true);
        }

        private void _OnSpawned(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnSpawned;

            var spawnedWave = GetFirstWaveIfAny(spawnedWaves);

            if (!IsWaveValid(spawnedWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.Invisible, string.Empty); // stop "invisible" wave.

            if (SetupNextWave(spawnedWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _spawnedWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_spawnedWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnDespawned() {
            if (listener != null) {
                listener.Despawned(this);
            }

            _OnDespawned(true);
        }

        private void _OnDespawned(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnDespawned;

            var despawnedWave = GetFirstWaveIfAny(despawnedWaves);

            if (!IsWaveValid(despawnedWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.Visible, string.Empty); // stop "visible" wave.

            if (SetupNextWave(despawnedWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _despawnedWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_despawnedWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnBecameVisible

        // ReSharper disable once UnusedMember.Local
        private void OnBecameVisible() {
            if (_isVisible) {
                return; // to fix Unity error
            }

            _OnBecameVisible(true);
        }

        private void _OnBecameVisible(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Visible;
            _isVisible = true;

            var visibleWave = GetFirstWaveIfAny(visibleWaves);

            if (!IsWaveValid(visibleWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.Invisible, string.Empty); // stop "invisible" wave.

            if (SetupNextWave(visibleWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _visibleWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_visibleWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnBecameInvisible

        // ReSharper disable once UnusedMember.Local
        private void OnBecameInvisible() {
            _OnBecameInvisible(true);
        }

        private void _OnBecameInvisible(bool calledFromSelf) {
            var invisibleWave = GetFirstWaveIfAny(invisibleWaves);

            StopOppositeWaveIfActive(GetFirstWaveIfAny(visibleWaves), TriggeredSpawner.EventType.Visible);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Invisible;
            _isVisible = false;

            if (!IsWaveValid(invisibleWave, eType, calledFromSelf)) {
                return;
            }

            EndWave(TriggeredSpawner.EventType.Visible, string.Empty); // stop "visible" wave.

            if (SetupNextWave(invisibleWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _invisibleWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_invisibleWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnMouseEnter

        // ReSharper disable once UnusedMember.Local
        private void OnMouseEnter() {
            _OnMouseEnter(true);
        }

        private void _OnMouseEnter(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.MouseOver_Legacy;

            var mouseOverWave = GetFirstWaveIfAny(mouseOverWaves);

            if (!IsSetToLegacyUI || !IsWaveValid(mouseOverWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(mouseOverWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _mouseOverWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_mouseOverWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnMouseDown

        // ReSharper disable once UnusedMember.Local
        private void OnMouseDown() {
            _OnMouseDown(true);
        }

        private void _OnMouseDown(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.MouseClick_Legacy;

            var mouseClickWave = GetFirstWaveIfAny(mouseClickWaves);

            if (!IsSetToLegacyUI || !IsWaveValid(mouseClickWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(mouseClickWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _mouseClickWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_mouseClickWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region NGUI events (onClick)

        // ReSharper disable once UnusedMember.Local
        private void OnClick() {
            _OnClick(true);
        }

        private void _OnClick(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnClick_NGUI;

            var clickWave = GetFirstWaveIfAny(clickWaves);

            if (!IsWaveValid(clickWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(clickWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _clickWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_clickWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnCollisionEnter

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter(Collision collision) {
            var collisionWave = GetFirstWaveIfAny(collisionWaves);
            if (collisionWave == null) {
                return;
            }

            // check filters for matches if turned on
            if (collisionWave.useLayerFilter && !collisionWave.matchingLayers.Contains(collision.gameObject.layer)) {
                return;
            }

            if (collisionWave.useTagFilter && !collisionWave.matchingTags.Contains(collision.gameObject.tag)) {
                return;
            }

            _OnCollisionEnter(true);
        }

        private void _OnCollisionEnter(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnCollision;

            var collisionWave = GetFirstWaveIfAny(collisionWaves);

            if (!IsWaveValid(collisionWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(collisionWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _collisionWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_collisionWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region OnTriggerEnter

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerEnter(Collider other) {
            _triggerEnterTime = Time.realtimeSinceStartup;

            var triggerEnterWave = GetFirstWaveIfAny(triggerEnterWaves);
            var triggerExitWave = GetFirstWaveIfAny(triggerExitWaves);

            // check filters for matches if turned on
            if (triggerEnterWave != null && triggerEnterWave.enableWave) {
                if (triggerEnterWave.useLayerFilter && !triggerEnterWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerEnterWave.useTagFilter && !triggerEnterWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerExitWave != null && triggerExitWave.enableWave && triggerExitWave.stopWaveOnOppositeEvent) {
                if (triggerExitWave.useLayerFilter && !triggerExitWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerExitWave.useTagFilter && !triggerExitWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            _OnTriggerEnter(true);
        }

        private void _OnTriggerEnter(bool calledFromSelf) {
            var triggerExitWave = GetFirstWaveIfAny(triggerExitWaves);
            var triggerEnterWave = GetFirstWaveIfAny(triggerEnterWaves);

            StopOppositeWaveIfActive(triggerExitWave, TriggeredSpawner.EventType.OnTriggerExit);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerEnter;

            if (!IsWaveValid(triggerEnterWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerEnterWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerEnterWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerEnterWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region TriggerStay

        #region OnTriggerStay

        private void OnTriggerStay(Collider other) {
            var triggerStayWave = GetFirstWaveIfAny(triggerStayWaves);

            // check filters for matches if turned on
            if (triggerStayWave != null && triggerStayWave.enableWave) {
                if (triggerStayWave.useLayerFilter && !triggerStayWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerStayWave.useTagFilter && !triggerStayWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            float stayTime = 0f;

            if (!_triggerEnterTime.HasValue) {
                stayTime = 0f;
            } else {
                stayTime = Time.realtimeSinceStartup - _triggerEnterTime.Value;
            }

            if (triggerStayWave != null && triggerStayWave.triggerStayForTime > stayTime) {
                return;
            }

            _OnTriggerStay(true);
        }

        private void _OnTriggerStay(bool calledFromSelf) {
            var triggerStayWave = GetFirstWaveIfAny(triggerStayWaves);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerStay;

            if (!IsWaveValid(triggerStayWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerStayWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerStayWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerStayWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #endregion

        #region OnTriggerExit

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerExit(Collider other) {
            _triggerEnter2dTime = null;

            var triggerExitWave = GetFirstWaveIfAny(triggerExitWaves);
            var triggerStayWave = GetFirstWaveIfAny(triggerStayWaves);
            var triggerEnterWave = GetFirstWaveIfAny(triggerEnterWaves);

            // check filters for matches if turned on
            if (triggerExitWave != null && triggerExitWave.enableWave) {
                if (triggerExitWave.useLayerFilter && !triggerExitWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerExitWave.useTagFilter && !triggerExitWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerEnterWave != null && triggerEnterWave.enableWave && triggerEnterWave.stopWaveOnOppositeEvent) {
                if (triggerEnterWave.useLayerFilter && !triggerEnterWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerEnterWave.useTagFilter && !triggerEnterWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerStayWave != null && triggerStayWave.enableWave && triggerStayWave.stopWaveOnOppositeEvent) {
                if (triggerStayWave.useLayerFilter && !triggerStayWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerStayWave.useTagFilter && !triggerStayWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            _OnTriggerExit(true);
        }

        private void _OnTriggerExit(bool calledFromSelf) {
            StopOppositeWaveIfActive(GetFirstWaveIfAny(triggerEnterWaves), TriggeredSpawner.EventType.OnTriggerEnter);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerExit;

            var triggerExitWave = GetFirstWaveIfAny(triggerExitWaves);

            if (!IsWaveValid(triggerExitWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerExitWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerExitWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerExitWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region Collision Enter / Trigger 2D events

        // ReSharper disable once UnusedMember.Local
        private void OnCollisionEnter2D(Collision2D collision) {
            var collision2dWave = GetFirstWaveIfAny(collision2dWaves);

            if (collision2dWave == null) {
                return;
            }

            // check filters for matches if turned on
            if (collision2dWave.useLayerFilter && !collision2dWave.matchingLayers.Contains(collision.gameObject.layer)) {
                return;
            }

            if (collision2dWave.useTagFilter && !collision2dWave.matchingTags.Contains(collision.gameObject.tag)) {
                return;
            }

            _OnCollision2dEnter(true);
        }

        private void _OnCollision2dEnter(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnCollision2D;

            // ReSharper disable once InconsistentNaming
            var collision2dWave = GetFirstWaveIfAny(collision2dWaves);

            if (!IsWaveValid(collision2dWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(collision2dWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _collision2DWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_collision2DWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        // ReSharper disable once UnusedMember.Local
        private void OnTriggerEnter2D(Collider2D other) {
            _triggerEnter2dTime = Time.realtimeSinceStartup;

            var triggerEnter2dWave = GetFirstWaveIfAny(triggerEnter2dWaves);
            var triggerExit2dWave = GetFirstWaveIfAny(triggerExit2dWaves);

            // check filters for matches if turned on
            if (triggerEnter2dWave != null && triggerEnter2dWave.enableWave) {
                if (triggerEnter2dWave.useLayerFilter &&
                    !triggerEnter2dWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerEnter2dWave.useTagFilter && !triggerEnter2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerExit2dWave != null && triggerExit2dWave.enableWave && triggerExit2dWave.stopWaveOnOppositeEvent) {
                if (triggerExit2dWave.useLayerFilter &&
                    !triggerExit2dWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerExit2dWave.useTagFilter && !triggerExit2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            _OnTriggerEnter2D(true);
        }

        private void _OnTriggerEnter2D(bool calledFromSelf) {
            StopOppositeWaveIfActive(GetFirstWaveIfAny(triggerExit2dWaves), TriggeredSpawner.EventType.OnTriggerExit2D);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerEnter2D;

            // ReSharper disable once InconsistentNaming
            var triggerEnter2dWave = GetFirstWaveIfAny(triggerEnter2dWaves);

            if (!IsWaveValid(triggerEnter2dWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerEnter2dWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerEnter2DWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerEnter2DWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        private void OnTriggerStay2D(Collider2D other) {
            var triggerStay2dWave = GetFirstWaveIfAny(triggerStay2dWaves);

            // check filters for matches if turned on
            if (triggerStay2dWave != null && triggerStay2dWave.enableWave) {
                if (triggerStay2dWave.useLayerFilter && !triggerStay2dWave.matchingLayers.Contains(other.gameObject.layer))  {
                    return;
                }

                if (triggerStay2dWave.useTagFilter && !triggerStay2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            float stayTime = 0f;

            if (!_triggerEnter2dTime.HasValue) {
                stayTime = 0f;
            } else {
                stayTime = Time.realtimeSinceStartup - _triggerEnter2dTime.Value;
            }

            if (triggerStay2dWave.triggerStayForTime > stayTime) {
                return;
            }

            _OnTriggerStay2D(true);
        }

        private void _OnTriggerStay2D(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerStay2D;

            // ReSharper disable once InconsistentNaming
            var triggerStay2dWave = GetFirstWaveIfAny(triggerStay2dWaves);

            if (!IsWaveValid(triggerStay2dWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerStay2dWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerStay2DWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerStay2DWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTriggerExit2D(Collider2D other) {
            _triggerEnter2dTime = null;

            var triggerExit2dWave = GetFirstWaveIfAny(triggerExit2dWaves);
            var triggerStay2dWave = GetFirstWaveIfAny(triggerStay2dWaves);
            var triggerEnter2dWave = GetFirstWaveIfAny(triggerEnter2dWaves);

            // check filters for matches if turned on
            if (triggerExit2dWave != null && triggerExit2dWave.enableWave) {
                if (triggerExit2dWave.useLayerFilter &&
                    !triggerExit2dWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerExit2dWave.useTagFilter && !triggerExit2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerEnter2dWave != null && triggerEnter2dWave.enableWave && triggerEnter2dWave.stopWaveOnOppositeEvent) {
                if (triggerEnter2dWave.useLayerFilter &&
                    !triggerEnter2dWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerEnter2dWave.useTagFilter && !triggerEnter2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            } else if (triggerStay2dWave != null && triggerStay2dWave.enableWave && triggerStay2dWave.stopWaveOnOppositeEvent) {
                if (triggerStay2dWave.useLayerFilter && !triggerStay2dWave.matchingLayers.Contains(other.gameObject.layer)) {
                    return;
                }

                if (triggerStay2dWave.useTagFilter && !triggerStay2dWave.matchingTags.Contains(other.gameObject.tag)) {
                    return;
                }
            }

            _OnTriggerExit2D(true);
        }

        private void _OnTriggerExit2D(bool calledFromSelf) {
            StopOppositeWaveIfActive(GetFirstWaveIfAny(triggerEnter2dWaves), TriggeredSpawner.EventType.OnTriggerEnter2D);

            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.OnTriggerExit2D;

            var triggerExit2dWave = GetFirstWaveIfAny(triggerExit2dWaves);

            if (!IsWaveValid(triggerExit2dWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(triggerExit2dWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _triggerExit2DWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_triggerExit2DWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        #region UI Events
        public void OnPointerEnter(PointerEventData data) {
            _OnPointerEnter(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnPointerEnter(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.PointerEnter_uGUI;

            var unityPointerEnterWave = GetFirstWaveIfAny(unityPointerEnterWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityPointerEnterWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityPointerEnterWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityPointerEnterWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityPointerEnterWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnPointerExit(PointerEventData data) {
            _OnPointerExit(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnPointerExit(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.PointerExit_uGUI;

            var unityPointerExitWave = GetFirstWaveIfAny(unityPointerExitWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityPointerExitWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityPointerExitWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityPointerExitWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityPointerExitWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnPointerDown(PointerEventData data) {
            _OnPointerDown(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnPointerDown(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.PointerDown_uGUI;

            var unityPointerDownWave = GetFirstWaveIfAny(unityPointerDownWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityPointerDownWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityPointerDownWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityPointerDownWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityPointerDownWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnPointerUp(PointerEventData data) {
            _OnPointerUp(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnPointerUp(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.PointerUp_uGUI;

            var unityPointerUpWave = GetFirstWaveIfAny(unityPointerUpWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityPointerUpWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityPointerUpWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityPointerUpWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityPointerUpWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnDrag(PointerEventData data) {
            _OnDrag(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnDrag(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Drag_uGUI;

            var unityDragWave = GetFirstWaveIfAny(unityDragWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityDragWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityDragWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityDragWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityDragWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnDrop(PointerEventData data) {
            _OnDrop(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnDrop(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Drop_uGUI;

            var unityDropWave = GetFirstWaveIfAny(unityDropWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityDropWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityDropWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityDropWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityDropWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnScroll(PointerEventData data) {
            _OnScroll(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnScroll(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Scroll_uGUI;

            var unityScrollWave = GetFirstWaveIfAny(unityScrollWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityScrollWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityScrollWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityScrollWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityScrollWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnUpdateSelected(BaseEventData data) {
            _OnUpdateSelected(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnUpdateSelected(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.UpdateSelected_uGUI;

            var unityUpdateSelectedWave = GetFirstWaveIfAny(unityEndDragWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityUpdateSelectedWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityUpdateSelectedWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityUpdateSelectedWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityUpdateSelectedWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnSelect(BaseEventData data) {
            _OnSelect(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnSelect(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Select_uGUI;

            var unitySelectWave = GetFirstWaveIfAny(unitySelectWaves);

            if (!IsSetToUGUI || !IsWaveValid(unitySelectWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unitySelectWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unitySelectWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unitySelectWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnDeselect(BaseEventData data) {
            _OnDeselect(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnDeselect(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Deselect_uGUI;

            var unityDeselectWave = GetFirstWaveIfAny(unityDeselectWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityDeselectWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityDeselectWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityDeselectWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityDeselectWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnMove(AxisEventData data) {
            _OnMove(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnMove(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Move_uGUI;

            var unityMoveWave = GetFirstWaveIfAny(unityMoveWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityMoveWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityMoveWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityMoveWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityMoveWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnInitializePotentialDrag(PointerEventData data) {
            _OnInitializePotentialDrag(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnInitializePotentialDrag(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.InitializePotentialDrag_uGUI;

            var unityInitializePotentialDragWave = GetFirstWaveIfAny(unityInitializePotentialDragWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityInitializePotentialDragWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityInitializePotentialDragWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityInitializePotentialDragWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityInitializePotentialDragWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnBeginDrag(PointerEventData data) {
            _OnBeginDrag(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnBeginDrag(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.BeginDrag_uGUI;

            var unityBeginDragWave = GetFirstWaveIfAny(unityBeginDragWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityBeginDragWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityBeginDragWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityBeginDragWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityBeginDragWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnEndDrag(PointerEventData data) {
            _OnEndDrag(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnEndDrag(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.EndDrag_uGUI;

            var unityEndDragWave = GetFirstWaveIfAny(unityEndDragWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityEndDragWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityEndDragWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityEndDragWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityEndDragWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        public void OnSubmit(BaseEventData data) {
            _OnSubmit(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnSubmit(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Submit_uGUI;

            var unitySubmitWave = GetFirstWaveIfAny(unitySubmitWaves);

            if (!IsSetToUGUI || !IsWaveValid(unitySubmitWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unitySubmitWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unitySubmitWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unitySubmitWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }


        public void OnCancel(BaseEventData data) {
            _OnCancel(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _OnCancel(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.Cancel_uGUI;

            var unityCancelWave = GetFirstWaveIfAny(unityCancelWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityCancelWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unityCancelWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityCancelWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityCancelWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        #endregion

        #region Unity UI Events (4.6)

        private void SliderChanged(float newValue) {
            _SliderChanged(true);
        }

        // ReSharper disable once UnusedParameter.Local
        private void _SliderChanged(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.SliderChanged_uGUI;

            var unitySliderChangedWave = GetFirstWaveIfAny(unitySliderChangedWaves);

            if (!IsSetToUGUI || !IsWaveValid(unitySliderChangedWave, eType, true)) {
                return;
            }

            if (SetupNextWave(unitySliderChangedWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unitySliderChangedWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unitySliderChangedWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }

        private void ButtonClicked() {
            _ButtonClicked(true);
        }

        private void _ButtonClicked(bool calledFromSelf) {
            const TriggeredSpawner.EventType eType = TriggeredSpawner.EventType.ButtonClicked_uGUI;

            var unityButtonClickedWave = GetFirstWaveIfAny(unityButtonClickedWaves);

            if (!IsSetToUGUI || !IsWaveValid(unityButtonClickedWave, eType, calledFromSelf)) {
                return;
            }

            if (SetupNextWave(unityButtonClickedWave, eType)) {
                if (listener != null) {
                    listener.WaveStart(eType, _unityButtonClickedWaveMeta.waveSpec);
                }
            }
            SpawnFromWaveMeta(_unityButtonClickedWaveMeta, eType);

            PropagateEventToChildSpawners(eType);
        }
        #endregion
#endif

        #region public methods

        /// <summary>
        /// This method returns a list of the child Spawners, if any.
        /// </summary>
        /// <param name="trans">The Transform of the Spawner to get child Spawners of.</param>
        /// <returns>A list of Triggered Spawner scripts for all child Spawners.</returns>
        public static List<TriggeredSpawnerV2> GetChildSpawners(Transform trans) {
            var childSpawn = new List<TriggeredSpawnerV2>();
            if (trans.childCount <= 0) {
                return childSpawn;
            }
            for (var i = 0; i < trans.childCount; i++) {
                var spawner = trans.GetChild(i).GetComponent<TriggeredSpawnerV2>();

                if (spawner == null || spawner.eventSourceType != TriggeredSpawner.SpawnerEventSource.ReceiveFromParent) {
                    continue;
                }

                childSpawn.Add(spawner);
            }

            return childSpawn;
        }

        /// <summary>
        /// Returns whether a wave of the current event is currently active or not.
        /// </summary>
        /// <param name="eType">Event Type</param>
        /// <param name="customEventName">The name of the custom event (if you're stopping a custom wave only.</param>
        /// <returns>True or false</returns>
        public bool HasActiveWaveOfType(TriggeredSpawner.EventType eType, string customEventName) {
            switch (eType) {
                case TriggeredSpawner.EventType.OnEnabled:
                    return _enableWaveMeta != null;
                case TriggeredSpawner.EventType.OnDisabled:
                    return _disableWaveMeta != null;
                case TriggeredSpawner.EventType.Visible:
                    return _visibleWaveMeta != null;
                case TriggeredSpawner.EventType.Invisible:
                    return _invisibleWaveMeta != null;
                case TriggeredSpawner.EventType.MouseOver_Legacy:
                    return _mouseOverWaveMeta != null;
                case TriggeredSpawner.EventType.MouseClick_Legacy:
                    return _mouseClickWaveMeta != null;
                case TriggeredSpawner.EventType.OnCollision:
                    return _collisionWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerEnter:
                    return _triggerEnterWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerStay:
                    return _triggerStayWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerExit:
                    return _triggerExitWaveMeta != null;
                case TriggeredSpawner.EventType.OnSpawned:
                    return _spawnedWaveMeta != null;
                case TriggeredSpawner.EventType.OnDespawned:
                    return _despawnedWaveMeta != null;
                case TriggeredSpawner.EventType.CodeTriggered1:
                    return _codeTriggeredWave1Meta != null;
                case TriggeredSpawner.EventType.CodeTriggered2:
                    return _codeTriggeredWave2Meta != null;
                case TriggeredSpawner.EventType.OnClick_NGUI:
                    return _clickWaveMeta != null;
                case TriggeredSpawner.EventType.OnCollision2D:
                    return _collision2DWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerEnter2D:
                    return _triggerEnter2DWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerStay2D:
                    return _triggerEnter2DWaveMeta != null;
                case TriggeredSpawner.EventType.OnTriggerExit2D:
                    return _triggerExit2DWaveMeta != null;
                case TriggeredSpawner.EventType.SliderChanged_uGUI:
                    return _unitySliderChangedWaveMeta != null;
                case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                    return _unityButtonClickedWaveMeta != null;
                case TriggeredSpawner.EventType.PointerDown_uGUI:
                    return _unityPointerDownWaveMeta != null;
                case TriggeredSpawner.EventType.PointerUp_uGUI:
                    return _unityPointerUpWaveMeta != null;
                case TriggeredSpawner.EventType.PointerEnter_uGUI:
                    return _unityPointerEnterWaveMeta != null;
                case TriggeredSpawner.EventType.PointerExit_uGUI:
                    return _unityPointerExitWaveMeta != null;
                case TriggeredSpawner.EventType.Drag_uGUI:
                    return _unityDragWaveMeta != null;
                case TriggeredSpawner.EventType.Drop_uGUI:
                    return _unityDropWaveMeta != null;
                case TriggeredSpawner.EventType.Scroll_uGUI:
                    return _unityScrollWaveMeta != null;
                case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                    return _unityUpdateSelectedWaveMeta != null;
                case TriggeredSpawner.EventType.Select_uGUI:
                    return _unitySelectWaveMeta != null;
                case TriggeredSpawner.EventType.Deselect_uGUI:
                    return _unityDeselectWaveMeta != null;
                case TriggeredSpawner.EventType.Move_uGUI:
                    return _unityMoveWaveMeta != null;
                case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                    return _unityInitializePotentialDragWaveMeta != null;
                case TriggeredSpawner.EventType.BeginDrag_uGUI:
                    return _unityBeginDragWaveMeta != null;
                case TriggeredSpawner.EventType.EndDrag_uGUI:
                    return _unityEndDragWaveMeta != null;
                case TriggeredSpawner.EventType.Submit_uGUI:
                    return _unitySubmitWaveMeta != null;
                case TriggeredSpawner.EventType.Cancel_uGUI:
                    return _unityCancelWaveMeta != null;
                case TriggeredSpawner.EventType.CustomEvent:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                        var anEvent = _userDefinedEventWaveMeta[i].waveSpec;
                        if (anEvent.customEventActive && anEvent.customEventName == customEventName) {
                            return true;
                        }
                    }

                    return false;
                default:
                    Debug.Log("Unknown event type: " + eType.ToString());
                    return false;
            }
        }

        /// <summary>
        /// This method stops the currently spawning wave of an Event Type you pass in, if there is a match.
        /// </summary>
        /// <param name="eType">The Event Type of the wave to end.</param>
        /// <param name="customEventName">The name of the custom event (if you're stopping a custom wave only.</param>
        public void EndWave(TriggeredSpawner.EventType eType, string customEventName) {
            var isEarlyEnd = HasActiveWaveOfType(eType, customEventName);

            switch (eType) {
                case TriggeredSpawner.EventType.CodeTriggered1:
                    _codeTriggeredWave1Meta = null;
                    break;
                case TriggeredSpawner.EventType.CodeTriggered2:
                    _codeTriggeredWave2Meta = null;
                    break;
                case TriggeredSpawner.EventType.Invisible:
                    _invisibleWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.MouseClick_Legacy:
                    _mouseClickWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.MouseOver_Legacy:
                    _mouseOverWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnClick_NGUI:
                    _clickWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnCollision:
                    _collisionWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnCollision2D:
                    _collision2DWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnDespawned:
                    _despawnedWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnDisabled:
                    _disableWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnEnabled:
                    _enableWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnSpawned:
                    _spawnedWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter:
                    _triggerEnterWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay:
                    _triggerStayWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit:
                    _triggerExitWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter2D:
                    _triggerEnter2DWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay2D:
                    _triggerStay2DWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit2D:
                    _triggerExit2DWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.SliderChanged_uGUI:
                    _unitySliderChangedWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                    _unityButtonClickedWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.PointerDown_uGUI:
                    _unityPointerDownWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.PointerUp_uGUI:
                    _unityPointerUpWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.PointerEnter_uGUI:
                    _unityPointerEnterWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.PointerExit_uGUI:
                    _unityPointerExitWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Drag_uGUI:
                    _unityDragWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Drop_uGUI:
                    _unityDropWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Scroll_uGUI:
                    _unityScrollWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                    _unityUpdateSelectedWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Select_uGUI:
                    _unitySelectWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Deselect_uGUI:
                    _unityDeselectWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Move_uGUI:
                    _unityMoveWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                    _unityInitializePotentialDragWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.BeginDrag_uGUI:
                    _unityBeginDragWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.EndDrag_uGUI:
                    _unityEndDragWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Submit_uGUI:
                    _unitySubmitWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Cancel_uGUI:
                    _unityCancelWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.Visible:
                    _visibleWaveMeta = null;
                    break;
                case TriggeredSpawner.EventType.CustomEvent:
                    for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                        var anEvent = _userDefinedEventWaveMeta[i];
                        if (!anEvent.waveSpec.customEventActive || anEvent.waveSpec.customEventName != customEventName) {
                            continue;
                        }
                        _userDefinedEventWaveMeta.Remove(anEvent);
                        break;
                    }
                    break;
                default:
                    Debug.LogError("Illegal event: " + eType.ToString());
                    return;
            }

            if (listener != null && isEarlyEnd) {
                listener.WaveEndedEarly(eType);
            }

            PropagateEndWaveToChildSpawners(eType, customEventName);
        }

        /*! \cond PRIVATE */
        public bool IsUsingPrefabPool(Transform poolTrans) {
            var poolName = poolTrans.name;
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(enableWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(disableWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(visibleWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(invisibleWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(mouseOverWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(mouseClickWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(collisionWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(triggerEnterWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(triggerExitWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(spawnedWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(despawnedWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(codeTriggeredWaves1), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(codeTriggeredWaves2), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(clickWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(collision2dWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(triggerEnter2dWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(triggerExit2dWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unitySliderChangedWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityButtonClickedWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityPointerDownWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityPointerUpWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityPointerEnterWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityPointerExitWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityDragWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityDropWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityScrollWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityUpdateSelectedWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unitySelectWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityDeselectWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityMoveWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityInitializePotentialDragWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityBeginDragWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityEndDragWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unitySubmitWaves), poolName)) {
                return true;
            }
            if (WaveIsUsingPrefabPool(GetFirstWaveIfAny(unityCancelWaves), poolName)) {
                return true;
            }
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < userDefinedEventWaves.Count; i++) {
                var anEvent = userDefinedEventWaves[i];
                if (WaveIsUsingPrefabPool(anEvent, poolName)) {
                    return true;
                }
            }

            return false;
        }

        public void SpawnWaveVisual(TriggeredWaveSpecifics wave) {
            if (Application.isPlaying) {
                // let's not lock up the CPU!
                return;
            }

            var isSphere = wave.spawnSource != WaveSpecifics.SpawnOrigin.Specific || wave.prefabToSpawn == null;

            var nbrToSpawn = SpawnerUtility.GetMaxVisualizeItems(wave.NumberToSpwn);

            for (var i = 0; i < nbrToSpawn; i++) {
                var spawnPosition = GetSpawnPositionForVisualization(wave, transform.position, i);

                var rotation = GetSpawnRotationForVisualization(wave, transform, i);

                Transform spawned;

                if (isSphere) {
                    spawned = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    spawned.transform.position = spawnPosition;
                    spawned.transform.rotation = rotation;
                } else {
                    // ReSharper disable once PossibleNullReferenceException
                    spawned = ((Transform)Instantiate(wave.prefabToSpawn, spawnPosition, rotation));
                }

                spawned.parent = transform;
                spawned.gameObject.AddComponent<VisualizationMarker>();

                AfterSpawnForVisualization(wave, spawned.transform);
            }
        }

        /*! \endcond */
        #endregion

        #region overridable methods

        /// <summary>
        /// Override this method to call a Network Instantiate method or other.
        /// </summary>
        /// <param name="prefabToSpawn">The prefab to spawn.</param>
        /// <param name="spawnPosition">The position to spawn in.</param>
        /// <param name="rotation">The rotation to spawn with.</param>
        /// <returns>Spawned item Transform</returns>
        protected virtual Transform SpawnWaveItem(Transform prefabToSpawn, Vector3 spawnPosition, Quaternion rotation, TriggeredWaveMetaData wave) {
            Transform spawned;

            if (spawnOutsidePool) {
                spawned = PoolBoss.SpawnOutsidePool(prefabToSpawn, spawnPosition, rotation);
            }

            spawned = PoolBoss.SpawnInPool(prefabToSpawn, spawnPosition, rotation);

            if (spawned != null) {
                wave.spawnedWaveMembers.Add(spawned);
            }

            return spawned;
        }

        /// <summary>
        /// Override this method to call a Network Destroy method or other.
        /// </summary>
        protected virtual void DespawnSpawner() {
            PoolBoss.Despawn(Trans);
        }

        /// <summary>
        /// This method gets called whenever the object is spawned or starts in a Scene (from Awake event)
        /// </summary>
        protected virtual void SpawnedOrAwake() {
            _isVisible = false;

            // reset any in-progress waves that were despawned.
            _enableWaveMeta = null;
            _disableWaveMeta = null;
            _visibleWaveMeta = null;
            _invisibleWaveMeta = null;
            _mouseOverWaveMeta = null;
            _mouseClickWaveMeta = null;
            _collisionWaveMeta = null;
            _triggerEnterWaveMeta = null;
            _triggerExitWaveMeta = null;
            _spawnedWaveMeta = null;
            _despawnedWaveMeta = null;
            _codeTriggeredWave1Meta = null;
            _codeTriggeredWave2Meta = null;
            _clickWaveMeta = null;
            _collision2DWaveMeta = null;
            _triggerEnter2DWaveMeta = null;
            _triggerExit2DWaveMeta = null;
            _unitySliderChangedWaveMeta = null;
            _unityButtonClickedWaveMeta = null;
            _unityPointerDownWaveMeta = null;
            _unityPointerUpWaveMeta = null;
            _unityPointerEnterWaveMeta = null;
            _unityPointerExitWaveMeta = null;
            _unityDragWaveMeta = null;
            _unityDropWaveMeta = null;
            _unityScrollWaveMeta = null;
            _unityUpdateSelectedWaveMeta = null;
            _unitySelectWaveMeta = null;
            _unityDeselectWaveMeta = null;
            _unityMoveWaveMeta = null;
            _unityInitializePotentialDragWaveMeta = null;
            _unityBeginDragWaveMeta = null;
            _unityEndDragWaveMeta = null;
            _unitySubmitWaveMeta = null;
            _unityCancelWaveMeta = null;

            // scan for and cache child spawners
            _childSpawners = GetChildSpawners(Trans);
        }

        /// <summary>
        /// This returns the item to spawn. Override this to apply custom logic if needed.
        /// </summary>
        /// <returns>The Transform to spawn.</returns>
        protected virtual Transform GetSpawnable(TriggeredWaveMetaData wave) {
            switch (wave.waveSpec.spawnSource) {
                case WaveSpecifics.SpawnOrigin.Specific:
                    return wave.waveSpec.prefabToSpawn;
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    return wave.wavePool.GetRandomWeightedTransform();
            }

            return null;
        }

        /// <summary>
        /// Always returns true. Subclass this to override it if needed.
        /// </summary>
        /// <returns>boolean value.</returns>
        protected virtual bool CanSpawnOne() {
            return true; // this is for later subclasses to override (or ones you make!)
        }

        /// <summary>
        /// Returns the position to spawn an item in. Override for your custom logic.
        /// </summary>
        /// <param name="pos">Spawner position.</param>
        /// <param name="itemSpawnedIndex">Index (counter) of item to spawn. Incremental Settings use this.</param>
        /// <param name="wave">The wave that is being spawned.</param>
        /// <returns>The modified position to spawn in.</returns>
        protected virtual Vector3 GetSpawnPosition(Vector3 pos, int itemSpawnedIndex, TriggeredWaveMetaData wave) {
            switch (wave.waveSpec.positionXmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.x = wave.waveSpec.customPosX.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.waveSpec.otherObjectX != null) {
                        pos.x = wave.waveSpec.otherObjectX.position.x;
                    }
                    break;
            }

            switch (wave.waveSpec.positionYmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.y = wave.waveSpec.customPosY.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.waveSpec.otherObjectY != null) {
                        pos.y = wave.waveSpec.otherObjectY.position.y;
                    }
                    break;
            }

            switch (wave.waveSpec.positionZmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.z = wave.waveSpec.customPosZ.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.waveSpec.otherObjectZ != null) {
                        pos.z = wave.waveSpec.otherObjectZ.position.z;
                    }
                    break;
            }

            var addVector = Vector3.zero;

            var currentWave = wave.waveSpec;

            var offset = currentWave.WaveOffset;

            addVector += new Vector3(transform.right.x * offset.x,
                transform.up.y * offset.y,
                transform.forward.z * offset.z);

            if (currentWave.enableRandomizations) {
                addVector.x += Random.Range(-currentWave.randomDistX.Value, currentWave.randomDistX.Value);
                addVector.y += Random.Range(-currentWave.randomDistY.Value, currentWave.randomDistY.Value);
                addVector.z += Random.Range(-currentWave.randomDistZ.Value, currentWave.randomDistZ.Value);
            }

            if (!currentWave.enableIncrements || itemSpawnedIndex <= 0) {
                return pos + addVector;
            }

            addVector.x += (currentWave.incrementPositionX.Value * itemSpawnedIndex);
            addVector.y += (currentWave.incrementPositionY.Value * itemSpawnedIndex);
            addVector.z += (currentWave.incrementPositionZ.Value * itemSpawnedIndex);

            return pos + addVector;
        }

        /// <summary>
        /// Returns the rotation to spawn the item in. Override if you need custom logic for this.
        /// </summary>
        /// <param name="prefabToSpawn">The prefab to spawn.</param>
        /// <param name="itemSpawnedIndex">Index (counter) of item to spawn. Incremental Settings use this.</param>
        /// <param name="wave">The wave that is being spawned.</param>
        /// <returns>The modified rotation to spawn in.</returns>
        protected virtual Quaternion GetSpawnRotation(Transform prefabToSpawn, int itemSpawnedIndex,
            TriggeredWaveMetaData wave) {
            var currentWave = wave.waveSpec;

            var euler = Vector3.zero;

            switch (currentWave.curRotationMode) {
                case WaveSpecifics.RotationMode.UsePrefabRotation:
                    euler = prefabToSpawn.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.UseSpawnerRotation:
                    euler = Trans.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.CustomRotation:
                    euler = currentWave.customRotation;
                    break;
                case WaveSpecifics.RotationMode.LookAtCustomEventOrigin:
                    if (!currentWave.isCustomEvent) {
                        Debug.LogError(
                            "Spawn Rotation Mode is set to 'Look At Custom Event Origin' but that is invalid on non-custom event. Take a look in the Inspector for '" +
                            name + "'.");
                        break;
                    }

                    euler = currentWave.customEventLookRotation;
                    break;
            }

            if (wave.waveSpec.enableKeepCenter) {
                euler += wave.waveSpec.keepCenterRotation;
            }

            if (currentWave.enableRandomizations && currentWave.randomXRotation) {
                euler.x = Random.Range(wave.waveSpec.randomXRotMin.Value, wave.waveSpec.randomXRotMax.Value);
            } else if (currentWave.enableIncrements && itemSpawnedIndex > 0) {
                euler.x += itemSpawnedIndex * currentWave.incrementRotX.Value;
            }

            if (currentWave.enableRandomizations && currentWave.randomYRotation) {
                euler.y = Random.Range(wave.waveSpec.randomYRotMin.Value, wave.waveSpec.randomYRotMax.Value);
            } else if (currentWave.enableIncrements && itemSpawnedIndex > 0) {
                euler.y += itemSpawnedIndex * currentWave.incrementRotY.Value;
            }

            if (currentWave.enableRandomizations && currentWave.randomZRotation) {
                euler.z = Random.Range(wave.waveSpec.randomZRotMin.Value, wave.waveSpec.randomZRotMax.Value);
            } else if (currentWave.enableIncrements && itemSpawnedIndex > 0) {
                euler.z += itemSpawnedIndex * currentWave.incrementRotZ.Value;
            }

            return Quaternion.Euler(euler);
        }

        /// <summary>
        /// Fires immediately after the item spawns. Used by post-spawn nudge. Override if you need custom logic here.
        /// </summary>
        /// <param name="spawnedTrans">The spawned Transform.</param>
        /// <param name="wave">The wave the item came from.</param>
        /// <param name="eType">The event type.</param>
        protected virtual void AfterSpawn(Transform spawnedTrans, TriggeredWaveMetaData wave, TriggeredSpawner.EventType eType) {
            var currentWave = wave.waveSpec;

            if (currentWave.enablePostSpawnNudge) {
                spawnedTrans.Translate(Vector3.forward * currentWave.postSpawnNudgeFwd.Value);
                spawnedTrans.Translate(Vector3.right * currentWave.postSpawnNudgeRgt.Value);
                spawnedTrans.Translate(Vector3.down * currentWave.postSpawnNudgeDwn.Value);
            }

            switch (spawnLayerMode) {
                case WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnerSettings:
                    if (applyLayerRecursively) {
                        spawnedTrans.SetLayerOnAllChildren(_go.layer);
                    } else {
                        spawnedTrans.gameObject.layer = _go.layer;
                    }
                    break;
                case WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom:
                    if (applyLayerRecursively) {
                        spawnedTrans.SetLayerOnAllChildren(spawnCustomLayer);
                    } else {
                        spawnedTrans.gameObject.layer = spawnCustomLayer;
                    }
                    break;
            }

            switch (spawnTagMode) {
                case WaveSyncroPrefabSpawner.SpawnLayerTagMode.UseSpawnerSettings:
                    spawnedTrans.gameObject.tag = _go.tag;
                    break;
                case WaveSyncroPrefabSpawner.SpawnLayerTagMode.Custom:
                    spawnedTrans.gameObject.tag = spawnCustomTag;
                    break;
            }

            if (listener != null) {
                listener.ItemSpawned(eType, spawnedTrans);
            }
        }

        #endregion

        #region ICgkEventReceiver methods
        /*! \cond PRIVATE */
        public virtual void CheckForIllegalCustomEvents() {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < userDefinedEventWaves.Count; i++) {
                var custEvent = userDefinedEventWaves[i];

                LogIfCustomEventMissing(custEvent);
            }
        }

        public virtual void ReceiveEvent(string customEventName, Vector3 eventOrigin) {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < userDefinedEventWaves.Count; i++) {
                var userDefWave = userDefinedEventWaves[i];

                if (!userDefWave.customEventActive || string.IsNullOrEmpty(userDefWave.customEventName)) {
                    continue;
                }

                if (!userDefWave.customEventName.Equals(customEventName)) {
                    continue;
                }

                if (listener != null) {
                    listener.CustomEventReceived(customEventName, eventOrigin);
                }

                var oldRotation = Trans.rotation;

                if (userDefWave.eventOriginIgnoreX) {
                    eventOrigin.x = Trans.position.x;
                }
                if (userDefWave.eventOriginIgnoreY) {
                    eventOrigin.x = Trans.position.y;
                }
                if (userDefWave.eventOriginIgnoreZ) {
                    eventOrigin.x = Trans.position.z;
                }

                Trans.LookAt(eventOrigin);
                userDefWave.customEventLookRotation = Trans.rotation.eulerAngles;

                if (userDefWave.curSpawnerRotMode == WaveSpecifics.SpawnerRotationMode.KeepRotation) {
                    Trans.rotation = oldRotation;
                }

                if (!IsWaveValid(userDefWave, TriggeredSpawner.EventType.CustomEvent, false)) {
                    // not valid due to Retrigger Limit or something else.
                    continue;
                }

                SetupNextWave(userDefWave, TriggeredSpawner.EventType.CustomEvent);
            }
        }

        public virtual bool SubscribesToEvent(string customEventName) {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < userDefinedEventWaves.Count; i++) {
                var customGrp = userDefinedEventWaves[i];

                if (customGrp.customEventActive && !string.IsNullOrEmpty(customGrp.customEventName) &&
                    customGrp.customEventName.Equals(customEventName)) {
                    return true;
                }
            }

            return false;
        }

        public virtual void RegisterReceiver() {
            if (userDefinedEventWaves.Count > 0) {
                LevelSettings.AddCustomEventReceiver(this, Trans);
            }
        }

        public virtual void UnregisterReceiver() {
            if (userDefinedEventWaves.Count > 0) {
                LevelSettings.RemoveCustomEventReceiver(this);
            }
        }
        /*! \endcond */
        #endregion

#if UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
        #region uGUI Handlers
        // UGUI events are handled by separate components so that only
        // the events we care about are actually trapped and handled
        private void AddUGUIComponents() {
            var unityPointerEnterWave = GetFirstWaveIfAny(unityPointerEnterWaves);
            var unityPointerExitWave = GetFirstWaveIfAny(unityPointerExitWaves);
            var unityPointerDownWave = GetFirstWaveIfAny(unityPointerDownWaves);
            var unityPointerUpWave = GetFirstWaveIfAny(unityPointerUpWaves);
            var unityDragWave = GetFirstWaveIfAny(unityDragWaves);
            var unityDropWave = GetFirstWaveIfAny(unityDropWaves);
            var unityScrollWave = GetFirstWaveIfAny(unityScrollWaves);
            var unityUpdateSelectedWave = GetFirstWaveIfAny(unityUpdateSelectedWaves);
            var unitySelectWave = GetFirstWaveIfAny(unitySelectWaves);
            var unityDeselectWave = GetFirstWaveIfAny(unityDeselectWaves);
            var mouseOverWave = GetFirstWaveIfAny(mouseOverWaves);
            var unityInitializePotentialDragWave = GetFirstWaveIfAny(unityInitializePotentialDragWaves);
            var unityBeginDragWave = GetFirstWaveIfAny(unityBeginDragWaves);
            var unityEndDragWave = GetFirstWaveIfAny(unityEndDragWaves);
            var unitySubmitWave = GetFirstWaveIfAny(unitySubmitWaves);
            var unityCancelWave = GetFirstWaveIfAny(unityCancelWaves);

            AddUGUIHandler<TriggeredSpawnerPointerEnterHandler>(unityPointerEnterWave != null && unityPointerEnterWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerPointerExitHandler>(unityPointerExitWave != null && unityPointerExitWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerPointerDownHandler>(unityPointerDownWave != null && unityPointerDownWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerPointerUpHandler>(unityPointerUpWave != null && unityPointerUpWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerDragHandler>(unityDragWave != null && unityDragWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerDropHandler>(unityDropWave != null && unityDropWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerScrollHandler>(unityScrollWave != null && unityScrollWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerUpdateSelectedHandler>(unityUpdateSelectedWave != null && unityUpdateSelectedWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerSelectHandler>(unitySelectWave != null && unitySelectWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerDeselectHandler>(unityDeselectWave != null && unityDeselectWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerMoveHandler>(mouseOverWave != null && mouseOverWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerInitializePotentialDragHandler>(unityInitializePotentialDragWave != null && unityInitializePotentialDragWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerBeginDragHandler>(unityBeginDragWave != null && unityBeginDragWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerEndDragHandler>(unityEndDragWave != null && unityEndDragWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerSubmitHandler>(unitySubmitWave != null && unitySubmitWave.enableWave);
            AddUGUIHandler<TriggeredSpawnerCancelHandler>(unityCancelWave != null && unityCancelWave.enableWave);
        }

        private void AddUGUIHandler<T>(bool useEvent) where T : TriggeredSpawnerUGUIHandler {
            if (!useEvent) {
                return;
            }

            var handler = gameObject.AddComponent<T>();
            handler.trigSpawner = this;
        }

        // UGUI event handler components
        public class TriggeredSpawnerUGUIHandler : MonoBehaviour {
            // ReSharper disable once InconsistentNaming
            public TriggeredSpawnerV2 trigSpawner { get; set; }
        }

        public class TriggeredSpawnerPointerEnterHandler : TriggeredSpawnerUGUIHandler, IPointerEnterHandler {
            public void OnPointerEnter(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnPointerEnter(data);
                }
            }
        }

        public class TriggeredSpawnerPointerExitHandler : TriggeredSpawnerUGUIHandler, IPointerExitHandler {
            public void OnPointerExit(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnPointerExit(data);
                }
            }
        }

        public class TriggeredSpawnerPointerDownHandler : TriggeredSpawnerUGUIHandler, IPointerDownHandler {
            public void OnPointerDown(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnPointerDown(data);
                }
            }
        }

        public class TriggeredSpawnerPointerUpHandler : TriggeredSpawnerUGUIHandler, IPointerUpHandler {
            public void OnPointerUp(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnPointerUp(data);
                }
            }
        }

        public class TriggeredSpawnerDragHandler : TriggeredSpawnerUGUIHandler, IDragHandler {
            public void OnDrag(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnDrag(data);
                }
            }
        }

        public class TriggeredSpawnerDropHandler : TriggeredSpawnerUGUIHandler, IDropHandler {
            public void OnDrop(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnDrop(data);
                }
            }
        }

        public class TriggeredSpawnerScrollHandler : TriggeredSpawnerUGUIHandler, IScrollHandler {
            public void OnScroll(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnScroll(data);
                }
            }
        }

        public class TriggeredSpawnerUpdateSelectedHandler : TriggeredSpawnerUGUIHandler, IUpdateSelectedHandler {
            public void OnUpdateSelected(BaseEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnUpdateSelected(data);
                }
            }
        }

        public class TriggeredSpawnerSelectHandler : TriggeredSpawnerUGUIHandler, ISelectHandler {
            public void OnSelect(BaseEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnSelect(data);
                }
            }
        }

        public class TriggeredSpawnerDeselectHandler : TriggeredSpawnerUGUIHandler, IDeselectHandler {
            public void OnDeselect(BaseEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnDeselect(data);
                }
            }
        }

        public class TriggeredSpawnerMoveHandler : TriggeredSpawnerUGUIHandler, IMoveHandler {
            public void OnMove(AxisEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnMove(data);
                }
            }
        }

        public class TriggeredSpawnerInitializePotentialDragHandler : TriggeredSpawnerUGUIHandler, IInitializePotentialDragHandler {
            public void OnInitializePotentialDrag(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnInitializePotentialDrag(data);
                }
            }
        }

        public class TriggeredSpawnerBeginDragHandler : TriggeredSpawnerUGUIHandler, IBeginDragHandler {
            public void OnBeginDrag(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnBeginDrag(data);
                }
            }
        }

        public class TriggeredSpawnerEndDragHandler : TriggeredSpawnerUGUIHandler, IEndDragHandler {
            public void OnEndDrag(PointerEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnEndDrag(data);
                }
            }
        }

        public class TriggeredSpawnerSubmitHandler : TriggeredSpawnerUGUIHandler, ISubmitHandler {
            public void OnSubmit(BaseEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnSubmit(data);
                }
            }
        }

        public class TriggeredSpawnerCancelHandler : TriggeredSpawnerUGUIHandler, ICancelHandler {
            public void OnCancel(BaseEventData data) {
                if (trigSpawner != null) {
                    trigSpawner.OnCancel(data);
                }
            }
        }
        #endregion
#endif

        #region Helper methods

        private static Vector3 GetSpawnPositionForVisualization(TriggeredWaveSpecifics wave, Vector3 pos, int itemSpawnedIndex) {
            switch (wave.positionXmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.x = wave.customPosX.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectX != null) {
                        pos.x = wave.otherObjectX.position.x;
                    }
                    break;
            }

            switch (wave.positionYmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.y = wave.customPosY.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectY != null) {
                        pos.y = wave.otherObjectY.position.y;
                    }
                    break;
            }

            switch (wave.positionZmode) {
                case WaveSpecifics.PositionMode.CustomPosition:
                    pos.z = wave.customPosZ.Value;
                    break;
                case WaveSpecifics.PositionMode.OtherObjectPosition:
                    if (wave.otherObjectZ != null) {
                        pos.z = wave.otherObjectZ.position.z;
                    }
                    break;
            }

            var addVector = Vector3.zero;

            addVector += wave.WaveOffset;

            if (wave.enableRandomizations) {
                addVector.x += Random.Range(-wave.randomDistX.Value, wave.randomDistX.Value);
                addVector.y += Random.Range(-wave.randomDistY.Value, wave.randomDistY.Value);
                addVector.z += Random.Range(-wave.randomDistZ.Value, wave.randomDistZ.Value);
            }

            if (!wave.enableIncrements || itemSpawnedIndex <= 0) {
                return pos + addVector;
            }
            addVector.x += (wave.incrementPositionX.Value * itemSpawnedIndex);
            addVector.y += (wave.incrementPositionY.Value * itemSpawnedIndex);
            addVector.z += (wave.incrementPositionZ.Value * itemSpawnedIndex);

            return pos + addVector;
        }

        private static Quaternion GetSpawnRotationForVisualization(TriggeredWaveSpecifics wave, Transform spawner,
            int itemSpawnedIndex) {
            var euler = Vector3.zero;

            switch (wave.curRotationMode) {
                case WaveSpecifics.RotationMode.UsePrefabRotation:
                    break;
                case WaveSpecifics.RotationMode.UseSpawnerRotation:
                    euler = spawner.transform.rotation.eulerAngles;
                    break;
                case WaveSpecifics.RotationMode.CustomRotation:
                    euler = wave.customRotation;
                    break;
            }

            if (wave.enableRandomizations && wave.randomXRotation) {
                euler.x = Random.Range(wave.randomXRotMin.Value, wave.randomXRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.x += (itemSpawnedIndex * wave.incrementRotX.Value -
                                (wave.NumberToSpwn.Value * wave.incrementRotX.Value * .5f));
                } else {
                    euler.x += (itemSpawnedIndex * wave.incrementRotX.Value);
                }
            }

            if (wave.enableRandomizations && wave.randomYRotation) {
                euler.y = Random.Range(wave.randomYRotMin.Value, wave.randomYRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.y += (itemSpawnedIndex * wave.incrementRotY.Value -
                                (wave.NumberToSpwn.Value * wave.incrementRotY.Value * .5f));
                } else {
                    euler.y += (itemSpawnedIndex * wave.incrementRotY.Value);
                }
            }

            if (wave.enableRandomizations && wave.randomZRotation) {
                euler.z = Random.Range(wave.randomZRotMin.Value, wave.randomZRotMax.Value);
            } else if (wave.enableIncrements && itemSpawnedIndex > 0) {
                if (wave.enableKeepCenter) {
                    euler.z += (itemSpawnedIndex * wave.incrementRotZ.Value -
                                (wave.NumberToSpwn.Value * wave.incrementRotZ.Value * .5f));
                } else {
                    euler.z += (itemSpawnedIndex * wave.incrementRotZ.Value);
                }
            }

            return Quaternion.Euler(euler);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void AfterSpawnForVisualization(TriggeredWaveSpecifics wave, Transform spawnedTrans) {
            // ReSharper disable once InvertIf
            if (wave.enablePostSpawnNudge) {
                spawnedTrans.Translate(Vector3.forward * wave.postSpawnNudgeFwd.Value);
                spawnedTrans.Translate(Vector3.right * wave.postSpawnNudgeRgt.Value);
                spawnedTrans.Translate(Vector3.down * wave.postSpawnNudgeDwn.Value);
            }
        }

        /*! \cond PRIVATE */

        /// <summary>
        /// Called by "Copy to V2" script. Do not ever call.
        /// </summary>
        public void DeleteAllWaves() {
            if (Application.isPlaying) {
                return; // only run this at edit time.
            }

            enableWaves = new List<TriggeredWaveSpecifics>(0);
            disableWaves = new List<TriggeredWaveSpecifics>(0);
            visibleWaves = new List<TriggeredWaveSpecifics>(0);
            invisibleWaves = new List<TriggeredWaveSpecifics>(0);
            mouseOverWaves = new List<TriggeredWaveSpecifics>(0);
            mouseClickWaves = new List<TriggeredWaveSpecifics>(0);
            collisionWaves = new List<TriggeredWaveSpecifics>(0);
            triggerEnterWaves = new List<TriggeredWaveSpecifics>(0);
            triggerExitWaves = new List<TriggeredWaveSpecifics>(0);
            spawnedWaves = new List<TriggeredWaveSpecifics>(0);
            despawnedWaves = new List<TriggeredWaveSpecifics>(0);
            codeTriggeredWaves1 = new List<TriggeredWaveSpecifics>(0);
            codeTriggeredWaves2 = new List<TriggeredWaveSpecifics>(0);
            clickWaves = new List<TriggeredWaveSpecifics>(0);
            collision2dWaves = new List<TriggeredWaveSpecifics>(0);
            triggerEnter2dWaves = new List<TriggeredWaveSpecifics>(0);
            triggerExit2dWaves = new List<TriggeredWaveSpecifics>(0);
            userDefinedEventWaves = new List<TriggeredWaveSpecifics>(0);
            unitySliderChangedWaves = new List<TriggeredWaveSpecifics>(0);
            unityButtonClickedWaves = new List<TriggeredWaveSpecifics>(0);
            unityPointerDownWaves = new List<TriggeredWaveSpecifics>(0);
            unityPointerUpWaves = new List<TriggeredWaveSpecifics>(0);
            unityPointerEnterWaves = new List<TriggeredWaveSpecifics>(0);
            unityPointerExitWaves = new List<TriggeredWaveSpecifics>(0);
            unityDragWaves = new List<TriggeredWaveSpecifics>(0);
            unityDropWaves = new List<TriggeredWaveSpecifics>(0);
            unityScrollWaves = new List<TriggeredWaveSpecifics>(0);
            unityUpdateSelectedWaves = new List<TriggeredWaveSpecifics>(0);
            unitySelectWaves = new List<TriggeredWaveSpecifics>(0);
            unityDeselectWaves = new List<TriggeredWaveSpecifics>(0);
            unityMoveWaves = new List<TriggeredWaveSpecifics>(0);
            unityInitializePotentialDragWaves = new List<TriggeredWaveSpecifics>(0);
            unityBeginDragWaves = new List<TriggeredWaveSpecifics>(0);
            unityEndDragWaves = new List<TriggeredWaveSpecifics>(0);
            unitySubmitWaves = new List<TriggeredWaveSpecifics>(0);
            unityCancelWaves = new List<TriggeredWaveSpecifics>(0);
        }

        /// <summary>
        /// Called by Trig Spawn Tracker. Do not call.
        /// </summary>
        /// <param name="transDeadItem"></param>
        /// <param name="eType"></param>
        /// <param name="customEventName"></param>
        /// <param name="waveMeta"></param>
        public void RemoveItemFromWave(Transform transDeadItem, TriggeredSpawner.EventType eType, string customEventName, TriggeredWaveMetaData waveMeta) {
            if (transDeadItem == null) {
                return;
            }

            var probableWaveMeta = GetWaveMetaByType(eType, customEventName);

            if (probableWaveMeta == null) { // wave already ended or no Transform
                return;
            }

            if (waveMeta != probableWaveMeta) { // wave probably restarted already, so we don't remove
                return;
            }

            if (probableWaveMeta.spawnedWaveMembers.Contains(transDeadItem)) {
                probableWaveMeta.lastPrefabKilled = transDeadItem;
            }

            probableWaveMeta.spawnedWaveMembers.Remove(transDeadItem);
            if (probableWaveMeta.spawnedWaveMembers.Count > 0) {
                return;
            }

            if (probableWaveMeta.currentWaveSize - probableWaveMeta.countSpawned > 0) { // wave not done spawning but zero spawned items still left
                return;
            }

            waveMeta.eliminationTime = Time.time;

            if (waveMeta.waveSpec.repeatWaitsForType == RepeatWaitFor.ItemsDoneSpawning) {
                return; // this section wouldn't be visible anyway, so get out of here.
            }

            if (listener != null) {
                listener.WaveEliminated(eType, waveMeta.waveSpec);
            }

            SpawnBonusPrefabIfAny(waveMeta.waveSpec, probableWaveMeta.lastPrefabKilled.position);
            LevelSettings.TriggeredSpawnerWaveEliminated(eType, this, waveMeta.waveSpec);

            AwardEliminationBonusesAndFireEventsIfAny(waveMeta.waveSpec);
        }

        private void AwardEliminationBonusesAndFireEventsIfAny(TriggeredWaveSpecifics wave) {
            if (!wave.waveElimBonusesEnabled) {
                return;
            }

            // ReSharper disable once TooWideLocalVariableScope
            WorldVariableModifier mod;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < wave.waveElimVariableModifiers.statMods.Count; i++) {
                mod = wave.waveElimVariableModifiers.statMods[i];
                WorldVariableTracker.ModifyPlayerStat(mod, Trans);
            }

            // ReSharper disable once InvertIf
            if (wave.waveElimFireEvents) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < wave.waveElimCustomEvents.Count; i++) {
                    var anEvent = wave.waveElimCustomEvents[i].CustomEventName;

                    LevelSettings.FireCustomEventIfValid(anEvent, Trans);
                }
            }
        }

        private TriggeredWaveMetaData GetWaveMetaByType(TriggeredSpawner.EventType eType, string customEventName) {
            switch (eType) {
                case TriggeredSpawner.EventType.CodeTriggered1:
                    return _codeTriggeredWave1Meta;
                case TriggeredSpawner.EventType.CodeTriggered2:
                    return _codeTriggeredWave2Meta;
                case TriggeredSpawner.EventType.Invisible:
                    return _invisibleWaveMeta;
                case TriggeredSpawner.EventType.MouseClick_Legacy:
                    return _mouseClickWaveMeta;
                case TriggeredSpawner.EventType.MouseOver_Legacy:
                    return _mouseOverWaveMeta;
                case TriggeredSpawner.EventType.OnClick_NGUI:
                    return _clickWaveMeta;
                case TriggeredSpawner.EventType.OnCollision:
                    return _collisionWaveMeta;
                case TriggeredSpawner.EventType.OnCollision2D:
                    return _collision2DWaveMeta;
                case TriggeredSpawner.EventType.OnDespawned:
                    return _despawnedWaveMeta;
                case TriggeredSpawner.EventType.OnDisabled:
                    return _disableWaveMeta;
                case TriggeredSpawner.EventType.OnEnabled:
                    return _enableWaveMeta;
                case TriggeredSpawner.EventType.OnSpawned:
                    return _spawnedWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerEnter:
                    return _triggerEnterWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerStay:
                    return _triggerStayWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerExit:
                    return _triggerExitWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerEnter2D:
                    return _triggerEnter2DWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerStay2D:
                    return _triggerStay2DWaveMeta;
                case TriggeredSpawner.EventType.OnTriggerExit2D:
                    return _triggerExit2DWaveMeta;
                case TriggeredSpawner.EventType.SliderChanged_uGUI:
                    return _unitySliderChangedWaveMeta;
                case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                    return _unityButtonClickedWaveMeta;
                case TriggeredSpawner.EventType.PointerDown_uGUI:
                    return _unityPointerDownWaveMeta;
                case TriggeredSpawner.EventType.PointerUp_uGUI:
                    return _unityPointerUpWaveMeta;
                case TriggeredSpawner.EventType.PointerEnter_uGUI:
                    return _unityPointerEnterWaveMeta;
                case TriggeredSpawner.EventType.PointerExit_uGUI:
                    return _unityPointerExitWaveMeta;
                case TriggeredSpawner.EventType.Drag_uGUI:
                    return _unityDragWaveMeta;
                case TriggeredSpawner.EventType.Drop_uGUI:
                    return _unityDropWaveMeta;
                case TriggeredSpawner.EventType.Scroll_uGUI:
                    return _unityScrollWaveMeta;
                case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                    return _unityUpdateSelectedWaveMeta;
                case TriggeredSpawner.EventType.Select_uGUI:
                    return _unitySelectWaveMeta;
                case TriggeredSpawner.EventType.Deselect_uGUI:
                    return _unityDeselectWaveMeta;
                case TriggeredSpawner.EventType.Move_uGUI:
                    return _unityMoveWaveMeta;
                case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                    return _unityInitializePotentialDragWaveMeta;
                case TriggeredSpawner.EventType.BeginDrag_uGUI:
                    return _unityBeginDragWaveMeta;
                case TriggeredSpawner.EventType.EndDrag_uGUI:
                    return _unityEndDragWaveMeta;
                case TriggeredSpawner.EventType.Submit_uGUI:
                    return _unitySubmitWaveMeta;
                case TriggeredSpawner.EventType.Cancel_uGUI:
                    return _unityCancelWaveMeta;
                case TriggeredSpawner.EventType.Visible:
                    return _visibleWaveMeta;
                case TriggeredSpawner.EventType.CustomEvent:
                    for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                        var anEvent = _userDefinedEventWaveMeta[i];
                        if (!anEvent.waveSpec.customEventActive || anEvent.waveSpec.customEventName != customEventName) {
                            continue;
                        }

                        return anEvent;
                    }
                    break;
                default:
                    Debug.LogError("Illegal event type: " + eType.ToString());
                    break;
            }

            return null;
        }

        /*! \endcond */

        private void AddFirstWaveIfAny(TriggeredSpawner.EventType eType, List<TriggeredWaveSpecifics> listOfSingleWaveType, ref Dictionary<TriggeredSpawner.EventType, List<TriggeredWaveSpecifics>> allWaves) {
            var firstOfType = GetFirstWaveIfAny(listOfSingleWaveType);

            if (firstOfType == null) {
                return;
            }


            allWaves.Add(eType, new List<TriggeredWaveSpecifics> { firstOfType });
        }

        private TriggeredWaveSpecifics GetFirstWaveIfAny(List<TriggeredWaveSpecifics> waveList) {
            if (waveList.Count == 0) {
                return null;
            }

            return waveList[0];
        }

        private bool IsWaveValid(TriggeredWaveSpecifics wave, TriggeredSpawner.EventType eType, bool calledFromSelf) {
            if (GameIsOverForSpawner || !SpawnerIsActive) {
                return false;
            }

            if (wave == null || !wave.enableWave) {
                return false;
            }

            switch (eventSourceType) {
                case TriggeredSpawner.SpawnerEventSource.Self:
                    // just fine in all scenarios.
                    break;
                case TriggeredSpawner.SpawnerEventSource.ReceiveFromParent:
                    if (calledFromSelf) {
                        return false;
                    }
                    break;
                case TriggeredSpawner.SpawnerEventSource.None:
                    return false; // disabled!
            }

            // check for limiting restraints
            switch (wave.retriggerLimitMode) {
                case TriggeredSpawner.RetriggerLimitMode.FrameBased:
                    if (Time.frameCount - wave.trigLastFrame < wave.limitPerXFrm.Value) {
                        if (LevelSettings.IsLoggingOn) {
                            Debug.LogError(
                                string.Format(
                                    "{0} Wave of transform: '{1}' was limited by retrigger frame count setting.",
                                    eType.ToString(),
                                    Trans.name
                                    ));
                        }
                        return false;
                    }
                    break;
                case TriggeredSpawner.RetriggerLimitMode.TimeBased:
                    if (Time.time - wave.trigLastTime < wave.limitPerXSec.Value) {
                        if (LevelSettings.IsLoggingOn) {
                            Debug.LogError(
                                string.Format("{0} Wave of transform: '{1}' was limited by retrigger time setting.",
                                    eType.ToString(),
                                    Trans.name
                                    ));
                        }
                        return false;
                    }
                    break;
            }

            CheckForValidVariablesForWave(wave, eType);

            return true;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private float WaveEndTime(TriggeredWaveMetaData wave) {
            if (wave.waveSpec.repeatWaitsForType == RepeatWaitFor.ItemsDoneSpawning) {
                return wave.previousWaveEndTime;
            }

            return wave.eliminationTime;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private bool WaveEliminatedIfRequired(TriggeredWaveMetaData wave) {
            if (wave.waveSpec.repeatWaitsForType == RepeatWaitFor.ItemsDoneSpawning) {
                return true;
            }

            return wave.spawnedWaveMembers.Count == 0;
        }

        private bool CanRepeatWave(TriggeredWaveMetaData wave) {
            switch (wave.waveSpec.curWaveRepeatMode) {
                case WaveSpecifics.RepeatWaveMode.NumberOfRepetitions:
                    return wave.waveRepetitionNumber < wave.waveSpec.maxRepeat.Value;
                case WaveSpecifics.RepeatWaveMode.Endless:
                    return true;
                case WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < wave.waveSpec.repeatPassCriteria.statMods.Count; i++) {
                        var stat = wave.waveSpec.repeatPassCriteria.statMods[i];

                        if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                            LevelSettings.LogIfNew(
                                string.Format(
                                    "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                    Trans.name,
                                    stat._statName));
                            return false;
                        }

                        var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                        if (variable == null) {
                            return false;
                        }
                        var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? variable.CurrentIntValue
                            : variable.CurrentFloatValue;
                        var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? stat._modValueIntAmt.Value
                            : stat._modValueFloatAmt.Value;

                        if (varVal < compareVal) {
                            return true;
                        }
                    }

                    return false;
                case WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow:
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < wave.waveSpec.repeatPassCriteria.statMods.Count; i++) {
                        var stat = wave.waveSpec.repeatPassCriteria.statMods[i];

                        if (!WorldVariableTracker.VariableExistsInScene(stat._statName)) {
                            LevelSettings.LogIfNew(
                                string.Format(
                                    "Spawner '{0}' wants to repeat until World Variable '{1}' is a certain value, but that Variable is not in the Scene.",
                                    Trans.name,
                                    stat._statName));
                            return false;
                        }

                        var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                        if (variable == null) {
                            return false;
                        }

                        var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? variable.CurrentIntValue
                            : variable.CurrentFloatValue;
                        var compareVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                            ? stat._modValueIntAmt.Value
                            : stat._modValueFloatAmt.Value;

                        if (varVal > compareVal) {
                            return true;
                        }
                    }

                    return false;
                default:
                    LevelSettings.LogIfNew("Handle new wave repetition type: " + wave.waveSpec.curWaveRepeatMode);
                    return false;
            }
        }

        private void PropagateEndWaveToChildSpawners(TriggeredSpawner.EventType eType, string customEventName) {
            if (!transmitEventsToChildren) {
                return;
            }

            if (_childSpawners.Count <= 0) {
                return;
            }
            if (listener != null) {
                listener.PropagatedWaveEndedEarly(eType, customEventName, Trans, _childSpawners.Count);
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _childSpawners.Count; i++) {
                _childSpawners[i].EndWave(eType, customEventName);
            }
        }

        private void SpawnFromWaveMeta(TriggeredWaveMetaData wave, TriggeredSpawner.EventType eType) {
            if (wave == null || SpawnerIsPaused) {
                return;
            }

            if (wave.waveFinishedSpawning // done spawning, wait
                || (Time.time - wave.waveStartTime < WaveDelay(wave)) // Wave Delay not done
                || (Time.time - wave.lastSpawnTime <= wave.singleSpawnTime && wave.singleSpawnTime > Time.deltaTime)) { // still waiting for "single spawn time"

                if (!wave.waveFinishedSpawning
                    || !wave.waveSpec.enableRepeatWave
                    || !CanRepeatWave(wave)
                    || !WaveEliminatedIfRequired(wave)
                    || !(Time.time - WaveEndTime(wave) > wave.waveSpec.repeatWavePauseSec.Value)) {

                    return;
                }
                if (!SetupNextWave(wave.waveSpec, eType, wave.waveRepetitionNumber, wave.waveRepetitionWithResetNum)) {
                    return;
                }
                if (listener != null) {
                    listener.WaveRepeat(eType, wave.waveSpec);
                }

                return;
            }

            var numberToSpawn = 1;

            if (wave.currentWaveSize > 0) {
                if (wave.singleSpawnTime < Time.deltaTime) {
                    if (wave.singleSpawnTime == 0) {
                        numberToSpawn = wave.currentWaveSize;
                    } else {
                        numberToSpawn = (int)Math.Ceiling(Time.deltaTime / wave.singleSpawnTime);
                    }
                }
            } else {
                numberToSpawn = 0;
            }

            for (var i = 0; i < numberToSpawn; i++) {
                if (CanSpawnOne()) {
                    var prefabToSpawn = GetSpawnable(wave);
                    if (wave.waveSpec.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool && prefabToSpawn == null) {
                        // "no item"
                        continue;
                    }
                    if (prefabToSpawn == null) {
                        LevelSettings.LogIfNew(
                            string.Format("Triggered Spawner '{0}' has no prefab to spawn for event: {1}",
                                name,
                                eType.ToString()));

                        switch (eType) {
                            case TriggeredSpawner.EventType.OnEnabled:
                                EndWave(TriggeredSpawner.EventType.OnEnabled, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnDisabled:
                                EndWave(TriggeredSpawner.EventType.OnDisabled, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Visible:
                                EndWave(TriggeredSpawner.EventType.Visible, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Invisible:
                                EndWave(TriggeredSpawner.EventType.Invisible, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.MouseOver_Legacy:
                                EndWave(TriggeredSpawner.EventType.MouseOver_Legacy, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.MouseClick_Legacy:
                                EndWave(TriggeredSpawner.EventType.MouseClick_Legacy, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnCollision:
                                EndWave(TriggeredSpawner.EventType.OnCollision, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerEnter:
                                EndWave(TriggeredSpawner.EventType.OnTriggerEnter, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerStay:
                                EndWave(TriggeredSpawner.EventType.OnTriggerStay, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerExit:
                                EndWave(TriggeredSpawner.EventType.OnTriggerExit, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnSpawned:
                                EndWave(TriggeredSpawner.EventType.OnSpawned, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnDespawned:
                                EndWave(TriggeredSpawner.EventType.OnDespawned, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.CodeTriggered1:
                                EndWave(TriggeredSpawner.EventType.CodeTriggered1, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.CodeTriggered2:
                                EndWave(TriggeredSpawner.EventType.CodeTriggered2, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnClick_NGUI:
                                EndWave(TriggeredSpawner.EventType.OnClick_NGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnCollision2D:
                                EndWave(TriggeredSpawner.EventType.OnCollision2D, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerEnter2D:
                                EndWave(TriggeredSpawner.EventType.OnTriggerEnter2D, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerStay2D:
                                EndWave(TriggeredSpawner.EventType.OnTriggerStay2D, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.OnTriggerExit2D:
                                EndWave(TriggeredSpawner.EventType.OnTriggerExit2D, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.CustomEvent:
                                EndWave(TriggeredSpawner.EventType.CustomEvent, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.BeginDrag_uGUI:
                                EndWave(TriggeredSpawner.EventType.BeginDrag_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                                EndWave(TriggeredSpawner.EventType.ButtonClicked_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Cancel_uGUI:
                                EndWave(TriggeredSpawner.EventType.Cancel_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Deselect_uGUI:
                                EndWave(TriggeredSpawner.EventType.Deselect_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Drag_uGUI:
                                EndWave(TriggeredSpawner.EventType.Drag_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Drop_uGUI:
                                EndWave(TriggeredSpawner.EventType.Drop_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.EndDrag_uGUI:
                                EndWave(TriggeredSpawner.EventType.EndDrag_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                                EndWave(TriggeredSpawner.EventType.InitializePotentialDrag_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.PointerDown_uGUI:
                                EndWave(TriggeredSpawner.EventType.PointerDown_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.PointerEnter_uGUI:
                                EndWave(TriggeredSpawner.EventType.PointerEnter_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.PointerExit_uGUI:
                                EndWave(TriggeredSpawner.EventType.PointerExit_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.PointerUp_uGUI:
                                EndWave(TriggeredSpawner.EventType.PointerUp_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Scroll_uGUI:
                                EndWave(TriggeredSpawner.EventType.Scroll_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Select_uGUI:
                                EndWave(TriggeredSpawner.EventType.Select_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.SliderChanged_uGUI:
                                EndWave(TriggeredSpawner.EventType.SliderChanged_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.Submit_uGUI:
                                EndWave(TriggeredSpawner.EventType.Submit_uGUI, wave.waveSpec.customEventName);
                                break;
                            case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                                EndWave(TriggeredSpawner.EventType.UpdateSelected_uGUI, wave.waveSpec.customEventName);
                                break;
                            default:
                                LevelSettings.LogIfNew("need event stop code for event: " + eType.ToString());
                                break;
                        }

                        return;
                    }

                    var spawnPosition = GetSpawnPosition(Trans.position, wave.countSpawned, wave);

                    var spawnedPrefab = SpawnWaveItem(prefabToSpawn, spawnPosition, GetSpawnRotation(prefabToSpawn, wave.countSpawned, wave), wave);

                    if (!LevelSettings.AppIsShuttingDown) {
                        if (spawnedPrefab == null) {
                            if (listener != null) {
                                listener.ItemFailedToSpawn(eType, prefabToSpawn);
                            }

                            LevelSettings.LogIfNew("Could not spawn: " + prefabToSpawn);

                            return;
                        }

                        SpawnUtility.RecordSpawnerObjectIfKillable(spawnedPrefab, _go);

                        AddSpawnTracker(spawnedPrefab, eType, wave);
                    }

                    AfterSpawn(spawnedPrefab, wave, eType);
                }

                wave.countSpawned++;

                if (wave.countSpawned >= wave.currentWaveSize) {
                    if (LevelSettings.IsLoggingOn) {
                        Debug.Log(string.Format("Triggered Spawner '{0}' finished spawning wave from event: {1}.",
                            name,
                            eType));
                    }
                    wave.waveFinishedSpawning = true;
                    if (listener != null) {
                        listener.WaveFinishedSpawning(eType, wave.waveSpec);
                    }

                    if (wave.waveSpec.enableRepeatWave) {
                        wave.previousWaveEndTime = Time.time;
                        wave.waveRepetitionNumber++;
                        wave.waveRepetitionWithResetNum++;

						if (!CanRepeatWave(wave)) {
							if (wave.waveSpec.disableAfterFirstTrigger) {
								wave.waveSpec.enableWave = false;
							}
						}
					} else {
						if (wave.waveSpec.disableAfterFirstTrigger) {
							wave.waveSpec.enableWave = false;
						}
					}
                }

                wave.lastSpawnTime = Time.time;
            }

            AfterSpawnWave(wave);
        }

        private void AddSpawnTracker(Transform spawnedTrans, TriggeredSpawner.EventType eType, TriggeredWaveMetaData waveMeta) {
            var wrongTracker = spawnedTrans.GetComponent<SpawnTracker>();
            if (wrongTracker == null) {
                GameObject.DestroyImmediate(wrongTracker);
            }

            var tracker = spawnedTrans.GetComponent<TrigSpawnTracker>();
            if (tracker == null) {
                spawnedTrans.gameObject.AddComponent(typeof(TrigSpawnTracker));
                tracker = spawnedTrans.GetComponent<TrigSpawnTracker>();
            }

            tracker.StartTracking(this, eType, waveMeta.waveSpec.customEventName, waveMeta);
        }

        private void AfterSpawnWave(TriggeredWaveMetaData newWave) {
            if (!newWave.waveSpec.willDespawnOnEvent) {
                return;
            }
            if (listener != null) {
                listener.SpawnerDespawning(Trans);
            }

            DespawnSpawner();
        }

        private bool SetupNextWave(TriggeredWaveSpecifics newWave, TriggeredSpawner.EventType eventType, int repetitionNumber = 0, int repetitionNumberWithReset = 0) {
            if (newWave == null || !newWave.enableWave) {
                // even in repeating waves we need to check.
                return false;
            }

            if (LevelSettings.IsLoggingOn) {
                Debug.Log(string.Format("Starting wave from triggered spawner: {0}, event: {1}.",
                    name,
                    eventType.ToString()));
            }

            // award bonuses
            if (newWave.waveSpawnBonusesEnabled) {
                if ((repetitionNumber == 0 && newWave.useWaveSpawnBonusForBeginning) || (repetitionNumber != 0 && newWave.useWaveSpawnBonusForRepeats)) {
                    // ReSharper disable once TooWideLocalVariableScope
                    WorldVariableModifier mod;

                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < newWave.waveSpawnVariableModifiers.statMods.Count; i++) {
                        mod = newWave.waveSpawnVariableModifiers.statMods[i];
                        WorldVariableTracker.ModifyPlayerStat(mod, Trans);
                    }
                }

                if (newWave.waveSpawnFireEvents) {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < newWave.waveSpawnCustomEvents.Count; i++) {
                        var anEvent = newWave.waveSpawnCustomEvents[i].CustomEventName;

                        LevelSettings.FireCustomEventIfValid(anEvent, Trans);
                    }
                }
            }

            if (repetitionNumber > 0) {
                if (newWave.waveRepeatFireEvents) {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < newWave.waveRepeatCustomEvents.Count; i++) {
                        var anEvent = newWave.waveRepeatCustomEvents[i].CustomEventName;

                        LevelSettings.FireCustomEventIfValid(anEvent, Trans);
                    }
                }
            }

            WavePrefabPool myWavePool;

            if (newWave.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool) {
                var poolTrans = LevelSettings.GetFirstMatchingPrefabPool(newWave.prefabPoolName);
                if (poolTrans == null) {
                    LevelSettings.LogIfNew(
                        string.Format("Spawner '{0}' event: {1} is trying to use a Prefab Pool that can't be found.",
                            name,
                            eventType.ToString()));
                    return false;
                }

                myWavePool = poolTrans;
            } else {
                myWavePool = null;
            }

            var larger = Math.Max(newWave.NumberToSpwn.Value, newWave.MaxToSpawn.Value);
            var smaller = Math.Min(newWave.NumberToSpwn.Value, newWave.MaxToSpawn.Value);

            var origCurrentWaveSize = Random.Range(smaller, larger + 1);
            var myCurrentWaveSize = origCurrentWaveSize;

            if (newWave.enableRepeatWave) {
                if (newWave.repeatItemInc.Value != 0) {
                    myCurrentWaveSize += repetitionNumberWithReset * newWave.repeatItemInc.Value;

                    var resetQty = false;

                    if (myCurrentWaveSize < newWave.repeatItemMinLmt.Value) {
                        if (newWave.resetOnItemLimitReached) {
                            resetQty = true;
                        } else {
                            myCurrentWaveSize = newWave.repeatItemMinLmt.Value;
                        }
                    } else if (myCurrentWaveSize > newWave.repeatItemLmt.Value) {
                        if (newWave.resetOnItemLimitReached) {
                            resetQty = true;
                        } else {
                            myCurrentWaveSize = newWave.repeatItemLmt.Value;
                        }
                    }

                    if (resetQty) {
                        repetitionNumberWithReset = 0;
                        myCurrentWaveSize = origCurrentWaveSize;
                    }
                }
            }

            myCurrentWaveSize = Math.Max(0, myCurrentWaveSize);

            var timeToSpawnWave = newWave.TimeToSpawnEntireWave.Value;
            var origSpawnWaveTime = timeToSpawnWave;

            if (newWave.enableRepeatWave) {
                if (newWave.repeatTimeInc.Value != 0) {
                    timeToSpawnWave += repetitionNumberWithReset * newWave.repeatTimeInc.Value;

                    var resetTime = false;

                    if (timeToSpawnWave < newWave.repeatTimeMinLmt.Value) {
                        if (newWave.resetOnTimeLimitReached) {
                            resetTime = true;
                        } else {
                            timeToSpawnWave = newWave.repeatTimeMinLmt.Value;
                        }
                    } else if (timeToSpawnWave > newWave.repeatTimeLmt.Value) {
                        if (newWave.resetOnTimeLimitReached) {
                            resetTime = true;
                        } else {
                            timeToSpawnWave = newWave.repeatTimeLmt.Value;
                        }
                    }

                    if (resetTime) {
                        timeToSpawnWave = origSpawnWaveTime;
                        repetitionNumberWithReset = 0;
                    }
                }
            }

            timeToSpawnWave = Math.Max(0f, timeToSpawnWave);

            var mySingleSpawnTime = timeToSpawnWave / myCurrentWaveSize;

            var newMetaWave = new TriggeredWaveMetaData() {
                wavePool = myWavePool,
                currentWaveSize = myCurrentWaveSize,
                waveStartTime = Time.time,
                singleSpawnTime = mySingleSpawnTime,
                waveSpec = newWave,
                waveRepetitionNumber = repetitionNumber,
                waveRepetitionWithResetNum = repetitionNumberWithReset,
                spawnedWaveMembers = new List<Transform>(myCurrentWaveSize)
            };


            if (newWave.enableKeepCenter) {
                var waveCalcSize = (myCurrentWaveSize - 1) * -.5f;

                newMetaWave.waveSpec.keepCenterRotation = new Vector3() {
                    x = waveCalcSize * newMetaWave.waveSpec.incrementRotX.Value,
                    y = waveCalcSize * newMetaWave.waveSpec.incrementRotY.Value,
                    z = waveCalcSize * newMetaWave.waveSpec.incrementRotZ.Value,
                };
            } else {
                newMetaWave.waveSpec.keepCenterRotation = Vector3.zero;
            }

            switch (eventType) {
                case TriggeredSpawner.EventType.OnEnabled:
                    _enableWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnDisabled:
                    _disableWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Visible:
                    _visibleWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Invisible:
                    _invisibleWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.MouseOver_Legacy:
                    _mouseOverWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.MouseClick_Legacy:
                    _mouseClickWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnCollision:
                    _collisionWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter:
                    _triggerEnterWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay:
                    _triggerStayWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit:
                    _triggerExitWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnSpawned:
                    _spawnedWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnDespawned:
                    _despawnedWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.CodeTriggered1:
                    _codeTriggeredWave1Meta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.CodeTriggered2:
                    _codeTriggeredWave2Meta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnClick_NGUI:
                    _clickWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnCollision2D:
                    _collision2DWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerEnter2D:
                    _triggerEnter2DWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerStay2D:
                    _triggerStay2DWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.OnTriggerExit2D:
                    _triggerExit2DWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.SliderChanged_uGUI:
                    _unitySliderChangedWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.ButtonClicked_uGUI:
                    _unityButtonClickedWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.PointerDown_uGUI:
                    _unityPointerDownWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.PointerUp_uGUI:
                    _unityPointerUpWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.PointerEnter_uGUI:
                    _unityPointerEnterWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.PointerExit_uGUI:
                    _unityPointerExitWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Drag_uGUI:
                    _unityDragWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Drop_uGUI:
                    _unityDropWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Scroll_uGUI:
                    _unityScrollWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.UpdateSelected_uGUI:
                    _unityUpdateSelectedWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Select_uGUI:
                    _unitySelectWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Deselect_uGUI:
                    _unityDeselectWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Move_uGUI:
                    _unityMoveWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.InitializePotentialDrag_uGUI:
                    _unityInitializePotentialDragWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.BeginDrag_uGUI:
                    _unityBeginDragWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.EndDrag_uGUI:
                    _unityEndDragWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Submit_uGUI:
                    _unitySubmitWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.Cancel_uGUI:
                    _unityCancelWaveMeta = newMetaWave;
                    break;
                case TriggeredSpawner.EventType.CustomEvent:
                    // remove existing
                    int? matchIndex = null;
                    for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                        var aWave = _userDefinedEventWaveMeta[i];
                        if (aWave.waveSpec.customEventName != newMetaWave.waveSpec.customEventName) {
                            continue;
                        }
                        matchIndex = i;
                        break;
                    }

                    if (matchIndex.HasValue) {
                        _userDefinedEventWaveMeta.RemoveAt(matchIndex.Value);
                    }

                    _userDefinedEventWaveMeta.Add(newMetaWave);
                    break;
                default:
                    LevelSettings.LogIfNew("No matching event type: " + eventType.ToString());
                    return false;
            }

            switch (newMetaWave.waveSpec.retriggerLimitMode) {
                case TriggeredSpawner.RetriggerLimitMode.FrameBased:
                    newMetaWave.waveSpec.trigLastFrame = Time.frameCount;
                    break;
                case TriggeredSpawner.RetriggerLimitMode.TimeBased:
                    newMetaWave.waveSpec.trigLastTime = Time.time;
                    break;
            }

            newMetaWave.lastSpawnTime = Time.time + WaveDelay(newMetaWave) - newMetaWave.singleSpawnTime;

            return true;
        }

        private void LogIfCustomEventMissing(TriggeredWaveSpecifics eventGroup) {
            if (!logMissingEvents) {
                return;
            }

            if (!eventGroup.customEventActive || string.IsNullOrEmpty(eventGroup.customEventName)) {
                return;
            }

            var customEventName = eventGroup.customEventName;

            if (customEventName != LevelSettings.NoEventName && !LevelSettings.CustomEventExists(customEventName)) {
                LevelSettings.LogIfNew("Transform '" + name + "' is set up to receive or fire Custom Event '" +
                                       customEventName + "', which does not exist in Core GameKit.");
            }
        }

        private void CheckForValidVariablesForWave(TriggeredWaveSpecifics wave, TriggeredSpawner.EventType eType) {
            if (wave == null || !wave.enableWave) {
                return; // no need to check.
            }

            // check KillerInts for invalid types
            wave.NumberToSpwn.LogIfInvalid(Trans, "Min To Spawn", null, null, eType.ToString());
            wave.MaxToSpawn.LogIfInvalid(Trans, "Max To Spawn", null, null, eType.ToString());
            wave.maxRepeat.LogIfInvalid(Trans, "Wave Repetitions", null, null, eType.ToString());
            wave.repeatItemInc.LogIfInvalid(Trans, "Spawn Increase", null, null, eType.ToString());
            wave.repeatItemMinLmt.LogIfInvalid(Trans, "Spawn Min Limit", null, null, eType.ToString());
            wave.repeatItemLmt.LogIfInvalid(Trans, "Spawn Max Limit", null, null, eType.ToString());
            wave.repeatTimeInc.LogIfInvalid(Trans, "Time Increase", null, null, eType.ToString());
            wave.repeatTimeMinLmt.LogIfInvalid(Trans, "Time Min Limit", null, null, eType.ToString());
            wave.repeatTimeLmt.LogIfInvalid(Trans, "Time Max Limit", null, null, eType.ToString());

            wave.limitPerXFrm.LogIfInvalid(Trans, "Retrigger Min Frames Between", null, null, eType.ToString());

            if (wave.positionXmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosX.LogIfInvalid(Trans, "Custom X Position", null, null, eType.ToString());
            }
            if (wave.positionYmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosY.LogIfInvalid(Trans, "Custom Y Position", null, null, eType.ToString());
            }
            if (wave.positionZmode == WaveSpecifics.PositionMode.CustomPosition) {
                wave.customPosZ.LogIfInvalid(Trans, "Custom Z Position", null, null, eType.ToString());
            }

            // check KillerFloats for invalid types
            wave.WaveDelaySec.LogIfInvalid(Trans, "Delay Wave (sec)", null, null, eType.ToString());
            wave.TimeToSpawnEntireWave.LogIfInvalid(Trans, "Time To Spawn All", null, null, eType.ToString());
            wave.repeatWavePauseSec.LogIfInvalid(Trans, "Pause Before Repeat", null, null, eType.ToString());
            wave.repeatTimeInc.LogIfInvalid(Trans, "Repeat Time Increase", null, null, eType.ToString());
            wave.repeatTimeMinLmt.LogIfInvalid(Trans, "Repeat Time Min Limit", null, null, eType.ToString());
            wave.repeatTimeLmt.LogIfInvalid(Trans, "Repeat Time Max Limit", null, null, eType.ToString());
            wave.randomDistX.LogIfInvalid(Trans, "Rand. Distance X", null, null, eType.ToString());
            wave.randomDistY.LogIfInvalid(Trans, "Rand. Distance Y", null, null, eType.ToString());
            wave.randomDistZ.LogIfInvalid(Trans, "Rand. Distance Z", null, null, eType.ToString());
            wave.randomXRotMin.LogIfInvalid(Trans, "Rand. X Rot. Min", null, null, eType.ToString());
            wave.randomXRotMax.LogIfInvalid(Trans, "Rand. X Rot. Max", null, null, eType.ToString());
            wave.randomYRotMin.LogIfInvalid(Trans, "Rand. Y Rot. Min", null, null, eType.ToString());
            wave.randomYRotMax.LogIfInvalid(Trans, "Rand. Y Rot. Max", null, null, eType.ToString());
            wave.randomZRotMin.LogIfInvalid(Trans, "Rand. Z Rot. Min", null, null, eType.ToString());
            wave.randomZRotMax.LogIfInvalid(Trans, "Rand. Z Rot. Max", null, null, eType.ToString());
            wave.incrementPositionX.LogIfInvalid(Trans, "Incremental Distance X", null, null, eType.ToString());
            wave.incrementPositionY.LogIfInvalid(Trans, "Incremental Distance Y", null, null, eType.ToString());
            wave.incrementPositionZ.LogIfInvalid(Trans, "Incremental Distance Z", null, null, eType.ToString());
            wave.incrementRotX.LogIfInvalid(Trans, "Incremental Rotation X", null, null, eType.ToString());
            wave.incrementRotY.LogIfInvalid(Trans, "Incremental Rotation Y", null, null, eType.ToString());
            wave.incrementRotZ.LogIfInvalid(Trans, "Incremental Rotation Z", null, null, eType.ToString());
            wave.postSpawnNudgeFwd.LogIfInvalid(Trans, "Nudge Forward", null, null, eType.ToString());
            wave.postSpawnNudgeRgt.LogIfInvalid(Trans, "Nudge Right", null, null, eType.ToString());
            wave.postSpawnNudgeDwn.LogIfInvalid(Trans, "Nudge Down", null, null, eType.ToString());
            wave.limitPerXSec.LogIfInvalid(Trans, "Retrigger Min Seconds Between", null, null, eType.ToString());

            if (wave.curWaveRepeatMode == WaveSpecifics.RepeatWaveMode.UntilWorldVariableAbove ||
                wave.curWaveRepeatMode == WaveSpecifics.RepeatWaveMode.UntilWorldVariableBelow) {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < wave.repeatPassCriteria.statMods.Count; i++) {
                    var crit = wave.repeatPassCriteria.statMods[i];

                    switch (crit._varTypeToUse) {
                        case WorldVariableTracker.VariableType._integer:
                            if (crit._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (!WorldVariableTracker.VariableExistsInScene(crit._modValueIntAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            crit._modValueIntAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' has a Repeat Item Limit criteria with no World Variable selected. Please select one.",
                                                Trans.name,
                                                eType));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1} has a Repeat Item Limit using the value of World Variable '{2}', which doesn't exist in the Scene.",
                                                Trans.name,
                                                eType,
                                                crit._modValueIntAmt.worldVariableName));
                                    }
                                }
                            }

                            break;
                        case WorldVariableTracker.VariableType._float:
                            if (crit._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(crit._modValueFloatAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            crit._modValueFloatAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' has a Repeat Item Limit criteria with no World Variable selected. Please select one.",
                                                Trans.name,
                                                eType));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' has a Repeat Item Limit using the value of World Variable '{2}', which doesn't exist in the Scene.",
                                                Trans.name,
                                                eType,
                                                crit._modValueFloatAmt.worldVariableName));
                                    }
                                }
                            }

                            break;
                        default:
                            LevelSettings.LogIfNew("Add code for varType: " + crit._varTypeToUse.ToString());
                            break;
                    }
                }
            }

            if (!wave.waveSpawnBonusesEnabled) {
                return;
            }
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var b = 0; b < wave.waveSpawnVariableModifiers.statMods.Count; b++) {
                var spawnMod = wave.waveSpawnVariableModifiers.statMods[b];

                if (WorldVariableTracker.IsBlankVariableName(spawnMod._statName)) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Spawner '{0}', event '{1}' specifies a Wave Spawn Bonus with no World Variable selected. Please select one.",
                            Trans.name,
                            eType));
                } else if (!WorldVariableTracker.VariableExistsInScene(spawnMod._statName)) {
                    LevelSettings.LogIfNew(
                        string.Format(
                            "Spawner '{0}', event '{1}' specifies a Wave Spawn Bonus of World Variable '{2}', which doesn't exist in the scene.",
                            Trans.name,
                            eType,
                            spawnMod._statName));
                } else {
                    switch (spawnMod._varTypeToUse) {
                        case WorldVariableTracker.VariableType._integer:
                            if (spawnMod._modValueIntAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(
                                        spawnMod._modValueIntAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            spawnMod._modValueIntAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' wants to award Wave Spawn Bonus if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                Trans.name,
                                                eType,
                                                spawnMod._statName));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' wants to award Wave Spawn Bonus if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                Trans.name,
                                                eType,
                                                spawnMod._statName,
                                                spawnMod._modValueIntAmt.worldVariableName));
                                    }
                                }
                            }

                            break;
                        case WorldVariableTracker.VariableType._float:
                            if (spawnMod._modValueFloatAmt.variableSource == LevelSettings.VariableSource.Variable) {
                                if (
                                    !WorldVariableTracker.VariableExistsInScene(
                                        spawnMod._modValueFloatAmt.worldVariableName)) {
                                    if (
                                        LevelSettings.IllegalVariableNames.Contains(
                                            spawnMod._modValueFloatAmt.worldVariableName)) {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' wants to award Wave Spawn Bonus if World Variable '{2}' is above the value of an unspecified World Variable. Please select one.",
                                                Trans.name,
                                                eType,
                                                spawnMod._statName));
                                    } else {
                                        LevelSettings.LogIfNew(
                                            string.Format(
                                                "Spawner '{0}', event '{1}' wants to award Wave Spawn Bonus if World Variable '{2}' is above the value of World Variable '{3}', but the latter is not in the Scene.",
                                                Trans.name,
                                                eType,
                                                spawnMod._statName,
                                                spawnMod._modValueFloatAmt.worldVariableName));
                                    }
                                }
                            }

                            break;
                        default:
                            LevelSettings.LogIfNew("Add code for varType: " + spawnMod._varTypeToUse.ToString());
                            break;
                    }
                }
            }
        }

        private static bool WaveIsUsingPrefabPool(TriggeredWaveSpecifics spec, string poolName) {
            if (spec == null || !spec.enableWave) {
                return false;
            }

            if (spec.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool && spec.prefabPoolName == poolName) {
                return true;
            }

            return false;
        }

        private void StopOppositeWaveIfActive(TriggeredWaveSpecifics wave, TriggeredSpawner.EventType eType) {
            if (wave == null) {
                return;
            }

            if (wave.enableWave && wave.stopWaveOnOppositeEvent) {
                EndWave(eType, string.Empty);
            }
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private float WaveDelay(TriggeredWaveMetaData wave) {
            var delay = wave.waveSpec.WaveDelaySec.Value;
            if (wave.waveRepetitionNumber > 0 && !wave.waveSpec.doesRepeatUseWaveDelay) {
                delay = 0;
            }
            return delay;
        }

        private void SpawnBonusPrefabIfAny(TriggeredWaveSpecifics currentWave, Vector3 spawnPosition) {
            var waveBeat = currentWave;

            if (currentWave.repeatWaitsForType != RepeatWaitFor.ItemsEliminated) {
                return;
            }

            if (!waveBeat.useSpawnBonusPrefab) {
                return;
            }

            if (Random.Range(0, 100) >= waveBeat.bonusPrefabSpawnPercent.Value) {
                return;
            }

            switch (waveBeat.bonusPrefabSource) {
                case WaveSpecifics.SpawnOrigin.PrefabPool:
                    var bonusPrefabPool = LevelSettings.GetFirstMatchingPrefabPool(waveBeat.bonusPrefabPoolName);
                    if (bonusPrefabPool == null) {
                        LevelSettings.LogIfNew("Could not find Prefab Pool '" + waveBeat.bonusPrefabPoolName + "' for Bonus Prefab on Level: " + LevelSettings.CurrentDisplayLevel + ", Wave: " + LevelSettings.CurrentDisplayWave);
                        return;
                    }

                    var numToSpawn = waveBeat.bonusPrefabQty.Value;
                    for (var i = 0; i < numToSpawn; i++) {
                        var prefabToSpawn = bonusPrefabPool.GetRandomWeightedTransform();
                        if (prefabToSpawn == null) {
                            LevelSettings.LogIfNew("Prefab Pool '" + bonusPrefabPool.name + "' has no items left to spawn for Bonus prefab.");
                            break;
                        }
                        var spawned = PoolBoss.SpawnInPool(prefabToSpawn, spawnPosition, Quaternion.identity);
                        if (spawned == null) {
                            break;
                        }
                    }

                    break;
                case WaveSpecifics.SpawnOrigin.Specific:
                    if (waveBeat.bonusPrefabSpecific == null) {
                        LevelSettings.LogIfNew("Bonus Prefab for Level: " + LevelSettings.CurrentDisplayLevel + ", Wave: " + LevelSettings.CurrentDisplayWave + " is unassigned.");
                        return;
                    }

                    var numberToSpawn = waveBeat.bonusPrefabQty.Value;
                    for (var i = 0; i < numberToSpawn; i++) {
                        var spawned = PoolBoss.SpawnInPool(waveBeat.bonusPrefabSpecific, spawnPosition, Quaternion.identity);
                        if (spawned == null) {
                            break;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// This returns a list of all the active elimination waves. 
        /// </summary>
        public List<TrigSpawnerWaiterSpecifics> ActiveEliminationWaves {
            get {
                var activeWaves = new List<TrigSpawnerWaiterSpecifics>();
                var allWaves = AllWaves;
                foreach (var key in allWaves.Keys) {
                    var waves = allWaves[key];

                    foreach (var wave in waves) {
                        if (!wave.enableWave || wave.repeatWaitsForType != RepeatWaitFor.ItemsEliminated) {
                            continue;
                        }

                        activeWaves.Add(new TrigSpawnerWaiterSpecifics {
                            EventType = key,
                            Wave = wave
                        });
                    }
                }

                return activeWaves;
            }
        }

        /// <summary>
        /// This returns a list of all the waves. Used by bulk "modify all waves" things such as visualization code.
        /// </summary>
        public Dictionary<TriggeredSpawner.EventType, List<TriggeredWaveSpecifics>> AllWaves {
            get {
                _allWaves.Clear();

                if (_allWaves.Count != 0) {
                    return _allWaves;
                }

                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnEnabled, enableWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnDisabled, disableWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Visible, visibleWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Invisible, invisibleWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.MouseOver_Legacy, mouseOverWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.MouseClick_Legacy, mouseClickWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnCollision, collisionWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerEnter, triggerEnterWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerStay, triggerStayWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerExit, triggerExitWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnSpawned, spawnedWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnDespawned, despawnedWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.CodeTriggered1, codeTriggeredWaves1, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.CodeTriggered2, codeTriggeredWaves2, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnClick_NGUI, clickWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnCollision2D, collision2dWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerEnter2D, triggerEnter2dWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerStay2D, triggerStay2dWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.OnTriggerExit2D, triggerExit2dWaves, ref _allWaves);

                // ReSharper disable once ForCanBeConvertedToForeach
                if (userDefinedEventWaves.Count > 0) {
                    _allWaves.Add(TriggeredSpawner.EventType.CustomEvent, userDefinedEventWaves);
                }

                AddFirstWaveIfAny(TriggeredSpawner.EventType.SliderChanged_uGUI, unitySliderChangedWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.ButtonClicked_uGUI, unityButtonClickedWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.PointerDown_uGUI, unityPointerDownWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.PointerUp_uGUI, unityPointerUpWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.PointerEnter_uGUI, unityPointerEnterWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.PointerExit_uGUI, unityPointerExitWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Drag_uGUI, unityDragWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Drop_uGUI, unityDropWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Scroll_uGUI, unityScrollWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.UpdateSelected_uGUI, unityUpdateSelectedWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Select_uGUI, unitySelectWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Deselect_uGUI, unityDeselectWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Move_uGUI, unityMoveWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.InitializePotentialDrag_uGUI, unityInitializePotentialDragWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.BeginDrag_uGUI, unityBeginDragWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.EndDrag_uGUI, unityEndDragWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Submit_uGUI, unitySubmitWaves, ref _allWaves);
                AddFirstWaveIfAny(TriggeredSpawner.EventType.Cancel_uGUI, unityCancelWaves, ref _allWaves);

                return _allWaves;
            }
        }

        /// <summary>
        /// This property will return true if the Wave Pause Behavior setting makes this spawner paused.
        /// </summary>
        public bool SpawnerIsPaused {
            get { return LevelSettings.WavesArePaused && wavePauseBehavior == TriggeredSpawner.WavePauseBehavior.Disable; }
        }

        /// <summary>
        /// This property will return true if the Game Over Behavior setting makes this spawner disabled.
        /// </summary>
        public bool GameIsOverForSpawner {
            get { return LevelSettings.IsGameOver && gameOverBehavior == TriggeredSpawner.GameOverBehavior.Disable; }
        }

        /*! \cond PRIVATE */
        public bool IsVisible {
            get { return _isVisible; }
        }

        public Transform Trans {
            get {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_trans == null) {
                    _trans = GetComponent<Transform>();
                }

                return _trans;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private bool IsSetToUGUI {
            get { return unityUIMode != TriggeredSpawner.Unity_UIVersion.Legacy; }
        }

        private bool IsSetToLegacyUI {
            get { return unityUIMode == TriggeredSpawner.Unity_UIVersion.Legacy; }
        }

        private bool HasActiveSpawningWave {
            get {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _userDefinedEventWaveMeta.Count; i++) {
                    if (_userDefinedEventWaveMeta[i].waveSpec.customEventActive) {
                        return true;
                    }
                }

                return _enableWaveMeta != null
                       || _disableWaveMeta != null
                       || _visibleWaveMeta != null
                       || _invisibleWaveMeta != null
                       || _mouseOverWaveMeta != null
                       || _mouseClickWaveMeta != null
                       || _collisionWaveMeta != null
                       || _triggerEnterWaveMeta != null
                       || _triggerExitWaveMeta != null
                       || _spawnedWaveMeta != null
                       || _despawnedWaveMeta != null
                       || _codeTriggeredWave1Meta != null
                       || _codeTriggeredWave2Meta != null
                       || _clickWaveMeta != null
                       || _collision2DWaveMeta != null
                       || _triggerEnter2DWaveMeta != null
                       || _triggerExit2DWaveMeta != null
                       || _unitySliderChangedWaveMeta != null
                       || _unityButtonClickedWaveMeta != null
                       || _unityPointerDownWaveMeta != null
                       || _unityPointerUpWaveMeta != null
                       || _unityPointerEnterWaveMeta != null
                       || _unityPointerExitWaveMeta != null
                       || _unityDragWaveMeta != null
                       || _unityDropWaveMeta != null
                       || _unityScrollWaveMeta != null
                       || _unityUpdateSelectedWaveMeta != null
                       || _unitySelectWaveMeta != null
                       || _unityDeselectWaveMeta != null
                       || _unityMoveWaveMeta != null
                       || _unityInitializePotentialDragWaveMeta != null
                       || _unityBeginDragWaveMeta != null
                       || _unityEndDragWaveMeta != null
                       || _unitySubmitWaveMeta != null
                       || _unityCancelWaveMeta != null;
            }
        }
        /*! \endcond */

        /// <summary>
        /// Returns whether the spawner is active or not, based on its Active Mode controls. This can restrict its active state based on World Variable values.
        /// </summary>
        public bool SpawnerIsActive {
            get {
                switch (activeMode) {
                    case LevelSettings.ActiveItemMode.Always:
                        return true;
                    case LevelSettings.ActiveItemMode.Never:
                        return false;
                    case LevelSettings.ActiveItemMode.IfWorldVariableInRange:
                        if (activeItemCriteria.statMods.Count == 0) {
                            return false;
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                            var stat = activeItemCriteria.statMods[i];
                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                return false;
                            }
                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;

                            var min = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMin
                                : stat._modValueFloatMin;
                            var max = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMax
                                : stat._modValueFloatMax;

                            if (min > max) {
                                LevelSettings.LogIfNew(
                                    "The Min cannot be greater than the Max for Active Item Limit in Triggered Spawner '" +
                                    transform.name + "'.");
                                return false;
                            }

                            if (varVal < min || varVal > max) {
                                return false;
                            }
                        }

                        break;
                    case LevelSettings.ActiveItemMode.IfWorldVariableOutsideRange:
                        if (activeItemCriteria.statMods.Count == 0) {
                            return false;
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < activeItemCriteria.statMods.Count; i++) {
                            var stat = activeItemCriteria.statMods[i];
                            var variable = WorldVariableTracker.GetWorldVariable(stat._statName);
                            if (variable == null) {
                                return false;
                            }

                            var varVal = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? variable.CurrentIntValue
                                : variable.CurrentFloatValue;

                            var min = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMin
                                : stat._modValueFloatMin;
                            var max = stat._varTypeToUse == WorldVariableTracker.VariableType._integer
                                ? stat._modValueIntMax
                                : stat._modValueFloatMax;

                            if (min > max) {
                                LevelSettings.LogIfNew(
                                    "The Min cannot be greater than the Max for Active Item Limit in Triggered Spawner '" +
                                    transform.name + "'.");
                                return false;
                            }

                            if (varVal >= min && varVal <= max) {
                                return false;
                            }
                        }

                        break;
                }

                return true;
            }
        }

        #endregion
    }
}