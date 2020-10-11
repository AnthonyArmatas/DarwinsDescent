using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public class EnemySMF : StateMachineFlags
    {
        public int HorizontalSpeedHash => Animator.StringToHash("HorizontalSpeed");
        public int VerticalSpeedHash => Animator.StringToHash("VerticalSpeed");
        public int GroundedHash => Animator.StringToHash("Grounded");
        public int RespawnParaHash => Animator.StringToHash("Respawn");
        public int HurtHash => Animator.StringToHash("Hurt");
        public int DeadHash => Animator.StringToHash("Dead");

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
                default:
                    return 0;
            }
        }
    }
}
