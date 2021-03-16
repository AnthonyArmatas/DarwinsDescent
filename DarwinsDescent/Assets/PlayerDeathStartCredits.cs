using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathStartCredits : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Text creditText = GameObject.Find("Credit_Text_Center").GetComponent<Text>();
        List<string> ExtraText = new List<string>()
        {
            string.Empty,
            "\nThat could have gone better.",
            "\nTry Harder.",
            "\nThat could not have been on purpose.",
            "\nThe great parasite feeds on your failure."
        };
        Random random = new Random();

        creditText.text = "YOUR DEAD" + ExtraText[Random.Range(0, ExtraText.Count - 1)];

        Animator creditAnimator = GameObject.Find("Credits").GetComponent<Animator>();
        creditAnimator.SetBool("StartCredits", true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
