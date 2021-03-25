using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashPanel : MonoBehaviour
{
    private Animator anim;
    private static readonly int AnimShowGo = Animator.StringToHash("ShowGo");

    private int currentState = 0;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void ShowGo()
    {
        if(currentState > 0) return;
        
        anim.SetTrigger(AnimShowGo);
        currentState++;
    }
}
