public abstract class EnemyState : BaseState<EnemyStateMachine.EEnemyState>
{
    protected EnemyContext enemyContext;
    protected EnemyAnimatorRequestor enemyAnimatorRequestor;
    protected EnemyMovementUtility enemyMovementUtility;

    public EnemyState(EnemyContext context, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor,  EnemyStateMachine.EEnemyState key) : base(key)
    {
        enemyContext = context;
        this.enemyAnimatorRequestor = enemyAnimatorRequestor;
        this.enemyMovementUtility = enemyMovementUtility;  
    }

    public override void UpdateSubState()
    {
        enemyContext.EnemyStateMachine.CurrentState(this);
    }
}
