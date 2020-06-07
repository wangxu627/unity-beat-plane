// ReSharper disable once CheckNamespace
using DarkTonic.CoreGameKit;

// ReSharper disable once CheckNamespace
public class KillableSubclass : Killable {
    // ReSharper disable once OptionalParameterHierarchyMismatch
    public override void DestroyKillable(string scenarioName, bool skipDeathDelay = false) {
        // ReSharper disable once ConvertToConstant.Local
        var enemyReachedTower = true; // change this to whatever your logic is to determine true or false

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (enemyReachedTower) {
            scenarioName = "Reached Tower"; // change the scenario name to one of your others, instead of the default.
        }

        base.DestroyKillable(scenarioName, skipDeathDelay);
    }
}
