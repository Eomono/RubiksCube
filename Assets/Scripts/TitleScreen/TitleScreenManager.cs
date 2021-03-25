using UnityEngine;
using UnityEngine.SceneManagement;
using static CubeUtils;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject LoadGameBttn;
    [SerializeField] private GameObject mainOptions;
    [SerializeField] private GameObject sizesOptions;
    [SerializeField] private GameObject confirmNewGameScreen;
    
    [SerializeField] private MiniCube miniCube;

    private int selectedSize = 3;
    
    private void Awake()
    {
        SaveManager.Instance.PreLoadSavedGame();
        
        LoadGameBttn.SetActive(SaveManager.Instance.SavedGameExists);
    }

    public void GoToSizeSelection()
    {
        miniCube.ChangeSize(selectedSize);
        miniCube.Show(true);
        mainOptions.SetActive(false);
        sizesOptions.SetActive(true);
    }

    public void ReturnToMainOptions()
    {
        miniCube.Show(false);
        sizesOptions.SetActive(false);
        mainOptions.SetActive(true);
    }

    public void PickNewCubeSize(bool next)
    {
        selectedSize += next ? 1 : -1;

        if (selectedSize > MaxCubeSize)
            selectedSize = MinCubeSize;
        else if (selectedSize < MinCubeSize)
            selectedSize = MaxCubeSize;
        
        miniCube.ChangeSize(selectedSize);
    }

    public void AskBeginNewGame()
    {
        if (SaveManager.Instance.SavedGameExists)
        {
            confirmNewGameScreen.SetActive(true);
            return;
        }
        
        BeginNewGame();
    }

    public void ConfirmNewGame()
    {
        BeginNewGame();
    }

    public void AskContinueGame()
    {
        GameSessionManager.Instance.NewGame = false;
        GoToCoreGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void BeginNewGame()
    {
        SaveManager.Instance.DeleteSavedGame();
        GameSessionManager.Instance.NewGame = true;
        GameSessionManager.Instance.CubeSize = selectedSize;
        GoToCoreGame();
    }

    private void GoToCoreGame()
    {
        GameSessionManager.Instance.NextScene = 2;
        SceneManager.LoadScene(0);
    }
}