/*! \cond PRIVATE */
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    // ReSharper disable once CheckNamespace
    public class WavePrefabPoolGroup : MonoBehaviour {
		public bool newPrefabPoolExpanded = true;
		public string newPrefabPoolName = "EnemiesPool";

		// ReSharper disable once UnusedMember.Local
        private void Awake() {
            useGUILayout = false;
        }

        // ReSharper disable once UnusedMember.Local
        private void Start() {
            var poolNames = new List<string>();

            // ReSharper disable once TooWideLocalVariableScope
            WavePrefabPool poolScript;

            for (var i = 0; i < transform.childCount; i++) {
                var pool = transform.GetChild(i);
                if (poolNames.Contains(pool.name)) {
                    LevelSettings.LogIfNew("You have more than one Prefab Pool with the name '" + pool.name +
                                           "'. Please fix this before continuing.");
                    LevelSettings.IsGameOver = true;
                    return;
                }

                poolScript = pool.GetComponent<WavePrefabPool>();
                if (poolScript == null) {
                    LevelSettings.LogIfNew("The Prefab Pool '" + pool.name +
                                           "' has no Prefab Pool script. Please delete it and fix this before continuing.");
                    LevelSettings.IsGameOver = true;
                    return;
                }

                poolNames.Add(pool.name);
            }
        }
    }
}
/*! \endcond */