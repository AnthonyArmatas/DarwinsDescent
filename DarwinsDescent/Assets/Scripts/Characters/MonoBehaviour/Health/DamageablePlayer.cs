using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarwinsDescent
{
    public class DamageablePlayer : Damageable
    {
        // Entire point of this is to be able to watch the players health from Gui
        public int curHealth;
        private PlayerSMF playerSMF = new PlayerSMF();
        
        public PlayerHealth playerHealth { get; set; }
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
            // TODO: Implement invincibility
            //if ((Invulnerable && !ignoreInvincible) || health.CurHealth <= 0)
            //    return;
            if (playerHealth.CurHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            //if (!Invulnerable)
            //{
            //    actor.health.CurHealth -= damager.damage;
            //    // Maybe Turn this OnHealth Set to an event that resets any temp timer
            //    // It makes sense to call it here since this is where it absolutely takes dmg away from
            //    // an enemy and since its an event its still loosely coupled.
            //    OnHealthSet.Invoke(this);
            //}

            PlayerHealth beforeDamage = (PlayerHealth)health;
            //health.TakeDamage(DamageAmount);
            if (DamageAmount >= playerHealth.CurHealth)
            {
                //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg) // SetPipDisplay(amount,typeFrom, TypeTp)
                playerHealth.TempHp = 0;
                playerHealth.RealHp = 0;
                UpdateHp.Invoke((PlayerHealth)health);
                animator.SetBool(playerSMF.DeadHash, true);
                return;
            }

            if (DamageAmount <= playerHealth.TempHp)
            {
                playerHealth.TempHp -= DamageAmount;
                UpdateHp.Invoke((PlayerHealth)health);
                //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg)
                return;
            }
            DamageAmount -= playerHealth.TempHp;
            playerHealth.TempHp = 0;
            //SetPipDisplay(playerHealth.TempHp,State.temp, state.Dmg)

            playerHealth.RealHp -= DamageAmount;
            animator.SetTrigger(playerSMF.HurtHash);

            curHealth = playerHealth.CurHealth;
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
    }
}