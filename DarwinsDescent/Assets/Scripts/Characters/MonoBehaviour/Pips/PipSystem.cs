using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour;
using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarwinsDescent
{
    /// <summary>
    /// Manages the logic behind the pipsystem.
    /// </summary>
    [RequireComponent(typeof(Actor))]
    [RequireComponent(typeof(Damageable))]

    public class PipSystem : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion
        public DamageablePlayer Damageable;


        // public int PipPool; is Actor.health

        //Top
        public PipModel Head = new PipModel();
        //Left
        public PipModel Chest = new PipModel();
        //Right
        public PipModel Arms = new PipModel();
        //Bottom
        public PipModel Legs = new PipModel();

        public PlayerCharacter PlayerCharacter;

        protected Color Disabled = new Color(127f, 127f, 127f);
        protected Color Enabled = new Color(191f, 191f, 0f);
        private int PipPoolCap;
        private int MinimumRequiredPipsInPool = 1;

        // Used as a work around since the dpad cannot be used as buttons in unity, only as an axis, and by extension you cannot get button up, down , or held. 
        private bool DpadHorWasDown = false;
        private bool DpadVertWasDown = false;
        private float DpadPrevHorVal = 0f;
        private float DpadPrevVertVal = 0f;

        #region Events
        public delegate void InitializePipParts(PipModel Head, PipModel Arms, PipModel Chest, PipModel Legs);
        public InitializePipParts Initialized;

        // Sets up the delegate so that the subscriber knows what it function needs to contain
        public delegate void UpdatePipPoolDisplay();
        // The Event publish. This is what the reviving methods subscribe to. So when update is invoked those other methods will run.
        public event UpdatePipPoolDisplay DisplayUpdated;

        // Sets up the delegate so that the subscriber knows what it function needs to contain
        public delegate void UpdatePipCount(PipModel PipSection);
        // The Event publish. This is what the reviving methods subscribe to. So when update is invoked those other methods will run.
        public event UpdatePipCount Updated;

        #endregion




        // Start is called before the first frame update
        void Awake()
        {
            Damageable = GetComponent<DamageablePlayer>();
            PlayerCharacter = GetComponent<PlayerCharacter>();
        }
        void Start()
        {
            // Initializing PipPoolCap in start because healthdetailed is initialized in awake and that needs to be set up first
            PipPoolCap = PlayerCharacter.healthDetailed.MaxHP;

            // Call function which sets the default/saved values for each of the pip models
            Initialized.Invoke(Head, Arms, Chest, Legs);
            if (Updated != null)
            {
                Updated.Invoke(Head);
                Updated.Invoke(Arms);
                Updated.Invoke(Chest);
                Updated.Invoke(Legs);
            }
        }

        void Update()
        {
            //UpdateTempPipTime();
        }

        void FixedUpdate()
        {
            AssignPips();
            //PlayerInput.Instance.Horizontal.Value
        }

        public void AssignPips()
        {
            if (PipPoolCap == MinimumRequiredPipsInPool)
                return;

            if (DpadVertWasDown == true &&
                PlayerInput.Instance.DPadVertical.ReceivingInput == false)
            {
                if (DpadPrevVertVal > 0)
                {
                    MovePips(Head);
                }
                if (DpadPrevVertVal < 0)
                {
                    MovePips(Legs);
                }
            }

            if (DpadHorWasDown == true && 
                PlayerInput.Instance.DPadHorizontal.ReceivingInput == false)
            {
                if (DpadPrevHorVal > 0)
                {
                    MovePips(Arms);
                }
                if (DpadPrevHorVal < 0)
                {
                    MovePips(Chest);
                }
            }

            DpadVertWasDown = PlayerInput.Instance.DPadVertical.ReceivingInput;
            DpadHorWasDown = PlayerInput.Instance.DPadHorizontal.ReceivingInput;
            DpadPrevVertVal = PlayerInput.Instance.DPadVertical.Value;
            DpadPrevHorVal = PlayerInput.Instance.DPadHorizontal.Value;

        }

        public void MovePips(PipModel PipSection)
        {
            // if there are not enough pips to give or the pip in question is locked or the max cap has been reached, return.
            if(PipSection.MaxCap <= 1  || 
                PipSection.Locked)
            {
                return;
            }

            // If the left trigger is held then return all pips
            if(PlayerInput.Instance.RefundPip.Value != 0)
            {
                // TODO: Call Damageable to refund health, filling up slots 
                // and using the rest as temp.
                PipSection.Allocated = 0;
                Updated.Invoke(PipSection);
                return;
            }

            if(PlayerCharacter.healthDetailed.CurHealth > PlayerCharacter.healthDetailed.MinHp &&
                PipSection.Allocated != PipSection.MaxCap)
            {
                // Call Pip Display to remove a pip and replace it with an empty one
                // Call the Damagable script lose a perm health
                PipSection.Allocated++;
                if (Updated != null)
                    Updated.Invoke(PipSection);

            }
        }
    }
}