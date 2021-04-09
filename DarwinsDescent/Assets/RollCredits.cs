using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollCredits : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioSource VictoryMusic = GameObject.Find("WallowBoss_Victory").GetComponent<AudioSource>();
        VictoryMusic.Play();
        AudioSource CreditsMusic = GameObject.Find("Credits_Music").GetComponent<AudioSource>();
        CreditsMusic.PlayDelayed(VictoryMusic.clip.length);        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PipSetter PipSetter = GameObject.Find("GameHandler").GetComponent<PipSetter>();
        PipSetter.CreditsAnimator.SetBool("StartCredits", true);
        Debug.Log("THE DEMO IS OVER");
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
