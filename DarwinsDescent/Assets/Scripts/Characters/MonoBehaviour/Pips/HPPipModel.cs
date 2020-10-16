using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DarwinsDescent
{
    public class HPPipModel : MonoBehaviour
    {
        public enum state
        {
            Real,
            Temp,
            Damaged,
            Lent
        }
        public state CurState;

        public int TempTime;

        public Image PipDisplayImage;
    }
}
