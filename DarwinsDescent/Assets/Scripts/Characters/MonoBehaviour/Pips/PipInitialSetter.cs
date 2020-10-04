using DarwinsDescent;
using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipInitialSetter : MonoBehaviour
{
    public PipSystem pipsetter;
    public bool HeadLocked;
    public int HeadMaxCap;
    public int HeadAllocated;

    public bool ArmsLocked;
    public int ArmsMaxCap;
    public int ArmsAllocated;

    public bool ChestLocked;
    public int ChestMaxCap;
    public int ChestAllocated;

    public bool LegsLocked;
    public int LegsMaxCap;
    public int LegsAllocated;



    void Awake()
    {
        pipsetter.Initialized += InitializePipParts;
    }
    
    public void InitializePipParts(PipModel Head, PipModel Arms, PipModel Chest, PipModel Legs)
    {
        if(Head == null ||
            Arms == null ||
            Chest == null ||
            Legs == null)
        {
            throw new ArgumentNullException(Head.ToString() + Arms.ToString() + Chest.ToString() + Legs.ToString());
        }

        SetAttributes(Head, HeadLocked, HeadMaxCap, HeadAllocated, PipModel.PartName.Head);
        SetAttributes(Arms, ArmsLocked, ArmsMaxCap, ArmsAllocated, PipModel.PartName.Arms);
        SetAttributes(Chest, ChestLocked, ChestMaxCap, ChestAllocated, PipModel.PartName.Chest);
        SetAttributes(Legs, LegsLocked, LegsMaxCap, LegsAllocated, PipModel.PartName.Legs);
    }

    private void SetAttributes(PipModel Part, bool Locked, int MaxCap, int Allocated, PipModel.PartName partName)
    {
        // Sets the parts instead of creating a new gameobject as that would change the memory address and the invoked parts
        // would not get updated.
        Part.Locked = Locked;
        Part.MaxCap = MaxCap;
        Part.Allocated = Allocated;
        Part.Name = partName;
    }
}
