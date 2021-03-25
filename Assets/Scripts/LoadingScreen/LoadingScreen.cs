using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    
    private void Awake()
    {
        StartCoroutine(AsyncLoad(GameSessionManager.Instance.NextScene));
    }

    private IEnumerator AsyncLoad(int sceneIndex)
    {
        yield return null;

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOp.allowSceneActivation = false;

        while(!asyncOp.isDone)
        {
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);
            loadingBar.value = progress * 100f;
            
            if (asyncOp.progress >= 0.9f)
                asyncOp.allowSceneActivation = true;
            yield return null;
        }
    }
}