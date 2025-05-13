using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    private int idleBreakChance;
    private bool wantingToIdleBreak;
    private bool isIdleBreak;

    public override void EnterState()
    {
        enemyMovementUtility.Agent.isStopped = true;

        if (enemyContext.LookAround)
        {
            wantingToIdleBreak = true;
            enemyContext.LookAround = false;
        }
        else
        {
            idleBreakChance = Random.Range(0, 3);
            if (idleBreakChance == 0) wantingToIdleBreak = true;
            else wantingToIdleBreak = false;
        }

        isIdleBreak = false;
        enemyAnimatorRequestor.AnimateIdle();
    }

    public override void ExitState()
    {
        enemyMovementUtility.Agent.isStopped = false;
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned;
        if (enemyContext.NearPlayers.Count != 0) { enemyAnimatorRequestor.ReEngage(); return EnemyStateMachine.EEnemyState.Chase; }
        if (enemyContext.AnimFinished)
        {
            if (isIdleBreak) isIdleBreak = false;
            if (wantingToIdleBreak) { enemyAnimatorRequestor.AnimateIdleBreak(); wantingToIdleBreak = false; isIdleBreak = true; }
            else { return EnemyStateMachine.EEnemyState.Patrol; }
        }

        return StateKey;
    }

    public override void UpdateState() { }
}
