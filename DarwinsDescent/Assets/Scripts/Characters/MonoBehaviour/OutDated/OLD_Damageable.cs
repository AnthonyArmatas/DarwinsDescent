using System;
using UnityEngine;
using UnityEngine.Events;

namespace DarwinsDescent
{
    public class OLD_Damageable : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion

        #region CommentedOut
        //[Serializable]
        //public class HealthEvent : UnityEvent<Damageable>
        //{ }

        //[Serializable]
        //public class DamageEvent : UnityEvent<Damager, Damageable>
        //{ }

        //[Serializable]
        //public class HealEvent : UnityEvent<int, Damageable>
        //{ }
        //public HealthEvent OnHealthSet;
        //public DamageEvent OnTakeDamage;
        //public DamageEvent OnDie;
        //public HealEvent OnGainHealth;

        //public delegate void TakeDamage();
        //public event TakeDamage Damaged;

        //[HideInInspector]
        //public DataSettings dataSettings;

        #endregion


        
        public bool invulnerableAfterDamage = true;
        public float invulnerabilityDuration = 3f;
        public bool disableOnDeath = false;
        // Figure out if this actually works.
        [Tooltip("An offset from the object position used to set from where the distance to the damager is computed")]
        public Vector2 centreOffset = new Vector2(0f, 1f);
        public Actor actor;
        // Used to remember where it would be healed back to full
        public int startingHealth;

        protected bool Invulnerable;
        protected float InulnerabilityTimer;
        protected Vector2 DamageDirection;
        protected bool ResetHealthOnSceneReload;

        void Start()
        {
            if (actor == null)
                actor = GetComponent<Actor>();

            //startingHealth = actor.health.MaxHP;

            //OnHealthSet.Invoke(this);
            DisableInvulnerability();
        }


        void Update()
        {
            if (Invulnerable)
            {
                InulnerabilityTimer -= Time.deltaTime;

                if (InulnerabilityTimer <= 0f)
                {
                    Invulnerable = false;
                }
            }
        }

        public void EnableInvulnerability(bool ignoreTimer = false)
        {
            Invulnerable = true;
            //technically don't ignore timer, just set it to an insanely big number. Allow to avoid to add more test & special case.
            InulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
        }

        public void DisableInvulnerability()
        {
            Invulnerable = false;
        }

        public Vector2 GetDamageDirection()
        {
            return DamageDirection;
        }

        public void TakeDamage(Damager damager, bool ignoreInvincible = false)
        {
            //if ((Invulnerable && !ignoreInvincible) || actor.health.CurHealth <= 0)
            //    return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            if (!Invulnerable)
            {
                //actor.health.CurHealth -= damager.damage;
                //OnHealthSet.Invoke(this);
            }

            DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;

            // this should call OnHurt, do that instead of invoke
            //OnTakeDamage.Invoke(damager, this);

            //if (actor.health.CurHealth <= 0)
            //{
            //    actor.animator.SetBool(actor.DeadParaHash, true);
            //}
            //actor.animator.SetTrigger(actor.HurtParaHash);
        }

        public void GainHealth(int amount)
        {
            //actor.health.CurHealth += amount;

            //if (actor.health.CurHealth > startingHealth)
            //    actor.health.CurHealth = startingHealth;

            //OnHealthSet.Invoke(this);

            //OnGainHealth.Invoke(amount, this);
        }

        public void SetHealth(int amount)
        {
            //actor.health.CurHealth = amount;

            //if (actor.health.CurHealth <= 0)
            //{
            //    //OnDie.Invoke(null, this);
            //    ResetHealthOnSceneReload = true;
            //    EnableInvulnerability();
            //    if (disableOnDeath) gameObject.SetActive(false);
            //}

            ////OnHealthSet.Invoke(this);
        }
    }

}