using System.Collections.Generic;
using DarkTonic.CoreGameKit;
using UnityEditor;
using UnityEngine;
using RelationsInspector.Backend.CoreGameKit;

// ReSharper disable once CheckNamespace
public static class DTInspectorUtility {
    private const string AlertTitle = "Core GameKit Alert";
    private const string AlertOkText = "Ok";
    private const string FoldOutTooltip = "Click to expand or collapse";

    // ReSharper disable InconsistentNaming
    // COLORS FOR DARK SCHEME
    private static readonly Color DarkSkin_OuterGroupBoxColor = new Color(.7f, 1f, 1f);
    private static readonly Color DarkSkin_SecondaryHeaderColor = new Color(.8f, .8f, .8f);
    private static readonly Color DarkSkin_GroupBoxColor = new Color(.6f, .6f, .6f);
    private static readonly Color DarkSkin_SecondaryGroupBoxColor = new Color(.5f, .8f, 1f);
    private static readonly Color DarkSkin_BrightButtonColor = Color.cyan;
    private static readonly Color DarkSkin_BrightTextColor = Color.yellow;
    private static readonly Color DarkSkin_DragAreaColor = Color.yellow;
    private static readonly Color DarkSkin_InactiveHeaderColor = new Color(.6f, .6f, .6f);
    private static readonly Color DarkSkin_ActiveHeaderColor = new Color(.3f, .8f, 1f);
    private static readonly Color DarkSkin_DeleteButtonColor = new Color(1f, .2f, .2f);
    private static readonly Color DarkSkin_AddButtonColor = Color.green;
    private static readonly Color DarkSkin_CloneButtonColor = new Color(1f, 1f, 1f);
	private static readonly Color DarkSkin_HelpIconColor = new Color(.2f, 1f, .2f);

    // COLORS FOR LIGHT SCHEME
    private static readonly Color LightSkin_OuterGroupBoxColor = Color.white;
    private static readonly Color LightSkin_SecondaryHeaderColor = Color.white;
    private static readonly Color LightSkin_GroupBoxColor = new Color(.7f, .7f, .8f);
    private static readonly Color LightSkin_SecondaryGroupBoxColor = new Color(.6f, 1f, 1f);
    private static readonly Color LightSkin_BrightButtonColor = new Color(0f, 1f, 1f);
    private static readonly Color LightSkin_BrightTextColor = Color.yellow;
    private static readonly Color LightSkin_DragAreaColor = new Color(1f, 1f, .3f);
    private static readonly Color LightSkin_InactiveHeaderColor = new Color(.6f, .6f, .6f);
    private static readonly Color LightSkin_ActiveHeaderColor = new Color(.3f, .8f, 1f);
    private static readonly Color LightSkin_DeleteButtonColor = new Color(1f, .2f, .2f);
    private static readonly Color LightSkin_AddButtonColor = Color.green;
    private static readonly Color LightSkin_CloneButtonColor = new Color(.3f, .6f, 1f);
	private static readonly Color LightSkin_HelpIconColor = Color.green;
	// ReSharper restore InconsistentNaming


    public enum FunctionButtons { None, Add, Remove, ShiftUp, ShiftDown, Edit, DespawnAll, Rename, Copy, Save, Cancel, Visualize, Hide, ShowRelations, Fire }

