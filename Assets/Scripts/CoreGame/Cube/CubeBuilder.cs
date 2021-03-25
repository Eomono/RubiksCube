using System.Collections.Generic;
using UnityEngine;
using static CubeUtils;

public class CubeBuilder : MonoBehaviour
{
    [SerializeField] private GameObject prefabCubelet;
    [SerializeField] private GameObject prefabSticker;

    public CubeManager CreateCube(int size)
    {
        CubeManager cubeManager = new GameObject("Cube").AddComponent<CubeManager>();

        List<Cubelet> cubeletList = new List<Cubelet>();

        float startingPoint = (size - 1) * -0.5f;
        
        Vector3 pos = Vector3.one * startingPoint;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    //Check if array is not inside of the cube
                    if (!((i > 0 && i < (size - 1)) && (j > 0 && j < (size - 1)) && (k > 0 && k < (size - 1))))
                    {
                        Cubelet cubelet = SpawnCubelet(cubeManager.transform, pos, ("C" + i + j + k));

                        if (i == 0)
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Back));
                        else if (i == (size - 1))
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Front));

                        if (k == 0)
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Left));
                        else if (k == (size - 1))
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Right));
                        
                        if(j == 0)
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Bottom));
                        else if(j == (size - 1))
                            cubelet.AddSticker(SpawnSticker(cubelet, CubeDirections.Top));
                        
                        cubeletList.Add(cubelet);
                    }

                    pos.x += CubeletSize;
                }
                
                pos.y += CubeletSize;
                pos.x = startingPoint;
            }

            pos.z += CubeletSize;
            pos.y = startingPoint;
        }
        
        cubeManager.SetCube(cubeletList, size);

        return cubeManager;
    }

    public CubeManager LoadCube(int size, List<List<List<int>>> stickers)
    {
        CubeManager cubeManager = new GameObject("Cube").AddComponent<CubeManager>();

        List<Cubelet> cubeletList = new List<Cubelet>();

        float startingPoint = (size - 1) * -0.5f;
        
        Vector3 pos = Vector3.one * startingPoint;
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    //Check if array is not inside of the cube
                    if (!((i > 0 && i < (size - 1)) && (j > 0 && j < (size - 1)) && (k > 0 && k < (size - 1))))
                    {
                        Cubelet cubelet = SpawnCubelet(cubeManager.transform, pos, ("C" + i + j + k));

                        if (i == 0)
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Back, (StickerColors)stickers[0][j][k]));
                        else if (i == (size - 1))
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Front, (StickerColors)stickers[2][j][k]));
                        
                        if (k == 0)
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Left, (StickerColors)stickers[3][j][i]));
                        else if (k == (size - 1))
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Right, (StickerColors)stickers[1][j][i]));
                        
                        if(j == 0)
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Bottom, (StickerColors)stickers[4][i][k]));
                        else if(j == (size - 1))
                            cubelet.AddSticker(LoadSticker(cubelet, CubeDirections.Top, (StickerColors)stickers[5][i][k]));
                        
                        cubeletList.Add(cubelet);
                    }

                    pos.x += CubeletSize;
                }
                
                pos.y += CubeletSize;
                pos.x = startingPoint;
            }

            pos.z += CubeletSize;
            pos.y = startingPoint;
        }
        
        cubeManager.SetCube(cubeletList, size);

        return cubeManager;
    }

    private Cubelet SpawnCubelet(Transform parent, Vector3 pos, string nm)
    {
        GameObject cubelet = Instantiate(prefabCubelet, pos, Quaternion.identity);
        cubelet.name = nm;
        cubelet.transform.SetParent(parent, true);
        return cubelet.AddComponent<Cubelet>();
    }

    private Sticker SpawnSticker(Cubelet parent, CubeDirections dir)
    {
        Sticker sticker = Instantiate(prefabSticker, parent.transform).AddComponent<Sticker>();

        Vector3 vDir = DirectionsOrdered[(int)dir];
        
        sticker.transform.localPosition = vDir * 0.5f;
        sticker.transform.forward = vDir;

        sticker.GetComponentInChildren<MeshRenderer>().material.color = StickersColorsOrdered[(int)dir];

        sticker.MyColor = (StickerColors)dir;

        return sticker;
    }
    
    private Sticker LoadSticker(Cubelet parent, CubeDirections dir, StickerColors color)
    {
        Sticker sticker = Instantiate(prefabSticker, parent.transform).AddComponent<Sticker>();

        Vector3 vDir = DirectionsOrdered[(int)dir];
        
        sticker.transform.localPosition = vDir * 0.5f;
        sticker.transform.forward = vDir;

        sticker.GetComponentInChildren<MeshRenderer>().material.color = StickersColorsOrdered[(int)color];
        sticker.MyColor = color;

        return sticker;
    }
}