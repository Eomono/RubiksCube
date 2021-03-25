using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static CubeUtils;

public class SaveManager : Singleton<SaveManager>
{
    private string filePath;

    private SavedGame currentSave;

    public override void Awake()
    {
        base.Awake();
        filePath = Application.persistentDataPath + "/save.data";
    }

    public bool SavedGameExists => currentSave != null && currentSave.HasData;

    public void SaveGame(bool hasData, int cubeSize, List<List<List<int>>> stickers, List<RotationAction> history, float timer)
    {
        int[][][] stckrs = new int[stickers.Count][][];

        for (int i = 0; i < stickers.Count; i++)
        {
            stckrs[i] = new int[stickers[i].Count][];

            for (int j = 0; j < stickers[i].Count; j++)
                stckrs[i][j] = stickers[i][j].ToArray();
        }

        int[][] hstry = new int[history.Count][];

        for (int i = 0; i < history.Count; i++)
        {
            hstry[i] = new[] {(int)history[i].RotAxis, history[i].Coordinate, history[i].Clockwise?1:0 };
        }

        SavedGame savedGame = new SavedGame(hasData, cubeSize, stckrs, hstry, timer);

        WriteSaveData(savedGame);
    }

    public void DeleteSavedGame()
    {
        int[][][] stckrs = new int[0][][];
        int[][] hstry = new int[0][];

        SavedGame savedGame = new SavedGame(false, 3, stckrs, hstry, 0f);
        
        WriteSaveData(savedGame);
    }

    private void WriteSaveData(SavedGame savedGame)
    {
        if(File.Exists(filePath))
            File.Delete(filePath);
        
        FileStream dataStream = new FileStream(filePath, FileMode.Create);
        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, savedGame);

        dataStream.Close();

        currentSave = savedGame;
    }

    public void PreLoadSavedGame()
    {
        if(!SavedGameExists)
            currentSave = LoadSavedGameData();
    }

    /// <summary>
    /// Remember to call PreLoadSavedGame before calling this function!
    /// </summary>
    public void LoadCurrentSavedGame(out int cubeSize, out List<List<List<int>>> stickers, out List<RotationAction> history, out float timer)
    {
        cubeSize = 3;
        timer = 0f;
        stickers = new List<List<List<int>>>();
        history = new List<RotationAction>();
        
        if (!SavedGameExists) return;

        cubeSize = currentSave.CubeSize;

        for (int i = 0; i < 6; i++)
        {
            List<List<int>> face = new List<List<int>>();

            for (int j = 0; j < cubeSize; j++)
            {
                List<int> row = new List<int>();

                for (int k = 0; k < cubeSize; k++)
                    row.Add(currentSave.StickersState[i][j][k]);

                face.Add(row);
            }

            stickers.Add(face);
        }

        for (int i = 0; i < currentSave.History.Length; i++)
        {
            int[] rawData = currentSave.History[i];
            RotationAction rotAction = new RotationAction((Axis)rawData[0], rawData[1], rawData[2] == 1);
            history.Add(rotAction);
        }

        timer = currentSave.Timer;
    }

    private SavedGame LoadSavedGameData()
    {
        if (!File.Exists(filePath)) return null;
        
        FileStream dataStream = new FileStream(filePath, FileMode.Open);

        BinaryFormatter converter = new BinaryFormatter();
        SavedGame saveData = converter.Deserialize(dataStream) as SavedGame;

        dataStream.Close();
            
        return saveData;
    }
}