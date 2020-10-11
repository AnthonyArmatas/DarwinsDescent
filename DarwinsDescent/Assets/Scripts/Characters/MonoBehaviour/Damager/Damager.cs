using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent
{
    public abstract class Damager : MonoBehaviour
    {
        public int damage;
        public bool ignoreInvincibility = false;
        public LayerMask hittableLayers;
        public Animator animator;
        public BoxCollider2D AttackCollider;
    }
}
