using DarwinsDescent.Assets.Scripts;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class Enemy : Actor
    {
        public int pipValue = 1;
        public int baseAtkDmg = 1;

        public AIPath aIPath;
        public AIDestinationSetter destinationSetter;

        void Awake()
        {
            this.Rigidbody2D = GetComponent<Rigidbody2D>();
            if (this.Rigidbody2D == null)
                this.Rigidbody2D = GetComponentInChildren<Rigidbody2D>();

            this.spriteRenderer = GetComponent<SpriteRenderer>();
            if (this.spriteRenderer == null)
                this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            this.animator = GetComponent<Animator>();
            if (this.animator == null)
                this.animator = GetComponentInChildren<Animator>();

            // This only works if the GFX subcomponent is a child game object of the main. TODO: Think of a better implementation
            this.boxCollider = GetComponent<BoxCollider2D>();
            if(this.boxCollider == null)
                this.boxCollider = GetComponentInChildren<BoxCollider2D>();

            aIPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            this.contactFilter = new ContactFilter2D()
            {
                layerMask = groundedLayerMask,
                useLayerMask = true,
                useTriggers = false,
            };

            health = new HealthModel(startingHealth);

            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (baseAttackDamage == 0)
                baseAttackDamage = 1;
        }

        void FixedUpdate()
        {
            CheckIsGrounded();
            if (health.CurHealth <= 0)
                destinationSetter.target = null;
                return;

            this.animator.SetFloat(this.HorizontalSpeedParaHash, aIPath.desiredVelocity.x);
            UpdateFacing(aIPath.desiredVelocity.x <= 0);
        }
    }
}