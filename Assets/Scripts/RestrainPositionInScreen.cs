using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrainPositionInScreen : MonoBehaviour
{
    public Camera cam;
    public float margin = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        this.cam = Camera.main;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    this.transform.position = this.GetRestrainedPosition(this.transform.position);
    //}

    public Vector3 GetRestrainedPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition = this.cam.WorldToScreenPoint(worldPosition);
        screenPosition.x = Mathf.Clamp(screenPosition.x, this.margin, Screen.width - this.margin);
        screenPosition.y = Mathf.Clamp(screenPosition.y, this.margin, Screen.height - this.margin);
        return this.cam.ScreenToWorldPoint(screenPosition);
    }
}
