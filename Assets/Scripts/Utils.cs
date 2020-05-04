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

    public static Vector3 ScreenPosition2WorldPosition(Vector2 screenPosition)
    {
        Plane plane = new Plane(Vector3.back, 0);
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    public static Vector2 WorldPosition2ScreenPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        return new Vector2(screenPosition.x, screenPosition.y);
    }
}
