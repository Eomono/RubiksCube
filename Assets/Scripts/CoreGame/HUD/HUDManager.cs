using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private SplashPanel splashPanel;
    [SerializeField] private ConfirmationDialog confirmationDialog;
    [SerializeField] private WinDialog winDialog;
    
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private Button menuBttn;
    [SerializeField] private Button undoBttn;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public void ShowGameHUDAtStart()
    {
        EnableUndo(false);
        menuBttn.interactable = false;
        timerLabel.gameObject.SetActive(true);
        menuBttn.gameObject.SetActive(true);
        undoBttn.gameObject.SetActive(true);
    }

    public void ShowSplashGo()
    {
        splashPanel.ShowGo();
    }

    public void EnableMenuButton()
    {
        menuBttn.interactable = true;
    }

    public void RefreshTimer(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timerLabel.text = (timeSpan.Hours > 0)? $@"{timeSpan:h\:mm\:ss\.ff}":$@"{timeSpan:mm\:ss\.ff}";
    }

    public void AskPause(bool value)
    {
        gameManager.PauseGame(value);
        DisplayPauseMenu(value);
    }

    public void HideTimer(bool value)
    {
        timerLabel.gameObject.SetActive(value);
    }

    public void AskToUndoRotation()
    {
        gameManager.AskUndoRotation();
    }

    public void AskToRestart()
    {
        confirmationDialog.ShowRestartWarning(ConfirmRestart, !gameManager.GameCompleted);
    }

    public void ConfirmRestart()
    {
        gameManager.ConfirmRestartGame();
    }

    public void AskToReturnToTitle()
    {
        confirmationDialog.ShowExitWarning(ConfirmReturn, !gameManager.GameCompleted);
    }

    public void ConfirmReturn()
    {
        gameManager.ConfirmReturnToTitle();
    }

    public void EnableUndo(bool value)
    {
        undoBttn.interactable = value;
    }

    public void ShowWinDialog(float playerTime,float bestTime)
    {
        winDialog.ShowDialog(playerTime, bestTime);
    }

    private void DisplayPauseMenu(bool value)
    {
        pauseMenu.SetActive(value);
    }
}