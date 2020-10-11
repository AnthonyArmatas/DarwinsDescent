using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent.Assets.Scripts
{
    public class HealthModel
    {
        public int MaxHP;

        private int _CurHealth;
        public int CurHealth 
        {
            get 
            {
                return _CurHealth;
            }
            set 
            {
                _CurHealth = value;
            }
        }
        public HealthModel(int maxHp)
        {
            MaxHP = maxHp;
            CurHealth = maxHp;
        }
    }
}
