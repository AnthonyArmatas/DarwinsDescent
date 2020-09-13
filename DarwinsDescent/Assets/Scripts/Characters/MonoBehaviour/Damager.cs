﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace DarwinsDescent
{
    public class Damager : MonoBehaviour
    {
        //[Serializable]
        //public class DamagableEvent : UnityEvent<Damager, Damageable>
        //{ }


        //[Serializable]
        //public class NonDamagableEvent : UnityEvent<Damager>
        //{ }

        ////call that from inside the onDamageableHIt or OnNonDamageableHit to get what was hit.
        //public Collider2D LastHit { get { return m_LastHit; } }

        public int damage = 1;
        //public Vector2 offset = new Vector2(1.5f, 1f);
        //public Vector2 size = new Vector2(2.5f, 1f);
        //[Tooltip("If this is set, the offset x will be changed base on the sprite flipX setting. e.g. Allow to make the damager alway forward in the direction of sprite")]
        //public bool offsetBasedOnSpriteFacing = true;
        //[Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
        //public SpriteRenderer spriteRenderer;
        //[Tooltip("If disabled, damager ignore trigger when casting for damage")]
        //public bool canHitTriggers;
        //public bool disableDamageAfterHit = false;
        //[Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
        //public bool forceRespawn = false;
        //[Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
        public bool ignoreInvincibility = false;
        public LayerMask hittableLayers;
        //public DamagableEvent OnDamageableHit;
        //public NonDamagableEvent OnNonDamageableHit;

        //protected bool m_SpriteOriginallyFlipped;
        //protected bool m_CanDamage = true;
        //protected ContactFilter2D m_AttackContactFilter;
        //protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
        //protected Transform m_DamagerTransform;
        //protected Collider2D m_LastHit;

        protected Animator animator;
        protected readonly int HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");
        protected BoxCollider2D AttackCollider = null;
        //
        //

        void Start()
        {
            //Physics.IgnoreLayerCollision(13, 21);
            animator = GetComponentInChildren<Animator>();
            //Can also use MeleeAtkBCollider = transform.Find("MeleeHitBox").GetComponent<BoxCollider2D>();
            BoxCollider2D[] childColliders = GetComponentsInChildren<BoxCollider2D>();
            for (int colliderCnt = 1; colliderCnt < childColliders.Length; colliderCnt++)
            {
                if (childColliders[colliderCnt].gameObject.name == "MeleeHitBox")
                {
                    AttackCollider = childColliders[colliderCnt];
                }
            }
            if(AttackCollider == null)
            {
                throw new ArgumentNullException(nameof(AttackCollider));
            }
        }

        void Awake()
        {
            //m_AttackContactFilter.layerMask = hittableLayers;
            //m_AttackContactFilter.useLayerMask = true;
            //m_AttackContactFilter.useTriggers = canHitTriggers;

            //if (offsetBasedOnSpriteFacing && spriteRenderer != null)
            //    m_SpriteOriginallyFlipped = spriteRenderer.flipX;

            //m_DamagerTransform = transform;
        }


        void FixedUpdate()
        {
            bool attacking = animator.GetBool(HashMeleeAttackPara);
            if (!attacking)
                return;

            //AttackCollider.ontr
        }

        // TODO: QUESTION: The gameobject this is attached to does not have a trigger, and this does not get called when it is hit, but its child object
        // does have a collider which is a trigger. Why does this work?
        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool attacking = animator.GetBool(HashMeleeAttackPara);
            if (!attacking)
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
                    damageable.TakeDamage(this, ignoreInvincibility);


                    //if (disableDamageAfterHit)
                    //    DisableDamage();
                }
                else
                {
                    //OnNonDamageableHit.Invoke(this);
                }
            }

        }
        //void FixedUpdate()
        //{
        //    bool attacking = animator.GetBool(HashMeleeAttackPara);

        //    if (!attacking)
        //        return;

        //    Vector2 scale = m_DamagerTransform.lossyScale;

        //    Vector2 facingOffset = Vector2.Scale(offset, scale);
        //    if (offsetBasedOnSpriteFacing && spriteRenderer != null && spriteRenderer.flipX != m_SpriteOriginallyFlipped)
        //        facingOffset = new Vector2(-offset.x * scale.x, offset.y * scale.y);

        //    Vector2 scaledSize = Vector2.Scale(size, scale);

        //    Vector2 pointA = (Vector2)m_DamagerTransform.position + facingOffset - scaledSize * 0.5f;
        //    Vector2 pointB = pointA + scaledSize;

        //    int hitCount = Physics2D.OverlapArea(pointA, pointB, m_AttackContactFilter, m_AttackOverlapResults);

        //    for (int i = 0; i < hitCount; i++)
        //    {
        //        m_LastHit = m_AttackOverlapResults[i];
        //        Damageable damageable = m_LastHit.GetComponent<Damageable>();

        //        if (damageable)
        //        {
        //            OnDamageableHit.Invoke(this, damageable);
        //            damageable.TakeDamage(this, ignoreInvincibility);
        //            if (disableDamageAfterHit)
        //                DisableDamage();
        //        }
        //        else
        //        {
        //            OnNonDamageableHit.Invoke(this);
        //        }
        //    }
        //}
    }
}