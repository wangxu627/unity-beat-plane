using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IntegrateAnimator : MonoBehaviour
{
    [MenuItem("Tools/AddObjectToAsset")]
    static void AddObjectToAsset()
    {
        // Get controller
         Object anim = AssetDatabase.LoadAssetAtPath("Assets/Animations/SettingPanel.controller", (typeof(Object)));
         // Add an animation clip to it
        //  AnimationClip animationClip = new AnimationClip();
         Object animationClip = AssetDatabase.LoadAssetAtPath("Assets/Animations/Open.anim", (typeof(AnimationClip)));
         animationClip.name = "Open";

         AssetDatabase.AddObjectToAsset(animationClip, anim);
         // Reimport the asset after adding an object.
         // Otherwise the change only shows up when saving the project
        //  AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
    }

    static void O()
    {
        // Get controller
         Object anim = AssetDatabase.LoadAssetAtPath("Assets/Something/SomeAnimator.controller", (typeof(Object)));
         // Add an animation clip to it
         AnimationClip animationClip = new AnimationClip();
         animationClip.name = "SomeClip";
         AssetDatabase.AddObjectToAsset(animationClip, anim);
         // Reimport the asset after adding an object.
         // Otherwise the change only shows up when saving the project
         AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
    }
}
