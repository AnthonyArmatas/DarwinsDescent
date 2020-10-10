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
        // TODO: Maybe move to sub class but then need to make methods virtual and override them,
        public BoxCollider2D meleeAtkBCollider;

        void Awake()
        {
            this.Rigidbody2D = GetComponent<Rigidbody2D>();
            if (this.Rigidbody2D == null)
                this.Rigidbody2D = GetComponentInChildren<Rigidbody2D>();

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

            this.contactFilter = new ContactFilter2D()
            {
                layerMask = groundedLayerMask,
                useLayerMask = true,
                useTriggers = false,
            };

            health = new HealthModel(startingHealth);

            if (baseMovementSpeed == 0)
                baseMovementSpeed = 10f;
            if (baseAttackDamage == 0)
                baseAttackDamage = 1;
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
            if (health.CurHealth <= 0)
            {
                destinationSetter.target = null;
                return;
            }

            CheckPlayerInRange();
             
            this.animator.SetFloat(this.HorizontalSpeedParaHash, aIPath.desiredVelocity.x);
            UpdateFacing(aIPath.desiredVelocity.x <= 0);
        }

        public new void UpdateFacing(bool faceLeft)
        {
            if (faceLeft)
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
                playerDistance = Vector3.Distance(PlayerCharacter.transform.position, this.Rigidbody2D.position);

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
                    Transform lookingPoint = this.Rigidbody2D.transform;
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