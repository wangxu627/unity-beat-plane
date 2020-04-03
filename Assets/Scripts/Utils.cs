using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class Utils
{
    public static Transform FindChildByName(GameObject gameObject, string name)
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }
}
