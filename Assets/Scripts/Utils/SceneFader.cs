using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneFader : Singleton<SceneFader>
{
    private FadingManager fadingManager;
    private Image cover;
    
    public override void Awake()
    {
        base.Awake();
        Canvas c = gameObject.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 10;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1024f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        //GraphicRaycaster gRaycaster = gameObject.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("FadingCover");
        panel.AddComponent<CanvasRenderer>();
        cover = panel.AddComponent<Image>();
        cover.rectTransform.anchorMin = new Vector2(0f, 0f);
        cover.rectTransform.anchorMax = new Vector2(1f, 1f);
        cover.color = Color.black;
        panel.transform.SetParent(transform, false);

        fadingManager = gameObject.AddComponent<FadingManager>();
    }

    public void FadeNow(float duration, bool fadeIn, Action callback)
    {
        fadingManager.BeginFade(cover, duration, fadeIn, callback);
    }
}