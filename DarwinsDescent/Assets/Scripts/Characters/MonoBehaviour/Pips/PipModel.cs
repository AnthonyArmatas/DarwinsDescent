using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips
{
    public class PipModel
    {
        public enum PartName
        {
            Head,
            Arms,
            Chest,
            Legs
        }

        public PartName Name { get; set; }

        // Max amount of pips
        public int MaxCap { get; set; }
        public int Allocated { get; set; }
        public int CurAvalible { get
            {
                return MaxCap - Allocated;
            }
        }
        public bool Locked { get; set; }
    }
}
