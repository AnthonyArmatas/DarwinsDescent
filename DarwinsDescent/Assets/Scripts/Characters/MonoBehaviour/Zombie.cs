using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour
{
    class Zombie : Enemy
    {
        public BoxCollider2D meleeAtkBCollider;

        //public void Start()
        //{
        //}

        public void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
            else
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(1, 1);
            }
        }
    }
}
