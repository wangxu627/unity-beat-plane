using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerLifeController lifeController;

    public void Hit()
    {
        Debug.Log("Player hit !!!!!!!!!!!!!!!");
        lifeController.DecreaseLife(1);
    }
}
