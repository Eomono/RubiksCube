using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static CubeUtils;

public class CubeManager : MonoBehaviour
{
    private enum CubeState { Idle, Rotating }
    
    public int CubeSize { get; private set; }

    private GameManager gameManager;
    private RotationsHistoryManager historyManager;

    private SectionRotator sectionRotator;

    private List<Cubelet> cubeletsList;

    private CubeState currentState;

    private int prevRandomAxis;
    
    public void Awake()
    {
        transform.position = Vector3.zero;
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        historyManager = gameManager.gameObject.GetComponent<RotationsHistoryManager>();
        
        sectionRotator = new GameObject("SectionRotator").AddComponent<SectionRotator>();
        sectionRotator.transform.SetParent(transform);
        sectionRotator.transform.localPosition = Vector3.zero;

        currentState = CubeState.Idle;
    }

    public void SetCube(List<Cubelet> cList, int size)
    {
        cubeletsList = cList;
        CubeSize = size;
    }

    public void UndoRotation()
    {
        if(currentState != CubeState.Idle) return;
        
        RotationAction rotUndo = historyManager.UndoRotation();
        if(rotUndo == null) return;
        RotateSectionAxis(rotUndo.RotAxis, rotUndo.Coordinate, !rotUndo.Clockwise, UndoRotSpeed);
    }

    public bool RandomRotation()
    {
        if(currentState != CubeState.Idle) return false;

        //Alternating axes to avoid a randomly repeated rotation but in opposite direction which doesn't look appealing during a scramble
        Axis rotAxis = (Axis)prevRandomAxis;
        prevRandomAxis++;
        if (prevRandomAxis > 2) prevRandomAxis = 0;
        
        int coord = Random.Range(0, CubeSize);
        bool clockwise = (Random.value > 0.5f);
        
        RotateSectionAxis(rotAxis, coord, clockwise, AutoRotSpeed);
        historyManager.RecordScramble(rotAxis, coord, clockwise);
        return true;
    }

    public void SetRotation(Transform cubelet, Axis rotAxis, bool clockwise)
    {
        if(currentState != CubeState.Idle) return;
        
        int coord = FindCubeletCoordAtAxis(rotAxis, cubelet.localPosition);

        RotateSectionAxis(rotAxis, coord, clockwise, PlayerRotSpeed);
        historyManager.RecordAction(rotAxis, coord, clockwise);
    }

    public List<RotationAction> PackDataForSaving(out int cSize, out List<List<List<int>>> stickers)
    {
        cSize = CubeSize;

        stickers = new List<List<List<int>>>
        {
            //Pack Front face
            PackFaceState(GetCubeletSectionForAxis(Axis.Z, 0), Vector3.back, Axis.X, Axis.Y),
            //Pack Right face
            PackFaceState(GetCubeletSectionForAxis(Axis.X, CubeSize - 1), Vector3.right, Axis.Z, Axis.Y),
            //Check Back face
            PackFaceState(GetCubeletSectionForAxis(Axis.Z, CubeSize - 1), Vector3.forward, Axis.X, Axis.Y),
            //Check Left face
            PackFaceState(GetCubeletSectionForAxis(Axis.X, 0), Vector3.left, Axis.Z, Axis.Y),
            //Check Bottom face
            PackFaceState(GetCubeletSectionForAxis(Axis.Y, 0), Vector3.down, Axis.X, Axis.Z),
            //Check Top face
            PackFaceState(GetCubeletSectionForAxis(Axis.Y, CubeSize - 1), Vector3.up, Axis.X, Axis.Z)
        };

        return historyManager.RotationHistory;
    }

    private void RotateSectionAxis(Axis rotAxis, int coord, bool clockwise, float speed)
    {
        currentState = CubeState.Rotating;
        
        Vector3 targetPos = Vector3.zero;
        targetPos[(int)rotAxis] = (((CubeSize - 1) * -0.5f) +coord);
        
        Vector3 rotEuler = Vector3.zero;
        rotEuler[(int) rotAxis] = clockwise ? 90f : -90f;
        Quaternion targetRot = Quaternion.Euler(rotEuler);

        List<Cubelet> cubeletsSection = GetCubeletSectionForAxis(rotAxis, coord);

        sectionRotator.transform.localPosition = targetPos;
        sectionRotator.transform.localRotation = Quaternion.identity;

        foreach (Cubelet c in cubeletsSection)
            c.transform.SetParent(sectionRotator.transform, true);

        StartCoroutine(RotatingSection(cubeletsSection, targetRot, speed));
    }

