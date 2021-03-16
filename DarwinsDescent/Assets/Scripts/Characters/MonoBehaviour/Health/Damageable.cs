using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public abstract class Damageable : MonoBehaviour
    {
        public virtual Health health { get; set; }
        public Animator animator;
        public int StartingHealth;
        public bool Invincible;
        public bool JustHit;
        public float InvincibleTime;


        public void SetHealth(Health hp)
        {
            health = hp;
        }

        public abstract void TakeDamage(int DamageAmount);

        public abstract void GainHealth(int HealAmount);

        public abstract void SetHealth(int HPAmount);

    }
}
