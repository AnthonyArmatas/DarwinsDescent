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
        private float jumpTimeCounter;
        protected BoxCollider2D meleeAtkBCollider;
        protected Vector2 moveVelocity;

        public Transform cameraFollowTarget;
        public bool jumpRequest;
        public bool jumping;
        public float jumpTime;
        public float jumpForce;
        public HealthPipModel healthDetailed;
        #endregion

        void Awake()
        {
            healthDetailed = new HealthPipModel(startingHealth, 1);
            health = healthDetailed;
            healthDetailed.RealHp = 3;
            healthDetailed.TempHp = 7;
            this.Rigidbody2D = GetComponent<Rigidbody2D>();
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.animator = GetComponent<Animator>();
            this.boxCollider = GetComponent<BoxCollider2D>();
            meleeAtkBCollider = transform.Find("MeleeHitBox")?.GetComponent<BoxCollider2D>();
            cameraFollowTarget = transform.Find("CameraFollowTarget")?.GetComponent<Transform>();

            this.contactFilter = new ContactFilter2D()
            {
                layerMask = groundedLayerMask,
                useLayerMask = true,
                useTriggers = false,
            };

            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (baseAttackDamage == 0)
                baseAttackDamage = 1;
            if (jumpForce == 0)
                jumpForce = 2;
            if (jumpTime == 0)
                jumpTime = 0.05f;
        }

        // Update is called once per frame
        void Update()
        {
            IsJumping();
            IsAttacking();
            UpdateFacing();
        }

        void FixedUpdate()
        {
            //characterController2D.Move(moveVector * Time.deltaTime);
            MoveAround();
            animator.SetFloat(HorizontalSpeedParaHash, this.Rigidbody2D.velocity.x);
            animator.SetFloat(VerticalSpeedParaHash, this.Rigidbody2D.velocity.y);

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
                // Immediately stops the player if he is on the floor and moving faster than small nudges. If more precision is desired remove the Mathf.Abs(this.Rigidbody2D.velocity.x) > 2f
                if (this.Rigidbody2D.velocity.x != 0 && isGrounded && Mathf.Abs(this.Rigidbody2D.velocity.x) > 2f)
                {
                    this.Rigidbody2D.velocity = new Vector2(0, this.Rigidbody2D.velocity.y);
                }

                return;
            }

            Vector2 targetVelocity = new Vector2(this.baseMovementSpeed * 10f, this.Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            Vector2 smoothedSpeed = Vector2.SmoothDamp(this.Rigidbody2D.velocity, PlayerInput.Instance.Horizontal.Value * targetVelocity, ref moveVelocity, .05f, this.baseMovementSpeed);
            if (smoothedSpeed.x > this.baseMovementSpeed)
            {
                smoothedSpeed.x = this.baseMovementSpeed;
            }
            this.Rigidbody2D.velocity = smoothedSpeed;
        }

        public void VerticalMovement()
        {
            if (jumpRequest)
            {
                this.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
                jumping = true;
            }
            else if (PlayerInput.Instance.Jump.Held && jumping == true)
            {
                if (jumpTimeCounter > 0 && this.Rigidbody2D.velocity.y > 0)
                {
                    this.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else if (jumpTimeCounter <= 0 || this.Rigidbody2D.velocity.y <= 0)
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
            if (Mathf.Round(this.Rigidbody2D.velocity.y) == 0 && this.isGrounded == false)
            {
                this.Rigidbody2D.gravityScale = .5f;
            }
            else if (this.Rigidbody2D.velocity.y < 0)
            {
                this.Rigidbody2D.gravityScale = 3f;
            }


            if (this.isGrounded && this.Rigidbody2D.gravityScale != 1f)
            {
                this.Rigidbody2D.gravityScale = 1f;
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
                animator.SetBool(MeleeAttackParaHash, true);
                // animator.SetTrigger(MeleeAttackParaHash) also worked
            }
        }

        #region Facing
        public new void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(1, 1);
            }
        }
        #endregion
    }
}