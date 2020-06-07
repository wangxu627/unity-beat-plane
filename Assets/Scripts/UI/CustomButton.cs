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
    private Button button;
    void Start()
    {
        button = GetComponent<Button>();
        if(button.transition == Selectable.Transition.ColorTint)
        {
            followTint = true;
            colors = button.colors;
        }
    }
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(!button.interactable)
        {
            return;
        }
        if(followTint)
        {
            text.color = colors.pressedColor;
        }
    }
    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if(!button.interactable)
        {
            return;
        }
        if(followTint)
        {
            text.color = colors.normalColor;
        }
    }

    public void UpdateState()
    {
        if(!button)
        {
            return;
        }
        if(button.interactable)
        {
            text.color = colors.normalColor;
        }
        else
        {
            text.color = colors.disabledColor;
        }
    }
}
