using UnityEngine;

public class GameSessionManager : Singleton<GameSessionManager>
{
    public int NextScene = 1;
    public int CubeSize { get; set; } = 3;
    public bool NewGame = true;

    private float[] bestTimes;

    public override void Awake()
    {
        base.Awake();

        LoadBestTimes();
    }

    public float GetBestTime(int size)
    {
        size -= 2;
        return bestTimes[size];
    }

    public void HandleNewCompletedTime(float playerTime, int size)
    {
        string recordKey = "BestTime" + size.ToString();
        size -= 2;
        if (playerTime >= bestTimes[size]) return;
        
        PlayerPrefs.SetFloat(recordKey, playerTime);
        PlayerPrefs.Save();
        bestTimes[size] = playerTime;
    }

    private void LoadBestTimes()
    {
        bestTimes = new[]
        {
            PlayerPrefs.GetFloat("BestTime2", 86400f),
            PlayerPrefs.GetFloat("BestTime3", 86400f),
            PlayerPrefs.GetFloat("BestTime4", 86400f),
            PlayerPrefs.GetFloat("BestTime5", 86400f),
            PlayerPrefs.GetFloat("BestTime6", 86400f)
        };
    }
}