    public static void FocusInProjectViewButton(string itemName, GameObject obj) {
        var settingsIcon = new GUIContent(CoreGameKitInspectorResources.PrefabTexture, "Click to select " + itemName + " in Project View");

        if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24))) {
            EditorGUIUtility.PingObject(obj);
        }
    }

    public static FunctionButtons AddControlButtons(string itemName) {
        GUIContent settingsIcon;
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (CoreGameKitInspectorResources.SettingsTexture != null) {
            settingsIcon = new GUIContent(CoreGameKitInspectorResources.SettingsTexture, "Click to edit " + itemName);
        } else {
            settingsIcon = new GUIContent("Edit", "Click to edit " + itemName);
        }

        if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24))) {
            return FunctionButtons.Edit;
        }

        return FunctionButtons.None;
    }

    public static FunctionButtons AddCancelSaveButtons(string itemName) {
        var cancelIcon = new GUIContent(CoreGameKitInspectorResources.CancelTexture,
                "Click to cancel renaming " + itemName);

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button(cancelIcon, EditorStyles.toolbarButton, GUILayout.Width(24),
            GUILayout.Height(16))) {
            return FunctionButtons.Cancel;
        }

        var saveIcon = new GUIContent(CoreGameKitInspectorResources.SaveTexture,
                "Click to save " + itemName);

        GUI.backgroundColor = Color.white;
        if (GUILayout.Button(saveIcon, EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16))) {
            return FunctionButtons.Save;
        }

        return FunctionButtons.None;
    }

    public static FunctionButtons AddFoldOutListItemButtons(int position, int totalPositions, string itemName, bool showFindButton, string findText, bool showAddButton, bool showMoveButtons = false, bool showCopyButton = false) {
        if (Application.isPlaying) {
            return FunctionButtons.None;
        }

        var oldBg = GUI.backgroundColor;
        var oldContent = GUI.contentColor;

        var shiftUp = false;
        var shiftDown = false;

        if (showMoveButtons) {
            if (position > 0) {
                // the up arrow.
                var upArrow = CoreGameKitInspectorResources.UpArrowTexture;
                if (GUILayout.Button(new GUIContent(upArrow, "Click to shift " + itemName + " up"), EditorStyles.toolbarButton, GUILayout.Width(24))) {
                    shiftUp = true;
                }
            } else {
                GUILayout.Space(24);
            }

            if (position < totalPositions - 1) {
                // The down arrow will move things towards the end of the List
                var dnArrow = CoreGameKitInspectorResources.DownArrowTexture;
                if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift " + itemName + " down"), EditorStyles.toolbarButton, GUILayout.Width(24))) {
                    shiftDown = true;
                }
            } else {
                GUILayout.Space(24);
            }
        }

        var addPressed = false;

        if (showAddButton) {
            GUI.contentColor = AddButtonColor;

            addPressed = GUILayout.Button(new GUIContent("Add", "Click to insert " + itemName),
                EditorStyles.toolbarButton, GUILayout.Width(32));
        }
        GUI.contentColor = oldContent;

        var isCopy = false;

        if (showCopyButton) {
            isCopy = ShowCopyIcon() == FunctionButtons.Copy;
        }

        if (showFindButton) {
            if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.ShowRelationsTexture, findText), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(24))) {
                return FunctionButtons.ShowRelations;
            }
        }

        // Remove Button - Process presses later
        GUI.backgroundColor = DeleteButtonColor;
        if (GUILayout.Button(new GUIContent("Delete", "Click to remove " + itemName), EditorStyles.miniButton, GUILayout.Width(45))) {
            return FunctionButtons.Remove;
        }

        GUI.backgroundColor = oldBg;

        if (isCopy) {
            return FunctionButtons.Copy;
        }

        if (shiftUp) {
            return FunctionButtons.ShiftUp;
        }
        if (shiftDown) {
            return FunctionButtons.ShiftDown;
        }
        if (addPressed) {
            return FunctionButtons.Add;
        }

        return FunctionButtons.None;
    }

    public static bool ShowRelationsButton() {
        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.ShowRelationsTexture, "Show Relations"), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(24))) {
            return true;
        }

        return false;
    }

    public static void ShowLevelAndWaveSpawnedPrefabs(int? levelNum, int? waveNum) {
        SyncroSpawnerBackend.levelFilter = levelNum.HasValue ? levelNum.Value : 0;
        SyncroSpawnerBackend.waveFilter = waveNum.HasValue ? waveNum.Value : 0;
        var targets = SyncroSpawnerBackend.GetSceneTargets();
        EditorWindow.GetWindow<RelationsInspector.RelationsInspectorWindow>().GetAPI1.ResetTargets(targets, typeof(SyncroSpawnerBackend));
    }

    public static void ShowKillableRelations() {
        KillableEventBackend.includeEvents = (PrefabEventType)(PrefabEventType.DealtDamage | PrefabEventType.Death | PrefabEventType.InvincibleHit | PrefabEventType.ReceivedDamaged | PrefabEventType.Vanished);
        var targets = KillableEventBackend.GetSceneTargets();
        EditorWindow.GetWindow<RelationsInspector.RelationsInspectorWindow>().GetAPI1.ResetTargets(targets, typeof(KillableEventBackend));
    }

    public static FunctionButtons ShowCopyIcon() {
        var oldColor = GUI.contentColor;

        GUI.contentColor = CloneButtonColor;

        var button = FunctionButtons.None;

        if (GUILayout.Button(new GUIContent(CoreGameKitInspectorResources.CopyTexture, "Click to clone item"), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(32))) {
            button = FunctionButtons.Copy;
        }

        GUI.contentColor = oldColor;

        return button;
    }

    public static FunctionButtons AddCustomEventIcons(bool showRename, bool showVisualize, bool showEdit = true, bool showFire = false, string itemName = "Custom Event") {
        if (showRename) {
            var buttonPressed = AddCancelSaveButtons(itemName);

            switch (buttonPressed) {
                case FunctionButtons.Save:
                    return FunctionButtons.Rename;
                case FunctionButtons.Cancel:
                    return FunctionButtons.Cancel;
            }
        } else if (showEdit) {
            var settingsIcon = new GUIContent(CoreGameKitInspectorResources.SettingsTexture,
                                              "Click to edit " + itemName);
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24),
                                 GUILayout.Height(16))) {
                return FunctionButtons.Edit;
            }
            GUI.backgroundColor = oldColor;
        }

        if (showVisualize) {
            GUI.contentColor = BrightButtonColor;
            if (GUILayout.Button(new GUIContent("Viz.", "Click to visualize items that spawn for " + itemName + "."), EditorStyles.toolbarButton, GUILayout.Width(35))) {
                return FunctionButtons.Visualize;
            }
            if (GUILayout.Button(new GUIContent("Hide", "Click to hide visualization for items that spawn for " + itemName + "."), EditorStyles.toolbarButton, GUILayout.Width(35))) {
                return FunctionButtons.Hide;
            }
        }

        if (showFire && Application.isPlaying) {
            GUI.backgroundColor = Color.white;
            GUI.contentColor = DTInspectorUtility.BrightButtonColor;
            if (GUILayout.Button("Fire!", EditorStyles.toolbarButton, GUILayout.Width(38), GUILayout.Height(16))) {
                return FunctionButtons.Fire;
            }
        }

        GUI.backgroundColor = DeleteButtonColor;
        GUI.contentColor = Color.white;
        var shouldDelete = GUILayout.Button(new GUIContent("Delete", "Click to delete " + itemName), EditorStyles.miniButton, GUILayout.MaxWidth(45));
        GUI.backgroundColor = Color.white;

        if (shouldDelete) {
            return FunctionButtons.Remove;
        }

        return FunctionButtons.None;
    }

    public static void ResetColors() {
        GUI.color = Color.white;
        GUI.contentColor = Color.white;
        GUI.backgroundColor = Color.white;
    }

    public static void VerticalSpace(int pixels) {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(pixels);
        EditorGUILayout.EndVertical();
    }

    public static bool IsDarkSkin {
        get {
            return EditorPrefs.GetInt("UserSkin") == 1;
        }
    }

	private static Color HelpIconColor {
		get {
			return IsDarkSkin ? DarkSkin_HelpIconColor : LightSkin_HelpIconColor;
		}
	}

    public static Color AddButtonColor {
        get {
            return IsDarkSkin ? DarkSkin_AddButtonColor : LightSkin_AddButtonColor;
        }
    }

    public static Color DeleteButtonColor {
        get {
            return IsDarkSkin ? DarkSkin_DeleteButtonColor : LightSkin_DeleteButtonColor;
        }
    }

    public static Color InactiveHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_InactiveHeaderColor : LightSkin_InactiveHeaderColor;
        }
    }

    public static Color CloneButtonColor {
        get {
            return IsDarkSkin ? DarkSkin_CloneButtonColor : LightSkin_CloneButtonColor;
        }
    }

    public static Color ActiveHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_ActiveHeaderColor : LightSkin_ActiveHeaderColor;
        }
    }

    public static Color DragAreaColor {
        get {
            return IsDarkSkin ? DarkSkin_DragAreaColor : LightSkin_DragAreaColor;
        }
    }

    public static Color BrightButtonColor {
        get {
            return IsDarkSkin ? DarkSkin_BrightButtonColor : LightSkin_BrightButtonColor;
        }
    }

    public static Color BrightTextColor {
        get {
            return IsDarkSkin ? DarkSkin_BrightTextColor : LightSkin_BrightTextColor;
        }
    }

    private static Color GroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_GroupBoxColor : LightSkin_GroupBoxColor;
        }
    }

    private static Color SecondaryHeaderColor {
        get {
            return IsDarkSkin ? DarkSkin_SecondaryHeaderColor : LightSkin_SecondaryHeaderColor;
        }
    }

    private static Color OuterGroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_OuterGroupBoxColor : LightSkin_OuterGroupBoxColor;
        }
    }

    public static Color SecondaryGroupBoxColor {
        get {
            return IsDarkSkin ? DarkSkin_SecondaryGroupBoxColor : LightSkin_SecondaryGroupBoxColor;
        }
    }

    private static GUIStyle CornerGUIStyle {
        get {
#if UNITY_5 || UNITY_2017_1_OR_NEWER
            return EditorStyles.helpBox;
#else
            return EditorStyles.textArea;
#endif
        }

    }

    public static void AddSpaceForNonU5(int height = 2) {
#if UNITY_5 || UNITY_2017_1_OR_NEWER
        //
#else
        GUILayout.Space(height);
#endif
    }

    public static void StartGroupHeader(int level = 0, bool showBoth = true) {
        switch (level) {
            case 0:
                GUI.backgroundColor = GroupBoxColor;
                break;
            case 1:
                GUI.backgroundColor = SecondaryGroupBoxColor;
                break;
        }

        EditorGUILayout.BeginVertical(CornerGUIStyle);

        if (!showBoth) {
            return;
        }

        switch (level) {
            case 0:
                GUI.backgroundColor = SecondaryHeaderColor;
                break;
        }

        EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
    }

    public static void EndGroupHeader() {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }

    public static void BeginGroupedControls() {
        GUI.backgroundColor = OuterGroupBoxColor;
        GUILayout.BeginHorizontal();

#if UNITY_2017_1_OR_NEWER
        EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
#else
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
#endif

        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public static void EndGroupedControls() {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(3f);
        GUILayout.EndHorizontal();

        GUILayout.Space(3f);
    }

    public static bool Foldout(bool expanded, string label, EditorStyles style = null) {
        var content = new GUIContent(label, FoldOutTooltip);

        expanded = EditorGUILayout.Foldout(expanded, content);

        return expanded;
    }

    public static void DrawTexture(Texture tex) {
        if (tex == null) {
            Debug.Log("Logo texture missing");
            return;
        }

        var rect = GUILayoutUtility.GetRect(0f, 0f);
        rect.width = tex.width;
        rect.height = tex.height;

        GUILayout.Space(rect.height);
        GUI.DrawTexture(rect, tex);

        var e = Event.current;

        if (rect.Contains(e.mousePosition)) {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
        } else {
            return;
        }

        if (e.type != EventType.MouseUp) {
            return;
        }

        if (e.mousePosition.x >= 312 && e.mousePosition.x <= 350) {
            Selection.activeObject = LevelSettings.PoolBossPanel;
            return;
        }
        if (e.mousePosition.x >= 359 && e.mousePosition.x <= 408) {
            Selection.activeObject = LevelSettings.PrefabPoolsPanel;
            return;
        }
        if (e.mousePosition.x >= 416 && e.mousePosition.x <= 458) {
            Selection.activeObject = LevelSettings.WorldVariablePanel;
            return;
        }

        var ls = LevelSettings.Instance;
        if (ls != null) {
            Selection.activeObject = ls.gameObject;
        }
    }

    public static string SelectCustomEventForVariable(ref bool isDirty, string customEventName, Object gameObject, string label, ref FunctionButtons buttonClicked) {
        var eventNames = LevelSettings.Instance.CustomEventNames;

        StartGroupHeader(1, false);

        var existingIndex = eventNames.IndexOf(customEventName);

        int? eventIndex = null;

        var noMatch = false;

        var isItemSelected = existingIndex >= 1;

        EditorGUILayout.BeginHorizontal();
        var isDeleteClicked = false;

        if (isItemSelected) {
            eventIndex = EditorGUILayout.Popup(label, existingIndex, eventNames.ToArray());
            isDeleteClicked = ShowDeleteButton("Custom Event");
            EditorGUILayout.EndHorizontal();
        } else if (existingIndex == -1 && customEventName == LevelSettings.NoEventName) {
            eventIndex = EditorGUILayout.Popup(label, existingIndex, eventNames.ToArray());
            isDeleteClicked = ShowDeleteButton("Custom Event");
            EditorGUILayout.EndHorizontal();
        } else {
            // non-match
            noMatch = true;

            var newGroup = EditorGUILayout.TextField(label, customEventName);
            if (newGroup != customEventName) {
                UndoHelper.RecordObjectPropertyForUndo(ref isDirty, gameObject, "change " + label);
                customEventName = newGroup;
            }

            isDeleteClicked = ShowDeleteButton("Custom Event");
            EditorGUILayout.EndHorizontal();
            var newIndex = EditorGUILayout.Popup("All Custom Events", -1, eventNames.ToArray());
            if (newIndex >= 0) {
                eventIndex = newIndex;
            }
        }

        if (isDeleteClicked) {
            buttonClicked = FunctionButtons.Remove;
        }

        if (noMatch) {
            ShowRedErrorBox("Custom Event found no match. Choose one from 'All Custom Events'.");
        }

        if (eventIndex.HasValue) {
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (eventIndex.Value == -1) {
                customEventName = LevelSettings.NoEventName;
            } else {
                customEventName = eventNames[eventIndex.Value];
            }
        }

        EditorGUILayout.EndVertical();

        return customEventName;
    }

    private static bool ShowDeleteButton(string itemName) {
        var oldBg = GUI.backgroundColor;

        var isClicked = false;
        // Remove Button - Process presses later
        GUI.backgroundColor = DeleteButtonColor;
        if (GUILayout.Button(new GUIContent("Delete", "Click to remove " + itemName), EditorStyles.miniButton, GUILayout.Width(45))) {
            isClicked = true;
        }

        GUI.backgroundColor = oldBg;

        return isClicked;
    }

    public static void RedBoldMessage(string msg) {
        var oldBg = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        ShowRedErrorBox(msg);
        GUI.backgroundColor = oldBg;
    }

    public static void ShowColorWarningBox(string warningText) {
        EditorGUILayout.HelpBox(warningText, MessageType.Info);
    }

    public static void ShowRedErrorBox(string errorText) {
        EditorGUILayout.HelpBox(errorText, MessageType.Error);
    }

    public static void ShowLargeBarAlertBox(string errorText) {
        EditorGUILayout.HelpBox(errorText, MessageType.Warning);
    }

    public static bool ConfirmDialog(string text) {
        if (Application.isPlaying) {
            return true;
        }

        return EditorUtility.DisplayDialog(AlertTitle, text, AlertOkText);
    }

    public static void ShowAlert(string text) {
        if (Application.isPlaying) {
            Debug.LogWarning(text);
        } else {
            EditorUtility.DisplayDialog(AlertTitle, text, AlertOkText);
        }
    }

	public static void HelpHeader(string helpUrl, string apiUrl = "http://www.dtdevtools.com/API/coregamekit/annotated.html") {
		EditorGUILayout.BeginHorizontal(CornerGUIStyle);
		AddHelpIcon(helpUrl, true);
		GUILayout.Label("Click button for online help!");
		AddAPIIcon(apiUrl);
		EditorGUILayout.EndHorizontal();
	}
	
	public static void AddHelpIcon(string helpUrl, bool useMiddleButton = false) {
		var oldColor = GUI.color;
		var oldBG = GUI.backgroundColor;
		GUI.color = HelpIconColor;
		GUI.backgroundColor = Color.white;
		var buttonStyle = EditorStyles.miniButtonRight;
		if (useMiddleButton) {
			buttonStyle = EditorStyles.miniButton;
		}
		
		if (GUILayout.Button(new GUIContent("?", "Online Help"), buttonStyle, GUILayout.MaxWidth(16), GUILayout.Height(15))) {
			Application.OpenURL(helpUrl);
		}
		GUILayout.Space(3);
		GUI.color = oldColor;
		GUI.backgroundColor = oldBG;
	}
	
	private static void AddAPIIcon(string apiUrl) {
		var oldColor = GUI.color;
		var oldBG = GUI.backgroundColor;
		GUI.color = HelpIconColor;
		GUI.backgroundColor = Color.white;
		var buttonStyle = EditorStyles.miniButton;
		
		if (GUILayout.Button(new GUIContent("API", "Online Coding API Guide"), buttonStyle, GUILayout.MaxWidth(32), GUILayout.Height(15))) {
			Application.OpenURL(apiUrl);
		}
		GUILayout.Space(3);
		GUI.color = oldColor;
		GUI.backgroundColor = oldBG;
	}

    public static void ShowTopGameObjectNotPrefabMessage() {
        ShowLargeBarAlertBox("You can only work on Core GameKit prefabs that are in a Scene.");
        ShowRedErrorBox("Do not drag this prefab into the Scene. It will be linked to this prefab if you do. Click the button below to create a LevelWaveSettings prefab in the Scene.");
        ShowColorWarningBox("If you have already done this and this is a Scene object, make a prefab out of it to complete the process so it can be recognized as a prefab.");
    }

    public static void ShowSubGameObjectNotPrefabMessage() {
        ShowLargeBarAlertBox("You can only edit Core GameKit prefabs that are in a Scene.");
        ShowRedErrorBox("Do not drag this prefab into the Scene. It will be linked to this prefab if you do. Click the parent Game Object 'LeveWaveSettings' and follow instructions to create a LevelWaveSettings prefab in the Scene.");
        ShowColorWarningBox("If you have already done this and this is a Scene object, make a prefab out of it to complete the process so it can be recognized as a prefab.");
    }

#if UNITY_2018_3_OR_NEWER
    public static bool IsPrefabInProjectView(Object gObject) {
        return GetPrefabType(gObject) != PrefabAssetType.Regular;
    }
#else
    public static bool IsPrefabInProjectView(Object gObject) {
        return GetPrefabType(gObject) == PrefabType.Prefab;
    }
#endif

#if UNITY_2018_3_OR_NEWER
    private static PrefabAssetType GetPrefabType(Object gObject) {
        return PrefabUtility.GetPrefabAssetType(gObject);
    }
#else
    private static PrefabType GetPrefabType(Object gObject) {
        return PrefabUtility.GetPrefabType(gObject);
    }
#endif

    public static void DestroyChildrenImmediateWithMarker(this GameObject go) {
        var children = new List<GameObject>();

        foreach (Transform tran in go.transform) {
            if (tran.GetComponent<VisualizationMarker>() != null) {
                children.Add(tran.gameObject);
            }
        }

        // ReSharper disable once ConvertClosureToMethodGroup
        // ReSharper disable once AccessToStaticMemberViaDerivedType
        children.ForEach(child => GameObject.DestroyImmediate(child));
    }
}
