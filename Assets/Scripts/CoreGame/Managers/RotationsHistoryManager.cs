using System;
using System.Collections.Generic;
using UnityEngine;
using static CubeUtils;

public class RotationsHistoryManager : MonoBehaviour
{
    public List<RotationAction> RotationHistory { get; private set; } = new List<RotationAction>();
    public List<RotationAction> ScrambleHistory { get; private set; } = new List<RotationAction>();

    private HUDManager hudManager;

    private void Awake()
    {
        hudManager = GameObject.FindWithTag("HUD").GetComponent<HUDManager>();
    }

    public void LoadRotationHistory(List<RotationAction> history)
    {
        RotationHistory = history;
    }

    public void CleanHistory()
    {
        RotationHistory.Clear();
        hudManager.EnableUndo(false);
    }

    public void RecordScramble(Axis rotAx, int coord, bool cw)
    {
        ScrambleHistory.Add(new RotationAction(rotAx, coord, cw));
    }

    public void RecordAction(Axis rotAx, int coord, bool cw)
    {
        RotationHistory.Add(new RotationAction(rotAx, coord, cw));
        hudManager.EnableUndo(RotationHistory.Count > 0);
    }

    public RotationAction UndoRotation()
    {
        if (RotationHistory.Count < 1) return null;
        
        RotationAction rotAct = RotationHistory[RotationHistory.Count - 1];
        RotationHistory.RemoveAt(RotationHistory.Count - 1);
        
        hudManager.EnableUndo(RotationHistory.Count > 0);

        return rotAct;
    }
}