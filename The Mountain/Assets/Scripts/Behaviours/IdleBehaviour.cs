using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour {

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!GrapplingHookCharacterController.GrapplingHookMode)
        {
            animator.applyRootMotion = true;
        }
        animator.SetInteger(HashTable.ComboCountParam, 0);//Reset combo count because we fell out of combo 
        animator.SetBool(HashTable.ComboParam, false);//Reset the combo parameter just in case 
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
 //   override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
 //       if((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) > .96f && animator.GetCurrentAnimatorStateInfo(0).IsName("Motion"))
 //       {
 //           //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
 //           if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 6.0f)
 //           {
 //               animator.SetFloat(HashTable.IdleAltParam, Random.value);
 //               if (animator.GetFloat(HashTable.IdleAltParam) == .5f)//Force it to a value 
 //               {
 //                   animator.SetFloat(HashTable.IdleAltParam, 0.6f);
 //               }
 //           }
 //       }
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
 //   {
 //       if (stateInfo.IsName("Idle_Base"))
 //       {
 //           animator.SetFloat(HashTable.IdleAltParam, 0);
 //       }
 //   }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
