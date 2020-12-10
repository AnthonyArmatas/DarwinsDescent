using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class EnemyDamager : Damager
    {
        private EnemySMF enemySMF = new EnemySMF();
        public bool NeedToBeAttacking = false;

        void Awake()
        {
            if (damage == 0)
                damage = 1;

            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    animator = GetComponentInParent<Animator>();
                }
            }

            if (AttackCollider == null)
            {
                AttackCollider = GetComponent<BoxCollider2D>();
            }

            if (AttackCollider == null)
            {
                throw new ArgumentNullException(nameof(AttackCollider));
            }
        }

        // TODO: QUESTION: The gameobject this is attached to does not have a trigger, and this does not get called when it is hit, but its child object
        // does have a collider which is a trigger. Why does this work?
        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool attacking = animator.GetBool(enemySMF.BaseAttackHash);
            if (!attacking && NeedToBeAttacking)
                return;

            // This checks the layers its hit.
            if (hittableLayers.value == (hittableLayers.value | (1 << collision.gameObject.layer)))
            {
                Damageable damageable = collision.GetComponentInParent<Damageable>();
                if (damageable)
                {
                    // The idea behind this is to call some function when hit (Lets say there was a desire for an explosion to dmg enemies on hit
                    // From there that function could be called instead of invoke.)
                    //OnDamageableHit.Invoke(this, damageable);
                    damageable.TakeDamage(damage);

                    //if (disableDamageAfterHit)
                    //    DisableDamage();
                }
                else
                {
                    //OnNonDamageableHit.Invoke(this);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            bool attacking = animator.GetBool(enemySMF.BaseAttackHash);
            if (!attacking)
                return;

            // This checks the layers its hit.
            if (hittableLayers.value == (hittableLayers.value | (1 << collision.gameObject.layer)))
            {
                Damageable damageable = collision.collider.GetComponentInParent<Damageable>();
                if (damageable)
                {
                    // The idea behind this is to call some function when hit (Lets say there was a desire for an explosion to dmg enemies on hit
                    // From there that function could be called instead of invoke.)
                    //OnDamageableHit.Invoke(this, damageable);
                    damageable.TakeDamage(damage);

                    //if (disableDamageAfterHit)
                    //    DisableDamage();
                }
                else
                {
                    //OnNonDamageableHit.Invoke(this);
                }
            }
        }
    }
}