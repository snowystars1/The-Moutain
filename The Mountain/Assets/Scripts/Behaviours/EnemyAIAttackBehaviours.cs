using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAttackBehaviours : StateMachineBehaviour {

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AI boss;
        try
        {
            boss = GameObject.FindGameObjectWithTag("SlimeBoss").GetComponent<AI>();
        }
        catch (NullReferenceException e)
        {
            return;
        }

        if (animator == boss.sBRightAnim && AI.switchState != 1)
        {
            return;
        }
        if (animator == boss.sBLeftAnim && AI.switchState != 2)
        {
            return;
        }
        if (animator == boss.sBAnim && AI.switchState != 3)
        {
            return;
        }
        AI.randomChoice = -1f;//This is to prevent this loop from running again because in the time it takes to create a random number, this loop will run like 4 times
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
