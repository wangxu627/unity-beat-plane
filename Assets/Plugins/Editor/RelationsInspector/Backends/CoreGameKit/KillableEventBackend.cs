using UnityEngine;
using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using System.Linq;

namespace RelationsInspector.Backend.CoreGameKit
{
	public enum PrefabEventType
	{
		ReceivedDamaged = 1,
		DealtDamage = 2,
		InvincibleHit = 4,
		Vanished = 8,
		Death = 16
	};

	public class KillableEventBackend : MinimalBackend<Object, PrefabEventType>
	{
		struct PrefabEventData
		{
			public System.Func<Killable, bool> usePool;
			public System.Func<Killable, string> getPoolName;
			public System.Func<Killable, bool> useSpecific;
			public System.Func<Killable, Transform> getSpecificPrefab;
			public Color edgeMarkerColor;
		}

		static Dictionary<PrefabEventType, PrefabEventData> prefabEventData = new Dictionary<PrefabEventType, PrefabEventData>()
		{
			{
				PrefabEventType.ReceivedDamaged,
				new PrefabEventData()
				{
					usePool = k=> k.damagePrefabSource == Killable.SpawnSource.PrefabPool,
					getPoolName = k=>k.damagePrefabPoolName,
					useSpecific = k=>k.damagePrefabSource == Killable.SpawnSource.Specific,
					getSpecificPrefab = k=>k.damagePrefabSpecific,
					edgeMarkerColor = Color.yellow
				}
			},
			{
				PrefabEventType.DealtDamage,
				new PrefabEventData()
				{
					usePool = k=> k.dealDamagePrefabSource == Killable.SpawnSource.PrefabPool,
					getPoolName = k=>k.dealDamagePrefabPoolName,
					useSpecific = k=>k.dealDamagePrefabSource == Killable.SpawnSource.Specific,
					getSpecificPrefab = k=>k.dealDamagePrefabSpecific,
					edgeMarkerColor = Color.cyan
				}
			},
			{
				PrefabEventType.InvincibleHit,
				new PrefabEventData()
				{
					usePool = k=> k.invinceHitPrefabSource == Killable.SpawnSource.PrefabPool,
					getPoolName = k=>k.invinceHitPrefabPoolName,
					useSpecific = k=>k.invinceHitPrefabSource == Killable.SpawnSource.Specific,
					getSpecificPrefab = k=>k.invinceHitPrefabSpecific,
					edgeMarkerColor = Color.green
				}
			},
			{
				PrefabEventType.Vanished,
				new PrefabEventData()
				{
					usePool = k=> k.vanishPrefabSource == Killable.SpawnSource.PrefabPool,
					getPoolName = k=>k.vanishPrefabPoolName,
					useSpecific = k=>k.vanishPrefabSource == Killable.SpawnSource.Specific,
					getSpecificPrefab = k=>k.vanishPrefabSpecific,
					edgeMarkerColor = Color.grey
				}
			},
			{
				PrefabEventType.Death,
				new PrefabEventData()
				{
					usePool = k=> k.deathPrefabSource == WaveSpecifics.SpawnOrigin.PrefabPool,
					getPoolName = k=>k.deathPrefabPoolName,
					useSpecific = k=>k.deathPrefabSource == WaveSpecifics.SpawnOrigin.Specific,
					getSpecificPrefab = k=>k.deathPrefabSpecific,
					edgeMarkerColor = Color.red
				}
			}
		};

		static ColorLegendEntry[] colorLegend = prefabEventData
			.Select( pair => new ColorLegendEntry() { text = pair.Key.ToString(), color = pair.Value.edgeMarkerColor } )
			.ToArray();

		public static PrefabEventType includeEvents = (PrefabEventType) 31;

		static bool showLegend;

		public override IEnumerable<Object> Init( object target )
		{
			var asKillable = target as Killable;
			if ( asKillable == null )
				yield break;
			yield return asKillable;
		}

		public override IEnumerable<Relation<Object, PrefabEventType>> GetRelations( Object entity )
		{
			var asGameObject = entity as GameObject;

			Killable asKillable = ( asGameObject != null ) ? asGameObject.GetComponent<Killable>() : entity as Killable;
			if ( asKillable != null )
			{
				foreach ( PrefabEventType type in System.Enum.GetValues( typeof( PrefabEventType ) ) )
				{
					if ( ( includeEvents & type ) == 0 )
						continue;

					var handler = prefabEventData[ type ];
					if ( handler.useSpecific( asKillable ) )
					{
						var prefabTransform = handler.getSpecificPrefab( asKillable );
						if ( prefabTransform != null )
							yield return new Relation<Object, PrefabEventType>( entity, prefabTransform.gameObject, type );
					}
					else if ( handler.usePool( asKillable ) )
					{
						var pool = LevelSettings.GetFirstMatchingPrefabPool( handler.getPoolName( asKillable ) );
						if ( pool == null )
							continue;

						foreach ( var item in pool.poolItems )
							yield return new Relation<Object, PrefabEventType>( entity, item.prefabToSpawn.gameObject, type );
					}
				}
				yield break;
			}
		}

		public override Color GetRelationColor( PrefabEventType relationTagValue )
		{
			PrefabEventData eventTypeData;
			if ( prefabEventData.TryGetValue( relationTagValue, out eventTypeData ) )
				return eventTypeData.edgeMarkerColor;

			return Color.white;
		}

		public override Rect OnGUI()
		{
			GUILayout.BeginHorizontal( EditorStyles.toolbar );
			{
				if ( GUILayout.Button( "Show active scene", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) ) )
				{
					api.ResetTargets( GetSceneTargets() );
				}

				GUILayout.Space( 20 );
				EditorGUI.BeginChangeCheck();
				EditorGUIUtility.labelWidth = 40;
				EditorGUIUtility.fieldWidth = 120;


#if UNITY_2017_3_OR_NEWER
                includeEvents = (PrefabEventType)EditorGUILayout.EnumFlagsField(new GUIContent("Filter"), includeEvents);
#else
                includeEvents = (PrefabEventType)EditorGUILayout.EnumMaskField(new GUIContent("Filter"), includeEvents);
#endif

                if ( EditorGUI.EndChangeCheck() )
				{
					api.Rebuild();
				}

				GUILayout.FlexibleSpace();
				showLegend = GUILayout.Toggle(showLegend, "Legend", EditorStyles.toolbarButton, GUILayout.ExpandWidth( false ) );
				if ( showLegend )
				{
					string title = "Prefab events";
					var boxSize = ColorLegendBox.GetSize( title, colorLegend );
					float boxPosX = EditorGUIUtility.currentViewWidth - boxSize.x - 10;
					float boxPosY = 42;
					ColorLegendBox.Draw( new Rect( boxPosX, boxPosY, boxSize.x, boxSize.y ), title, colorLegend );
				}
			}
			GUILayout.EndHorizontal();

			return base.OnGUI();
		}

		public static Killable[] GetSceneTargets()
		{
			return Object.FindObjectsOfType<Killable>().ToArray();
		}
	}
}
