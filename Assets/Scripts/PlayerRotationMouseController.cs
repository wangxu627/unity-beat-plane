using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotationMouseController : MonoBehaviour
{
    private Plane plane;
    // Start is called before the first frame update
    void Start()
    {
        this.plane = new Plane(Vector3.back, 0);
    }
    // Update is called once per frame
    void Update()
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            this.transform.LookAt(worldPosition, Vector3.back);
        }
 
    }
}
