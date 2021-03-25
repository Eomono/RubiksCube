using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadingManager : MonoBehaviour
{
    public void BeginFade(Image cover, float duration, bool fadeIn, Action callback)
    {
        StartCoroutine(Fading(cover, duration, fadeIn, callback));
    }

    private IEnumerator Fading(Image cover, float duration, bool fadeIn, Action callback)
    {
        float totalFade = duration;

        if (fadeIn)
        {
            cover.color = Color.black;
            
            while (duration > 0f)
            {
                cover.color = new Color(0f, 0f, 0f, duration / totalFade);
                duration -= Time.deltaTime;
                yield return null;
            }
            
            cover.color = Color.clear;
        }
        else
        {
            cover.color = Color.clear;
            
            while (duration > 0f)
            {
                cover.color = new Color(0f, 0f, 0f, 1f - (duration / totalFade));
                duration -= Time.deltaTime;
                yield return null;
            }
            
            cover.color = Color.black;
        }

        if (callback != null) callback();
    }
}