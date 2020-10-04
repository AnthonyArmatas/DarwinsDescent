using DarwinsDescent.Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class HealthPipModel : HealthModel
    {
        public int MinHp { get; set; }
        public int RealHp { get; set; }
        public int LentHp { get; set; }
        public int TempHp { get; set; }

        public new int CurHealth
        {
            get
            {
                return GetCurHealth();
            }
        }

        public HealthPipModel(int maxHp, int minHp)
            :base(maxHp)
        {
            MaxHP = maxHp;
            MinHp = minHp;
            RealHp = maxHp;
        }

        protected int GetCurHealth()
        {
            return RealHp + TempHp;
        }
    }

}