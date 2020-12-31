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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ShowInteractObj();
            playerCharacter.interact += TriggerEvent;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            HideInteractObj();
            playerCharacter.interact -= TriggerEvent;
        }

        public void TriggerEvent()
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
