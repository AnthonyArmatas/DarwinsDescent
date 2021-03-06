﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    /// <summary>
    /// PipModel for Pip Pad
    /// </summary>
    [System.Serializable]
    public class PipModel
    {
        public enum PartName
        {
            Head,
            Arms,
            Chest,
            Legs
        }

        public PartName Name;

        // Max amount of pips
        public int MaxCap;
        public int Allocated;
        public int CurAvalible { get
            {
                return MaxCap - Allocated;
            }
        }
        public bool Locked;

        public PipModel() 
        {

        }

        public PipModel(PartName partName, int maxCap, int allocated, bool locked)
        {
            Name = partName;
            MaxCap = maxCap;
            Allocated = allocated;
            Locked = locked;
        }

        public void ApplyPipModifications(PlayerCharacter PlayerCharacter)
        {
            switch (Name)
            {
                case PartName.Head:
                    break;
                case PartName.Arms:
                    PlayerCharacter.animator.SetBool("MeleeAttack_Upgraded", ArmPipUp.AttackUpgrade[this.Allocated]);
                    switch (this.Allocated)
                    {
                        case 0:
                            PlayerCharacter.CurrentAttackSound = PlayerCharacter.BaseAttackSound;
                            PlayerCharacter.CurrentWeaponSound = PlayerCharacter.BaseWeaponSound;
                            break;
                        case 1:
                            PlayerCharacter.CurrentAttackSound = PlayerCharacter.UpgradedAttackSound;
                            PlayerCharacter.CurrentWeaponSound = PlayerCharacter.UpgradedWeaponSound;
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                    }
                    break;
                case PartName.Chest:
                    break;
                case PartName.Legs:
                    PlayerCharacter.baseMovementSpeed = LegPipUp.MovementSpeed[this.Allocated];
                    PlayerCharacter.jumpForce = LegPipUp.JumpForce[Allocated];
                    break;
            }
        }
    }

    public static class LegPipUp
    {
        public static Dictionary<int, float> JumpForce = new Dictionary<int, float>()
        {
            {0, 1f },
            {1, 1.25f },
            {2, 1.5f },
            {3, 1.75f },
        };
        public static Dictionary<int, float> MovementSpeed = new Dictionary<int, float>()
        {
            {0, 2f },
            {1, 2.25f },
            {2, 2.5f },
            {3, 3f },
        };
    }

    public class ArmPipUp
    {
        public static Dictionary<int, bool> AttackUpgrade = new Dictionary<int, bool>()
        {
            {0, false },
            {1, true },
            {2, true },
            {3, true },
        };
    }
}
