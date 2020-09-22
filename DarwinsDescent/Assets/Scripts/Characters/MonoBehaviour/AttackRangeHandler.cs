using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    public class AttackRangeHandler : MonoBehaviour
    {
        // TODO: QUESTION: The gameobject this is attached to does not have a trigger, and this does not get called when it is hit, but its child object
        // does have a collider which is a trigger. Why does this work?
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Set up an animation parameter which will set ISWithinRange to true, which will set Melee attacking.

        }
    }
}