    private IEnumerator RotatingSection(List<Cubelet> children, Quaternion targetRot, float rotSpeed)
    {
        while (sectionRotator.RotationArrived(targetRot, rotSpeed))
            yield return null;

        sectionRotator.transform.localRotation = targetRot;
        
        foreach (Cubelet c in children)
            c.transform.SetParent(transform, true);
        
        yield return null;
        
        CheckCubeCompleted();
        
        currentState = CubeState.Idle;
    }

    private int FindCubeletCoordAtAxis(Axis rotAxis, Vector3 cubeletPos)
    {
        //This step is not essential for executing a rotation since what is actually used is the local position. However, I think it looks cleaner to store a rotation action with an int index rather than using a float from a position component
        int axisIndex = (int)rotAxis;
        for (int i = 0; i < CubeSize; i++)
        {
            float coordSpacePoint = (((CubeSize - 1) * -0.5f) + (i * CubeletSize));
            if (AreFloatsClose(cubeletPos[axisIndex], coordSpacePoint))
                return i;
        }

        //This shouldn't happen
        return 0;
    }
    
    private void CheckCubeCompleted()
    {
        //Check Front face
        if(!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.Z, 0), Vector3.back)) return;
        //Check Right face
        if(!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.X, CubeSize-1), Vector3.right)) return;
        //Check Back face
        if(!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.Z, CubeSize-1), Vector3.forward)) return;
        //Check Left face
        if(!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.X, 0), Vector3.left)) return;
        //Check Bottom face
        if (!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.Y, 0), Vector3.down)) return;
        //Check Top face
        if (!CheckFaceCompleted(GetCubeletSectionForAxis(Axis.Y, CubeSize - 1), Vector3.up)) return;
        
        gameManager.PlayerWon();
    }

    private List<Cubelet> GetCubeletSectionForAxis(Axis rotAxis, int atCoord)
    {
        List<Cubelet> section = new List<Cubelet>();
        float coordSpacePoint = (((CubeSize - 1) * -0.5f) + (atCoord * CubeletSize));

        int axisIndex = (int)rotAxis;
        foreach (Cubelet c in cubeletsList)
        {
            if(AreFloatsClose(c.transform.localPosition[axisIndex], coordSpacePoint))
                section.Add(c);
        }

        return section;
    }

    private bool CheckFaceCompleted(List<Cubelet> cubeletsEdge, Vector3 dir)
    {
        StickerColors colorToCheck = cubeletsEdge[0].GetStickerFromSide(dir).MyColor;

        for (int i = 1; i < cubeletsEdge.Count; i++)
        {
            if (cubeletsEdge[i].GetStickerFromSide(dir).MyColor != colorToCheck)
                return false;
        }

        return true;
    }

    private bool AreFloatsClose(float a, float b)
    {
        const float treshold = 0.1f;
        return (a < (b + treshold) && a > (b - treshold));
    }

    private List<List<int>> PackFaceState(List<Cubelet> cubeletsEdge, Vector3 dir, Axis localX, Axis localY)
    {
        List<List<int>> face = new List<List<int>>();

        for (int i = 0; i < CubeSize; i++)
        {
            List<int> row = new List<int>();
            
            for (int j = 0; j < CubeSize; j++)
            {
                foreach (Cubelet c in cubeletsEdge)
                {
                    Vector3 localPos = c.transform.localPosition;
                    int coordX = FindCubeletCoordAtAxis(localX, localPos);
                    int coordY = FindCubeletCoordAtAxis(localY, localPos);
                    if (coordX != j || coordY != i) continue;
                    row.Add((int)c.GetStickerFromSide(dir).MyColor);
                    break;
                }
            }
            
            face.Add(row);
        }

        return face;
    }
}