using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
// ReSharper disable once CheckNamespace
public class CoreGameKitHierIcon : MonoBehaviour {
    static readonly Texture2D PreviewItemIcon;
    static readonly Texture2D PreviewItemIconDisabled;
  
    static CoreGameKitHierIcon() { 
        PreviewItemIcon = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/CoreGameKit/PreviewIcon.png", typeof(Texture2D)) as Texture2D;
		PreviewItemIconDisabled = AssetDatabase.LoadAssetAtPath("Assets/Gizmos/CoreGameKit/PreviewIconOff.png", typeof(Texture2D)) as Texture2D;

        if (PreviewItemIcon == null) {
            return;
        }

        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        EditorApplication.RepaintHierarchyWindow();
    }

    // ReSharper disable once InconsistentNaming
    static void HierarchyItemCB(int instanceId, Rect selectionRect) {
        var visualizationGameObject = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

        if (visualizationGameObject == null) {
            return;
        }

        // ReSharper disable once InvertIf
        if (visualizationGameObject.GetComponent<VisualizationMarker>() != null) {
            Texture icon = null;
            if (PreviewItemIcon != null && visualizationGameObject.activeInHierarchy) {
                icon = PreviewItemIcon;
            } else if (PreviewItemIconDisabled != null) {
                icon = PreviewItemIconDisabled;
            } 

            if (icon == null) {
                return;
            }

            var iconRect = new Rect(selectionRect);
            // Always position the hierarchy icon on the right no matter how deep the GameObject is within the hierarchy
            iconRect.x = iconRect.width + (selectionRect.x - 16);
            iconRect.width = 16;
            iconRect.height = 16;

            GUI.DrawTexture(iconRect, icon);
        }
    }
}
