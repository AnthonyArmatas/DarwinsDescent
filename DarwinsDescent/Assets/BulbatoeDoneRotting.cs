using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbatoeDoneRotting : StateMachineBehaviour
{
    public GameObject JoyDemon = (GameObject)Resources.Load("Prefabs/JoyDemon", typeof(GameObject));

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BossBulbatoes bossBulbatoe = animator.gameObject.transform.GetComponent<BossBulbatoes>();
        if (bossBulbatoe == null)
            throw new MissingReferenceException();

        // If this check is removed the joy demon will spawn if the bulbatoe is killed while rotting. 
        // This would be a way to increase difficulty. 
        if (!animator.GetBool("Destroyed")  && bossBulbatoe.WallowBoss.animator.GetBool("Dead") == false)
        {
            if (JoyDemon != null)
            {
                GameObject.Instantiate(JoyDemon, animator.gameObject.transform);
            }
            
        }
        
        

        bossBulbatoe.BossBulbatoeHandler.ResetBulbatoe(bossBulbatoe);
        animator.SetBool("Killable", false);
        animator.SetBool("Rot", false);
        animator.SetBool("Destroyed", false);
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
