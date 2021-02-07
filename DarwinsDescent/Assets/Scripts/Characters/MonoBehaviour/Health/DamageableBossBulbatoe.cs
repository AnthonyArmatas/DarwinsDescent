using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class DamageableBossBulbatoe : Damageable
    {
        public WallowBoss wallowBoss;
        public BossBulbatoes bossBulbatoes;
        public EnemyHealth enemyHealth { get; set; }
        public override Health health
        {
            get { return enemyHealth; }
            set { enemyHealth = (EnemyHealth)value; }
        }

        // Initializes onEnable so the Enemy is Initialized first
        void Awake()
        {
            if (health == null)
                health = GetComponent<EnemyHealth>();

            if (health != null)
            {
                health.InitializeHealth(StartingHealth);
            }
            else
            {
                throw new ArgumentNullException("Could not find EnemyHealth");
            }

            if (wallowBoss == null)
            {
                wallowBoss = GameObject.Find("WallowBossCore").GetComponent<WallowBoss>();
            }

            if (bossBulbatoes == null)
                bossBulbatoes = GetComponent<BossBulbatoes>();
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
            if (health.CurHealth <= 0 || !animator.GetBool("Killable"))
                return;

            if (DamageAmount >= health.CurHealth)
            {
                animator.SetBool("Killable", false);
                animator.SetTrigger("Reset");

                // We want to skip this if it is rotting so it is not double called.
                if (!bossBulbatoes.Rotting)
                    bossBulbatoes.BossBulbatoeHandler.ResetBulbatoe(bossBulbatoes);
                
                health.CurHealth = 0;
                animator.SetBool("Destroyed", true);
                animator.SetTrigger("Dead");


                // For each bulbatoe destroyed make the wallow demon take one dmg
                wallowBoss.TakeDamage(1);
                return;
            }
            health.CurHealth -= DamageAmount;
        }
    }
}
