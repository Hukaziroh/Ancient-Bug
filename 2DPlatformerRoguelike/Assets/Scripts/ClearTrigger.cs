using UnityEngine;

public class ClearTrigger : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        animator.ResetTrigger("Attack");
    }
}
