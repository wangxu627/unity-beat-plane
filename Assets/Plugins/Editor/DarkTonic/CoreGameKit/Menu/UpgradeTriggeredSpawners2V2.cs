#if UNITY_5_4_OR_NEWER
using UnityEditor;
using UnityEngine;
using DarkTonic.CoreGameKit;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

//Step 1----->Run UpgradeSpawners2V2 to add V2 spawners and copy data from v1.
//Step 2----->Run RemoveAllV1Spawners to remove all the V1 spawners

public class UpgradeTriggeredSpawners2V2 : EditorWindow {
    [MenuItem("Window/Core GameKit/Upgrade Triggered Spawners to V2")]
    public static void UpgradeTriggeredSpawners() {
        TriggeredSpawner[] _searchResults = GameObject.FindObjectsOfType<TriggeredSpawner>();
        //Select the parent gameobject having all triggered spawners
        for (int i = 0; i < _searchResults.Length; i++) {
            CopyWaveDataToV2(_searchResults[i]);
        }

        //Mark scene dirty 
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("Finished Adding V2 Triggered Spawners with Count: " + _searchResults.Length);
    }

    [MenuItem("Window/Core GameKit/Remove V1 Triggered Spawners")]
    public static void RemoveV1TriggeredSpawners() {
        int _removeCount = 0;
        TriggeredSpawner[] _searchResults = GameObject.FindObjectsOfType<TriggeredSpawner>();
        //Iterate on list and get V1 script to be removed
        for (int i = 0; i < _searchResults.Length; i++) {
            //We can not destroy the objects while iterating on them hence get V1 from added V2
            var _oldV1 = _searchResults[i].GetComponent<TriggeredSpawner>();

            if (_oldV1 != null) {
                _removeCount += 1;
                DestroyImmediate(_oldV1);
            }
        }
        //Mark scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log(string.Format("Found V2: {0}, Removed V1 Triggered Spawners Count: {1}", _searchResults.Length, _removeCount));
    }

