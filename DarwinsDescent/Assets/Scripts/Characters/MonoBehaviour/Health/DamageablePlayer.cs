using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent
{
    public class DamageablePlayer : Damageable
    {
        void Awake()
        {
            if (health == null)
                health = GetComponent<PlayerHealth>();
            health.InitializeHealth(StartingHealth);
        }

        public override void GainHealth(int HealAmount)
        {
            throw new NotImplementedException();
        }

        public override void SetHealth(int HPAmount)
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(int DamageAmount)
        {
            throw new NotImplementedException();
        }
    }
}
