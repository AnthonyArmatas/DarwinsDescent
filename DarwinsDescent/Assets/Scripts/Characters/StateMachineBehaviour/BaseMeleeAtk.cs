using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMeleeAtk : StateMachineBehaviour
{
    public AudioSource Attack_Darwin_Sound;
    public AudioSource Attack_Weapon_Sound;
    public PlayerCharacter Darwin;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Darwin = animator.transform.GetComponent<PlayerCharacter>();
        if (Darwin == null)
        {
            Darwin = GameObject.Find("Darwin").GetComponent<PlayerCharacter>();
        }

        Darwin.CurrentAttackSound.Play();
        Darwin.CurrentWeaponSound.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Sets the MeleeAttack Bool to false to let the game know its no longer attacking.
        animator.SetBool(Animator.StringToHash("MeleeAttack"), false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
