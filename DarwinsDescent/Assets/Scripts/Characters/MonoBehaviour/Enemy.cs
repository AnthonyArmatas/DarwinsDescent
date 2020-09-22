using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class Enemy : Actor
    {
        void Awake()
        {
            this.Rigidbody2D = GetComponent<Rigidbody2D>();
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.animator = GetComponent<Animator>();
            this.boxCollider = GetComponent<BoxCollider2D>();

            this.contactFilter = new ContactFilter2D()
            {
                layerMask = groundedLayerMask,
                useLayerMask = true,
                useTriggers = false,
            };

            if (health == 0)
                health = 5;
            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (baseAttackDamage == 0)
                baseAttackDamage = 1;
        }
    }
}