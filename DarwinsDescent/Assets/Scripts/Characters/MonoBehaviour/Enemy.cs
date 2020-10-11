using DarwinsDescent.Assets.Scripts;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class Enemy : Actor
    {
        public int pipValue = 1;
        public int baseAtkDmg = 1;

        public AIPath aIPath;
        public AIDestinationSetter destinationSetter;
        public GameObject PlayerCharacter;
        public GameObject Target;
        public List<GameObject> WanderPoints = new List<GameObject>();
        public float StartingDetectionRange;
        public float FollowDetectionRange;
        protected GameObject Headpoint;
        public bool LookForHeadPoint = false;
        private System.Random Random = new System.Random();
        public EnemySMF SMF = new EnemySMF();
        // TODO: Maybe move to sub class but then need to make methods virtual and override them,
        public BoxCollider2D meleeAtkBCollider;

        void Awake()
        {
            this.rigidbody2D = GetComponent<Rigidbody2D>();
            if (this.rigidbody2D == null)
                this.rigidbody2D = GetComponentInChildren<Rigidbody2D>();

            this.spriteRenderer = GetComponent<SpriteRenderer>();
            if (this.spriteRenderer == null)
                this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            this.animator = GetComponent<Animator>();
            if (this.animator == null)
                this.animator = GetComponentInChildren<Animator>();

            // This only works if the GFX subcomponent is a child game object of the main. TODO: Think of a better implementation
            this.boxCollider = GetComponent<BoxCollider2D>();
            if(this.boxCollider == null)
                this.boxCollider = GetComponentInChildren<BoxCollider2D>();

            if (this.damageable == null)
                damageable = GetComponent<DamageableEnemy>();

            if (PlayerCharacter == null)
                PlayerCharacter = GameObject.Find("Darwin");

            foreach (Transform child in this.transform.parent.transform)
            {
                if(child.name == "WanderPoints")
                {
                    foreach (Transform grandChild in child)
                    {
                        WanderPoints.Add(grandChild.gameObject);
                    }
                    break;
                }
            }

            aIPath = GetComponent<AIPath>();
            destinationSetter = GetComponent<AIDestinationSetter>();

            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (StartingDetectionRange == 0f)
                StartingDetectionRange = 5f;
            if (FollowDetectionRange == 0f)
                FollowDetectionRange = 10f;
            if (LookForHeadPoint)
                this.transform.Find(Headpoint.ToString());
        }

        void FixedUpdate()
        {
            CheckIsGrounded();

            if (damageable?.health.CurHealth <= 0)
            {
                destinationSetter.target = null;
                return;
            }

            CheckPlayerInRange();
             
            this.animator.SetFloat(SMF.HorizontalSpeedHash, aIPath.desiredVelocity.x);
            UpdateFacing();
        }

        #region Grounded
        public override bool CheckIsGrounded()
        {
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

        public override void UpdateFacing()
        {
            if (aIPath.desiredVelocity.x <= 0)
            {
                spriteRenderer.flipX = !spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(1, 1);
            }
            else
            {
                spriteRenderer.flipX = spriteOriginallyFacesLeft;
                meleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
            }
        }

        public void CheckPlayerInRange()
        {
            float playerDistance = 0f;
            RaycastHit2D raycastHit = new RaycastHit2D();

            if (PlayerCharacter != null)
                playerDistance = Vector3.Distance(PlayerCharacter.transform.position, this.rigidbody2D.position);

            if(playerDistance != 0f)
            {
                float distanceToFollow = StartingDetectionRange;
                if (destinationSetter.target == PlayerCharacter.transform)
                {
                    distanceToFollow = FollowDetectionRange;
                }

                if(playerDistance < distanceToFollow)
                {
                    // TODO: still using ai finder for raycast, figure out why
                    // I think a solution would be to make a child Gameobject and throw all of the AI things in there.
                    Transform lookingPoint = this.rigidbody2D.transform;
                    if (Headpoint != null)
                    {
                        lookingPoint = Headpoint.transform;
                    }

                    Vector3 Direction = (PlayerCharacter.transform.position - lookingPoint.position).normalized;

                    raycastHit = Physics2D.Raycast(lookingPoint.position, Direction, distanceToFollow);
                    Debug.DrawRay(lookingPoint.position, Direction * distanceToFollow, Color.green);

                    if (raycastHit.collider != null)
                    {
                        Target = PlayerCharacter;
                    }
                }
            }

            if (raycastHit.collider == null && WanderPoints.Count != 0)
                Target = WanderPoints[Random.Next(WanderPoints.Count)];

            destinationSetter.target = Target.transform;


        }
    }
}