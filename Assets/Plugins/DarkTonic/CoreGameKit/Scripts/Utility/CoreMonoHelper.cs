using UnityEngine;

/*! \cond PRIVATE */
namespace DarkTonic.CoreGameKit {
	public static class CoreMonoHelper {
		public static Transform GetChildTransform(this Transform transParent, string childName) {
			#if UNITY_5_6 || UNITY_2017_1_OR_NEWER
			return transParent.Find(childName);
			#else
			return transParent.FindChild(childName);
			#endif
		}

		public static void SetLayerOnAllChildren(this Transform trans, int layer) {
			var go = trans.gameObject;

			go.layer = layer;

			for (var i = 0; i < trans.childCount; i++) {
				trans.GetChild(i).SetLayerOnAllChildren(layer);
			}
		}
	}
}
/*! \endcond */