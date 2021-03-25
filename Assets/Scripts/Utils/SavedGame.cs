[System.Serializable]
public class SavedGame
{
    public bool HasData;
    public int CubeSize;
    public int[][][] StickersState;
    public int[][] History;
    public float Timer;

    public SavedGame(bool hasData, int cubeSize, int[][][] stickersState, int[][] history, float timer)
    {
        HasData = hasData;
        CubeSize = cubeSize;
        StickersState = stickersState;
        History = history;
        Timer = timer;
    }
}