    private static void CopyWaveDataToV2(TriggeredSpawner _V1_Version_TS) {
        var v2 = _V1_Version_TS.GetComponent<TriggeredSpawnerV2>();

        if (v2 == null) {
            _V1_Version_TS.gameObject.AddComponent<TriggeredSpawnerV2>();
            v2 = _V1_Version_TS.GetComponent<TriggeredSpawnerV2>();
        }

        v2.DeleteAllWaves();

        CopyWaveIfAny(_V1_Version_TS.enableWave, ref v2.enableWaves);
        CopyWaveIfAny(_V1_Version_TS.disableWave, ref v2.disableWaves);
        CopyWaveIfAny(_V1_Version_TS.visibleWave, ref v2.visibleWaves);
        CopyWaveIfAny(_V1_Version_TS.invisibleWave, ref v2.invisibleWaves);
        CopyWaveIfAny(_V1_Version_TS.mouseOverWave, ref v2.mouseOverWaves);
        CopyWaveIfAny(_V1_Version_TS.mouseClickWave, ref v2.mouseClickWaves);
        CopyWaveIfAny(_V1_Version_TS.collisionWave, ref v2.collisionWaves);
        CopyWaveIfAny(_V1_Version_TS.triggerEnterWave, ref v2.triggerEnterWaves);
        CopyWaveIfAny(_V1_Version_TS.triggerExitWave, ref v2.triggerExitWaves);
        CopyWaveIfAny(_V1_Version_TS.spawnedWave, ref v2.spawnedWaves);
        CopyWaveIfAny(_V1_Version_TS.despawnedWave, ref v2.despawnedWaves);
        CopyWaveIfAny(_V1_Version_TS.codeTriggeredWave1, ref v2.codeTriggeredWaves1);
        CopyWaveIfAny(_V1_Version_TS.codeTriggeredWave2, ref v2.codeTriggeredWaves2);
        CopyWaveIfAny(_V1_Version_TS.clickWave, ref v2.clickWaves);
        CopyWaveIfAny(_V1_Version_TS.collision2dWave, ref v2.collision2dWaves);
        CopyWaveIfAny(_V1_Version_TS.triggerEnter2dWave, ref v2.triggerEnter2dWaves);
        CopyWaveIfAny(_V1_Version_TS.triggerExit2dWave, ref v2.triggerExit2dWaves);
        v2.userDefinedEventWaves.AddRange(_V1_Version_TS.userDefinedEventWaves);
        CopyWaveIfAny(_V1_Version_TS.unitySliderChangedWave, ref v2.unitySliderChangedWaves);
        CopyWaveIfAny(_V1_Version_TS.unityButtonClickedWave, ref v2.unityButtonClickedWaves);
        CopyWaveIfAny(_V1_Version_TS.unityPointerDownWave, ref v2.unityPointerDownWaves);
        CopyWaveIfAny(_V1_Version_TS.unityPointerUpWave, ref v2.unityPointerUpWaves);
        CopyWaveIfAny(_V1_Version_TS.unityPointerEnterWave, ref v2.unityPointerEnterWaves);
        CopyWaveIfAny(_V1_Version_TS.unityPointerExitWave, ref v2.unityPointerExitWaves);
        CopyWaveIfAny(_V1_Version_TS.unityDragWave, ref v2.unityDragWaves);
        CopyWaveIfAny(_V1_Version_TS.unityDropWave, ref v2.unityDropWaves);
        CopyWaveIfAny(_V1_Version_TS.unityScrollWave, ref v2.unityScrollWaves);
        CopyWaveIfAny(_V1_Version_TS.unityUpdateSelectedWave, ref v2.unityUpdateSelectedWaves);
        CopyWaveIfAny(_V1_Version_TS.unitySelectWave, ref v2.unitySelectWaves);
        CopyWaveIfAny(_V1_Version_TS.unityDeselectWave, ref v2.unityDeselectWaves);
        CopyWaveIfAny(_V1_Version_TS.unityMoveWave, ref v2.unityMoveWaves);
        CopyWaveIfAny(_V1_Version_TS.unityInitializePotentialDragWave, ref v2.unityInitializePotentialDragWaves);
        CopyWaveIfAny(_V1_Version_TS.unityBeginDragWave, ref v2.unityBeginDragWaves);
        CopyWaveIfAny(_V1_Version_TS.unityEndDragWave, ref v2.unityEndDragWaves);
        CopyWaveIfAny(_V1_Version_TS.unitySubmitWave, ref v2.unitySubmitWaves);
        CopyWaveIfAny(_V1_Version_TS.unityCancelWave, ref v2.unityCancelWaves);

        // copy non-wave fields too
        v2.activeMode = _V1_Version_TS.activeMode;
        v2.activeItemCriteria = _V1_Version_TS.activeItemCriteria;
        v2.gameOverBehavior = _V1_Version_TS.gameOverBehavior;
        v2.wavePauseBehavior = _V1_Version_TS.wavePauseBehavior;
        v2.unityUIMode = _V1_Version_TS.unityUIMode;
        v2.eventSourceType = _V1_Version_TS.eventSourceType;
        v2.spawnOutsidePool = _V1_Version_TS.spawnOutsidePool;
        v2.logMissingEvents = _V1_Version_TS.logMissingEvents;
        v2.listener = _V1_Version_TS.listener;
        v2.spawnLayerMode = _V1_Version_TS.spawnLayerMode;
        v2.spawnCustomLayer = _V1_Version_TS.spawnCustomLayer;
        v2.applyLayerRecursively = _V1_Version_TS.applyLayerRecursively;
        v2.spawnTagMode = _V1_Version_TS.spawnTagMode;
        v2.spawnCustomTag = _V1_Version_TS.spawnCustomTag;

        EditorUtility.SetDirty(v2);
    }

    private static void CopyWaveIfAny(TriggeredWaveSpecifics firstSourceWave, ref List<TriggeredWaveSpecifics> targetWaveList) {
        if (firstSourceWave == null) {
            return;
        }
        
        if (!firstSourceWave.enableWave) { 
            return;
        }

        targetWaveList = new List<TriggeredWaveSpecifics>(1) {
            firstSourceWave
        };
    }

}
#endif