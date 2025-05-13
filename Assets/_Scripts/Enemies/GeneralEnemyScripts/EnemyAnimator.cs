using UnityEngine;

public class EnemyAnimator : MonoBehaviour, IEnemyAnimatorHelper
{
    [SerializeField] private Animator animator;
    private EnemyStateMachine EnemyStateMachine;


    private int animIDBlock;
    private int animIDSuccessfulBlock;
    private int animIDSpeed;
    private int animIDImpact;
    private int animIDImpactX;
    private int animIDImpactZ;
    private int animIDIdle;
    private int animIDIdleBreak;
    private int animIDReEngage;
    private int animIDFalling;
    private int animIDLanding;
    private int animIDWakeUp;
    private int animIDCharge;

    void Start()
    {
        EnemyStateMachine = this.gameObject.GetComponent<EnemyStateMachine>();
        EnemyAnimatorRequestor enemyAnimatorRequestor = EnemyStateMachine.EnemyAnimatorRequestor;
        enemyAnimatorRequestor.OnAttack += EnemyContext_OnAttack;
        enemyAnimatorRequestor.OnBlock += EnemyContext_OnBlock;
        enemyAnimatorRequestor.OnImpact += EnemyContext_OnImpact;
        enemyAnimatorRequestor.OnMove += EnemyContext_OnMove;
        enemyAnimatorRequestor.OnExitBlock += EnemyContext_OnExitBlock;
        enemyAnimatorRequestor.OnSpecialAttack += EnemyContext_OnSpecialAttack;
        enemyAnimatorRequestor.OnAttackCombo += EnemyContext_OnAttackCombo;
        enemyAnimatorRequestor.OnIdle += EnemyContext_OnIdle;
        enemyAnimatorRequestor.OnReEngage += EnemyContext_OnReEngage;
        enemyAnimatorRequestor.OnIdleBreak += EnemyContext_OnIdleBreak;
        enemyAnimatorRequestor.OnFalling += EnemyContext_OnFalling;
        enemyAnimatorRequestor.OnLanding += EnemyContext_OnLanding;
        enemyAnimatorRequestor.OnSuccessfulBlock += EnemyContext_OnSuccessfulBlock;
        enemyAnimatorRequestor.OnWakeUp += EnemyAnimatorRequestor_OnWakeUp;
        enemyAnimatorRequestor.OnCharge += EnemyAnimatorRequestor_OnCharge;
        AssignAnimationIDs();
    }

    private void EnemyAnimatorRequestor_OnCharge(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDCharge);
    }

    private void EnemyAnimatorRequestor_OnWakeUp(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDWakeUp);
    }

    private void EnemyContext_OnSuccessfulBlock(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDSuccessfulBlock);
    }

    private void EnemyContext_OnLanding(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDLanding);
    }

    private void EnemyContext_OnFalling(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDFalling);
    }

    private void EnemyContext_OnReEngage(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDReEngage);
    }

    private void EnemyContext_OnSpecialAttack(object sender, EnemyAnimatorRequestor.AttackEventArgs e)
    {
        animator.Play(e.attackType.attackClipType.ToString(), 2);
    }

    private void EnemyContext_OnAttackCombo(object sender, EnemyAnimatorRequestor.AttackEventArgs e)
    {
        animator.Play(e.attackType.attackClipType.ToString(), 2);
    }

    private void EnemyContext_OnAttack(object sender, EnemyAnimatorRequestor.AttackEventArgs e)
    {
        animator.Play(e.attackType.attackClipType.ToString(), 2);
    }

    private void EnemyContext_OnIdleBreak(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDIdleBreak);
    }

    private void EnemyContext_OnIdle(object sender, System.EventArgs e)
    {
        animator.SetTrigger(animIDIdle);
    }


    private void EnemyContext_OnImpact(object sender, EnemyAnimatorRequestor.OnImpactEventArgs e)
    {
        animator.SetFloat(animIDImpactX, e.impactX);
        animator.SetFloat(animIDImpactZ, e.impactZ);
        animator.SetTrigger(animIDImpact);
    }

    private void EnemyContext_OnExitBlock(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDBlock, false);
    }

    private void EnemyContext_OnMove(object sender, EnemyAnimatorRequestor.OnMoveEventArgs e)
    {
        animator.SetFloat(animIDSpeed, e.speed);
    }

    private void EnemyContext_OnBlock(object sender, System.EventArgs e)
    {
        animator.SetBool(animIDBlock, true);
    }

    //string to cached int
    private void AssignAnimationIDs()
    {
        animIDBlock = Animator.StringToHash("Block");
        animIDSpeed = Animator.StringToHash("Speed");
        animIDImpact = Animator.StringToHash("Impact");
        animIDImpactX = Animator.StringToHash("ImpactX");
        animIDImpactZ = Animator.StringToHash("ImpactZ");
        animIDIdle = Animator.StringToHash("Idle");
        animIDIdleBreak = Animator.StringToHash("IdleBreak");
        animIDReEngage = Animator.StringToHash("ReEngage");
        animIDFalling = Animator.StringToHash("Falling");
        animIDLanding = Animator.StringToHash("Landing");
        animIDSuccessfulBlock = Animator.StringToHash("SuccessfulBlock");
        animIDWakeUp = Animator.StringToHash("WakeUp");
        animIDCharge = Animator.StringToHash("Charge");
    }

    public void AnimationStarted()
    {
        EnemyStateMachine.AnimStarted();
    }

    public void AnimationFinished()
    {
        EnemyStateMachine.AnimFinished();
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
        //GetComponent<BossWakeUp>().DeactivateSpineConstraint();
    }

    public void EnableSpineIK()
    {
        //GetComponent<BossWakeUp>().ActivateSpineConstraint();
    }
}
