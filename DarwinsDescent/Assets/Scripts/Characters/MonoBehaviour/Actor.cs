using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    [RequireComponent(typeof(Animator))]
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

        protected Animator animator;
        protected BoxCollider2D boxCollider;
        protected ContactFilter2D contactFilter;
        public SpriteRenderer spriteRenderer;
        public bool spriteOriginallyFacesLeft = false;
        public float groundedRaycastDistanceCheck = .5f;

        Rigidbody2D rigidbody2D;
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } set { this.rigidbody2D = value; } }

        // State Machine Flags
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

        #endregion

        void Awake()
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();

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
            // bool wasGrounded = animator.GetBool(HashGroundedPara);
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedRaycastDistanceCheck, contactFilter.layerMask);
            isGrounded = raycastHit.collider != null;
            animator.SetBool(HashGroundedPara, isGrounded);

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
