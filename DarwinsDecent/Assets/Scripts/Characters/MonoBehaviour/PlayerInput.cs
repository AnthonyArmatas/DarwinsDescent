using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDecent
{
    public class PlayerInput : InputComponent
    {
        public static PlayerInput Instance
        {
            get { return instance; }
        }

        protected static PlayerInput instance;


        public bool HaveControl { get { return haveControl; } }

        public InputButton Pause = new InputButton(KeyCode.Escape, XboxControllerButtons.Menu);
        public InputButton Interact = new InputButton(KeyCode.E, XboxControllerButtons.Y);
        public InputButton MeleeAttack = new InputButton(KeyCode.K, XboxControllerButtons.X);
        public InputButton RangedAttack = new InputButton(KeyCode.O, XboxControllerButtons.B);
        public InputButton Jump = new InputButton(KeyCode.Space, XboxControllerButtons.A);
        public InputAxis Horizontal = new InputAxis(KeyCode.D, KeyCode.A, XboxControllerAxes.LeftstickHorizontal);
        public InputAxis Vertical = new InputAxis(KeyCode.W, KeyCode.S, XboxControllerAxes.LeftstickVertical);

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
            Pause.Get(fixedUpdateHappened, inputType);
            Interact.Get(fixedUpdateHappened, inputType);
            MeleeAttack.Get(fixedUpdateHappened, inputType);
            RangedAttack.Get(fixedUpdateHappened, inputType);
            Jump.Get(fixedUpdateHappened, inputType);
            Horizontal.Get(inputType);
            Vertical.Get(inputType);
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
        }
    }
}