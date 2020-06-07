using UnityEngine;
using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using System.Linq;

namespace RelationsInspector.Backend.CoreGameKit
{
	class PoolNode
	{
		public WavePrefabPool pool;
	}

	class GameObjectNode
	{
		public GameObject gameObject;
	}

	public class SyncroSpawnerBackend : MinimalBackend<object,string>
	{
		public static int levelFilter;
		public static int waveFilter;
		static Color inactiveColor = new Color( 1, 0.3f, 0.3f, 1 );

		public override IEnumerable<object> Init( object target )
		{
			var asSpawner = target as WaveSyncroPrefabSpawner;
			if ( asSpawner != null )
				yield return asSpawner;
		}

		public override IEnumerable<Relation<object, string>> GetRelations( object entity )
		{
			var asSpawner = entity as WaveSyncroPrefabSpawner;
			if ( asSpawner != null )
			{
				if ( asSpawner.activeMode == LevelSettings.ActiveItemMode.Never )
					yield break;

				foreach ( var spec in asSpawner.waveSpecs )
				{
					if ( !DoIncludeWaveSpec( spec, levelFilter, waveFilter ) )
						continue;

					yield return new Relation<object, string>(entity, spec, string.Empty);
				}
				yield break;
			}

			var asWaveSpec = entity as WaveSpecifics;
			if ( asWaveSpec != null )
			{
				if ( !asWaveSpec.enableWave )
					yield break;

				if ( asWaveSpec.spawnSource == WaveSpecifics.SpawnOrigin.Specific )
				{
					var prefabNode = new GameObjectNode() { gameObject = asWaveSpec.prefabToSpawn.gameObject };
					yield return new Relation<object, string>( entity, prefabNode, "Spawns prefab" );
				}
				else if ( asWaveSpec.spawnSource == WaveSpecifics.SpawnOrigin.PrefabPool )
				{
					var pool = LevelSettings.GetFirstMatchingPrefabPool( asWaveSpec.prefabPoolName );
					if ( pool != null )
					{
						var poolNode = new PoolNode() { pool = pool };
						yield return new Relation<object, string>( entity, poolNode, "Spawns prefab of pool" );
					}
				}
				yield break;
			}

			var asPool = entity as PoolNode;
			if ( asPool != null )
			{
				// connect to members
				foreach ( var item in asPool.pool.poolItems )
					yield return new Relation<object, string>( entity, item.prefabToSpawn.gameObject, "Contains prefab" );
				yield break;
			}

			// GameObject: do nothing. prefabs are leaf nodes

			yield break;
		}

		public override Rect OnGUI()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);

