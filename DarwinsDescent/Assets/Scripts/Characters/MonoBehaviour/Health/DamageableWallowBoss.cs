using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent
{
    public class DamageableWallowBoss : Damageable
    {
        public EnemyHealth enemyHealth { get; set; }
        public override Health health
        {
            get { return enemyHealth; }
            set { enemyHealth = (EnemyHealth)value; }
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
            if (health.CurHealth <= 0)
                return;

            if (DamageAmount >= health.CurHealth)
            {
                health.CurHealth = 0;
                animator.SetTrigger("Dead");
                return;
            }
            health.CurHealth -= DamageAmount;
            animator.SetTrigger("Hurt");

        }
    }
}
