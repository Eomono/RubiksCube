using UnityEngine;

public static class CubeUtils
{
    public enum Axis { X = 0, Y = 1, Z = 2 }
    
    public enum CubeDirections { Front = 0, Right, Back, Left, Bottom, Top }
    
    public enum StickerColors { Blue = 0, Red, Green, Orange, Yellow, White }

    public static Vector3[] DirectionsOrdered = new Vector3[]
    {
        Vector3.forward, Vector3.right, Vector3.back, Vector3.left, Vector3.down, Vector3.up
    };

    public static Color[] StickersColorsOrdered = new Color[]
    {
        new Color(0f, 0.2705f, 0.6784f),
        new Color(0.7254f, 0f, 0f),
        new Color(0f, 0.6078f, 0.2823f),
        new Color(1f, 0.349f, 0f),
        new Color(1f, 0.8352f, 0f),
        new Color(1f, 1f, 1f)
    };

    public static float CubeletSize = 1f;
    
    public static float AutoRotSpeed = 30f;
    public static float PlayerRotSpeed = 12f;
    public static float UndoRotSpeed = 16f;

    public static int MinCubeSize = 2;
    public static int MaxCubeSize = 6;
    
    public static int ScrambleRotationsMin = 20;
    public static int ScrambleRotationsMax = 30;
}