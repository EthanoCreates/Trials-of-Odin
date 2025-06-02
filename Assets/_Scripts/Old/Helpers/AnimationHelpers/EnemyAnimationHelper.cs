using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationHelper : StateMachineBehaviour
{
    [SerializeField] private bool useAnimFinished;
    [SerializeField] private bool useRootMotion;
    [SerializeField] private bool warpAgentPosition;

    [BoxGroup("Warp Settings")]
    [ShowIf("warpAgentPosition")]
    [SerializeField] private Transform warpBone;

    [BoxGroup("Warp Settings")]
    [ShowIf("warpAgentPosition")]
    [CustomValueDrawer("MyBlendTime")]
    [SerializeField] private float blendTime =.9f;

    [SerializeField] private bool disableSpineIK;

    private NavMeshAgent navMeshAgent;
    private IEnemyAnimatorHelper animatorHelper;
    private bool haswarped;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorHelper = animator.gameObject.GetComponent<IEnemyAnimatorHelper>();

        if (useAnimFinished) animatorHelper.AnimationStarted();
        if (useRootMotion) animatorHelper.EnableRootMotion();
        if (disableSpineIK) animatorHelper.DisableSpineIK();
        if (warpAgentPosition) { navMeshAgent = animator.gameObject.GetComponent<NavMeshAgent>(); navMeshAgent.enabled = false; haswarped = false; }
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (warpAgentPosition && !haswarped)
        {
            // Check if normalized time is past the blend time and no transition is happening
            if (stateInfo.normalizedTime >= blendTime && !animator.IsInTransition(0))
            {
                if (warpBone == null) { Debug.Log("Add warp bone"); return; }

                animatorHelper.DisableRootMotion();
                Vector3 warpBonePosition = warpBone.position;

                navMeshAgent.Warp(warpBonePosition);
                navMeshAgent.nextPosition = warpBonePosition;

                navMeshAgent.enabled = true;
                haswarped = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (useAnimFinished) animatorHelper.AnimationFinished();
        if (useRootMotion) animatorHelper.DisableRootMotion();
        if (disableSpineIK) animatorHelper.EnableSpineIK();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
