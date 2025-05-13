using System;
using UnityEngine;

public class DummyAnimator : MonoBehaviour, IEnemyAnimatorHelper
{
    [SerializeField] private Animator animator;

    public event EventHandler OnAnimationStarted;
    public event EventHandler OnAnimationFinished;

    private void Awake()
    {
        GetComponent<DummyStateMachine>().OnScriptsInitialized += DummyAnimator_OnScriptsInitialized;
    }

    private void DummyAnimator_OnScriptsInitialized(object sender, System.EventArgs e)
    {
        DummyAnimationRequestor animationRequestor = GetComponent<DummyStateMachine>().GetAnimationRequestor();
        animationRequestor.OnAttack += AnimationRequestor_OnAttack;
        animationRequestor.OnStun += AnimationRequestor_OnStun;
        animationRequestor.OnLoopingStun += AnimationRequestor_OnLoopingStun;
        animationRequestor.OnLoopingStunExit += AnimationRequestor_OnLoopingStunExit;
    }

    private void AnimationRequestor_OnLoopingStunExit(object sender, EventArgs e)
    {
        animator.SetTrigger("ExitStun");
    }

    private void AnimationRequestor_OnLoopingStun(object sender, EventArgs e)
    {
        animator.SetTrigger("LoopStun");
    }

    private void AnimationRequestor_OnStun(object sender, System.EventArgs e)
    {
        animator.SetTrigger("Stunned");
    }

    private void AnimationRequestor_OnAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger("Attack");
    }

    public void AnimationStarted()
    {
        OnAnimationStarted?.Invoke(this, EventArgs.Empty);
    }

    public void AnimationFinished()
    {
        OnAnimationFinished?.Invoke(this, EventArgs.Empty);
    }

    public void EnableRootMotion()
    {
        animator.applyRootMotion = true;
    }

    public void DisableRootMotion()
    {
        animator.applyRootMotion = false;
    }

    public void DisableSpineIK()
    {
        throw new System.NotImplementedException();
    }

    public void EnableSpineIK()
    {
        throw new System.NotImplementedException();
    }
}
