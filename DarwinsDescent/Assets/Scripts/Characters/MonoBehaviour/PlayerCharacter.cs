using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


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
        public SpriteRenderer InteractObjRenderer;
        public GameHandler GameHandler;
        public UnityEngine.InputSystem.PlayerInput PlayerInput;

        private float jumpTimeCounter;
        public bool jumpRequest;
        public bool jumping;
        public bool moving;
        public float jumpTime;
        public float jumpForce;
        public bool IsDead;
        public bool movementDisabled;
        #endregion

        #region Events
        // Instantiates the event for others to subscribe to with their functions.
        public delegate void InteractWithObj();
        public InteractWithObj interact;

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
            if (GameHandler == null)
                GameHandler = transform.Find("GameHandler")?.GetComponent<GameHandler>();
            if (cameraFollowTarget == null)
                cameraFollowTarget = transform.Find("CameraFollowTarget")?.GetComponent<Transform>();
            if (PlayerInput == null)
                PlayerInput = new UnityEngine.InputSystem.PlayerInput();
            
            if (InteractObjRenderer == null) 
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "InteractFlag")
                        InteractObjRenderer = child.GetComponent<SpriteRenderer>();
                }

                if (InteractObjRenderer == null)
                    InteractObjRenderer = transform.Find("InteractFlag")?.GetComponent<SpriteRenderer>();
            }


            // Get PipModel for legs and get the associated LegPipUp values
            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (jumpForce == 0)
                jumpForce = 2;
            if (jumpTime == 0)
                jumpTime = 0.05f;

            // Basic all in one shader if added as the material
            // Look to AllIn1SpriteShader.shader file for the names
            // for invincibility
            // FLICKER_ON, percent 0.25 | frequent 1
            //Material mat = GetComponent<Renderer>().material;
            //mat.SetFloat("_Glow", 10f);
            //mat.EnableKeyword("GLOW_ON");

        }

        // Update is called once per frame
        void Update()
        {
            if (IsDead || movementDisabled || GameHandler.GameIsPaused)
                return;
            //IsJumping();
            //IsAttacking();
            //IsInteracting();
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

            // Get input should only be done in Update since fixed update "can and will miss inputs"
            // https://forum.unity.com/threads/check-for-user-input-in-fixedupdate.214706/#:~:text=Tracking%20input%20in%20FixedUpdate%20absolutely,respond%20to%20it%20in%20FixedUpdate.
            // There doesnt seems to be any issues, but if I dump this into update the jump gets weird and flys up.
            //  TODO: Look into this and make sure the  playerinput class and values are only taken from update.
            //  Did a quick check, and it looks like GetInputs is in update in InputComponent. Should look in deeper to make sure.
            MoveAround();
            animator.SetFloat(SMF.HorizontalSpeedHash, this.rigidbody2D.velocity.x);
            animator.SetFloat(SMF.VerticalSpeedHash, this.rigidbody2D.velocity.y);

            // Maybe Add to both Updates
            CheckIsGrounded();
        }

        #region PlayerMovement
        ///// <summary>
        ///// Controls base movement.
        ///// </summary>
        ///// <param name="speedScale"></param>
        public void MoveAround()
        {
            HorizontalMovement();
            VerticalMovement();
        }

        public void IsMoving(InputAction.CallbackContext Value)
        {
            // Updates the movement here on preformed to actually get the direction and on canceled to set it to 0 when it stops.
            // HorizontalMovement handles the actual calculations about where and when the movement is happening, this is just getting the input immediately via events. 
            moveVelocity = Value.ReadValue<Vector2>();
        }

        public void HorizontalMovement()
        {

            //Debug.Log(moveVelocity);
            //Debug.Log(Value.action.phase);
            if (moveVelocity.x == 0 || movementDisabled)
            {
                // Immediately stops the player if he is on the floor and moving faster than small nudges. If more precision is desired remove the Mathf.Abs(this.rigidbody2D.velocity.x) > 2f
                if (this.rigidbody2D.velocity.x != 0 && isGrounded && Mathf.Abs(this.rigidbody2D.velocity.x) > 0f)
                {
                    this.rigidbody2D.velocity = new Vector2(0, this.rigidbody2D.velocity.y);
                }

                return;
            }

            if (moveVelocity.x > 0 &&
                this.rigidbody2D.velocity.x > this.baseMovementSpeed)
            {
                this.rigidbody2D.velocity = new Vector2(this.baseMovementSpeed, this.rigidbody2D.velocity.y);
                return;
            }

            if (moveVelocity.x < 0 &&
                this.rigidbody2D.velocity.x < (this.baseMovementSpeed * -1))
            {
                this.rigidbody2D.velocity = new Vector2((this.baseMovementSpeed * -1), this.rigidbody2D.velocity.y);
                return;
            }

            if (moveVelocity.x > 0)
            {
                this.rigidbody2D.AddForce(new Vector2(this.baseAccelerationSpeed, 0), ForceMode2D.Impulse);
                return;
            }

            if (moveVelocity.x < 0)
            {
                this.rigidbody2D.AddForce(new Vector2(this.baseAccelerationSpeed * -1, 0), ForceMode2D.Impulse);
                return;
            }

            
        }

        public void VerticalMovement()
        {
            if (jumpRequest)
            {
                this.rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
                jumping = true;
            }
            else if (jumping == true)
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
        public void IsJumping(InputAction.CallbackContext Value)
        {
            if (isGrounded && Value.performed)
            {
                jumpRequest = true;
                jumpTimeCounter = jumpTime;
            }
            if (Value.canceled)
            {
                jumping = false;
            }
        }
        #endregion

        public void MeleeAttack(InputAction.CallbackContext Value)
        {
            if (Value.performed)
            {
                // attacking = true;

                // Setting this true here so that the damager can check if the trigger collision happens while MeleeAttack is active, and when the animation state exits, it sets it to false.
                // This allows every frame of the animation to be register the hits. A different method should be used for specific frames of animation that the damage should be dealt (Or different Dmg).
                animator.SetBool(SMF.MeleeAttackHash, true);
                // animator.SetTrigger(MeleeAttackParaHash) also worked
            }
        }

        public void IsInteracting(InputAction.CallbackContext Value)
        {
            if (Value.performed)
            {
                Debug.Log("Before interact");
                if(interact != null)
                    interact.Invoke();

                Debug.Log("after interact");
            }
        }

        #region Facing
        public override void UpdateFacing()
        {
            bool faceLeft = moveVelocity.x < 0f;
            bool faceRight = moveVelocity.x > 0f;

            if (faceLeft)
            {
                spriteRenderer.transform.localScale = new Vector3(-1, 1);
                InteractObjRenderer.flipX = true;
                //spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                //meleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
            else if (faceRight)
            {
                spriteRenderer.transform.localScale = new Vector3(1, 1);
                InteractObjRenderer.flipX = false;
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