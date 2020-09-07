using DarwinsDescent;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHandler : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public bool spriteOriginallyFacesLeft;
    public Damageable damageable;
    public Damager meleeDamager;

    public Animator animator;
    public AIPath aIPath;
    protected Rigidbody2D Rigidbody2D;
    protected readonly int HashDeadPara = Animator.StringToHash("Dead");
    protected readonly int HashHurtPara = Animator.StringToHash("Hurt");
    protected readonly int HashHorizontalSpeedPara = Animator.StringToHash("HorizontalSpeed");


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        Rigidbody2D = GetComponentInChildren<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
        meleeDamager = GetComponent<Damager>();
        aIPath = GetComponent<AIPath>();
    }

    void FixedUpdate()
    {
        animator.SetFloat(HashHorizontalSpeedPara, aIPath.desiredVelocity.x);
    }

    public void UpdateFacing()
    {
        bool faceLeft = PlayerInput.Instance.Horizontal.Value < 0f;
        bool faceRight = PlayerInput.Instance.Horizontal.Value > 0f;

        if (faceLeft)
        {
            spriteRenderer.flipX = !spriteOriginallyFacesLeft;
            //MeleeAtkBCollider.transform.localScale = new Vector3(-1, 1);
        }
        else if (faceRight)
        {
            spriteRenderer.flipX = spriteOriginallyFacesLeft;
            //MeleeAtkBCollider.transform.localScale = new Vector3(1, 1);
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

    public void OnHurt(Damager damager, Damageable damageable)
    {

        UpdateFacing(damageable.GetDamageDirection().x > 0f);
        //damageable.EnableInvulnerability();

        animator.SetTrigger(HashHurtPara);
    }

    public void OnDie()
    {
        animator.SetTrigger(HashDeadPara);
    }
}
