using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDecent
{
    public class MidAirSMB : SceneLinkedSMB<PlayerCharacter>
    {
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.UpdateFacing();
            m_MonoBehaviour.CheckForGrounded();
            //if (m_MonoBehaviour.CheckForMeleeAttackInput())
            //    m_MonoBehaviour.MeleeAttack();
        }
    }

}