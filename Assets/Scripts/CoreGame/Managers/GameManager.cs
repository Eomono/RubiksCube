using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CubeUtils;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private enum GameState { Scrambling, GameOn, GamePaused, PlayerWon, Lingering }

    [SerializeField] private PlayerInputManager playerInputManager;
    
    public bool PlayerCanMoveCamera => (currentState == GameState.GameOn || currentState == GameState.Lingering);
    public bool PlayerCanRotateCube => (currentState == GameState.GameOn);
    public bool GameCompleted => (currentState == GameState.PlayerWon || currentState == GameState.Lingering);

    private CameraMovementManager cameraMovementManager;
    private CubeManager cubeManager;
    private RotationsHistoryManager historyManager;
    private HUDManager hudManager;

    private GameState currentState = GameState.Scrambling;

    private float timer;
    
    private void Awake()
    {
        int size = GameSessionManager.Instance.CubeSize;
        SaveManager.Instance.PreLoadSavedGame();
        
        historyManager = GetComponent<RotationsHistoryManager>();

        bool loaded = false;

        if (!GameSessionManager.Instance.NewGame && SaveManager.Instance.SavedGameExists)
        {
            SaveManager.Instance.LoadCurrentSavedGame(out size, out List<List<List<int>>> stickers, out List<RotationAction> history, out timer);
            cubeManager = GetComponent<CubeBuilder>().LoadCube(size, stickers);
            historyManager.LoadRotationHistory(history);
            loaded = true;
        }
        else
            cubeManager = GetComponent<CubeBuilder>().CreateCube(size);

        playerInputManager.SetCubeManager(cubeManager);
        cameraMovementManager = playerInputManager.gameObject.GetComponent<CameraMovementManager>();
        cameraMovementManager.SetUpInitialTargets(size);
        playerInputManager.gameObject.GetComponent<LevelBGManager>().SetLvlBG(size);
        
        hudManager = GameObject.FindWithTag("HUD").GetComponent<HUDManager>();
        
        hudManager.ShowGameHUDAtStart();
        
        SceneFader.Instance.FadeNow(0.25f, true,
            loaded
                ? (Action)(()=>StartCoroutine(FromLoadedGame()))
                : (Action)(()=>StartCoroutine(ScramblingCube())));
    }

    private void Update()
    {
        if(currentState != GameState.GameOn) return;
        
        CheckTimer();
    }

    private IEnumerator ScramblingCube()
    {
        yield return new WaitForSeconds(0.3f);
        int rotations = Mathf.FloorToInt(Random.Range(ScrambleRotationsMin, ScrambleRotationsMax) * (cubeManager.CubeSize * 0.4f));
        while (rotations > 0)
        {
            cameraMovementManager.RotateAroundCenter(new Vector2(-0.3f, Mathf.Sin(Time.time * 3f) * 0.03f));
            if (cubeManager.RandomRotation())
                rotations--;
            
            if(rotations < 2) hudManager.ShowSplashGo();
            
            yield return null;
        }
        
        cameraMovementManager.OverrideToPosition(cubeManager.CubeSize * 1.1f, -20f, 20f);
        yield return new WaitForSeconds(0.1f);
        
        BeginGame();
    }

    private IEnumerator FromLoadedGame()
    {
        yield return null;
        hudManager.RefreshTimer(timer);
        
        yield return new WaitForSeconds(0.1f);
        float duration = 3f;
        while (duration > 0f)
        {
            cameraMovementManager.RotateAroundCenter(new Vector2(-0.1f, Mathf.Sin(Time.time * 1.5f) * 0.01f));
            duration -= Time.deltaTime;
            
            if(duration < 0.1f) hudManager.ShowSplashGo();
            
            yield return null;
        }
        
        cameraMovementManager.OverrideToPosition(cubeManager.CubeSize * 1.1f, -20f, 20f);
        yield return new WaitForSeconds(0.2f);
        
        BeginGame();
    }

    public void PauseGame(bool value)
    {
        if(currentState == GameState.Lingering) return;
        
        if (value)
            currentState = GameState.GamePaused;
        else
            currentState = GameState.GameOn;
    }

    public void PlayerWon()
    {
        currentState = GameState.PlayerWon;
        historyManager.CleanHistory();
        StartCoroutine(CompletionExhibit());
    }
    
    private IEnumerator CompletionExhibit()
    {
        cameraMovementManager.OverrideToPosition(cubeManager.CubeSize * 1.2f, 0f, 14f);
        yield return new WaitForSeconds(0.2f);
        
        float duration = 8f;
        while (duration > 0f)
        {
            float speed = duration > 3f ? (-0.04f * duration) : -0.09f;
            cameraMovementManager.RotateAroundCenter(new Vector2(speed, Mathf.Sin(Time.time * 1.5f) * -0.02f));
            duration -= Time.deltaTime;
            yield return null;
        }
        
        cameraMovementManager.OverrideToPosition(cubeManager.CubeSize * 1.4f, 10f, 20f);
        
        yield return new WaitForSeconds(0.1f);
        
        GameSessionManager.Instance.HandleNewCompletedTime(timer, cubeManager.CubeSize);
        
        currentState = GameState.Lingering;
        hudManager.ShowWinDialog(timer, GameSessionManager.Instance.GetBestTime(cubeManager.CubeSize));
    }

    public void AskUndoRotation()
    {
        cubeManager.UndoRotation();
    }

    public void ConfirmRestartGame()
    {
        SaveManager.Instance.DeleteSavedGame();
        GameSessionManager.Instance.NewGame = true;
        GameSessionManager.Instance.CubeSize = cubeManager.CubeSize;
        GameSessionManager.Instance.NextScene = 2;
        SceneFader.Instance.FadeNow(0.25f, false, ()=>SceneManager.LoadScene(0));
    }

    public void ConfirmReturnToTitle()
    {
        if(!GameCompleted)
            SaveData();
        else
            SaveManager.Instance.DeleteSavedGame();
        
        GameSessionManager.Instance.NextScene = 1;
        SceneFader.Instance.FadeNow(0.25f, false, ()=>SceneManager.LoadScene(0));
    }

    private void BeginGame()
    {
        currentState = GameState.GameOn;
        hudManager.EnableMenuButton();
    }

    private void CheckTimer()
    {
        timer += Time.deltaTime;
        hudManager.RefreshTimer(timer);
    }

    private void SaveData()
    {
        List<RotationAction> history = cubeManager.PackDataForSaving(out int cSize, out List<List<List<int>>> stickers);
        SaveManager.Instance.SaveGame(true, cSize, stickers, history, timer);
    }
    
    private void OnApplicationQuit()
    {
        //This function is not called in some platforms
        SaveData();
    }
}