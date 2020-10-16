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
    }
}
