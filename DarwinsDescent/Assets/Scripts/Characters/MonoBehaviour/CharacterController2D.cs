using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace DarwinsDescent
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;
        [Tooltip("The distance down to check for ground.")]
        public float groundedRaycastDistance = 0.1f;

        Rigidbody2D rigidbody2D;
        BoxCollider2D boxCollider;
        public float groundedRaycastDistanceCheck = .5f;
        Vector2 previousPosition;
        Vector2 currentPosition;
        Vector2 nextMovement;

        ContactFilter2D contactFilter;
        public bool isGrounded = false;
        public bool IsGrounded { get { return isGrounded; } protected set { isGrounded = value; } }
        public bool IsCeilinged { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Rigidbody2D Rigidbody2D { get { return rigidbody2D; } }
        public ContactFilter2D ContactFilter { get { return contactFilter; } }

        // Run when the script becomes active
        void Awake()
        {
            // Retrieves Components from the game object
            rigidbody2D = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            // Sets the bots positions to: TODO
            currentPosition = rigidbody2D.position;
            previousPosition = rigidbody2D.position;

            // Sets the layermask to determine what the character should or should not stand on
            contactFilter.layerMask = groundedLayerMask;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = false;

            // Don't want false positives when checking for the floor
            Physics2D.queriesStartInColliders = false;
        }

        //Updated every second instead of every frame like Update() used for physics and other more precise things.
        void FixedUpdate()
        {
            //previousPosition = rigidbody2D.position;
            //currentPosition = previousPosition + nextMovement;
            //Velocity = (currentPosition - previousPosition) / Time.deltaTime;

            //rigidbody2D.MovePosition(currentPosition);
            //nextMovement = Vector2.zero;

            CheckIsGrounded();
        }

        /// <summary>
        /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
        /// </summary>
        /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            nextMovement += movement;
        }

        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - currentPosition;
            previousPosition += delta;
            currentPosition = position;
            rigidbody2D.MovePosition(position);
        }

        /// <summary>
        /// This updates the state of IsGrounded.  It is called automatically in FixedUpdate but can be called more frequently if higher accuracy is required.
        /// </summary>
        public void CheckIsGrounded()
        {
            RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, groundedRaycastDistanceCheck, contactFilter.layerMask);
            Color rayColor;
            if(raycastHit.collider != null)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            // Uncomment to see visual Debug
            //Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
            //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), rayColor);
            //Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + groundedRaycastDistanceCheck), Vector2.right * (boxCollider.bounds.extents.y), rayColor);
            //Debug.Log(raycastHit.collider);

            IsGrounded = raycastHit.collider != null;
        }
    }
}