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
        protected readonly int MeleeAttackParaHash = Animator.StringToHash("MeleeAttack");
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
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            animator.SetTrigger(MeleeAttackParaHash);
        }
    }
}
