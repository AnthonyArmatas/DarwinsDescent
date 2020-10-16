using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public abstract class Health : MonoBehaviour
    {
        public int MaxHP;
        public virtual int CurHealth { get; set; }

        public abstract int GetCurrentHealth();
        public abstract void InitializeHealth(int StartingHealth);
        public abstract void InitializeHealth(int StartingHealth, int MaxHp);
        public abstract void InitializeHealth(int StartingHealth, int MaxHp, int RealHp, int TempHp);
    }
}