			if ( GUILayout.Button( "Show active scene", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
				SetSceneTargets();

			DrawFilterGUI();

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			return base.OnGUI();
		}

		void DrawFilterGUI()
		{
            if (LevelSettings.Instance == null) {
                return;
            }
            
            GUILayout.Space(20);
			{
				GUILayout.Label( "Show" );

				int numLevels = LevelSettings.Instance.LevelTimes.Count;
				var displayOptions = new[] { "All Levels" }
					.Concat( Enumerable.Range( 0, numLevels ).Select( i => "Level " + (i + 1)) )
					.ToArray();

				EditorGUI.BeginChangeCheck();
				levelFilter = EditorGUILayout.Popup( levelFilter, displayOptions, GUILayout.Width(100) );
				if ( EditorGUI.EndChangeCheck() )
				{
					waveFilter = 0;
					SetSceneTargets();
				}

				if ( levelFilter != 0 )
					DrawWaveFilterGUI();
			}
		}

		void DrawWaveFilterGUI()
		{
			GUILayout.Space( 20 );

			int numWaves = LevelSettings.Instance.LevelTimes[ levelFilter-1 ].WaveSettings.Count;
			var displayOptions = new[] { "All Waves" }
				.Concat( Enumerable.Range( 0, numWaves ).Select( i => "Wave " + (i + 1) ) )
				.ToArray();

			EditorGUI.BeginChangeCheck();
			waveFilter = EditorGUILayout.Popup( waveFilter, displayOptions, GUILayout.Width( 100 ) );
			if ( EditorGUI.EndChangeCheck() )
				SetSceneTargets();
		}

		void SetSceneTargets()
		{
			api.ResetTargets( GetSceneTargets() );
		}

		public static WaveSyncroPrefabSpawner[] GetSceneTargets()
		{
			if ( LevelSettings.Instance == null )
				return new WaveSyncroPrefabSpawner[0];

			return LevelSettings.GetAllSpawners
				.Select( transform => transform.gameObject.GetComponent<WaveSyncroPrefabSpawner>() )
				.Where( spawner => spawner != null && DoIncludeSpawner( spawner, levelFilter, waveFilter ) )
				.ToArray();
		}

		static bool DoIncludeSpawner( WaveSyncroPrefabSpawner spawner, int includeLevel, int includeWave )
		{
			// if we filter, then also exclude inactive spawners
			if(includeLevel != 0 && spawner.activeMode == LevelSettings.ActiveItemMode.Never )
				return false;

			return spawner.waveSpecs.Any( spec => DoIncludeWaveSpec( spec, includeLevel, includeWave ) );
		}

		static bool DoIncludeWaveSpec( WaveSpecifics waveSpec, int includeLevel, int includeWave )
		{
			// no level filter: everything passes
			if ( includeLevel == 0 )
				return true;

			// level filter set. only matching levels pass
			if ( waveSpec.SpawnLevelNumber != ( includeLevel - 1 ) )
				return false;

			if ( includeWave == 0 )
				return true;

			return waveSpec.SpawnWaveNumber == ( includeWave - 1 );
		}

		public override Rect DrawContent( object entity, EntityDrawContext drawContext )
		{
			var asWaveSpec = entity as WaveSpecifics;
			var asSpawner = entity as WaveSyncroPrefabSpawner;
			bool disabledNode =
				( asWaveSpec != null && asWaveSpec.enableWave == false ) ||
				( asSpawner != null && asSpawner.activeMode == LevelSettings.ActiveItemMode.Never );

			if ( disabledNode )
			{
				var backupBgColor = drawContext.style.backgroundColor;
				var backendTargetBgColor = drawContext.style.targetBackgroundColor;
				drawContext.style.backgroundColor = drawContext.style.targetBackgroundColor = inactiveColor;
				var rect = base.DrawContent( entity, drawContext );
				drawContext.style.backgroundColor = backupBgColor;
				drawContext.style.targetBackgroundColor = backendTargetBgColor;
				return rect;
			}
			else
				return base.DrawContent( entity, drawContext );
		}

		public override GUIContent GetContent( object entity )
		{
			var asSpawner = entity as WaveSyncroPrefabSpawner;
			if ( asSpawner != null )
			{
				string enabled = ( asSpawner.activeMode == LevelSettings.ActiveItemMode.Never ) ? "(inactive) " : string.Empty;
				return new GUIContent( string.Format( "{0}{1}", enabled, asSpawner.name ) );
			}

			var asWaveSpec = entity as WaveSpecifics;
			if ( asWaveSpec != null )
			{
				string enabled = asWaveSpec.enableWave ? string.Empty : "(disabled) ";
				return new GUIContent( string.Format( "{0}Level {1}, Wave {2}",enabled , asWaveSpec.SpawnLevelNumber + 1, asWaveSpec.SpawnWaveNumber + 1 ) );
			}

			var asGameObjectNode = entity as GameObjectNode;
			if ( asGameObjectNode != null )
				return base.GetContent( asGameObjectNode.gameObject );

			var asPoolNode = entity as PoolNode;
			if ( asPoolNode != null )
				return new GUIContent( "Prefab pool: " + asPoolNode.pool.name );//  string.Format("Prefab pool") );

			return base.GetContent( entity );
		}

		static object GetUnityObject( object entity )
		{
			var asGameObjectNode = entity as GameObjectNode;
			if ( asGameObjectNode != null )
				return asGameObjectNode.gameObject;

			var asPoolNode = entity as PoolNode;
			if ( asPoolNode != null )
				return asPoolNode.pool;

			if ( entity is WaveSpecifics )
				return LevelSettings.Instance;

			return entity;
		}

		public override void OnEntitySelectionChange( object[] selection )
		{
			Selection.objects = selection
				.Select( x => GetUnityObject( x ) )
				.OfType<Object>()
				.ToArray();
		}
	}
}
