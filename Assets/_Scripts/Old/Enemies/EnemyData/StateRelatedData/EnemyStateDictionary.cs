using System.Collections.Generic;

public class EnemyStateDictionary
{
    private readonly EnemyContext enemyContext;
    private readonly EnemyMovementUtility enemyMovementUtility;
    private readonly EnemyAnimatorRequestor enemyAnimatorRequestor;

    public enum GroundedState { GeneralGroundedState, }
    public enum LandState { GeneralLandState, }
    public enum IdleState { GeneralIdleState, }
    public enum PatrolState { GeneralPatrolState, }
    public enum ChaseState { GeneralChaseState, HumanoidBossChaseState, }
    public enum SearchState { GeneralSearchState, }
    public enum AttackState { GeneralAttackState, HumanoidBossAttackState, }
    public enum BlockState { GeneralBlockState, HumanoidBossBlockState, }
    public enum StunnedState { GeneralStunnedState, }
    public enum FallingState { GeneralFallingState, }

    private Dictionary<GroundedState, EnemyState> groundedStates;
    private Dictionary<LandState, EnemyState> landStates;
    private Dictionary<IdleState, EnemyState> idleStates;
    private Dictionary<PatrolState, EnemyState> patrolStates;
    private Dictionary<ChaseState, EnemyState> chaseStates;
    private Dictionary<SearchState, EnemyState> searchStates;
    private Dictionary<AttackState, EnemyState> attackStates;
    private Dictionary<BlockState, EnemyState> blockStates;
    private Dictionary<StunnedState, EnemyState> stunnedStates;
    private Dictionary<FallingState, EnemyState> fallingStates;

    public EnemyStateDictionary(EnemyContext enemyContext, EnemyMovementUtility enemyMovementUtility, EnemyAnimatorRequestor enemyAnimatorRequestor)
    {
        this.enemyContext = enemyContext;
        this.enemyMovementUtility = enemyMovementUtility;
        this.enemyAnimatorRequestor = enemyAnimatorRequestor;

        InitializeDictionaries();
    }

    private void InitializeDictionaries()
    {
        groundedStates = new Dictionary<GroundedState, EnemyState>
        {
            { GroundedState.GeneralGroundedState, new EnemyGroundedState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Grounded) }
        };
        landStates = new Dictionary<LandState, EnemyState>
        {
            { LandState.GeneralLandState, new EnemyLandState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Land) }
        };
        idleStates = new Dictionary<IdleState, EnemyState>
        {
            { IdleState.GeneralIdleState, new EnemyIdleState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Idle) }
        };
        patrolStates = new Dictionary<PatrolState, EnemyState>
        {
            { PatrolState.GeneralPatrolState, new EnemyPatrolState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Patrol) }
        };
        chaseStates = new Dictionary<ChaseState, EnemyState>
        {
            { ChaseState.GeneralChaseState, new EnemyChaseState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Chase) },
            { ChaseState.HumanoidBossChaseState, new HumanoidBossChaseState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Chase) }
        };
        searchStates = new Dictionary<SearchState, EnemyState>
        {
            { SearchState.GeneralSearchState, new EnemySearchState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Search) }
        };
        attackStates = new Dictionary<AttackState, EnemyState>
        {
            { AttackState.GeneralAttackState, new EnemyAttackState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Attack) },
            { AttackState.HumanoidBossAttackState, new HumanoidBossAttackState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Attack) }
        };
        blockStates = new Dictionary<BlockState, EnemyState>
        {
            { BlockState.GeneralBlockState, new EnemyBlockState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Block) },
            { BlockState.HumanoidBossBlockState, new HumanoidBossBlockState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Block)}
        };
        stunnedStates = new Dictionary<StunnedState, EnemyState>
        {
            { StunnedState.GeneralStunnedState, new EnemyStunnedState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Stunned) }
        };
        fallingStates = new Dictionary<FallingState, EnemyState>
        {
            { FallingState.GeneralFallingState, new EnemyFallingState(enemyContext, enemyMovementUtility, enemyAnimatorRequestor, EnemyStateMachine.EEnemyState.Falling) }
        };
    }

    public EnemyState GetGroundedState(GroundedState state) => groundedStates[state];
    public EnemyState GetLandState(LandState state) => landStates[state];
    public EnemyState GetIdleState(IdleState state) => idleStates[state];
    public EnemyState GetPatrolState(PatrolState state) => patrolStates[state];
    public EnemyState GetChaseState(ChaseState state) => chaseStates[state];
    public EnemyState GetSearchState(SearchState state) => searchStates[state];
    public EnemyState GetAttackState(AttackState state) => attackStates[state];
    public EnemyState GetBlockState(BlockState state) => blockStates[state];
    public EnemyState GetStunnedState(StunnedState state) => stunnedStates[state];
    public EnemyState GetFallingState(FallingState state) => fallingStates[state];
}
