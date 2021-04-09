using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class PowerUp : Interactable
    {
        public bool UsedUp;
        
        //Currently just adding in the gui for expediency of development.
        public PipSystem PipSystem;

        public Animator Animator;
        public AudioSource PowerUpDarwinSound;
        public AudioSource PowerUpSound;

        public int ActivateHash => Animator.StringToHash("Activate");

        void Start()
        {
            UsedUp = false;
            // TODO: Doesnt work, figure out why
            //if (playerCharacter == null)
            //{
            //    playerCharacter = transform.Find("Darwin")?.GetComponent<PlayerCharacter>();
            //}
            //if (pipSystem == null) { ... }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!UsedUp)
            {
                ShowInteractObj();
                playerCharacter.interact += TriggerEvent;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            HideInteractObj();
            playerCharacter.interact -= TriggerEvent;
        }

        public virtual void TriggerEvent()
        {
            Debug.Log("EventTriggered");
        }

        ////
        ///Create an event in the player character script called interact
        ///When a powerup(anyinteractable) has the player character enter
        ///the on trigger, have that script subscribe to the interactable
        ///function. When interact is hit, it will call invoke and that should
        ///call all of the subscribed functions.

    }
}
