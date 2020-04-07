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
        if (animator.gameObject.name == "Shovel")
        {
            animator.SetBool("IsDigging", false);
            animator.SetBool("IsTapping", false);
        }
        if (animator.gameObject.name == "Can")
        {
            animator.SetBool("IsPouring", false);
        }
        if (animator.gameObject.name == "Seed Bag")
        {
            animator.SetBool("IsPlacing", false);
        }
    }
}
