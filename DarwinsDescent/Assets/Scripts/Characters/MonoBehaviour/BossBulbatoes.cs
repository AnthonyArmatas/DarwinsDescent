using System;
using UnityEngine;

namespace DarwinsDescent
{
    public class BossBulbatoes : Actor
    {
        public WallowBoss WallowBoss;
        public BossBulbatoeHandler BossBulbatoeHandler;
        public DamageableBossBulbatoe damageableBossBulbatoe;
        public bool Activated;
        public bool Rotting;
        public bool Killable;

        public float TimeToCrestGrow;
        public float TimeToRot;
        public float Timer;

        void Start()
        {
            if (WallowBoss == null)
                WallowBoss = GameObject.Find("WallowBossCore").GetComponent<WallowBoss>();

            if (BossBulbatoeHandler == null)
                BossBulbatoeHandler = GameObject.Find("BossBulbatoeSpawnPoints").GetComponent<BossBulbatoeHandler>();

            if (damageableBossBulbatoe == null)
                damageableBossBulbatoe = GetComponent<DamageableBossBulbatoe>();

            if (this.rigidbody2D == null)
            {
                this.rigidbody2D = GetComponent<Rigidbody2D>();
                if (this.rigidbody2D == null)
                    this.rigidbody2D = GetComponentInChildren<Rigidbody2D>();
            }

            if (this.spriteRenderer == null)
            {
                this.spriteRenderer = GetComponent<SpriteRenderer>();
                if (this.spriteRenderer == null)
                    this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (this.animator == null)
            {
                this.animator = GetComponent<Animator>();
                if (this.animator == null)
                    this.animator = GetComponentInChildren<Animator>();
            }

            // This only works if the GFX subcomponent is a child game object of the main. TODO: Think of a better implementation
            if (this.boxCollider == null)
            {
                this.boxCollider = GetComponent<BoxCollider2D>();
                if (this.boxCollider == null)
                    this.boxCollider = GetComponentInChildren<BoxCollider2D>();
            }

            if (this.damageable == null)
                damageable = GetComponent<DamageableEnemy>();

            Activated = false;
        }

        public void StartGrowth()
        {
            animator.SetTrigger("Activated");
            Activated = true;
        }

        void Update()
        {
            if (Activated && !Rotting)
            {
                if (Timer >= TimeToCrestGrow)
                {
                    damageableBossBulbatoe.health.CurHealth = damageableBossBulbatoe.StartingHealth;
                    animator.SetTrigger("CrestGrow");
                    animator.SetBool("Killable", true);
                    Rotting = true;
                }
                else
                {
                    Timer += Time.deltaTime;
                }
            }
            if (Rotting)
            {
                if (Timer >= TimeToRot)
                {
                    animator.SetBool("Rot", true);
                    Activated = false;
                    Rotting = false;
                }
                else
                {
                    Timer += Time.deltaTime;
                }
            }
        }

        public override bool CheckIsGrounded()
        {
            throw new NotImplementedException();
        }

        public override void UpdateFacing()
        {
            throw new NotImplementedException();
        }
    }
}
