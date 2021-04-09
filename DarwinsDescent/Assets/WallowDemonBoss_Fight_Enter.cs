using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallowDemonBoss_Fight_Enter : StateMachineBehaviour
{
    public PlayerCharacter playerCharacter;
    public BossBulbatoeHandler bossBulbatoeHandler;
    public AudioSource battleMuisic;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerCharacter == null)
        {
            playerCharacter = GameObject.Find("Darwin").GetComponent<PlayerCharacter>();
            bossBulbatoeHandler = GameObject.Find("BossBulbatoeSpawnPoints").GetComponent<BossBulbatoeHandler>();
            battleMuisic = GameObject.Find("WallowBoss_Music").GetComponent<AudioSource>();
        }
        if (!battleMuisic.isPlaying)
        {
            battleMuisic.Play();
        }
        playerCharacter.movementDisabled = false;
        bossBulbatoeHandler.Fightstarted = true;
        animator.SetBool("InFight", true);
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
