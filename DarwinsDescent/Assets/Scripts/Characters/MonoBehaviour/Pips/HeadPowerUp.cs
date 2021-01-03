using UnityEngine;

namespace DarwinsDescent
{
    public class HeadPowerUp : PowerUp
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
            if (PipSystem.Head.Locked)
                PipSystem.Head.Locked = false;
            PipSystem.Head.MaxCap++;
            PipSystem.UpdatePipPad(PipSystem.Head);
            Animator.SetTrigger(ActivateHash);
        }
    }
}
