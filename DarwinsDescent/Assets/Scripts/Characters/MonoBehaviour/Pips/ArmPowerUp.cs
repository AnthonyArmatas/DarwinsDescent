using UnityEngine;

namespace DarwinsDescent
{
    public class ArmPowerUp : PowerUp
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
            if (PipSystem.Arms.Locked)
                PipSystem.Arms.Locked = false;
            PipSystem.Arms.MaxCap++;
            PipSystem.UpdatePipPad(PipSystem.Arms);
            Animator.SetTrigger(ActivateHash);
            PowerUpSound.Play();
            PowerUpDarwinSound.PlayDelayed(PowerUpSound.clip.length);
        }
    }
}
