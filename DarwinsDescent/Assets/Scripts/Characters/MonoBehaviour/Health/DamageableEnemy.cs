using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class DamageableEnemy : Damageable
    {
        public Enemy enemy;
        public EnemySMF SMF;

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

            if(health != null)
            {
                health.InitializeHealth(StartingHealth);
            }
            else
            {
                throw new ArgumentNullException("Could not find EnemyHealth");
            }
        }

        void OnEnable()
        {
            if(enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }
            if (enemy != null)
            {
                SMF = enemy.SMF;
                animator = enemy.animator;
            }
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
            // TODO: Implement invincibility
            //if ((Invulnerable && !ignoreInvincible) || health.CurHealth <= 0)
            //    return;
            if (health.CurHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            //if (!Invulnerable)
            //{
            //    actor.health.CurHealth -= damager.damage;
            //    // Maybe Turn this OnHealth Set to an event that resets any temp timer
            //    // It makes sense to call it here since this is where it absolutely takes dmg away from
            //    // an enemy and since its an event its still loosely coupled.
            //    OnHealthSet.Invoke(this);
            //}
            if (DamageAmount >= health.CurHealth)
            {
                health.CurHealth = 0;
                animator.SetBool(SMF.DeadHash, true);
                return;
            }
            health.CurHealth -= DamageAmount;
            animator.SetTrigger(SMF.HurtHash);

            // Do we want to do dmg direction for enemies? I'm not sure we do
            //DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;        
        }
    }
}
