using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI text;
    private bool followTint = false;
    private ColorBlock colors;
    void Start()
    {
        Button btn = GetComponent<Button>();
        if(btn.transition == Selectable.Transition.ColorTint)
        {
            followTint = true;
            colors = btn.colors;
        }
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(followTint)
        {
            text.color = colors.pressedColor;
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if(followTint)
        {
            text.color = colors.normalColor;
        }
    }
}
