using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class CustomToggle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private bool isOn;
    public Sprite onImage;
    public Sprite offImage;
    private Button button;

    public bool IsOn {
        get { return isOn; }
        set { isOn = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        UpdateUI();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        isOn = !isOn;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(isOn)
        {
            button.image.sprite = onImage;
        }
        else
        {
            button.image.sprite = offImage;
        }
    }
}
