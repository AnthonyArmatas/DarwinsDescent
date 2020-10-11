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
        public int CurHealth;

        public abstract int GetCurrentHealth();
        public abstract void HealDamage();
        public abstract void TakeDamage(int DamageAmount);
        public abstract void InitializeHealth(int StartingHealth);
        public abstract void InitializeHealth(int StartingHealth, int MaxHp);
        public abstract void InitializeHealth(int StartingHealth, int MaxHp, int RealHp, int TempHp);
    }
}
