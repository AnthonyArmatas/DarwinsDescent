using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class LegPowerUp : PowerUp
    {
        void Awake()
        {
            UsedUp = false;
            if (Animator == null)
                Animator = GetComponent<Animator>();
        }

        public override void TriggerEvent()
        {
            //playerCharacter.
            this.UsedUp = true;
            playerCharacter.interact -= TriggerEvent;
            HideInteractObj();
            if (PipSystem.Legs.Locked)
                PipSystem.Legs.Locked = false;
            PipSystem.Legs.MaxCap++;
            PipSystem.UpdatePipPad(PipSystem.Legs);
            Animator.SetTrigger(ActivateHash);
        }
    }
}
