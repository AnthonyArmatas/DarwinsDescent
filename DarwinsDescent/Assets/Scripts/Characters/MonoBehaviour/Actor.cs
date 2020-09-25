using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class Actor : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion

        #region Vars

        // Character Info
        public int health;
        public string status;
        public float baseMovementSpeed;
        public int baseAttackDamage;
        public bool isGrounded = false;

        // Actor components and default values
        [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;

        
        protected BoxCollider2D boxCollider;
        protected ContactFilter2D contactFilter;
        protected Damageable damageable;
        protected Damager damager;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public bool spriteOriginallyFacesLeft = false;
        public float groundedRaycastDistanceCheck = .5f;

        Rigidbody2D rigidbody2D;
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } set { this.rigidbody2D = value; } }

        // State Machine Flags
        public readonly int HorizontalSpeedParaHash = Animator.StringToHash("HorizontalSpeed");
        public readonly int VerticalSpeedParaHash = Animator.StringToHash("VerticalSpeed");
        public readonly int GroundedParaHash = Animator.StringToHash("Grounded");
        public readonly int CrouchingParaHash = Animator.StringToHash("Crouching");
        public readonly int PushingParaHash = Animator.StringToHash("Pushing");
        public readonly int TimeoutParaHash = Animator.StringToHash("Timeout");
        public readonly int RespawnParaHash = Animator.StringToHash("Respawn");
        public readonly int HurtParaHash = Animator.StringToHash("Hurt");
        public readonly int ForcedRespawnParaHash = Animator.StringToHash("ForcedRespawn");
        public readonly int MeleeAttackParaHash = Animator.StringToHash("MeleeAttack");
        public readonly int DeadParaHash = Animator.StringToHash("Dead");


        #endregion

        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            damageable = GetComponent<Damageable>();
            damager = GetComponent<Damager>();

            contactFilter.layerMask = groundedLayerMask;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = false;
        }

        void FixedUpdate()
        {
            CheckIsGrounded();
        }

        #region Facing
        // Should add to a common function script.
        // The PlayerInput in this script makes it directly tied to the controller input. TODO: replace this or add this to the player character as an override
        public void UpdateFacing()
        {
            bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
            bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

            if (faceLeft)
            {
                Debug.Log("InsideActor");
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
            }
            else if (faceRight)
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
            }
        }

        // A good way of determining this is by passing aIPath.desiredVelocity.x <= 0 
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
        public void CheckIsGrounded()
        {
            // bool wasGrounded = animator.GetBool(GroundedParaHash);
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedRaycastDistanceCheck, contactFilter.layerMask);
            isGrounded = raycastHit.collider != null;
            animator.SetBool(GroundedParaHash, isGrounded);

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
        }
        #endregion

        #region MoveActor
        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            //Vector2 delta = position - currentPosition;
            //previousPosition += delta;
            //currentPosition = position;
            rigidbody2D.MovePosition(position);
        }
        #endregion
    }
}
