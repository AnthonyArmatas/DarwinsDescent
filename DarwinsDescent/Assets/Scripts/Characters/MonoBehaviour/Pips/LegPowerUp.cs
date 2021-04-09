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
            this.UsedUp = true;
            playerCharacter.interact -= TriggerEvent;
            HideInteractObj();
            if (PipSystem.Legs.Locked)
                PipSystem.Legs.Locked = false;
            PipSystem.Legs.MaxCap++;
            PipSystem.UpdatePipPad(PipSystem.Legs);
            Animator.SetTrigger(ActivateHash);
            PowerUpSound.Play();
            PowerUpDarwinSound.PlayDelayed(PowerUpSound.clip.length);
        }
    }
}
