using UnityEngine;

public class VariableEnemyAnimationHelper : StateMachineBehaviour
{
    [SerializeField] private bool useTimedAnimFinished;
    [SerializeField] private bool useTimedRootMotion;
    [SerializeField] private bool disableSpineIK;
    [SerializeField] private float animFinishedTime = 1;

    private IEnemyAnimatorHelper animatorHelper;

    float currentTime;
    bool hasFinishedAnim;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorHelper = animator.gameObject.GetComponent<IEnemyAnimatorHelper>();

        hasFinishedAnim = false;

        if (useTimedAnimFinished) animatorHelper.AnimationStarted();
        if (useTimedRootMotion) animatorHelper.EnableRootMotion();
        if (disableSpineIK) animatorHelper.DisableSpineIK();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!hasFinishedAnim)
        {
            currentTime = stateInfo.normalizedTime - Mathf.Floor(stateInfo.normalizedTime);

            if (currentTime >= animFinishedTime)
            {
                if (useTimedAnimFinished) animatorHelper.AnimationFinished();
                if (useTimedRootMotion) animatorHelper.DisableRootMotion();
                if (disableSpineIK) animatorHelper.EnableSpineIK();
                hasFinishedAnim = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

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
