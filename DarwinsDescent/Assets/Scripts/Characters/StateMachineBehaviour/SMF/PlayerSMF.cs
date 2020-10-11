using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class PlayerSMF : StateMachineFlags
    {
        public int HorizontalSpeedHash => Animator.StringToHash("HorizontalSpeed");
        public int VerticalSpeedHash => Animator.StringToHash("VerticalSpeed");
        public int GroundedHash => Animator.StringToHash("Grounded");
        public int RespawnParaHash => Animator.StringToHash("Respawn");
        public int HurtHash => Animator.StringToHash("Hurt");
        public int DeadHash => Animator.StringToHash("Dead");
        public int CrouchingHash => Animator.StringToHash("Crouching");
        public int MeleeAttackHash => Animator.StringToHash("MeleeAttack");

        // Only Needed if we dont want to cast StateMachineFlags as PlayerSMF to access its properties
        // May just delete, because upkeep seems tedious when we can just cast. ((PlayerSMF)SMF).MeleeAttackHash
        public int GetExtendedHash(string HashName)
        {
            switch (HashName)
            {
                case "HorizontalSpeed":
                    return HorizontalSpeedHash;
                case "VerticalSpeed":
                    return VerticalSpeedHash;
                case "Grounded":
                    return GroundedHash;
                case "Respawn":
                    return RespawnParaHash;
                case "Hurt":
                    return HurtHash;
                case "Dead":
                    return DeadHash;
                case "Crouching":
                    return CrouchingHash;
                case "MeleeAttack":
                    return MeleeAttackHash;
                default:
                    return 0;
            }
        }
    }
}
