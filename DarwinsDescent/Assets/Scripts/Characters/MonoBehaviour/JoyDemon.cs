using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour
{
    class JoyDemon : Enemy
    {
        public bool TargetDirectionLeft = true;
        public float ForcePower = 5f;
        public bool CanLeap = false;
        public float TimeTillLeap = 1.5f;
        public float CurLeapTime = 0;
        public AudioSource JumpAttack;

        void FixedUpdate()
        {
            CheckIsGrounded();

            if (damageable?.health.CurHealth <= 0)
            {
                destinationSetter.target = null;
                return;
            }

            CheckPlayerInRange();

            //if(destinationSetter.target.name == "Point1")
            //{
            //    Console.WriteLine();
            //    var asdasd = this.gameObject.transform.position.x - aIPath.destination.x;
            //}

            LeapTowardsTarget();



            this.animator.SetFloat(SMF.HorizontalSpeedHash, this.rigidbody2D.velocity.x);
            this.animator.SetFloat(SMF.VerticalSpeedHash, this.rigidbody2D.velocity.y);
            UpdateFacing();
        }

        public void LeapTowardsTarget()
        {
            TargetDirectionLeft = this.spriteRenderer.transform.position.x - aIPath.destination.x >= 0 ? true : false;

            if (isGrounded)
            {
                CurLeapTime += Time.deltaTime;

                if (CurLeapTime >= TimeTillLeap)
                {
                    //pick the direction you want the ball to go - here I'm just pointing it up and to the right a bit
                    //you'll probably want to base it off the forwards vector of your cannon or something?
                    //I'm also normalizing it, as I want it of unit length for my future calculations 
                    //if it was already normalized you wouldn't need to do this!
                    //now we could accelerate the cannon ball in my direction in several ways

                    //option 1: apply an IMPULSE - this will accelerate the ball, but will go more slowly the heavier the ball is
                    //this assumes you have a variable called 'impulseSize' for how hard to push it
                    //this.rigidbody2D.AddForce(dir * ForcePower, ForceMode2D.Impulse);

                    if (TargetDirectionLeft)
                    {
                        Vector2 dir = new Vector2(.5f * -1, 1);
                        this.rigidbody2D.AddForce(dir * ForcePower, ForceMode2D.Impulse);
                    }
                    else
                    {
                        Vector2 dir = new Vector2(.5f, 1);
                        this.rigidbody2D.AddForce(dir * ForcePower, ForceMode2D.Impulse);
                    }
                    if (JumpAttack != null)
                    {
                        JumpAttack.Play();
                    }
                    



                    //option 2: apply an ACCELERATION - this will directly accelerate the ball by a fixed amount regardless of mass
                    //this assumes you have a variable called 'desiredSpeed' for how fast it should go
                    //this.rigidbody2D.AddForce(dir * ForcePower, ForceMode2D.Force);

                    //option3: directly set the velocity of the cannon ball. because it starts stationary, this is exactly
                    //the same as using the acceleration force mode
                    //cannonBall.rigidBody.velocity = dir * desiredSpeed;

                    //this.rigidbody2D.AddForce(new Vector2(ForcePower, ForcePower), ForceMode2D.Impulse);
                }

                return;            
            }

            CurLeapTime = 0;

        }

        public override void UpdateFacing()
        {
            if (this.rigidbody2D.velocity.x < 0)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
            }
            if (this.rigidbody2D.velocity.x > 0)
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
            }
        }
    }
}
