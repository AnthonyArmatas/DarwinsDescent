using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DarwinsDescent
{
    [RequireComponent(typeof(OutDated_CharacterController2D))]
    [RequireComponent(typeof(Animator))]
    public class OutDated_PlayerCharacter : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion

        #region Vars
        //static protected PlayerCharacter s_PlayerInstance;
        //static public PlayerCharacter PlayerInstance { get { return s_PlayerInstance; } }

        public SpriteRenderer spriteRenderer;
        public Damageable damageable;
        public Damager meleeDamager;
        public Transform cameraFollowTarget;

        public float maxSpeed = 10f;

        [Range(k_MinHurtJumpAngle, k_MaxHurtJumpAngle)] public float hurtJumpAngle = 45f;
        public float hurtJumpSpeed = 5f;
        public float flickeringDuration = 0.1f;

        public float jumpForce = 20f;
        public bool jumpRequest;
        public bool jumping;
        private float jumpTimeCounter;
        public float jumpTime;

        public bool attacking;

        public float cameraHorizontalFacingOffset;
        public float cameraHorizontalSpeedOffset;
        public float cameraVerticalInputOffset;
        public float maxHorizontalDeltaDampTime;
        public float maxVerticalDeltaDampTime;
        public float verticalCameraOffsetDelay;

        public bool spriteOriginallyFacesLeft;

        protected OutDated_CharacterController2D characterController2D;
        protected Animator animator;
        protected BoxCollider2D bCollider;
        protected BoxCollider2D MeleeAtkBCollider;
        protected Vector2 moveVector;
        protected Vector2 moveVelocity;
        protected float m_TanHurtJumpAngle;
        protected WaitForSeconds m_FlickeringWait;
        protected Coroutine m_FlickerCoroutine;
        protected TileBase m_CurrentSurface;
        protected float m_CamFollowHorizontalSpeed;
        protected float m_CamFollowVerticalSpeed;
        protected float m_VerticalCameraOffsetTimer;

        protected Vector2 startingPosition = Vector2.zero;
        protected bool startingFacingLeft = false;

        protected bool inPause = false;

        protected readonly int HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");
        protected readonly int HashVerticalSpeedPara = Animator.StringToHash("VerticalSpeed");
        protected readonly int HashGroundedPara = Animator.StringToHash("Grounded");
        protected readonly int HashCrouchingPara = Animator.StringToHash("Crouching");
        protected readonly int HashPushingPara = Animator.StringToHash("Pushing");
        protected readonly int HashTimeoutPara = Animator.StringToHash("Timeout");
        protected readonly int HashRespawnPara = Animator.StringToHash("Respawn");
        protected readonly int HashDeadPara = Animator.StringToHash("Dead");
        protected readonly int HashHurtPara = Animator.StringToHash("Hurt");
        protected readonly int HashForcedRespawnPara = Animator.StringToHash("ForcedRespawn");
        protected readonly int HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");

        //used in non alloc version of physic function
        protected ContactPoint2D[] m_ContactsBuffer = new ContactPoint2D[16];
        #endregion

        #region Constants
        protected const float k_MinHurtJumpAngle = 0.001f;
        protected const float k_MaxHurtJumpAngle = 89.999f;
        protected const float k_GroundedStickingVelocityMultiplier = 3f;    // This is to help the character stick to vertically moving platforms.
        #endregion


        // MonoBehaviour Messages - called by Unity internally.
        void Awake()
        {
            //s_PlayerInstance = this;

            characterController2D = GetComponent<OutDated_CharacterController2D>();
            animator = GetComponent<Animator>();
            bCollider = GetComponent<BoxCollider2D>();
            MeleeAtkBCollider = transform.Find("MeleeHitBox").GetComponent<BoxCollider2D>();
        }


        void Start()
        {
            hurtJumpAngle = Mathf.Clamp(hurtJumpAngle, k_MinHurtJumpAngle, k_MaxHurtJumpAngle);
            m_TanHurtJumpAngle = Mathf.Tan(Mathf.Deg2Rad * hurtJumpAngle);
            m_FlickeringWait = new WaitForSeconds(flickeringDuration);

            if (!Mathf.Approximately(maxHorizontalDeltaDampTime, 0f))
            {
                float maxHorizontalDelta = maxSpeed * cameraHorizontalSpeedOffset + cameraHorizontalFacingOffset;
                m_CamFollowHorizontalSpeed = maxHorizontalDelta / maxHorizontalDeltaDampTime;
            }

            if (!Mathf.Approximately(maxVerticalDeltaDampTime, 0f))
            {
                float maxVerticalDelta = cameraVerticalInputOffset;
                m_CamFollowVerticalSpeed = maxVerticalDelta / maxVerticalDeltaDampTime;
            }

            SceneLinkedSMB<OutDated_PlayerCharacter>.Initialise(animator, this);

            startingPosition = transform.position;
            startingFacingLeft = GetFacing() < 0.0f;
        }

        #region OnTriggerEnter2D
        void OnTriggerEnter2D(Collider2D other)
        {

        }
        #endregion

        #region OnTriggerExit2D
        void OnTriggerExit2D(Collider2D other)
        {

        }
        #endregion

        void Update()
        {
            if (PlayerInput.Instance.Pause.Down)
            {
                if (!inPause)
                {
                    if (ScreenFader.IsFading)
                        return;

                    PlayerInput.Instance.ReleaseControl(false);
                    PlayerInput.Instance.Pause.GainControl();
                    inPause = true;
                    Time.timeScale = 0;
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("UIMenus", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                }
                else
                {
                    Unpause();
                }
            }

            IsJumping();

            IsAttacking();

            // Maybe Add to both Updates
            UpdateFacing();
        }

        void FixedUpdate()
        {
            //characterController2D.Move(moveVector * Time.deltaTime);
            MoveAround();
            moveVector = characterController2D.Rigidbody2D.velocity;
            animator.SetFloat(HashHorizontalSpeedPara, moveVector.x);
            animator.SetFloat(HashVerticalSpeedPara, moveVector.y);

            // Maybe Add to both Updates
            CheckForGrounded();
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
                // Immediately stops the player if he is on the floor and moving faster than small nudges. If more precision is desired remove the Mathf.Abs(characterController2D.Rigidbody2D.velocity.x) > 2f
                if (characterController2D.Rigidbody2D.velocity.x != 0 && characterController2D.isGrounded && Mathf.Abs(characterController2D.Rigidbody2D.velocity.x) > 2f)
                {
                    characterController2D.Rigidbody2D.velocity = new Vector2(0, characterController2D.Rigidbody2D.velocity.y);
                }

                return;
            }
                
            Vector2 targetVelocity = new Vector2(maxSpeed * 10f, characterController2D.Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            Vector2 smoothedSpeed = Vector2.SmoothDamp(characterController2D.Rigidbody2D.velocity, PlayerInput.Instance.Horizontal.Value * targetVelocity, ref moveVelocity, .05f, maxSpeed);
            if (smoothedSpeed.x > maxSpeed)
            {
                smoothedSpeed.x = maxSpeed;
            }
            characterController2D.Rigidbody2D.velocity = smoothedSpeed;
        }

        public void VerticalMovement()
        {
            if (jumpRequest)
            {
                characterController2D.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequest = false;
                jumping = true;
            }
            else if (PlayerInput.Instance.Jump.Held && jumping == true)
            {
                if (jumpTimeCounter > 0 && characterController2D.Rigidbody2D.velocity.y > 0)
                {
                    characterController2D.Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else if (jumpTimeCounter <= 0 || characterController2D.Rigidbody2D.velocity.y <= 0)
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
            if (Mathf.Round(characterController2D.Rigidbody2D.velocity.y) == 0 && characterController2D.IsGrounded == false)
            {
                characterController2D.Rigidbody2D.gravityScale = .5f;
            }
            else if(characterController2D.Rigidbody2D.velocity.y < 0)
            {
                characterController2D.Rigidbody2D.gravityScale = 3f;
            }


            if (characterController2D.IsGrounded && characterController2D.Rigidbody2D.gravityScale != 1f)
            {
                characterController2D.Rigidbody2D.gravityScale = 1f;
            }

        }

        /// <summary>
        /// Jump mechanics
        /// </summary>
        public void IsJumping()
        {
            if(characterController2D.IsGrounded && PlayerInput.Instance.Jump.Down)
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
                attacking = true;

                // Setting this true here so that the damager can check if the trigger collision happens while MeleeAttack is active, and when the animation state exits, it sets it to false.
                // This allows every frame of the animation to be register the hits. A different method should be used for specific frames of animation that the damage should be dealt (Or different Dmg).
                animator.SetBool(HashMeleeAttackPara,true);
                // animator.SetTrigger(HashMeleeAttackPara) also worked
            }
        }

        #region Facing
        // Should add to a common function script.
        public void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                MeleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                MeleeAtkBCollider.transform.localScale = new Vector3(1, 1);
            }
        }

        public void UpdateFacing(bool faceLeft)
        {
            if (faceLeft)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
            }
            else
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
            }
        }

        public float GetFacing()
        {
            return spriteRenderer.flipX != spriteOriginallyFacesLeft ? -1f : 1f;
        }
        #endregion

        #region Grounded
        public bool CheckForGrounded()
        {
            bool wasGrounded = animator.GetBool(HashGroundedPara);
            bool grounded = characterController2D.IsGrounded;

            if (grounded)
            {
                if (!wasGrounded && moveVector.y < -1.0f)
                {//only play the landing sound if falling "fast" enough (avoid small bump playing the landing sound)
                    //landingAudioPlayer.PlayRandomSound(m_CurrentSurface);
                }
            }

            animator.SetBool(HashGroundedPara, grounded);

            return grounded;
        }
        #endregion

        #region HoldOver-PotentiallyUseful-Code
        public void Unpause()
        {
            //if the timescale is already > 0, we 
            if (Time.timeScale > 0)
                return;

            StartCoroutine(UnpauseCoroutine());
        }

        protected IEnumerator UnpauseCoroutine()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("UIMenus");
            PlayerInput.Instance.GainControl();
            //we have to wait for a fixed update so the pause button state change, otherwise we can get in case were the update
            //of this script happen BEFORE the input is updated, leading to setting the game in pause once again
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            inPause = false;
        }

        public void CheckForCrouching()
        {
            animator.SetBool(HashCrouchingPara, PlayerInput.Instance.Vertical.Value < 0f);
        }

        protected IEnumerator Flicker()
        {
            float timer = 0f;

            while (timer < damageable.invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return m_FlickeringWait;
                timer += flickeringDuration;
            }

            spriteRenderer.enabled = true;
        }

        public Vector2 GetHurtDirection()
        {
            Vector2 damageDirection = damageable.GetDamageDirection();

            if (damageDirection.y < 0f)
                return new Vector2(Mathf.Sign(damageDirection.x), 0f);

            float y = Mathf.Abs(damageDirection.x) * m_TanHurtJumpAngle;

            return new Vector2(damageDirection.x, y).normalized;
        }

        public void OnHurt(Damager damager, Damageable damageable)
        {
            ////if the player don't have control, we shouldn't be able to be hurt as this wouldn't be fair
            //if (!PlayerInput.Instance.HaveControl)
            //    return;

            //UpdateFacing(damageable.GetDamageDirection().x > 0f);
            //damageable.EnableInvulnerability();

            //animator.SetTrigger(HashHurtPara);

            ////we only force respawn if health > 0, otherwise both forceRespawn & Death trigger are set in the animator, messing with each other.
            //if (damageable.CurrentHealth > 0 && damager.forceRespawn)
            //    animator.SetTrigger(HashForcedRespawnPara);

            //animator.SetBool(HashGroundedPara, false);

            ////if the health is < 0, mean die callback will take care of respawn
            //if (damager.forceRespawn && damageable.CurrentHealth > 0)
            //{
            //    StartCoroutine(DieRespawnCoroutine(false, true));
            //}
        }

        public void OnDie()
        {
            animator.SetTrigger(HashDeadPara);

            StartCoroutine(DieRespawnCoroutine(true, false));
        }

        IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint)
        {
            PlayerInput.Instance.ReleaseControl(true);
            yield return new WaitForSeconds(1.0f); //wait one second before respawing
            yield return StartCoroutine(ScreenFader.FadeSceneOut(useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
            if (!useCheckPoint)
                yield return new WaitForSeconds(2f);
            //Respawn(resetHealth, useCheckPoint);
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(ScreenFader.FadeSceneIn());
            PlayerInput.Instance.GainControl();
        }

        public void StartFlickering()
        {
            m_FlickerCoroutine = StartCoroutine(Flicker());
        }

        public void StopFlickering()
        {
            StopCoroutine(m_FlickerCoroutine);
            spriteRenderer.enabled = true;
        }

        public void TeleportToColliderBottom()
        {
            Vector2 colliderBottom = characterController2D.Rigidbody2D.position + bCollider.offset + Vector2.down * bCollider.size.y * 0.5f;
            characterController2D.Teleport(colliderBottom);
        }
        #endregion
    }
}