using System;
using TMPro;
using UnityEngine;

public class WinDialog : MonoBehaviour
{
    [SerializeField] private TMP_Text timeLabel;

    public void ShowDialog(float playerTime, float bestTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playerTime);
        TimeSpan bestSpan = TimeSpan.FromSeconds(bestTime);
        
        timeLabel.text = "Your time: " + 
                         ((timeSpan.Hours > 0)? $@"{timeSpan:h\:mm\:ss\.ff}":$@"{timeSpan:mm\:ss\.ff}") + 
                         "\nBest time: " + 
                         (((bestSpan.Hours > 0)? $@"{bestSpan:h\:mm\:ss\.ff}":$@"{bestSpan:mm\:ss\.ff}"));
        
        gameObject.SetActive(true);
    }
}