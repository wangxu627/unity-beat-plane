using System;
using DarkTonic.CoreGameKit;
using UnityEditor;

[InitializeOnLoad]
// ReSharper disable once CheckNamespace
public class CoreScriptOrderManager {

    static CoreScriptOrderManager() {
        foreach (var monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
            if (monoScript.GetClass() == null) {
                continue;
            }

            foreach (var a in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(CoreScriptOrder))) {
                var currentOrder = MonoImporter.GetExecutionOrder(monoScript);
                var newOrder = ((CoreScriptOrder)a).Order;
                if (currentOrder != newOrder) {
                    MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
    }
}
