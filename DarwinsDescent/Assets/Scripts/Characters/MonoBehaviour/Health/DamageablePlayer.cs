﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class DamageablePlayer : Damageable
    {
        // Entire point of this is to be able to watch the players health from Gui
        public int curHealth;
        private PlayerSMF playerSMF = new PlayerSMF();
        
        public PlayerHealth playerHealth { get; set; }
        public AudioSource TakeDamageSound;
        public AudioSource OnDeathSound;
        public AudioSource DarwinsBreathing;
        public AudioSource DarwinsHeartbeat;
        public AudioSource DarwinsHeal;
        public override Health health 
        { 
            get { return playerHealth; }
            set { playerHealth = (PlayerHealth)value; } 
        }

        #region Events
        // Instantiates the event for others to subscribe to with their functions.
        public delegate void UpdatePipHPGUI(PlayerHealth playerHP);
        public UpdatePipHPGUI UpdateHp;

        #endregion

        void Awake()
        {
            if (health == null)
                health = GetComponent<PlayerHealth>();
            health.InitializeHealth(StartingHealth);
            curHealth = StartingHealth;
            Invincible = false;
        }

        private void Update()
        {
            if(playerHealth.CurHealth == 1)
            {
                if(!DarwinsBreathing.isPlaying && !DarwinsHeartbeat.isPlaying)
                {
                    PlayDarwinNearDeath();
                }
            }

            if (playerHealth.CurHealth != 1)
            {
                if (DarwinsBreathing.isPlaying || DarwinsHeartbeat.isPlaying)
                {
                    StopDarwinNearDeath();
                }
            }
        }

        // TODO: Would it just be easier to keep track of the damaged health if 
        // calculated in the player health class instead of needing to do it every time 
        // the damaged health needs to be taken into account?
        // Could Probably redesign with Priority queue
        public override void GainHealth(int HealAmount)
        {
            // Look to fill damaged health first
            if (playerHealth.DamagedHealth > 0)
            {
                if (HealAmount <= playerHealth.DamagedHealth)
                {
                    playerHealth.RealHp += HealAmount;
                    UpdateHp.Invoke((PlayerHealth)health);
                    // Maybe call for an effect eventually
                    DarwinsHeal.Play();
                    return;
                }

                HealAmount -= playerHealth.DamagedHealth;
                playerHealth.RealHp += playerHealth.DamagedHealth;
            }
            
            // If lent is greater than 0 and temp is not at or above the cap (it can be above due to returning lent health while temp exsists)
            if(playerHealth.LentHp > 0 &&
                playerHealth.TempHp < playerHealth.LentHp)
            {
                playerHealth.TempHp += HealAmount;
                if (playerHealth.TempHp > playerHealth.LentHp)
                    playerHealth.TempHp = playerHealth.LentHp;
                DarwinsHeal.Play();
            }

            UpdateHp.Invoke((PlayerHealth)health);
            // Maybe call for an effect eventually
        }

        public override void SetHealth(int HPAmount)
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(int DamageAmount)
        {
            if (playerHealth.CurHealth <= 0 || Invincible)
                return;

            PlayerHealth beforeDamage = (PlayerHealth)health;
            //health.TakeDamage(DamageAmount);
            if (DamageAmount >= playerHealth.CurHealth)
            {
                StopDarwinNearDeath();
                //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg) // SetPipDisplay(amount,typeFrom, TypeTp)
                playerHealth.TempHp = 0;
                playerHealth.RealHp = 0;
                UpdateHp.Invoke((PlayerHealth)health);
                animator.SetBool(playerSMF.DeadHash, true);
                OnDeathSound.Play();
                return;
            }

            JustHit = true;
            if (DamageAmount <= playerHealth.TempHp)
            {
                playerHealth.TempHp -= DamageAmount;
                UpdateHp.Invoke((PlayerHealth)health);
                //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg)
                TakeDamageSound.Play();
                return;
            }
            DamageAmount -= playerHealth.TempHp;
            playerHealth.TempHp = 0;
            //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg)

            playerHealth.RealHp -= DamageAmount;
            animator.SetTrigger(playerSMF.HurtHash);

            curHealth = playerHealth.CurHealth;
            TakeDamageSound.Play();
            UpdateHp.Invoke(playerHealth);
            // Do we want to do dmg direction for enemies? I'm not sure we do
            //DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;
        }

        public void LoanHealth(int LoanAmount)
        {
            if (LoanAmount >= playerHealth.RealHp ||
                LoanAmount >= health.MaxHP)
                return;

            playerHealth.RealHp -= LoanAmount;
            playerHealth.LentHp += LoanAmount;

            curHealth = playerHealth.CurHealth;
            UpdateHp.Invoke((PlayerHealth)health);
        }

        public void GetBackLoanHealth(PipModel PipSection)
        {
            if (PipSection.Allocated <= 0)
                return;

            playerHealth.LentHp -= PipSection.Allocated;
            playerHealth.RealHp += PipSection.Allocated;
            PipSection.Allocated = 0;
        }

        public void PlayDarwinNearDeath()
        {
            DarwinsBreathing.Play();
            DarwinsHeartbeat.Play();
        }

        public void StopDarwinNearDeath()
        {
            DarwinsBreathing.Stop();
            DarwinsHeartbeat.Stop();
        }
    }
}