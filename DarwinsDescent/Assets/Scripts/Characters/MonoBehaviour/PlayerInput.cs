using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class PlayerInput : InputComponent
    {
        public static PlayerInput Instance
        {
            get { return instance; }
        }

        protected static PlayerInput instance;


        public bool HaveControl { get { return haveControl; } }

        /// <summary>
        /// BIG NOTE WARNING!!! THESE CAN BE OVERWRITTEN IN THE GUI! I KNOW THAT IS OBVIOUS BUT I HAVE FORGOTTEN THAT LIKE THREE TIMES NOW SO IF THERE IS AN ISSUE WITH CONFIG LOOK THERE FIRST
        /// </summary>
        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu);
        public InputButton Interact = new InputButton(KeyCode.E, XboxControllerButtons.Y);
        public InputButton MeleeAttack = new InputButton(KeyCode.K, XboxControllerButtons.X);
        public InputButton RangedAttack = new InputButton(KeyCode.O, XboxControllerButtons.B);
        public InputButton Jump = new InputButton(KeyCode.Space, XboxControllerButtons.A);
        //public InputButton DpadUp = new InputButton(KeyCode.Keypad8, XboxControllerButtons.DPadUp);
        //public InputButton DPadDown = new InputButton(KeyCode.Keypad2, XboxControllerButtons.DPadDown);
        //public InputButton DPadLeft = new InputButton(KeyCode.Keypad4, XboxControllerButtons.DPadLeft);
        //public InputButton DPadRight = new InputButton(KeyCode.Keypad6, XboxControllerButtons.DPadRight);

        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizontal);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical);
        public InputAxis DPadHorizontal = new InputAxis(KeyCode.Keypad6, KeyCode.Keypad4, XboxControllerAxes.DpadHorizontal);
        public InputAxis DPadVertical = new InputAxis(KeyCode.Keypad8, KeyCode.Keypad2, XboxControllerAxes.DpadVertical);

        public InputAxis RefundPip = new InputAxis(KeyCode.KeypadPlus, KeyCode.KeypadMinus, XboxControllerAxes.LeftTrigger);

        protected bool haveControl = true;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + instance.name + " and " + name + ".");
        }

        void OnEnable()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + instance.name + " and " + name + ".");
        }

        void OnDisable()
        {
            instance = null;
        }

        protected override void GetInputs(bool fixedUpdateHappened)
        {
            // fixedUpdateHappened allows things like check when it is down or up
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            MeleeAttack.Get(fixedUpdateHappened, inputType);
            RangedAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);
            //DpadUp.Get(fixedUpdateHappened, inputType);
            //DPadDown.Get(fixedUpdateHappened, inputType);
            //DPadLeft.Get(fixedUpdateHappened, inputType);
            //DPadRight.Get(fixedUpdateHappened, inputType);
            Horizontal.Get(inputType);
            Vertical.Get(inputType);
            RefundPip.Get(inputType);
            DPadHorizontal.Get(inputType);
            DPadVertical.Get(inputType);
        }


        public override void GainControl()
        {
            haveControl = true;

            GainControl(Pause);
            GainControl(Interact);
            GainControl(MeleeAttack);
            GainControl(RangedAttack);
            GainControl(Jump);
            GainControl(Horizontal);
            GainControl(Vertical);
            GainControl(DPadHorizontal);
            GainControl(DPadVertical);
            //GainControl(DpadUp);
            //GainControl(DPadDown);
            //GainControl(DPadLeft);
            //GainControl(DPadRight);
            GainControl(RefundPip);
        }

        public override void ReleaseControl(bool resetValues = true)
        {
            haveControl = false;

            ReleaseControl(Pause, resetValues);
            ReleaseControl(Interact, resetValues);
            ReleaseControl(MeleeAttack, resetValues);
            ReleaseControl(RangedAttack, resetValues);
            ReleaseControl(Jump, resetValues);
            ReleaseControl(Horizontal, resetValues);
            ReleaseControl(Vertical, resetValues);
            ReleaseControl(DPadHorizontal, resetValues);
            ReleaseControl(DPadVertical, resetValues);
            //ReleaseControl(DpadUp, resetValues);
            //ReleaseControl(DPadDown, resetValues);
            //ReleaseControl(DPadLeft, resetValues);
            //ReleaseControl(DPadRight, resetValues);
            ReleaseControl(RefundPip, resetValues);
        }
    }
}