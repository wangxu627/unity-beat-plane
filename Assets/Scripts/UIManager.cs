using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SunnyCat.Tools;
using DG.Tweening;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Group")]
    public GameObject menuGroup;
    public GameObject gameGroup;
    [Header("Panel & Node")]
    public GameObject commonPanel;
    public GameObject settingPanel;
    public GameObject pauseNode;
    public GameObject overNode;
    [Header("Button")]
    public GameObject pauseButton;
    public TextMeshProUGUI commonText;
    
    public TextMeshProUGUI counterdownText;
    public TextMeshProUGUI messageText;

    // private CountdownController countdownController;
    private bool isPauseShow;
    private bool isSettingPanelOpen = false;

    // protected override void Awake()
    // {
    //     base.Awake();
    //     countdownController = GetComponent<CountdownController>();
    // }
    void Start()
    {
        messageText.gameObject.SetActive(false);
        messageText.alpha = 0;
    }
    public void SwitchSettingPanel()
    {
        isSettingPanelOpen = !isSettingPanelOpen;
        if(isSettingPanelOpen)
        {
            OpenSettingPanel();
        }
        else
        {
            CloseSettingPanel();
        }
    }
    public void OpenSettingPanel()
    {
        // settingPanel.GetComponent<Animator>()?.SetBool("Open", true);
        settingPanel.GetComponent<Animator>()?.Play("Open");
    }

    public void CloseSettingPanel()
    {
        // settingPanel.GetComponent<Animator>()?.SetBool("Open", false);
        settingPanel.GetComponent<Animator>()?.Play("Close");
    }

    public void OpenMenuGroup(bool animate = false)
    {
        if(!animate)
        {
            menuGroup.gameObject.SetActive(true);
            CanvasGroup canvasGroup = menuGroup.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
        }
        else
        {
            menuGroup.gameObject.SetActive(true);
            CanvasGroup canvasGroup = menuGroup.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            var s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(1.0f, 1.0f));
        }
        isPauseShow = false;
    }

    public void CloseMenuGroup(bool animate = false)
    {
        if(!animate)
        {
            menuGroup.gameObject.SetActive(false);
            CanvasGroup canvasGroup = menuGroup.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }
        else
        {
            CanvasGroup canvasGroup = menuGroup.GetComponent<CanvasGroup>();
            var s = DOTween.Sequence();
            s.Append(canvasGroup.DOFade(0.0f, 1.0f).SetDelay(0.5f));
            s.AppendCallback(() => {
                canvasGroup.gameObject.SetActive(false);
            });
        }
    }

    public void OnPlayButtonClicked()
    {
        CloseMenuGroup(true);
        StartCoroutine(Utils.DelayCall(()=>{
            GameManager.Instance.ChangeGameState(GameManager.GameState.Intro);
        }, 1.0f));
    }

    public void OnPauseButtonClicked()
    {
        isPauseShow = !isPauseShow;
        if(isPauseShow)
        {
            OpenPausedPanel();
            GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
        }
        else
        {
            ClosePausedPanel();
        }
    }
    public void OnQuitButtonClicked()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.Menu);
    }

    public void OnContinueButtonClicked()
    {
        isPauseShow = false;
        ClosePausedPanel();
        GameManager.Instance.ChangeGameState(GameManager.GameState.Playing);
    }

    public void OnReviveButtonClicked()
    {
        ClosePausedPanel();
        GameManager.Instance.Revive();
    }

    public void OpenGameGroup()
    {
        gameGroup.SetActive(true);
        pauseButton.SetActive(true);
        counterdownText.gameObject.SetActive(true);
    }

    public void OpenGameGroupOnlyMessage()
    {
        OpenGameGroup();
        pauseButton.SetActive(false);
        counterdownText.gameObject.SetActive(false);
    }

    public void CloseGameGroup()
    {
        gameGroup.SetActive(false);
        pauseButton.SetActive(false);
        commonPanel.SetActive(false);
        counterdownText.gameObject.SetActive(false);
    }
    public void OpenFailedPanel()
    {
        HideAlllNodes();
        overNode.SetActive(true);
        commonText.text = "YOU DIED!";
        CanvasGroup canvasGroup = commonPanel.GetComponent<CanvasGroup>();
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0.0f;
        var s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(1.0f, 1.0f));
        s.AppendCallback(() => {});
    }

    public void CloseFailedPanel()
    {
        overNode.SetActive(false);
        CanvasGroup canvasGroup = commonPanel.GetComponent<CanvasGroup>();
        canvasGroup.gameObject.SetActive(false);
        canvasGroup.alpha = 0.0f;
    }
    public void OpenPausedPanel()
    {
        HideAlllNodes();
        pauseNode.SetActive(true);
        commonText.text = "PAUSED";
        CanvasGroup canvasGroup = commonPanel.GetComponent<CanvasGroup>();
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 0.0f;
        var s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(1.0f, 1.0f));
        s.AppendCallback(() => {});
       
    }

    public void ClosePausedPanel()
    {   
        pauseNode.SetActive(false);
        CanvasGroup canvasGroup = commonPanel.GetComponent<CanvasGroup>();
        var s = DOTween.Sequence();
        s.Append(canvasGroup.DOFade(0.0f, 1.0f));
        s.AppendCallback(() => {
            canvasGroup.gameObject.SetActive(false);
        });
    }
    private void HideAlllNodes()
    {
        pauseNode.SetActive(false);
        overNode.SetActive(false);
    }
    public void ShowMessage(string message)
    {
        messageText.gameObject.SetActive(true);
        messageText.alpha = 0;
        messageText.text = message;
        messageText.DOFade(1.0f, 1.0f);
    }
    public void HideMessage()
    {
        var s = DOTween.Sequence();
        s.Append(messageText.DOFade(0.0f, 1.0f));
        s.AppendCallback(() => {
            messageText.gameObject.SetActive(false);
        });
    }
}
