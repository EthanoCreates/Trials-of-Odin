using TrialsOfOdin.State;
using UnityEngine;

public class VariablePlayerAnimationHelper : StateMachineBehaviour
{
    [SerializeField] private bool useTimedAnimFinished;
    [SerializeField] private bool useTimedRootMotion;
    [SerializeField] private bool checkForPlayerCombo;
    [SerializeField] private bool disablePlayerMovement;
    [SerializeField] private float animFinishedTime = 1;

    private PlayerStateMachine playerStateMachine;
    private IAnimatorHelper animatorHelper;

    float elapsedTime;
    float animationLength;
    bool hasFinishedAnim;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime = 0;
        hasFinishedAnim = false;
        animationLength = stateInfo.length;

        if (animatorHelper == null) animatorHelper = animator.gameObject.GetComponent<IAnimatorHelper>();

        if (checkForPlayerCombo) playerStateMachine = PlayerStateMachine.LocalInstance;

        if (useTimedAnimFinished) animatorHelper.AnimationStarted();
        if (useTimedRootMotion) animatorHelper.EnableRootMotion();
        if (disablePlayerMovement) animatorHelper.DisablePlayerMovement();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!hasFinishedAnim)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= animFinishedTime * animationLength)
            {
                hasFinishedAnim = true;
                CheckForCombo();
            }
        }
    }

    private void CheckForCombo()
    {
        if (checkForPlayerCombo) 
        {
            if (!playerStateMachine.CombatSystem.CanQueueCombo)
            {
                if (useTimedAnimFinished) animatorHelper.AnimationFinished();
                if (useTimedRootMotion) animatorHelper.DisableRootMotion();
                if (disablePlayerMovement) animatorHelper.EnablePlayerMovement();
            }
        }
        else
        {
            if (useTimedAnimFinished) animatorHelper.AnimationFinished();
            if (useTimedRootMotion) animatorHelper.DisableRootMotion();
            if (disablePlayerMovement) animatorHelper.EnablePlayerMovement();
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //These need to be here, due to when playing same animation state while in same animation state update will
        //seem to run before start is played
        elapsedTime = 0;
        hasFinishedAnim = false;
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
