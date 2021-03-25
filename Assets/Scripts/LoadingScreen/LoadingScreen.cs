using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    
    private void Awake()
    {
        SceneFader.Instance.FadeNow(0.25f, true, ()=>StartCoroutine(AsyncLoad(GameSessionManager.Instance.NextScene)));
    }

    private IEnumerator AsyncLoad(int sceneIndex)
    {
        yield return null;

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOp.allowSceneActivation = false;

        bool fadeStarted = false;

        while(!asyncOp.isDone)
        {
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
            loadingBar.value = progress * 100f;

            if (asyncOp.progress >= 0.9f && !fadeStarted)
            {
                fadeStarted = true;
                SceneFader.Instance.FadeNow(0.25f, false, ()=>asyncOp.allowSceneActivation = true);
            }

            yield return null;
        }
    }
}