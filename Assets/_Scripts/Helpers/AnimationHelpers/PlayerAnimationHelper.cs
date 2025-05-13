using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerAnimationHelper : StateMachineBehaviour
{
    [SerializeField] private bool useAnimFinished;
    [ShowIf("useAnimFinished")]
    [SerializeField] private bool onlyStartAnim;
    [SerializeField] private bool checkForPlayerCombo;
    [SerializeField] private bool useRootMotion;
    [SerializeField] private bool disablePlayerMovement;

    private PlayerStateMachine playerStateMachine;
    private IAnimatorHelper animatorHelper;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorHelper = animator.gameObject.GetComponent<IAnimatorHelper>();

        if(checkForPlayerCombo) playerStateMachine = PlayerStateMachine.LocalInstance;

        if(animatorHelper == null) return;

        if (useAnimFinished) animatorHelper.AnimationStarted();
        if (useRootMotion) animatorHelper.EnableRootMotion();
        if (disablePlayerMovement) animatorHelper.DisablePlayerMovement();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CheckForCombo();
        if (disablePlayerMovement) animatorHelper.EnablePlayerMovement();
    }

    private void CheckForCombo()
    {
        if (checkForPlayerCombo) if (playerStateMachine.CombatManager.IsComboAvailable) return;
        if (useAnimFinished && !onlyStartAnim) animatorHelper.AnimationFinished();
        if (useRootMotion) animatorHelper.DisableRootMotion();
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
