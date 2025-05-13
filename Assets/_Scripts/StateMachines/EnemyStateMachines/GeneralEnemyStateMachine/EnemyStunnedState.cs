public class EnemyStunnedState : EnemyState
{
    public EnemyStunnedState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor, EnemyStateMachine.EEnemyState key) : base(context, enemyMovementUtility, enemyAnimatorRequestor, key) { }

    public override void EnterState() { }

    public override void ExitState() { }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (enemyContext.AnimFinished)
        {
            if (enemyContext.NearPlayers.Count == 0) { return EnemyStateMachine.EEnemyState.Search; } //leaving stunned true so search knows its coming from stunned
            enemyContext.IsDamaged = false;
            return EnemyStateMachine.EEnemyState.Chase;
        }
        return StateKey;
    }

    public override void UpdateState() { }
}
