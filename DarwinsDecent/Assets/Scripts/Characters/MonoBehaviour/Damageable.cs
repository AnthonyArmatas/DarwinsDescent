using System;
using UnityEngine;
using UnityEngine.Events;

namespace DarwinsDecent
{
    public class Damageable : MonoBehaviour
    {
        [Serializable]
        public class HealthEvent : UnityEvent<Damageable>
        { }

        [Serializable]
        public class DamageEvent : UnityEvent<Damager, Damageable>
        { }

        [Serializable]
        public class HealEvent : UnityEvent<int, Damageable>
        { }

        public int startingHealth = 5;
        public bool invulnerableAfterDamage = true;
        public float invulnerabilityDuration = 3f;
        public bool disableOnDeath = false;
        [Tooltip("An offset from the object position used to set from where the distance to the damager is computed")]
        public Vector2 centreOffset = new Vector2(0f, 1f);
        public HealthEvent OnHealthSet;
        public DamageEvent OnTakeDamage;
        public DamageEvent OnDie;
        public HealEvent OnGainHealth;

        //public delegate void TakeDamage();
        //public event TakeDamage Damaged;

        [HideInInspector]
        //public DataSettings dataSettings;

        protected bool Invulnerable;
        protected float InulnerabilityTimer;
        protected int CurHealth;
        protected Vector2 DamageDirection;
        protected bool ResetHealthOnSceneReload;

        public int CurrentHealth
        {
            get { return CurHealth; }
        }

        void OnEnable()
        {
            //PersistentDataManager.RegisterPersister(this);
            CurHealth = startingHealth;

            OnHealthSet.Invoke(this);

            DisableInvulnerability();
        }

        void OnDisable()
        {
            //PersistentDataManager.UnregisterPersister(this);
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
            if ((Invulnerable && !ignoreInvincible) || CurHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            if (!Invulnerable)
            {
                CurHealth -= damager.damage;
                //OnHealthSet.Invoke(this);
            }

            DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;

            // this should call OnHurt, do that instead of invoke
            OnTakeDamage.Invoke(damager, this);

            if (CurHealth <= 0)
            {
                // this should call on die. See if the game object can get the owner of the scripts game object to make that call or if the gui is needed and then some form of invoke is appropriate.
                OnDie.Invoke(damager, this);
                ResetHealthOnSceneReload = true;
                EnableInvulnerability();
                if (disableOnDeath) gameObject.SetActive(false);
            }
        }

        public void GainHealth(int amount)
        {
            CurHealth += amount;

            if (CurHealth > startingHealth)
                CurHealth = startingHealth;

            OnHealthSet.Invoke(this);

            OnGainHealth.Invoke(amount, this);
        }

        public void SetHealth(int amount)
        {
            CurHealth = amount;

            if (CurHealth <= 0)
            {
                OnDie.Invoke(null, this);
                ResetHealthOnSceneReload = true;
                EnableInvulnerability();
                if (disableOnDeath) gameObject.SetActive(false);
            }

            OnHealthSet.Invoke(this);
        }

        //public DataSettings GetDataSettings()
        //{
        //    return dataSettings;
        //}

        //public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        //{
        //    dataSettings.dataTag = dataTag;
        //    dataSettings.persistenceType = persistenceType;
        //}

        //public Data SaveData()
        //{
        //    return new Data<int, bool>(CurrentHealth, ResetHealthOnSceneReload);
        //}

        //public void LoadData(Data data)
        //{
        //    Data<int, bool> healthData = (Data<int, bool>)data;
        //    CurHealth = healthData.value1 ? startingHealth : healthData.value0;
        //    OnHealthSet.Invoke(this);
        //}
    }

}