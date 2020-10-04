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

        private int _curHealth;
        public int CurHealth 
        {
            get 
            {
                return _curHealth;
            }
            set 
            {
                _curHealth = value;
            }
        }
        public HealthModel(int maxHp)
        {
            MaxHP = maxHp;
            CurHealth = maxHp;
        }
    }
}
