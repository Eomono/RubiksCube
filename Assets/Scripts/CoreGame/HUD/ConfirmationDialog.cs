using System;
using TMPro;
using UnityEngine;

public class ConfirmationDialog : MonoBehaviour
{
    [SerializeField] private TMP_Text warningLabel;
    [SerializeField] private TMP_Text okButtonLabel;
    
    private Action confirmCallback;
    
    public void ShowRestartWarning(Action callback, bool stillPlaying)
    {
        confirmCallback = callback;
        warningLabel.text = "Are you sure you want to restart the game?" + (stillPlaying?" You will lose any progress you have made so far!":"");
        okButtonLabel.text = "Yes, restart";
        gameObject.SetActive(true);
    }

    public void ShowExitWarning(Action callback, bool stillPlaying)
    {
        confirmCallback = callback;
        warningLabel.text = "Are you sure you want to go back to the title screen?" + (stillPlaying?" Your current progress will be saved.":"");
        okButtonLabel.text = stillPlaying?"Yes, save":"Yes, go back";
        gameObject.SetActive(true);
    }

    public void ConfirmAction()
    {
        if (confirmCallback != null)
            confirmCallback();
    }
}