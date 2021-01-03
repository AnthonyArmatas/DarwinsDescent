using UnityEngine;

namespace DarwinsDescent
{
    public class ChestPowerUp : PowerUp
    {
        void Awake()
        {
            UsedUp = false;
            if (Animator == null)
                Animator = GetComponent<Animator>();
        }

        public override void TriggerEvent()
        {
            this.UsedUp = true;
            playerCharacter.interact -= TriggerEvent;
            HideInteractObj();
            if (PipSystem.Chest.Locked)
                PipSystem.Chest.Locked = false;
            PipSystem.Chest.MaxCap++;
            PipSystem.UpdatePipPad(PipSystem.Chest);
            Animator.SetTrigger(ActivateHash);
        }
    }
}
