using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace DarwinsDescent
{
    public class DamagerRange : MonoBehaviour
    {
        public Animator animator;
        public Enemy enemy;
        public EnemySMF enemySMF = new EnemySMF();
        protected BoxCollider2D AttackCollider = null;

        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }
            }
            if (AttackCollider == null)
            {
                AttackCollider = GetComponent<BoxCollider2D>();
                if (AttackCollider == null)
                {
                    AttackCollider = GetComponentInChildren<BoxCollider2D>();
                }
            }
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
                if (enemy == null)
                {
                    enemy = GetComponentInParent<Enemy>();
                }
                if (enemy == null)
                {
                    enemy = GetComponentInChildren<Enemy>();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (enemy.damageable.health.CurHealth <= 0)
                return;

            animator.SetTrigger(enemySMF.BaseAttackHash);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (enemy.damageable.health.CurHealth <= 0)
                return;

            animator.SetTrigger(enemySMF.BaseAttackHash);
        }
    }
}
