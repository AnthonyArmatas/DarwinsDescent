using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class PlayerCharacter : Actor
    {
        #region CopyPasteRegion
        #endregion

        #region Vars
        protected Vector2 moveVelocity;
        public Transform cameraFollowTarget;
        public PlayerSMF SMF = new PlayerSMF();

        private float jumpTimeCounter;
        public bool jumpRequest;
        public bool jumping;
        public float jumpTime;
        public float jumpForce;
        public bool IsDead;
        #endregion

        //public PlayerCharacter(Damageable dmg, Animator ani, SpriteRenderer sr, Rigidbody2D rb, BoxCollider2D bc, float baseMovementSpeed )
        //    :base(dmg, ani, sr, rb, bc, baseMovementSpeed)
        //{
        //}

        // According to this https://forum.unity.com/threads/onenable-before-awake.361429/ awake, onenable, start, ect are by script
        // and do not have call all of the scripts awake, then OnEnable, ect.
        // According to this https://answers.unity.com/questions/13304/how-to-set-starting-order-of-scripts.html
        // and actual attempts the way to set script execution is from within the unity gui
        // Go To Edit -> Prokect Settings -> Script Execution Order and add scripts there in the order they should
        // be run.
        void Awake()
        {
            //healthDetailed = new HealthPipModel(startingHealth, 1);
            //health = healthDetailed;
            //healthDetailed.RealHp = 3;
            //healthDetailed.TempHp = 7;
            if(rigidbody2D == null)
                rigidbody2D = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();
            if (boxCollider == null)
                boxCollider = GetComponent<BoxCollider2D>();
            if (damageable == null)
                damageable = GetComponent<DamageablePlayer>();
            if (cameraFollowTarget == null)
                cameraFollowTarget = transform.Find("CameraFollowTarget")?.GetComponent<Transform>();


            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (jumpForce == 0)
                jumpForce = 2;
            if (jumpTime == 0)
                jumpTime = 0.05f;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsDead)
                return;
            IsJumping();
            IsAttacking();
            UpdateFacing();
        }

        void FixedUpdate()
        {
            // Should add a call to the gui to get respawn functionality going or something.
            if (animator.GetBool(SMF.DeadHash))
            {
                IsDead = true;
                return;
            }

            MoveAround();
            animator.SetFloat(SMF.HorizontalSpeedHash, this.rigidbody2D.velocity.x);
            animator.SetFloat(SMF.VerticalSpeedHash, this.rigidbody2D.velocity.y);

            // Maybe Add to both Updates
            CheckIsGrounded();
        }

        #region PlayerMovement
        /// <summary>
        /// Controls base movement.
        /// </summary>
        /// <param name="speedScale"></param>
        public void MoveAround(float speedScale = 1f)
        {
            
            HorizontalMovement();
            VerticalMovement();
        }

        public void HorizontalMovement(float speedScale = 1f)
        {
            if (PlayerInput.Instance.Horizontal.Value == 0)
            {
                // Immediately stops the player if he is on the floor and moving faster than small nudges. If more precision is desired remove the Mathf.Abs(this.rigidbody2D.velocity.x) > 2f
                if (this.rigidbody2D.velocity.x != 0 && isGrounded && Mathf.Abs(this.rigidbody2D.velocity.x) > 2f)
                {
                    this.rigidbody2D.velocity = new Vector2(0, this.rigidbody2D.velocity.y);
                }

                return;
            }

            //Vector2 targetVelocity = new Vector2(this.baseMovementSpeed * 10f, this.rigidbody2D.velocity.y);
            //// And then smoothing it out and applying it to the character
            //Vector2 smoothedSpeed = Vector2.SmoothDamp(this.rigidbody2D.velocity, PlayerInput.Instance.Horizontal.Value * targetVelocity, ref moveVelocity, .05f, this.baseMovementSpeed);
            //if (Mathf.Abs(smoothedSpeed.x) > this.baseMovementSpeed)
            //{
            //    if(smoothedSpeed.x > 0)
            //        smoothedSpeed.x = this.baseMovementSpeed;
            //    else
            //        smoothedSpeed.x = this.baseMovementSpeed * -1;
            //}
            //this.rigidbody2D.velocity = smoothedSpeed;



            this.rigidbody2D.velocity = new Vector2(this.baseMovementSpeed * PlayerInput.Instance.Horizontal.Value, this.rigidbody2D.velocity.y);
        }

        public void VerticalMovement()
        {
            if (jumpRequest)
            {
                this.rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
                jumping = true;
            }
            else if (PlayerInput.Instance.Jump.Held && jumping == true)
            {
                if (jumpTimeCounter > 0 && this.rigidbody2D.velocity.y > 0)
                {
                    this.rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else if (jumpTimeCounter <= 0 || this.rigidbody2D.velocity.y <= 0)
                {
                    jumping = false;
                }
            }

            if (PlayerInput.Instance.Jump.Up)
            {
                jumping = false;
            }


            // QoL Addition: Adds a small pause to the peak of a jump which should give the player a moment to adjust their landing.
            // Mathf.Round is like MidpointRounding.ToEven,if MidpointRounding.ToZero is desired use Math
            if (Mathf.Round(this.rigidbody2D.velocity.y) == 0 && this.isGrounded == false)
            {
                this.rigidbody2D.gravityScale = .5f;
            }
            else if (this.rigidbody2D.velocity.y < 0)
            {
                this.rigidbody2D.gravityScale = 3f;
            }


            if (this.isGrounded && this.rigidbody2D.gravityScale != 1f)
            {
                this.rigidbody2D.gravityScale = 1f;
            }
        }

        /// <summary>
        /// Jump mechanics
        /// </summary>
        public void IsJumping()
        {
            if (isGrounded && PlayerInput.Instance.Jump.Down)
            {
                jumpRequest = true;
                jumpTimeCounter = jumpTime;
            }
        }
        #endregion

        public void IsAttacking()
        {
            if (PlayerInput.Instance.MeleeAttack.Down)
            {
                // attacking = true;

                // Setting this true here so that the damager can check if the trigger collision happens while MeleeAttack is active, and when the animation state exits, it sets it to false.
                // This allows every frame of the animation to be register the hits. A different method should be used for specific frames of animation that the damage should be dealt (Or different Dmg).
                animator.SetBool(SMF.MeleeAttackHash, true);
                // animator.SetTrigger(MeleeAttackParaHash) also worked
            }
        }

        #region Facing
        public override void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.transform.localScale = new Vector3(-1, 1);
                //spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                //meleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
            else if (faceRight)
            {
                spriteRenderer.transform.localScale = new Vector3(1, 1);    

                //spriteRenderer.flipX = spriteOriginallyFacesLeft;
                //meleeAtkBCollider.transform.localScale = new Vector3(1, 1);
            }
        }
        #endregion

        #region Grounded
        public override bool CheckIsGrounded()
        {
            bool wasGrounded = animator.GetBool(SMF.GroundedHash);
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedRaycastDistanceCheck, groundedLayerMask);
            isGrounded = raycastHit.collider != null;
            animator.SetBool(SMF.GroundedHash, isGrounded);

            #region Uncomment to see visual Debug
            // Uncomment to see visual Debug
            //Color rayColor;
            //if (raycastHit.collider != null)
            //{
            //    rayColor = Color.green;
            //}
            //else
            //{
            //    rayColor = Color.red;
            //}

            //Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
            //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
            //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), Vector2.right * (boxCollider.bounds.extents.y), rayColor);
            //Debug.Log(raycastHit.collider);
            #endregion

            return isGrounded;
        }
        #endregion
    }
}