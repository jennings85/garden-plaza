using UnityEngine;

public class AnimationTools : StateMachineBehaviour
{
    [SerializeField]


    // OnStateExit is called when a transition ends and the state 
    //machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsSwitchingOut", false);
        animator.SetBool("IsSwitchingIn", false);
    }
}
