using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour
{
    public class DamageablePlayer : Damageable
    {
        public PlayerCharacter playerCharacter;
        void Start()
        {
            if (playerCharacter == null)
                playerCharacter = GetComponent<PlayerCharacter>();
            actor = playerCharacter;

            startingHealth = playerCharacter.health.CurHealth;

            //OnHealthSet.Invoke(this);
            DisableInvulnerability();
        }

        public new void TakeDamage(Damager damager, bool ignoreInvincible = false)
        {
            if ((Invulnerable && !ignoreInvincible) || playerCharacter.healthDetailed.CurHealth <= 0)
                return;

            //we can reach that point if the damager was one that was ignoring invincible state.
            //We still want the callback that we were hit, but not the damage to be removed from health.
            if (!Invulnerable)
            {
                int dmgToPlayer = damager.damage;

                if(playerCharacter.healthDetailed.TempHp > 0)
                {
                    // Check to see if there is left over dmg for the real hp
                    if(damager.damage > playerCharacter.healthDetailed.TempHp)
                    {
                        dmgToPlayer -= playerCharacter.healthDetailed.TempHp;
                        playerCharacter.healthDetailed.TempHp = 0;
                    }
                    else
                    {
                        playerCharacter.healthDetailed.TempHp -= dmgToPlayer;
                        dmgToPlayer = 0;
                    }
                }
                playerCharacter.healthDetailed.RealHp -= damager.damage;
            }

            DamageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;

            // this should call OnHurt, do that instead of invoke
            //OnTakeDamage.Invoke(damager, this);

            if (playerCharacter.healthDetailed.CurHealth <= 0)
            {
                playerCharacter.animator.SetBool(actor.DeadParaHash, true);
            }
            playerCharacter.animator.SetTrigger(actor.HurtParaHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="RealHP">Bool determining whether it health gained is real or temp.
        /// Should change to enum if I decide to add another HP type.</param>
        public void GainHealth(int amount, bool RealHP)
        {
            if (RealHP)
            {

            }
            //actor.health.CurHealth += amount;

            //if (actor.health.CurHealth > startingHealth)
            //    actor.health.CurHealth = startingHealth;

            //OnHealthSet.Invoke(this);

            //OnGainHealth.Invoke(amount, this);
        }
    }
}
