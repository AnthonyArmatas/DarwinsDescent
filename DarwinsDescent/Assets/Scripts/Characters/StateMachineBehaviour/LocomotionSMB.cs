using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class LocomotionSMB : SceneLinkedSMB<PlayerCharacter>
    {
        // OnSLStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.TeleportToColliderBottom();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //m_MonoBehaviour.UpdateFacing();
            //m_MonoBehaviour.CheckForCrouching();
            //m_MonoBehaviour.CheckForGrounded();
            //else if (m_MonoBehaviour.CheckForMeleeAttackInput())
            //    //m_MonoBehaviour.MeleeAttack();
        }
    }
}
