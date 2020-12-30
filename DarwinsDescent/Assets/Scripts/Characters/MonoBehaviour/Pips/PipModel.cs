using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    /// <summary>
    /// PipModel for Pip Pad
    /// </summary>
    [System.Serializable]
    public class PipModel
    {
        public enum PartName
        {
            Head,
            Arms,
            Chest,
            Legs
        }

        public PartName Name;

        // Max amount of pips
        public int MaxCap;
        public int Allocated;
        public int CurAvalible { get
            {
                return MaxCap - Allocated;
            }
        }
        public bool Locked;

        public PipModel() 
        {

        }

        public PipModel(PartName partName, int maxCap, int allocated, bool locked)
        {
            Name = partName;
            MaxCap = maxCap;
            Allocated = allocated;
            Locked = locked;
        }
    }
}
