using DarwinsDecent;
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
    protected readonly int HashDeadPara = Animator.StringToHash("Dead");
    protected readonly int HashHurtPara = Animator.StringToHash("Hurt");


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        damageable = GetComponent<Damageable>();
        meleeDamager = GetComponent<Damager>();
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
