using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMoveController))]
public class PlayerRotationMouseController : MonoBehaviour
{
    private Plane plane;
    // Start is called before the first frame update
    void Start()
    {
        if(InputManager.Instance.controllerType != InputManager.ControllerType.Mouse)
        {
            Destroy(this);
        }
        this.plane = new Plane(Vector3.back, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentGameState == GameManager.GameState.Playing)
        {
            AlignToPoint2D(Input.mousePosition);
        }
    }

    private void AlignToPoint2D(Vector2 position)
    {
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(position);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            this.transform.LookAt(worldPosition, Vector3.back);
        }
    }

    public void ResetRotation()
    {
        Vector2 position = new Vector2(Screen.width / 2, Screen.height);
        AlignToPoint2D(position);
    }
}
