using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class CoreGameKitInspectorResources {
	public const string CoreGameKitFolderPath = "Core GameKit";

	public static Texture LogoTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/inspector_header_killer_waves.png", CoreGameKitFolderPath)) as Texture;
    public static Texture SettingsTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/gearIcon.png", CoreGameKitFolderPath)) as Texture;
	public static Texture LeftArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_left.png", CoreGameKitFolderPath)) as Texture;
	public static Texture RightArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_right.png", CoreGameKitFolderPath)) as Texture;
	public static Texture UpArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_up.png", CoreGameKitFolderPath)) as Texture;
	public static Texture DownArrowTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/arrow_down.png", CoreGameKitFolderPath)) as Texture;
    public static Texture CopyTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/copyIcon.png", CoreGameKitFolderPath)) as Texture;
    public static Texture SaveTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/save.png", CoreGameKitFolderPath)) as Texture;
    public static Texture CancelTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/cancel.png", CoreGameKitFolderPath)) as Texture;
	public static Texture ShowRelationsTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/showRelations.png", CoreGameKitFolderPath)) as Texture;
	public static Texture KillTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/kill.png", CoreGameKitFolderPath)) as Texture;
	public static Texture DespawnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/despawn.png", CoreGameKitFolderPath)) as Texture;
	public static Texture DamageTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/damage.png", CoreGameKitFolderPath)) as Texture;

	private static readonly string SkinColor = DTInspectorUtility.IsDarkSkin ? "Dark" : "Light";
	public static Texture PrefabTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/prefabIcon{1}.png", CoreGameKitFolderPath, SkinColor)) as Texture;
}
