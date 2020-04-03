using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class UbhAutoScaleGUIText : MonoBehaviour
{
    Text _GuiText;
    float _OrgFontSize;

    void Awake ()
    {
        _GuiText = GetComponent<Text>();
        _OrgFontSize = _GuiText.fontSize;
    }

    void Update ()
    {
        float screenScaleX = (float) Screen.width / (float) UbhManager.BASE_SCREEN_WIDTH;
        float screenScaleY = (float) Screen.height / (float) UbhManager.BASE_SCREEN_HEIGHT;
        float screenScale = Screen.height < Screen.width ? screenScaleY : screenScaleX;

        _GuiText.fontSize = (int)(_OrgFontSize * screenScale);
    }
}
