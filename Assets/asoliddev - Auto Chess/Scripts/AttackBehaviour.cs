using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is to check when attack animation is finished playing
/// </summary>
public class AttackBehaviour : StateMachineBehaviour
{
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

    /// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("attack anim finished");

        var ca = animator.GetComponentInParent<ChampionAnimation>();
        if (ca != null)
        {
            ca.OnAttackAnimationFinished();
            return;
        }

        // (폴백) 혹시 애니메이션 쪽이 아니라 컨트롤러에서 처리한다면
        var ctrl = animator.GetComponentInParent<ChampionController>();
        if (ctrl != null)
        {
            ctrl.OnAttackAnimationFinished();
            return;
        }

        // 둘 다 없으면 로그만 남기고 종료 (NRE 방지)
        Debug.LogWarning($"[AttackBehaviour] No ChampionAnimation/ChampionController found for {animator.name}");
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
