public class EnemyLandState : EnemyState
{
    public EnemyLandState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    public override void EnterState()
    {
        enemyAnimatorRequestor.AnimateLanding();
    }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.IsDamaged) return EnemyStateMachine.EEnemyState.Stunned; 
        if (enemyContext.AnimFinished) return EnemyStateMachine.EEnemyState.Patrol;
        return StateKey;
    }

    public override void UpdateState() { }
}
