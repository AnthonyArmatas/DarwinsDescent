using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{ 
    public class PlayerHealth : Health
    {
        public int RealHp { get; set; }

        // Min Real HP that can exist in the hp pool. Used when funneling to the pip pad so the player does not kill self.
        public int MinRealHp 
        { 
            get
            {
                return 1;
            }
        }
        public int LentHp;
        public int TempHp { get; set; }

        public override int CurHealth => GetCurrentHealth();

        public int DamagedHealth => GetDamagedHealth();

        public override int GetCurrentHealth()
        {
            return RealHp + TempHp;
        }

        private int GetDamagedHealth()
        {
            //Lent HP is not Damaged since it was full and given to the pippad
            return MaxHP - RealHp - LentHp;
        }

        public override void InitializeHealth(int StartingHealth)
        {
            InitializeHealth(StartingHealth, StartingHealth, StartingHealth, 0);
        }

        public override void InitializeHealth(int StartingHealth, int MaxHp)
        {
            if (StartingHealth > MaxHp)
                StartingHealth = MaxHp;
            InitializeHealth(StartingHealth, MaxHp, StartingHealth, 0);
        }

        public override void InitializeHealth(int startingHealth, int maxHp, int realHp, int tempHp)
        {
            if (startingHealth != realHp + tempHp)
                throw new ArgumentException("Passed parameters do not match up correctly." +
                    " Starting Health " + startingHealth + " does not equal the sum of Real Hp and Temp Hp " + realHp + " " + tempHp);

            if(realHp == 0)
            {
                realHp = startingHealth;
            }

            RealHp = realHp;
            TempHp = tempHp;
            MaxHP = maxHp;
            LentHp = 0;
        }
    }
}
