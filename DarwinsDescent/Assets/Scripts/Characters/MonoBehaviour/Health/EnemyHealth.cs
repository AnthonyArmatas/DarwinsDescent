using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class EnemyHealth : Health
    {
        public override int GetCurrentHealth()
        {
            return CurHealth;
        }

        public override void InitializeHealth(int StartingHealth)
        {
            InitializeHealth(StartingHealth, StartingHealth);
        }

        public override void InitializeHealth(int StartingHealth, int MaxHp)
        {
            if (StartingHealth > MaxHp)
                StartingHealth = MaxHp;

            MaxHP = MaxHp;
            CurHealth = StartingHealth;
        }

        public override void InitializeHealth(int StartingHealth, int MaxHp, int RealHp, int TempHp)
        {
            if (StartingHealth > MaxHp)
                StartingHealth = MaxHp;

            InitializeHealth(StartingHealth, MaxHp);
        }
    }
}
