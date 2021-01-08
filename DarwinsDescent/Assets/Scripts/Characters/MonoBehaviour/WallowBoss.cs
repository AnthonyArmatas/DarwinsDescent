using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class WallowBoss : Enemy
    {
        ////
        ///Write up about the WallowBoss fight.
        ///Darwin comes in the camera shifts and he is immobilized.
        ///The wallow demon in the center of the screen shakes
        ///.
        ///.                      ~~~~~~~~~~~~~~~~            ||
        ///.                     /                \           ||
        ///.                    /  [0]        [0]  \          ||
        ///.    0              /                    \         ||
        ///.    |             /        ^~!!!~^       \        ||
        ///.__________________________________________________||
        /// It wakes it an animation and noise where is opens its
        /// eyes mouth and shoots out its tentacles and then goes 
        /// back dormant. The fight will be one phase where 
        /// bulbatoes will grow from the ground rapidly and once at full
        /// size a crest will grow and unfurl. While it is unfurling it 
        /// will be attackable. If attacked it will do dmg to the boss 
        /// (Causing it to flash to indicate dmg). If left untouched it
        /// will change color every second turning from a bright purple
        /// to a pale green like the boss. This should take from 1.5 to 3 seconds
        /// Once its time cap is met it turn to a small version of the 
        /// wallow demon dormant which will pop out 1-2 joy demons.
        /// Every 10-15 seconds the wallow demons dormant form will shake twice as 
        /// a telegraph. After the second shake its eyes will open and its tentacles
        /// will shoot out Darwin will take 1 dmg if hit by the tentacles or the mouth.
        /// Play test to see if darwin should take dmg from the eyes or if the eyes should
        /// be vulnerable for darwin to hit.
        /// I think it should have about 10hp. So 1 hp per bulbatoe and potential eye hit.

        public float TimeToAttack;
        public float AttackTime;
        public float TelegraphTime;
        public bool HasTelegraphed;
        public int BossHealth = 10;

        // Start is called before the first frame update
        void Start()
        {
            TimeToAttack = 0;

            if (TelegraphTime == 0)
                TelegraphTime = 5f;
            if (AttackTime == 0)
                AttackTime = 10f;
            HasTelegraphed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (animator.GetBool("Dead"))
                return;

            if (animator.GetBool("InFight"))
            {
                TimeToAttack += Time.deltaTime;
                if(TimeToAttack >= TelegraphTime &&
                    HasTelegraphed == false)
                {
                    animator.SetBool("Prepare_Attack", true);
                    HasTelegraphed = true;
                }

                if (TimeToAttack >= AttackTime)
                {
                    animator.SetBool("Attack", true);
                    HasTelegraphed = false;
                    TimeToAttack = 0;
                    animator.SetBool("Prepare_Attack", false);
                }
            }
        }

        void FixedUpdate()
        {

        }
    }
}
