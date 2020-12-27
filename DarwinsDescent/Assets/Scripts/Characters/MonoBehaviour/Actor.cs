using DarwinsDescent.Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public abstract class Actor : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion

        #region Vars

        // Character Info
        public Damageable damageable;
        public Animator animator;
        public SpriteRenderer spriteRenderer;
        public Rigidbody2D rigidbody2D;
        public BoxCollider2D boxCollider;

        [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;
        public float baseMovementSpeed;
        public float baseAccelerationSpeed;
        public bool isGrounded = false;
        public float groundedRaycastDistanceCheck = 0.05f;
        public bool spriteOriginallyFacesLeft = false;



        #endregion

        // This Makes sense if we are problematically setting up scenes.
        //public Actor(Damageable dmg, Animator ani, SpriteRenderer sr, Rigidbody2D rb, BoxCollider2D bc, float baseMoveSpeed)
        //{
        //    damageable = dmg;
        //    animator = ani;
        //    spriteRenderer = sr;
        //    rigidbody2D = rb;
        //    boxCollider = bc;
        //    baseMovementSpeed = baseMoveSpeed;
        //}

        public void setDamageable(Damageable dmg) 
        {
            this.damageable = dmg;
        }

        public void setSpriteRenderer(SpriteRenderer sr)
        {
            this.spriteRenderer = sr;
        }

        public void setRigidbody2D(Rigidbody2D rb)
        {
            this.rigidbody2D = rb;
        }

        public void setBoxCollider2D(BoxCollider2D bc)
        {
            this.boxCollider = bc;
        }

        public void setBaseMovementSpeed(float baseMoveSpeed)
        {
            this.baseMovementSpeed = baseMoveSpeed;
        }

        public abstract bool CheckIsGrounded();

        public abstract void UpdateFacing();

        //void Awake()
        //{
        //    rigidbody2D = GetComponent<Rigidbody2D>();
        //    spriteRenderer = GetComponent<SpriteRenderer>();
        //    animator = GetComponent<Animator>();
        //    boxCollider = GetComponent<BoxCollider2D>();
        //    damageable = GetComponent<Damageable>();
        //    damager = GetComponent<Damager>();
        //    health = new HealthModel(startingHealth);

        //    contactFilter.layerMask = groundedLayerMask;
        //    contactFilter.useLayerMask = true;
        //    contactFilter.useTriggers = false;
        //}

        //void FixedUpdate()
        //{
        //    CheckIsGrounded();
        //}

        //#region Facing
        //// Should add to a common function script.
        //// The PlayerInput in this script makes it directly tied to the controller input. TODO: replace this or add this to the player character as an override
        //public void UpdateFacing()
        //{
        //    bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
        //    bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

        //    if (faceLeft)
        //    {
        //        Debug.Log("InsideActor");
        //        spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        //    }
        //    else if (faceRight)
        //    {
        //        spriteRenderer.flipX = spriteOriginallyFacesLeft;
        //    }
        //}

        //// A good way of determining this is by passing aIPath.desiredVelocity.x <= 0 
        //public void UpdateFacing(bool faceLeft)
        //{
        //    if (faceLeft)
        //    {
        //        spriteRenderer.flipX = !spriteOriginallyFacesLeft;
        //    }
        //    else
        //    {
        //        spriteRenderer.flipX = spriteOriginallyFacesLeft;
        //    }
        //}

        //public float GetFacing()
        //{
        //    return spriteRenderer.flipX != spriteOriginallyFacesLeft ? -1f : 1f;
        //}
        //#endregion

        //#region Grounded
        //public void CheckIsGrounded()
        //{
        //    // bool wasGrounded = animator.GetBool(GroundedParaHash);
        //    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedRaycastDistanceCheck, contactFilter.layerMask);
        //    isGrounded = raycastHit.collider != null;
        //    animator.SetBool(GroundedParaHash, isGrounded);

        //    #region Uncomment to see visual Debug
        //    // Uncomment to see visual Debug
        //    //Color rayColor;
        //    //if (raycastHit.collider != null)
        //    //{
        //    //    rayColor = Color.green;
        //    //}
        //    //else
        //    //{
        //    //    rayColor = Color.red;
        //    //}

        //    //Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
        //    //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
        //    //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), Vector2.right * (boxCollider.bounds.extents.y), rayColor);
        //    //Debug.Log(raycastHit.collider);
        //    #endregion
        //}
        //#endregion

        //#region MoveActor
        ///// <summary>
        ///// This moves the character without any implied velocity.
        ///// </summary>
        ///// <param name="position">The new position of the character in global space.</param>
        //public void Teleport(Vector2 position)
        //{
        //    //Vector2 delta = position - currentPosition;
        //    //previousPosition += delta;
        //    //currentPosition = position;
        //    rigidbody2D.MovePosition(position);
        //}
        //#endregion
    }
}
